using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumeration that defines possible room contents.
/// </summary>
public enum RoomContents
{
    [Mutable(true)][Traversable(true)] Ground, // TODO: colocar tudo em ingles

    [Mutable(true)][Traversable(false)] Obstacle1,
    [Mutable(true)][Traversable(false)] Obstacle2,
    [Mutable(true)][Traversable(false)] Obstacle3,

    [Mutable(true)][Traversable(true)] Enemy1,
    [Mutable(true)][Traversable(true)] Enemy2,
    [Mutable(true)][Traversable(true)] Enemy3,

    [Mutable(false)][Traversable(true)] Nothing,
    [Mutable(false)][Traversable(true)] Door,
    [Mutable(false)][Traversable(false)] Wall,
    [Mutable(false)][Traversable(true)] LevelEnd,
}

/// <summary>
/// Attribute that defines whether content is mutable.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class MutableAttribute : Attribute
{
    public bool IsMutable { get; }
    public MutableAttribute(bool isMutable) => IsMutable = isMutable;
}

/// <summary>
/// Attribute that defines whether content is traversable.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class TraversableAttribute : Attribute
{
    public bool IsTraversable { get; }
    public TraversableAttribute(bool isTraversable) => IsTraversable = isTraversable;
}

/// <summary>
/// Class that represents a room.
/// </summary>
public class Room
{
    RoomContents[,] values;

    public RoomContents[,] Values { get => values; set => values = value; }
    public RoomContents[] Enemies { get; }
    public RoomContents[] Obstacles { get; }
    public HashSet<Position> ChangeablesPositions { get; }
    public Position[] DoorPositions { get; }
    public float Difficulty { get; }

    public Room(RoomData roomData)
    {
        Enemies = roomData.enemies;
        Obstacles = roomData.obstacles;
        DoorPositions = roomData.doorPositions;
        ChangeablesPositions = GameConstants.ALL_POSITIONS_IN_ROOM;
        Difficulty = Mathf.Clamp(roomData.difficulty, 0f, 1f);

        Values = new RoomContents[GameConstants.ROOM_WIDTH, GameConstants.ROOM_HEIGHT];

        PutTheWalls();
        PutTheDoors();
    }

    void PutTheWalls()
    {
        for (int j = 0; j < GameConstants.ROOM_HEIGHT; j++)
        {
            PlaceTheImmutableRoomContentInPosition(RoomContents.Wall, new Position() { X = GameConstants.ROOM_WIDTH - 1, Y = j });
            PlaceTheImmutableRoomContentInPosition(RoomContents.Wall, new Position() { X = 0, Y = j });
        }
        for (int i = 0; i < GameConstants.ROOM_WIDTH; i++)
        {
            PlaceTheImmutableRoomContentInPosition(RoomContents.Wall, new Position() { X = i, Y = GameConstants.ROOM_HEIGHT - 1 });
            PlaceTheImmutableRoomContentInPosition(RoomContents.Wall, new Position() { X = i, Y = 0 });
        }
    }

    void PutTheDoors()
    {
        foreach (Position position in DoorPositions)
        {
            PlaceTheImmutableRoomContentInPosition(RoomContents.Door, position);

            // nao pode ter nada na frente da porta
            PutTheNothingsBeforeTheDoors(position);
        }
    }

    void PutTheNothingsBeforeTheDoors(Position doorPosition)
    {
        // porta pra esquerda ou pra direita
        if (doorPosition.Y == GameConstants.ROOM_MIDDLE.Y)
        {
            if (doorPosition.X == GameConstants.ROOM_WIDTH - 1) // porta na direita da room
            {
                PlaceTheImmutableRoomContentInPosition(RoomContents.Nothing, new Position() { X = doorPosition.X - 1, Y = doorPosition.Y });
            }
            else if (doorPosition.X == 0) // porta na esquerda da room
            {
                PlaceTheImmutableRoomContentInPosition(RoomContents.Nothing, new Position() { X = doorPosition.X + 1, Y = doorPosition.Y });
            }
        }

        // porta pra cima ou pra baixo
        else if (doorPosition.X == GameConstants.ROOM_MIDDLE.X)
        {
            if (doorPosition.Y == GameConstants.ROOM_HEIGHT - 1) // porta pra cima na room
            {
                PlaceTheImmutableRoomContentInPosition(RoomContents.Nothing, new Position() { X = doorPosition.X, Y = doorPosition.Y - 1 });
            }
            else if (doorPosition.Y == 0) // porta pra baixo na room
            {
                PlaceTheImmutableRoomContentInPosition(RoomContents.Nothing, new Position() { X = doorPosition.X, Y = doorPosition.Y + 1 });
            }
        }
    }

    void PlaceTheImmutableRoomContentInPosition(RoomContents roomContent, Position position)
    {
        Values[position.X, position.Y] = roomContent;
        ChangeablesPositions.Remove(position);
    }
}
