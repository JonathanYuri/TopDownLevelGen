using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{
    float totalCoroutineExecutionTime = 0f;

    [SerializeField] GameObject rooms;
    [SerializeField] GameObject room;

    HashSet<Position> map;
    Position initialRoomPosition;
    Position finalRoomPosition;
    int distanceFromInitialToFinalRoom;

    RoomObjectSpawner roomObjectSpawner;

    public HashSet<Position> Map { get => map; set => map = value; }

    private void Awake()
    {
        Map = new();
    }

    private void Start()
    {
        roomObjectSpawner = GetComponent<RoomObjectSpawner>();
    }

    public HashSet<Position> Generate()
    {
        GenerateMap();
        GenerateInitialRoom();
        GenerateFinalRoom();
        distanceFromInitialToFinalRoom = Utils.CalculateDistance(initialRoomPosition, finalRoomPosition);
        GenerateRemainingRooms();
        return Map;
    }

    void GenerateMap()
    {
        Queue<Position> queue = new();
        queue.Enqueue(new Position { X = 0, Y = 0 });

        while (Map.Count < GameConstants.NUMBER_OF_ROOMS)
        {
            if (queue.Count == 0)
            {
                // adicionar uma posicao aleatoria que tem no mapa na queue
                int pos = UnityEngine.Random.Range(0, Map.Count);
                queue.Enqueue(Map.ElementAt(pos));
            }

            Position position = queue.Dequeue();
            Map.Add(position);

            // escolher qualquer posição
            Direction[] shuffledArr = Enum.GetValues(typeof(Direction)).Cast<Direction>().Shuffle();

            // destravar as posições
            foreach (Direction direction in shuffledArr)
            {
                Position adjacentPosition = position.Move(direction);

                if (!Map.Contains(adjacentPosition) && UnityEngine.Random.value < GameConstants.PROBABILITY_OF_GENERATING_ROOM_IN_NEIGHBORHOOD)
                {
                    queue.Enqueue(adjacentPosition);
                }
            }
        }
    }

    void GenerateInitialRoom()
    {
        initialRoomPosition = new() { X = 0, Y = 0 };
        GenerateObjectRoom(initialRoomPosition, false); // gerar so o esqueleto
    }

    void GenerateFinalRoom()
    {
        finalRoomPosition = ChooseFinalRoomPosition();
        GenerateObjectRoom(finalRoomPosition, false); // gerar so o esqueleto
    }

    Position ChooseFinalRoomPosition()
    {
        Position[] selectedRoom = { initialRoomPosition };
        Position[] withoutInitialPosition = Map.Except(selectedRoom).ToArray();

        // fazer todas as distancias
        List<int> distancesToInitialPosition = new();
        foreach (Position position in withoutInitialPosition)
        {
            distancesToInitialPosition.Add(Utils.CalculateDistance(position, initialRoomPosition));
        }

        int maxIndex = distancesToInitialPosition.IndexOfMax(distance => distance);
        return withoutInitialPosition[maxIndex];
    }

    void GenerateRemainingRooms()
    {
        Position[] selectedRooms = {initialRoomPosition, finalRoomPosition};
        foreach (Position position in Map.Except(selectedRooms))
        {
            //Debug.LogWarning("Position No Mapa: " + position.X + " x " + position.Y);
            GenerateObjectRoom(position);
        }
    }

    void GenerateObjectRoom(Position position, bool generateObjectsInRoom = true)
    {
        int distanceToInitialRoom = Utils.CalculateDistance(initialRoomPosition, position);
        float difficulty = (float)distanceToInitialRoom / (float)distanceFromInitialToFinalRoom;
        
        Vector2 roomObjectPosition = Utils.TransformAMapPositionIntoAUnityPosition(position);
        GameObject roomObject = Instantiate(room, roomObjectPosition, Quaternion.identity, rooms.transform);

        List<Direction> neighborsDirection = GetDirectionsToNeighboringRooms(position);
        StartCoroutine(GenerateRoom(roomObject, neighborsDirection, difficulty, generateObjectsInRoom));
    }

    List<Direction> GetDirectionsToNeighboringRooms(Position position)
    {
        List<Direction> neighborsDirection = new();
        foreach (Direction direction in Enum.GetValues(typeof(Direction)).Cast<Direction>())
        {
            Position adjacentPosition = position.Move(direction);
            if (Map.Contains(adjacentPosition))
            {
                neighborsDirection.Add(direction);
            }
        }
        return neighborsDirection;
    }

    Position[] GetDoorPositions(List<Direction> neighborsDirection)
    {
        List<Position> doorPositions = new();
        foreach (Direction direction in neighborsDirection)
        {
            doorPositions.Add(GameConstants.NEIGHBOR_DIRECTION_TO_DOOR_POSITION[direction]);
        }
        return doorPositions.ToArray();
    }

    IEnumerator GenerateRoom(GameObject roomObject, List<Direction> neighborsDirection, float difficulty, bool generateObjectsInRoom = true)
    {
        // TODO: melhorar a eficiencia do algoritmo
        Position[] doorPositions = GetDoorPositions(neighborsDirection);

        Room room = new(doorPositions, Knapsack.ResolveKnapsackEnemies(), Knapsack.ResolveKnapsackObstacles(), difficulty);
        GeneticRoomGenerator geneticRoomGenerator = new(room);

        if (generateObjectsInRoom)
        {
            yield return StartCoroutine(GenerateRoomsInBackground(room, geneticRoomGenerator));
        }

        roomObjectSpawner.SpawnRoomObjects(room, roomObject);
    }

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
