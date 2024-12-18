using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public struct SubsetSumSelectionResult
{
    public List<RoomContents> ChosenEnemies { get; private set; }
    public List<int> ChosenEnemiesDifficulty { get; private set; }
    public List<RoomContents> ChosenObstacles { get; private set; }
    public List<int> ChosenObstaclesDifficulty { get; private set; }

    public SubsetSumSelectionResult(List<RoomContents> chosenEnemies, List<int> chosenEnemiesDifficulty,
        List<RoomContents> chosenObstacles, List<int> chosenObstaclesDifficulty)
    {
        ChosenEnemies = chosenEnemies;
        ChosenEnemiesDifficulty = chosenEnemiesDifficulty;
        ChosenObstacles = chosenObstacles;
        ChosenObstaclesDifficulty = chosenObstaclesDifficulty;
    }
}

public struct SubsetSumParams
{
    public List<RoomContents> Contents { get; private set; }
    public List<int> ContentsValues { get; private set; }
    public int ContentsCapacity { get; private set; }
    public SubsetSumParams(List<RoomContents> contents, List<int> contentsValues, int contentsCapacity)
    {
        Contents = contents;
        ContentsValues = contentsValues;
        ContentsCapacity = contentsCapacity;
    }
}

public static class SubsetSumCriteriaSet
{
    public static Func<int, int, List<int>, List<int>, bool> BestCriteria { get; } = BestCriteriaFunction;
    public static Func<int, int, List<int>, List<int>, bool> BestAndSmallerCriteria { get; } = BestAndSmallerCriteriaFunction;
    public static Func<int, int, List<int>, List<int>, bool> BestAndRandomCriteria { get; } = BestAndRandomCriteriaFunction;

    static bool BestAndSmallerCriteriaFunction(int newValue, int maxValue, List<int> chosenItemsWithRemainingCapacity, List<int> chosenItems)
    {
        if (BestCriteriaFunction(newValue, maxValue, chosenItemsWithRemainingCapacity, chosenItems))
        {
            return true;
        }

        int chosenItemsWithNewValue = chosenItemsWithRemainingCapacity.Count + 1;

        // se as duas solucoes forem iguais eu escolho a que tem menos items
        return (newValue == maxValue && chosenItemsWithNewValue < chosenItems.Count);
    }

    static bool BestCriteriaFunction(int newValue, int maxValue, List<int> chosenItemsWithRemainingCapacity, List<int> chosenItems)
    {
        return newValue > maxValue;
    }

    static bool BestAndRandomCriteriaFunction(int newValue, int maxValue, List<int> chosenItemsWithRemainingCapacity, List<int> chosenItems)
    {
        if (BestCriteriaFunction(newValue, maxValue, chosenItemsWithRemainingCapacity, chosenItems))
        {
            return true;
        }

        bool random = Random.Range(0, 2) == 0;

        // se as duas solucoes forem iguais eu escolho uma das duas aleatoriamente
        return (newValue == maxValue && random);
    }
}
