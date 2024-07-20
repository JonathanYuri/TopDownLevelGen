using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoomGeneticAlgorithm.Constants;
using System.Text;

namespace RoomGeneticAlgorithm.GeneticOperations
{
    public static class Mutation
    {
        // pelo menos 70 % do hashset vai ser mutado
        readonly static float minMutations = 0.7f;
        static HashSet<Position> positions;

        /// <summary>
        /// Swaps the positions of two room contents randomly in an individual's room matrix.
        /// </summary>
        /// <param name="individual">The room individual whose room matrix will be modified.</param>
        /// <param name="position1">The first position to swap content with another position.</param>
        static void SwapRoomPositionsRandomly(RoomIndividual individual, Position position1)
        {
            if (positions.Count == 0) return;

            int idx2 = Random.Range(0, positions.Count);
            Position position2 = positions.ElementAt(idx2);
            positions.Remove(position2);
            individual.RoomMatrix.SwapPositions(position1, position2, GeneticAlgorithmConstants.ROOM.Enemies, GeneticAlgorithmConstants.ROOM.Obstacles);
        }

        /// <summary>
        /// Mutates an individual's room matrix by randomly changing the positions of room contents in the provided set of positions.
        /// </summary>
        /// <param name="individual">The room individual to mutate.</param>
        /// <param name="positionsToMutate">The set of positions in which room contents should be mutated.</param>
        static void Mutate(RoomIndividual individual, HashSet<Position> positionsToMutate)
        {
            float minPercentMutations = minMutations * positionsToMutate.Count;
            int numMutations = Random.Range((int)minPercentMutations, positionsToMutate.Count + 1);

            Position[] positionsToChange = positionsToMutate.SelectRandomDistinctElements(numMutations);
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
            Mutate(individual, individual.RoomMatrix.EnemiesPositions);
            Mutate(individual, individual.RoomMatrix.ObstaclesPositions);
        }

        /// <summary>
        /// Mutates the entire population by applying mutations to each individual based on the mutation probability.
        /// </summary>
        /// <param name="population">The population of room individuals to mutate.</param>
        public static void MutatePopulation(RoomIndividual[] population)
        {
            positions = new(GeneticAlgorithmConstants.ROOM.ChangeablesPositions);
            foreach (RoomIndividual individual in population)
            {
                if (Random.value < GeneticAlgorithmConstants.MUTATION_PROBABILITY)
                {
                    Mutate(individual);
                    individual.Modified = true;
                }
            }
            positions.Clear();
        }
    }
}