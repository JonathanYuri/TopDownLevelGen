using System.Collections.Generic;
using RoomGeneticAlgorithm.Constants;

namespace RoomGeneticAlgorithm.Variables
{

    /// <summary>
    /// Represents a matrix that defines the contents of a room, such as enemies and obstacles.
    /// </summary>
    public class RoomMatrix
    {
        RoomContents[,] values;

        public RoomContents[,] Values { get => values; set => values = value; }
        public HashSet<Position> EnemiesPositions { get; private set; }
        public HashSet<Position> ObstaclesPositions { get; private set; }

        public RoomMatrix(RoomContents[,] values)
        {
            this.values = values;
            EnemiesPositions = new();
            ObstaclesPositions = new();
        }

        /// <summary>
        /// Puts the specified content in the given position of the room.
        /// </summary>
        /// <param name="content">The content to place in the room.</param>
        /// <param name="position">The position where the content should be placed.</param>
        public void PutContentInPosition(RoomContents content, Position position)
        {
            if (Utils.IsAEnemy(GeneticAlgorithmConstants.ROOM.Enemies, content))
            {
                EnemiesPositions.Add(position);
            }
            else if (Utils.IsAObstacle(GeneticAlgorithmConstants.ROOM.Obstacles, content))
            {
                ObstaclesPositions.Add(position);
            }

            Values[position.X, position.Y] = content;
        }


        void RemoveFromPosition(Position position)
        {
            RoomContents content = Values[position.X, position.Y];
            if (Utils.IsAEnemy(GeneticAlgorithmConstants.ROOM.Enemies, content))
            {
                EnemiesPositions.Remove(position);
            }
            else if (Utils.IsAObstacle(GeneticAlgorithmConstants.ROOM.Obstacles, content))
            {
                ObstaclesPositions.Remove(position);
            }
        }

        /// <summary>
        /// Updates the content in the specified position by changing it to a new content.
        /// </summary>
        /// <param name="position">The position of the content to be updated.</param>
        /// <param name="changeTo">The content to replace it with.</param>
        void UpdateContentInPosition(Position position, RoomContents changeTo)
        {
            RemoveFromPosition(position);
            PutContentInPosition(changeTo, position);
        }

        public void SwapPositions(Position position1, Position position2)
        {
            RoomContents content1 = Values[position1.X, position1.Y];
            RoomContents content2 = Values[position2.X, position2.Y];

            UpdateContentInPosition(position1, content2);
            UpdateContentInPosition(position2, content1);
        }
    }
}