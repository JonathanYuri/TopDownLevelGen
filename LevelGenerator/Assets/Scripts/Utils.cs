using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class EnumExtensions
{
    public static T GetAttribute<T>(this Enum enumValue) where T : Attribute
    {
        var type = enumValue.GetType();
        var memberInfo = type.GetMember(enumValue.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
        return (T)attributes[0];
    }
}

public static class Utils
{
    public static int CountOccurrences(Possibilidades[,] matriz, Possibilidades toBeCounted)
    {
        int count = 0;
        for (int i = 0; i < matriz.GetLength(0); i++)
        {
            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                if (matriz[i, j].Equals(toBeCounted))
                {
                    count++;
                }
            }
        }
        return count;
    }

    static Tuple<int, int> ChooseLocationFreeFrom(Possibilidades[,] matriz, List<Tuple<int, int>> changeablesPositions, Possibilidades freeFrom)
    {
        List<Tuple<int, int>> positionsFreeFrom = new();
        foreach (Tuple<int, int> position in changeablesPositions)
        {
            if (!matriz[position.Item1, position.Item2].Equals(freeFrom))
            {
                positionsFreeFrom.Add(position);
            }
        }

        if (positionsFreeFrom.Count == 0)
        {
            return new Tuple<int, int>(-1, -1);
        }
        int idx = Random.Range(0, positionsFreeFrom.Count);
        return positionsFreeFrom[idx];
    }

    static Tuple<int, int> ChooseLocationThatHas(Possibilidades[,] matriz, List<Tuple<int, int>> changeablesPositions, Possibilidades has)
    {
        List<Tuple<int, int>> positionsHas = new();
        foreach (Tuple<int, int> position in changeablesPositions)
        {
            if (matriz[position.Item1, position.Item2].Equals(has))
            {
                positionsHas.Add(position);
            }
        }

        if (positionsHas.Count == 0)
        {
            return new Tuple<int, int>(-1, -1);
        }
        int idx = Random.Range(0, positionsHas.Count);
        return positionsHas[idx];
    }

    public static void AddToMatrix(Sala sala, Possibilidades[,] roomMatrix, Possibilidades add)
    {
        // escolher um lugar que nao tem o que eu quero adicionar
        Tuple<int, int> position = ChooseLocationFreeFrom(roomMatrix, sala.changeablesPositions, add);
        if (position.Item1 != -1 && position.Item2 != -1)
        {
            roomMatrix[position.Item1, position.Item2] = add;
        }
    }

    public static void RemoveFromMatrix(Sala sala, Possibilidades[,] roomMatrix, Possibilidades remove)
    {
        // escolher um lugar que tem o que eu quero remover
        Tuple<int, int> position = ChooseLocationThatHas(roomMatrix, sala.changeablesPositions, remove);
        if (position.Item1 != -1 && position.Item2 != -1)
        {
            // mudar por algo que nao é o que eu removi
            List<Possibilidades> others = RemoveFromList(sala.changeablesPossibilities, remove);

            int rand = Random.Range(0, others.Count);
            roomMatrix[position.Item1, position.Item2] = others[rand];
        }
    }

    public static List<Possibilidades> RemoveFromList(List<Possibilidades> list, Possibilidades remove)
    {
        List<Possibilidades> newList = new();
        foreach (Possibilidades p in list)
        {
            if (!p.Equals(remove))
            {
                newList.Add(p);
            }
        }
        return newList;
    }

    public static int CountPathsBetweenDoors(Possibilidades[,] roomMatrix, List<Tuple<int, int>> doorsPositions)
    {
        // transformar 
        int[,] matriz = TransformRoomForCountPaths(roomMatrix);

        int qntCaminhos = 0;
        for (int i = 0; i < doorsPositions.Count; i++)
        {
            for (int j = i + 1; j < doorsPositions.Count; j++)
            {
                //Console.WriteLine("PORTA: " + i + " x " + j);
                int paths = CountPaths(matriz, doorsPositions[i], doorsPositions[j]);
                qntCaminhos += paths;

                //Console.WriteLine("QUANTIDADE DE CAMINHOS: " + paths);
                if (paths == 0)
                {
                    return int.MinValue;
                }
            }
        }

        return qntCaminhos;
    }

    private static int[,] TransformRoomForCountPaths(Possibilidades[,] roomMatrix)
    {
        int[,] matriz = new int[roomMatrix.GetLength(0), roomMatrix.GetLength(1)];

        for (int i = 0; i < matriz.GetLength(0); i++)
        {
            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                if (roomMatrix[i, j].GetAttribute<UltrapassavelAttribute>().IsUltrapassavel)
                {
                    matriz[i, j] = 1;
                }
                else
                {
                    matriz[i, j] = 0;
                }
            }
        }
        return matriz;
    }

    private static int CountPaths(int[,] matrix, Tuple<int, int> initialPosition, Tuple<int, int> endPosition)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        int[,] countPath = new int[rows, cols];

        Queue<(int, int)> queue = new();

        // Adicionar o ponto de partida à fila
        queue.Enqueue((initialPosition.Item1, initialPosition.Item2));
        countPath[initialPosition.Item1, initialPosition.Item2] = 1;

        // acima, abaixo, esquerda, direita
        int[] directionRow = { -1, 1, 0, 0 };
        int[] directionColumn = { 0, 0, -1, 1 };

        // BFS
        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int nx = x + directionRow[i];
                int ny = y + directionColumn[i];

                // Verificar se a próxima posição está dentro dos limites da matriz e é acessível
                if (nx >= 0 && ny >= 0 && nx < rows && ny < cols && matrix[nx, ny] == 1)
                {
                    // Se ainda não foi visitada, adicionar à fila
                    if (countPath[nx, ny] == 0)
                    {
                        queue.Enqueue((nx, ny));
                    }

                    countPath[nx, ny] += countPath[x, y];
                }
            }
        }

        return countPath[endPosition.Item1, endPosition.Item2];
    }

    public static int CalculateDistanceToRange(int min, int max, int number)
    {
        int distanceToRange;
        if (number > max) distanceToRange = max - number;
        else if (number < min) distanceToRange = number - min;
        else distanceToRange = 0;

        return distanceToRange;
    }
}
