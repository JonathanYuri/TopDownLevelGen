using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// A static class that provides various operations related to room contents.
/// </summary>
public static class RoomOperations
{
    /// <summary>
    /// Groups positions based on the room value in the room matrix.
    /// </summary>
    /// <param name="roomMatrix">The room matrix representing the room layout.</param>
    /// <param name="targetPositions">The positions to group based on room values.</param>
    /// <returns>A dictionary that maps room contents to lists of positions with the same value.</returns>
    public static Dictionary<RoomContents, List<Position>> GroupPositionsByRoomValue(RoomContents[,] roomMatrix, HashSet<Position> targetPositions)
    {
        // agrupar as posicoes com base no valor da matrix na posicao
        IEnumerable<IGrouping<RoomContents, Position>> groupedPositions = targetPositions.GroupBy(p => roomMatrix[p.X, p.Y]);

        // transformar o groupedPositions em dicionario
        Dictionary<RoomContents, List<Position>> positionsByPossibilidades = groupedPositions.ToDictionary(group => group.Key, group => group.ToList());

        return positionsByPossibilidades;
    }

    public static int CountEnemiesNextToObstacles(
        RoomMatrix roomMatrix)
    {
        int enemies = 0;
        foreach (Position obstaclePosition in roomMatrix.ObstaclesPositions)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = obstaclePosition.Move(direction);
                if (roomMatrix.Values.IsPositionWithinBounds(adjacentPosition.X, adjacentPosition.Y) && roomMatrix.EnemiesPositions.Contains(adjacentPosition))
                {
                    enemies++;
                }
            }
        }

        return enemies;
    }

    public static double AverageDistanceFromDoorsToEnemies(HashSet<Position> enemiesPositions)
    {
        List<double> averagesDistances = new();
        foreach (Position doorPosition in GeneticAlgorithmConstants.ROOM.DoorsPositions)
        {
            List<int> averagesDistancesFromDoorPosition = new();
            foreach (Position enemyPosition in enemiesPositions)
            {
                averagesDistancesFromDoorPosition.Add(Utils.CalculateDistance(doorPosition, enemyPosition));
            }
            averagesDistances.Add(averagesDistancesFromDoorPosition.Average());
        }
        return averagesDistances.Average();
    }

    public static int MinimumDistanceBetweenDoorsAndEnemies(HashSet<Position> enemiesPositions)
    {
        int minDistance = int.MaxValue;
        foreach (Position doorPosition in GeneticAlgorithmConstants.ROOM.DoorsPositions)
        {
            minDistance = Math.Min(minDistance, enemiesPositions.Min(enemyPosition => Utils.CalculateDistance(doorPosition, enemyPosition)));
        }
        return minDistance;
    }
}
