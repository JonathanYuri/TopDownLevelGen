using System;
using System.Collections.Generic;
using UnityEngine;

public enum Possibilidades
{
    [Mutavel(true)][Ultrapassavel(true)] Chao, // TODO: colocar tudo em ingles
    [Mutavel(true)][Ultrapassavel(false)] Obstaculo,
    [Mutavel(true)][Ultrapassavel(true)] Inimigo,

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
    public Range enemiesRange, obstaclesRange;

    public Possibilidades[,] matriz;

    public List<Tuple<int, int>> doorsPositions;

    // changeables
    public List<Tuple<int, int>> changeablesPositions;
    public List<Possibilidades> changeablesPossibilities;

    public int Rows { get => rows; set => rows = value; }
    public int Cols { get => cols; set => cols = value; }

    public Sala(int rows, int cols, List<Tuple<int, int>> doorsPositions, Range enemiesRange, Range obstaclesRange)
    {
        this.Rows = rows;
        this.Cols = cols;

        this.enemiesRange = enemiesRange;
        this.obstaclesRange = obstaclesRange;

        this.doorsPositions = doorsPositions;

        this.changeablesPositions = new();
        this.changeablesPossibilities = new();

        matriz = new Possibilidades[rows, cols];

        PutTheWalls();
        PutTheDoors();

        GetTheChangeablesPositions();
        GetTheChangeablesPossibilities();
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

    public void GetTheChangeablesPossibilities()
    {
        Type enumType = typeof(Possibilidades);
        foreach (Possibilidades valor in Enum.GetValues(enumType))
        {
            if (valor.GetAttribute<MutavelAttribute>().IsMutavel)
            {
                changeablesPossibilities.Add(valor);
            }
        }
    }
}
