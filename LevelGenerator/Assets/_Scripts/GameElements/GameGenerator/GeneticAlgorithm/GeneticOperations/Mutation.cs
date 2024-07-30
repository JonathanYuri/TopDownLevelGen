using RoomGeneticAlgorithm.Constants;
using System.Collections.Generic;
using UnityEngine;

namespace RoomGeneticAlgorithm.GeneticOperations
{
    public class Mutation
    {
        HashSet<Position> availablePositions;

        readonly SharedRoomData sharedRoomData;

        public Mutation(SharedRoomData sharedRoomData)
        {
            this.sharedRoomData = sharedRoomData;
        }

        /// <summary>
        /// Swaps the positions of two room contents randomly in an individual's room matrix.
        /// </summary>
        /// <param name="individual">The room individual whose room matrix will be modified.</param>
        /// <param name="position1">The first position to swap content with another position.</param>
        void SwapRoomPositionsRandomly(RoomIndividual individual, Position position1)
        {
            if (availablePositions.Count == 0) return;

            Position position2 = availablePositions.GetRandomElement();
            availablePositions.Remove(position2);
            individual.RoomMatrix.SwapPositions(position1, position2);
        }

        /// <summary>
        /// Mutates an individual's room matrix by randomly changing the positions of room contents in the provided set of positions.
        /// </summary>
        /// <param name="individual">The room individual to mutate.</param>
        /// <param name="positionsToMutate">The set of positions in which room contents should be mutated.</param>
        void Mutate(RoomIndividual individual, HashSet<Position> positionsToMutate)
        {
            float minPercentMutations = GeneticAlgorithmConstants.MIN_MUTATIONS_PERCENT * positionsToMutate.Count;
            int numMutations = Random.Range((int)minPercentMutations, positionsToMutate.Count + 1);

            Position[] positionsToChange = positionsToMutate.GetRandomElements(numMutations);
            foreach (Position position in positionsToChange)
            {
                SwapRoomPositionsRandomly(individual, position);
            }
        }

        /// <summary>
        /// Mutates an individual's room matrix by randomly changing the positions of either enemies or obstacles.
        /// </summary>
        /// <param name="individual">The room individual to mutate.</param>
        void Mutate(RoomIndividual individual)
        {
            Mutate(individual, individual.RoomMatrix.EnemiesPositions);
            Mutate(individual, individual.RoomMatrix.ObstaclesPositions);
        }

        /// <summary>
        /// Mutates the entire population by applying mutations to each individual based on the mutation probability.
        /// </summary>
        /// <param name="population">The population of room individuals to mutate.</param>
        public void MutatePopulation(RoomIndividual[] population)
        {
            availablePositions = new(sharedRoomData.ChangeablePositions);
            foreach (RoomIndividual individual in population)
            {
                if (Random.value < GeneticAlgorithmConstants.MUTATION_PROBABILITY)
                {
                    Mutate(individual);
                    individual.Modified = true;
                }
            }
            availablePositions.Clear();
        }
    }
}