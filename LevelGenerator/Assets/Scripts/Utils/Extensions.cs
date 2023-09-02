using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

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

public static class IEnumerableExtensions
{
    public static T[] Shuffle<T>(this IEnumerable<T> collection)
    {
        T[] shuffled = collection.ToArray();
        int n = shuffled.Length;

        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (shuffled[n], shuffled[k]) = (shuffled[k], shuffled[n]);
        }

        return shuffled;
    }

    public static T[] SelectRandomDistinctElements<T>(this IEnumerable<T> allelements, int numberOfPositionsToSelect)
    {
        T[] selectedElements = new T[numberOfPositionsToSelect];

        List<T> allElementsList = new(allelements);
        allElementsList.Shuffle();

        for (int i = 0; i < numberOfPositionsToSelect; i++)
        {
            selectedElements[i] = allElementsList[i];
        }

        return selectedElements;
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