using UnityEngine;

namespace RoomGeneticAlgorithm.Constants
{
    /// <summary>
    /// A static class containing constants and parameters for the genetic algorithm.
    /// </summary>
    public static class GeneticAlgorithmConstants
    {
        // TODO: mudar a probabilidade de crossover e mutacao durante a execucao?

        public static readonly int ITERATIONS_WITHOUT_IMPROVEMENT = 20;
        public static readonly float CROSSOVER_PROBABILITY = 1.0f; // 0 a 1
        public static readonly float MUTATION_PROBABILITY = 1.0f; // 0 a 1
        public static readonly float MIN_MUTATIONS_PERCENT = 0.7f; // pelo menos x% do hashset vai ser mutado
        public static readonly int POPULATION_SIZE = 6;
        public static readonly int TOURNAMENT_SIZE = 4;
        public static readonly int NUM_PARENTS_TOURNAMENT = 2;

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
    using System.Collections.Generic;

    /// <summary>
    /// Represents a class for generating and evolving room configurations using a genetic algorithm.
    /// </summary>
    public class GeneticRoomGenerator
    {
        RoomIndividual[] population;

        readonly FitnessHandler fitnessHandler;
        readonly Reproduction reproduction;
        readonly Mutation mutation;

        public int Iterations { get; private set; } = 0;

        public RoomIndividual Best { get; set; }

        public GeneticRoomGenerator(SharedRoomData sharedRoomData)
        {
            population = new RoomIndividual[GeneticAlgorithmConstants.POPULATION_SIZE];
            fitnessHandler = new(sharedRoomData.Difficulty);
            reproduction = new(sharedRoomData);
            mutation = new(sharedRoomData);

            for (int i = 0; i < GeneticAlgorithmConstants.POPULATION_SIZE; i++)
            {
                population[i] = new(sharedRoomData);
            }

            fitnessHandler.EvaluatePopulation(population);
            Best = new(population.GetBestIndividual());
        }

        public FitnessStatistics GetFitnessVarsValues() =>
            fitnessHandler.FitnessCalculator.GetFitnessVarsValues();
        public List<string> GetFitnessVarsNames() => fitnessHandler.FitnessCalculator.GetFitnessVarsNames();

        /// <summary>
        /// Executes the main loop of the genetic algorithm to evolve a population of individuals.
        /// </summary>
        /// <returns>A matrix representing the best room configuration found by the genetic algorithm.</returns>
        public IEnumerator GeneticLooping()
        {
            int iterationsWithoutImprovement = 0;
            Iterations = 0;

            while (ShouldContinueLooping(iterationsWithoutImprovement))
            {
                //Debug.LogWarning("NUMERO DE INTERACOES: " + iterations +  " MELHOR: " + Best.Value);
                UpdateBestIndividual(ref iterationsWithoutImprovement);
                PerformGeneticOperations();
                Iterations++;
                yield return null;
            }

            UpdateBestIndividual(ref iterationsWithoutImprovement);
            Debug.LogError("Melhor individual: " + Best.Value + " interacoes: " + Iterations);
            //Debug.LogError("TimeInPathFindeR:" + fitnessHandler.FitnessCalculator.totalTimeInPathFinder);
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
            fitnessHandler.EvaluatePopulation(population);
            fitnessHandler.FitnessCalculator.Evaluate(Best);
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