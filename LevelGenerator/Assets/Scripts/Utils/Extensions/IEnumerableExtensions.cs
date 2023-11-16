using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// Provides extension methods for working with IEnumerable collections.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Shuffles the elements in the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to shuffle.</param>
    /// <returns>The shuffled collection.</returns>
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

    /// <summary>
    /// Selects a specified number of random distinct elements from the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="allElements">The collection to select elements from.</param>
    /// <param name="numberToSelect">The number of elements to select.</param>
    /// <returns>An array containing the selected random elements.</returns>
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

    public static List<T> GetElementsByIndexes<T>(this IEnumerable<T> allElements, IEnumerable<int> indexes)
    {
        List<T> selectedElements = indexes.Select(index => allElements.ElementAtOrDefault(index)).ToList();
        return selectedElements;
    }

    /// <summary>
    /// Finds the element in the collection with the maximum key as determined by the specified selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the collection.</typeparam>
    /// <typeparam name="TKey">The type of the key used for comparison.</typeparam>
    /// <param name="source">The collection to search.</param>
    /// <param name="keySelector">A function to extract a key from each element.</param>
    /// <returns>The element with the maximum key.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
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

    /// <summary>
    /// Finds the index of the element in the collection with the maximum key as determined by the specified selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the collection.</typeparam>
    /// <typeparam name="TKey">The type of the key used for comparison.</typeparam>
    /// <param name="source">The collection to search.</param>
    /// <param name="keySelector">A function to extract a key from each element.</param>
    /// <returns>The index of the element with the maximum key.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static int IndexOfMax<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

        TKey maxKey = default;
        int maxIndex = -1;

        bool firstItem = true;
        int currentIndex = 0;
        foreach (var item in source)
        {
            var itemKey = keySelector(item);

            if (firstItem || Comparer<TKey>.Default.Compare(itemKey, maxKey) > 0)
            {
                maxKey = itemKey;
                maxIndex = currentIndex;
                firstItem = false;
            }
            currentIndex++;
        }

        if (firstItem)
            throw new InvalidOperationException("A sequencia esta vazia.");

        return maxIndex;
    }

    /// <summary>
    /// Finds the elements in the collection with the maximum key and minimum key as determined by the specified selector function.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static (TKey, TKey) MaxAndMin<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

        bool firstItem = true;
        TKey maxKey = default;
        TKey minKey = default;

        foreach (var item in source)
        {
            var itemKey = keySelector(item);

            if (firstItem || Comparer<TKey>.Default.Compare(itemKey, maxKey) > 0)
            {
                maxKey = itemKey;
            }

            if (firstItem || Comparer<TKey>.Default.Compare(itemKey, minKey) < 0)
            {
                minKey = itemKey;
            }

            firstItem = false;
        }

        if (firstItem)
            throw new InvalidOperationException("A sequência está vazia.");

        return (maxKey, minKey);
    }
}

