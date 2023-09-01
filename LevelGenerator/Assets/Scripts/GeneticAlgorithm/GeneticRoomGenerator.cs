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
        foreach (GeneticRoomIndividual individual in population.Where(individual => individual.itWasModified))
        {
            individual.Evaluate(sala);
            individual.itWasModified = false;
        }
    }

    void MutatePopulation()
    {
        foreach (GeneticRoomIndividual individual in population)
        {
            if (Random.value < mutationProbability)
            {
                individual.Mutate(sala);
                individual.itWasModified = true;
            }
        }
    }

    GeneticRoomIndividual Crossover(GeneticRoomIndividual pai, GeneticRoomIndividual mae)
    {
        GeneticRoomIndividual individual = new(sala, false);

        Dictionary<Possibilidades, List<Position>> posicaoDosInimigosNoPai = RoomOperations.GetPositionsByEnumType(pai.roomMatrix, typeof(Enemies));
        Dictionary<Possibilidades, List<Position>> posicaoDosObstaculosNoPai = RoomOperations.GetPositionsByEnumType(pai.roomMatrix, typeof(Obstacles));

        Dictionary<Possibilidades, List<Position>> posicaoDosInimigosNaMae = RoomOperations.GetPositionsByEnumType(mae.roomMatrix, typeof(Enemies));
        Dictionary<Possibilidades, List<Position>> posicaoDosObstaculosNaMae = RoomOperations.GetPositionsByEnumType(mae.roomMatrix, typeof(Obstacles));

        if (posicaoDosInimigosNaMae.Keys.Count != posicaoDosInimigosNoPai.Keys.Count)
        {
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

            List<Position> chosenPositions = Utils.SelectRandomPositions(combinedPositions, posicaoDosInimigosNaMae[key].Count);
            foreach (Position pos in chosenPositions)
            {
                individual.roomMatrix[pos.Row, pos.Column] = key;
                possiblePositions.Remove(pos);
            }
        }

        foreach (Possibilidades key in posicaoDosObstaculosNoPai.Keys)
        {
            HashSet<string> enemies = Utils.GetEnumValueStrings(typeof(Enemies));

            // pra nao ter repeticao de posicao
            HashSet<Position> combinedPositions = new(posicaoDosObstaculosNoPai[key]);
            combinedPositions.UnionWith(posicaoDosObstaculosNaMae[key]);

            List<Position> chosenPositions = Utils.SelectRandomPositions(combinedPositions, posicaoDosObstaculosNoPai[key].Count);
            int colocados = 0;
            foreach (Position pos in chosenPositions)
            {
                if (!enemies.Contains(individual.roomMatrix[pos.Row, pos.Column].ToString()))
                {
                    individual.roomMatrix[pos.Row, pos.Column] = key;
                    possiblePositions.Remove(pos);
                    colocados++;
                }
            }

            // colocar o que falta
            int faltando = chosenPositions.Count - colocados;
            List<Position> positions = Utils.SelectRandomPositions(new HashSet<Position>(possiblePositions), faltando);
            foreach (Position pos in positions)
            {
                individual.roomMatrix[pos.Row, pos.Column] = key;
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
        Debug.Log("Inimigo: " + RoomOperations.CountOccurrences(melhorIndividuo.roomMatrix, typeof(Enemies)));
        Debug.Log("Obstaculo: " + RoomOperations.CountOccurrences(melhorIndividuo.roomMatrix, typeof(Obstacles)));
        Debug.Log("qntInimigosProximosDeObstaculos: " + RoomOperations.CountEnemiesNextToObstacles(melhorIndividuo.roomMatrix));

        // retornar o melhor
        return melhorIndividuo.roomMatrix;
    }
}
