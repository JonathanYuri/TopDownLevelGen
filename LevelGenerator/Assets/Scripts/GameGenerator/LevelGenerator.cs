using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Generates and manages levels using a room-based approach.
/// </summary>
[RequireComponent(typeof(RoomObjectSpawner))]
public class LevelGenerator : MonoBehaviour
{
    LevelDataManager levelDataManager;
    float totalCoroutineExecutionTime = 0f;

    [SerializeField] GameObject rooms;
    [SerializeField] GameObject room;

    HashSet<Position> map;

    Position initialRoomPosition;
    Position finalRoomPosition;
    int distanceFromInitialToFinalRoom;

    RoomObjectSpawner roomObjectSpawner;

    private void Awake()
    {
        map = new();
    }

    private void Start()
    {
        roomObjectSpawner = GetComponent<RoomObjectSpawner>();
        levelDataManager = FindFirstObjectByType<LevelDataManager>();
    }

    /// <summary>
    /// Generates a level, including its map.
    /// </summary>
    /// <returns>A collection of positions representing the generated map.</returns>
    public HashSet<Position> Generate()
    {
        DestroyAllPastObjects();

        GenerateMap();
        GenerateInitialRoom();
        GenerateFinalRoom();
        distanceFromInitialToFinalRoom = Utils.CalculateDistance(initialRoomPosition, finalRoomPosition);
        GenerateRemainingRooms();
        return map;
    }

    /// <summary>
    /// Destroys all previously generated objects and clears the map.
    /// </summary>
    void DestroyAllPastObjects()
    {
        map.Clear();
        for (int i = 0; i < rooms.transform.childCount; i++)
        {
            Destroy(rooms.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Generates a map of positions for the level layout based on a random and dynamic room generation algorithm.
    /// </summary>
    void GenerateMap()
    {
        totalCoroutineExecutionTime = 0;
        Queue<Position> queue = new();
        queue.Enqueue(new Position { X = 0, Y = 0 });

        while (map.Count < levelDataManager.RoomCount)
        {
            if (queue.Count == 0)
            {
                int pos = UnityEngine.Random.Range(0, map.Count);
                queue.Enqueue(map.ElementAt(pos));
            }

            Position position = queue.Dequeue();
            map.Add(position);

            Direction[] shuffledArr = Enum.GetValues(typeof(Direction)).Cast<Direction>().Shuffle();
            foreach (Direction direction in shuffledArr)
            {
                Position adjacentPosition = position.Move(direction);

                if (!map.Contains(adjacentPosition) && UnityEngine.Random.value < GameConstants.PROBABILITY_OF_GENERATING_ROOM_IN_NEIGHBORHOOD)
                {
                    queue.Enqueue(adjacentPosition);
                }
            }
        }
    }

    /// <summary>
    /// Generates the initial room at the starting position of the level layout.
    /// </summary>
    void GenerateInitialRoom()
    {
        initialRoomPosition = new() { X = 0, Y = 0 };
        GenerateRoomObject(initialRoomPosition, false); // gerar so o esqueleto
    }

    /// <summary>
    /// Generates the final room at a calculated position within the level layout.
    /// </summary>
    void GenerateFinalRoom()
    {
        finalRoomPosition = ChooseFinalRoomPosition();
        GenerateRoomObject(finalRoomPosition, false); // gerar so o esqueleto
    }

    /// <summary>
    /// Calculates and selects the position for the final room within the level layout.
    /// </summary>
    /// <returns>The chosen position for the final room.</returns>
    Position ChooseFinalRoomPosition()
    {
        Position[] selectedRoom = { initialRoomPosition };
        Position[] withoutInitialPosition = map.Except(selectedRoom).ToArray();

        return withoutInitialPosition.MaxBy(position => Utils.CalculateDistance(position, initialRoomPosition));
    }

    /// <summary>
    /// Generates the remaining rooms within the level layout, excluding the initial and final rooms.
    /// </summary>
    void GenerateRemainingRooms()
    {
        Position[] selectedRooms = {initialRoomPosition, finalRoomPosition};
        foreach (Position roomPosition in map.Except(selectedRooms))
        {
            //Debug.LogWarning("Position No Mapa: " + position.X + " x " + position.Y);
            GenerateRoomObject(roomPosition);
        }
    }

    /// <summary>
    /// Generates a room object at the specified position, with optional content generation.
    /// </summary>
    /// <param name="roomPosition">The position where the room object will be generated.</param>
    /// <param name="generateObjectsInRoom">Determines whether to generate content within the room object (default: true).</param>
    void GenerateRoomObject(Position roomPosition, bool generateObjectsInRoom = true)
    {
        Vector2 roomObjectPosition = Utils.TransformAMapPositionIntoAUnityPosition(roomPosition);
        GameObject roomObject = Instantiate(room, roomObjectPosition, Quaternion.identity, rooms.transform);

        StartCoroutine(GenerateRoom(roomObject, roomPosition, generateObjectsInRoom));
    }

    /// <summary>
    /// Retrieves an array of door positions based on neighboring room directions from the specified room position.
    /// </summary>
    /// <param name="roomPosition">The position of the room for which to find door positions.</param>
    /// <returns>An array of positions representing door locations.</returns>
    Position[] GetDoorPositionsFromRoomPosition(Position roomPosition)
    {
        List<Position> doorsPositions = new();
        foreach (Direction direction in Enum.GetValues(typeof(Direction)).Cast<Direction>())
        {
            Position adjacentPosition = roomPosition.Move(direction);
            if (map.Contains(adjacentPosition))
            {
                doorsPositions.Add(GameConstants.NEIGHBOR_DIRECTION_TO_DOOR_POSITION[direction]);
            }
        }
        return doorsPositions.ToArray();
    }

    /// <summary>
    /// Generates a room within the specified room object and position.
    /// </summary>
    /// <param name="roomObject">The GameObject representing the room object to generate.</param>
    /// <param name="roomPosition">The position of the room to be generated.</param>
    /// <param name="generateObjectsInRoom">Determines whether to generate objects in the room (default: true).</param>
    /// <returns>An IEnumerator for asynchronous room generation.</returns>
    IEnumerator GenerateRoom(GameObject roomObject, Position roomPosition, bool generateObjectsInRoom = true)
    {
        Position[] doorPositions = GetDoorPositionsFromRoomPosition(roomPosition);
        RoomContents[] enemies = Knapsack.ResolveKnapsackEnemies(levelDataManager.Enemies, levelDataManager.EnemiesValues, levelDataManager.EnemiesCapacity);
        RoomContents[] obstacles = Knapsack.ResolveKnapsackObstacles(levelDataManager.Obstacles, levelDataManager.ObstaclesValues, levelDataManager.ObstaclesCapacity);
        int distanceToInitialRoom = Utils.CalculateDistance(initialRoomPosition, roomPosition);
        float difficulty = (float)distanceToInitialRoom / (float)distanceFromInitialToFinalRoom;

        Room room = new(doorPositions, enemies, obstacles, difficulty);
        GeneticRoomGenerator geneticRoomGenerator = new(room);

        if (generateObjectsInRoom)
        {
            yield return StartCoroutine(GenerateRoomsInBackground(room, geneticRoomGenerator));
        }

        if (finalRoomPosition != null && finalRoomPosition.Equals(roomPosition))
        {
            room.Values[GameConstants.ROOM_MIDDLE.X, GameConstants.ROOM_MIDDLE.Y] = RoomContents.Portal;
        }

        roomObjectSpawner.SpawnRoomObjects(room, roomObject);
    }

    /// <summary>
    /// Generates room content asynchronously within a room using a genetic algorithm.
    /// </summary>
    /// <param name="room">The room for which to generate content.</param>
    /// <param name="geneticRoomGenerator">The genetic algorithm generator for the room.</param>
    /// <returns>An IEnumerator for asynchronous room content generation.</returns>
    IEnumerator GenerateRoomsInBackground(Room room, GeneticRoomGenerator geneticRoomGenerator)
    {
        float startTime = Time.realtimeSinceStartup;

        room.Values = geneticRoomGenerator.GeneticLooping();
        yield return null;

        float endTime = Time.realtimeSinceStartup;
        float executionTime = endTime - startTime;
        totalCoroutineExecutionTime += executionTime;
        Debug.LogError("Tempo de execução da corrotina: " + executionTime + " segundos");
        Debug.LogError("Tempo total de execução ate agora: " + totalCoroutineExecutionTime + " segundos");
    }
}
