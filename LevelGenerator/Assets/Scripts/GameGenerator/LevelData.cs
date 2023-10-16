using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "ScriptableObject/Level")]
public class LevelData : ScriptableObject
{
    public int roomCount;

    [Header("Enemies")]

    public List<RoomContents> enemies;
    public List<int> enemiesDifficult;

    [Header("Obstacles")]

    public List<RoomContents> obstacles;
    public List<int> obstaclesDifficult;
}