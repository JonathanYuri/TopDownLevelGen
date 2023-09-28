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
    public static float Difficulty = 0f; // 0 a 1
    public static int PopulationSize = 6;
    public static int TournamentSize = 5;
    public static int NumParentsTournament = 2;
    public static Room Room;

    public static void LimitVariables()
    {
        CrossoverProbability = Mathf.Clamp(CrossoverProbability, 0f, 1f);
        MutationProbability = Mathf.Clamp(MutationProbability, 0f, 1f);
        Difficulty = Mathf.Clamp(Difficulty, 0f, 1f);
    }
}

public class GeneticRoomGenerator
{
    RoomIndividual[] population;
    List<Range> boundsOfFitnessVars; // pra normalizar as variaveis da fitness do individual
    Dictionary<int, List<int>> allFitness;

    public GeneticRoomGenerator(Room room)
    {
        boundsOfFitnessVars = new();
        allFitness = new();
        GeneticAlgorithmConstants.Room = room;
        GeneticAlgorithmConstants.LimitVariables();
        population = new RoomIndividual[GeneticAlgorithmConstants.PopulationSize];
    }

    void GeneratePopulation()
    {
        for (int i = 0; i < GeneticAlgorithmConstants.PopulationSize; i++)
        {
            population[i] = new();
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
            List<int> groups = GroupCounter.CountGroups(population[i].RoomMatrix.Values, population[i].RoomMatrix.EnemiesPositions);
            double media = groups.Average();

            //
            double averageDistanceFromDoorsToEnemies = RoomOperations.AverageDistanceFromDoorsToEnemies(GeneticAlgorithmConstants.Room.doorsPositions, population[i].RoomMatrix.EnemiesPositions);
            //double averageDistanceFromDoorsToEnemies = RoomOperations.MinimumDistanceBetweenDoorsAndEnemies(GeneticAlgorithmConstants.Room.doorsPositions, population[i].EnemiesPositions);
            float valueWhenDifficultyIsMinimal = (float)averageDistanceFromDoorsToEnemies; // maximizar a distancia entre os inimigos e as portas
            float valueWhenDifficultyIsMaximal = (float)-averageDistanceFromDoorsToEnemies; // minimizar a distancia entre os inimigos e as portas
            float value = Mathf.Lerp(valueWhenDifficultyIsMinimal, valueWhenDifficultyIsMaximal, GeneticAlgorithmConstants.Difficulty);

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

        // avaliar o individual se ele foi modificado
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
                child.RoomMatrix.PutEnemyInPosition(key, pos);
                avaliablePositions.Remove(pos);
            }

            // nao colocou todos, colocar o resto aleatoriamente no que eu tenho
            int missing = enemiesPositionsInFather[key].Count - chosenPositions.Length;
            if (missing == 0) continue;

            Position[] positions = avaliablePositions.SelectRandomDistinctElements(missing);
            foreach (Position pos in positions)
            {
                child.RoomMatrix.PutEnemyInPosition(key, pos);
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
                child.RoomMatrix.PutObstacleInPosition(key, pos);
                avaliablePositions.Remove(pos);
            }

            // nao colocou todos, colocar o resto aleatoriamente no que eu tenho
            int missing = obstaclesPositionsInFather[key].Count - chosenPositions.Length;
            if (missing == 0) continue;

            Position[] positions = avaliablePositions.SelectRandomDistinctElements(missing);
            foreach (Position pos in positions)
            {
                child.RoomMatrix.PutObstacleInPosition(key, pos);
                avaliablePositions.Remove(pos);
            }
        }
    }

    RoomIndividual Crossover(RoomIndividual father, RoomIndividual mother)
    {
        RoomIndividual child = new(false);

        Dictionary<RoomContents, List<Position>> enemiesPositionsInFather = RoomOperations.GroupPositionsByRoomValue(father.RoomMatrix.Values, father.RoomMatrix.EnemiesPositions);
        Dictionary<RoomContents, List<Position>> enemiesPositionsInMother = RoomOperations.GroupPositionsByRoomValue(mother.RoomMatrix.Values, mother.RoomMatrix.EnemiesPositions);

        Dictionary<RoomContents, List<Position>> obstaclesPositionsInFather = RoomOperations.GroupPositionsByRoomValue(father.RoomMatrix.Values, father.RoomMatrix.ObstaclesPositions);
        Dictionary<RoomContents, List<Position>> obstaclesPositionsInMother = RoomOperations.GroupPositionsByRoomValue(mother.RoomMatrix.Values, mother.RoomMatrix.ObstaclesPositions);

        if (enemiesPositionsInMother.Keys.Count != enemiesPositionsInFather.Keys.Count) throw new Exception("Inimigos diferentes no father e na mother");
        if (obstaclesPositionsInMother.Keys.Count != obstaclesPositionsInFather.Keys.Count) throw new Exception("Obstaculos diferentes no father e na mother");

        List<Position> avaliablePositions = new(GeneticAlgorithmConstants.Room.changeablesPositions);
        PlaceEnemiesInChild(child, enemiesPositionsInFather, enemiesPositionsInMother, avaliablePositions);
        PlaceObstaclesInChild(child, obstaclesPositionsInFather, obstaclesPositionsInMother, avaliablePositions);

        if (child.RoomMatrix.EnemiesPositions.Count != father.RoomMatrix.EnemiesPositions.Count
                                        ||
            child.RoomMatrix.ObstaclesPositions.Count != father.RoomMatrix.ObstaclesPositions.Count)
        {
            throw new ReproductionException("Gerou individual monstro na Reproducao",
                father.RoomMatrix.EnemiesPositions, mother.RoomMatrix.EnemiesPositions, child.RoomMatrix.EnemiesPositions,
                father.RoomMatrix.ObstaclesPositions, mother.RoomMatrix.ObstaclesPositions, child.RoomMatrix.ObstaclesPositions);
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
                // Se nao houver cruzamento, copie os fathers diretamente para a nova populacao
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

        RoomIndividual best = new(population.MaxBy(individual => individual.Value));

        int iterationsWithoutImprovement = 0;
        int iterations = 0;
        while (ShouldContinueLooping(iterationsWithoutImprovement, best))
        {
            RoomIndividual bestInGeneration = population.MaxBy(individual => individual.Value);
            
            Debug.Log("NUMERO DE INTERACOES: " + iterations + " MELHOR ATUAL: " + bestInGeneration.Value + " MELHOR: " + best.Value);
            if (bestInGeneration.Value > best.Value)
            {
                iterationsWithoutImprovement = 0;
                best = new(bestInGeneration);
            }
            else
            {
                iterationsWithoutImprovement++;
            }

            Reproduction();
            MutatePopulation();
            EvaluatePopulation();

            iterations++;
        }

        RoomIndividual bestInGenerationFinal = population.MaxBy(individual => individual.Value);
        if (bestInGenerationFinal.Value > best.Value)
        {
            best = new(bestInGenerationFinal);
        }

        Debug.Log("Melhor individual: " + best.Value);
        Debug.Log("Inimigo: " + best.RoomMatrix.EnemiesPositions.Count);
        Debug.Log("Obstaculo: " + best.RoomMatrix.ObstaclesPositions.Count);
        Debug.Log("qntInimigosProximosDeObstaculos: " +
            RoomOperations.CountEnemiesNextToObstacles(best.RoomMatrix.Values, best.RoomMatrix.EnemiesPositions, best.RoomMatrix.ObstaclesPositions));
        // retornar o melhor
        return best.RoomMatrix.Values;
    }

    bool ShouldContinueLooping(int iterationsWithoutImprovement, RoomIndividual best)
    {
        if (iterationsWithoutImprovement < GeneticAlgorithmConstants.IterationsWithoutImprovement)
        {
            return true;
        }
        if (best.Value == int.MinValue)
        {
            return true;
        }
        return false;
    }
}
