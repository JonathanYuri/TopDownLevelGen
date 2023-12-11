using System;
using System.Collections.Generic;
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
        levelGenerator.OnMapCreated += OnMapCreated;

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

        if (levelGenerator != null)
        {
            levelGenerator.OnMapCreated -= OnMapCreated;
        }
    }

    void GenerateGame()
    {
        TryCreatePlayer();
        gameMapManager.SetEnemiesTargetPlayer(player.transform, playerLocationManager.Location);
    }

    void TryCreatePlayer()
    {
        if (player != null)
        {
            return;
        }

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

    void OnMapCreated(object sender, MapCreatedEventArgs e)
    {
        playerLocationManager.SetPlayerToInitialRoom(sceneCamera, e.map.ElementAt(0));
        gameMapManager.ConfigureAStar(e.map);
    }
}
