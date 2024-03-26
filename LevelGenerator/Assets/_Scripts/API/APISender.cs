using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class APISender : MonoBehaviour
{
    public class UserData
    {
        public string username;
        public int level;
        public float difficulty;
        public List<string> enemies;
        public List<string> obstacles;
        public float time;
        public string version;
    }

    LevelDataManager levelDataManager;
    PlayerLocationManager playerLocationManager;
    PlayerInfo playerInfo;

    const string baseUrl = "http://game-runs.glitch.me";
    const string adminKey = "123456";

    bool closingTheGame = false;

    void Start()
    {
        levelDataManager = FindObjectOfType<LevelDataManager>();
        playerLocationManager = FindObjectOfType<PlayerLocationManager>();
        playerInfo = FindObjectOfType<PlayerInfo>();
    }

    void OnApplicationQuit()
    {
        closingTheGame = true;
    }

    public void SendRoomConclusionData(float time)
    {
        if (!closingTheGame)
        {
            StartCoroutine(SendPostRequest(time));
        }
    }

    IEnumerator SendPostRequest(float time)
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
            version = "1.0.0"
        };

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
}
