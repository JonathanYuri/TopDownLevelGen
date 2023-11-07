using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a ScriptableObject that stores level data.
/// </summary>
[CreateAssetMenu(fileName = "NewLevel", menuName = "ScriptableObject/Level")]
public class LevelData : ScriptableObject
{
    public int roomCount;

    [Header("Enemies")]

    public List<RoomContents> enemies;
    public List<int> enemiesDifficult;
    [Range(0, 50)] public int enemiesCapacity = 30;

    [Header("Obstacles")]

    public List<RoomContents> obstacles;
    public List<int> obstaclesDifficult;
    [Range(0, 50)] public int obstaclesCapacity = 30;
}