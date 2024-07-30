using System.Collections.Generic;
using UnityEngine;

namespace RoomGeneticAlgorithm.Fitness
{
    /// <summary>
    /// Calculate fitness for room individuals.
    /// </summary>
    public class FitnessCalculator
    {
        readonly List<FitnessVar> fitnessVars = new();

        public FitnessCalculator(float roomDifficulty)
        {
            fitnessVars.AddRange(FitnessVarsConstants.VARS.GetRandomElements());

            foreach (var fitnessVar in fitnessVars)
            {
                fitnessVar.SetIdealValue(roomDifficulty);
            }
        }

        public List<string> GetFitnessVarsNames()
        {
            List<string> names = new();
            foreach (var fitnessVar in fitnessVars)
            {
                names.Add(fitnessVar.Name);
            }
            return names;
        }

        /// <summary>
        /// Calculates and assigns the fitness value for an individual based on provided fitness variables.
        /// If the individual is considered "monstrous," their fitness value is set to the minimum possible integer value.
        /// </summary>
        /// <param name="individual">The individual to evaluate.</param>
        public void Evaluate(RoomIndividual individual)
        {
            if (IsMonstrous(individual))
            {
                individual.Value = int.MinValue;
                return;
            }

            individual.Value = CalculateFitnessValue(individual);
        }

        /// <summary>
        /// Calculates fitness variables based on the characteristics of the room individual.
        /// </summary>
        /// <param name="individual">The room individual for which to calculate fitness variables.</param>
        /// <returns>A array of calculated fitness variables.</returns>
        int CalculateFitnessValue(RoomIndividual individual)
        {
            int value = 0;
            foreach (var fitnessVar in fitnessVars)
            {
                float fitnessVarValue = fitnessVar.FitnessVarValue(individual);
                float distance = Mathf.Abs(fitnessVarValue - fitnessVar.Ideal);
                float normalizedVar = fitnessVar.Normalize(distance);
                value += (int)(normalizedVar * fitnessVar.Importance);
            }
            return value;
        }

        /// <summary>
        /// Determines if a given room individual is considered "monstrous" based on certain criteria.
        /// </summary>
        /// <param name="individual">The room individual to evaluate.</param>
        /// <returns>True if the individual is monstrous; otherwise, false.</returns>
        bool IsMonstrous(RoomIndividual individual) =>
            !PathFinder.AreAllPathsValid(individual.RoomMatrix);
    }
}
