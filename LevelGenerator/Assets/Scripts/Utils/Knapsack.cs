using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knapsack
{
    public static RoomContents[] ResolveKnapsackEnemies()
    {
        List<int> valuesEnemies = new(GameConstants.ENEMIES_DIFFICULTY.Values);
        List<RoomContents> keysEnemies = new(GameConstants.ENEMIES_DIFFICULTY.Keys);

        List<int> chosenEnemiesIdx = ResolveKnapsack(valuesEnemies, GameConstants.ENEMIES_CAPACITY);

        RoomContents[] chosenEnemies = new RoomContents[chosenEnemiesIdx.Count];
        for (int i = 0; i < chosenEnemiesIdx.Count; i++)
        {
            int idx = chosenEnemiesIdx[i];
            chosenEnemies[i] = keysEnemies[idx];
        }

        //Debug.Log("Inimigos escolhidos: " + string.Join(", ", chosenEnemies));
        return chosenEnemies;
    }

    public static RoomContents[] ResolveKnapsackObstacles()
    {
        List<int> valuesObstacles = new(GameConstants.OBSTACLES_DIFFICULTY.Values);
        List<RoomContents> keysObstacles = new(GameConstants.OBSTACLES_DIFFICULTY.Keys);

        List<int> chosenObstaclesIdx = ResolveKnapsack(valuesObstacles, GameConstants.OBSTACLES_CAPACITY);

        RoomContents[] chosenObstacles = new RoomContents[chosenObstaclesIdx.Count];
        for (int i = 0; i < chosenObstaclesIdx.Count; i++)
        {
            int idx = chosenObstaclesIdx[i];
            chosenObstacles[i] = keysObstacles[idx];
        }

        //Debug.Log("Obstaculos escolhidos: " + string.Join(", ", chosenObstacles));
        return chosenObstacles;
    }

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
