using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages level data and provides access to the current level's information.
/// </summary>
public class LevelDataManager : MonoBehaviour
{
    int indexCurrentLevel = 0;

    public List<LevelData> levels;

    public int RoomCount { get { return levels[indexCurrentLevel].roomCount; } }
    public RoomContents[] Enemies { get; private set; }
    public RoomContents[] Obstacles { get; private set; }

    /// <summary>
    /// Advances to the next level in the list of available levels, if there is one.
    /// </summary>
    public void NextLevel()
    {
        if (indexCurrentLevel < levels.Count - 1)
        {
            indexCurrentLevel++;
        }
    }

    public void ResolveKnapsackForCurrentLevel()
    {
        LevelData currentLevelData = levels[indexCurrentLevel];
        Enemies = Knapsack.ResolveKnapsack(currentLevelData.enemies, currentLevelData.enemiesDifficult, currentLevelData.enemiesCapacity);
        Obstacles = Knapsack.ResolveKnapsack(currentLevelData.obstacles, currentLevelData.obstaclesDifficult, currentLevelData.obstaclesCapacity);
    }
}