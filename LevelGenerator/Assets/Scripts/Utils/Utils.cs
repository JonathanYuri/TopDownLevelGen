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
    // pra spawnar as rooms
    public static Vector2 TransformAMapPositionIntoAUnityPosition(Position mapPosition)
    {
        return new Vector2(mapPosition.X * GameConstants.ROOM_WIDTH + mapPosition.X, mapPosition.Y * GameConstants.ROOM_HEIGHT + mapPosition.Y);
    }

    public static double Normalization(double value, double min, double max)
    {
        if (max == min)
        {
            return 0.0;
        }
        else
        {
            return ((value - min) / (max - min)) * 100.0f;
        }
    }

    public static HashSet<Position> CombinePositions(List<Position> positions1, List<Position> positions2)
    {
        HashSet<Position> combinedPositions = new(positions1);
        combinedPositions.UnionWith(positions2);
        return combinedPositions;
    }

    public static bool IsAObstacle(RoomContents content) => GeneticAlgorithmConstants.ROOM.Obstacles.Contains(content);

    public static bool IsAEnemy(RoomContents content) => GeneticAlgorithmConstants.ROOM.Enemies.Contains(content);

    public static int CalculateDistance(Position position1, Position position2) => ManhattanDistance(position1, position2);

    static int ManhattanDistance(Position position1, Position position2)
    {
        return Math.Abs(position1.X - position2.X) + Math.Abs(position1.Y - position2.Y);
    }
}
