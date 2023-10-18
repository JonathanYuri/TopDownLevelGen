using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A static class that provides game-related constants and configurations.
/// </summary>
public static class GameConstants
{
    public static float PROBABILITY_OF_GENERATING_ROOM_IN_NEIGHBORHOOD = 0.5f;
    public static int ROOM_HEIGHT = 9;
    public static int ROOM_WIDTH = 15;

    public static Position ROOM_MIDDLE = new () { X = (int)(ROOM_WIDTH / 2), Y = (int)(ROOM_HEIGHT / 2) };

    public static readonly Dictionary<Direction, Position> NEIGHBOR_DIRECTION_TO_DOOR_POSITION = new()
    {
        { Direction.Up, new Position { X = GameConstants.ROOM_MIDDLE.X, Y = GameConstants.ROOM_HEIGHT - 1 } },
        { Direction.Down, new Position { X = GameConstants.ROOM_MIDDLE.X, Y = 0 } },
        { Direction.Left, new Position { X = 0, Y = GameConstants.ROOM_MIDDLE.Y } },
        { Direction.Right, new Position { X = GameConstants.ROOM_WIDTH - 1, Y = GameConstants.ROOM_MIDDLE.Y } }
    };

    public static readonly HashSet<Position> ALL_POSITIONS_IN_ROOM = new(
        Enumerable.Range(0, ROOM_WIDTH)
        .SelectMany(x => Enumerable.Range(0, ROOM_HEIGHT), (x, y) => new Position { X = x, Y = y })
    );
}