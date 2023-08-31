using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneticRoomGenerator
{
    readonly Sala sala;
    readonly int populationSize;
    List<GeneticRoomIndividual> population;
    readonly float crossoverProbability;
    readonly float mutationProbability;

    public GeneticRoomGenerator(Sala sala, int populationSize, float crossoverProbability, float mutationProbability)
    {
        this.sala = sala;
        this.populationSize = populationSize;
        this.mutationProbability = mutationProbability;
        this.crossoverProbability = crossoverProbability;
        population = new();
    }

    void GeneratePopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GeneticRoomIndividual individual = new(sala);
            population.Add(individual);
        }
    }

    void EvaluatePopulation()
    {
        foreach (GeneticRoomIndividual individual in population)
        {
            individual.Evaluate(sala);
        }
    }

    void MutatePopulation()
    {
        foreach (GeneticRoomIndividual individual in population)
        {
            if (Random.value < mutationProbability)
            {
                individual.Mutate(sala);
            }
        }

        EvaluatePopulation();
    }

    // TODO: mudar o crossover para algo assim, pega um clone da sala que ja tem as portas os nadas e as paredes, e oq vai fazer:
    // pega as posicoes dos inimigos / obstaculos do pai e da mae, e faz o seguinte, quando for colocar um tipo de inimigo escolhe entre a posicao daquele inimigo
    // no pai ou na mae, isso serve pros obstaculos tbm
    // basicamente: lista de posicoes dos tipos de inimigos, quando for colocar aquele inimigo, escolhe a posicao ou do pai ou da mae
    GeneticRoomIndividual Crossover(GeneticRoomIndividual pai, GeneticRoomIndividual mae)
    {
        GeneticRoomIndividual individual = new(sala.Rows, sala.Cols);

        for (int i = 0; i <= sala.Rows / 2; i++)
        {
            for (int j = 0; j < sala.Cols; j++)
            {
                individual.roomMatrix[i, j] = pai.roomMatrix[i, j];
            }
        }

        for (int i = sala.Rows / 2 + 1; i < sala.Rows; i++)
        {
            for (int j = 0; j < sala.Cols; j++)
            {
                individual.roomMatrix[i, j] = mae.roomMatrix[i, j];
            }
        }

        return individual;
    }

    public List<GeneticRoomIndividual> TournamentSelection(int tournamentSize, int numParents)
    {
        List<GeneticRoomIndividual> parents = new();

        for (int i = 0; i < numParents; i++)
        {
            List<GeneticRoomIndividual> tournament = new();

            // Seleciona aleatoriamente "tournamentSize" indivíduos para o torneio
            for (int j = 0; j < tournamentSize; j++)
            {
                int randomIndex = Random.Range(0, population.Count);
                tournament.Add(population[randomIndex]);
            }

            // Ordena os indivíduos do torneio por fitness (do melhor para o pior)
            tournament.Sort((a, b) =>
            {
                if (a.value.HasValue && b.value.HasValue)
                    return b.value.Value.CompareTo(a.value.Value);
                else if (a.value.HasValue)
                    return -1; // Coloca os valores não nulos antes dos valores nulos
                else if (b.value.HasValue)
                    return 1; // Coloca os valores não nulos antes dos valores nulos
                else
                    return 0; // Ambos são nulos, mantém a ordem atual
            });

            // O vencedor do torneio é selecionado para reprodução
            parents.Add(tournament[0]);
        }

        return parents;
    }

    void Reproduction()
    {
        List<GeneticRoomIndividual> newPopulation = new();

        while (newPopulation.Count < populationSize)
        {
            List<GeneticRoomIndividual> parents = TournamentSelection(5, 2);

            GeneticRoomIndividual children1 = Crossover(parents[0], parents[1]);
            GeneticRoomIndividual children2 = Crossover(parents[1], parents[0]);

            newPopulation.Add(children1);
            newPopulation.Add(children2);
        }

        population = newPopulation;
    }

    public Possibilidades[,] GeneticLooping()
    {
        GeneratePopulation();
        EvaluatePopulation();
        int numInteracoesSemMelhora = 0;

        GeneticRoomIndividual melhorIndividuo = new(population.OrderByDescending(individuo => individuo.value).First()); // copia
        int numInteracoes = 0;
        while (numInteracoesSemMelhora < 20 || melhorIndividuo.value == int.MinValue)
        {
            int? melhorValorAtual = population.Max(individuo => individuo.value);
            Debug.Log("NUMERO DE INTERACOES: " + numInteracoes + " MELHOR ATUAL: " + melhorValorAtual + " MELHOR: " + melhorIndividuo.value);

            if (melhorValorAtual > melhorIndividuo.value) // melhorValorAtual > melhorValor
            {
                Debug.Log("TROCOU " + melhorIndividuo.value + " POR " + melhorValorAtual);

                numInteracoesSemMelhora = 0;
                melhorIndividuo = new(population.OrderByDescending(individuo => individuo.value).First()); // copia
            }
            else
            {
                numInteracoesSemMelhora++;
            }

            Reproduction();
            MutatePopulation();
            EvaluatePopulation();

            numInteracoes++;
        }


        int? melhorValorFinal = population.Max(individuo => individuo.value);
        if (melhorValorFinal > melhorIndividuo.value)
        {
            Debug.Log("TROCOU " + melhorIndividuo.value + " POR " + melhorValorFinal);
            melhorIndividuo = new(population.OrderByDescending(individuo => individuo.value).First());
        }
        Debug.Log(melhorIndividuo.value);

        Debug.Log("Inimigo: " + RoomOperations.CountOccurrences(melhorIndividuo.roomMatrix, typeof(Enemies)));
        Debug.Log("Obstaculo: " + RoomOperations.CountOccurrences(melhorIndividuo.roomMatrix, typeof(Obstacles)));
        Debug.Log("qntInimigosProximosDeObstaculos: " + RoomOperations.CountEnemiesNextToObstacles(melhorIndividuo.roomMatrix));

        // retornar o melhor
        return melhorIndividuo.roomMatrix;
    }
}
