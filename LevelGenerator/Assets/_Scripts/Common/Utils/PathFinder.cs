using RoomGeneticAlgorithm.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

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
    static bool HasPathBetweenPositions(RoomContents[,] roomContents, Position startPosition, Position endPosition)
    {
        //bool[,] visited = new bool[matrix.GetLength(0), matrix.GetLength(1)];
        return BFS(roomContents, startPosition, endPosition);
    }


    static bool IsValidMove(Position position, RoomContents[,] roomContents, bool[,] visited)
    {
        return roomContents.IsPositionWithinBounds(position.X, position.Y)
            && RoomContentsInfo.IsTransversable(roomContents[position.X, position.Y])
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
    static bool DFS(RoomContents[,] roomContents, Position currentPosition, Position endPosition, bool[,] visited)
    {
        if (currentPosition.Equals(endPosition))
        {
            return true;
        }

        visited[currentPosition.X, currentPosition.Y] = true;

        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            Position adjacentPosition = currentPosition.Move(direction);
            if (adjacentPosition.Equals(endPosition)) return true;

            if (IsValidMove(adjacentPosition, roomContents, visited) &&
                DFS(roomContents, adjacentPosition, endPosition, visited))
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
    static bool BFS(RoomContents[,] roomContents, Position startPosition, Position endPosition)
    {
        int rows = roomContents.GetLength(0);
        int cols = roomContents.GetLength(1);
        bool[,] visited = new bool[rows, cols];

        Queue<Position> queue = new();
        queue.Enqueue(startPosition);

        while (queue.Count > 0)
        {
            Position currentPosition = queue.Dequeue();
            if (currentPosition.Equals(endPosition)) return true;

            visited[currentPosition.X, currentPosition.Y] = true;

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = currentPosition.Move(direction);
                if (adjacentPosition.Equals(endPosition)) return true;

                if (IsValidMove(adjacentPosition, roomContents, visited))
                {
                    queue.Enqueue(adjacentPosition);
                    visited[adjacentPosition.X, adjacentPosition.Y] = true;
                }
            }
        }

        return false;
    }

    public static bool AreAllDoorsAndEnemiesReachable(RoomMatrix roomMatrix)
    {
        RoomContents[,] roomContents = roomMatrix.Values;
        HashSet<Position> doorPositions = roomMatrix.SharedRoomData.DoorPositions;
        HashSet<Position> enemyPositions = roomMatrix.EnemiesPositions;

        int totalObjects = doorPositions.Count + enemyPositions.Count;

        bool[,] visited = new bool[roomContents.GetLength(0), roomContents.GetLength(1)];
        Queue<Position> queue = new();

        // Comecar pela primeira porta
        Position firstDoor = doorPositions.First();
        queue.Enqueue(firstDoor);
        visited[firstDoor.X, firstDoor.Y] = true;

        int found = 1;

        while (queue.Count > 0)
        {
            Position currentPosition = queue.Dequeue();

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacent = currentPosition.Move(direction);

                if (IsValidMove(adjacent, roomContents, visited))
                {
                    if (doorPositions.Contains(adjacent) || enemyPositions.Contains(adjacent))
                    {
                        found++;
                        if (found == totalObjects)
                        {
                            return true;
                        }
                    }
                    queue.Enqueue(adjacent);
                    visited[adjacent.X, adjacent.Y] = true;
                }
            }
        }

        return false;
    }
}
