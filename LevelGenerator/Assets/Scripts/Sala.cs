using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static UnityEngine.EventSystems.EventTrigger;

public enum Enemies
{
    Inimigo1,
    Inimigo2,
    Inimigo3,
}

public enum Obstacles
{
    Obstaculo1,
    Obstaculo2,
    Obstaculo3
}

public enum RoomContents
{
    [Mutavel(true)][Ultrapassavel(true)] Chao, // TODO: colocar tudo em ingles

    [Mutavel(true)][Ultrapassavel(false)] Obstaculo1,
    [Mutavel(true)][Ultrapassavel(false)] Obstaculo2,
    [Mutavel(true)][Ultrapassavel(false)] Obstaculo3,

    [Mutavel(true)][Ultrapassavel(true)] Inimigo1,
    [Mutavel(true)][Ultrapassavel(true)] Inimigo2,
    [Mutavel(true)][Ultrapassavel(true)] Inimigo3,

    [Mutavel(false)][Ultrapassavel(true)] Nada,
    [Mutavel(false)][Ultrapassavel(true)] Porta,
    [Mutavel(false)][Ultrapassavel(false)] Parede,
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
    int rows, cols;

    public RoomContents[,] matriz;

    public readonly Position[] doorsPositions;
    public List<Position> changeablesPositions;

    public readonly Enemies[] enemies;
    public readonly Obstacles[] obstacles;

    public Dictionary<Enemies, RoomContents> enemiesToPossibilidades;
    public Dictionary<Obstacles, RoomContents> obstaclesToPossibilidades;

    public int Rows { get => rows; set => rows = value; }
    public int Cols { get => cols; set => cols = value; }

    public Sala(int rows, int cols, Position[] doorsPositions, Enemies[] enemies, Obstacles[] obstacles)
    {
        this.Rows = rows;
        this.Cols = cols;

        this.doorsPositions = doorsPositions;
        this.changeablesPositions = new();

        this.enemies = enemies;
        this.obstacles = obstacles;
        this.enemiesToPossibilidades = new();
        this.obstaclesToPossibilidades = new();

        this.matriz = new RoomContents[rows, cols];

        PutTheWalls();
        PutTheDoors();

        GetTheChangeablesPositions();
        TransformEnemiesToPossibilidades();
        TransformObstaclesToPossibilidades();
    }

    public void PutTheWalls()
    {
        for (int i = 0; i < Rows; i++)
        {
            matriz[i, Cols - 1] = RoomContents.Parede;
            matriz[i, 0] = RoomContents.Parede;
        }
        for (int j = 0; j < Cols; j++)
        {
            matriz[Rows - 1, j] = RoomContents.Parede;
            matriz[0, j] = RoomContents.Parede;
        }
    }

    public void PutTheDoors()
    {
        foreach (Position position in doorsPositions)
        {
            matriz[position.Row, position.Column] = RoomContents.Porta;

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
                matriz[doorPosition.Row, doorPosition.Column - 1] = RoomContents.Nada;
            }
            else if (doorPosition.Column == 0) // porta na esquerda da sala
            {
                matriz[doorPosition.Row, doorPosition.Column + 1] = RoomContents.Nada;
            }
        }

        // porta pra cima ou pra baixo
        else if (doorPosition.Column == (int)(Cols / 2))
        {
            if (doorPosition.Row == Rows - 1) // porta embaixo na sala
            {
                matriz[doorPosition.Row - 1, doorPosition.Column] = RoomContents.Nada;
            }
            else if (doorPosition.Row == 0) // porta pra cima na sala
            {
                matriz[doorPosition.Row + 1, doorPosition.Column] = RoomContents.Nada;
            }
        }
    }

    public void GetTheChangeablesPositions()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                var valor = matriz[i, j];

                if (valor.GetAttribute<MutavelAttribute>().IsMutavel) changeablesPositions.Add(new Position { Row = i, Column = j });
            }
        }
    }

    void TransformEnemiesToPossibilidades()
    {
        foreach (Enemies enemy in enemies)
        {
            if (Enum.TryParse(enemy.ToString(), out RoomContents possibility))
            {
                enemiesToPossibilidades[enemy] = possibility;
            }
        }
    }

    void TransformObstaclesToPossibilidades()
    {
        foreach (Obstacles obstacle in obstacles)
        {
            if (Enum.TryParse(obstacle.ToString(), out RoomContents possibility))
            {
                obstaclesToPossibilidades[obstacle] = possibility;
            }
        }
    }
}
