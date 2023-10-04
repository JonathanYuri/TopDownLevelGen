using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GeneticAlgorithmConstants
{
    // TODO: mudar a probabilidade de crossover e mutacao durante a execucao?

    public static int ITERATIONS_WITHOUT_IMPROVEMENT = 20;
    public static float CROSSOVER_PROBABILITY = 0.8f; // 0 a 1
    public static float MUTATION_PROBABILITY = 0.8f; // 0 a 1
    public static float DIFFICULTY = 0f; // 0 a 1
    public static int POPULATION_SIZE = 6;
    public static int TOURNAMENT_SIZE = 5;
    public static int NUM_PARENTS_TOURNAMENT = 2;
    public static Room ROOM;

    public static void LimitVariables()
    {
        CROSSOVER_PROBABILITY = Mathf.Clamp(CROSSOVER_PROBABILITY, 0f, 1f);
        MUTATION_PROBABILITY = Mathf.Clamp(MUTATION_PROBABILITY, 0f, 1f);
        DIFFICULTY = Mathf.Clamp(DIFFICULTY, 0f, 1f);
    }
}

public class GeneticRoomGenerator
{
    RoomIndividual[] population;
    readonly FitnessCalculator fitnessCalculator;

    public GeneticRoomGenerator(Room room)
    {
        GeneticAlgorithmConstants.ROOM = room;
        GeneticAlgorithmConstants.LimitVariables();
        population = new RoomIndividual[GeneticAlgorithmConstants.POPULATION_SIZE];
        fitnessCalculator = new();
    }

    void GeneratePopulation()
    {
        for (int i = 0; i < GeneticAlgorithmConstants.POPULATION_SIZE; i++)
        {
            population[i] = new();
        }
    }

    public RoomContents[,] GeneticLooping()
    {
        GeneratePopulation();
        fitnessCalculator.EvaluatePopulation(population);

        RoomIndividual best = new(population.MaxBy(individual => individual.Value));

        int iterationsWithoutImprovement = 0;
        int iterations = 0;
        while (ShouldContinueLooping(iterationsWithoutImprovement, best))
        {
            RoomIndividual bestInGeneration = population.MaxBy(individual => individual.Value);
            
            Debug.Log("NUMERO DE INTERACOES: " + iterations + " MELHOR ATUAL: " + bestInGeneration.Value + " MELHOR: " + best.Value);
            if (bestInGeneration.Value > best.Value)
            {
                iterationsWithoutImprovement = 0;
                best = new(bestInGeneration);
            }
            else
            {
                iterationsWithoutImprovement++;
            }

            population = GeneticOperator.PerformReproduction(population);
            GeneticOperator.MutatePopulation(population);
            fitnessCalculator.EvaluatePopulation(population);

            iterations++;
        }

        RoomIndividual bestInGenerationFinal = population.MaxBy(individual => individual.Value);
        if (bestInGenerationFinal.Value > best.Value)
        {
            best = new(bestInGenerationFinal);
        }

        Debug.Log("Melhor individual: " + best.Value);
        Debug.Log("Inimigo: " + best.RoomMatrix.EnemiesPositions.Count);
        Debug.Log("Obstaculo: " + best.RoomMatrix.ObstaclesPositions.Count);
        Debug.Log("qntInimigosProximosDeObstaculos: " +
            RoomOperations.CountEnemiesNextToObstacles(best.RoomMatrix));
        // retornar o melhor
        return best.RoomMatrix.Values;
    }

    bool ShouldContinueLooping(int iterationsWithoutImprovement, RoomIndividual best)
    {
        if (iterationsWithoutImprovement < GeneticAlgorithmConstants.ITERATIONS_WITHOUT_IMPROVEMENT)
        {
            return true;
        }
        if (best.Value == int.MinValue)
        {
            return true;
        }
        return false;
    }
}
