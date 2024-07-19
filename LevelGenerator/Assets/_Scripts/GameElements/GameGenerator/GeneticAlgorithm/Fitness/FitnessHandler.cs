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
        internal static List<Range<int>> boundsOfFitnessVars = new(); // pra normalizar as variaveis da fitness do individual
        static bool areBoundsModified = true;

        static readonly float numGroupsImportance = 1f;
        static readonly float enemiesPerGroupAverageImportance = 1f;
        static readonly float enemyDoorDistanceImportance = 1f;

        static readonly Range<int> numGroupsRange = new(1, 10);
        static readonly Range<float> enemiesPerGroupRange = new(1f, 5f);
        static readonly Range<int> enemyDoorDistanceRange = new(1, 90);

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
            numGroupsImportance = Mathf.Clamp01(numGroupsImportance);
            enemiesPerGroupAverageImportance = Mathf.Clamp01(enemiesPerGroupAverageImportance);
            enemyDoorDistanceImportance = Mathf.Clamp01(enemyDoorDistanceImportance);
        }

        public static void StartAlgorithm()
        {
            allFitness.Clear();
            boundsOfFitnessVars.Clear();

            // initialize allFitness
            for (int i = 0; i < numberOfFitnessVariables; i++)
            {
                allFitness.Add(i, Enumerable.Repeat(int.MinValue, GeneticAlgorithmConstants.POPULATION_SIZE).ToArray());
            }

            // initialize bounds
            for (int i = 0; i < numberOfFitnessVariables; i++)
            {
                boundsOfFitnessVars.Add(new(int.MaxValue, int.MinValue));
            }
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
                Range<int> varBound = new(min, max);

                UpdateExistingBound(i, varBound, ref boundsModified);
            }
            areBoundsModified = boundsModified;
        }

        static void UpdateExistingBound(int boundIndex, Range<int> varBound, ref bool boundsModified)
        {
            if (varBound.Max > boundsOfFitnessVars[boundIndex].Max)
            {
                //Debug.LogWarning("UPDATE MAX BOUND");
                boundsOfFitnessVars[boundIndex].Max = varBound.Max;
                boundsModified = true;
            }
            if (varBound.Min < boundsOfFitnessVars[boundIndex].Min)
            {
                //Debug.LogWarning("UPDATE MIN BOUND");
                boundsOfFitnessVars[boundIndex].Min = varBound.Min;
                boundsModified = true;
            }
        }

        /// <summary>
        /// Calculates fitness variables based on the characteristics of the room individual.
        /// </summary>
        /// <param name="individual">The room individual for which to calculate fitness variables.</param>
        /// <returns>A array of calculated fitness variables.</returns>
        internal static int[] CalculeAllFitnessVars(RoomIndividual individual)
        {
            float difficulty = GeneticAlgorithmConstants.ROOM.Difficulty;
            List<int> groupSizes = GroupCounter.GetGroupSizes(individual.RoomMatrix.Values, individual.RoomMatrix.EnemiesPositions);
            float averageEnemiesPerGroup = (float)groupSizes.Average();

            //double averageDistanceFromDoorsToEnemies =
            //    RoomOperations.AverageDistanceFromDoorsToEnemies(individual.RoomMatrix.EnemiesPositions, GeneticAlgorithmConstants.ROOM.DoorPositions);

            int distanceFromDoorsToEnemies =
                RoomOperations.DistanceFromDoorsToEnemies(individual.RoomMatrix.EnemiesPositions, GeneticAlgorithmConstants.ROOM.DoorPositions);

            // Calculate ideal values based on difficulty

            // quanto maior dificuldade quero diminuir o numero de grupos
            float numGroupsIdeal = Mathf.Lerp(numGroupsRange.Max, numGroupsRange.Min, difficulty);
            // quanto maior dificuldade quero aumentar a quantidade de inimigos por grupo
            float enemiesPerGroupAverageIdeal = Mathf.Lerp(enemiesPerGroupRange.Min, enemiesPerGroupRange.Max, difficulty);
            // quanto maior dificuldade quero diminuir a distancia das portas para os inimigos
            float enemyDoorDistanceIdeal = Mathf.Lerp(enemyDoorDistanceRange.Max, enemyDoorDistanceRange.Min, difficulty);

            // Calculate the fitness values (a distancia para o valor ideal)
            float numGroupsValue = -Mathf.Abs(groupSizes.Count - numGroupsIdeal);
            float enemiesPerGroupAverageValue = -Mathf.Abs(averageEnemiesPerGroup - enemiesPerGroupAverageIdeal);
            float enemyDoorDistanceValue = -Mathf.Abs(distanceFromDoorsToEnemies - enemyDoorDistanceIdeal);

            /*
            Debug.LogError($"IDEAL Num grups: {numGroupsIdeal} Count groups: {groupSizes.Count}" + "\n" +
                $"IDEAL enemiesPerGroup: {enemiesPerGroupAverageIdeal} average: {averageEnemiesPerGroup}" + "\n" +
                $"IDEAL enemyDoorDistance: {enemyDoorDistanceIdeal} distance: {distanceFromDoorsToEnemies}" + "\n" +
                $"DIFICULDADE: {difficulty}");
            */

            /*enemiesPerGroupAverageValue *= 10f;
            Debug.Log(
                $"numGroupsValue (float): {numGroupsValue} (int): {(int)numGroupsValue}" + "\n" +
                $"enemiesPerGroupAverageValue(float): {enemiesPerGroupAverageValue} (int): {(int)enemiesPerGroupAverageValue}" + "\n" +
                $"enemyDoorDistanceValue (float): {enemyDoorDistanceValue} (int): {(int)enemyDoorDistanceValue}" + "\n");
            */
            int[] vars = new int[numberOfFitnessVariables];
            vars[(int)FitnessVariable.NumGroups] = (int)numGroupsValue;
            vars[(int)FitnessVariable.EnemiesPerGroupAverage] = (int)enemiesPerGroupAverageValue;
            vars[(int)FitnessVariable.EnemyDoorDistance] = (int)enemyDoorDistanceValue;
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
            if (individual.Modified)
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
                population[i].Modified = false;
            }
        }
    }
}