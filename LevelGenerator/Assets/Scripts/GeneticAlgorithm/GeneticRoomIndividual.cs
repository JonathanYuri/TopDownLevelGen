using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneticRoomIndividual
{
    public Possibilidades[,] roomMatrix;
    public int? value;

    // usado pra cruzamento
    public GeneticRoomIndividual(int rows, int cols)
    {
        value = null;
        roomMatrix = new Possibilidades[rows, cols];
    }

    // usado pra criar aleatoriamente a sala, quando gera a populacao
    public GeneticRoomIndividual(Sala sala) : this(sala.Rows, sala.Cols) // chama o construtor
    {
        // gerar aleatoriamente a sala
        GenerateRoomRandomly(sala);
    }

    public GeneticRoomIndividual(GeneticRoomIndividual individual)
    {
        value = individual.value;
        roomMatrix = individual.roomMatrix;
    }

    void GenerateRoomRandomly(Sala sala)
    {
        roomMatrix = (Possibilidades[,])sala.matriz.Clone();

        // Só as partes mutaveis, gerar aleatorio
        foreach (Tuple<int, int> positions in sala.changeablesPositions)
        {
            int rand = Random.Range(0, sala.changeablesPossibilities.Count);
            roomMatrix[positions.Item1, positions.Item2] = sala.changeablesPossibilities[rand];
        }
    }

    private void MutateAnyPosition(Sala sala)
    {
        // escolher uma posicao que seja mutavel
        int posicaoMutar = Random.Range(0, sala.changeablesPositions.Count);
        Tuple<int, int> posicaoEscolhida = sala.changeablesPositions[posicaoMutar];

        // escolher por qual ele vai ser mutado
        int rand = Random.Range(0, sala.changeablesPossibilities.Count);
        roomMatrix[posicaoEscolhida.Item1, posicaoEscolhida.Item2] = sala.changeablesPossibilities[rand];
    }

    public void Mutate(Sala sala)
    {
        int qntInimigos = Utils.CountOccurrences(roomMatrix, Possibilidades.Inimigo);
        int qntObstaculos = Utils.CountOccurrences(roomMatrix, Possibilidades.Obstaculo);

        // nao preciso ajustar nem os inimigos nem os obstaculos
        if (qntInimigos >= sala.enemiesRange.min && qntInimigos <= sala.enemiesRange.max
                                                        &&
            qntObstaculos >= sala.obstaclesRange.min && qntObstaculos <= sala.obstaclesRange.max)
        {
            MutateAnyPosition(sala);
        }
        else
        {
            // tratar os inimigos
            if (qntInimigos < sala.enemiesRange.min) // vou precisar escolher um lugar pra colocar um inimigo
            {
                Utils.AddToMatrix(sala, roomMatrix, Possibilidades.Inimigo);
            }
            else if (qntInimigos > sala.enemiesRange.max) // vou precisar tirar um inimigo
            {
                Utils.RemoveFromMatrix(sala, roomMatrix, Possibilidades.Inimigo);
            }

            // tratar os obstaculos
            if (qntObstaculos < sala.obstaclesRange.min) // vou precisar escolher um lugar pra colocar um obstaculo
            {
                Utils.AddToMatrix(sala, roomMatrix, Possibilidades.Obstaculo);
            }
            else if (qntObstaculos > sala.obstaclesRange.max) // vou precisar tirar um obstaculo
            {
                Utils.RemoveFromMatrix(sala, roomMatrix, Possibilidades.Obstaculo);
            }
        }
    }

    public void Evaluate(Sala sala)
    {
        // quantidade de caminhos de uma porta a outra, se nao tiver 0 caminhos de uma porta, MONSTRO
        int qntCaminhos = Utils.CountPathsBetweenDoors(roomMatrix, sala.doorsPositions);
        if (qntCaminhos == int.MinValue)
        {
            value = int.MinValue;
            return;
        }

        int qntInimigos = Utils.CountOccurrences(roomMatrix, Possibilidades.Inimigo);
        int qntObstaculos = Utils.CountOccurrences(roomMatrix, Possibilidades.Obstaculo);

        int distanceToRangeEnemies = Utils.CalculateDistanceToRange(sala.enemiesRange.min, sala.enemiesRange.max, qntInimigos);
        int distanceToRangeObstacles = Utils.CalculateDistanceToRange(sala.obstaclesRange.min, sala.obstaclesRange.max, qntObstaculos);

        double pesoInimigos = 1.0, pesoObstaculos = 1.0, pesoCaminhos = 1.0;

        value = (int)(distanceToRangeEnemies * pesoInimigos + distanceToRangeObstacles * pesoObstaculos - qntCaminhos * pesoCaminhos);
    }
}
