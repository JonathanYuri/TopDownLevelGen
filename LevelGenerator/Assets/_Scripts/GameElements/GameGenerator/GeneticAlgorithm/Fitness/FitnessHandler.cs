namespace RoomGeneticAlgorithm.Fitness
{
    /// <summary>
    /// Manages fitness calculation for room individuals in the genetic algorithm.
    /// </summary>
    public class FitnessHandler
    {
        readonly FitnessCalculator fitnessCalculator;
        public FitnessCalculator FitnessCalculator => fitnessCalculator;

        public FitnessHandler(float roomDifficulty)
        {
            fitnessCalculator = new(roomDifficulty);
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
        public void EvaluatePopulation(RoomIndividual[] population)
        {
            /*
            Parallel.For(0, population.Length, i =>
            {
                if (ShouldRecalculateFitness(population[i]))
                {
                    FitnessCalculator.Evaluate(population[i]);
                    population[i].Modified = false;
                }
            });
            */
            for (int i = 0; i < population.Length; i++)
            {
                if (ShouldRecalculateFitness(population[i]))
                {
                    FitnessCalculator.Evaluate(population[i]);
                    population[i].Modified = false;
                }
            }
        }
    }
}