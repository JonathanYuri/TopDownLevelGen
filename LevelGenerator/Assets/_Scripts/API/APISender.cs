using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APISender : MonoBehaviour
{
    public class UserData
    {
        public string usuario;
        public int nivel;
        public float dificuldade;
        public float tempo;
        public string versao;
    }

    LevelDataManager levelDataManager;
    RoomInfoProvider roomInfoProvider;
    PlayerLocationManager playerLocationManager;
    PlayerInfo playerInfo;

    const string baseUrl = "http://game-runs.glitch.me";
    const string adminKey = "123456";

    bool closingTheGame = false;

    void Start()
    {
        levelDataManager = FindObjectOfType<LevelDataManager>();
        roomInfoProvider = FindObjectOfType<RoomInfoProvider>();
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
        UserData userData = new()
        {
            usuario = playerInfo.Username,
            nivel = levelDataManager.IndexCurrentLevel,
            dificuldade = roomInfoProvider.GetRoomData(playerLocationManager.PlayerLocation.RoomPosition, GameMapSingleton.Instance.RoomPositions).difficulty,
            tempo = time,
            versao = "1.0.0"
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
