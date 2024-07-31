using System.Collections.Generic;
using System.Linq;

namespace RoomGeneticAlgorithm.Variables
{

    /// <summary>
    /// Represents a matrix that defines the contents of a room, such as enemies and obstacles.
    /// </summary>
    public class RoomMatrix
    {
        RoomContents[,] values;
        readonly SharedRoomData sharedRoomData;

        public Dictionary<RoomContents, HashSet<Position>> EnemyTypeToPositions { get; private set; } = new();
        public Dictionary<RoomContents, HashSet<Position>> ObstacleTypeToPositions { get; private set; } = new();

        public RoomContents[,] Values { get => values; set => values = value; }
        public HashSet<Position> EnemiesPositions { get; private set; } = new();
        public HashSet<Position> ObstaclesPositions { get; private set; } = new();
        public SharedRoomData SharedRoomData => sharedRoomData;

        public RoomMatrix(SharedRoomData sharedRoomData)
        {
            this.sharedRoomData = sharedRoomData;
            values = (RoomContents[,])sharedRoomData.Values.Clone();

            foreach (RoomContents enemy in sharedRoomData.Enemies)
            {
                if (!EnemyTypeToPositions.ContainsKey(enemy))
                {
                    EnemyTypeToPositions.Add(enemy, new());
                }
            }

            foreach (RoomContents obstacle in sharedRoomData.Obstacles)
            {
                if (!ObstacleTypeToPositions.ContainsKey(obstacle))
                {
                    ObstacleTypeToPositions.Add(obstacle, new());
                }
            }
        }

        /// <summary>
        /// Puts the specified content in the given position of the room.
        /// </summary>
        /// <param name="content">The content to place in the room.</param>
        /// <param name="position">The position where the content should be placed.</param>
        public void PutContentInPosition(RoomContents content, Position position)
        {
            if (SharedRoomData.Enemies.Contains(content))
            {
                EnemiesPositions.Add(position);
                EnemyTypeToPositions[content].Add(position);
            }
            else if (SharedRoomData.Obstacles.Contains(content))
            {
                ObstaclesPositions.Add(position);
                ObstacleTypeToPositions[content].Add(position);
            }

            Values[position.X, position.Y] = content;
        }


        void RemoveFromPosition(Position position)
        {
            RoomContents content = Values[position.X, position.Y];
            if (SharedRoomData.Enemies.Contains(content))
            {
                EnemiesPositions.Remove(position);
                EnemyTypeToPositions[content].Remove(position);
            }
            else if (SharedRoomData.Obstacles.Contains(content))
            {
                ObstaclesPositions.Remove(position);
                ObstacleTypeToPositions[content].Remove(position);
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
            if (position1.Equals(position2))
            {
                return;
            }

            RoomContents content1 = Values[position1.X, position1.Y];
            RoomContents content2 = Values[position2.X, position2.Y];

            UpdateContentInPosition(position1, content2);
            UpdateContentInPosition(position2, content1);
        }
    }
}