using System.Collections.Generic;
using System;
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

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}

public static class HashSetExtensions
{
    public static List<Position> SelectRandomPositions(this HashSet<Position> allPositions, int numberOfPositionsToSelect)
    {
        List<Position> selectedPositions = new();

        List<Position> allPositionsList = new(allPositions);
        allPositionsList.Shuffle();

        for (int i = 0; i < numberOfPositionsToSelect; i++)
        {
            selectedPositions.Add(allPositionsList[i]);
        }

        return selectedPositions;
    }
}

public static class MatrixExtensions
{
    public static bool IsPositionWithinBounds<T>(this T[,] matrix, Position position)
    {
        return position.Row >= 0 && position.Row < matrix.GetLength(0) &&
                position.Column >= 0 && position.Column < matrix.GetLength(1);
    }
}
