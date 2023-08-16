using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

public enum Possibilidades
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

    public Possibilidades[,] matriz;

    public List<Tuple<int, int>> doorsPositions;
    public List<Tuple<int, int>> changeablesPositions;

    public List<Enemies> enemies;
    public List<Obstacles> obstacles;

    public int Rows { get => rows; set => rows = value; }
    public int Cols { get => cols; set => cols = value; }

    public Sala(int rows, int cols, List<Tuple<int, int>> doorsPositions, List<Enemies> enemies, List<Obstacles> obstacles)
    {
        this.Rows = rows;
        this.Cols = cols;

        this.doorsPositions = doorsPositions;
        this.changeablesPositions = new();

        this.enemies = enemies;
        this.obstacles = obstacles;

        this.matriz = new Possibilidades[rows, cols];

        PutTheWalls();
        PutTheDoors();

        GetTheChangeablesPositions();
    }

    public void PutTheWalls()
    {
        for (int i = 0; i < Rows; i++)
        {
            matriz[i, Cols - 1] = Possibilidades.Parede;
            matriz[i, 0] = Possibilidades.Parede;
        }
        for (int j = 0; j < Cols; j++)
        {
            matriz[Rows - 1, j] = Possibilidades.Parede;
            matriz[0, j] = Possibilidades.Parede;
        }
    }

    public void PutTheDoors()
    {
        foreach (Tuple<int, int> position in doorsPositions)
        {
            matriz[position.Item1, position.Item2] = Possibilidades.Porta;

            // nao pode ter nada na frente da porta

            PutTheNothingsBeforeTheDoors(position);
        }
    }

    public void PutTheNothingsBeforeTheDoors(Tuple<int, int> doorPosition)
    {
        // porta pra esquerda ou pra direita
        if (doorPosition.Item1 == (int)(Rows / 2))
        {
            if (doorPosition.Item2 == Cols - 1) // porta na direita da sala
            {
                matriz[doorPosition.Item1, doorPosition.Item2 - 1] = Possibilidades.Nada;
            }
            else if (doorPosition.Item2 == 0) // porta na esquerda da sala
            {
                matriz[doorPosition.Item1, doorPosition.Item2 + 1] = Possibilidades.Nada;
            }
        }

        // porta pra cima ou pra baixo
        else if (doorPosition.Item2 == (int)(Cols / 2))
        {
            if (doorPosition.Item1 == Rows - 1) // porta embaixo na sala
            {
                matriz[doorPosition.Item1 - 1, doorPosition.Item2] = Possibilidades.Nada;
            }
            else if (doorPosition.Item1 == 0) // porta pra cima na sala
            {
                matriz[doorPosition.Item1 + 1, doorPosition.Item2] = Possibilidades.Nada;
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

                if (valor.GetAttribute<MutavelAttribute>().IsMutavel) changeablesPositions.Add(new Tuple<int, int>(i, j));
            }
        }
    }
}
