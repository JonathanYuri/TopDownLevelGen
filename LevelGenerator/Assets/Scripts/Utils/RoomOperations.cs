using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public static class RoomOperations
{
    public static Dictionary<RoomContents, List<Position>> GroupPositionsByRoomValue(RoomContents[,] matrix, HashSet<Position> positionsOf)
    {
        // agrupar as posicoes com base no valor da matrix na posicao
        IEnumerable<IGrouping<RoomContents, Position>> groupedPositions = positionsOf.GroupBy(p => matrix[p.X, p.Y]);

        // transformar o groupedPositions em dicionario
        Dictionary<RoomContents, List<Position>> positionsByPossibilidades = groupedPositions.ToDictionary(group => group.Key, group => group.ToList());

        return positionsByPossibilidades;
    }

    public static int CountEnemiesNextToObstacles(
        RoomContents[,] matrix,
        HashSet<Position> enemiesPositions,
        HashSet<Position> obstaclesPositions)
    {
        int enemies = 0;
        foreach (Position obstaclePosition in obstaclesPositions)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = obstaclePosition.Move(direction);
                if (matrix.IsPositionWithinBounds(adjacentPosition.X, adjacentPosition.Y))
                {
                    if (enemiesPositions.Contains(adjacentPosition))
                    {
                        enemies++;
                    }
                }
            }
        }

        return enemies;
    }

    public static double AverageDistanceFromDoorsToEnemies(Position[] doorsPositions, HashSet<Position> enemiesPositions)
    {
        List<double> averagesDistances = new();
        foreach (Position doorPosition in doorsPositions)
        {
            List<int> averagesDistancesFromDoorPosition = new();
            foreach (Position enemyPosition in enemiesPositions)
            {
                averagesDistancesFromDoorPosition.Add(Utils.ManhattanDistance(doorPosition, enemyPosition));
            }
            averagesDistances.Add(averagesDistancesFromDoorPosition.Average());
        }
        return averagesDistances.Average();
    }

    public static int MinimumDistanceBetweenDoorsAndEnemies(Position[] doorsPositions, HashSet<Position> enemiesPositions)
    {
        int minDistance = int.MaxValue;
        foreach (Position doorPosition in doorsPositions)
        {
            int currentMinDistance = enemiesPositions.Min(enemyPosition => Utils.ManhattanDistance(doorPosition, enemyPosition));
            if (currentMinDistance < minDistance)
            {
                minDistance = currentMinDistance;
            }
        }
        return minDistance;
    }
}
