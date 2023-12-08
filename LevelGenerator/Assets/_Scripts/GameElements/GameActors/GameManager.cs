using UnityEngine;

/// <summary>
/// Manages the game and player interactions.
/// </summary>
[RequireComponent(typeof(LevelDataManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    PlayerController player;

    Camera sceneCamera;

    UIMapGenerator uiMapGenerator;
    LevelGenerator levelGenerator;

    int level = 0;
    LevelDataManager levelDataManager;

    void Start()
    {
        sceneCamera = FindFirstObjectByType<Camera>();
        uiMapGenerator = FindFirstObjectByType<UIMapGenerator>();
        levelGenerator = FindFirstObjectByType<LevelGenerator>();
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
        PlayerLocation.Instance.SetPlayerToInitialRoom(sceneCamera);

        uiMapGenerator.CreateUIMap();

        GameMapManager.Instance.ConfigureAStar();
        GameMapManager.Instance.SetEnemiesTargetPlayer(player.transform);
    }

    void CreatePlayer()
    {
        PlayerLocation.Instance.SpawnPlayer(playerPrefab);
        player = PlayerLocation.Instance.Player;

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
        Position playerOldPosition = PlayerLocation.Instance.Location.RoomPosition;
        PlayerLocation.Instance.TranslatePlayerToDirectionOfRoom(doorEventArgs.doorDirection, sceneCamera);

        uiMapGenerator.UpdateUIMap(playerOldPosition);
    }
}
