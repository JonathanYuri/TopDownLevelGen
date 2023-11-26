using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoomGeneticAlgorithm.Constants;

namespace RoomGeneticAlgorithm.GeneticOperations
{
    public static class Mutation
    {
        /// <summary>
        /// Swaps the positions of two room contents randomly in an individual's room matrix.
        /// </summary>
        /// <param name="individual">The room individual whose room matrix will be modified.</param>
        /// <param name="position1">The first position to swap content with another position.</param>
        static void SwapRoomPositionsRandomly(RoomIndividual individual, Position position1)
        {
            int idx2 = Random.Range(0, GeneticAlgorithmConstants.ROOM.ChangeablesPositions.Count);
            Position position2 = GeneticAlgorithmConstants.ROOM.ChangeablesPositions.ElementAt(idx2);

            individual.RoomMatrix.SwapPositions(position1, position2);
        }

        /// <summary>
        /// Mutates an individual's room matrix by randomly changing the positions of room contents in the provided set of positions.
        /// </summary>
        /// <param name="individual">The room individual to mutate.</param>
        /// <param name="positionsToMutate">The set of positions in which room contents should be mutated.</param>
        static void Mutate(RoomIndividual individual, HashSet<Position> positionsToMutate)
        {
            Position[] positionsToChange = positionsToMutate.SelectRandomDistinctElements(Random.Range(0, positionsToMutate.Count + 1));
            foreach (Position position in positionsToChange)
            {
                SwapRoomPositionsRandomly(individual, position);
            }
        }

        /// <summary>
        /// Mutates an individual's room matrix by randomly changing the positions of either enemies or obstacles.
        /// </summary>
        /// <param name="individual">The room individual to mutate.</param>
        static void Mutate(RoomIndividual individual)
        {
            // escolher inimigos ou obstaculos para mudar
            if (Random.value < 0.5f)
            {
                Mutate(individual, individual.RoomMatrix.EnemiesPositions);
            }
            else
            {
                Mutate(individual, individual.RoomMatrix.ObstaclesPositions);
            }
        }

        /// <summary>
        /// Mutates the entire population by applying mutations to each individual based on the mutation probability.
        /// </summary>
        /// <param name="population">The population of room individuals to mutate.</param>
        public static void MutatePopulation(RoomIndividual[] population)
        {
            foreach (RoomIndividual individual in population)
            {
                if (Random.value < GeneticAlgorithmConstants.MUTATION_PROBABILITY)
                {
                    Mutate(individual);
                    individual.ItWasModified = true;
                }
            }
        }
    }
}