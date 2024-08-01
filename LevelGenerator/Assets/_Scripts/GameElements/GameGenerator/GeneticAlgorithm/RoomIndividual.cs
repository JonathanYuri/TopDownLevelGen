using RoomGeneticAlgorithm.Variables;

namespace RoomGeneticAlgorithm
{
    /// <summary>
    /// Represents an individual in a genetic algorithm, specifically designed for room layout optimization.
    /// </summary>
    public class RoomIndividual
    {
        public int Value { get; set; }
        public bool Modified { get; set; }
        public RoomMatrix RoomMatrix { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomIndividual"/> class, optionally generating it randomly.
        /// </summary>
        /// <param name="generateRandomly">Determines whether to generate the room layout randomly.</param>
        public RoomIndividual(SharedRoomData sharedRoomData, bool generateRandomly = true)
        {
            Value = default;
            Modified = true;
            RoomMatrix = new RoomMatrix(sharedRoomData);

            if (generateRandomly)
            {
                GenerateRoomRandomly(sharedRoomData);
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
        void GenerateRoomRandomly(SharedRoomData sharedRoomData)
        {
            RoomContents[] enemies = sharedRoomData.Enemies;
            RoomContents[] obstacles = sharedRoomData.Obstacles;

            Position[] chosenPositions = sharedRoomData.ChangeablePositions.GetRandomElements(enemies.Length + obstacles.Length);

            int count = 0;
            foreach (RoomContents enemy in enemies)
            {
                RoomMatrix.PutContentInPosition(enemy, chosenPositions[count]);
                count++;
            }

            foreach (RoomContents obstacle in obstacles)
            {
                RoomMatrix.PutContentInPosition(obstacle, chosenPositions[count]);
                count++;
            }
        }
    }
}