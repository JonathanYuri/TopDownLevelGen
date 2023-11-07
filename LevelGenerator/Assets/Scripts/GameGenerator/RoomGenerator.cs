using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    private void Start()
    {
        levelDataManager = FindObjectOfType<LevelDataManager>();
    }

    /// <summary>
    /// Generates a room object at the specified position, with optional content generation.
    /// </summary>
    /// <param name="roomPosition">The position where the room object will be generated.</param>
    /// <returns>GameObject of the room</returns>
    GameObject GenerateRoomObject(Position roomPosition)
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
    public IEnumerator GenerateRoom(Position roomPosition, bool generateObjectsInRoom = true)
    {
        GameObject roomObject = GenerateRoomObject(roomPosition);
        RoomData roomData = GetRoomData(roomPosition);

        Room room = new(roomData.doorPositions, roomData.enemies, roomData.obstacles, roomData.difficulty);
        GeneticRoomGenerator geneticRoomGenerator = new(room);

        if (generateObjectsInRoom)
        {
            room.Values = geneticRoomGenerator.GeneticLooping();
            yield return null;
        }

        if (IsFinalRoom(roomPosition))
        {
            room.Values[GameConstants.ROOM_MIDDLE.X, GameConstants.ROOM_MIDDLE.Y] = RoomContents.Portal;
        }

        roomObjectSpawner.SpawnRoomObjects(room, roomObject);
    }

    bool IsFinalRoom(Position roomPosition) => levelGenerator.FinalRoomPosition != null && levelGenerator.FinalRoomPosition.Equals(roomPosition);

    RoomData GetRoomData(Position roomPosition)
    {
        int distanceToInitialRoom = Utils.CalculateDistance(levelGenerator.InitialRoomPosition, roomPosition);
        float difficulty = (float)distanceToInitialRoom / (float)levelGenerator.DistanceFromInitialToFinalRoom;

        return new(
                MapUtility.GetDoorPositionsFromRoomPosition(levelGenerator.Map, roomPosition),
                Knapsack.ResolveKnapsackEnemies(levelDataManager.Enemies, levelDataManager.EnemiesValues, levelDataManager.EnemiesCapacity),
                Knapsack.ResolveKnapsackObstacles(levelDataManager.Obstacles, levelDataManager.ObstaclesValues, levelDataManager.ObstaclesCapacity),
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
