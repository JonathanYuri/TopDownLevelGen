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

    public int RoomCount { get { return levels[indexCurrentLevel].roomCount; } }

    public List<RoomContents> Enemies { get { return levels[indexCurrentLevel].enemies; } }
    public List<int> EnemiesValues { get { return levels[indexCurrentLevel].enemiesDifficult; } }
    public int EnemiesCapacity { get { return levels[indexCurrentLevel].enemiesCapacity; } }

    public List<RoomContents> Obstacles { get { return levels[indexCurrentLevel].obstacles; } }
    public List<int> ObstaclesValues { get { return levels[indexCurrentLevel].obstaclesDifficult; } }
    public int ObstaclesCapacity { get { return levels[indexCurrentLevel].obstaclesCapacity; } }
}