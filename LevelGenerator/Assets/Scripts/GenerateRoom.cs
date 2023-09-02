using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateRoom : MonoBehaviour
{
    [SerializeField] GameObject parede;
    [SerializeField] GameObject porta;
    [SerializeField] GameObject inimigo;
    [SerializeField] GameObject obstaculo;
    [SerializeField] GameObject chao;

    [SerializeField] GameObject rooms;
    [SerializeField] GameObject room;

    bool isGenerating = false;

    [SerializeField] GameObject[] portas;
    [SerializeField] GameObject[] paredes;
    [SerializeField] GameObject chaoFinal;
    [SerializeField] GameObject player;

    Dictionary<Possibilidades, GameObject> objects;

    float totalCoroutineExecutionTime = 0f;

    // constantes
    const int numSalas = 10;
    const int rows = 9;
    const int cols = 15;

    HashSet<Position> mapa;

    private void Awake()
    {
        objects = new()
        {
            { Possibilidades.Chao, chao },
            { Possibilidades.Nada, chao },
            { Possibilidades.Parede, parede },
            { Possibilidades.Porta, porta },

            { Possibilidades.Obstaculo1, obstaculo },
            { Possibilidades.Obstaculo2, obstaculo },
            { Possibilidades.Obstaculo3, obstaculo },

            { Possibilidades.Inimigo1, inimigo },
            { Possibilidades.Inimigo2, inimigo },
            { Possibilidades.Inimigo3, inimigo },
        };

        mapa = new();
    }

    private void Start()
    {
        Generate();
    }

    Enemies[] ResolveKnapsackEnemies(int capacityEnemies)
    {
        Dictionary<Enemies, int> enemiesDifficult = new()
        {
            { Enemies.Inimigo1, 1 },
            { Enemies.Inimigo2, 2 },
            { Enemies.Inimigo3, 3 }
        };

        List<int> valuesEnemies = new(enemiesDifficult.Values);
        List<Enemies> keysEnemies = new(enemiesDifficult.Keys);

        List<int> chosenEnemiesIdx = Utils.ResolveKnapsack(valuesEnemies, capacityEnemies);

        Enemies[] chosenEnemies = new Enemies[chosenEnemiesIdx.Count];
        for (int i = 0; i < chosenEnemiesIdx.Count; i++)
        {
            int idx = chosenEnemiesIdx[i];
            chosenEnemies[i] = keysEnemies[idx];
        }

        //Debug.Log("Inimigos escolhidos: " + string.Join(", ", chosenEnemies));
        return chosenEnemies;
    }

    Obstacles[] ResolveKnapsackObstacles(int capacityObstacles)
    {
        Dictionary<Obstacles, int> obstaclesDifficult = new()
        {
            { Obstacles.Obstaculo1, 1 },
            { Obstacles.Obstaculo2, 2 },
            { Obstacles.Obstaculo3, 3 }
        };

        List<int> valuesObstacles = new(obstaclesDifficult.Values);
        List<Obstacles> keysObstacles = new(obstaclesDifficult.Keys);

        List<int> chosenObstaclesIdx = Utils.ResolveKnapsack(valuesObstacles, capacityObstacles);

        Obstacles[] chosenObstacles = new Obstacles[chosenObstaclesIdx.Count];
        for (int i = 0; i < chosenObstaclesIdx.Count; i++)
        {
            int idx = chosenObstaclesIdx[i];
            chosenObstacles[i] = keysObstacles[idx];
        }

        //Debug.Log("Obstaculos escolhidos: " + string.Join(", ", chosenObstacles));
        return chosenObstacles;
    }

    public void Generate()
    {
        if (isGenerating)
        {
            return;
        }

        foreach (Transform child in rooms.transform)
        {
            Destroy(child.gameObject);
        }

        float time = Time.time;
        GerarMapa();

        Debug.Log("mapa:");
        foreach (Position position in mapa)
        {
            Debug.Log(position.Row + ", " + position.Column + " iguais: " + mapa.Count(p => p.Equals(position)));

            Vector2 p = new(position.Row * cols + position.Row, position.Column * rows + position.Column);
            GameObject r = Instantiate(room, p, Quaternion.identity, rooms.transform);

            GerarSala(r);
        }
    }

    void GerarMapa()
    {
        Queue<Position> queue = new();
        queue.Enqueue(new Position { Row = 0, Column = 0 });

        while (mapa.Count < numSalas)
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

        // Spawnar Player
        Position initial = mapa.ElementAt(0);
        Instantiate(player, (Vector2)player.transform.position + new Vector2((int)(cols / 2), -(rows - 2)) + new Vector2(initial.Row, initial.Column), Quaternion.identity);

        isGenerating = false;
    }

    void GerarSala(GameObject room)
    {
        // ############# COMECO ##############
        // o (0, 0) é o canto superior esquerdo

        // TODO: modificar as portas pra fazer sentido com o mapa
        // TODO: melhorar a eficiencia do algoritmo
        /* TODO: mudar tudo pra privado e oq tiver sendo usado em outra classe usar propriedade pra acessar
         * 
         * 
         */

        Position[] doorsPosition = new Position[2];
        doorsPosition[0] = new Position { Row = (int)(rows / 2), Column = 0 };
        doorsPosition[1] = new Position { Row = rows - 1, Column = (int)(cols / 2) };

        Sala sala = new(rows, cols, doorsPosition, ResolveKnapsackEnemies(30), ResolveKnapsackObstacles(30));
        GeneticRoomGenerator geneticRoomGenerator = new(sala, 0.8f, 0.3f);

        StartCoroutine(GenerateRoomsInBackground(sala, geneticRoomGenerator, room));

        // TODO: aumento de dificuldade chegando mais perto do boss
        // TODO: gerar a sala inicial e a sala do boss diferente das outras e gerar antes, a sala inicial e a do boss nao tem nd, a do boss tem o boss claro
        // TODO? dificuldade depende da quantidade de caminhos entre uma porta e outra sem inimigos, quanto mais caminhos mais facil
    }

    IEnumerator GenerateRoomsInBackground(Sala sala, GeneticRoomGenerator geneticRoomGenerator, GameObject room)
    {
        //room.transform.localScale = new Vector3(1, 1, 1);
        //room.transform.position = new Vector3(0, 0, 0);

        isGenerating = true;

        // esperar ate o proximo frame pra ele atualizar o texto botao
        yield return null;

        float startTime = Time.realtimeSinceStartup;
        // Inicia a Coroutine para executar o algoritmo em segundo plano
        yield return StartCoroutine(GeneticLoopingCoroutine(sala, geneticRoomGenerator));
        float endTime = Time.realtimeSinceStartup;
        float executionTime = endTime - startTime;
        totalCoroutineExecutionTime += executionTime;
        Debug.LogError("Tempo de execução da corrotina: " + executionTime + " segundos");
        Debug.LogError("Tempo total de execução ate agora: " + totalCoroutineExecutionTime + " segundos");

        // O loop for é executado somente após o retorno da função GeneticLooping()
        for (int i = 0; i < sala.Rows; i++)
        {
            for (int j = 0; j < sala.Cols; j++)
            {
                GameObject tile = objects[sala.matriz[i, j]];

                if (sala.matriz[i, j] == Possibilidades.Porta)
                {
                    if (i == 0) // porta pra cima
                    {
                        tile = portas[0];
                    }
                    else if (i == sala.Rows - 1) // porta pra baixo
                    {
                        tile = portas[1];
                    }
                    else if (j == 0) // porta pra esquerda
                    {
                        tile = portas[2];
                    }
                    else if (j == sala.Cols - 1) // porta pra direita
                    {
                        tile = portas[3];
                    }
                }

                else if (sala.matriz[i, j] == Possibilidades.Parede)
                {
                    if (i == 0 && j == 0) // quina cima-esq
                    {
                        tile = paredes[0];
                    }
                    else if (i == 0 && j == sala.Cols - 1) // quina cima-direita
                    {
                        tile = paredes[1];
                    }
                    else if (i == sala.Rows - 1 && j == 0) // quina baixo-esq
                    {
                        tile = paredes[2];
                    }
                    else if (i == sala.Rows - 1 && j == sala.Cols - 1) // quina baixo-direita
                    {
                        tile = paredes[3];
                    }

                    // RESTO
                    else if (i == 0) // cima
                    {
                        tile = paredes[4];
                    }
                    else if (i == sala.Rows - 1) // baixo
                    {
                        tile = paredes[5];
                    }
                    else if (j == 0) // esq
                    {
                        tile = paredes[6];
                    }
                    else if (j == sala.Cols - 1) // direita
                    {
                        tile = paredes[7];
                    }
                }

                else if (sala.matriz[i, j] == Possibilidades.Chao || sala.matriz[i, j] == Possibilidades.Nada)
                {
                    tile = chaoFinal;
                }

                Instantiate(tile, (Vector2)tile.transform.position + new Vector2(j, -i) + (Vector2)room.transform.position, tile.transform.rotation, room.transform);
                //Debug.Log(sala.matriz[i, j].ToString()[..2] + " ");
            }
        }

        //room.transform.localScale = new Vector3(0.8141508f, 0.8141508f, 1);
        //room.transform.position = new Vector3(0, -1.32f, 0);
    }

    IEnumerator GeneticLoopingCoroutine(Sala sala, GeneticRoomGenerator geneticRoomGenerator)
    {
        sala.matriz = geneticRoomGenerator.GeneticLooping();
        yield return null;
    }
}
