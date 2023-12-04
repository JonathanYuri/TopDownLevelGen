using System.Collections;
using UnityEngine;
using RoomGeneticAlgorithm.Run;
using SpawnRoomObjects.SpawnAll;

[RequireComponent(typeof(LevelGenerator))]
[RequireComponent(typeof(RoomObjectSpawner))]
public class RoomGenerator : MonoBehaviour
{
    LevelDataManager levelDataManager;
    LevelGenerator levelGenerator;
    RoomObjectSpawner roomObjectSpawner;

    [SerializeField] GameObject roomPrefab;

    void Awake()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        roomObjectSpawner = GetComponent<RoomObjectSpawner>();
    }

    void Start()
    {
        levelDataManager = FindObjectOfType<LevelDataManager>();
    }

    /// <summary>
    /// Generates a room object at the specified position, with optional content generation.
    /// </summary>
    /// <param name="roomPosition">The position where the room object will be generated.</param>
    /// <returns>GameObject of the room</returns>
    GameObject GenerateRoomGameObject(Position roomPosition)
    {
        Vector2 roomObjectPosition = Utils.TransformAMapPositionIntoAUnityPosition(roomPosition);
        GameObject roomObject = Instantiate(roomPrefab, roomObjectPosition, Quaternion.identity, levelGenerator.Rooms);
        return roomObject;
    }

    /// <summary>
    /// Generates a room within the specified room object and position.
    /// </summary>
    /// <param name="roomPosition">The position of the room to be generated.</param>
    /// <param name="generateObjectsInRoom">Determines whether to generate objects in the room (default: true).</param>
    /// <returns>An IEnumerator for asynchronous room generation.</returns>
    public void GenerateRoom(Position roomPosition, bool generateObjectsInRoom = true)
    {
        GameObject roomObject = GenerateRoomGameObject(roomPosition);
        RoomData roomData = GetRoomData(roomPosition);

        RoomSkeleton room = new(roomData);
        
        if (generateObjectsInRoom)
        {
            float startTime = Time.realtimeSinceStartup;

            room.Values = GeneticRoomGenerator.GeneticLooping(room);

            float endTime = Time.realtimeSinceStartup;

            Debug.LogError("Tempo de execução da corrotina: " + (endTime - startTime) + " segundos");
            Debug.LogError("Tempo total de execução ate agora: " + endTime + " segundos");
        }

        if (IsFinalRoom(roomPosition))
        {
            room.Values[GameConstants.ROOM_MIDDLE.X, GameConstants.ROOM_MIDDLE.Y] = RoomContents.LevelEnd;
        }

        roomObjectSpawner.SpawnRoomObjects(room, roomObject);
    }

    bool IsFinalRoom(Position roomPosition) => levelGenerator.FinalRoomPosition != null && levelGenerator.FinalRoomPosition.Equals(roomPosition);

    RoomData GetRoomData(Position roomPosition)
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
            MapUtility.GetDoorPositionsFromRoomPosition(roomPosition),
            Knapsack.ResolveKnapsack(enemyKnapsackParams),
            Knapsack.ResolveKnapsack(obstacleKnapsackParams),
            difficulty
        );
    }
}

public struct RoomData
{
    public Position[] doorPositions;
    public RoomContents[] enemies;
    public RoomContents[] obstacles;
    public float difficulty;

    public RoomData(Position[] doorPositions, RoomContents[] enemies, RoomContents[] obstacles, float difficulty)
    {
        this.doorPositions = doorPositions;
        this.enemies = enemies;
        this.obstacles = obstacles;
        this.difficulty = difficulty;
    }
}
