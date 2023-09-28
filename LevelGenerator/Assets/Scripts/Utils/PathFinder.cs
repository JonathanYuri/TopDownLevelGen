using System;
using System.Collections.Generic;

public static class PathFinder
{
    static bool HasPathBetweenPositions(int[,] matrix, Position startPosition, Position endPosition)
    {
        bool[,] visited = new bool[matrix.GetLength(0), matrix.GetLength(1)];
        return DFS(matrix, startPosition, endPosition, visited);
    }

    static bool IsValidMove(Position position, int[,] matrix, bool[,] visited)
    {
        return matrix.IsPositionWithinBounds(position.X, position.Y)
            && matrix[position.X, position.Y] == 1
            && !visited[position.X, position.Y];
    }

    static bool DFS(int[,] matrix, Position currentPosition, Position endPosition, bool[,] visited)
    {
        if (currentPosition.Equals(endPosition))
        {
            return true;
        }

        visited[currentPosition.X, currentPosition.Y] = true;

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

    public static bool IsAPathBetweenDoors(RoomContents[,] roomMatrix)
    {
        int[,] matrix = TransformRoomForCountPaths(roomMatrix, IsPassable);
         
        for (int i = 0; i < GeneticAlgorithmConstants.ROOM.doorsPositions.Length; i++)
        {
            for (int j = i + 1; j < GeneticAlgorithmConstants.ROOM.doorsPositions.Length; j++)
            {
                if (!HasPathBetweenPositions(matrix, GeneticAlgorithmConstants.ROOM.doorsPositions[i], GeneticAlgorithmConstants.ROOM.doorsPositions[j]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool IsAPathBetweenDoorAndEnemies(RoomMatrix roomMatrix)
    {
        if (roomMatrix.EnemiesPositions.Count == 0)
        {
            return false;
        }

        int[,] matriz = TransformRoomForCountPaths(roomMatrix.Values, IsPassable);

        // so preciso ver de uma porta pra todos os inimigos, pq se tiver de uma tem da outra, ja que eu conto os caminhos de uma porta a outra
        // TODO: if inimigo nao for voador, se for voador nao precisa verificar se tem caminho pra ele eu acho
        foreach (Position enemiePosition in roomMatrix.EnemiesPositions)
        {
            if (!HasPathBetweenPositions(matriz, GeneticAlgorithmConstants.ROOM.doorsPositions[0], enemiePosition))
            {
                return false;
            }
        }

        return true;
    }

    static bool IsPassable(RoomContents roomContent) => roomContent.GetAttribute<UltrapassavelAttribute>().IsUltrapassavel;

    static int[,] TransformRoomForCountPaths(RoomContents[,] roomMatrix, Func<RoomContents, bool> criteria)
    {
        int[,] matrix = new int[roomMatrix.GetLength(0), roomMatrix.GetLength(1)];

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j] = criteria(roomMatrix[i, j]) ? 1 : 0;
            }
        }
        return matrix;
    }
}
