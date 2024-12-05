using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SubsetSumSolver
{
    public static SubsetSumSelectionResult ChooseEnemiesAndObstaclesToKnapsack(
        List<RoomContents> enemies, List<int> enemiesDifficulty,
        List<RoomContents> obstacles, List<int> obstaclesDifficulty)
    {
        int[] chosenIdxEnemies = enemies.RandomlySelectDistinctIndexes();
        int[] chosenIdxObstacles = obstacles.RandomlySelectDistinctIndexes();

        return new SubsetSumSelectionResult(
            enemies.GetElementsByIndexes(chosenIdxEnemies), enemiesDifficulty.GetElementsByIndexes(chosenIdxEnemies),
            obstacles.GetElementsByIndexes(chosenIdxObstacles), obstaclesDifficulty.GetElementsByIndexes(chosenIdxObstacles));
    }

    public static RoomContents[] ResolveSubsetSum(SubsetSumParams subsetSumParams)
    {
        List<int> chosenContentsIdx = ResolveSubsetSum(
            subsetSumParams.ContentsValues,
            subsetSumParams.ContentsCapacity,
            SubsetSumCriteriaSet.BestAndRandomCriteria);

        RoomContents[] chosenContents = new RoomContents[chosenContentsIdx.Count];
        for (int i = 0; i < chosenContentsIdx.Count; i++)
        {
            int idx = chosenContentsIdx[i];
            chosenContents[i] = subsetSumParams.Contents[idx];
        }

        //Debug.Log("Obstaculos escolhidos: " + string.Join(", ", chosenObstacles));
        return chosenContents;
    }

    public static List<int> ResolveSubsetSum(List<int> values, int target, Func<int, int, List<int>, List<int>, bool> criteria)
    {
        int n = values.Count;
        int[] dp = new int[target + 1];
        List<int>[] chosenItems = new List<int>[target + 1];

        for (int i = 0; i <= target; i++)
        {
            chosenItems[i] = new List<int>();
        }

        for (int w = 1; w <= target; w++)
        {
            int maxValue = dp[w];
            List<int> chosenItem = new();

            for (int i = 0; i < n; i++)
            {
                if (values[i] <= w)
                {
                    int newValue = dp[w - values[i]] + values[i];
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

        return chosenItems[target];
    }
}
