using RoomGeneticAlgorithm.Constants;
using System.Collections.Generic;
using System.Linq;

namespace RoomGeneticAlgorithm.Fitness
{
    public static class FitnessVarsConstants
    {
        public static readonly FitnessVar NUM_ENEMIES_GROUP =
            new("NumEnemiesGroup", new(1, 10), 10, 1f, false, GetGroupCount);

        public static readonly FitnessVar AVERAGE_ENEMIES_PER_GROUP =
            new("AverageEnemiesPerGroup", new(1f, 5f), 5f, 1f, true, GetAverageEnemiesPerGroup);

        public static readonly FitnessVar AVERAGE_ENEMY_DOOR_DISTANCE =
            new("AverageEnemyDoorDistance", new(1f, 7f), 10f, 1f, false, GetAverageEnemiesDoorsDistance);

        public static readonly FitnessVar AVERAGE_BETWEEN_ENEMIES_DISTANCE =
            new("AverageBetweenEnemiesDistances", new(1f, 7f), 10f, 1f, false, GetAverageDistanceBetweenEnemies);

        public static readonly FitnessVar AVERAGE_OBSTACLES_NEXT_TO_ENEMIES =
            new("AverageObstaclesNextToEnemies", new(0f, 3f), 2f, 1f, false, GetAverageObstaclesNextToEnemies);

        public static readonly FitnessVar AVERAGE_ENEMIES_WITH_COVER =
            new("AverageEnemiesWithCover", new(0f, 1f), 0.5f, 1f, true, GetAverageEnemiesWithCover);

        public static readonly FitnessVar[] VARS = new FitnessVar[]
        {
            NUM_ENEMIES_GROUP, AVERAGE_ENEMIES_PER_GROUP, AVERAGE_ENEMY_DOOR_DISTANCE,
            AVERAGE_BETWEEN_ENEMIES_DISTANCE, AVERAGE_OBSTACLES_NEXT_TO_ENEMIES, AVERAGE_ENEMIES_WITH_COVER
        };

        public static float GetGroupCount(RoomIndividual individual)
        {
            List<int> groupSizes = GroupCounter.GetGroupSizes(
                individual.RoomMatrix.Values, individual.RoomMatrix.EnemiesPositions);
            return groupSizes.Count;
        }

        public static float GetAverageEnemiesPerGroup(RoomIndividual individual)
        {
            List<int> groupSizes = GroupCounter.GetGroupSizes(
                individual.RoomMatrix.Values, individual.RoomMatrix.EnemiesPositions);
            return (float)groupSizes.Average();
        }

        public static float GetAverageEnemiesDoorsDistance(RoomIndividual individual)
        {
            return RoomOperations.AverageDistanceFromDoorsToEnemies(
                individual.RoomMatrix.EnemiesPositions,
                individual.RoomMatrix.SharedRoomData.DoorPositions
            );
        }

        public static float GetAverageDistanceBetweenEnemies(RoomIndividual individual)
        {
            return RoomOperations.AverageDistanceBetweenEnemies(individual.RoomMatrix.EnemiesPositions.ToList());
        }

        public static float GetAverageObstaclesNextToEnemies(RoomIndividual individual)
        {
            return RoomOperations.AverageObstaclesNextToEnemies(individual.RoomMatrix);
        }

        public static float GetAverageEnemiesWithCover(RoomIndividual individual)
        {
            return RoomOperations.AverageEnemiesWithCover(individual.RoomMatrix);
        }
    }
}
