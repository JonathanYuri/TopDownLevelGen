using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoomGeneticAlgorithm.Constants;

namespace RoomGeneticAlgorithm.Fitness
{
    /// <summary>
    /// Manages fitness calculation for room individuals in the genetic algorithm.
    /// </summary>
    public static class FitnessHandler
    {
        internal static readonly int numberOfFitnessVariables = 3;

        static Dictionary<int, int[]> allFitness = new();
        internal static List<Range> boundsOfFitnessVars = new(); // pra normalizar as variaveis da fitness do individual
        static bool areBoundsModified = true;

        static readonly float numGroupsImportance = 1f;
        static readonly float enemiesPerGroupAverageImportance = 1f;
        static readonly float enemyDoorDistanceImportance = 1f;

        internal enum FitnessVariable
        {
            NumGroups = 0,
            EnemiesPerGroupAverage = 1,
            EnemyDoorDistance = 2
        }

        internal static readonly Dictionary<FitnessVariable, float> importances = new()
        {
            { FitnessVariable.NumGroups, numGroupsImportance },
            { FitnessVariable.EnemiesPerGroupAverage, enemiesPerGroupAverageImportance },
            { FitnessVariable.EnemyDoorDistance, enemyDoorDistanceImportance }
        };

        static FitnessHandler()
        {
            numGroupsImportance = Mathf.Clamp(numGroupsImportance, 0f, 1f);
            enemiesPerGroupAverageImportance = Mathf.Clamp(enemiesPerGroupAverageImportance, 0f, 1f);
            enemyDoorDistanceImportance = Mathf.Clamp(enemyDoorDistanceImportance, 0f, 1f);
        }

        /// <summary>
        /// Determines the bounds of fitness variables based on the current and previous fitness values.
        /// </summary>
        static void DetermineFitnessVarBounds()
        {
            bool boundsModified = false;
            for (int i = 0; i < numberOfFitnessVariables; i++) // pra cada variavel do fitness
            {
                (int max, int min) = allFitness.MaxAndMin(fitness => fitness.Value[i]);

                if (i < boundsOfFitnessVars.Count) // se tiver elementos naquela posicao
                {
                    if (max > boundsOfFitnessVars[i].max)
                    {
                        boundsOfFitnessVars[i].max = max;
                        boundsModified = true;
                    }
                    if (min < boundsOfFitnessVars[i].min)
                    {
                        boundsOfFitnessVars[i].min = min;
                        boundsModified = true;
                    }
                }
                else
                {
                    boundsOfFitnessVars.Add(new Range { max = max, min = min });
                }
            }

            areBoundsModified = boundsModified;
        }

        /// <summary>
        /// Calculates fitness variables based on the characteristics of the room individual.
        /// </summary>
        /// <param name="individual">The room individual for which to calculate fitness variables.</param>
        /// <returns>A array of calculated fitness variables.</returns>
        internal static int[] CalculeAllFitnessVars(RoomIndividual individual)
        {
            List<int> groups = GroupCounter.CountGroups(individual.RoomMatrix.Values, individual.RoomMatrix.EnemiesPositions);
            double media = groups.Average();

            double averageDistanceFromDoorsToEnemies =
                RoomOperations.AverageDistanceFromDoorsToEnemies(individual.RoomMatrix.EnemiesPositions, GeneticAlgorithmConstants.ROOM.DoorPositions);

            float valueWhenDifficultyIsMinimal = (float)averageDistanceFromDoorsToEnemies; // maximizar a distancia entre os inimigos e as portas
            float valueWhenDifficultyIsMaximal = (float)-averageDistanceFromDoorsToEnemies; // minimizar a distancia entre os inimigos e as portas

            //double minimunDistanceFromDoorsToEnemies = RoomOperations.MinimumDistanceBetweenDoorsAndEnemies(individual.RoomMatrix.EnemiesPositions);
            //float valueWhenDifficultyIsMinimal = (float)minimunDistanceFromDoorsToEnemies; // maximizar a minima distancia entre os inimigos e as portas
            //float valueWhenDifficultyIsMaximal = (float)-minimunDistanceFromDoorsToEnemies; // minimizar a minima distancia entre os inimigos e as portas

            // Calculate the final value by interpolating between minimal and maximal values based on the difficulty.
            float value = Mathf.Lerp(valueWhenDifficultyIsMinimal, valueWhenDifficultyIsMaximal, GeneticAlgorithmConstants.ROOM.Difficulty);

            int[] vars = new int[numberOfFitnessVariables];
            vars[(int)FitnessVariable.NumGroups] = -groups.Count; // minimizar a quantidade de grupos
            vars[(int)FitnessVariable.EnemiesPerGroupAverage] = -(int)media; // minimizar a media de inimigos por grupos
            vars[(int)FitnessVariable.EnemyDoorDistance] = (int)value; // maximizar o value
            return vars;
        }

        /// <summary>
        /// Determines whether the fitness should be recalculated for a specific individual.
        /// This is based on whether the individual has been modified or if the bounds of fitness variables have been modified.
        /// </summary>
        /// <param name="individual">The individual for which fitness is being evaluated.</param>
        /// <returns>True if fitness should be recalculated, false otherwise.</returns>
        static bool ShouldRecalculateFitness(RoomIndividual individual)
        {
            if (individual.ItWasModified)
            {
                return true;
            }
            if (areBoundsModified)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates fitness variables for all individuals within a population if recalculation is necessary.
        /// If an individual has been modified or the bounds of fitness variables have changed, their fitness variables are recalculated.
        /// </summary>
        /// <param name="population">The population of individuals for which to calculate fitness variables.</param>
        static void CalculeAllFitnessVarsOfPopulation(RoomIndividual[] population)
        {
            for (int i = 0; i < population.Length; i++)
            {
                if (!ShouldRecalculateFitness(population[i]))
                {
                    continue;
                }

                allFitness[i] = CalculeAllFitnessVars(population[i]);
            }
        }

        /// <summary>
        /// Evaluates the fitness of an entire population of individuals.
        /// Fitness variables are calculated for each individual, and bounds of fitness variables are determined.
        /// If an individual has been modified or the bounds of fitness variables have changed, their fitness is recalculated.
        /// </summary>
        /// <param name="population">The population of individuals to evaluate.</param>
        public static void EvaluatePopulation(RoomIndividual[] population)
        {
            CalculeAllFitnessVarsOfPopulation(population);
            DetermineFitnessVarBounds();

            // avaliar o individual se ele foi modificado
            for (int i = 0; i < population.Length; i++)
            {
                if (!ShouldRecalculateFitness(population[i]))
                {
                    continue;
                }

                FitnessCalculator.Evaluate(population[i], allFitness[i]);
                population[i].ItWasModified = false;
            }
        }
    }
}