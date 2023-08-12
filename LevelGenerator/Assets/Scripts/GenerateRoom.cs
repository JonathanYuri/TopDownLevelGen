using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] TMP_InputField minObstaclesInput;
    [SerializeField] TMP_InputField maxObstaclesInput;
    [SerializeField] TMP_InputField minEnemiesInput;
    [SerializeField] TMP_InputField maxEnemiesInput;
    
    int minObstacles = 3, maxObstacles = 6;
    int minEnemies = 4, maxEnemies = 7;

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
            { Possibilidades.Obstaculo, obstaculo },
            { Possibilidades.Inimigo, inimigo }
        };
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

        /*
            Mapa mapa = new(5, 6, 3, 5);
            mapa.AdicionarNaLista(0, 0);
            //mapa.GerarSalaRecursivo(0, 0);
            mapa.GerarSalas(30);
            mapa.PrintarSalas();
        */

        // ############# COMECO ##############

        // se eu estou na sala inicial eu preciso que do meio eu consiga ter um caminho pra todas as outras portas
        // já quando eu estou em outras salas eu preciso ter um caminho de uma porta pra todas as outras
        // tem que ter um algoritmo genetico pra gerar as salas centrais do (0,0) e outro pra gerar as outras salas
        // o (0, 0) é o canto superior esquerdo

        int rows = 9;
        int cols = 15;

        List<Tuple<int, int>> doorsPosition = new()
        {
            new Tuple<int, int>((int)(rows / 2), 0),
            new Tuple<int, int>(rows - 1, (int)(cols / 2))
        };

        VerifyInputs();

        if (this.minObstacles >= this.maxObstacles)
        {
            return;
        }
        if (this.minEnemies >= this.maxEnemies)
        {
            return;
        }

        Range enemiesRange = new()
        {
            min = this.minEnemies,
            max = this.maxEnemies
        };
        Range obstaclesRange = new()
        {
            min = this.minObstacles,
            max = this.maxObstacles
        };

        Sala sala = new(rows, cols, doorsPosition, enemiesRange, obstaclesRange);

        int populationSize = 10;
        GeneticRoomGenerator geneticRoomGenerator = new(sala, populationSize, 0.8f, 0.3f);

        StartCoroutine(GenerateRoomsInBackground(sala, geneticRoomGenerator));

        // TODO: diferentes inimigos diferentes pesos, cada inimigo vai ter um peso diferente na dificuldade
        // TODO: diferentes obstaculos diferentes pesos

        // TODO: algoritmo genetico pra gerar o mapa
        // TODO: gerar a sala inicial e a sala final na mao, ja que sao vazias, so preciso colocar as portas, sala do boss so tem uma entrada
        // TODO: aumento de dificuldade chegando mais perto do boss, gerar a sala inicial e a sala do boss diferente das outras e gerar antes
    }

    void VerifyInputs()
    {
        if (minObstaclesInput.text.Length > 0
            && int.TryParse(minObstaclesInput.text, out int minObstacles))
        {
            this.minObstacles = minObstacles;
        }
        if (maxObstaclesInput.text.Length > 0
            && int.TryParse(maxObstaclesInput.text, out int maxObstacles))
        {
            this.maxObstacles = maxObstacles;
        }
        if (minEnemiesInput.text.Length > 0
            && int.TryParse(minEnemiesInput.text, out int minEnemies))
        {
            this.minEnemies = minEnemies;
        }
        if (maxEnemiesInput.text.Length > 0
            && int.TryParse(maxEnemiesInput.text, out int maxEnemies))
        {
            this.maxEnemies = maxEnemies;
        }
    }

    IEnumerator GenerateRoomsInBackground(Sala sala, GeneticRoomGenerator geneticRoomGenerator)
    {
        room.transform.localScale = new Vector3(1, 1, 1);
        room.transform.position = new Vector3(0, 0, 0);

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

        room.transform.localScale = new Vector3(0.8141508f, 0.8141508f, 1);
        room.transform.position = new Vector3(0, -1.32f, 0);
    }

    IEnumerator GeneticLoopingCoroutine(Sala sala, GeneticRoomGenerator geneticRoomGenerator)
    {
        sala.matriz = geneticRoomGenerator.GeneticLooping();
        yield return null;
    }
}
