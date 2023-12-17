using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the game
/// </summary>
[RequireComponent(typeof(LevelDataManager))]
public class GameManager : MonoBehaviour
{
    UIMapGenerator uiMapGenerator;
    LevelGenerator levelGenerator;
    PlayerManager playerManager;
    PlayerLocationManager playerLocationManager;
    LevelDataManager levelDataManager;

    void Awake()
    {
        levelDataManager = GetComponent<LevelDataManager>();
    }

    void Start()
    {
        uiMapGenerator = FindObjectOfType<UIMapGenerator>();
        levelGenerator = FindObjectOfType<LevelGenerator>();
        playerManager = FindObjectOfType<PlayerManager>();
        playerLocationManager = FindObjectOfType<PlayerLocationManager>();
        GenerateGame();
    }

    void GenerateGame()
    {
        GameMapSingleton.Instance.ClearMap();
        levelGenerator.Generate();

        playerLocationManager.SetPlayerToInitialRoom(GameMapSingleton.Instance.RoomPositions.ElementAt(0));
        uiMapGenerator.CreateUIMap();

        GameMapSingleton.Instance.ConfigureAStar();
        GameMapSingleton.Instance.SetEnemiesTargetPlayer(playerManager.Player.transform, playerLocationManager.Location);
    }

    public void LoadNextLevel()
    {
        levelDataManager.NextLevel();
        GenerateGame();
    }
}
