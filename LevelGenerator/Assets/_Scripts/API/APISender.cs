using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(TimeRecorder))]
public class APISender : MonoBehaviour
{
    const string baseUrl = "http://game-runs.glitch.me";
    const string adminKey = "123456";

    public class UserData
    {
        public string username;
        public int level;
        public float difficulty;
        public List<string> enemies;
        public List<string> obstacles;
        public float time;
        public int lostLife;
        public string version;
    }

    LevelDataManager levelDataManager;
    PlayerLocationManager playerLocationManager;
    RoomConclusionManager roomConclusionManager;
    PlayerController playerController;
    TimeRecorder timeRecorder;
    PlayerInfo playerInfo;

    int previousLife;
    bool closingTheGame = false;

    public List<Position> CompletedRooms { get; set; }

    void Awake()
    {
        CompletedRooms = new();
        timeRecorder = GetComponent<TimeRecorder>();
    }

    void Start()
    {
        playerInfo = FindObjectOfType<PlayerInfo>();
        levelDataManager = FindObjectOfType<LevelDataManager>();
        playerLocationManager = FindObjectOfType<PlayerLocationManager>();

        roomConclusionManager = FindObjectOfType<RoomConclusionManager>();
        roomConclusionManager.OnRoomDoorsOpened += OnRoomDoorsOpened;

        playerController = FindObjectOfType<PlayerController>();
        previousLife = playerController.Life;
        playerController.PassedThroughTheDoorEvent += PlayerPassedThroughTheDoor;
    }

    void OnApplicationQuit()
    {
        closingTheGame = true;
    }

    void OnDestroy()
    {
        closingTheGame = true;
        if (roomConclusionManager != null)
        {
            roomConclusionManager.OnRoomDoorsOpened -= OnRoomDoorsOpened;
        }

        if (playerController != null)
        {
            playerController.PassedThroughTheDoorEvent -= PlayerPassedThroughTheDoor;
        }
    }

    public void SendRoomConclusionData(float time, int lostLife)
    {
        if (!closingTheGame)
        {
            StartCoroutine(SendPostRequest(time, lostLife));
        }
    }

    IEnumerator SendPostRequest(float time, int lostLife)
    {
        Room room = GameMapSingleton.Instance.RoomPositions[playerLocationManager.PlayerLocation.RoomPosition];

        List<string> enemiesName = new();
        List<string> obstaclesName = new();

        if (room.Enemies != null && room.Enemies.Count != 0)
        {
            enemiesName = room.Enemies.Select(enemy => enemy.ToString()).ToList();
        }

        if (room.Obstacles != null && room.Obstacles.Count != 0)
        {
            obstaclesName = room.Obstacles.Select(obstacle => obstacle.ToString()).ToList();
        }

        UserData userData = new()
        {
            username = playerInfo.Username,
            level = levelDataManager.IndexCurrentLevel,
            difficulty = room.Difficulty,
            enemies = enemiesName,
            obstacles = obstaclesName,
            time = time,
            lostLife = lostLife,
            version = "1.0.0"
        };

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

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Erro na requisição: " + request.error);
        }
        else
        {
            Debug.Log("Resposta: " + request.downloadHandler.text);
        }
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
            Debug.LogWarning("SendRoomConclusionData (0): " + timeRecorder.Time);
            CompletedRooms.Add(playerLocationManager.PlayerLocation.RoomPosition);
            SendRoomConclusionData(timeRecorder.Time, previousLife - playerController.Life);

            timeRecorder.EnemiesInRoom = false;
            timeRecorder.Time = 0;

            previousLife = playerController.Life;
        }
    }
}
