using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserData
{
    public string username;
    public int level;
    public bool isCompleted;
    public float difficulty;
    public float previousDifficulty;
    public string playerFeedback;
    public List<string> enemyNames;
    public List<int> enemyQuantities;
    public List<int> enemyDamageValues;
    public List<string> obstacleNames;
    public List<int> obstacleQuantities;
    public List<int> obstacleDamageValues;
    public int fitnessVarsId;
    public float time;
    public int lostLife;
    public string version;
}

public class RoomGeneratedData
{
    public float time;
    public List<string> fitnessVarsNames;
    public List<int> max;
    public List<float> mean;
    public List<float> stdDev;
    public List<int> min;
    public int iterations;
    public int bestValue;
}

[System.Serializable]
public class Data
{
    public int id;
}

[System.Serializable]
public class RoomResponse
{
    public string status;
    public Data data;
}

[RequireComponent(typeof(TimeRecorder))]
[RequireComponent(typeof(PlayerDamageRecorder))]
public class APISender : MonoBehaviour
{
    const string baseUrl = "http://game-runs.glitch.me";
    const string adminKey = "123456";

    GameManager gameManager;
    LevelDataManager levelDataManager;
    InputManager inputManager;
    PlayerLocationManager playerLocationManager;
    RoomConclusionManager roomConclusionManager;
    PlayerController playerController;
    TimeRecorder timeRecorder;
    PlayerDamageRecorder playerDamageRecorder;
    PlayerInfo playerInfo;

    bool closingTheGame = false;

    float time;
    int lostLife;
    float previousDifficulty = 0f;

    [SerializeField] GameObject roomConclusionPanel;
    ToggleGroup toggleGroup;

    public List<Position> CompletedRooms { get; set; }
    public int PreviousLife { get; set; }

    void Awake()
    {
        CompletedRooms = new();
        toggleGroup = roomConclusionPanel.GetComponentInChildren<ToggleGroup>();
        timeRecorder = GetComponent<TimeRecorder>();
        playerDamageRecorder = GetComponent<PlayerDamageRecorder>();
    }

    void Start()
    {
        playerInfo = FindObjectOfType<PlayerInfo>();
        levelDataManager = FindObjectOfType<LevelDataManager>();
        playerLocationManager = FindObjectOfType<PlayerLocationManager>();
        inputManager = FindObjectOfType<InputManager>();
        gameManager = FindObjectOfType<GameManager>();

        roomConclusionManager = FindObjectOfType<RoomConclusionManager>();
        roomConclusionManager.OnRoomDoorsOpened += OnRoomDoorsOpened;

        playerController = FindObjectOfType<PlayerController>();
        PreviousLife = playerController.Life;
        playerController.PassedThroughTheDoorEvent += PlayerPassedThroughTheDoor;
        playerController.OnDied += OnPlayerDied;
    }

    void OnApplicationQuit()
    {
        closingTheGame = true;
    }

    void OnDestroy()
    {
        if (roomConclusionManager != null)
        {
            roomConclusionManager.OnRoomDoorsOpened -= OnRoomDoorsOpened;
        }

        if (playerController != null)
        {
            playerController.PassedThroughTheDoorEvent -= PlayerPassedThroughTheDoor;
            playerController.OnDied -= OnPlayerDied;
        }
    }

    public IEnumerator SendRoomGeneratedPostRequest(Room room, float time,
        List<string> fitnessVarNames,
        FitnessStatistics fitnessVarValues, int iterations, int bestValue)
    {
        RoomGeneratedData roomGeneratedData = new()
        {
            time = time,
            fitnessVarsNames = fitnessVarNames,
            max = fitnessVarValues.max,
            mean = fitnessVarValues.mean,
            stdDev = fitnessVarValues.stdDev,
            min = fitnessVarValues.min,
            iterations = iterations,
            bestValue = bestValue,
        };

        string jsonData = JsonUtility.ToJson(roomGeneratedData);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new($"{baseUrl}/generated-room", "POST")
        {
            uploadHandler = new UploadHandlerRaw(postData),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("admin_key", adminKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Erro na requisição: " + request.error);
        }
        else
        {
            RoomResponse roomResponse = JsonUtility.FromJson<RoomResponse>(request.downloadHandler.text);
            room.Id = roomResponse.data.id;
            //Debug.Log("Resposta: " + request.downloadHandler.text);
        }
    }

    public void SendRoomConclusionData(float time, int lostLife, string toggleName = "", bool isCompleted = true, Action action = null)
    {
        if (!closingTheGame && !SceneChangeManager.Instance.LoadingScene)
        {
            StartCoroutine(SendRoomConclusionPostRequest(time, lostLife, toggleName, isCompleted, action));
        }
    }

    (List<string>, List<int>, List<int>) GetObjectsInfo(List<RoomContents> items)
    {
        Dictionary<string, int> objectNameToQuantity = new();

        List<string> objectNames = new();
        List<int> quantities = new();
        List<int> damage = new();
        items.ForEach(item =>
        {
            string itemName = item.ToString();
            if (objectNameToQuantity.ContainsKey(itemName))
            {
                objectNameToQuantity[itemName]++;
            }
            else
            {
                objectNameToQuantity.Add(itemName, 1);
            }
        });

        var objectPairs = objectNameToQuantity.ToList();
        objectNames = objectPairs.Select(pair => pair.Key).ToList();
        quantities = objectPairs.Select(pair => pair.Value).ToList();

        Dictionary<string, int> damageRecord = playerDamageRecorder.DamageRecord;

        foreach (string objectName in objectNames)
        {
            if (damageRecord.ContainsKey(objectName))
            {
                damage.Add(damageRecord[objectName]);
            }
            else
            {
                damage.Add(0);
            }
        }

        return (objectNames, quantities, damage);
    }

    IEnumerator SendRoomConclusionPostRequest(float time, int lostLife, string toggleName = "", bool isCompleted = true, Action action = null)
    {
        Room room = GameMapSingleton.Instance.RoomPositions[playerLocationManager.PlayerLocation.RoomPosition];

        (List<string> enemyNames, List<int> enemyQuantities, List<int> enemyDamageValues) = GetObjectsInfo(room.Enemies);
        (List<string> obstacleNames, List<int> obstacleQuantities, List<int> obstacleDamageValues) = GetObjectsInfo(room.Obstacles);

        UserData userData = new()
        {
            username = playerInfo.Username,
            level = levelDataManager.IndexCurrentLevel,
            isCompleted = isCompleted,
            difficulty = room.Difficulty,
            previousDifficulty = previousDifficulty,
            playerFeedback = toggleName,
            enemyNames = enemyNames,
            enemyQuantities = enemyQuantities,
            enemyDamageValues = enemyDamageValues,
            obstacleNames = obstacleNames,
            obstacleQuantities = obstacleQuantities,
            obstacleDamageValues = obstacleDamageValues,
            fitnessVarsId = room.Id,
            time = time,
            lostLife = lostLife,
            version = "1.0.0"
        };

        previousDifficulty = room.Difficulty;

        Debug.LogWarning("SendRoomConclusionData (1): " + userData.time);

        string jsonData = JsonUtility.ToJson(userData);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new($"{baseUrl}/run", "POST")
        {
            uploadHandler = new UploadHandlerRaw(postData),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("admin_key", adminKey);

        CleanVariables();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Erro na requisição: " + request.error);
        }
        else
        {
            Debug.Log("Resposta: " + request.downloadHandler.text);
        }

        action?.Invoke();
    }

    bool CountTimeCondition()
    {
        // sala inicial
        if (playerLocationManager.PlayerLocation.RoomPosition.Equals(GameMapSingleton.Instance.InitialRoomPosition))
        {
            return false;
        }

        // se for a sala final
        else if (playerLocationManager.PlayerLocation.RoomPosition.Equals(GameMapSingleton.Instance.FinalRoomPosition))
        {
            return false;
        }
        // se ja passei por essa sala
        else if (CompletedRooms.Contains(playerLocationManager.PlayerLocation.RoomPosition))
        {
            return false;
        }

        return true;
    }

    void PlayerPassedThroughTheDoor(object player, DoorEventArgs doorEventArgs)
    {
        timeRecorder.EnemiesInRoom = CountTimeCondition();
    }

    void OnRoomDoorsOpened()
    {
        if (CountTimeCondition())
        {
            time = timeRecorder.Time;
            lostLife = PreviousLife - playerController.Life;
            CompletedRooms.Add(playerLocationManager.PlayerLocation.RoomPosition);

            playerController.CanTakeDamage = false;
            inputManager.DisableInput();
            roomConclusionPanel.SetActive(true);
        }
    }

    void OnPlayerDied()
    {
        SendRoomConclusionData(timeRecorder.Time, PreviousLife, "", false, gameManager.RestartGame);
    }

    public void SendToggleInfo()
    {
        Debug.LogWarning("SendRoomConclusionData (0): " + time);
        SendRoomConclusionData(time, lostLife, GetActiveToggle(toggleGroup).name);
        roomConclusionPanel.SetActive(false);
        inputManager.EnableInput();
        playerController.CanTakeDamage = true;
    }

    Toggle GetActiveToggle(ToggleGroup group)
    {
        foreach (var toggle in group.ActiveToggles())
        {
            if (toggle.isOn)
            {
                return toggle;
            }
        }
        return null;
    }

    void CleanVariables()
    {
        timeRecorder.EnemiesInRoom = false;
        timeRecorder.Time = 0;
        PreviousLife = playerController.Life;
        playerDamageRecorder.DamageRecord.Clear();
    }
}
