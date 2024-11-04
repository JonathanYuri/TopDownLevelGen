using System.Collections.Generic;

public class SharedRoomData
{
    public RoomContents[,] Values { get; }
    public RoomContents[] Enemies { get; }
    public RoomContents[] Obstacles { get; }
    public HashSet<Position> ChangeablePositions { get; }
    public HashSet<Position> DoorPositions { get; }
    public float Difficulty { get; }

    public SharedRoomData(RoomSkeleton roomSkeleton)
    {
        Values = roomSkeleton.Values;
        Enemies = roomSkeleton.Enemies;
        Obstacles = roomSkeleton.Obstacles;
        ChangeablePositions = roomSkeleton.ChangeablePositions;
        DoorPositions = roomSkeleton.DoorPositions;
        Difficulty = roomSkeleton.Difficulty;
    }
}