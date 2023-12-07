using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generates and manages levels using a room-based approach.
/// </summary>
[RequireComponent(typeof(RoomGenerator))]
public class LevelGenerator : MonoBehaviour
{
    LevelDataManager levelDataManager;
    RoomGenerator roomGenerator;

    [SerializeField] Transform rooms;

    public Position InitialRoomPosition { get; private set; }
    public Position FinalRoomPosition { get; private set; }
    public int DistanceFromInitialToFinalRoom { get; private set; }
    public Transform Rooms { get => rooms; set => rooms = value; }

    void Awake()
    {
        roomGenerator = GetComponent<RoomGenerator>();
    }

    void Start()
    {
        levelDataManager = FindFirstObjectByType<LevelDataManager>();
    }

    /// <summary>
    /// Generates a level, including its map.
    /// </summary>
    /// <returns>A collection of positions representing the generated map.</returns>
    public void Generate()
    {
        DestroyAllPastObjects();
        GenerateMap();
        GenerateInitialAndFinalRoom();
        GenerateRemainingRooms();
    }

    /// <summary>
    /// Destroys all previously generated objects and clears the map.
    /// </summary>
    void DestroyAllPastObjects()
    {
        GameMapManager.Instance.RoomPositions.Clear();
        for (int i = 0; i < Rooms.childCount; i++)
        {
            Destroy(Rooms.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Generates a map of positions for the level layout based on a random and dynamic room generation algorithm.
    /// </summary>
    void GenerateMap()
    {
        Queue<Position> queue = new();
        queue.Enqueue(new Position { X = 0, Y = 0 });

        HashSet<Position> map = new();
        while (map.Count < levelDataManager.RoomCount)
        {
            if (queue.Count == 0)
            {
                int pos = UnityEngine.Random.Range(0, map.Count);
                queue.Enqueue(map.ElementAt(pos));
            }

            Position position = queue.Dequeue();
            map.Add(position);

            Direction[] shuffledDirections = Enum.GetValues(typeof(Direction)).Cast<Direction>().Shuffle();
            foreach (Direction direction in shuffledDirections)
            {
                Position adjacentPosition = position.Move(direction);

                if (!map.Contains(adjacentPosition) && UnityEngine.Random.value < GameConstants.PROBABILITY_OF_GENERATING_ROOM_IN_NEIGHBORHOOD)
                {
                    queue.Enqueue(adjacentPosition);
                }
            }
        }

        GameMapManager.Instance.RoomPositions = map;
    }

    /// <summary>
    /// Generates the final room at a calculated position within the level layout.
    /// Generates the initial room at the starting position of the level layout.
    /// </summary>
    void GenerateInitialAndFinalRoom()
    {
        InitialRoomPosition = new() { X = 0, Y = 0 };
        FinalRoomPosition = ChooseFinalRoomPosition();

        DistanceFromInitialToFinalRoom = Utils.CalculateDistance(InitialRoomPosition, FinalRoomPosition);

        roomGenerator.GenerateRoom(InitialRoomPosition, false); // gerar so o esqueleto
        roomGenerator.GenerateRoom(FinalRoomPosition, false);  // gerar so o esqueleto
    }

    /// <summary>
    /// Calculates and selects the position for the final room within the level layout.
    /// </summary>
    /// <returns>The chosen position for the final room.</returns>
    Position ChooseFinalRoomPosition()
    {
        Position[] selectedRoom = { InitialRoomPosition };
        Position[] withoutInitialPosition = GameMapManager.Instance.RoomPositions.Except(selectedRoom).ToArray();

        return withoutInitialPosition.MaxBy(position => Utils.CalculateDistance(position, InitialRoomPosition));
    }

    /// <summary>
    /// Generates the remaining rooms within the level layout, excluding the initial and final rooms.
    /// </summary>
    void GenerateRemainingRooms()
    {
        Position[] selectedRooms = {InitialRoomPosition, FinalRoomPosition};
        var remainingRooms = GameMapManager.Instance.RoomPositions.Except(selectedRooms);

        foreach (Position roomPosition in remainingRooms)
        {
            //Debug.LogWarning("Position No Mapa: " + position.X + " x " + position.Y);
            roomGenerator.GenerateRoom(roomPosition);
        }
    }
}
