using System.Collections;
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
    LoadingManager loadingManager;

    void Awake()
    {
        levelDataManager = GetComponent<LevelDataManager>();
    }

    void Start()
    {
        levelGenerator = FindObjectOfType<LevelGenerator>();
        playerManager = FindObjectOfType<PlayerManager>();
        playerLocationManager = FindObjectOfType<PlayerLocationManager>();
        loadingManager = FindObjectOfType<LoadingManager>(true);
        StartCoroutine(GenerateGame());
    }

    IEnumerator GenerateGame()
    {
        GameMapSingleton.Instance.ClearMap();

        loadingManager.StartLoading();
        yield return levelGenerator.Generate();
        loadingManager.StopLoading();

        GameMapSingleton.Instance.ConfigureAStar();
        GameMapSingleton.Instance.SetEnemiesTargetPlayer(playerManager.Player.transform, playerLocationManager.PlayerLocation);
    }

    public IEnumerator LoadNextLevel()
    {
        levelDataManager.NextLevel();
        yield return GenerateGame();
    }
}
