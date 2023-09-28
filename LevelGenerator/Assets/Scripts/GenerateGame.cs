using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateGame : MonoBehaviour
{
    [SerializeField] GameObject parede;
    [SerializeField] GameObject porta;
    [SerializeField] GameObject inimigo;
    [SerializeField] GameObject obstaculo;

    [SerializeField] GameObject rooms;
    [SerializeField] GameObject room;

    [SerializeField] GameObject[] portas;
    [SerializeField] GameObject[] paredes;
    [SerializeField] GameObject chao;

    Dictionary<RoomContents, GameObject> objects;

    float totalCoroutineExecutionTime = 0f;

    public HashSet<Position> mapa;

    // TODO: colocar isso numa constante, pq no player location tbm usa na variavel directionToPositionInRoomMatrix
    Dictionary<Direction, Position> neighboorDirectionToDoorPosition;

    private void Awake()
    {
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

        mapa = new();

        neighboorDirectionToDoorPosition = new()
        {
            { Direction.Up, new Position { X = (int)(GameConstants.Width / 2), Y = GameConstants.Height - 1 } },
            { Direction.Down, new Position { X = (int)(GameConstants.Width / 2), Y = 0 } },
            { Direction.Left, new Position { X = 0, Y = (int)(GameConstants.Height / 2) } },
            { Direction.Right, new Position { X = GameConstants.Width - 1, Y = (int)(GameConstants.Height / 2) } }
        };
    }

    RoomContents[] ResolveKnapsackEnemies(int capacityEnemies)
    {
        Dictionary<RoomContents, int> enemiesDifficult = new()
        {
            { RoomContents.Enemy1, 1 },
            { RoomContents.Enemy2, 2 },
            { RoomContents.Enemy3, 3 }
        };

        List<int> valuesEnemies = new(enemiesDifficult.Values);
        List<RoomContents> keysEnemies = new(enemiesDifficult.Keys);

        List<int> chosenEnemiesIdx = Utils.ResolveKnapsack(valuesEnemies, capacityEnemies);

        RoomContents[] chosenEnemies = new RoomContents[chosenEnemiesIdx.Count];
        for (int i = 0; i < chosenEnemiesIdx.Count; i++)
        {
            int idx = chosenEnemiesIdx[i];
            chosenEnemies[i] = keysEnemies[idx];
        }

        //Debug.Log("Inimigos escolhidos: " + string.Join(", ", chosenEnemies));
        return chosenEnemies;
    }

    RoomContents[] ResolveKnapsackObstacles(int capacityObstacles)
    {
        Dictionary<RoomContents, int> obstaclesDifficult = new()
        {
            { RoomContents.Obstacle1, 1 },
            { RoomContents.Obstacle2, 2 },
            { RoomContents.Obstacle3, 3 }
        };

        List<int> valuesObstacles = new(obstaclesDifficult.Values);
        List<RoomContents> keysObstacles = new(obstaclesDifficult.Keys);

        List<int> chosenObstaclesIdx = Utils.ResolveKnapsack(valuesObstacles, capacityObstacles);

        RoomContents[] chosenObstacles = new RoomContents[chosenObstaclesIdx.Count];
        for (int i = 0; i < chosenObstaclesIdx.Count; i++)
        {
            int idx = chosenObstaclesIdx[i];
            chosenObstacles[i] = keysObstacles[idx];
        }

        //Debug.Log("Obstaculos escolhidos: " + string.Join(", ", chosenObstacles));
        return chosenObstacles;
    }

    public HashSet<Position> Generate()
    {
        foreach (Transform child in rooms.transform)
        {
            Destroy(child.gameObject);
        }

        GerarMapa();

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
            GerarSala(r, neighboorsDirection);
        }

        return mapa;
    }

    void GerarMapa()
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

    void GerarSala(GameObject room, List<Direction> neighboorsDirection)
    {
        // ############# COMECO ##############
        // o (0, 0) é o canto superior esquerdo

        // TODO: melhorar a eficiencia do algoritmo
        // TODO: mudar tudo pra privado e oq tiver sendo usado em outra classe usar propriedade pra acessar

        List<Position> doorPositions = new();
        foreach (Direction direction in neighboorsDirection)
        {
            doorPositions.Add(neighboorDirectionToDoorPosition[direction]);
        }

        Sala sala = new(GameConstants.Height, GameConstants.Width, doorPositions.ToArray(), ResolveKnapsackEnemies(30), ResolveKnapsackObstacles(30));
        GeneticRoomGenerator geneticRoomGenerator = new(sala);

        StartCoroutine(GenerateRoomsInBackground(sala, geneticRoomGenerator, room));

        // TODO: aumento de dificuldade chegando mais perto do boss
        // TODO: gerar a sala inicial e a sala do boss diferente das outras e gerar antes, a sala inicial e a do boss nao tem nd, a do boss tem o boss claro
    }

    IEnumerator GenerateRoomsInBackground(Sala sala, GeneticRoomGenerator geneticRoomGenerator, GameObject room)
    {
        //room.transform.localScale = new Vector3(1, 1, 1);
        //room.transform.position = new Vector3(0, 0, 0);

        float startTime = Time.realtimeSinceStartup;
        // Inicia a Coroutine para executar o algoritmo em segundo plano
        yield return StartCoroutine(GeneticLoopingCoroutine(sala, geneticRoomGenerator));
        float endTime = Time.realtimeSinceStartup;
        float executionTime = endTime - startTime;
        totalCoroutineExecutionTime += executionTime;
        Debug.LogError("Tempo de execução da corrotina: " + executionTime + " segundos");
        Debug.LogError("Tempo total de execução ate agora: " + totalCoroutineExecutionTime + " segundos");

        // O loop for é executado somente após o retorno da função GeneticLooping()
        for (int i = 0; i < sala.Width; i++)
        {
            for (int j = 0; j < sala.Height; j++)
            {
                //Debug.LogWarning($"i: {i}, j: {j}: {sala.Values[i, j]}");
                GameObject tile = chao;
                if (objects.ContainsKey(sala.Values[i, j]))
                {
                    tile = objects[sala.Values[i, j]];
                }
                else if (sala.Values[i, j] == RoomContents.Door)
                {
                    tile = SelectTheRightDoor(sala, i, j);
                }
                else if (sala.Values[i, j] == RoomContents.Wall)
                {
                    GameObject corner = SelectTheRightCorner(sala, i, j);
                    tile = corner switch
                    {
                        null => SelectTheRightWall(sala, i, j),
                        _ => corner,
                    };
                }

                Instantiate(tile, (Vector2)tile.transform.position + new Vector2(i, j) + (Vector2)room.transform.position, tile.transform.rotation, room.transform);
            }
        }
    }

    GameObject SelectTheRightDoor(Sala sala, int i, int j)
    {
        if (i == 0) // porta pra esquerda
        {
            return portas[2];
        }
        else if (i == sala.Width - 1) // porta pra direita
        {
            return portas[3];
        }
        else if (j == 0) // porta pra baixo
        {
            return portas[1];
        }
        else if (j == sala.Height - 1) // porta pra cima
        {
            return portas[0];
        }
        return null;
    }

    GameObject SelectTheRightCorner(Sala sala, int i, int j)
    {
        if (i == 0 && j == 0) // quina baixo-esq
        {
            return paredes[2];
        }
        else if (i == 0 && j == sala.Height - 1) // quina cima-esq
        {
            return paredes[0];
        }
        else if (i == sala.Width - 1 && j == 0) // quina baixo-direita
        {
            return paredes[3];
        }
        else if (i == sala.Width - 1 && j == sala.Height - 1) // quina cima-direita
        {
            return paredes[1];
        }
        return null;
    }

    GameObject SelectTheRightWall(Sala sala, int i, int j)
    {
        if (i == 0) // esq
        {
            return paredes[6];
        }
        else if (i == sala.Width - 1) // direita
        {
            return paredes[7];
        }
        else if (j == 0) // baixo
        {
            return paredes[5];
        }
        else if (j == sala.Height - 1) // cima
        {
            return paredes[4];
        }
        return null;
    }

    IEnumerator GeneticLoopingCoroutine(Sala sala, GeneticRoomGenerator geneticRoomGenerator)
    {
        sala.Values = geneticRoomGenerator.GeneticLooping();
        yield return null;
    }
}
