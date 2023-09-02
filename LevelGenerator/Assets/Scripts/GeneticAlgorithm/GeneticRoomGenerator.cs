using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GeneticAlgorithmConstants
{
    public static int IterationsWithoutImprovement = 20;
    public static int PopulationSize = 6;
    public static int TournamentSize = 5;
    public static int NumParentsTournament = 2;
}

public class GeneticRoomGenerator
{
    readonly Sala sala;
    GeneticRoomIndividual[] population;
    readonly float crossoverProbability;
    readonly float mutationProbability;

    public GeneticRoomGenerator(Sala sala, float crossoverProbability, float mutationProbability)
    {
        this.sala = sala;
        this.mutationProbability = mutationProbability;
        this.crossoverProbability = crossoverProbability;
        population = new GeneticRoomIndividual[GeneticAlgorithmConstants.PopulationSize];
    }

    void GeneratePopulation()
    {
        for (int i = 0; i < GeneticAlgorithmConstants.PopulationSize; i++)
        {
            GeneticRoomIndividual individual = new(sala);
            population[i] = individual;
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

        Dictionary<Possibilidades, List<Position>> posicaoDosInimigosNoPai = RoomOperations.GroupPositionsByMatrixValue(pai.roomMatrix, pai.enemiesPositions);
        Dictionary<Possibilidades, List<Position>> posicaoDosObstaculosNoPai = RoomOperations.GroupPositionsByMatrixValue(pai.roomMatrix, pai.obstaclesPositions);

        Dictionary<Possibilidades, List<Position>> posicaoDosInimigosNaMae = RoomOperations.GroupPositionsByMatrixValue(mae.roomMatrix, mae.enemiesPositions);
        Dictionary<Possibilidades, List<Position>> posicaoDosObstaculosNaMae = RoomOperations.GroupPositionsByMatrixValue(mae.roomMatrix, mae.obstaclesPositions);

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

            Position[] chosenPositions = combinedPositions.SelectRandomDistinctElements(posicaoDosInimigosNaMae[key].Count);
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

            Position[] chosenPositions = combinedPositions.SelectRandomDistinctElements(posicaoDosObstaculosNoPai[key].Count);
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
            int faltando = chosenPositions.Length - colocados;
            Position[] positions = new HashSet<Position>(possiblePositions).SelectRandomDistinctElements(faltando);
            foreach (Position pos in positions)
            {
                individual.PutObstacleInPosition(key, pos);
            }
        }

        return individual;
    }

    public GeneticRoomIndividual[] TournamentSelection()
    {
        GeneticRoomIndividual[] parents = new GeneticRoomIndividual[GeneticAlgorithmConstants.NumParentsTournament];

        for (int i = 0; i < GeneticAlgorithmConstants.NumParentsTournament; i++)
        {
            List<GeneticRoomIndividual> tournament = new();

            // Seleciona aleatoriamente "tournamentSize" indivíduos para o torneio
            tournament = population.SelectRandomDistinctElements(GeneticAlgorithmConstants.TournamentSize).ToList();

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
            parents[i] = tournament[0];
        }

        return parents;
    }

    void Reproduction()
    {
        GeneticRoomIndividual[] newPopulation = new GeneticRoomIndividual[GeneticAlgorithmConstants.PopulationSize];

        for (int count = 0; count < GeneticAlgorithmConstants.PopulationSize; count += 2)
        {
            GeneticRoomIndividual[] parents = TournamentSelection();

            if (Random.value < crossoverProbability)
            {
                GeneticRoomIndividual children1 = Crossover(parents[0], parents[1]);
                GeneticRoomIndividual children2 = Crossover(parents[1], parents[0]);

                newPopulation[count] = children1;
                newPopulation[count + 1] = children2;
            }
            else
            {
                // Se nao houver cruzamento, copie os pais diretamente para a nova populacao
                newPopulation[count] = parents[0];
                newPopulation[count + 1] = parents[1];
            }
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
        Debug.Log("qntInimigosProximosDeObstaculos: " + RoomOperations.CountEnemiesNextToObstacles(melhorIndividuo.roomMatrix, melhorIndividuo.obstaclesPositions, sala.enemiesToPossibilidades));

        // retornar o melhor
        return melhorIndividuo.roomMatrix;
    }
}
