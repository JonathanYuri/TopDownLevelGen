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

    public HashSet<Position> mapa;

    RoomObjectSpawner roomObjectSpawner;

    private void Awake()
    {
        mapa = new();
    }

    private void Start()
    {
        roomObjectSpawner = GetComponent<RoomObjectSpawner>();
    }

    public HashSet<Position> Generate()
    {
        GenerateMap();
        GenerateEachRoomInMap();
        return mapa;
    }

    void GenerateMap()
    {
        Queue<Position> queue = new();
        queue.Enqueue(new Position { X = 0, Y = 0 });

        while (mapa.Count < GameConstants.NumberOfRooms)
        {
            if (queue.Count == 0)
            {
                // adicionar uma posicao aleatoria que tem no mapa na queue
                int pos = UnityEngine.Random.Range(0, mapa.Count);
                queue.Enqueue(mapa.ElementAt(pos));
            }

            Position position = queue.Dequeue();
            mapa.Add(position);

            // escolher qualquer posição
            Direction[] shuffledArr = Enum.GetValues(typeof(Direction)).Cast<Direction>().Shuffle();

            // destravar as posições
            foreach (Direction direction in shuffledArr)
            {
                Position adjacentPosition = position.Move(direction);

                if (!mapa.Contains(adjacentPosition) && UnityEngine.Random.value < 0.5f)
                {
                    queue.Enqueue(adjacentPosition);
                }
            }
        }
    }

    void GenerateEachRoomInMap()
    {
        foreach (Position position in mapa)
        {
            //Debug.LogWarning("Position No Mapa: " + position.X + " x " + position.Y);
            // TODO: aq ja da pra gerar o esqueleto da sala, as portas e as paredes, so se quiser mais eficiencia
            Vector2 p = Utils.TransformAMapPositionIntoAUnityPosition(position);
            GameObject r = Instantiate(room, p, Quaternion.identity, rooms.transform);

            List<Direction> neighboorsDirection = new();
            foreach (Direction direction in Enum.GetValues(typeof(Direction)).Cast<Direction>())
            {
                Position adjacentPosition = position.Move(direction);
                if (mapa.Contains(adjacentPosition))
                {
                    neighboorsDirection.Add(direction);
                }
            }
            GenerateRoom(r, neighboorsDirection);
        }
    }

    void GenerateRoom(GameObject room, List<Direction> neighboorsDirection)
    {
        // TODO: melhorar a eficiencia do algoritmo
        // TODO: mudar tudo pra privado e oq tiver sendo usado em outra classe usar propriedade pra acessar

        List<Position> doorPositions = new();
        foreach (Direction direction in neighboorsDirection)
        {
            doorPositions.Add(GameConstants.NeighboorDirectionToDoorPosition[direction]);
        }

        Sala sala = new(GameConstants.Height, GameConstants.Width, doorPositions.ToArray(),
            Utils.ResolveKnapsackEnemies(GameConstants.EnemiesCapacity), Utils.ResolveKnapsackObstacles(GameConstants.ObstaclesCapacity));
        GeneticRoomGenerator geneticRoomGenerator = new(sala);

        StartCoroutine(GenerateRoomsInBackground(sala, geneticRoomGenerator, room));

        // TODO: aumento de dificuldade chegando mais perto do boss
        // TODO: gerar a sala inicial e a sala do boss diferente das outras e gerar antes, a sala inicial e a do boss nao tem nd, a do boss tem o boss claro
    }

    IEnumerator GenerateRoomsInBackground(Sala sala, GeneticRoomGenerator geneticRoomGenerator, GameObject room)
    {
        float startTime = Time.realtimeSinceStartup;
        // Inicia a Coroutine para executar o algoritmo em segundo plano
        yield return StartCoroutine(GeneticLoopingCoroutine(sala, geneticRoomGenerator));
        float endTime = Time.realtimeSinceStartup;
        float executionTime = endTime - startTime;
        totalCoroutineExecutionTime += executionTime;
        Debug.LogError("Tempo de execução da corrotina: " + executionTime + " segundos");
        Debug.LogError("Tempo total de execução ate agora: " + totalCoroutineExecutionTime + " segundos");

        // O loop for é executado somente após o retorno da função GeneticLooping()
        roomObjectSpawner.SpawnRoomObjects(sala, room);
    }

    IEnumerator GeneticLoopingCoroutine(Sala sala, GeneticRoomGenerator geneticRoomGenerator)
    {
        sala.Values = geneticRoomGenerator.GeneticLooping();
        yield return null;
    }
}
