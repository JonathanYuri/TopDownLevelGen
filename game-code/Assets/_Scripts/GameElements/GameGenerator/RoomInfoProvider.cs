using System.Collections.Generic;
using UnityEngine;

public class RoomInfoProvider : MonoBehaviour
{
    LevelGenerator levelGenerator;
    LevelDataManager levelDataManager;

    void Start()
    {
        levelGenerator = FindObjectOfType<LevelGenerator>();
        levelDataManager = FindObjectOfType<LevelDataManager>();
    }

    public bool IsFinalRoom(Position roomPosition) => levelGenerator.FinalRoomPosition != null && levelGenerator.FinalRoomPosition.Equals(roomPosition);

    public RoomData GetRoomData(Position roomPosition, HashSet<Position> map)
    {
        int distanceToInitialRoom = Utils.CalculateDistance(levelGenerator.InitialRoomPosition, roomPosition);
        float difficulty = (float)distanceToInitialRoom / (float)levelGenerator.DistanceFromInitialToFinalRoom;

        SubsetSumSelectionResult subsetSumSelectionResult = SubsetSumSolver.ChooseEnemiesAndObstaclesToKnapsack(
            levelDataManager.Enemies, levelDataManager.EnemiesDifficulty,
            levelDataManager.Obstacles, levelDataManager.ObstaclesDifficulty
        );

        SubsetSumParams enemySubsetParams = new(subsetSumSelectionResult.ChosenEnemies, subsetSumSelectionResult.ChosenEnemiesDifficulty, levelDataManager.EnemiesDifficultyBudget);
        SubsetSumParams obstacleSubsetParams = new(subsetSumSelectionResult.ChosenObstacles, subsetSumSelectionResult.ChosenObstaclesDifficulty, levelDataManager.ObstaclesDifficultyBudget);

        return new(
            MapUtility.GetDoorPositionsFromRoomPosition(roomPosition, map),
            SubsetSumSolver.ResolveSubsetSum(enemySubsetParams),
            SubsetSumSolver.ResolveSubsetSum(obstacleSubsetParams),
            difficulty
        );
    }
}
