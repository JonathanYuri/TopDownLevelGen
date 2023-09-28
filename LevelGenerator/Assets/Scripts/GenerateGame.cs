using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateGame : MonoBehaviour
{
    #region Object Settings

    [SerializeField] GameObject parede;
    [SerializeField] GameObject porta;
    [SerializeField] GameObject inimigo;
    [SerializeField] GameObject obstaculo;

    [SerializeField] GameObject rooms;
    [SerializeField] GameObject room;

    [SerializeField] GameObject[] portas;
    [SerializeField] GameObject[] paredes;

    [SerializeField] GameObject chao;

    #endregion

    #region Object Indexes

    enum CornerIndex
    {
        TopLeftCorner = 0,
        TopRightCorner = 1,
        BottomLeftCorner = 2,
        BottomRightCorner = 3,
    }

    enum WallIndex
    {
        Left = 6,
        Right = 7,
        Bottom = 5,
        Top = 4
    }

    enum DoorIndex
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
    }

    #endregion

    #region Object Dictionaries

    Dictionary<RoomContents, GameObject> objects;

    Dictionary<Direction, Position> neighboorDirectionToDoorPosition;

    Dictionary<Direction, GameObject> directionOfDoorsToGameObject;

    Dictionary<Position, GameObject> cornerPositionToGameObject;

    #endregion

    float totalCoroutineExecutionTime = 0f;

    public HashSet<Position> mapa;

    private void Awake()
    {
        mapa = new();

        #region Object Dictionaries Initialization

        objects = new()
        {
            { RoomContents.Ground, chao },
            { RoomContents.Nothing, chao },

            { RoomContents.Obstacle1, obstaculo },
            { RoomContents.Obstacle2, obstaculo },
            { RoomContents.Obstacle3, obstaculo },

            { RoomContents.Enemy1, inimigo },
            { RoomContents.Enemy2, inimigo },
            { RoomContents.Enemy3, inimigo },
        };

        neighboorDirectionToDoorPosition = new()
        {
            { Direction.Up, new Position { X = GameConstants.RoomMiddle.X, Y = GameConstants.Height - 1 } },
            { Direction.Down, new Position { X = GameConstants.RoomMiddle.X, Y = 0 } },
            { Direction.Left, new Position { X = 0, Y = GameConstants.RoomMiddle.Y } },
            { Direction.Right, new Position { X = GameConstants.Width - 1, Y = GameConstants.RoomMiddle.Y } }
        };

        directionOfDoorsToGameObject = new()
        {
            { Direction.Up, portas[(int)DoorIndex.Up] },
            { Direction.Down, portas[(int)DoorIndex.Down] },
            { Direction.Left, portas[(int)DoorIndex.Left] },
            { Direction.Right, portas[(int)DoorIndex.Right] },
        };

        cornerPositionToGameObject = new()
        {
            { new Position { X = 0, Y = GameConstants.Height - 1 }, paredes[(int)CornerIndex.TopLeftCorner] },
            { new Position { X = GameConstants.Width - 1, Y = GameConstants.Height - 1 }, paredes[(int)CornerIndex.TopRightCorner] },
            { new Position { X = 0, Y = 0 }, paredes[(int)CornerIndex.BottomLeftCorner] },
            { new Position { X = GameConstants.Width - 1, Y = 0 }, paredes[(int)CornerIndex.BottomRightCorner] },
        };

        #endregion
    }

    #region Generation
    public HashSet<Position> Generate()
    {
        foreach (Transform child in rooms.transform)
        {
            Destroy(child.gameObject);
        }

        GenerateMap();

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

            // escolher qualquer posi��o
            Direction[] shuffledArr = Enum.GetValues(typeof(Direction)).Cast<Direction>().Shuffle();

            // destravar as posi��es
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

    void GenerateRoom(GameObject room, List<Direction> neighboorsDirection)
    {
        // TODO: melhorar a eficiencia do algoritmo
        // TODO: mudar tudo pra privado e oq tiver sendo usado em outra classe usar propriedade pra acessar

        List<Position> doorPositions = new();
        foreach (Direction direction in neighboorsDirection)
        {
            doorPositions.Add(neighboorDirectionToDoorPosition[direction]);
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
        Debug.LogError("Tempo de execu��o da corrotina: " + executionTime + " segundos");
        Debug.LogError("Tempo total de execu��o ate agora: " + totalCoroutineExecutionTime + " segundos");

        // O loop for � executado somente ap�s o retorno da fun��o GeneticLooping()
        for (int i = 0; i < sala.Width; i++)
        {
            for (int j = 0; j < sala.Height; j++)
            {
                //Debug.LogWarning($"i: {i}, j: {j}: {sala.Values[i, j]}");
                Position position = new() { X = i, Y = j };
                GameObject tile = SelectTheRightObjectsToSpawnInPosition(sala, position);

                Instantiate(tile, (Vector2)tile.transform.position + new Vector2(i, j) + (Vector2)room.transform.position, tile.transform.rotation, room.transform);
            }
        }
    }
    #endregion

    #region ObjectSelection
    GameObject SelectTheRightObjectsToSpawnInPosition(Sala sala, Position position)
    {
        GameObject tile = chao;
        if (objects.TryGetValue(sala.Values[position.X, position.Y], out GameObject gameObject))
        {
            tile = gameObject;
        }
        else if (sala.Values[position.X, position.Y] == RoomContents.Door)
        {
            tile = SelectTheRightPositionDoor(position);
        }
        else if (sala.Values[position.X, position.Y] == RoomContents.Wall)
        {
            GameObject corner = SelectTheRightPositionCorner(position);
            tile = corner switch
            {
                null => SelectTheRightPositionWall(position),
                _ => corner,
            };
        }
        return tile;
    }

    GameObject SelectTheRightPositionDoor(Position position)
    {
        // pegar todas as posicoes das portas e ver se eh igual a que estou agora
        foreach (var kvp in neighboorDirectionToDoorPosition)
        {
            if (kvp.Value.Equals(position))
            {
                return directionOfDoorsToGameObject[kvp.Key];
            }
        }
        return null;
    }

    GameObject SelectTheRightPositionCorner(Position position)
    {
        if (cornerPositionToGameObject.TryGetValue(position, out GameObject corner))
        {
            return corner;
        }
        return null;
    }

    GameObject SelectTheRightPositionWall(Position position)
    {
        if (position.X == 0)
        {
            return paredes[(int)WallIndex.Left];
        }
        else if (position.X == GameConstants.Width - 1)
        {
            return paredes[(int)WallIndex.Right];
        }
        else if (position.Y == 0)
        {
            return paredes[(int)WallIndex.Bottom];
        }
        else if (position.Y == GameConstants.Height - 1)
        {
            return paredes[(int)WallIndex.Top];
        }
        return null;
    }
    #endregion

    IEnumerator GeneticLoopingCoroutine(Sala sala, GeneticRoomGenerator geneticRoomGenerator)
    {
        sala.Values = geneticRoomGenerator.GeneticLooping();
        yield return null;
    }
}
