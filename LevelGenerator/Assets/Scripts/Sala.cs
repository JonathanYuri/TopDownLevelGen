using System;
using System.Collections.Generic;

public enum Enemies
{
    Enemy1,
    Enemy2,
    Enemy3,
}

public enum Obstacles
{
    Obstacle1,
    Obstacle2,
    Obstacle3
}

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

public class Sala
{
    RoomContents[,] values;

    public readonly Position[] doorsPositions;
    public List<Position> changeablesPositions;

    public int Rows { get; }
    public int Cols { get; }

    public RoomContents[] Enemies { get; }
    public RoomContents[] Obstacles { get; }

    public RoomContents[,] Values { get => values; set => values = value; }

    public Sala(int rows, int cols, Position[] doorsPositions, Enemies[] enemies, Obstacles[] obstacles)
    {
        Rows = rows;
        Cols = cols;

        this.doorsPositions = doorsPositions;
        this.changeablesPositions = new();

        Enemies = new RoomContents[enemies.Length];
        Obstacles = new RoomContents[obstacles.Length];

        Values = new RoomContents[rows, cols];

        PutTheWalls();
        PutTheDoors();

        GetTheChangeablesPositions();
        TransformToRoomContents(enemies, Enemies);
        TransformToRoomContents(obstacles, Obstacles);
    }

    public void PutTheWalls()
    {
        for (int i = 0; i < Rows; i++)
        {
            Values[i, Cols - 1] = RoomContents.Wall;
            Values[i, 0] = RoomContents.Wall;
        }
        for (int j = 0; j < Cols; j++)
        {
            Values[Rows - 1, j] = RoomContents.Wall;
            Values[0, j] = RoomContents.Wall;
        }
    }

    public void PutTheDoors()
    {
        foreach (Position position in doorsPositions)
        {
            Values[position.Row, position.Column] = RoomContents.Door;

            // nao pode ter nada na frente da porta
            PutTheNothingsBeforeTheDoors(position);
        }
    }

    public void PutTheNothingsBeforeTheDoors(Position doorPosition)
    {
        // porta pra esquerda ou pra direita
        if (doorPosition.Row == (int)(Rows / 2))
        {
            if (doorPosition.Column == Cols - 1) // porta na direita da sala
            {
                Values[doorPosition.Row, doorPosition.Column - 1] = RoomContents.Nothing;
            }
            else if (doorPosition.Column == 0) // porta na esquerda da sala
            {
                Values[doorPosition.Row, doorPosition.Column + 1] = RoomContents.Nothing;
            }
        }

        // porta pra cima ou pra baixo
        else if (doorPosition.Column == (int)(Cols / 2))
        {
            if (doorPosition.Row == Rows - 1) // porta embaixo na sala
            {
                Values[doorPosition.Row - 1, doorPosition.Column] = RoomContents.Nothing;
            }
            else if (doorPosition.Row == 0) // porta pra cima na sala
            {
                Values[doorPosition.Row + 1, doorPosition.Column] = RoomContents.Nothing;
            }
        }
    }

    public void GetTheChangeablesPositions()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                var valor = Values[i, j];

                if (valor.GetAttribute<MutavelAttribute>().IsMutavel) changeablesPositions.Add(new Position { Row = i, Column = j });
            }
        }
    }

    void TransformToRoomContents<T>(T[] objects, RoomContents[] type)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (Enum.TryParse(objects[i].ToString(), out RoomContents content))
            {
                type[i] = content;
            }
        }
    }
}
