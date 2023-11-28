using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

/// <summary>
/// Provides extension methods for working with enums.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets a custom attribute of the specified type applied to an enum value.
    /// </summary>
    /// <typeparam name="T">The type of attribute to retrieve.</typeparam>
    /// <param name="enumValue">The enum value.</param>
    /// <returns>The attribute of the specified type applied to the enum value.</returns>
    public static T GetAttribute<T>(this Enum enumValue) where T : Attribute
    {
        var type = enumValue.GetType();
        var memberInfo = type.GetMember(enumValue.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
        return (T)attributes[0];
    }
}

/// <summary>
/// Provides extension methods for working with matrices.
/// </summary>
public static class MatrixExtensions
{
    /// <summary>
    /// Checks whether a specified position is within the bounds of the matrix.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>
    /// <param name="matrix">The matrix to check against.</param>
    /// <param name="row">The row index of the position.</param>
    /// <param name="col">The column index of the position.</param>
    /// <returns>True if the position is within the matrix bounds; otherwise, false.</returns>
    public static bool IsPositionWithinBounds<T>(this T[,] matrix, int row, int col)
    {
        return row >= 0 && row < matrix.GetLength(0) &&
               col >= 0 && col < matrix.GetLength(1);
    }

    /// <summary>
    /// Checks whether a specified position is within the bounds of the matrix.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>
    /// <param name="matrix">The matrix to check against.</param>
    /// <param name="position">The position to check.</param>
    /// <returns>True if the position is within the matrix bounds; otherwise, false.</returns>
    public static bool IsPositionWithinBounds<T>(this T[,] matrix, Position position) => IsPositionWithinBounds(matrix, position.X, position.Y);
}

public static class DictionaryExtensions
{
    public static Dictionary<TValue, TKey> InvertDictionary<TKey, TValue>(this Dictionary<TKey, TValue> originalDictionary)
    {
        Dictionary<TValue, TKey> invertedDictionary = new();
        foreach (var kvp in originalDictionary)
        {
            invertedDictionary[kvp.Value] = kvp.Key;
        }

        return invertedDictionary;
    }
}

public static class IListExtensions
{
    /// <summary>
    /// Selects a specified number of random distinct elements from the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="allElements">The collection to select elements from.</param>
    /// <param name="numberToSelect">The number of elements to select.</param>
    /// <returns>An array containing the selected random elements.</returns>
    static int[] SelectRandomDistinctIndexes<T>(this IList<T> allElements, int numberToSelect)
    {
        if (numberToSelect >= allElements.Count())
        {
            return allElements.Select((element, index) => index).ToArray();
        }

        List<T> allelements = new(allElements);
        int[] selectedIndexes = new int[numberToSelect];

        for (int i = 0; i < numberToSelect; i++)
        {
            int k = Random.Range(0, allelements.Count);
            selectedIndexes[i] = k;
            allelements.RemoveAt(k);
        }

        return selectedIndexes;
    }

    public static int[] RandomlySelectDistinctIndexes<T>(this IList<T> allElements)
    {
        int totalToSelect = Random.Range(1, allElements.Count + 1);
        return allElements.SelectRandomDistinctIndexes(totalToSelect);
    }
}