using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A static class that provides methods for solving the Knapsack problem to select items.
/// </summary>
public class Knapsack
{
    /// <summary>
    /// Resolves the Knapsack problem for selecting enemies based on their values and a capacity constraint.
    /// </summary>
    /// <param name="enemies">The list of enemies to choose from.</param>
    /// <param name="enemiesValues">The list of values associated with each enemy.</param>
    /// <param name="enemiesCapacity">The maximum capacity for selecting enemies.</param>
    /// <returns>An array of selected enemies based on the Knapsack problem solution.</returns>
    public static RoomContents[] ResolveKnapsackEnemies(List<RoomContents> enemies, List<int> enemiesValues, int enemiesCapacity)
    {
        List<int> chosenEnemiesIdx = ResolveKnapsack(enemiesValues, enemiesCapacity);

        RoomContents[] chosenEnemies = new RoomContents[chosenEnemiesIdx.Count];
        for (int i = 0; i < chosenEnemiesIdx.Count; i++)
        {
            int idx = chosenEnemiesIdx[i];
            chosenEnemies[i] = enemies[idx];
        }

        //Debug.Log("Inimigos escolhidos: " + string.Join(", ", chosenEnemies));
        return chosenEnemies;
    }

    /// <summary>
    /// Resolves the Knapsack problem for selecting obstacles based on their values and a capacity constraint.
    /// </summary>
    /// <param name="obstacles">The list of obstacles to choose from.</param>
    /// <param name="obstaclesValues">The list of values associated with each obstacle.</param>
    /// <param name="obstaclesCapacity">The maximum capacity for selecting obstacles.</param>
    /// <returns>An array of selected obstacles based on the Knapsack problem solution.</returns>
    public static RoomContents[] ResolveKnapsackObstacles(List<RoomContents> obstacles, List<int> obstaclesValues, int obstaclesCapacity)
    {
        List<int> chosenObstaclesIdx = ResolveKnapsack(obstaclesValues, obstaclesCapacity);

        RoomContents[] chosenObstacles = new RoomContents[chosenObstaclesIdx.Count];
        for (int i = 0; i < chosenObstaclesIdx.Count; i++)
        {
            int idx = chosenObstaclesIdx[i];
            chosenObstacles[i] = obstacles[idx];
        }

        //Debug.Log("Obstaculos escolhidos: " + string.Join(", ", chosenObstacles));
        return chosenObstacles;
    }

    /// <summary>
    /// Resolves the Knapsack problem for selecting items based on their values and a capacity constraint.
    /// </summary>
    /// <param name="values">The list of values associated with each item.</param>
    /// <param name="capacity">The maximum capacity for selecting items.</param>
    /// <returns>A list of indices representing the selected items based on the Knapsack problem solution.</returns>
    public static List<int> ResolveKnapsack(in List<int> values, in int capacity)
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
                    if (newValue > maxValue || (newValue == maxValue && chosenItems[w - values[i]].Count + 1 < chosenItem.Count))
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
