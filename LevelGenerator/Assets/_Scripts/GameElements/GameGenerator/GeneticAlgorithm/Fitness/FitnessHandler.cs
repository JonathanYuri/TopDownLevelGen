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
        Dictionary<int, List<int>> allFitness = new();
        bool areBoundsModified = true;

        internal List<FitnessVar> fitnessVars = new();

        public bool AreBoundsModified { get => areBoundsModified; set => areBoundsModified = value; }

        public FitnessHandler()
        {
            //fitnessVars.Add(FitnessVarsConstants.NUM_ENEMIES_GROUPS);
            //fitnessVars.Add(FitnessVarsConstants.ENEMIES_PER_GROUP_AVERAGE);
            fitnessVars.Add(FitnessVarsConstants.ENEMY_DOOR_DISTANCE);
            //fitnessVars.Add(FitnessVarsConstants.BETWEEN_ENEMIES_DISTANCE);

            foreach (var fitnessVar in fitnessVars)
            {
                fitnessVar.SetIdealValue(GeneticAlgorithmConstants.ROOM.Difficulty);
            }

            // initialize allFitness
            for (int i = 0; i < fitnessVars.Count; i++)
            {
                allFitness.Add(i, Enumerable.Repeat(int.MinValue, GeneticAlgorithmConstants.POPULATION_SIZE).ToList());
            }
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
            if (AreBoundsModified)
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
        void CalculeAllFitnessVarsOfPopulation(RoomIndividual[] population, FitnessHandler fitnessHandler)
        {
            AreBoundsModified = false;
            for (int i = 0; i < population.Length; i++)
            {
                if (!ShouldRecalculateFitness(population[i]))
                {
                    continue;
                }

                allFitness[i] = FitnessCalculator.CalculateAllFitnessVars(population[i], fitnessHandler);
            }
        }

        /// <summary>
        /// Evaluates the fitness of an entire population of individuals.
        /// Fitness variables are calculated for each individual, and bounds of fitness variables are determined.
        /// If an individual has been modified or the bounds of fitness variables have changed, their fitness is recalculated.
        /// </summary>
        /// <param name="population">The population of individuals to evaluate.</param>
        public void EvaluatePopulation(RoomIndividual[] population, FitnessHandler fitnessHandler)
        {
            CalculeAllFitnessVarsOfPopulation(population, fitnessHandler);

            // avaliar o individual se ele foi modificado
            for (int i = 0; i < population.Length; i++)
            {
                if (!ShouldRecalculateFitness(population[i]))
                {
                    continue;
                }

                FitnessCalculator.Evaluate(population[i], fitnessHandler, allFitness[i]);
                population[i].Modified = false;
            }
        }
    }
}