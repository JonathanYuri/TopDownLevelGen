using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomConclusionManager : MonoBehaviour
{
    LevelGenerator levelGenerator;

    void Start()
    {
        levelGenerator = FindObjectOfType<LevelGenerator>();
        levelGenerator.OnLevelGenerated += OnLevelGenerated;
    }

    void OnDestroy()
    {
        if (levelGenerator != null)
        {
            levelGenerator.OnLevelGenerated -= OnLevelGenerated;
        }

        UnsubscribeEnemyEvents();
    }

    void OnLevelGenerated()
    {
        OpenInitialRoomDoors();

        foreach (var enemies in GameMapSingleton.Instance.EachRoomEnemies.Values)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.OnDefeated += OnEnemyDefeated;
            }
        }
    }

    void UnsubscribeEnemyEvents()
    {
        if (GameMapSingleton.Instance == null
            || GameMapSingleton.Instance.EachRoomEnemies == null
            || GameMapSingleton.Instance.EachRoomEnemies.Values == null)
        {
            return;
        }

        foreach (var enemies in GameMapSingleton.Instance.EachRoomEnemies.Values)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.OnDefeated -= OnEnemyDefeated;
                }
            }
        }
    }

    void OpenInitialRoomDoors()
    {
        OpenAllDoorsOfRoom(GameMapSingleton.Instance.RoomPositions.ElementAt(0));
    }

    void OnEnemyDefeated(object sender, EnemyDefeatedEventArgs e)
    {
        e.enemy.OnDefeated -= OnEnemyDefeated;

        GameMapSingleton.Instance.EachRoomEnemies[e.roomPosition].Remove(e.enemy);
        if (GameMapSingleton.Instance.EachRoomEnemies[e.roomPosition].Count == 0)
        {
            OpenAllDoorsOfRoom(e.roomPosition);
        }
    }

    void OpenAllDoorsOfRoom(Position roomPosition)
    {
        foreach (Door door in GameMapSingleton.Instance.EachRoomDoors[roomPosition])
        {
            if (door != null)
            {
                door.Open();
            }
        }
    }
}
