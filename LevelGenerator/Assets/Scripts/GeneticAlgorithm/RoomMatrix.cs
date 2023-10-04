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

    public void PutContentInPosition(RoomContents content, HashSet<Position> positions, Position position)
    {
        Values[position.X, position.Y] = content;
        positions.Add(position);
    }

    void RemoveFromPosition(HashSet<Position> positions, Position position)
    {
        positions.Remove(position);
    }

    public void UpdateContentInPosition(Position position, RoomContents changeTo)
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

        if (Utils.IsAEnemy(Values[position.X, position.Y]))
        {
            RemoveFromPosition(EnemiesPositions, position);
            PutContentInPosition(changeTo, addContentInPositions, position);
        }
        else if (Utils.IsAObstacle(Values[position.X, position.Y]))
        {
            RemoveFromPosition(ObstaclesPositions, position);
            PutContentInPosition(changeTo, addContentInPositions, position);
        }
    }
}
