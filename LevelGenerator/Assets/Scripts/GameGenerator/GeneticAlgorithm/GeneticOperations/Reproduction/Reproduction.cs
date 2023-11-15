using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using RoomGeneticAlgorithm.Constants;

namespace RoomGeneticAlgorithm.GeneticOperations
{
    public static class Reproduction
    {
        /// <summary>
        /// Performs crossover between a father individual and a mother individual to create a child individual.
        /// </summary>
        /// <param name="parent1">The father individual.</param>
        /// <param name="parent2">The mother individual.</param>
        /// <returns>The child individual resulting from the crossover operation.</returns>
        static RoomIndividual Crossover(RoomIndividual parent1, RoomIndividual parent2)
        {
            RoomIndividual child = new(false);

            Dictionary<RoomContents, List<Position>> enemiesPositionsInParent1 = RoomOperations.GroupPositionsByRoomValue(parent1.RoomMatrix.Values, parent1.RoomMatrix.EnemiesPositions);
            Dictionary<RoomContents, List<Position>> enemiesPositionsInParent2 = RoomOperations.GroupPositionsByRoomValue(parent2.RoomMatrix.Values, parent2.RoomMatrix.EnemiesPositions);

            Dictionary<RoomContents, List<Position>> obstaclesPositionsInParent1 = RoomOperations.GroupPositionsByRoomValue(parent1.RoomMatrix.Values, parent1.RoomMatrix.ObstaclesPositions);
            Dictionary<RoomContents, List<Position>> obstaclesPositionsInParent2 = RoomOperations.GroupPositionsByRoomValue(parent2.RoomMatrix.Values, parent2.RoomMatrix.ObstaclesPositions);

            if (enemiesPositionsInParent2.Keys.Count != enemiesPositionsInParent1.Keys.Count) throw new Exception("Inimigos diferentes no parent1 e no parent2");
            if (obstaclesPositionsInParent2.Keys.Count != obstaclesPositionsInParent1.Keys.Count) throw new Exception("Obstaculos diferentes no parent1 e no parent2");

            ChildContentPlacement.avaliablePositions = new(GeneticAlgorithmConstants.ROOM.ChangeablesPositions);

            ChildContentPlacement.PlaceContentsInChild(child, enemiesPositionsInParent1, enemiesPositionsInParent2);
            ChildContentPlacement.PlaceContentsInChild(child, obstaclesPositionsInParent1, obstaclesPositionsInParent2);

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
            RoomIndividual[] parents = new RoomIndividual[GeneticAlgorithmConstants.NUM_PARENTS_TOURNAMENT];

            for (int i = 0; i < GeneticAlgorithmConstants.NUM_PARENTS_TOURNAMENT; i++)
            {
                List<RoomIndividual> tournament = population.SelectRandomDistinctElements(GeneticAlgorithmConstants.TOURNAMENT_SIZE).ToList();

                // O vencedor do torneio (quem tem a maior fitness dos selecionados aleatoriamente) é selecionado para reprodução
                parents[i] = tournament.MaxBy(individual => individual.Value);
            }

            return parents;
        }

        /// <summary>
        /// Performs reproduction by selecting parents and creating child individuals through crossover.
        /// </summary>
        /// <param name="population">The population of room individuals to perform reproduction on.</param>
        /// <returns>An array of child individuals created through reproduction.</returns>
        public static RoomIndividual[] PerformReproduction(RoomIndividual[] population)
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
                    // Se nao houver cruzamento, copie os fathers diretamente para a nova populacao
                    newPopulation[count] = parents[0];
                    newPopulation[count + 1] = parents[1];
                }
            }

            return newPopulation;
        }
    }
}
