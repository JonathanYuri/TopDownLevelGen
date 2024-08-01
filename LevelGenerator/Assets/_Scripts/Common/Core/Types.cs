using System;
using System.Collections.Generic;
/// <summary>
/// Enumeration that defines possible room contents.
/// </summary>
public enum RoomContents
{
    Ground,
    Obstacle1,
    Obstacle2,
    Obstacle3,
    Enemy1,
    Enemy2,
    Enemy3,
    Nothing,
    Door,
    Wall,
    LevelEnd,
}

public static class RoomContentsInfo
{
    public static readonly Dictionary<RoomContents, bool> isMutable = new()
    {
        { RoomContents.Ground, true },
        { RoomContents.Obstacle1, true },
        { RoomContents.Obstacle2, true },
        { RoomContents.Obstacle3, true },
        { RoomContents.Enemy1, true },
        { RoomContents.Enemy2, true },
        { RoomContents.Enemy3, true },

        { RoomContents.Nothing, false },
        { RoomContents.Door, false },
        { RoomContents.Wall, false },
        { RoomContents.LevelEnd, false },
    };

    public static readonly Dictionary<RoomContents, bool> isTraversable = new()
    {
        { RoomContents.Ground, true },
        { RoomContents.Obstacle1, false },
        { RoomContents.Obstacle2, false },
        { RoomContents.Obstacle3, false },
        { RoomContents.Enemy1, true },
        { RoomContents.Enemy2, true },
        { RoomContents.Enemy3, true },

        { RoomContents.Nothing, true },
        { RoomContents.Door, true },
        { RoomContents.Wall, false },
        { RoomContents.LevelEnd, true },
    };

    public static bool IsMutable(RoomContents roomContents) => isMutable[roomContents];
    public static bool IsTransversable(RoomContents roomContents) => isTraversable[roomContents];
}