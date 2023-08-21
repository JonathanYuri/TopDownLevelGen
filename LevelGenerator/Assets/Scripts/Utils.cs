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

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class Position
{
    public int Row { get; set; }
    public int Column { get; set; }

    public Position Move(Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Position { Row = Row - 1, Column = Column },
            Direction.Down => new Position { Row = Row + 1, Column = Column },
            Direction.Left => new Position { Row = Row, Column = Column - 1 },
            Direction.Right => new Position { Row = Row, Column = Column + 1 },
            Direction _ => throw new ArgumentException("Direção desconhecida: " + direction)
        };
    }
}

public static class Utils
{
    static HashSet<string> GetEnumValueStrings(Type enumType)
    {
        return new HashSet<string>(Enum.GetValues(enumType).Cast<Enum>().Select(value => value.ToString()));
    }

    public static bool IsPositionWithinBounds<T>(T[,] matrix, Position position)
    {
        return position.Row >= 0 && position.Row < matrix.GetLength(0) &&
               position.Column >= 0 && position.Column < matrix.GetLength(1);
    }

    public static int CountEnemiesNextToObstacles(Possibilidades[,] matrix)
    {
        List<Position> obstaclesPositions = GetPositionsThatHas(matrix, typeof(Obstacles));
        IEnumerable<Enum> enemiesValues = Enum.GetValues(typeof(Enemies)).Cast<Enum>();

        int enemies = 0;
        foreach (var obstaclePosition in obstaclesPositions)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = obstaclePosition.Move(direction);
                if (IsPositionWithinBounds(matrix, adjacentPosition))
                {
                    if (enemiesValues.Any(value => matrix[adjacentPosition.Row, adjacentPosition.Column].ToString() == value.ToString()))
                    {
                        enemies++;
                    }
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

        HashSet<string> enumValueStrings = GetEnumValueStrings(enumType);
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

    public static int CountOccurrences(Possibilidades[,] matriz, Type enumType)
    {
        HashSet<string> enumValueStrings = GetEnumValueStrings(enumType);

        int count = 0;
        for (int i = 0; i < matriz.GetLength(0); i++)
        {
            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                if (enumValueStrings.Contains(matriz[i, j].ToString()))
                {
                    count++;
                }
            }
        }
        return count;
    }

    static List<Position> GetPositionsThatHas(Possibilidades[,] matrix, Type enumType)
    {
        List<Position> positionsHas = new();

        HashSet<string> enumValueStrings = GetEnumValueStrings(enumType);

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (enumValueStrings.Contains(matrix[i, j].ToString()))
                {
                    positionsHas.Add(new Position { Row = i, Column = j } );
                }
            }
        }
        return positionsHas;
    }

    public static Position ChooseLocationFreeFrom(Possibilidades[,] matriz, List<Position> changeablesPositions, Possibilidades freeFrom)
    {
        List<Position> positionsFreeFrom = new();
        foreach (Position position in changeablesPositions)
        {
            if (!matriz[position.Row, position.Column].Equals(freeFrom))
            {
                positionsFreeFrom.Add(position);
            }
        }

        if (positionsFreeFrom.Count == 0)
        {
            return new Position { Row = -1, Column = -1 };
        }
        int idx = Random.Range(0, positionsFreeFrom.Count);
        return positionsFreeFrom[idx];
    }

    public static Position ChooseLocationThatHas(Possibilidades[,] matriz, List<Position> changeablesPositions, Type enumType)
    {
        List<Position> positionsHas = new();

        HashSet<string> enumValueStrings = GetEnumValueStrings(enumType);
        foreach (Position position in changeablesPositions)
        {
            if (enumValueStrings.Contains(matriz[position.Row, position.Column].ToString()))
            {
                positionsHas.Add(position);
            }
        }

        if (positionsHas.Count == 0)
        {
            return new Position{ Row = -1, Column = -1 };
        }
        int idx = Random.Range(0, positionsHas.Count);
        return positionsHas[idx];
    }

    public static bool IsPathBetweenDoorAndEnemies(Possibilidades[,] roomMatrix, List<Position> doorsPositions)
    {
        List<Position> enemiesPositions = GetPositionsThatHas(roomMatrix, typeof(Enemies));
        if (enemiesPositions.Count == 0)
        {
            return false;
        }

        int[,] matriz = TransformRoomForCountPaths(roomMatrix);

        // so preciso ver de uma porta pra todos os inimigos, pq se tiver de uma tem da outra, ja que eu conto os caminhos de uma porta a outra
        // TODO: if inimigo nao for voador, se for voador nao precisa verificar se tem caminho pra ele eu acho
        foreach (Position enemiePosition in enemiesPositions)
        {
            int[,] paths = CountPaths(matriz, doorsPositions[0]);
            if (paths[enemiePosition.Row, enemiePosition.Column] == 0)
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

    public static int CountPathsBetweenDoors(Possibilidades[,] roomMatrix, List<Position> doorsPositions)
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

                if (paths[doorsPositions[j].Row, doorsPositions[j].Column] == 0)
                {
                    return int.MinValue;
                }
                qntCaminhos += paths[doorsPositions[j].Row, doorsPositions[j].Column];
            }
        }

        return qntCaminhos;
    }

    static int[,] TransformRoomForCountPaths(Possibilidades[,] roomMatrix)
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

    static int[,] CountPaths(int[,] matrix, Position initialPosition)
    {
        int[,] countPath = new int[matrix.GetLength(0), matrix.GetLength(1)];

        Queue<Position> queue = new();

        // Adicionar o ponto de partida à fila
        queue.Enqueue(initialPosition);
        countPath[initialPosition.Row, initialPosition.Column] = 1;

        // BFS
        while (queue.Count > 0)
        {
            Position position = queue.Dequeue();

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = position.Move(direction);
                if (IsPositionWithinBounds(matrix, adjacentPosition) && matrix[adjacentPosition.Row, adjacentPosition.Column] == 1)
                {
                    // Se ainda não foi visitada, adicionar à fila
                    if (countPath[adjacentPosition.Row, adjacentPosition.Column] == 0)
                    {
                        queue.Enqueue(adjacentPosition);
                    }

                    countPath[adjacentPosition.Row, adjacentPosition.Column] += countPath[position.Row, position.Column];
                }
            }
        }

        return countPath;
    }
}
