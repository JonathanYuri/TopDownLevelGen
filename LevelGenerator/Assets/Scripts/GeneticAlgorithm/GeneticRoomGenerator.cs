using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GeneticAlgorithmConstants
{
    public static int IterationsWithoutImprovement = 20;
    public static int PopulationSize = 5;
    public static int TournamentSize = 5;
    public static int NumParentsTournament = 2;
}

public class GeneticRoomGenerator
{
    readonly Sala sala;
    List<GeneticRoomIndividual> population;
    readonly float crossoverProbability;
    readonly float mutationProbability;

    public GeneticRoomGenerator(Sala sala, float crossoverProbability, float mutationProbability)
    {
        this.sala = sala;
        this.mutationProbability = mutationProbability;
        this.crossoverProbability = crossoverProbability;
        population = new();
    }

    void GeneratePopulation()
    {
        for (int i = 0; i < GeneticAlgorithmConstants.PopulationSize; i++)
        {
            GeneticRoomIndividual individual = new(sala);
            population.Add(individual);
        }
    }

    void EvaluatePopulation()
    {
        // avaliar o individuo se ele foi modificado
        foreach (GeneticRoomIndividual individual in population)
        {
            if (individual.itWasModified)
            {
                individual.Evaluate();
                individual.itWasModified = false;
            }
        }
    }

    void MutatePopulation()
    {
        foreach (GeneticRoomIndividual individual in population)
        {
            if (Random.value < mutationProbability)
            {
                individual.Mutate();
                individual.itWasModified = true;
            }
        }
    }

    GeneticRoomIndividual Crossover(GeneticRoomIndividual pai, GeneticRoomIndividual mae)
    {
        GeneticRoomIndividual individual = new(sala, false);

        Dictionary<Possibilidades, List<Position>> posicaoDosInimigosNoPai = RoomOperations.GetPositionsOf(pai.roomMatrix, pai.enemiesPositions);
        Dictionary<Possibilidades, List<Position>> posicaoDosObstaculosNoPai = RoomOperations.GetPositionsOf(pai.roomMatrix, pai.obstaclesPositions);

        Dictionary<Possibilidades, List<Position>> posicaoDosInimigosNaMae = RoomOperations.GetPositionsOf(mae.roomMatrix, mae.enemiesPositions);
        Dictionary<Possibilidades, List<Position>> posicaoDosObstaculosNaMae = RoomOperations.GetPositionsOf(mae.roomMatrix, mae.obstaclesPositions);

        if (posicaoDosInimigosNaMae.Keys.Count != posicaoDosInimigosNoPai.Keys.Count)
        {
            Debug.LogError("pai:");
            for (int i = 0; i < pai.roomMatrix.GetLength(0); i++)
            {
                List<Possibilidades> p = new();
                for (int j = 0; j < pai.roomMatrix.GetLength(1); j++)
                {
                    p.Add(pai.roomMatrix[i, j]);
                }
                Debug.LogError(string.Join(' ', p));
            }

            Debug.LogError("inimigos pai:");
            foreach (var p in posicaoDosInimigosNoPai)
            {
                Debug.LogError("chave: " + p.Key + ": " + string.Join(' ', p.Value));
            }
            Debug.LogError("obstaculos pai:");
            foreach (var p in posicaoDosObstaculosNoPai)
            {
                Debug.LogError("chave: " + p.Key + ": " + string.Join(' ', p.Value));
            }

            Debug.LogError("mae:");
            for (int i = 0; i < mae.roomMatrix.GetLength(0); i++)
            {
                List<Possibilidades> p = new();
                for (int j = 0; j < mae.roomMatrix.GetLength(1); j++)
                {
                    p.Add(mae.roomMatrix[i, j]);
                }
                Debug.LogError(string.Join(' ', p));
            }

            Debug.LogError("inimigos mae:");
            foreach (var p in posicaoDosInimigosNaMae)
            {
                Debug.LogError("chave: " + p.Key + ": " + string.Join(' ', p.Value));
            }
            Debug.LogError("obstaculos mae:");
            foreach (var p in posicaoDosObstaculosNaMae)
            {
                Debug.LogError("chave: " + p.Key + ": " + string.Join(' ', p.Value));
            }

            throw new Exception("Inimigos diferentes no pai e na mae");
        }

        if (posicaoDosObstaculosNaMae.Keys.Count != posicaoDosObstaculosNoPai.Keys.Count)
        {
            throw new Exception("Obstaculos diferentes no pai e na mae");
        }

        // fazer a escolha
        List<Position> possiblePositions = new(sala.changeablesPositions);
        foreach (Possibilidades key in posicaoDosInimigosNoPai.Keys)
        {
            // pra nao ter repeticao de posicao
            HashSet<Position> combinedPositions = new(posicaoDosInimigosNoPai[key]);
            combinedPositions.UnionWith(posicaoDosInimigosNaMae[key]);

            List<Position> chosenPositions = combinedPositions.SelectRandomPositions(posicaoDosInimigosNaMae[key].Count);
            foreach (Position pos in chosenPositions)
            {
                individual.PutEnemyInPosition(key, pos);
                possiblePositions.Remove(pos);
            }
        }

        foreach (Possibilidades key in posicaoDosObstaculosNoPai.Keys)
        {
            // pra nao ter repeticao de posicao
            HashSet<Position> combinedPositions = new(posicaoDosObstaculosNoPai[key]);
            combinedPositions.UnionWith(posicaoDosObstaculosNaMae[key]);

            List<Position> chosenPositions = combinedPositions.SelectRandomPositions(posicaoDosObstaculosNoPai[key].Count);
            int colocados = 0;
            foreach (Position pos in chosenPositions)
            {
                // TODO: fazer assim pra pegar verificar posicoes dos inimigos VVVVVV
                if (!individual.enemiesPositions.Contains(pos))
                {
                    individual.PutObstacleInPosition(key, pos);
                    possiblePositions.Remove(pos);
                    colocados++;
                }
            }

            // colocar o que falta
            int faltando = chosenPositions.Count - colocados;
            List<Position> positions = new HashSet<Position>(possiblePositions).SelectRandomPositions(faltando);
            foreach (Position pos in positions)
            {
                individual.PutObstacleInPosition(key, pos);
            }
        }

        return individual;
    }

    public List<GeneticRoomIndividual> TournamentSelection()
    {
        List<GeneticRoomIndividual> parents = new();

        for (int i = 0; i < GeneticAlgorithmConstants.NumParentsTournament; i++)
        {
            List<GeneticRoomIndividual> tournament = new();

            // Seleciona aleatoriamente "tournamentSize" indivíduos para o torneio
            for (int j = 0; j < GeneticAlgorithmConstants.TournamentSize; j++)
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

        while (newPopulation.Count < GeneticAlgorithmConstants.PopulationSize)
        {
            List<GeneticRoomIndividual> parents = TournamentSelection();

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

        GeneticRoomIndividual melhorIndividuo = new(population.OrderByDescending(individuo => individuo.value).First()); // copia

        int iterationsWithoutImprovement = 0;
        int numInteracoes = 0;
        while (iterationsWithoutImprovement < GeneticAlgorithmConstants.IterationsWithoutImprovement || melhorIndividuo.value == int.MinValue)
        {
            int? melhorValorAtual = population.Max(individuo => individuo.value);
            Debug.Log("NUMERO DE INTERACOES: " + numInteracoes + " MELHOR ATUAL: " + melhorValorAtual + " MELHOR: " + melhorIndividuo.value);

            if (melhorValorAtual > melhorIndividuo.value)
            {
                //Debug.Log("TROCOU " + melhorIndividuo.value + " POR " + melhorValorAtual);
                iterationsWithoutImprovement = 0;
                melhorIndividuo = new(population.OrderByDescending(individuo => individuo.value).First());
            }
            else
            {
                iterationsWithoutImprovement++;
            }

            Reproduction();
            MutatePopulation();
            EvaluatePopulation();

            numInteracoes++;
        }


        int? melhorValorFinal = population.Max(individuo => individuo.value);
        if (melhorValorFinal > melhorIndividuo.value)
        {
            melhorIndividuo = new(population.OrderByDescending(individuo => individuo.value).First());
        }

        Debug.Log("Melhor individuo: " + melhorIndividuo.value);
        Debug.Log("Inimigo: " + melhorIndividuo.enemiesPositions.Count);
        Debug.Log("Obstaculo: " + melhorIndividuo.obstaclesPositions.Count);
        Debug.Log("qntInimigosProximosDeObstaculos: " + RoomOperations.CountEnemiesNextToObstacles(melhorIndividuo.roomMatrix, melhorIndividuo.obstaclesPositions));

        // retornar o melhor
        return melhorIndividuo.roomMatrix;
    }
}
