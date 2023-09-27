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
    public int Row { get; set; }
    public int Column { get; set; }

    public Position Move(Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Position { Row = Row + 1, Column = Column },
            Direction.Down => new Position { Row = Row - 1, Column = Column },
            Direction.Left => new Position { Row = Row, Column = Column - 1 },
            Direction.Right => new Position { Row = Row, Column = Column + 1 },
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
        return Row == other.Row && Column == other.Column;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }
}

public static class Utils
{
    public static Vector2 TransformAMapPositionIntoAUnityPosition(Position mapPosition)
    {
        return new Vector2(mapPosition.Column * GameConstants.Cols + mapPosition.Column, mapPosition.Row * GameConstants.Rows + mapPosition.Row);
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
        return Math.Abs(position1.Row - position2.Column) + Math.Abs(position1.Row - position2.Column);
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
}
