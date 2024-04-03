using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages level data and provides access to the current level's information.
/// </summary>
public class LevelDataManager : MonoBehaviour
{
    public List<LevelData> levels;

    public int RoomCount { get { return levels[IndexCurrentLevel].roomCount; } }
    public List<RoomContents> Enemies { get { return levels[IndexCurrentLevel].enemies; } }
    public List<int> EnemiesDifficulty { get { return levels[IndexCurrentLevel].enemiesDifficult; } }
    public int EnemiesCapacity { get { return levels[IndexCurrentLevel].enemiesCapacity; } }
    public List<RoomContents> Obstacles { get { return levels[IndexCurrentLevel].obstacles; } }
    public List<int> ObstaclesDifficulty { get { return levels[IndexCurrentLevel].obstaclesDifficult; } }
    public int ObstaclesCapacity { get { return levels[IndexCurrentLevel].obstaclesCapacity; } }
    public int IndexCurrentLevel { get; private set; }
    public int MaxIndexLevel { get; private set; }

    void Awake()
    {
        MaxIndexLevel = levels.Count - 1;
    }

    /// <summary>
    /// Advances to the next level in the list of available levels, if there is one.
    /// </summary>
    public void NextLevel()
    {
        if (IndexCurrentLevel < MaxIndexLevel)
        {
            IndexCurrentLevel++;
        }
    }
}