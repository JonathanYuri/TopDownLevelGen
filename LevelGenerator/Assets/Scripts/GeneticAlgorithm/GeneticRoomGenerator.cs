using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GeneticAlgorithmConstants
{
    // TODO: mudar a probabilidade de crossover e mutacao durante a execucao?
    // a diversidade da populacao seria baseado na distancia do min pro max de cada variavel do fitness

    public static int IterationsWithoutImprovement = 20;
    public static float CrossoverProbability = 0.8f; // 0 a 1
    public static float MutationProbability = 0.8f; // 0 a 1
    public static float Difficult = 0f; // 0 a 1
    public static int PopulationSize = 6;
    public static int TournamentSize = 5;
    public static int NumParentsTournament = 2;
    static Sala sala;

    public static Sala Sala { get => sala; set => sala = value; }

    public static void LimitVariables()
    {
        CrossoverProbability = Mathf.Clamp(CrossoverProbability, 0f, 1f);
        MutationProbability = Mathf.Clamp(MutationProbability, 0f, 1f);
        Difficult = Mathf.Clamp(Difficult, 0f, 1f);
    }
}

public class GeneticRoomGenerator
{
    RoomIndividual[] population;
    List<Range> boundsOfFitnessVars; // pra normalizar as variaveis da fitness do individuo
    Dictionary<int, List<int>> allFitness;

    public GeneticRoomGenerator(Sala sala)
    {
        boundsOfFitnessVars = new();
        allFitness = new();
        GeneticAlgorithmConstants.Sala = sala;
        GeneticAlgorithmConstants.LimitVariables();
        population = new RoomIndividual[GeneticAlgorithmConstants.PopulationSize];
    }

    void GeneratePopulation()
    {
        for (int i = 0; i < GeneticAlgorithmConstants.PopulationSize; i++)
        {
            population[i] = new(GeneticAlgorithmConstants.Sala);
        }
    }

    void CalculeAllFitnessVars()
    {
        for (int i = 0; i < population.Length; i++)
        {
            if (!population[i].ItWasModified) // so recalcular os fitness de quem foi modificado
            {
                continue;
            }
            // 
            List<int> groups = GroupCounter.CountGroups(population[i].RoomValues, population[i].EnemiesPositions);
            double media = groups.Average();

            //
            double averageDistanceFromDoorsToEnemies = RoomOperations.AverageDistanceFromDoorsToEnemies(GeneticAlgorithmConstants.Sala.doorsPositions, population[i].EnemiesPositions);
            float valueWhenDifficultyIsMinimal = (float)averageDistanceFromDoorsToEnemies; // maximizar a distancia entre os inimigos e as portas
            float valueWhenDifficultyIsMaximal = (float)-averageDistanceFromDoorsToEnemies; // minimizar a distancia entre os inimigos e as portas
            float value = Mathf.Lerp(valueWhenDifficultyIsMinimal, valueWhenDifficultyIsMaximal, GeneticAlgorithmConstants.Difficult);

            //
            int var1 = -groups.Count; // minimizar a quantidade de grupos
            int var2 = -(int)media; // minimizar a media de inimigos por grupos
            int var3 = (int)value; // maximizar o value

            allFitness[i] = new List<int> { var1, var2, var3 };
        }
    }

    void DetermineFitnessVarBounds()
    {
        for (int i = 0; i < allFitness[0].Count; i++)
        {
            int max = allFitness.Max(fitness => fitness.Value[i]);
            int min = allFitness.Min(fitness => fitness.Value[i]);

            if (i < boundsOfFitnessVars.Count) // tentar trocar
            {
                if (max > boundsOfFitnessVars[i].max)
                {
                    boundsOfFitnessVars[i].max = max;
                }
                if (min < boundsOfFitnessVars[i].min)
                {
                    boundsOfFitnessVars[i].min = min;
                }
            }
            else
            {
                boundsOfFitnessVars.Add(new Range { max = max, min = min });
            }
        }
    }

    void EvaluatePopulation()
    {
        CalculeAllFitnessVars();
        DetermineFitnessVarBounds();

        // avaliar o individuo se ele foi modificado
        for (int i = 0; i < population.Length; i++)
        {
            if (population[i].ItWasModified)
            {
                population[i].Evaluate(allFitness[i], boundsOfFitnessVars);
                population[i].ItWasModified = false;
            }
        }
    }

    void MutatePopulation()
    {
        foreach (RoomIndividual individual in population)
        {
            if (Random.value < GeneticAlgorithmConstants.MutationProbability)
            {
                individual.Mutate();
                individual.ItWasModified = true;
            }
        }
    }

    void PlaceEnemiesInChild(
        RoomIndividual child,
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
        RoomIndividual child,
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

    RoomIndividual Crossover(RoomIndividual pai, RoomIndividual mae)
    {
        RoomIndividual child = new(GeneticAlgorithmConstants.Sala, false);

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

    RoomIndividual[] TournamentSelection()
    {
        RoomIndividual[] parents = new RoomIndividual[GeneticAlgorithmConstants.NumParentsTournament];

        for (int i = 0; i < GeneticAlgorithmConstants.NumParentsTournament; i++)
        {
            List<RoomIndividual> tournament = new();

            // Seleciona aleatoriamente "tournamentSize" indivíduos para o torneio
            tournament = population.SelectRandomDistinctElements(GeneticAlgorithmConstants.TournamentSize).ToList();

            // Ordena os indivíduos do torneio por fitness (do melhor para o pior)
            tournament.Sort((a, b) =>
            {
                return b.Value.CompareTo(a.Value);
            });

            // O vencedor do torneio é selecionado para reprodução
            parents[i] = tournament[0];
        }

        return parents;
    }

    void Reproduction()
    {
        RoomIndividual[] newPopulation = new RoomIndividual[GeneticAlgorithmConstants.PopulationSize];

        for (int count = 0; count < GeneticAlgorithmConstants.PopulationSize; count += 2)
        {
            RoomIndividual[] parents = TournamentSelection();

            if (Random.value < GeneticAlgorithmConstants.CrossoverProbability)
            {
                RoomIndividual children1 = Crossover(parents[0], parents[1]);
                RoomIndividual children2 = Crossover(parents[1], parents[0]);

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

        RoomIndividual melhorIndividuo = new(population.MaxBy(individuo => individuo.Value));

        int iterationsWithoutImprovement = 0;
        int numInteracoes = 0;
        while (iterationsWithoutImprovement < GeneticAlgorithmConstants.IterationsWithoutImprovement || melhorIndividuo.Value == int.MinValue)
        {
            RoomIndividual bestInGeneration = population.MaxBy(individuo => individuo.Value);
            
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

        RoomIndividual bestInGenerationFinal = population.MaxBy(individuo => individuo.Value);
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
