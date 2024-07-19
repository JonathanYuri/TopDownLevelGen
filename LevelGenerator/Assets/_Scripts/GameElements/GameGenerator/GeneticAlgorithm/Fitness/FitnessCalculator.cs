using RoomGeneticAlgorithm.Constants;
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
        public static void Evaluate(RoomIndividual individual, int[] allFitnessVars)
        {
            if (IsMonstrous(individual))
            {
                individual.Value = int.MinValue;
                return;
            }

            individual.Value = CalculateNormalizedFitnessValue(allFitnessVars);
            //Value = - groups.Count - (int)media + (int)value; // + qntInimigosProximosDeObstaculos;
        }

        /// <summary>
        /// Calculates and assigns the fitness value for an individual.
        /// Fitness variables are determined internally for the individual before evaluation.
        /// If the individual is considered "monstrous," their fitness value is set to the minimum possible integer value.
        /// </summary>
        /// <param name="individual">The individual to evaluate.</param>
        public static void Evaluate(RoomIndividual individual)
        {
            if (IsMonstrous(individual))
            {
                individual.Value = int.MinValue;
                return;
            }

            individual.Value = CalculateNormalizedFitnessValue(FitnessHandler.CalculeAllFitnessVars(individual));
        }

        /// <summary>
        /// Calculates the normalized fitness value based on a set of fitness variables.
        /// Normalization ensures that the fitness value falls within specified bounds.
        /// </summary>
        /// <param name="fitnessVars">An array of fitness variables used to calculate the fitness value.</param>
        /// <returns>The normalized fitness value.</returns>
        static int CalculateNormalizedFitnessValue(int[] fitnessVars)
        {
            int value = 0;
            for (int i = 0; i < FitnessHandler.numberOfFitnessVariables; i++)
            {
                Range<int> varBound = FitnessHandler.boundsOfFitnessVars[i];
                float varImportance = FitnessHandler.importances[(FitnessHandler.FitnessVariable)i];

                double normalizedValue = Utils.Normalization(fitnessVars[i], varBound.Min, varBound.Max);
                value += (int)(normalizedValue * varImportance);
            }
            return value;
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
