using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public struct Range
{
    public int min;
    public int max;
}

public class GenerateRoom : MonoBehaviour
{
    [SerializeField] GameObject parede;
    [SerializeField] GameObject porta;
    [SerializeField] GameObject inimigo;
    [SerializeField] GameObject obstaculo;
    [SerializeField] GameObject chao;

    [SerializeField] GameObject room;

    [SerializeField] TMP_Text botaoGerar;
    string botaoGerarInicial;
    bool isGenerating = false;

    Dictionary<Possibilidades, GameObject> objects;

    private void Awake()
    {
        botaoGerarInicial = botaoGerar.text;
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
    }

    List<Enemies> ResolveKnapsackEnemies(int capacityEnemies)
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

        List<Enemies> chosenEnemies = new();
        chosenEnemiesIdx.ForEach(idx => chosenEnemies.Add(keysEnemies[idx]));

        Debug.Log("Inimigos escolhidos: " + string.Join(", ", chosenEnemies));
        return chosenEnemies;
    }

    List<Obstacles> ResolveKnapsackObstacles(int capacityObstacles)
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

        List<Obstacles> chosenObstacles = new();
        chosenObstaclesIdx.ForEach(idx => chosenObstacles.Add(keysObstacles[idx]));

        Debug.Log("Obstaculos escolhidos: " + string.Join(", ", chosenObstacles));
        return chosenObstacles;
    }

    public void Generate()
    {
        if (isGenerating)
        {
            return;
        }

        foreach (Transform child in room.transform)
        {
            Destroy(child.gameObject);
        }

        // ############# COMECO ##############
        // o (0, 0) é o canto superior esquerdo

        int rows = 9;
        int cols = 15;

        List<Position> doorsPosition = new()
        {
            new Position{ Row = (int)(rows / 2), Column = 0 },
            new Position{ Row = rows - 1, Column =(int)(cols / 2) }
        };

        Sala sala = new(rows, cols, doorsPosition, ResolveKnapsackEnemies(30), ResolveKnapsackObstacles(30));

        int populationSize = 10;
        GeneticRoomGenerator geneticRoomGenerator = new(sala, populationSize, 0.8f, 0.3f);

        StartCoroutine(GenerateRoomsInBackground(sala, geneticRoomGenerator));

        // TODO: gerar mapa com aquela logica simples de colocar numa pilha
        // TODO: aumento de dificuldade chegando mais perto do boss
        // TODO: gerar a sala inicial e a sala do boss diferente das outras e gerar antes, a sala inicial e a do boss nao tem nd, a do boss tem o boss claro
    }

    IEnumerator GenerateRoomsInBackground(Sala sala, GeneticRoomGenerator geneticRoomGenerator)
    {
        //room.transform.localScale = new Vector3(1, 1, 1);
        //room.transform.position = new Vector3(0, 0, 0);

        isGenerating = true;
        botaoGerar.text = "Gerando...";

        // esperar ate o proximo frame pra ele atualizar o texto botao
        yield return null;

        // Inicia a Coroutine para executar o algoritmo em segundo plano
        yield return StartCoroutine(GeneticLoopingCoroutine(sala, geneticRoomGenerator));

        // O loop for é executado somente após o retorno da função GeneticLooping()
        for (int i = 0; i < sala.Rows; i++)
        {
            for (int j = 0; j < sala.Cols; j++)
            {
                GameObject tile = objects[sala.matriz[i, j]];
                Instantiate(tile, (Vector2)tile.transform.position + new Vector2(j, -i), Quaternion.identity, room.transform);
                //Debug.Log(sala.matriz[i, j].ToString()[..2] + " ");
            }
        }

        botaoGerar.text = botaoGerarInicial;
        isGenerating = false;

        //room.transform.localScale = new Vector3(0.8141508f, 0.8141508f, 1);
        //room.transform.position = new Vector3(0, -1.32f, 0);
    }

    IEnumerator GeneticLoopingCoroutine(Sala sala, GeneticRoomGenerator geneticRoomGenerator)
    {
        sala.matriz = geneticRoomGenerator.GeneticLooping();
        yield return null;
    }
}
