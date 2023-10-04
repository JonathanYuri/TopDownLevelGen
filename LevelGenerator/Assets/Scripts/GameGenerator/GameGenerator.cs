using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameGenerator : MonoBehaviour
{
    float totalCoroutineExecutionTime = 0f;

    [SerializeField] GameObject rooms;
    [SerializeField] GameObject room;

    HashSet<Position> map;

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
        GenerateEachRoomInMap();
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

    void GenerateEachRoomInMap()
    {
        foreach (Position position in Map)
        {
            //Debug.LogWarning("Position No Mapa: " + position.X + " x " + position.Y);
            // TODO: aq ja da pra gerar o esqueleto da room, as portas e as paredes, so se quiser mais eficiencia
            Vector2 p = Utils.TransformAMapPositionIntoAUnityPosition(position);
            GameObject r = Instantiate(room, p, Quaternion.identity, rooms.transform);

            List<Direction> neighborsDirection = GetDirectionsToNeighboringRooms(position);
            GenerateRoom(r, neighborsDirection);
        }
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

    void GenerateRoom(GameObject roomObject, List<Direction> neighborsDirection)
    {
        // TODO: melhorar a eficiencia do algoritmo
        // TODO: mudar tudo pra privado e oq tiver sendo usado em outra classe usar propriedade pra acessar

        Position[] doorPositions = GetDoorPositions(neighborsDirection);

        Room room = new(doorPositions, Knapsack.ResolveKnapsackEnemies(), Knapsack.ResolveKnapsackObstacles());
        GeneticRoomGenerator geneticRoomGenerator = new(room);

        StartCoroutine(GenerateRoomsInBackground(room, geneticRoomGenerator, roomObject));

        // TODO: aumento de dificuldade chegando mais perto do boss
        // TODO: gerar a room inicial e a room do boss diferente das outras e gerar antes, a room inicial e a do boss nao tem nd, a do boss tem o boss claro
    }

    IEnumerator GenerateRoomsInBackground(Room room, GeneticRoomGenerator geneticRoomGenerator, GameObject roomObject)
    {
        float startTime = Time.realtimeSinceStartup;
        // Inicia a Coroutine para executar o algoritmo em segundo plano
        yield return StartCoroutine(GeneticLoopingCoroutine(room, geneticRoomGenerator));
        float endTime = Time.realtimeSinceStartup;
        float executionTime = endTime - startTime;
        totalCoroutineExecutionTime += executionTime;
        Debug.LogError("Tempo de execução da corrotina: " + executionTime + " segundos");
        Debug.LogError("Tempo total de execução ate agora: " + totalCoroutineExecutionTime + " segundos");

        // O loop for é executado somente após o retorno da função GeneticLooping()
        roomObjectSpawner.SpawnRoomObjects(room, roomObject);
    }

    IEnumerator GeneticLoopingCoroutine(Room room, GeneticRoomGenerator geneticRoomGenerator)
    {
        room.Values = geneticRoomGenerator.GeneticLooping();
        yield return null;
    }
}
