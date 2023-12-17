using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the game and player interactions.
/// </summary>
[RequireComponent(typeof(LevelDataManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    PlayerController player;
    PlayerLocationManager playerLocationManager;

    Camera sceneCamera;

    UIMapGenerator uiMapGenerator;
    LevelGenerator levelGenerator;

    LevelDataManager levelDataManager;

    void Start()
    {
        sceneCamera = FindObjectOfType<Camera>();
        uiMapGenerator = FindObjectOfType<UIMapGenerator>();
        levelGenerator = FindObjectOfType<LevelGenerator>();
        playerLocationManager = FindObjectOfType<PlayerLocationManager>();
        levelDataManager = GetComponent<LevelDataManager>();
        GenerateGame();
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.PassedThroughTheDoorEvent -= Player_PassedThroughTheDoor;
            player.OnLevelComplete -= Player_OnLevelComplete;
        }
    }

    void GenerateGame()
    {
        GameMapSingleton.Instance.ClearMap();
        GameMapSingleton.Instance.RoomPositions = levelGenerator.Generate();

        if (player == null)
        {
            CreatePlayer();
        }
        playerLocationManager.SetPlayerToInitialRoom(sceneCamera, GameMapSingleton.Instance.RoomPositions.ElementAt(0));

        uiMapGenerator.CreateUIMap(GameMapSingleton.Instance.RoomPositions, playerLocationManager.Location);

        GameMapSingleton.Instance.ConfigureAStar();
        GameMapSingleton.Instance.SetEnemiesTargetPlayer(player.transform, playerLocationManager.Location);
    }

    void CreatePlayer()
    {
        SpawnPlayer();
        playerLocationManager.Player = player;

        player.PassedThroughTheDoorEvent += Player_PassedThroughTheDoor;
        player.OnLevelComplete += Player_OnLevelComplete;
    }

    public void SpawnPlayer()
    {
        GameObject playerObject = Instantiate(playerPrefab);
        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        player = playerController;
    }

    void Player_OnLevelComplete()
    {
        levelDataManager.NextLevel();
        GenerateGame();
    }

    void Player_PassedThroughTheDoor(object player, DoorEventArgs doorEventArgs)
    {
        Position playerOldPosition = playerLocationManager.Location.RoomPosition;
        playerLocationManager.TranslatePlayerToDirectionOfRoom(doorEventArgs.doorDirection, sceneCamera);
        uiMapGenerator.UpdateUIMap(playerOldPosition, playerLocationManager.Location.RoomPosition);
    }
}
