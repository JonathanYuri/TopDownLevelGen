using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the game
/// </summary>
[RequireComponent(typeof(LevelDataManager))]
public class GameManager : MonoBehaviour
{
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
        levelGenerator = FindObjectOfType<LevelGenerator>();
        playerManager = FindObjectOfType<PlayerManager>();
        playerLocationManager = FindObjectOfType<PlayerLocationManager>();
        GenerateGame();
    }

    void GenerateGame()
    {
        GameMapSingleton.Instance.ClearMap();
        levelGenerator.Generate();

        GameMapSingleton.Instance.ConfigureAStar();
        GameMapSingleton.Instance.SetEnemiesTargetPlayer(playerManager.Player.transform, playerLocationManager.PlayerLocation);
    }

    public void LoadNextLevel()
    {
        levelDataManager.NextLevel();
        GenerateGame();
    }
}
