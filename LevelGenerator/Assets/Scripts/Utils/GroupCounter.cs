using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A static class that provides methods for counting groups of connected positions in a matrix.
/// </summary>
public static class GroupCounter
{
    /// <summary>
    /// Counts the sizes of connected groups of positions in a matrix.
    /// </summary>
    /// <param name="matrix">The matrix of room contents.</param>
    /// <param name="positions">The set of positions to count groups in.</param>
    /// <returns>A list of group sizes.</returns>
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

    /// <summary>
    /// Recursively counts the size of a connected group of positions starting from a given position.
    /// </summary>
    /// <param name="matrix">The matrix of room contents.</param>
    /// <param name="visited">The set of visited positions.</param>
    /// <param name="positions">The set of positions to count groups in.</param>
    /// <param name="position">The starting position for group counting.</param>
    /// <returns>The size of the connected group starting from the given position.</returns>
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
