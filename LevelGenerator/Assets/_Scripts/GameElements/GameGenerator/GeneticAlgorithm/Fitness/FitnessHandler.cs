using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoomGeneticAlgorithm.Constants;

namespace RoomGeneticAlgorithm.Fitness
{
    /// <summary>
    /// Manages fitness calculation for room individuals in the genetic algorithm.
    /// </summary>
    public class FitnessHandler
    {
        internal List<FitnessVar> fitnessVars = new();

        public FitnessHandler(float roomDifficulty)
        {
            fitnessVars.Add(FitnessVarsConstants.NUM_ENEMIES_GROUP);
            fitnessVars.Add(FitnessVarsConstants.AVERAGE_ENEMIES_PER_GROUP);
            fitnessVars.Add(FitnessVarsConstants.AVERAGE_ENEMY_DOOR_DISTANCE);
            fitnessVars.Add(FitnessVarsConstants.AVERAGE_BETWEEN_ENEMIES_DISTANCE);
            //fitnessVars.Add(FitnessVarsConstants.AVERAGE_OBSTACLES_NEXT_TO_ENEMIES);
            //fitnessVars.Add(FitnessVarsConstants.AVERAGE_ENEMIES_WITH_COVER);

            foreach (var fitnessVar in fitnessVars)
            {
                fitnessVar.SetIdealValue(roomDifficulty);
            }

            Debug.LogWarning("ideal: " + fitnessVars[0].Ideal);
        }

        /// <summary>
        /// Determines whether the fitness should be recalculated for a specific individual.
        /// This is based on whether the individual has been modified or if the bounds of fitness variables have been modified.
        /// </summary>
        /// <param name="individual">The individual for which fitness is being evaluated.</param>
        /// <returns>True if fitness should be recalculated, false otherwise.</returns>
        bool ShouldRecalculateFitness(RoomIndividual individual)
        {
            if (individual.Modified)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evaluates the fitness of an entire population of individuals.
        /// Fitness variables are calculated for each individual, and bounds of fitness variables are determined.
        /// If an individual has been modified or the bounds of fitness variables have changed, their fitness is recalculated.
        /// </summary>
        /// <param name="population">The population of individuals to evaluate.</param>
        public void EvaluatePopulation(RoomIndividual[] population, FitnessHandler fitnessHandler)
        {
            // avaliar o individual se ele foi modificado
            for (int i = 0; i < population.Length; i++)
            {
                if (!ShouldRecalculateFitness(population[i]))
                {
                    continue;
                }

                FitnessCalculator.Evaluate(population[i], fitnessHandler);
                population[i].Modified = false;
            }
        }
    }
}