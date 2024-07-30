using System;
using System.Linq;
using UnityEngine;

public class RoomConclusionManager : MonoBehaviour
{
    LevelGenerator levelGenerator;

    public event Action OnRoomDoorsOpened;

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
        OpenAllDoorsOfRoom(GameMapSingleton.Instance.RoomPositions.Keys.ElementAt(0));
    }

    void OnEnemyDefeated(object sender, EnemyDefeatedEventArgs e)
    {
        e.enemy.OnDefeated -= OnEnemyDefeated;

        if (GameMapSingleton.Instance.EachRoomEnemies.TryGetValue(e.roomPosition, out var enemies))
        {
            enemies.Remove(e.enemy);

            if (enemies.Count == 0)
            {
                OpenAllDoorsOfRoom(e.roomPosition);
            }
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

        OnRoomDoorsOpened?.Invoke();
    }
}
