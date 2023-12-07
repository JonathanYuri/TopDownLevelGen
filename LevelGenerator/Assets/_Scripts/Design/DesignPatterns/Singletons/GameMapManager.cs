using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapManager : SingletonMonoBehaviour<GameMapManager>
{
    public HashSet<Position> RoomPositions { get; set; } = new();

    public void ConfigureAStar()
    {
        
    }

    public void SetEnemiesTargetPlayer(Transform player)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            enemy.SetTargetPlayer(player);
        }
    }
}
