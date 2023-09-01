using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public static class RoomOperations
{
    public static Dictionary<Possibilidades, List<Position>> GetPositionsOf(Possibilidades[,] matrix, HashSet<Position> positionsOf)
    {
        Dictionary<Possibilidades, List<Position>> positionsByEnumType = new();
        foreach (Position p in positionsOf)
        {
            Possibilidades type = matrix[p.Row, p.Column];
            if (positionsByEnumType.ContainsKey(type))
            {
                positionsByEnumType[type].Add(p);
            }
            else
            {
                positionsByEnumType.Add(type, new List<Position> { p });
            }
        }

        return positionsByEnumType;
    }

    public static int CountEnemiesNextToObstacles(Possibilidades[,] matrix, HashSet<Position> obstaclesPositions)
    {
        HashSet<string> enumValueStrings = Utils.GetEnumValueStrings(typeof(Enemies));

        int enemies = 0;
        foreach (var obstaclePosition in obstaclesPositions)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Position adjacentPosition = obstaclePosition.Move(direction);
                if (matrix.IsPositionWithinBounds(adjacentPosition))
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

    public static bool HasTheRightObjects(Possibilidades[,] matrix, Type obj, List<Position> objectsPositions, List<object> values)
    {
        var enumValues = Enum.GetValues(obj).Cast<object>();

        List<object> objs = new();
        foreach (Position position in objectsPositions)
        {
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
