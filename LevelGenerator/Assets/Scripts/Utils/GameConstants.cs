using System.Collections.Generic;

public static class GameConstants
{
    public static float ProbabilityOfGeneratingRoomInNeighborhood = 0.5f;

    public static int NumberOfRooms = 10;
    public static int Height = 9;
    public static int Width = 15;
    public static int EnemiesCapacity = 30;
    public static int ObstaclesCapacity = 30;

    public static Position RoomMiddle = new () { X = (int)(Width / 2), Y = (int)(Height / 2) };

    public static Dictionary<Direction, Position> NeighborDirectionToDoorPosition;

    public static void InitializeDictionary()
    {
        NeighborDirectionToDoorPosition = new()
        {
            { Direction.Up, new Position { X = GameConstants.RoomMiddle.X, Y = GameConstants.Height - 1 } },
            { Direction.Down, new Position { X = GameConstants.RoomMiddle.X, Y = 0 } },
            { Direction.Left, new Position { X = 0, Y = GameConstants.RoomMiddle.Y } },
            { Direction.Right, new Position { X = GameConstants.Width - 1, Y = GameConstants.RoomMiddle.Y } }
        };
    }
}
