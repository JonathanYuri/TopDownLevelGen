using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public static class ISetExtensions
{
    public static T GetRandomElement<T>(this ISet<T> allElements)
    {
        if (allElements == null || allElements.Count == 0)
            throw new ArgumentException("The collection is empty or null.", nameof(allElements));

        int idx = Random.Range(0, allElements.Count);
        return allElements.ElementAt(idx);
    }
}
