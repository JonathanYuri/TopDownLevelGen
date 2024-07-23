using RoomGeneticAlgorithm.Constants;
using System.Collections.Generic;
using System.Linq;

namespace RoomGeneticAlgorithm.Fitness
{
    public static class FitnessVarsConstants
    {
        public static readonly FitnessVar NUM_ENEMIES_GROUP =
            new(new(1, 10), 10, 1f, false, GetGroupCount);
        public static readonly FitnessVar AVERAGE_ENEMIES_PER_GROUP =
            new(new(1f, 5f), 5f, 1f, true, GetAverageEnemiesPerGroup);
        public static readonly FitnessVar AVERAGE_ENEMY_DOOR_DISTANCE =
            new(new(1f, 7f), 10f, 1f, false, GetAverageEnemiesDoorsDistance);
        public static readonly FitnessVar AVERAGE_BETWEEN_ENEMIES_DISTANCE =
            new(new(1f, 7f), 10f, 1f, false, GetAverageDistanceBetweenEnemies);

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
    }
}
