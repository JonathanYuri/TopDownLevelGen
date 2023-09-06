using System;
using System.Collections.Generic;
using UnityEngine.Networking.Match;

public static class PathFinder
{
    static bool HasPath(int[,] matrix, Position startPosition, Position endPosition)
    {
        bool[,] visited = new bool[matrix.GetLength(0), matrix.GetLength(1)];
        return DFS(matrix, startPosition, endPosition, visited);
    }

    static bool IsValidMove(Position position, int[,] matrix, bool[,] visited)
    {
        return matrix.IsPositionWithinBounds(position.Row, position.Column)
            && matrix[position.Row, position.Column] == 1
            && !visited[position.Row, position.Column];
    }

    static bool DFS(int[,] matrix, Position currentPosition, Position endPosition, bool[,] visited)
    {
        if (currentPosition.Equals(endPosition))
        {
            return true;
        }

        visited[currentPosition.Row, currentPosition.Column] = true;

        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            Position adjacentPosition = currentPosition.Move(direction);
            if (IsValidMove(adjacentPosition, matrix, visited) && DFS(matrix, adjacentPosition, endPosition, visited))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsAPathBetweenDoors(RoomContents[,] roomMatrix, Position[] doorsPositions)
    {
        int[,] matrix = TransformRoomForCountPaths(roomMatrix);

        for (int i = 0; i < doorsPositions.Length; i++)
        {
            for (int j = i + 1; j < doorsPositions.Length; j++)
            {
                if (!HasPath(matrix, doorsPositions[i], doorsPositions[j]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool IsAPathBetweenDoorAndEnemies(RoomContents[,] roomMatrix, Position[] doorsPositions, HashSet<Position> enemiesPositions)
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
            if (!HasPath(matriz, doorsPositions[0], enemiePosition))
            {
                return false;
            }
        }

        return true;
    }

    static int[,] TransformRoomForCountPaths(RoomContents[,] roomMatrix)
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
}
