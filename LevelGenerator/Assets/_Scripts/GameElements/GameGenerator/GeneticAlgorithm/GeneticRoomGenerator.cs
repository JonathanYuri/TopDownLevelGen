using UnityEngine;

namespace RoomGeneticAlgorithm.Constants
{
    /// <summary>
    /// A static class containing constants and parameters for the genetic algorithm.
    /// </summary>
    public static class GeneticAlgorithmConstants
    {
        // TODO: mudar a probabilidade de crossover e mutacao durante a execucao?

        public static int ITERATIONS_WITHOUT_IMPROVEMENT = 20;
        public static float CROSSOVER_PROBABILITY = 1.0f; // 0 a 1
        public static float MUTATION_PROBABILITY = 1.0f; // 0 a 1
        public static int POPULATION_SIZE = 6;
        public static int TOURNAMENT_SIZE = 5;
        public static int NUM_PARENTS_TOURNAMENT = 2;
        public static RoomSkeleton ROOM;

        /// <summary>
        /// Limits the genetic algorithm constants to a valid range.
        /// </summary>
        static GeneticAlgorithmConstants()
        {
            CROSSOVER_PROBABILITY = Mathf.Clamp01(CROSSOVER_PROBABILITY);
            MUTATION_PROBABILITY = Mathf.Clamp01(MUTATION_PROBABILITY);
        }
    }
}

namespace RoomGeneticAlgorithm.Run
{
    using RoomGeneticAlgorithm.Constants;
    using RoomGeneticAlgorithm.Fitness;
    using RoomGeneticAlgorithm.GeneticOperations;
    using System.Collections;

    /// <summary>
    /// Represents a class for generating and evolving room configurations using a genetic algorithm.
    /// </summary>
    public static class GeneticRoomGenerator
    {
        static RoomIndividual[] population = new RoomIndividual[GeneticAlgorithmConstants.POPULATION_SIZE];
        public static RoomIndividual Best { get; private set; }

        /// <summary>
        /// Generates the initial population of individuals for the genetic algorithm.
        /// </summary>
        static void GeneratePopulation()
        {
            for (int i = 0; i < GeneticAlgorithmConstants.POPULATION_SIZE; i++)
            {
                population[i] = new();
            }
        }

        /// <summary>
        /// Executes the main loop of the genetic algorithm to evolve a population of individuals.
        /// </summary>
        /// <returns>A matrix representing the best room configuration found by the genetic algorithm.</returns>
        public static IEnumerator GeneticLooping(RoomSkeleton room)
        {
            GeneticAlgorithmConstants.ROOM = room;

            GeneratePopulation();
            FitnessHandler.StartAlgorithm();
            FitnessHandler.EvaluatePopulation(population);

            Best = new(population.MaxBy(individual => individual.Value));

            int iterationsWithoutImprovement = 0;
            int iterations = 0;

            while (ShouldContinueLooping(iterationsWithoutImprovement))
            {
                foreach (RoomIndividual roomIndividual in population)
                {
                    //Debug.Log("value: " + roomIndividual.Value);
                }
                RoomIndividual bestInGeneration = population.MaxBy(individual => individual.Value);
                Debug.LogError("NUMERO DE INTERACOES: " + iterations + " MELHOR ATUAL: " + bestInGeneration.Value + " MELHOR: " + Best.Value);
                UpdateBestIndividual(bestInGeneration, ref iterationsWithoutImprovement);
                PerformGeneticOperations();
                iterations++;
                yield return null;
            }

            RoomIndividual bestInGenerationFinal = population.MaxBy(individual => individual.Value);
            if (bestInGenerationFinal.Value > Best.Value)
            {
                Best = new(bestInGenerationFinal);
            }

            Debug.Log("Melhor individual: " + Best.Value);
            Debug.Log("Inimigo: " + Best.RoomMatrix.EnemiesPositions.Count);
            Debug.Log("Obstaculo: " + Best.RoomMatrix.ObstaclesPositions.Count);
            Debug.Log("qntInimigosProximosDeObstaculos: " +
                RoomOperations.CountEnemiesNextToObstacles(Best.RoomMatrix));
        }

        static void UpdateBestIndividual(RoomIndividual bestInGeneration, ref int iterationsWithoutImprovement)
        {
            if (bestInGeneration.Value > Best.Value)
            {
                iterationsWithoutImprovement = 0;
                Best = new(bestInGeneration);
            }
            else
            {
                iterationsWithoutImprovement++;
            }
        }

        static void PerformGeneticOperations()
        {
            population = Reproduction.PerformReproduction(population);
            Mutation.MutatePopulation(population);
            FitnessHandler.EvaluatePopulation(population);
            FitnessCalculator.Evaluate(Best);
        }

        /// <summary>
        /// Determines whether the genetic algorithm should continue the loop.
        /// </summary>
        /// <param name="iterationsWithoutImprovement">The number of iterations without improvement.</param>
        /// <param name="best">The best individual found so far.</param>
        /// <returns><c>true</c> if the loop should continue; otherwise, <c>false</c>.</returns>
        static bool ShouldContinueLooping(int iterationsWithoutImprovement)
        {
            if (iterationsWithoutImprovement < GeneticAlgorithmConstants.ITERATIONS_WITHOUT_IMPROVEMENT)
            {
                return true;
            }
            if (Best.Value == int.MinValue)
            {
                return true;
            }
            return false;
        }
    }
}