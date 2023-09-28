using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class Range
{
    public int min;
    public int max;
}

// TODO: fazer essa classe ser uma extensao de vector2, como tem no extensions, talvez n pq no vector2 pode ser float
public class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public Position Move(Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Position { X = X, Y = Y + 1 },
            Direction.Down => new Position { X = X, Y = Y - 1 },
            Direction.Left => new Position { X = X - 1, Y = Y },
            Direction.Right => new Position { X = X + 1, Y = Y },
            Direction _ => throw new ArgumentException("Direção desconhecida: " + direction)
        };
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Position other = (Position)obj;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}

public static class Utils
{
    #region KnapsackProblem
    public static RoomContents[] ResolveKnapsackEnemies(int capacityEnemies)
    {
        Dictionary<RoomContents, int> enemiesDifficult = new()
        {
            { RoomContents.Enemy1, 1 },
            { RoomContents.Enemy2, 2 },
            { RoomContents.Enemy3, 3 }
        };

        List<int> valuesEnemies = new(enemiesDifficult.Values);
        List<RoomContents> keysEnemies = new(enemiesDifficult.Keys);

        List<int> chosenEnemiesIdx = Utils.ResolveKnapsack(valuesEnemies, capacityEnemies);

        RoomContents[] chosenEnemies = new RoomContents[chosenEnemiesIdx.Count];
        for (int i = 0; i < chosenEnemiesIdx.Count; i++)
        {
            int idx = chosenEnemiesIdx[i];
            chosenEnemies[i] = keysEnemies[idx];
        }

        //Debug.Log("Inimigos escolhidos: " + string.Join(", ", chosenEnemies));
        return chosenEnemies;
    }

    public static RoomContents[] ResolveKnapsackObstacles(int capacityObstacles)
    {
        Dictionary<RoomContents, int> obstaclesDifficult = new()
        {
            { RoomContents.Obstacle1, 1 },
            { RoomContents.Obstacle2, 2 },
            { RoomContents.Obstacle3, 3 }
        };

        List<int> valuesObstacles = new(obstaclesDifficult.Values);
        List<RoomContents> keysObstacles = new(obstaclesDifficult.Keys);

        List<int> chosenObstaclesIdx = Utils.ResolveKnapsack(valuesObstacles, capacityObstacles);

        RoomContents[] chosenObstacles = new RoomContents[chosenObstaclesIdx.Count];
        for (int i = 0; i < chosenObstaclesIdx.Count; i++)
        {
            int idx = chosenObstaclesIdx[i];
            chosenObstacles[i] = keysObstacles[idx];
        }

        //Debug.Log("Obstaculos escolhidos: " + string.Join(", ", chosenObstacles));
        return chosenObstacles;
    }

    public static List<int> ResolveKnapsack(List<int> values, int capacity)
    {
        int n = values.Count;
        int[] dp = new int[capacity + 1];
        List<int>[] chosenItems = new List<int>[capacity + 1];
        for (int i = 0; i <= capacity; i++)
        {
            chosenItems[i] = new List<int>();
        }

        for (int w = 1; w <= capacity; w++)
        {
            int maxValue = dp[w];
            List<int> chosenItem = new();

            for (int i = 0; i < n; i++)
            {
                if (values[i] <= w)
                {
                    int newValue = dp[w - values[i]] + values[i];
                    // se o valor for melhor ou se o valor for igual e tiver menos itens, eu coloco
                    if (newValue > maxValue || (newValue == maxValue && chosenItems[w - values[i]].Count + 1 < chosenItem.Count))
                    {
                        maxValue = newValue;
                        chosenItem = new List<int>(chosenItems[w - values[i]]) { i };
                    }
                }
            }

            dp[w] = maxValue;
            chosenItems[w] = chosenItem;
        }

        return chosenItems[capacity];
    }
    #endregion

    // pra spawnar as salas
    public static Vector2 TransformAMapPositionIntoAUnityPosition(Position mapPosition)
    {
        return new Vector2(mapPosition.X * GameConstants.Width + mapPosition.X, mapPosition.Y * GameConstants.Height + mapPosition.Y);
    }

    public static double MinMaxNormalization(double value, double min, double max)
    {
        if (max == min)
        {
            return 0.0;
        }
        else
        {
            return (value - min) / (max - min) * 100.0f;
        }
    }

    public static HashSet<Position> CombinePositions(List<Position> positions1, List<Position> positions2)
    {
        HashSet<Position> combinedPositions = new(positions1);
        combinedPositions.UnionWith(positions2);
        return combinedPositions;
    }

    public static int ManhattanDistance(Position position1, Position position2)
    {
        return Math.Abs(position1.X - position2.Y) + Math.Abs(position1.X - position2.Y);
    }
}
