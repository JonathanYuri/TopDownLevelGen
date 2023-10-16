using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataManager : MonoBehaviour
{
    public LevelData sharedLevelData;

    public List<RoomContents> Enemies { get { return sharedLevelData.enemies; } }
    public List<int> EnemiesValues { get { return sharedLevelData.enemiesDifficult; } }

    public List<RoomContents> Obstacles { get { return sharedLevelData.obstacles; } }
    public List<int> ObstaclesValues { get { return sharedLevelData.obstaclesDifficult; } }
}