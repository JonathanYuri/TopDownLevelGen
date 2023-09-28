using System;
using System.Collections.Generic;
using UnityEngine;

public static class GroupCounter
{
    public static List<int> CountGroups(RoomContents[,] matrix, HashSet<Position> positions)
    {
        HashSet<Position> visited = new();
        List<int> groupSizes = new();

        foreach (Position position in positions)
        {
            if (!visited.Contains(position))
            {
                int groupSize = CountGroupSize(matrix, visited, positions, position);
                groupSizes.Add(groupSize);
            }
        }

        return groupSizes;
    }

    static int CountGroupSize(RoomContents[,] matriz, HashSet<Position> visited, HashSet<Position> positions, Position position)
    {
        if (!matriz.IsPositionWithinBounds(position) || visited.Contains(position) || !positions.Contains(position))
            return 0;

        visited.Add(position);

        int tamanhoAtual = 1;

        tamanhoAtual += CountGroupSize(matriz, visited, positions, position.Move(Direction.Down));
        tamanhoAtual += CountGroupSize(matriz, visited, positions, position.Move(Direction.Up));
        tamanhoAtual += CountGroupSize(matriz, visited, positions, position.Move(Direction.Left));
        tamanhoAtual += CountGroupSize(matriz, visited, positions, position.Move(Direction.Right));

        return tamanhoAtual;
    }
}
