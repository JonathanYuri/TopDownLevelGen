using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents a room skeleton.
/// </summary>
public class RoomSkeleton
{
    RoomContents[,] values;

    public RoomContents[,] Values { get => values; set => values = value; }
    public RoomContents[] Enemies { get; }
    public RoomContents[] Obstacles { get; }
    public HashSet<Position> ChangeablesPositions { get; }
    public Position[] DoorPositions { get; }
    public float Difficulty { get; }

    public RoomSkeleton(RoomData roomData)
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
        int nothingRange = GameConstants.NOTHING_RANGE_BEFORE_DOORS;

        // porta pra esquerda ou pra direita
        if (doorPosition.Y == GameConstants.ROOM_MIDDLE.Y)
        {
            if (doorPosition.X == GameConstants.ROOM_WIDTH - 1) // porta na direita da room
            {
                Position initialNothingPosition = new() { X = doorPosition.X - 1, Y = doorPosition.Y };
                Range rangeX = new() { max = 0, min = -nothingRange + 1 };
                Range rangeY = new() { max = nothingRange - 1, min = -nothingRange + 1 };

                PlaceTheImmutableRoomContentInRange(RoomContents.Nothing, initialNothingPosition, rangeX, rangeY, nothingRange);
            }
            else if (doorPosition.X == 0) // porta na esquerda da room
            {
                Position initialNothingPosition = new() { X = doorPosition.X + 1, Y = doorPosition.Y };
                Range rangeX = new() { max = nothingRange - 1, min = 0 };
                Range rangeY = new() { max = nothingRange - 1, min = -nothingRange + 1 };

                PlaceTheImmutableRoomContentInRange(RoomContents.Nothing, initialNothingPosition, rangeX, rangeY, nothingRange);
            }
        }

        // porta pra cima ou pra baixo
        else if (doorPosition.X == GameConstants.ROOM_MIDDLE.X)
        {
            if (doorPosition.Y == GameConstants.ROOM_HEIGHT - 1) // porta pra cima na room
            {
                Position initialNothingPosition = new() { X = doorPosition.X, Y = doorPosition.Y - 1 };
                Range rangeX = new() { max = nothingRange - 1, min = -nothingRange + 1 };
                Range rangeY = new() { max = 0, min = -nothingRange + 1 };

                PlaceTheImmutableRoomContentInRange(RoomContents.Nothing, initialNothingPosition, rangeX, rangeY, nothingRange);
            }
            else if (doorPosition.Y == 0) // porta pra baixo na room
            {
                Position initialNothingPosition = new() { X = doorPosition.X, Y = doorPosition.Y + 1 };
                Range rangeX = new() { max = nothingRange - 1, min = -nothingRange + 1 };
                Range rangeY = new() { max = nothingRange - 1, min = 0 };

                PlaceTheImmutableRoomContentInRange(RoomContents.Nothing, initialNothingPosition, rangeX, rangeY, nothingRange);
            }
        }
    }

    void PlaceTheImmutableRoomContentInRange(RoomContents roomContent, Position initialPosition, Range rangeX, Range rangeY, int maxDistanceToAccept)
    {
        for (int i = rangeX.min; i <= rangeX.max; i++)
        {
            for (int j = rangeY.min; j <= rangeY.max; j++)
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
        ChangeablesPositions.Remove(position);
    }
}

public struct RoomData
{
    public Position[] doorPositions;
    public RoomContents[] enemies;
    public RoomContents[] obstacles;
    public float difficulty;

    public RoomData(Position[] doorPositions, RoomContents[] enemies, RoomContents[] obstacles, float difficulty)
    {
        this.doorPositions = doorPositions;
        this.enemies = enemies;
        this.obstacles = obstacles;
        this.difficulty = difficulty;
    }
}