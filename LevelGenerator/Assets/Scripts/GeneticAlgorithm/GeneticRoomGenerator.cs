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
    static Sala sala;

    public static Sala Sala { get => sala; set => sala = value; }
}

public class GeneticRoomGenerator
{
    GeneticRoomIndividual[] population;
    readonly float crossoverProbability;
    readonly float mutationProbability;

    public GeneticRoomGenerator(Sala sala, float crossoverProbability, float mutationProbability)
    {
        GeneticAlgorithmConstants.Sala = sala;
        this.mutationProbability = mutationProbability;
        this.crossoverProbability = crossoverProbability;
        population = new GeneticRoomIndividual[GeneticAlgorithmConstants.PopulationSize];
    }

    void GeneratePopulation()
    {
        for (int i = 0; i < GeneticAlgorithmConstants.PopulationSize; i++)
        {
            population[i] = new(GeneticAlgorithmConstants.Sala);
        }
    }

    void EvaluatePopulation()
    {
        // avaliar o individuo se ele foi modificado
        foreach (GeneticRoomIndividual individual in population)
        {
            if (individual.ItWasModified)
            {
                individual.Evaluate();
                individual.ItWasModified = false;
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
                individual.ItWasModified = true;
            }
        }
    }

    void PlaceEnemiesInChild(
        GeneticRoomIndividual child,
        Dictionary<RoomContents, List<Position>> enemiesPositionsInFather,
        Dictionary<RoomContents, List<Position>> enemiesPositionsInMother,
        List<Position> avaliablePositions)
    {
        foreach (RoomContents key in enemiesPositionsInFather.Keys)
        {
            HashSet<Position> combinedPositions = Utils.CombinePositions(enemiesPositionsInFather[key], enemiesPositionsInMother[key]);
            combinedPositions = combinedPositions.Intersect(avaliablePositions).ToHashSet(); // pra sempre ser uma posicao valida
            Position[] chosenPositions = combinedPositions.SelectRandomDistinctElements(enemiesPositionsInFather[key].Count);

            foreach (Position pos in chosenPositions)
            {
                child.PutEnemyInPosition(key, pos);
                avaliablePositions.Remove(pos);
            }

            // nao colocou todos, colocar o resto aleatoriamente no que eu tenho
            int faltando = enemiesPositionsInFather[key].Count - chosenPositions.Length;
            if (faltando == 0) continue;

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
        Dictionary<RoomContents, List<Position>> obstaclesPositionsInFather,
        Dictionary<RoomContents, List<Position>> obstaclesPositionsInMother,
        List<Position> avaliablePositions)
    {
        foreach (RoomContents key in obstaclesPositionsInFather.Keys)
        {
            HashSet<Position> combinedPositions = Utils.CombinePositions(obstaclesPositionsInFather[key], obstaclesPositionsInMother[key]);
            combinedPositions = combinedPositions.Intersect(avaliablePositions).ToHashSet(); // pra sempre ser uma posicao valida
            Position[] chosenPositions = combinedPositions.SelectRandomDistinctElements(obstaclesPositionsInFather[key].Count);

            foreach (Position pos in chosenPositions)
            {
                child.PutObstacleInPosition(key, pos);
                avaliablePositions.Remove(pos);
            }

            // nao colocou todos, colocar o resto aleatoriamente no que eu tenho
            int faltando = obstaclesPositionsInFather[key].Count - chosenPositions.Length;
            if (faltando == 0) continue;

            Position[] positions = avaliablePositions.SelectRandomDistinctElements(faltando);
            foreach (Position pos in positions)
            {
                child.PutObstacleInPosition(key, pos);
                avaliablePositions.Remove(pos);
            }
        }
    }

    GeneticRoomIndividual Crossover(GeneticRoomIndividual pai, GeneticRoomIndividual mae)
    {
        GeneticRoomIndividual child = new(GeneticAlgorithmConstants.Sala, false);

        Dictionary<RoomContents, List<Position>> enemiesPositionsInFather = RoomOperations.GroupPositionsByRoomValue(pai.RoomValues, pai.EnemiesPositions);
        Dictionary<RoomContents, List<Position>> enemiesPositionsInMother = RoomOperations.GroupPositionsByRoomValue(mae.RoomValues, mae.EnemiesPositions);

        Dictionary<RoomContents, List<Position>> obstaclesPositionsInFather = RoomOperations.GroupPositionsByRoomValue(pai.RoomValues, pai.ObstaclesPositions);
        Dictionary<RoomContents, List<Position>> obstaclesPositionsInMother = RoomOperations.GroupPositionsByRoomValue(mae.RoomValues, mae.ObstaclesPositions);

        if (enemiesPositionsInMother.Keys.Count != enemiesPositionsInFather.Keys.Count) throw new Exception("Inimigos diferentes no pai e na mae");
        if (obstaclesPositionsInMother.Keys.Count != obstaclesPositionsInFather.Keys.Count) throw new Exception("Obstaculos diferentes no pai e na mae");

        List<Position> avaliablePositions = new(GeneticAlgorithmConstants.Sala.changeablesPositions);
        PlaceEnemiesInChild(child, enemiesPositionsInFather, enemiesPositionsInMother, avaliablePositions);
        PlaceObstaclesInChild(child, obstaclesPositionsInFather, obstaclesPositionsInMother, avaliablePositions);

        if (child.EnemiesPositions.Count != pai.EnemiesPositions.Count
                                        ||
            child.ObstaclesPositions.Count != pai.ObstaclesPositions.Count)
        {
            throw new ReproductionException("Gerou individuo monstro na Reproducao",
                pai.EnemiesPositions, mae.EnemiesPositions, child.EnemiesPositions,
                pai.ObstaclesPositions, mae.ObstaclesPositions, child.ObstaclesPositions);
        }

        return child;
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
                if (a.Value.HasValue && b.Value.HasValue)
                    return b.Value.Value.CompareTo(a.Value.Value);
                else if (a.Value.HasValue)
                    return -1; // Coloca os valores não nulos antes dos valores nulos
                else if (b.Value.HasValue)
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

        GeneticRoomIndividual melhorIndividuo = new(population.MaxBy(individuo => individuo.Value));

        int iterationsWithoutImprovement = 0;
        int numInteracoes = 0;
        while (iterationsWithoutImprovement < GeneticAlgorithmConstants.IterationsWithoutImprovement || melhorIndividuo.Value == int.MinValue)
        {
            GeneticRoomIndividual bestInGeneration = population.MaxBy(individuo => individuo.Value);
            
            Debug.Log("NUMERO DE INTERACOES: " + numInteracoes + " MELHOR ATUAL: " + bestInGeneration.Value + " MELHOR: " + melhorIndividuo.Value);
            if (bestInGeneration.Value > melhorIndividuo.Value)
            {
                iterationsWithoutImprovement = 0;
                melhorIndividuo = new(bestInGeneration);
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

        GeneticRoomIndividual bestInGenerationFinal = population.MaxBy(individuo => individuo.Value);
        if (bestInGenerationFinal.Value > melhorIndividuo.Value)
        {
            melhorIndividuo = new(bestInGenerationFinal);
        }

        Debug.Log("Melhor individuo: " + melhorIndividuo.Value);
        Debug.Log("Inimigo: " + melhorIndividuo.EnemiesPositions.Count);
        Debug.Log("Obstaculo: " + melhorIndividuo.ObstaclesPositions.Count);
        Debug.Log("qntInimigosProximosDeObstaculos: " + RoomOperations.CountEnemiesNextToObstacles(melhorIndividuo.RoomValues, melhorIndividuo.EnemiesPositions, melhorIndividuo.ObstaclesPositions));

        // retornar o melhor
        return melhorIndividuo.RoomValues;
    }
}
