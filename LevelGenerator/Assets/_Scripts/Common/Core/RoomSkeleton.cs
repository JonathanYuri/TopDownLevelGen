using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents a room skeleton.
/// </summary>
public class RoomSkeleton
{
    public RoomContents[,] Values { get; }
    public RoomContents[] Enemies { get; }
    public RoomContents[] Obstacles { get; }
    public HashSet<Position> ChangeablePositions { get; }
    public HashSet<Position> DoorPositions { get; }
    public float Difficulty { get; }

    public RoomSkeleton(RoomData roomData)
    {
        Enemies = roomData.enemies;
        Obstacles = roomData.obstacles;
        DoorPositions = roomData.doorPositions;
        ChangeablePositions = GameConstants.ALL_POSITIONS_IN_ROOM;
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
        int nothingRange = GameConstants.NOTHING_RANGE_BEFORE_DOORS;

        // porta pra esquerda ou pra direita
        if (doorPosition.Y == GameConstants.ROOM_MIDDLE.Y)
        {
            if (doorPosition.X == GameConstants.ROOM_WIDTH - 1) // porta na direita da room
            {
                Position initialNothingPosition = new() { X = doorPosition.X - 1, Y = doorPosition.Y };
                Range<int> rangeX = new(-nothingRange + 1, 0);
                Range<int> rangeY = new(-nothingRange + 1, nothingRange - 1);

                PlaceTheImmutableRoomContentInRange(RoomContents.Nothing, initialNothingPosition, rangeX, rangeY, nothingRange);
            }
            else if (doorPosition.X == 0) // porta na esquerda da room
            {
                Position initialNothingPosition = new() { X = doorPosition.X + 1, Y = doorPosition.Y };
                Range<int> rangeX = new(0, nothingRange - 1);
                Range<int> rangeY = new(-nothingRange + 1, nothingRange - 1);

                PlaceTheImmutableRoomContentInRange(RoomContents.Nothing, initialNothingPosition, rangeX, rangeY, nothingRange);
            }
        }

        // porta pra cima ou pra baixo
        else if (doorPosition.X == GameConstants.ROOM_MIDDLE.X)
        {
            if (doorPosition.Y == GameConstants.ROOM_HEIGHT - 1) // porta pra cima na room
            {
                Position initialNothingPosition = new() { X = doorPosition.X, Y = doorPosition.Y - 1 };
                Range<int> rangeX = new(-nothingRange + 1, nothingRange - 1);
                Range<int> rangeY = new(-nothingRange + 1, 0);

                PlaceTheImmutableRoomContentInRange(RoomContents.Nothing, initialNothingPosition, rangeX, rangeY, nothingRange);
            }
            else if (doorPosition.Y == 0) // porta pra baixo na room
            {
                Position initialNothingPosition = new() { X = doorPosition.X, Y = doorPosition.Y + 1 };
                Range<int> rangeX = new(-nothingRange + 1, nothingRange - 1);
                Range<int> rangeY = new(0, nothingRange - 1);

                PlaceTheImmutableRoomContentInRange(RoomContents.Nothing, initialNothingPosition, rangeX, rangeY, nothingRange);
            }
        }
    }

    void PlaceTheImmutableRoomContentInRange(RoomContents roomContent, Position initialPosition, Range<int> rangeX, Range<int> rangeY, int maxDistanceToAccept)
    {
        for (int i = rangeX.Min; i <= rangeX.Max; i++)
        {
            for (int j = rangeY.Min; j <= rangeY.Max; j++)
            {
                Position roomContentPosition = new() { X = initialPosition.X + i, Y = initialPosition.Y + j };
                if (Utils.CalculateDistance(initialPosition, roomContentPosition) < maxDistanceToAccept)
                {
                    PlaceTheImmutableRoomContentInPosition(roomContent, roomContentPosition);
                }
            }
        }
    }

    void PlaceTheImmutableRoomContentInPosition(RoomContents roomContent, Position position)
    {
        Values[position.X, position.Y] = roomContent;
        ChangeablePositions.Remove(position);
    }
}

public struct RoomData
{
    public HashSet<Position> doorPositions;
    public RoomContents[] enemies;
    public RoomContents[] obstacles;
    public float difficulty;

    public RoomData(HashSet<Position> doorPositions, RoomContents[] enemies, RoomContents[] obstacles, float difficulty)
    {
        this.doorPositions = doorPositions;
        this.enemies = enemies;
        this.obstacles = obstacles;
        this.difficulty = difficulty;
    }
}