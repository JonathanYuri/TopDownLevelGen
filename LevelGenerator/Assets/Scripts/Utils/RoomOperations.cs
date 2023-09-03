using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public static class RoomOperations
{
    // TODO: Tirar o nome matriz para Room
    public static Dictionary<RoomContents, List<Position>> GroupPositionsByRoomValue(RoomContents[,] matrix, HashSet<Position> positionsOf)
    {
        // agrupar as posicoes com base no valor da matrix na posicao
        IEnumerable<IGrouping<RoomContents, Position>> groupedPositions = positionsOf.GroupBy(p => matrix[p.Row, p.Column]);

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
        foreach (var obstaclePosition in obstaclesPositions)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = obstaclePosition.Move(direction);
                if (matrix.IsPositionWithinBounds(adjacentPosition))
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
}
