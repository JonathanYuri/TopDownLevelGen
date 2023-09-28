public struct GameConstants
{
    public static int NumberOfRooms = 10;
    public static int Height = 9;
    public static int Width = 15;
    public static int EnemiesCapacity = 30;
    public static int ObstaclesCapacity = 30;

    public static Position RoomMiddle = new () { X = (int)(Width / 2), Y = (int)(Height / 2) };
}
