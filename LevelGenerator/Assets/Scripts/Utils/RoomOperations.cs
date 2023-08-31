using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public static class RoomOperations
{
    public static List<Position> GetPositionsThatHas(Possibilidades[,] matrix, Type enumType)
    {
        List<Position> positionsHas = new();

        HashSet<string> enumValueStrings = Utils.GetEnumValueStrings(enumType);

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (enumValueStrings.Contains(matrix[i, j].ToString()))
                {
                    positionsHas.Add(new Position { Row = i, Column = j });
                }
            }
        }
        return positionsHas;
    }

    public static Position ChooseLocationThatHas(Possibilidades[,] matrix, Type enumType)
    {
        List<Position> positionsHas = GetPositionsThatHas(matrix, enumType);

        if (positionsHas.Count == 0)
        {
            return new Position { Row = -1, Column = -1 };
        }
        int idx = Random.Range(0, positionsHas.Count);
        return positionsHas[idx];
    }

    public static Position ChooseLocationFreeFrom(Possibilidades[,] matrix, List<Position> changeablesPositions, Possibilidades freeFrom)
    {
        List<Position> positionsFreeFrom = new();
        foreach (Position position in changeablesPositions)
        {
            if (!matrix[position.Row, position.Column].Equals(freeFrom))
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

    public static int CountEnemiesNextToObstacles(Possibilidades[,] matrix)
    {
        List<Position> obstaclesPositions = GetPositionsThatHas(matrix, typeof(Obstacles));
        HashSet<string> enumValueStrings = Utils.GetEnumValueStrings(typeof(Enemies));

        int enemies = 0;
        foreach (var obstaclePosition in obstaclesPositions)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = obstaclePosition.Move(direction);
                if (Utils.IsPositionWithinBounds(matrix, adjacentPosition))
                {
                    if (enumValueStrings.Contains(matrix[adjacentPosition.Row, adjacentPosition.Column].ToString()))
                    {
                        enemies++;
                    }
                }
            }
        }

        return enemies;
    }

    public static int CountOccurrences(Possibilidades[,] matriz, Type enumType)
    {
        HashSet<string> enumValueStrings = Utils.GetEnumValueStrings(enumType);

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

    public static bool HasTheRightObjects(Possibilidades[,] matrix, Type obj, List<Position> objectsPositions, List<object> values)
    {
        var enumValues = Enum.GetValues(obj).Cast<object>();

        List<object> objs = new();
        foreach (Position position in objectsPositions)
        {
            // pegar o inimigo equivalente no enum de Possibilidades // TODO: separar esse codigo em funcao, usa bastante, na GenerateRoomRandomly ja usa isso
            foreach (object value in enumValues)
            {
                if (matrix[position.Row, position.Column].ToString() == value.ToString())
                {
                    objs.Add(value);
                }
            }
        }

        objs.Sort();
        List<object> aux = new(values);
        aux.Sort();

        //Debug.Log(obj.ToString());
        //Debug.Log("era pra ter: " + string.Join(", ", aux));
        //Debug.Log("tem: " + string.Join(", ", objs));

        return objs.SequenceEqual(aux); // as listas sao iguais?
    }
}
