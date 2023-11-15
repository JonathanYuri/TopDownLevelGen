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

    void GenerateGame()
    {
        levelGenerator.Generate();

        if (player == null)
        {
            PlayerLocation.Instance.SpawnPlayer(playerPrefab);
            player = PlayerLocation.Instance.Player;

            player.PassedThroughTheDoorEvent += Player_PassedThroughTheDoor;
            player.OnLevelComplete += Player_OnLevelComplete;
        }

        PlayerLocation.Instance.SetPlayerToInitialRoom(sceneCamera);
        uiMapGenerator.CreateUIMap();
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.PassedThroughTheDoorEvent -= Player_PassedThroughTheDoor;
            player.OnLevelComplete -= Player_OnLevelComplete;
        }
    }

    void Player_OnLevelComplete()
    {
        level++;
        levelDataManager.NextLevel();
        GenerateGame();
    }

    void Player_PassedThroughTheDoor(object player, DoorEventArgs doorEventArgs)
    {
        Position playerOldPosition = PlayerLocation.Instance.AtRoom;
        PlayerLocation.Instance.TranslatePlayerToDirectionOfRoom(doorEventArgs.doorDirection, sceneCamera);

        uiMapGenerator.UpdateUIMap(playerOldPosition);
    }
}
