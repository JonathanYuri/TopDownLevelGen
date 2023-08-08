using System;
using UnityEngine;

public class GeneticRoomIndividual
{
    public Possibilidades[,] roomMatrix;
    public int? value;
    readonly System.Random random;

    // usado pra cruzamento
    public GeneticRoomIndividual(int rows, int cols)
    {
        value = null;
        roomMatrix = new Possibilidades[rows, cols];
        random = new();
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
        random = individual.random;
    }

    void GenerateRoomRandomly(Sala sala)
    {
        roomMatrix = sala.matriz;

        // Só as partes mutaveis, gerar aleatorio
        foreach (Tuple<int, int> positions in sala.changeablesPositions)
        {
            int rand = random.Next(sala.changeablesPossibilities.Count);
            roomMatrix[positions.Item1, positions.Item2] = sala.changeablesPossibilities[rand];
        }
    }

    private void MutateAnyPosition(Sala sala)
    {
        // escolher uma posicao que seja mutavel
        int posicaoMutar = random.Next(sala.changeablesPositions.Count);
        Tuple<int, int> posicaoEscolhida = sala.changeablesPositions[posicaoMutar];

        // escolher por qual ele vai ser mutado
        int rand = random.Next(sala.changeablesPossibilities.Count);
        roomMatrix[posicaoEscolhida.Item1, posicaoEscolhida.Item2] = sala.changeablesPossibilities[rand];
    }

    public void Mutate(Sala sala)
    {
        int qntInimigos = Utils.CountOccurrences(roomMatrix, Possibilidades.Inimigo);
        int qntObstaculos = Utils.CountOccurrences(roomMatrix, Possibilidades.Obstaculo);

        // nao preciso ajustar nem os inimigos nem os obstaculos
        if (qntInimigos >= sala.enemiesRange.Start.Value && qntInimigos <= sala.enemiesRange.End.Value
                                                        &&
            qntObstaculos >= sala.obstaclesRange.Start.Value && qntObstaculos <= sala.obstaclesRange.End.Value)
        {
            MutateAnyPosition(sala);
        }
        else
        {
            // tratar os inimigos
            if (qntInimigos < sala.enemiesRange.Start.Value) // vou precisar escolher um lugar pra colocar um inimigo
            {
                Utils.AddToMatrix(sala, roomMatrix, Possibilidades.Inimigo, random);
            }
            else if (qntInimigos > sala.enemiesRange.End.Value) // vou precisar tirar um inimigo
            {
                Utils.RemoveFromMatrix(sala, roomMatrix, Possibilidades.Inimigo, random);
            }

            // tratar os obstaculos
            if (qntObstaculos < sala.obstaclesRange.Start.Value) // vou precisar escolher um lugar pra colocar um obstaculo
            {
                Utils.AddToMatrix(sala, roomMatrix, Possibilidades.Obstaculo, random);
            }
            else if (qntObstaculos > sala.obstaclesRange.End.Value) // vou precisar tirar um obstaculo
            {
                Utils.RemoveFromMatrix(sala, roomMatrix, Possibilidades.Obstaculo, random);
            }
        }
    }

    public void Evaluate(Sala sala)
    {
        // quantidade de caminhos de uma porta a outra, se nao tiver 0 caminhos de uma porta, MONSTRO
        int? qntCaminhos = Utils.CountPathsBetweenDoors(roomMatrix, sala.doorsPositions);
        if (qntCaminhos == null)
        {
            value = int.MinValue;
            return;
        }

        int qntInimigos = Utils.CountOccurrences(roomMatrix, Possibilidades.Inimigo);
        int qntObstaculos = Utils.CountOccurrences(roomMatrix, Possibilidades.Obstaculo);

        double pesoInimigos = 1.0, pesoObstaculos = 1.0, pesoCaminhos = 1.0;

        int distanceToRangeEnemies = Utils.CalculateDistanceToRange(sala.enemiesRange.Start.Value, sala.enemiesRange.End.Value, qntInimigos);
        int distanceToRangeObstacles = Utils.CalculateDistanceToRange(sala.obstaclesRange.Start.Value, sala.obstaclesRange.End.Value, qntObstaculos);

        value = (int)(distanceToRangeEnemies * pesoInimigos + distanceToRangeObstacles * pesoObstaculos - qntCaminhos * pesoCaminhos);
    }
}
