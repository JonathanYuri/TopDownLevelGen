using System.Collections;
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

        KnapsackSelectionResult knapsackSelectionResult = Knapsack.ChooseEnemiesAndObstaclesToKnapsack(
            levelDataManager.Enemies, levelDataManager.EnemiesDifficulty,
            levelDataManager.Obstacles, levelDataManager.ObstaclesDifficulty
        );

        KnapsackParams enemyKnapsackParams = new(knapsackSelectionResult.ChosenEnemies, knapsackSelectionResult.ChosenEnemiesDifficulty, levelDataManager.EnemiesCapacity);
        KnapsackParams obstacleKnapsackParams = new(knapsackSelectionResult.ChosenObstacles, knapsackSelectionResult.ChosenObstaclesDifficulty, levelDataManager.ObstaclesCapacity);

        return new(
            MapUtility.GetDoorPositionsFromRoomPosition(roomPosition, map),
            Knapsack.ResolveKnapsack(enemyKnapsackParams),
            Knapsack.ResolveKnapsack(obstacleKnapsackParams),
            difficulty
        );
    }
}
