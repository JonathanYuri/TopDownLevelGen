using System;
using System.Collections.Generic;

/// <summary>
/// A static class that provides methods for pathfinding and determining connectivity in a room matrix.
/// </summary>
public static class PathFinder
{
    /// <summary>
    /// Checks if a path exists between two positions in a given matrix using Breadth-First Search.
    /// </summary>
    /// <param name="matrix">The matrix representing the room layout.</param>
    /// <param name="startPosition">The starting position for the path.</param>
    /// <param name="endPosition">The ending position for the path.</param>
    /// <returns>True if a path exists between the start and end positions; otherwise, false.</returns>
    static bool HasPathBetweenPositions(int[,] matrix, Position startPosition, Position endPosition)
    {
        //bool[,] visited = new bool[matrix.GetLength(0), matrix.GetLength(1)];
        return BFS(matrix, startPosition, endPosition);
    }


    static bool IsValidMove(Position position, int[,] matrix, bool[,] visited)
    {
        return matrix.IsPositionWithinBounds(position.X, position.Y)
            && matrix[position.X, position.Y] == 1
            && !visited[position.X, position.Y];
    }

    /// <summary>
    /// Implements Depth-First Search to check for a path between positions in the matrix.
    /// </summary>
    /// <param name="matrix">The matrix representing the room layout.</param>
    /// <param name="currentPosition">The current position in the search.</param>
    /// <param name="endPosition">The ending position for the path.</param>
    /// <param name="visited">A matrix to track visited positions.</param>
    /// <returns>True if a path exists between the current and end positions; otherwise, false.</returns>
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

    /// <summary>
    /// Implements Breadth-First Search to check for a path between positions in the matrix.
    /// </summary>
    /// <param name="matrix">The matrix representing the room layout.</param>
    /// <param name="startPosition">The starting position for the search.</param>
    /// <param name="endPosition">The ending position for the path.</param>
    /// <returns>True if a path exists between the start and end positions; otherwise, false.</returns>
    static bool BFS(int[,] matrix, Position startPosition, Position endPosition)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        bool[,] visited = new bool[rows, cols];

        Queue<Position> queue = new();
        queue.Enqueue(startPosition);

        while (queue.Count > 0)
        {
            Position currentPosition = queue.Dequeue();

            if (currentPosition.Equals(endPosition))
            {
                return true;
            }

            visited[currentPosition.X, currentPosition.Y] = true;

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = currentPosition.Move(direction);
                if (IsValidMove(adjacentPosition, matrix, visited))
                {
                    queue.Enqueue(adjacentPosition);
                    visited[adjacentPosition.X, adjacentPosition.Y] = true;
                }
            }
        }

        return false;
    }

    public static bool IsAPathBetweenDoors(RoomContents[,] roomMatrix)
    {
        int[,] matrix = TransformRoomForCountPaths(roomMatrix, IsPassable);
         
        for (int i = 0; i < GeneticAlgorithmConstants.ROOM.DoorsPositions.Length; i++)
        {
            for (int j = i + 1; j < GeneticAlgorithmConstants.ROOM.DoorsPositions.Length; j++)
            {
                if (!HasPathBetweenPositions(matrix, GeneticAlgorithmConstants.ROOM.DoorsPositions[i], GeneticAlgorithmConstants.ROOM.DoorsPositions[j]))
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
            if (!HasPathBetweenPositions(matriz, GeneticAlgorithmConstants.ROOM.DoorsPositions[0], enemiePosition))
            {
                return false;
            }
        }

        return true;
    }

    static bool IsPassable(RoomContents roomContent) => roomContent.GetAttribute<TraversableAttribute>().IsTraversable;

    /// <summary>
    /// Transforms a room matrix into an integer matrix with 0s and 1s based on a given criteria.
    /// </summary>
    /// <param name="roomMatrix">The room matrix to transform.</param>
    /// <param name="criteria">The criteria function for transformation.</param>
    /// <returns>An integer matrix where 1s represent passable cells based on the criteria.</returns>
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
