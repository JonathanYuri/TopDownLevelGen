using RoomGeneticAlgorithm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomIndividualExtensions
{
    public static T GetBestIndividual<T>(this IEnumerable<T> allElements)
        where T : RoomIndividual
        => allElements.MaxBy(individual => individual.Value);
}
