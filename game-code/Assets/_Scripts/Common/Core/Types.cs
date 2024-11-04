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
    readonly static HashSet<RoomContents> enemies = new()
    {
        RoomContents.Enemy1,
        RoomContents.Enemy2,
        RoomContents.Enemy3,
    };

    readonly static HashSet<RoomContents> obstacles = new()
    {
        RoomContents.Obstacle1,
        RoomContents.Obstacle2,
        RoomContents.Obstacle3,
    };

    public static bool IsEnemy(RoomContents content) => enemies.Contains(content);
    public static bool IsObstacle(RoomContents content) => obstacles.Contains(content);
}