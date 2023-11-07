using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a direction in 2D space.
/// </summary>
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

/// <summary>
/// Represents a 2D position with X and Y coordinates.
/// </summary>
public class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    /// <summary>
    /// Moves the position in a specified direction.
    /// </summary>
    /// <param name="direction">The direction in which to move the position.</param>
    /// <returns>A new position after moving in the specified direction.</returns>
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

/// <summary>
/// A utility class containing various helper methods and extensions.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Transforms a map position into a Unity position for spawning rooms.
    /// </summary>
    /// <param name="mapPosition">The position in the map coordinate system.</param>
    /// <returns>A Unity Vector2 position for spawning rooms.</returns>
    public static Vector2 TransformAMapPositionIntoAUnityPosition(Position mapPosition)
    {
        return new Vector2(mapPosition.X * GameConstants.ROOM_WIDTH + mapPosition.X, mapPosition.Y * GameConstants.ROOM_HEIGHT + mapPosition.Y);
    }

    /// <summary>
    /// Normalizes a value within a given range to a 0-100 scale.
    /// </summary>
    /// <param name="value">The value to normalize.</param>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    /// <returns>The normalized value in the 0-100 scale.</returns>
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

public static class MapUtility
{
    /// <summary>
    /// Retrieves an array of door positions based on neighboring room directions from the specified room position.
    /// </summary>
    /// <param name="roomPosition">The position of the room for which to find door positions.</param>
    /// <returns>An array of positions representing door locations.</returns>
    public static Position[] GetDoorPositionsFromRoomPosition(HashSet<Position> roomPositionsInMap, Position roomPosition)
    {
        List<Position> doorsPositions = new();
        foreach (Direction direction in Enum.GetValues(typeof(Direction)).Cast<Direction>())
        {
            Position adjacentPosition = roomPosition.Move(direction);
            if (roomPositionsInMap.Contains(adjacentPosition))
            {
                doorsPositions.Add(GameConstants.NEIGHBOR_DIRECTION_TO_DOOR_POSITION[direction]);
            }
        }
        return doorsPositions.ToArray();
    }
}