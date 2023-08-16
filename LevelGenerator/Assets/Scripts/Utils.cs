using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class EnumExtensions
{
    public static T GetAttribute<T>(this Enum enumValue) where T : Attribute
    {
        var type = enumValue.GetType();
        var memberInfo = type.GetMember(enumValue.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
        return (T)attributes[0];
    }
}

public static class Utils
{
    public static int CountEnemiesNextToObstacles(Possibilidades[,] matrix)
    {
        List<Tuple<int, int>> obstaclesPositions = GetPositionsThatHas(matrix, typeof(Obstacles));
        IEnumerable<Enum> enemiesValues = Enum.GetValues(typeof(Enemies)).Cast<Enum>();

        // acima, abaixo, esquerda, direita
        int[] directionRow = { -1, 1, 0, 0 };
        int[] directionColumn = { 0, 0, -1, 1 };

        int enemies = 0;
        foreach (var obstaclePosition in obstaclesPositions)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = obstaclePosition.Item1 + directionRow[i];
                int ny = obstaclePosition.Item2 + directionColumn[i];

                if (nx < 0 || ny < 0 || nx >= matrix.GetLength(0) || ny >= matrix.GetLength(1))
                {
                    continue;
                }

                if (enemiesValues.Any(value => matrix[nx, ny].ToString() == value.ToString()))
                {
                    enemies++;
                }
            }
        }

        return enemies;
    }

    public static float CalculateAverage(List<int> values)
    {
        if (values.Count == 0)
            return 0;

        int sum = 0;
        foreach (int value in values)
        {
            sum += value;
        }

        return (float)sum / values.Count;
    }

    public static List<int> CountGroups(Possibilidades[,] matrix, Type enumType)
    {
        bool[,] visitado = new bool[matrix.GetLength(0), matrix.GetLength(1)];

        IEnumerable<Enum> enumValues = Enum.GetValues(enumType).Cast<Enum>();

        List<int> tamanhosGrupos = new();
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                if (!visitado[row, col] && enumValues.Any(value => matrix[row, col].ToString() == value.ToString()))
                {
                    int tamanhoGrupo = CountGroup(matrix, visitado, enumValues, row, col);
                    tamanhosGrupos.Add(tamanhoGrupo);
                }
            }
        }

        return tamanhosGrupos;
    }

    static int CountGroup(Possibilidades[,] matriz, bool[,] visited, IEnumerable<Enum> enumValues, int row, int col)
    {
        if (row < 0 || row >= matriz.GetLength(0) || col < 0 || col >= matriz.GetLength(1) || visited[row, col] || enumValues.All(value => matriz[row, col].ToString() != value.ToString()))
            return 0;

        visited[row, col] = true;

        int tamanhoAtual = 1;

        tamanhoAtual += CountGroup(matriz, visited, enumValues, row - 1, col);
        tamanhoAtual += CountGroup(matriz, visited, enumValues, row + 1, col);
        tamanhoAtual += CountGroup(matriz, visited, enumValues, row, col - 1);
        tamanhoAtual += CountGroup(matriz, visited, enumValues, row, col + 1);

        return tamanhoAtual;
    }

    public static int CountOccurrences(Possibilidades[,] matriz, Type enumType)
    {
        var enumValues = Enum.GetValues(enumType).Cast<Enum>();

        int count = 0;

        for (int i = 0; i < matriz.GetLength(0); i++)
        {
            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                if (enumValues.Any(value => matriz[i, j].ToString() == value.ToString()))
                {
                    count++;
                }
            }
        }
        return count;
    }

    static List<Tuple<int, int>> GetPositionsThatHas(Possibilidades[,] matrix, Type enumType)
    {
        List<Tuple<int, int>> positionsHas = new();

        var enumValues = Enum.GetValues(enumType).Cast<Enum>();

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (enumValues.Any(value => matrix[i, j].ToString() == value.ToString()))
                {
                    positionsHas.Add(new Tuple<int, int>(i, j));
                }
            }
        }
        return positionsHas;
    }

    public static Tuple<int, int> ChooseLocationFreeFrom(Possibilidades[,] matriz, List<Tuple<int, int>> changeablesPositions, Possibilidades freeFrom)
    {
        List<Tuple<int, int>> positionsFreeFrom = new();
        foreach (Tuple<int, int> position in changeablesPositions)
        {
            if (!matriz[position.Item1, position.Item2].Equals(freeFrom))
            {
                positionsFreeFrom.Add(position);
            }
        }

        if (positionsFreeFrom.Count == 0)
        {
            return new Tuple<int, int>(-1, -1);
        }
        int idx = Random.Range(0, positionsFreeFrom.Count);
        return positionsFreeFrom[idx];
    }

    public static Tuple<int, int> ChooseLocationThatHas(Possibilidades[,] matriz, List<Tuple<int, int>> changeablesPositions, Type enumType)
    {
        List<Tuple<int, int>> positionsHas = new();

        var enumValues = Enum.GetValues(enumType).Cast<Enum>();
        foreach (Tuple<int, int> position in changeablesPositions)
        {
            if (enumValues.Any(value => matriz[position.Item1, position.Item2].ToString() == value.ToString()))
            {
                positionsHas.Add(position);
            }
        }

        if (positionsHas.Count == 0)
        {
            return new Tuple<int, int>(-1, -1);
        }
        int idx = Random.Range(0, positionsHas.Count);
        return positionsHas[idx];
    }

    public static bool IsPathBetweenDoorAndEnemies(Possibilidades[,] roomMatrix, List<Tuple<int, int>> doorsPositions)
    {
        List<Tuple<int, int>> enemiesPositions = GetPositionsThatHas(roomMatrix, typeof(Enemies));
        if (enemiesPositions.Count == 0)
        {
            return false;
        }

        int[,] matriz = TransformRoomForCountPaths(roomMatrix);

        // so preciso ver de uma porta pra todos os inimigos, pq se tiver de uma tem da outra, ja que eu conto os caminhos de uma porta a outra
        // TODO: if inimigo nao for voador, se for voador nao precisa verificar se tem caminho pra ele eu acho
        foreach (Tuple<int, int> enemiePosition in enemiesPositions)
        {
            int[,] paths = CountPaths(matriz, doorsPositions[0]);
            if (paths[enemiePosition.Item1, enemiePosition.Item2] == 0)
            {
                return false;
            }
        }

        return true;
    }

    public static List<int> ResolveKnapsack(List<int> values, int capacity)
    {
        int n = values.Count;
        int[] dp = new int[capacity + 1];
        List<int>[] chosenItems = new List<int>[capacity + 1];
        for (int i = 0; i <= capacity; i++)
        {
            chosenItems[i] = new List<int>();
        }

        for (int w = 1; w <= capacity; w++)
        {
            int maxValue = dp[w];
            List<int> chosenItem = new();

            for (int i = 0; i < n; i++)
            {
                if (values[i] <= w)
                {
                    int newValue = dp[w - values[i]] + values[i];
                    // se o valor for melhor ou se o valor for igual e tiver menos itens, eu coloco
                    if (newValue > maxValue || (newValue == maxValue && chosenItems[w - values[i]].Count + 1 < chosenItem.Count))
                    {
                        maxValue = newValue;
                        chosenItem = new List<int>(chosenItems[w - values[i]]) { i };
                    }
                }
            }

            dp[w] = maxValue;
            chosenItems[w] = chosenItem;
        }

        return chosenItems[capacity];
    }

    public static int CountPathsBetweenDoors(Possibilidades[,] roomMatrix, List<Tuple<int, int>> doorsPositions)
    {
        // transformar 
        int[,] matriz = TransformRoomForCountPaths(roomMatrix);

        int qntCaminhos = 0;
        for (int i = 0; i < doorsPositions.Count; i++)
        {
            for (int j = i + 1; j < doorsPositions.Count; j++)
            {
                //Console.WriteLine("PORTA: " + i + " x " + j);
                int[,] paths = CountPaths(matriz, doorsPositions[i]);

                if (paths[doorsPositions[j].Item1, doorsPositions[j].Item2] == 0)
                {
                    return int.MinValue;
                }
                qntCaminhos += paths[doorsPositions[j].Item1, doorsPositions[j].Item2];
            }
        }

        return qntCaminhos;
    }

    private static int[,] TransformRoomForCountPaths(Possibilidades[,] roomMatrix)
    {
        int[,] matriz = new int[roomMatrix.GetLength(0), roomMatrix.GetLength(1)];

        for (int i = 0; i < matriz.GetLength(0); i++)
        {
            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                if (roomMatrix[i, j].GetAttribute<UltrapassavelAttribute>().IsUltrapassavel)
                {
                    matriz[i, j] = 1;
                }
                else
                {
                    matriz[i, j] = 0;
                }
            }
        }
        return matriz;
    }

    private static int[,] CountPaths(int[,] matrix, Tuple<int, int> initialPosition)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        int[,] countPath = new int[rows, cols];

        Queue<(int, int)> queue = new();

        // Adicionar o ponto de partida à fila
        queue.Enqueue((initialPosition.Item1, initialPosition.Item2));
        countPath[initialPosition.Item1, initialPosition.Item2] = 1;

        // acima, abaixo, esquerda, direita
        int[] directionRow = { -1, 1, 0, 0 };
        int[] directionColumn = { 0, 0, -1, 1 };

        // BFS
        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int nx = x + directionRow[i];
                int ny = y + directionColumn[i];

                // Verificar se a próxima posição está dentro dos limites da matriz e é acessível
                if (nx >= 0 && ny >= 0 && nx < rows && ny < cols && matrix[nx, ny] == 1)
                {
                    // Se ainda não foi visitada, adicionar à fila
                    if (countPath[nx, ny] == 0)
                    {
                        queue.Enqueue((nx, ny));
                    }

                    countPath[nx, ny] += countPath[x, y];
                }
            }
        }

        return countPath;
    }

    public static int CalculateDistanceToRange(int min, int max, int number)
    {
        int distanceToRange;
        if (number > max) distanceToRange = max - number;
        else if (number < min) distanceToRange = number - min;
        else distanceToRange = 0;

        return distanceToRange;
    }
}
