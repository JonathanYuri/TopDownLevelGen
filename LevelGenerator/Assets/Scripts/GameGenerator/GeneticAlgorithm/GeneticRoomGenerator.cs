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
        public static float CROSSOVER_PROBABILITY = 0.8f; // 0 a 1
        public static float MUTATION_PROBABILITY = 0.8f; // 0 a 1
        public static int POPULATION_SIZE = 6;
        public static int TOURNAMENT_SIZE = 5;
        public static int NUM_PARENTS_TOURNAMENT = 2;
        public static RoomSkeleton ROOM;

        /// <summary>
        /// Limits the genetic algorithm constants to a valid range.
        /// </summary>
        static GeneticAlgorithmConstants()
        {
            CROSSOVER_PROBABILITY = Mathf.Clamp(CROSSOVER_PROBABILITY, 0f, 1f);
            MUTATION_PROBABILITY = Mathf.Clamp(MUTATION_PROBABILITY, 0f, 1f);
        }
    }
}

namespace RoomGeneticAlgorithm.Run
{
    using RoomGeneticAlgorithm.Constants;
    using RoomGeneticAlgorithm.Fitness;
    using RoomGeneticAlgorithm.GeneticOperations;

    /// <summary>
    /// Represents a class for generating and evolving room configurations using a genetic algorithm.
    /// </summary>
    public class GeneticRoomGenerator
    {
        RoomIndividual[] population;

        /// <summary>
        /// Initializes a new instance of the GeneticRoomGenerator class.
        /// </summary>
        /// <param name="room">The room pattern for which the genetic algorithm will generate individuals.</param>
        public GeneticRoomGenerator(RoomSkeleton room)
        {
            GeneticAlgorithmConstants.ROOM = room;
            population = new RoomIndividual[GeneticAlgorithmConstants.POPULATION_SIZE];
        }

        /// <summary>
        /// Generates the initial population of individuals for the genetic algorithm.
        /// </summary>
        void GeneratePopulation()
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
        public RoomContents[,] GeneticLooping()
        {
            GeneratePopulation();
            FitnessHandler.EvaluatePopulation(population);

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

                population = Reproduction.PerformReproduction(population);
                Mutation.MutatePopulation(population);
                FitnessHandler.EvaluatePopulation(population);
                FitnessCalculator.Evaluate(best);

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

        /// <summary>
        /// Determines whether the genetic algorithm should continue the loop.
        /// </summary>
        /// <param name="iterationsWithoutImprovement">The number of iterations without improvement.</param>
        /// <param name="best">The best individual found so far.</param>
        /// <returns><c>true</c> if the loop should continue; otherwise, <c>false</c>.</returns>
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
}