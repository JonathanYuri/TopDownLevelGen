using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a matrix that defines the contents of a room, such as enemies and obstacles.
/// </summary>
public class RoomMatrix
{
    RoomContents[,] values;
    HashSet<Position> enemiesPositions;
    HashSet<Position> obstaclesPositions;

    public RoomContents[,] Values { get => values; set => values = value; }
    public HashSet<Position> EnemiesPositions { get => enemiesPositions; set => enemiesPositions = value; }
    public HashSet<Position> ObstaclesPositions { get => obstaclesPositions; set => obstaclesPositions = value; }

    public RoomMatrix(RoomContents[,] values)
    {
        this.values = values;
        enemiesPositions = new();
        obstaclesPositions = new();
    }

    /// <summary>
    /// Puts the specified content in the given position of the room.
    /// </summary>
    /// <param name="content">The content to place in the room.</param>
    /// <param name="positions">A set of positions to track content placement.</param>
    /// <param name="position">The position where the content should be placed.</param>
    public void PutContentInPosition(RoomContents content, HashSet<Position> positions, Position position)
    {
        Values[position.X, position.Y] = content;
        positions.Add(position);
    }


    void RemoveFromPosition(HashSet<Position> positions, Position position)
    {
        positions.Remove(position);
    }

    /// <summary>
    /// Updates the content in the specified position by changing it to a new content.
    /// </summary>
    /// <param name="position">The position of the content to be updated.</param>
    /// <param name="toChange">The content to be replaced.</param>
    /// <param name="changeTo">The content to replace it with.</param>
    public void UpdateContentInPosition(Position position, RoomContents toChange, RoomContents changeTo)
    {
        HashSet<Position> addContentInPositions = new();
        if (Utils.IsAEnemy(changeTo))
        {
            addContentInPositions = EnemiesPositions;
        }
        else if (Utils.IsAObstacle(changeTo))
        {
            addContentInPositions = ObstaclesPositions;
        }

        if (Utils.IsAEnemy(toChange))
        {
            RemoveFromPosition(EnemiesPositions, position);
        }
        else if (Utils.IsAObstacle(toChange))
        {
            RemoveFromPosition(ObstaclesPositions, position);
        }
        PutContentInPosition(changeTo, addContentInPositions, position);
    }
}
