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
        public static void Evaluate(RoomIndividual individual, FitnessHandler fitnessHandler)
        {
            if (IsMonstrous(individual))
            {
                individual.Value = int.MinValue;
                return;
            }

            individual.Value = CalculateFitnessValue(individual, fitnessHandler);
        }

        /// <summary>
        /// Calculates fitness variables based on the characteristics of the room individual.
        /// </summary>
        /// <param name="individual">The room individual for which to calculate fitness variables.</param>
        /// <returns>A array of calculated fitness variables.</returns>
        internal static int CalculateFitnessValue(RoomIndividual individual, FitnessHandler fitnessHandler)
        {
            int value = 0;
            foreach (var fitnessVar in fitnessHandler.fitnessVars)
            {
                float fitnessVarValue = fitnessVar.FitnessVarValue(individual);
                float distance = Mathf.Abs(fitnessVarValue - fitnessVar.Ideal);
                float normalizedVar = fitnessVar.Normalize(distance);

                Debug.Log("Value: " + fitnessVarValue + " Distance: " + distance + " normalizedVar: " + normalizedVar);
                value += (int)(normalizedVar * fitnessVar.Importance);
            }
            return value;
        }

        /// <summary>
        /// Determines if a given room individual is considered "monstrous" based on certain criteria.
        /// </summary>
        /// <param name="individual">The room individual to evaluate.</param>
        /// <returns>True if the individual is monstrous; otherwise, false.</returns>
        static bool IsMonstrous(RoomIndividual individual) =>
            !PathFinder.AreAllPathsValid(individual.RoomMatrix);
    }
}
