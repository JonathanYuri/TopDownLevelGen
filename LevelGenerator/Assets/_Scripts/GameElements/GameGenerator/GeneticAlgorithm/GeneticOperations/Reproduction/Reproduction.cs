using RoomGeneticAlgorithm.Constants;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace RoomGeneticAlgorithm.GeneticOperations
{
    public class Reproduction
    {
        readonly ChildContentPlacement childContentPlacement;
        readonly SharedRoomData sharedRoomData;

        public Reproduction(SharedRoomData sharedRoomData)
        {
            childContentPlacement = new();
            this.sharedRoomData = sharedRoomData;
        }

        /// <summary>
        /// Performs crossover between a father individual and a mother individual to create a child individual.
        /// </summary>
        /// <param name="parent1">The father individual.</param>
        /// <param name="parent2">The mother individual.</param>
        /// <returns>The child individual resulting from the crossover operation.</returns>
        RoomIndividual Crossover(RoomIndividual parent1, RoomIndividual parent2)
        {
            RoomIndividual child = new(sharedRoomData, false);

            var enemiesPositionsInParent1 = parent1.RoomMatrix.EnemyTypeToPositions;
            var enemiesPositionsInParent2 = parent2.RoomMatrix.EnemyTypeToPositions;

            var obstaclesPositionsInParent1 = parent1.RoomMatrix.ObstacleTypeToPositions;
            var obstaclesPositionsInParent2 = parent2.RoomMatrix.ObstacleTypeToPositions;

            if (enemiesPositionsInParent2.Keys.Count != enemiesPositionsInParent1.Keys.Count)
                throw new Exception("Inimigos diferentes no parent1 e no parent2");
            if (obstaclesPositionsInParent2.Keys.Count != obstaclesPositionsInParent1.Keys.Count)
                throw new Exception("Obstaculos diferentes no parent1 e no parent2");

            childContentPlacement.SetAvaliablePositions(sharedRoomData.ChangeablePositions);

            childContentPlacement.PlaceContentsInChild(child, enemiesPositionsInParent1, enemiesPositionsInParent2);
            childContentPlacement.PlaceContentsInChild(child, obstaclesPositionsInParent1, obstaclesPositionsInParent2);

            if (child.RoomMatrix.EnemiesPositions.Count != parent1.RoomMatrix.EnemiesPositions.Count
                                            ||
                child.RoomMatrix.ObstaclesPositions.Count != parent1.RoomMatrix.ObstaclesPositions.Count)
            {
                throw new ReproductionException("Gerou individual monstro na Reproducao",
                    parent1.RoomMatrix.EnemiesPositions, parent2.RoomMatrix.EnemiesPositions, child.RoomMatrix.EnemiesPositions,
                    parent1.RoomMatrix.ObstaclesPositions, parent2.RoomMatrix.ObstaclesPositions, child.RoomMatrix.ObstaclesPositions);
            }
            return child;
        }

        /// <summary>
        /// Selects parents for reproduction using tournament selection.
        /// </summary>
        /// <param name="population">The population of room individuals to select parents from.</param>
        /// <returns>An array of parent individuals selected for reproduction.</returns>
        static RoomIndividual[] TournamentSelection(RoomIndividual[] population)
        {
            // para nao selecionar o mesmo individuo que ganhou o torneio
            List<RoomIndividual> populationCopy = new(population);
            RoomIndividual[] parents = new RoomIndividual[GeneticAlgorithmConstants.NUM_PARENTS_TOURNAMENT];
            for (int i = 0; i < GeneticAlgorithmConstants.NUM_PARENTS_TOURNAMENT; i++)
            {
                RoomIndividual[] tournament = populationCopy.GetRandomElements(GeneticAlgorithmConstants.TOURNAMENT_SIZE);
                RoomIndividual winner = tournament.GetBestIndividual();
                parents[i] = winner;
                populationCopy.Remove(winner);
            }

            return parents;
        }

        /// <summary>
        /// Performs reproduction by selecting parents and creating child individuals through crossover.
        /// </summary>
        /// <param name="population">The population of room individuals to perform reproduction on.</param>
        /// <returns>An array of child individuals created through reproduction.</returns>
        public RoomIndividual[] PerformReproduction(RoomIndividual[] population)
        {
            RoomIndividual[] newPopulation = new RoomIndividual[GeneticAlgorithmConstants.POPULATION_SIZE];

            for (int count = 0; count < GeneticAlgorithmConstants.POPULATION_SIZE; count += 2)
            {
                RoomIndividual[] parents = TournamentSelection(population);

                if (Random.value < GeneticAlgorithmConstants.CROSSOVER_PROBABILITY)
                {
                    RoomIndividual children1 = Crossover(parents[0], parents[1]);
                    RoomIndividual children2 = Crossover(parents[1], parents[0]);

                    newPopulation[count] = children1;
                    newPopulation[count + 1] = children2;
                }
                else
                {
                    newPopulation[count] = parents[0];
                    newPopulation[count + 1] = parents[1];
                }
            }

            return newPopulation;
        }
    }
}
