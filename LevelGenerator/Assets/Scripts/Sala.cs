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

public class Sala
{
    RoomContents[,] values;

    public readonly Position[] doorsPositions;
    public List<Position> changeablesPositions;

    public int Height { get; }
    public int Width { get; }

    public RoomContents[] Enemies { get; }
    public RoomContents[] Obstacles { get; }

    public RoomContents[,] Values { get => values; set => values = value; }

    public Sala(int height, int width, Position[] doorsPositions, RoomContents[] enemies, RoomContents[] obstacles)
    {
        Height = height;
        Width = width;

        this.doorsPositions = doorsPositions;
        this.changeablesPositions = new();

        Enemies = enemies;
        Obstacles = obstacles;

        Values = new RoomContents[width, height];

        PutTheWalls();
        PutTheDoors();

        GetTheChangeablesPositions();
    }

    public void PutTheWalls()
    {
        for (int j = 0; j < Height; j++)
        {
            Values[Width - 1, j] = RoomContents.Wall;
            Values[0, j] = RoomContents.Wall;
        }
        for (int i = 0; i < Width; i++)
        {
            Values[i, Height - 1] = RoomContents.Wall;
            Values[i, 0] = RoomContents.Wall;
        }
    }

    public void PutTheDoors()
    {
        foreach (Position position in doorsPositions)
        {
            Values[position.X, position.Y] = RoomContents.Door;

            // nao pode ter nada na frente da porta
            PutTheNothingsBeforeTheDoors(position);
        }
    }

    public void PutTheNothingsBeforeTheDoors(Position doorPosition)
    {
        // porta pra cima ou pra baixo
        if (doorPosition.X == (int)(Height / 2))
        {
            if (doorPosition.Y == Width - 1) // porta na direita da sala
            {
                Values[doorPosition.X, doorPosition.Y - 1] = RoomContents.Nothing;
            }
            else if (doorPosition.Y == 0) // porta na esquerda da sala
            {
                Values[doorPosition.X, doorPosition.Y + 1] = RoomContents.Nothing;
            }
        }

        // porta pra esquerda ou pra direita
        else if (doorPosition.Y == (int)(Width / 2))
        {
            if (doorPosition.X == Height - 1) // porta embaixo na sala
            {
                Values[doorPosition.X - 1, doorPosition.Y] = RoomContents.Nothing;
            }
            else if (doorPosition.X == 0) // porta pra cima na sala
            {
                Values[doorPosition.X + 1, doorPosition.Y] = RoomContents.Nothing;
            }
        }
    }

    public void GetTheChangeablesPositions()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                var valor = Values[i, j];

                if (valor.GetAttribute<MutavelAttribute>().IsMutavel) changeablesPositions.Add(new Position { X = i, Y = j });
            }
        }
    }
}
