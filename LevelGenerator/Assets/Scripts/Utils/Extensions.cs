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

    public static T[] SelectRandomDistinctElements<T>(this IEnumerable<T> allElements, int numberToSelect)
    {
        if (numberToSelect >= allElements.Count())
        {
            return allElements.ToArray();
        }

        List<T> allelements = new(allElements);
        T[] selectedElements = new T[numberToSelect];

        for (int i = 0; i < numberToSelect; i++)
        {
            int k = Random.Range(0, allelements.Count);
            selectedElements[i] = allelements[k];
            allelements.RemoveAt(k);
        }

        return selectedElements;
    }

    public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

        TSource maxItem = default;
        TKey maxKey = default;

        bool firstItem = true;
        foreach (var item in source)
        {
            var itemKey = keySelector(item);

            if (firstItem || Comparer<TKey>.Default.Compare(itemKey, maxKey) > 0)
            {
                maxItem = item;
                maxKey = itemKey;
                firstItem = false;
            }
        }

        if (firstItem)
            throw new InvalidOperationException("A sequencia esta vazia.");

        return maxItem;
    }
}

public static class MatrixExtensions
{
    public static bool IsPositionWithinBounds<T>(this T[,] matrix, int row, int col)
    {
        return row >= 0 && row < matrix.GetLength(0) &&
               col >= 0 && col < matrix.GetLength(1);
    }

    public static bool IsPositionWithinBounds<T>(this T[,] matrix, Position position) => IsPositionWithinBounds(matrix, position.X, position.Y);
}
