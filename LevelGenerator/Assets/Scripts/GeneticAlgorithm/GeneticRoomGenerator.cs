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
            population[i] = new(sala);
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

    void PlaceEnemiesInChild(
        GeneticRoomIndividual child,
        ICollection<RoomContents> enemies,
        Dictionary<RoomContents, List<Position>> enemiesPositionsInFather,
        Dictionary<RoomContents, List<Position>> enemiesPositionsInMother,
        List<Position> avaliablePositions)
    {
        foreach (RoomContents key in enemies)
        {
            HashSet<Position> combinedPositions = Utils.CombinePositions(enemiesPositionsInFather[key], enemiesPositionsInMother[key]);
            combinedPositions.Intersect(avaliablePositions); // pra sempre ser uma posicao valida
            Position[] chosenPositions = combinedPositions.SelectRandomDistinctElements(enemiesPositionsInFather[key].Count);

            foreach (Position pos in chosenPositions)
            {
                child.PutEnemyInPosition(key, pos);
                avaliablePositions.Remove(pos);
            }

            // nao colocou todos, colocar o resto aleatoriamente no que eu tenho
            int faltando = enemiesPositionsInFather[key].Count - chosenPositions.Length;
            Position[] positions = avaliablePositions.SelectRandomDistinctElements(faltando);
            foreach (Position pos in positions)
            {
                child.PutEnemyInPosition(key, pos);
                avaliablePositions.Remove(pos);
            }
        }
    }

    void PlaceObstaclesInChild(
        GeneticRoomIndividual child,
        ICollection<RoomContents> obstacles,
        Dictionary<RoomContents, List<Position>> obstaclesPositionsInFather,
        Dictionary<RoomContents, List<Position>> obstaclesPositionsInMother,
        List<Position> avaliablePositions)
    {
        foreach (RoomContents key in obstacles)
        {
            HashSet<Position> combinedPositions = Utils.CombinePositions(obstaclesPositionsInFather[key], obstaclesPositionsInMother[key]);
            combinedPositions.Intersect(avaliablePositions); // pra sempre ser uma posicao valida
            Position[] chosenPositions = combinedPositions.SelectRandomDistinctElements(obstaclesPositionsInFather[key].Count);

            foreach (Position pos in chosenPositions)
            {
                child.PutObstacleInPosition(key, pos);
                avaliablePositions.Remove(pos);
            }

            // nao colocou todos, colocar o resto aleatoriamente no que eu tenho
            int faltando = obstaclesPositionsInFather[key].Count - chosenPositions.Length;
            Position[] positions = avaliablePositions.SelectRandomDistinctElements(faltando);
            foreach (Position pos in positions)
            {
                child.PutObstacleInPosition(key, pos);
            }
        }
    }

    GeneticRoomIndividual Crossover(GeneticRoomIndividual pai, GeneticRoomIndividual mae)
    {
        GeneticRoomIndividual individual = new(sala, false);

        Dictionary<RoomContents, List<Position>> enemiesPositionsInFather = RoomOperations.GroupPositionsByRoomValue(pai.roomMatrix, pai.enemiesPositions);
        Dictionary<RoomContents, List<Position>> enemiesPositionsInMother = RoomOperations.GroupPositionsByRoomValue(mae.roomMatrix, mae.enemiesPositions);

        Dictionary<RoomContents, List<Position>> obstaclesPositionsInFather = RoomOperations.GroupPositionsByRoomValue(pai.roomMatrix, pai.obstaclesPositions);
        Dictionary<RoomContents, List<Position>> obstaclesPositionsInMother = RoomOperations.GroupPositionsByRoomValue(mae.roomMatrix, mae.obstaclesPositions);

        if (enemiesPositionsInMother.Keys.Count != enemiesPositionsInFather.Keys.Count) throw new Exception("Inimigos diferentes no pai e na mae");
        if (obstaclesPositionsInMother.Keys.Count != obstaclesPositionsInFather.Keys.Count) throw new Exception("Obstaculos diferentes no pai e na mae");

        List<Position> avaliablePositions = new(sala.changeablesPositions);

        PlaceEnemiesInChild(individual, enemiesPositionsInFather.Keys, enemiesPositionsInFather, enemiesPositionsInMother, avaliablePositions);
        PlaceObstaclesInChild(individual, obstaclesPositionsInFather.Keys, obstaclesPositionsInFather, obstaclesPositionsInMother, avaliablePositions);

        return individual;
    }

    GeneticRoomIndividual[] TournamentSelection()
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

    public RoomContents[,] GeneticLooping()
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
