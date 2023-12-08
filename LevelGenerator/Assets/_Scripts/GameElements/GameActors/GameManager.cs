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

    int level = 0;
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
        levelGenerator.Generate();

        if (player == null)
        {
            CreatePlayer();
        }
        playerLocationManager.SetPlayerToInitialRoom(sceneCamera);

        uiMapGenerator.CreateUIMap(playerLocationManager.Location);

        GameMapManager.Instance.ConfigureAStar();
        GameMapManager.Instance.SetEnemiesTargetPlayer(player.transform, playerLocationManager.Location);
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
