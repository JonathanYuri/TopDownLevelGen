using System;
using System.Collections.Generic;

public static class GroupCounter
{
    public static List<int> CountGroups(Possibilidades[,] matrix, Type enumType)
    {
        bool[,] visitado = new bool[matrix.GetLength(0), matrix.GetLength(1)];

        HashSet<string> enumValueStrings = Utils.GetEnumValueStrings(enumType);
        List<int> tamanhosGrupos = new();
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                if (!visitado[row, col] && enumValueStrings.Contains(matrix[row, col].ToString()))
                {
                    int tamanhoGrupo = CountGroup(matrix, visitado, enumValueStrings, row, col);
                    tamanhosGrupos.Add(tamanhoGrupo);
                }
            }
        }

        return tamanhosGrupos;
    }

    static int CountGroup(Possibilidades[,] matriz, bool[,] visited, HashSet<string> enumValueStrings, int row, int col)
    {
        if (row < 0 || row >= matriz.GetLength(0) || col < 0 || col >= matriz.GetLength(1) || visited[row, col] || !enumValueStrings.Contains(matriz[row, col].ToString()))
            return 0;

        visited[row, col] = true;

        int tamanhoAtual = 1;

        tamanhoAtual += CountGroup(matriz, visited, enumValueStrings, row - 1, col);
        tamanhoAtual += CountGroup(matriz, visited, enumValueStrings, row + 1, col);
        tamanhoAtual += CountGroup(matriz, visited, enumValueStrings, row, col - 1);
        tamanhoAtual += CountGroup(matriz, visited, enumValueStrings, row, col + 1);

        return tamanhoAtual;
    }
}
