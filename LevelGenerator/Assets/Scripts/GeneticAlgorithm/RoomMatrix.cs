using System.Collections;
using System.Collections.Generic;

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

    public void PutEnemyInPosition(RoomContents enemy, Position position)
    {
        Values[position.X, position.Y] = enemy;
        EnemiesPositions.Add(position);
    }

    public void PutObstacleInPosition(RoomContents obstacle, Position position)
    {
        Values[position.X, position.Y] = obstacle;
        ObstaclesPositions.Add(position);
    }

    public void RemoveFromPosition(HashSet<Position> positions, Position position)
    {
        positions.Remove(position);
    }
}
