using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GeneticAlgorithmConstants
{
    // TODO: mudar a probabilidade de crossover e mutacao durante a execucao?

    public static int ITERATIONS_WITHOUT_IMPROVEMENT = 20;
    public static float CROSSOVER_PROBABILITY = 0.8f; // 0 a 1
    public static float MUTATION_PROBABILITY = 0.8f; // 0 a 1
    public static float DIFFICULTY = 0f; // 0 a 1
    public static int POPULATION_SIZE = 6;
    public static int TOURNAMENT_SIZE = 5;
    public static int NUM_PARENTS_TOURNAMENT = 2;
    public static Room ROOM;

    public static void LimitVariables()
    {
        CROSSOVER_PROBABILITY = Mathf.Clamp(CROSSOVER_PROBABILITY, 0f, 1f);
        MUTATION_PROBABILITY = Mathf.Clamp(MUTATION_PROBABILITY, 0f, 1f);
        DIFFICULTY = Mathf.Clamp(DIFFICULTY, 0f, 1f);
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
        GeneticAlgorithmConstants.ROOM = room;
        GeneticAlgorithmConstants.LimitVariables();
        population = new RoomIndividual[GeneticAlgorithmConstants.POPULATION_SIZE];
    }

    void GeneratePopulation()
    {
        for (int i = 0; i < GeneticAlgorithmConstants.POPULATION_SIZE; i++)
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
            double averageDistanceFromDoorsToEnemies = RoomOperations.AverageDistanceFromDoorsToEnemies(population[i].RoomMatrix.EnemiesPositions);
            //double averageDistanceFromDoorsToEnemies = RoomOperations.MinimumDistanceBetweenDoorsAndEnemies(GeneticAlgorithmConstants.Room.doorsPositions, population[i].EnemiesPositions);
            float valueWhenDifficultyIsMinimal = (float)averageDistanceFromDoorsToEnemies; // maximizar a distancia entre os inimigos e as portas
            float valueWhenDifficultyIsMaximal = (float)-averageDistanceFromDoorsToEnemies; // minimizar a distancia entre os inimigos e as portas
            float value = Mathf.Lerp(valueWhenDifficultyIsMinimal, valueWhenDifficultyIsMaximal, GeneticAlgorithmConstants.DIFFICULTY);

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
            if (Random.value < GeneticAlgorithmConstants.MUTATION_PROBABILITY)
            {
                individual.Mutate();
                individual.ItWasModified = true;
            }
        }
    }

    void PlaceContentsInChild(
        RoomIndividual child,
        Dictionary<RoomContents, List<Position>> contentsPositionsInFather,
        Dictionary<RoomContents, List<Position>> contentsPositionsInMother,
        HashSet<Position> contentsPositionsToBePlaced,
        List<Position> avaliablePositions)
    {
        foreach (RoomContents key in contentsPositionsInFather.Keys)
        {
            HashSet<Position> combinedPositions = Utils.CombinePositions(contentsPositionsInFather[key], contentsPositionsInMother[key]);
            combinedPositions = combinedPositions.Intersect(avaliablePositions).ToHashSet(); // pra sempre ser uma posicao valida
            Position[] chosenPositions = combinedPositions.SelectRandomDistinctElements(contentsPositionsInFather[key].Count);

            foreach (Position pos in chosenPositions)
            {
                child.RoomMatrix.PutContentInPosition(key, contentsPositionsToBePlaced, pos);
                avaliablePositions.Remove(pos);
            }

            // nao colocou todos, colocar o resto aleatoriamente no que eu tenho
            int missing = contentsPositionsInFather[key].Count - chosenPositions.Length;
            if (missing == 0) continue;

            Position[] positions = avaliablePositions.SelectRandomDistinctElements(missing);
            foreach (Position pos in positions)
            {
                child.RoomMatrix.PutContentInPosition(key, contentsPositionsToBePlaced, pos);
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

        List<Position> avaliablePositions = new(GeneticAlgorithmConstants.ROOM.ChangeablesPositions);

        PlaceContentsInChild(child, enemiesPositionsInFather, enemiesPositionsInMother, child.RoomMatrix.EnemiesPositions, avaliablePositions);
        PlaceContentsInChild(child, obstaclesPositionsInFather, obstaclesPositionsInMother, child.RoomMatrix.ObstaclesPositions, avaliablePositions);

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
        RoomIndividual[] parents = new RoomIndividual[GeneticAlgorithmConstants.NUM_PARENTS_TOURNAMENT];

        for (int i = 0; i < GeneticAlgorithmConstants.NUM_PARENTS_TOURNAMENT; i++)
        {
            List<RoomIndividual> tournament = new();

            tournament = population.SelectRandomDistinctElements(GeneticAlgorithmConstants.TOURNAMENT_SIZE).ToList();

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
        RoomIndividual[] newPopulation = new RoomIndividual[GeneticAlgorithmConstants.POPULATION_SIZE];

        for (int count = 0; count < GeneticAlgorithmConstants.POPULATION_SIZE; count += 2)
        {
            RoomIndividual[] parents = TournamentSelection();

            if (Random.value < GeneticAlgorithmConstants.CROSSOVER_PROBABILITY)
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
            RoomOperations.CountEnemiesNextToObstacles(best.RoomMatrix));
        // retornar o melhor
        return best.RoomMatrix.Values;
    }

    bool ShouldContinueLooping(int iterationsWithoutImprovement, RoomIndividual best)
    {
        if (iterationsWithoutImprovement < GeneticAlgorithmConstants.ITERATIONS_WITHOUT_IMPROVEMENT)
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
