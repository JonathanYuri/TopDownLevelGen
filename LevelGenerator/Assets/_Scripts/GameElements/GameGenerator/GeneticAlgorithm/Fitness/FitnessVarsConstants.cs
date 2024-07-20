using RoomGeneticAlgorithm.Constants;
using System.Collections.Generic;
using System.Linq;

namespace RoomGeneticAlgorithm.Fitness
{
    public static class FitnessVarsConstants
    {
        public static FitnessVar NUM_ENEMIES_GROUPS = new(new(1, 10), 1f, false, GetGroupCount);
        public static FitnessVar ENEMIES_PER_GROUP_AVERAGE = new(new(1f, 5f), 1f, true, GetEnemiesPerGroupAverage);
        public static FitnessVar ENEMY_DOOR_DISTANCE = new(new(1, 90), 1f, false, GetEnemiesDoorsDistance);
        public static FitnessVar BETWEEN_ENEMIES_DISTANCE = new(new(1, 90), 1f, false, GetBetweenEnemiesDistance);

        static float GetGroupCount(RoomIndividual individual)
        {
            List<int> groupSizes = GroupCounter.GetGroupSizes(individual.RoomMatrix.Values, individual.RoomMatrix.EnemiesPositions);
            return groupSizes.Count;
        }

        static float GetEnemiesPerGroupAverage(RoomIndividual individual)
        {
            List<int> groupSizes = GroupCounter.GetGroupSizes(individual.RoomMatrix.Values, individual.RoomMatrix.EnemiesPositions);
            return (float)groupSizes.Average();
        }

        static float GetEnemiesDoorsDistance(RoomIndividual individual)
        {
            return RoomOperations.DistanceFromDoorsToEnemies(
                individual.RoomMatrix.EnemiesPositions,
                GeneticAlgorithmConstants.ROOM.DoorPositions
            );
        }

        static float GetBetweenEnemiesDistance(RoomIndividual individual)
        {
            return RoomOperations.DistanceBetweenEnemies(individual.RoomMatrix.EnemiesPositions.ToList());
        }
    }
}
