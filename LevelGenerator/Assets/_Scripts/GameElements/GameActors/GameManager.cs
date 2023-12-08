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
    GameMapManager gameMapManager;

    Camera sceneCamera;

    UIMapGenerator uiMapGenerator;
    LevelGenerator levelGenerator;

    int level = 0;
    LevelDataManager levelDataManager;

    void Start()
    {
        sceneCamera = FindObjectOfType<Camera>();
        uiMapGenerator = FindObjectOfType<UIMapGenerator>();
        levelGenerator = FindObjectOfType<LevelGenerator>();
        playerLocationManager = FindObjectOfType<PlayerLocationManager>();
        gameMapManager = FindObjectOfType<GameMapManager>();
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
        gameMapManager.RoomPositions.Clear();
        gameMapManager.RoomPositions = levelGenerator.Generate();

        if (player == null)
        {
            CreatePlayer();
        }
        playerLocationManager.SetPlayerToInitialRoom(sceneCamera, gameMapManager.RoomPositions.ElementAt(0));

        uiMapGenerator.CreateUIMap(gameMapManager.RoomPositions, playerLocationManager.Location);

        gameMapManager.ConfigureAStar();
        gameMapManager.SetEnemiesTargetPlayer(player.transform, playerLocationManager.Location);
    }

    void CreatePlayer()
    {
        playerLocationManager.SpawnPlayer(playerPrefab);
        player = playerLocationManager.Player;

        player.PassedThroughTheDoorEvent += Player_PassedThroughTheDoor;
        player.OnLevelComplete += Player_OnLevelComplete;
    }

    void Player_OnLevelComplete()
    {
        level++;
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
