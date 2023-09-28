using System;
using System.Collections.Generic;
using UnityEngine;

public static class GroupCounter
{
    public static List<int> CountGroups(RoomContents[,] matrix, HashSet<Position> positions)
    {
        bool[,] visitado = new bool[matrix.GetLength(0), matrix.GetLength(1)];

        List<int> tamanhosGrupos = new();
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                if (!visitado[row, col] && positions.Contains(new Position { X = row, Y = col }))
                {
                    int tamanhoGrupo = CountGroup(matrix, visitado, positions, row, col);
                    tamanhosGrupos.Add(tamanhoGrupo);
                }
            }
        }

        return tamanhosGrupos;
    }

    static int CountGroup(RoomContents[,] matriz, bool[,] visited, HashSet<Position> positions, int row, int col)
    {
        if (!matriz.IsPositionWithinBounds(row, col) || visited[row, col] || !positions.Contains(new Position { X = row, Y = col }))
            return 0;

        visited[row, col] = true;

        int tamanhoAtual = 1;

        tamanhoAtual += CountGroup(matriz, visited, positions, row - 1, col);
        tamanhoAtual += CountGroup(matriz, visited, positions, row + 1, col);
        tamanhoAtual += CountGroup(matriz, visited, positions, row, col - 1);
        tamanhoAtual += CountGroup(matriz, visited, positions, row, col + 1);

        return tamanhoAtual;
    }
}
