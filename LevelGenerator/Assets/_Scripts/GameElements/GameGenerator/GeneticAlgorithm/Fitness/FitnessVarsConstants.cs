using RoomGeneticAlgorithm.Constants;
using System.Collections.Generic;
using System.Linq;

namespace RoomGeneticAlgorithm.Fitness
{
    public static class FitnessVarsConstants
    {
        public static FitnessVar NUM_ENEMIES_GROUPS = new(new(1, 10), 10, 1f, false, GetGroupCount);
        public static FitnessVar AVERAGE_ENEMIES_PER_GROUP = new(new(1f, 5f), 5f, 1f, true, GetAverageEnemiesPerGroup);
        public static FitnessVar AVERAGE_ENEMY_DOOR_DISTANCE = new(new(1f, 7f), 10f, 1f, false, GetAverageEnemiesDoorsDistance);
        public static FitnessVar AVERAGE_BETWEEN_ENEMIES_DISTANCE = new(new(1f, 7f), 10f, 1f, false, GetAverageDistanceBetweenEnemies);

        static float GetGroupCount(RoomIndividual individual)
        {
            List<int> groupSizes = GroupCounter.GetGroupSizes(individual.RoomMatrix.Values, individual.RoomMatrix.EnemiesPositions);
            return groupSizes.Count;
        }

        static float GetAverageEnemiesPerGroup(RoomIndividual individual)
        {
            List<int> groupSizes = GroupCounter.GetGroupSizes(individual.RoomMatrix.Values, individual.RoomMatrix.EnemiesPositions);
            return (float)groupSizes.Average();
        }

        static float GetAverageEnemiesDoorsDistance(RoomIndividual individual)
        {
            return RoomOperations.AverageDistanceFromDoorsToEnemies(
                individual.RoomMatrix.EnemiesPositions,
                GeneticAlgorithmConstants.ROOM.DoorPositions
            );
        }

        static float GetAverageDistanceBetweenEnemies(RoomIndividual individual)
        {
            return RoomOperations.AverageDistanceBetweenEnemies(individual.RoomMatrix.EnemiesPositions.ToList());
        }
    }
}
