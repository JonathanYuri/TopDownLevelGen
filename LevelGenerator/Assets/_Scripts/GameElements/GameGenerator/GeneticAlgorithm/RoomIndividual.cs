using System.Collections.Generic;
using RoomGeneticAlgorithm.Constants;
using RoomGeneticAlgorithm.Variables;

namespace RoomGeneticAlgorithm
{
    /// <summary>
    /// Represents an individual in a genetic algorithm, specifically designed for room layout optimization.
    /// </summary>
    public class RoomIndividual
    {
        public int Value { get; set; }
        public bool ItWasModified { get; set; }
        public RoomMatrix RoomMatrix { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomIndividual"/> class, optionally generating it randomly.
        /// </summary>
        /// <param name="generateRandomly">Determines whether to generate the room layout randomly.</param>
        public RoomIndividual(bool generateRandomly = true)
        {
            Value = default;
            ItWasModified = true;
            RoomMatrix = new RoomMatrix((RoomContents[,])GeneticAlgorithmConstants.ROOM.Values.Clone());

            if (generateRandomly)
            {
                GenerateRoomRandomly();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomIndividual"/> class as a copy of another individual.
        /// </summary>
        /// <param name="individual">The individual to copy.</param>
        public RoomIndividual(RoomIndividual individual)
        {
            Value = individual.Value;
            RoomMatrix = individual.RoomMatrix;
        }

        /// <summary>
        /// Generates a random room layout by placing enemies and obstacles in available positions.
        /// </summary>
        void GenerateRoomRandomly()
        {
            List<Position> avaliablePositions = new(GeneticAlgorithmConstants.ROOM.ChangeablesPositions);
            int qntObjects = GeneticAlgorithmConstants.ROOM.Enemies.Length + GeneticAlgorithmConstants.ROOM.Obstacles.Length;
            Position[] chosenPositions = avaliablePositions.SelectRandomDistinctElements(qntObjects);

            int count = 0;
            foreach (RoomContents enemy in GeneticAlgorithmConstants.ROOM.Enemies)
            {
                RoomMatrix.PutContentInPosition(enemy, chosenPositions[count],
                    GeneticAlgorithmConstants.ROOM.Enemies, GeneticAlgorithmConstants.ROOM.Obstacles);
                count++;
            }

            foreach (RoomContents obstacle in GeneticAlgorithmConstants.ROOM.Obstacles)
            {
                RoomMatrix.PutContentInPosition(obstacle, chosenPositions[count],
                    GeneticAlgorithmConstants.ROOM.Enemies, GeneticAlgorithmConstants.ROOM.Obstacles);
                count++;
            }
        }
    }

}