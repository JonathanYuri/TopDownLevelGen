using System;
using System.Collections.Generic;

public static class PathFinder
{
    public static int CountPathsBetweenDoors(Possibilidades[,] roomMatrix, Position[] doorsPositions)
    {
        // transformar 
        int[,] matrix = TransformRoomForCountPaths(roomMatrix);

        int qntCaminhos = 0;
        for (int i = 0; i < doorsPositions.GetLength(0); i++)
        {
            for (int j = i + 1; j < doorsPositions.GetLength(0); j++)
            {
                //Console.WriteLine("PORTA: " + i + " x " + j);
                int[,] paths = CountPaths(matrix, doorsPositions[i]);

                if (paths[doorsPositions[j].Row, doorsPositions[j].Column] == 0)
                {
                    return int.MinValue;
                }
                qntCaminhos += paths[doorsPositions[j].Row, doorsPositions[j].Column];
            }
        }

        return qntCaminhos;
    }

    public static bool IsAPathBetweenDoorAndEnemies(Possibilidades[,] roomMatrix, Position[] doorsPositions, HashSet<Position> enemiesPositions)
    {
        if (enemiesPositions.Count == 0)
        {
            throw new Exception("Sem inimigos na sala");
        }

        int[,] matriz = TransformRoomForCountPaths(roomMatrix);

        // so preciso ver de uma porta pra todos os inimigos, pq se tiver de uma tem da outra, ja que eu conto os caminhos de uma porta a outra
        // TODO: if inimigo nao for voador, se for voador nao precisa verificar se tem caminho pra ele eu acho
        foreach (Position enemiePosition in enemiesPositions)
        {
            int[,] paths = CountPaths(matriz, doorsPositions[0]);
            if (paths[enemiePosition.Row, enemiePosition.Column] == 0)
            {
                return false;
            }
        }

        return true;
    }

    static int[,] TransformRoomForCountPaths(Possibilidades[,] roomMatrix)
    {
        int[,] matrix = new int[roomMatrix.GetLength(0), roomMatrix.GetLength(1)];

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (roomMatrix[i, j].GetAttribute<UltrapassavelAttribute>().IsUltrapassavel)
                {
                    matrix[i, j] = 1;
                }
                else
                {
                    matrix[i, j] = 0;
                }
            }
        }
        return matrix;
    }

    static int[,] CountPaths(int[,] matrix, Position initialPosition)
    {
        int[,] countPath = new int[matrix.GetLength(0), matrix.GetLength(1)];

        Queue<Position> queue = new();

        // Adicionar o ponto de partida à fila
        queue.Enqueue(initialPosition);
        countPath[initialPosition.Row, initialPosition.Column] = 1;

        // BFS
        while (queue.Count > 0)
        {
            Position position = queue.Dequeue();

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = position.Move(direction);
                if (matrix.IsPositionWithinBounds(adjacentPosition) && matrix[adjacentPosition.Row, adjacentPosition.Column] == 1)
                {
                    // Se ainda não foi visitada, adicionar à fila
                    if (countPath[adjacentPosition.Row, adjacentPosition.Column] == 0)
                    {
                        queue.Enqueue(adjacentPosition);
                    }

                    countPath[adjacentPosition.Row, adjacentPosition.Column] += countPath[position.Row, position.Column];
                }
            }
        }

        return countPath;
    }
}
