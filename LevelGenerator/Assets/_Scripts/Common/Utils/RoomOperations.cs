using RoomGeneticAlgorithm.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A static class that provides various operations related to room contents.
/// </summary>
public static class RoomOperations
{
    public static float AverageObstaclesNextToEnemies(RoomMatrix roomMatrix)
    {
        int totalObstaclesNextToEnemies = 0;
        foreach (Position enemyPosition in roomMatrix.EnemiesPositions)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = enemyPosition.Move(direction);
                if (roomMatrix.Values.IsPositionWithinBounds(adjacentPosition.X, adjacentPosition.Y) &&
                    roomMatrix.ObstaclesPositions.Contains(adjacentPosition))
                {
                    totalObstaclesNextToEnemies++;
                }
            }
        }

        return (float)totalObstaclesNextToEnemies / (float)roomMatrix.EnemiesPositions.Count;
    }

    public static float AverageEnemiesWithCover(RoomMatrix roomMatrix)
    {
        int totalEnemiesWithCover = 0;
        foreach (Position enemyPosition in roomMatrix.EnemiesPositions)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = enemyPosition.Move(direction);

                if (roomMatrix.Values.IsPositionWithinBounds(adjacentPosition.X, adjacentPosition.Y) &&
                    roomMatrix.ObstaclesPositions.Contains(adjacentPosition))
                {
                    totalEnemiesWithCover++;
                    break;
                }
            }
        }

        return (float)totalEnemiesWithCover / (float)roomMatrix.EnemiesPositions.Count;
    }

    public static float AverageDistanceFromDoorsToEnemies(HashSet<Position> enemiesPositions, Position[] doorPositions)
    {
        int totalDistance = 0;
        int pairCount = 0;

        foreach (Position doorPosition in doorPositions)
        {
            foreach (Position enemyPosition in enemiesPositions)
            {
                totalDistance += Utils.CalculateDistance(doorPosition, enemyPosition);
                pairCount++;
            }
        }
        return (float)totalDistance / pairCount;
    }

    public static float AverageDistanceBetweenEnemies(List<Position> enemiesPositions)
    {
        int totalDistance = 0;
        int pairCount = 0;

        for (int i = 0; i < enemiesPositions.Count; i++)
        {
            Position enemyPosition1 = enemiesPositions[i];
            for (int j = i + 1; j < enemiesPositions.Count; j++)
            {
                Position enemyPosition2 = enemiesPositions[j];
                totalDistance += Utils.CalculateDistance(enemyPosition1, enemyPosition2);
                pairCount++;
            }
        }
        return (float)totalDistance / pairCount;
    }

    public static int MinimumDistanceBetweenDoorsAndEnemies(HashSet<Position> enemiesPositions, Position[] doorPositions)
    {
        int minDistance = int.MaxValue;
        foreach (Position doorPosition in doorPositions)
        {
            minDistance = Math.Min(minDistance, enemiesPositions.Min(enemyPosition => Utils.CalculateDistance(doorPosition, enemyPosition)));
        }
        return minDistance;
    }
}
