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
    static readonly HashSet<RoomContents> transversableContents = new()
    {
        { RoomContents.Ground },
        { RoomContents.Enemy1 },
        { RoomContents.Enemy2 },
        { RoomContents.Enemy3 },
        { RoomContents.Nothing },
        { RoomContents.Door },
        { RoomContents.LevelEnd },
    };

    readonly static Direction[] directions = DirectionUtilities.allDirections;

    static bool IsValidMove(Position position, RoomContents[,] roomContents, bool[,] visited)
    {
        return roomContents.IsPositionWithinBounds(position)
            && transversableContents.Contains(roomContents[position.X, position.Y])
            && !visited[position.X, position.Y];
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

            foreach (Direction direction in directions)
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
