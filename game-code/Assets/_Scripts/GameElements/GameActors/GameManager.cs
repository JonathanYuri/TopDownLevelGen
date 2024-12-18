using System.Collections;
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
    LoadingManager loadingManager;
    APISender apiSender;

    public LevelDataManager LevelDataManager { get; set; }

    void Awake()
    {
        LevelDataManager = GetComponent<LevelDataManager>();
    }

    void Start()
    {
        apiSender = FindObjectOfType<APISender>();
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
        playerManager.Player.Heal(playerManager.Player.MaxLife - playerManager.Player.Life);

        apiSender.CompletedRooms.Clear();
        apiSender.PreviousLife = playerManager.Player.Life;

        LevelDataManager.NextLevel();
        yield return GenerateGame();
        playerManager.Player.CanTakeDamage = true;
    }

    public void RestartGame()
    {
        SceneChangeManager.Instance.LoadScene(SceneNamesConstants.MENU);
    }
}
