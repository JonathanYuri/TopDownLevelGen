using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public static class RoomOperations
{
    // TODO: Tirar o nome Possibilidades para MatrixValue eh mais bonito
    public static Dictionary<Possibilidades, List<Position>> GroupPositionsByMatrixValue(Possibilidades[,] matrix, HashSet<Position> positionsOf)
    {
        // agrupar as posicoes com base no valor da matrix na posicao
        IEnumerable<IGrouping<Possibilidades, Position>> groupedPositions = positionsOf.GroupBy(p => matrix[p.Row, p.Column]);

        // transformar o groupedPositions em dicionario
        Dictionary<Possibilidades, List<Position>> positionsByPossibilidades = groupedPositions.ToDictionary(group => group.Key, group => group.ToList());

        return positionsByPossibilidades;
    }

    public static int CountEnemiesNextToObstacles(
        Possibilidades[,] matrix,
        HashSet<Position> obstaclesPositions,
        Dictionary<Enemies, Possibilidades> enemiesToPossibilidades)
    {
        int enemies = 0;
        foreach (var obstaclePosition in obstaclesPositions)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = obstaclePosition.Move(direction);
                if (matrix.IsPositionWithinBounds(adjacentPosition))
                {
                    if (enemiesToPossibilidades.ContainsValue(matrix[adjacentPosition.Row, adjacentPosition.Column]))
                    {
                        enemies++;
                    }
                }
            }
        }

        return enemies;
    }
}
