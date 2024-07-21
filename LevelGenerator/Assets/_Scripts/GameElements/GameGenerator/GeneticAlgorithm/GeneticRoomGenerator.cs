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
        public static float MIN_MUTATIONS_PERCENT = 0.7f; // pelo menos x% do hashset vai ser mutado
        public static int POPULATION_SIZE = 6;
        public static int TOURNAMENT_SIZE = 4;
        public static int NUM_PARENTS_TOURNAMENT = 2;

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
    public class GeneticRoomGenerator
    {
        RoomIndividual[] population;

        readonly FitnessHandler fitnessHandler;
        readonly Reproduction reproduction;
        readonly Mutation mutation;
        readonly RoomSkeleton roomSkeleton;

        public RoomIndividual Best { get; set; }

        public GeneticRoomGenerator(RoomSkeleton roomSkeleton)
        {
            population = new RoomIndividual[GeneticAlgorithmConstants.POPULATION_SIZE];
            this.roomSkeleton = roomSkeleton;
            fitnessHandler = new(roomSkeleton);
            reproduction = new(roomSkeleton);
            mutation = new(roomSkeleton);

            for (int i = 0; i < GeneticAlgorithmConstants.POPULATION_SIZE; i++)
            {
                population[i] = new(roomSkeleton);
            }

            fitnessHandler.EvaluatePopulation(population, fitnessHandler);
            Best = new(population.GetBestIndividual());
        }

        /// <summary>
        /// Executes the main loop of the genetic algorithm to evolve a population of individuals.
        /// </summary>
        /// <returns>A matrix representing the best room configuration found by the genetic algorithm.</returns>
        public IEnumerator GeneticLooping()
        {
            int iterationsWithoutImprovement = 0;
            int iterations = 0;

            while (ShouldContinueLooping(iterationsWithoutImprovement))
            {
                //Debug.LogWarning("NUMERO DE INTERACOES: " +iterations + " MELHOR ATUAL: " + bestInGeneration.Value + " MELHOR: " + Best.Value);
                UpdateBestIndividual(ref iterationsWithoutImprovement);
                PerformGeneticOperations();
                iterations++;
                yield return null;
            }

            UpdateBestIndividual(ref iterationsWithoutImprovement);
            Debug.LogError("Melhor individual: " + Best.Value);
        }

        void UpdateBestIndividual(ref int iterationsWithoutImprovement)
        {
            RoomIndividual bestInGeneration = population.GetBestIndividual();
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

        void PerformGeneticOperations()
        {
            population = reproduction.PerformReproduction(population);
            mutation.MutatePopulation(population);
            fitnessHandler.EvaluatePopulation(population, fitnessHandler);
            FitnessCalculator.Evaluate(Best, fitnessHandler, roomSkeleton.DoorPositions);
        }

        /// <summary>
        /// Determines whether the genetic algorithm should continue the loop.
        /// </summary>
        /// <param name="iterationsWithoutImprovement">The number of iterations without improvement.</param>
        /// <returns><c>true</c> if the loop should continue; otherwise, <c>false</c>.</returns>
        bool ShouldContinueLooping(int iterationsWithoutImprovement)
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