using System;
using System.Collections.Generic;

/// <summary>
/// A static class that provides methods for solving the Knapsack problem to select items.
/// </summary>
public static class Knapsack
{
    public static KnapsackSelectionResult ChooseEnemiesAndObstaclesToKnapsack(
        List<RoomContents> enemies, List<int> enemiesDifficulty,
        List<RoomContents> obstacles, List<int> obstaclesDifficulty)
    {
        int[] chosenIdxEnemies = enemies.RandomlySelectDistinctIndexes();
        int[] chosenIdxObstacles = obstacles.RandomlySelectDistinctIndexes();

        return new KnapsackSelectionResult(
            enemies.GetElementsByIndexes(chosenIdxEnemies), enemiesDifficulty.GetElementsByIndexes(chosenIdxEnemies),
            obstacles.GetElementsByIndexes(chosenIdxObstacles), obstaclesDifficulty.GetElementsByIndexes(chosenIdxObstacles));
    }

    /// <summary>
    /// Resolves the Knapsack problem for selecting contents based on their values and a capacity constraint.
    /// </summary>
    /// <param name="contents">The list of contents to choose from.</param>
    /// <param name="contentsValues">The list of values associated with each content.</param>
    /// <param name="contentsCapacity">The maximum capacity for selecting contents.</param>
    /// <returns>An array of selected contents based on the Knapsack problem solution.</returns>
    public static RoomContents[] ResolveKnapsack(KnapsackParams knapsackParams)
    {
        List<int> chosenContentsIdx = ResolveKnapsack(
            knapsackParams.ContentsValues,
            knapsackParams.ContentsCapacity,
            KnapsackCriteriaSet.BestAndRandomCriteria);

        RoomContents[] chosenContents = new RoomContents[chosenContentsIdx.Count];
        for (int i = 0; i < chosenContentsIdx.Count; i++)
        {
            int idx = chosenContentsIdx[i];
            chosenContents[i] = knapsackParams.Contents[idx];
        }

        //Debug.Log("Obstaculos escolhidos: " + string.Join(", ", chosenObstacles));
        return chosenContents;
    }

    /// <summary>
    /// Resolves the Knapsack problem for selecting items based on their values and a capacity constraint.
    /// </summary>
    /// <param name="values">The list of values associated with each item.</param>
    /// <param name="capacity">The maximum capacity for selecting items.</param>
    /// <returns>A list of indices representing the selected items based on the Knapsack problem solution.</returns>
    public static List<int> ResolveKnapsack(in List<int> values, in int capacity, Func<int, int, List<int>, List<int>, bool> criteria)
    {
        int n = values.Count;
        int[] dp = new int[capacity + 1];
        List<int>[] chosenItems = new List<int>[capacity + 1];
        for (int i = 0; i <= capacity; i++)
        {
            chosenItems[i] = new List<int>();
        }

        for (int w = 1; w <= capacity; w++)
        {
            int maxValue = dp[w];
            List<int> chosenItem = new();

            for (int i = 0; i < n; i++)
            {
                if (values[i] <= w)
                {
                    int newValue = dp[w - values[i]] + values[i];
                    // se o valor for melhor ou se o valor for igual e tiver menos itens, eu coloco
                    if (criteria(newValue, maxValue, chosenItems[w - values[i]], chosenItem))
                    {
                        maxValue = newValue;
                        chosenItem = new List<int>(chosenItems[w - values[i]]) { i };
                    }
                }
            }

            dp[w] = maxValue;
            chosenItems[w] = chosenItem;
        }

        return chosenItems[capacity];
    }
}
