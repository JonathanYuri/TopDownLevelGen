using RoomGeneticAlgorithm.Constants;
using System.Collections.Generic;
using UnityEngine;

namespace RoomGeneticAlgorithm.Fitness
{
    /// <summary>
    /// Calculate fitness for room individuals.
    /// </summary>
    public static class FitnessCalculator
    {
        /// <summary>
        /// Calculates and assigns the fitness value for an individual based on provided fitness variables.
        /// If the individual is considered "monstrous," their fitness value is set to the minimum possible integer value.
        /// </summary>
        /// <param name="individual">The individual to evaluate.</param>
        /// <param name="allFitnessVars">An array of fitness variables used in the evaluation.</param>
        public static void Evaluate(RoomIndividual individual, FitnessHandler fitnessHandler, List<int> allFitnessVars)
        {
            if (IsMonstrous(individual))
            {
                individual.Value = int.MinValue;
                return;
            }

            individual.Value = CalculateNormalizedFitnessValue(fitnessHandler.fitnessVars, allFitnessVars);
        }

        /// <summary>
        /// Calculates and assigns the fitness value for an individual.
        /// Fitness variables are determined internally for the individual before evaluation.
        /// If the individual is considered "monstrous," their fitness value is set to the minimum possible integer value.
        /// </summary>
        /// <param name="individual">The individual to evaluate.</param>
        public static void Evaluate(RoomIndividual individual, FitnessHandler fitnessHandler)
        {
            if (IsMonstrous(individual))
            {
                individual.Value = int.MinValue;
                return;
            }

            individual.Value = CalculateNormalizedFitnessValue(
                fitnessHandler.fitnessVars,
                CalculateAllFitnessVars(individual, fitnessHandler)
            );
        }

        /// <summary>
        /// Calculates the normalized fitness value based on a set of fitness variables.
        /// Normalization ensures that the fitness value falls within specified bounds.
        /// </summary>
        /// <param name="fitnessVarsValue">An array of fitness variables used to calculate the fitness value.</param>
        /// <returns>The normalized fitness value.</returns>
        static int CalculateNormalizedFitnessValue(List<FitnessVar> fitnessVars, List<int> fitnessVarsValue)
        {
            int value = 0;
            for (int i = 0; i < fitnessVars.Count; i++)
            {
                FitnessVar fitnessVar = fitnessVars[i];
                Range<int> varBound = fitnessVar.CurrentBound;

                double normalizedValue = Utils.Normalization(fitnessVarsValue[i], varBound.Min, varBound.Max);
                value += (int)(normalizedValue * fitnessVar.Importance);
            }
            return value;
        }

        /// <summary>
        /// Calculates fitness variables based on the characteristics of the room individual.
        /// </summary>
        /// <param name="individual">The room individual for which to calculate fitness variables.</param>
        /// <returns>A array of calculated fitness variables.</returns>
        internal static List<int> CalculateAllFitnessVars(RoomIndividual individual, FitnessHandler fitnessHandler)
        {
            List<int> vars = new();
            float difficulty = GeneticAlgorithmConstants.ROOM.Difficulty;

            // Calculate ideal values based on difficulty
            foreach (var fitnessVar in fitnessHandler.fitnessVars)
            {
                float varValue = -Mathf.Abs(fitnessVar.FitnessVarValue(individual) - fitnessVar.Ideal);
                vars.Add((int)varValue);
            }
            return vars;
        }

        /// <summary>
        /// Determines if a given room individual is considered "monstrous" based on certain criteria.
        /// </summary>
        /// <param name="individual">The room individual to evaluate.</param>
        /// <returns>True if the individual is monstrous; otherwise, false.</returns>
        static bool IsMonstrous(RoomIndividual individual)
        {
            if (!PathFinder.IsAPathBetweenDoors(individual.RoomMatrix.Values, GeneticAlgorithmConstants.ROOM.DoorPositions))
            {
                return true;
            }

            if (!PathFinder.IsAPathBetweenDoorAndEnemies(individual.RoomMatrix, GeneticAlgorithmConstants.ROOM.DoorPositions))
            {
                return true;
            }

            bool hasTheRightAmountOfEnemies = individual.RoomMatrix.EnemiesPositions.Count == GeneticAlgorithmConstants.ROOM.Enemies.Length;
            if (!hasTheRightAmountOfEnemies)
            {
                return true;
            }

            bool hasTheRightAmountOfObstacles = individual.RoomMatrix.ObstaclesPositions.Count == GeneticAlgorithmConstants.ROOM.Obstacles.Length;
            if (!hasTheRightAmountOfObstacles)
            {
                return true;
            }

            /*
            if (!RoomOperations.HasTheRightObjects(roomMatrix, typeof(Enemies), enemiesPositions, room.enemies.Cast<object>().ToList()))
            {
                //Debug.Log("Mostro por causa dos inimigos");
                return true;
            }

            if (!RoomOperations.HasTheRightObjects(roomMatrix, typeof(Obstacles), obstaclesPositions, room.obstacles.Cast<object>().ToList()))
            {
                //Debug.Log("Mostro por causa dos obstaculo");
                return true;
            }
            */

            return false;
        }
    }
}
