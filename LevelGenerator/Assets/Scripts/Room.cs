using System;
using System.Collections.Generic;

public enum RoomContents
{
    [Mutavel(true)][Ultrapassavel(true)] Ground, // TODO: colocar tudo em ingles

    [Mutavel(true)][Ultrapassavel(false)] Obstacle1,
    [Mutavel(true)][Ultrapassavel(false)] Obstacle2,
    [Mutavel(true)][Ultrapassavel(false)] Obstacle3,

    [Mutavel(true)][Ultrapassavel(true)] Enemy1,
    [Mutavel(true)][Ultrapassavel(true)] Enemy2,
    [Mutavel(true)][Ultrapassavel(true)] Enemy3,

    [Mutavel(false)][Ultrapassavel(true)] Nothing,
    [Mutavel(false)][Ultrapassavel(true)] Door,
    [Mutavel(false)][Ultrapassavel(false)] Wall,
}

[AttributeUsage(AttributeTargets.Field)]
public class MutavelAttribute : Attribute
{
    public bool IsMutavel { get; }
    public MutavelAttribute(bool isMutavel) => IsMutavel = isMutavel;
}

[AttributeUsage(AttributeTargets.Field)]
public class UltrapassavelAttribute : Attribute
{
    public bool IsUltrapassavel { get; }
    public UltrapassavelAttribute(bool isUltrapassavel) => IsUltrapassavel = isUltrapassavel;
}

public class Room
{
    RoomContents[,] values;

    public readonly Position[] doorsPositions;
    HashSet<Position> changeablesPositions;

    public RoomContents[] Enemies { get; }
    public RoomContents[] Obstacles { get; }

    public RoomContents[,] Values { get => values; set => values = value; }
    public HashSet<Position> ChangeablesPositions { get => changeablesPositions; set => changeablesPositions = value; }

    public Room(Position[] doorsPositions, RoomContents[] enemies, RoomContents[] obstacles)
    {
        this.doorsPositions = doorsPositions;
        ChangeablesPositions = GameConstants.ALL_POSITIONS_IN_ROOM;

        Enemies = enemies;
        Obstacles = obstacles;

        Values = new RoomContents[GameConstants.ROOM_WIDTH, GameConstants.ROOM_HEIGHT];

        PutTheWalls();
        PutTheDoors();
    }

    public void PutTheWalls()
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

    public void PutTheDoors()
    {
        foreach (Position position in doorsPositions)
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
