using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages fitness calculation for room individuals in the genetic algorithm.
/// </summary>
public class FitnessCalculator
{
    const int numberOfFitnessVariables = 3;

    Dictionary<int, int[]> allFitness;
    List<Range> boundsOfFitnessVars; // pra normalizar as variaveis da fitness do individual
    bool areBoundsModified = true;

    float numGroupsImportance = 1f;
    float enemiesPerGroupAverageImportance = 1f;
    float enemyDoorDistanceImportance = 1f;

    enum FitnessVariable
    {
        NumGroups = 0,
        EnemiesPerGroupAverage = 1,
        EnemyDoorDistance = 2
    }

    Dictionary<FitnessVariable, float> importances;

    public FitnessCalculator()
    {
        allFitness = new();
        boundsOfFitnessVars = new();

        LimitImportances();

        importances = new()
        {
            { FitnessVariable.NumGroups, numGroupsImportance },
            { FitnessVariable.EnemiesPerGroupAverage, enemiesPerGroupAverageImportance },
            { FitnessVariable.EnemyDoorDistance, enemyDoorDistanceImportance }
        };
    }

    void LimitImportances()
    {
        numGroupsImportance = Mathf.Clamp(numGroupsImportance, 0f, 1f);
        enemiesPerGroupAverageImportance = Mathf.Clamp(enemiesPerGroupAverageImportance, 0f, 1f);
        enemyDoorDistanceImportance = Mathf.Clamp(enemyDoorDistanceImportance, 0f, 1f);
    }

    /// <summary>
    /// Determines the bounds of fitness variables based on the current and previous fitness values.
    /// </summary>
    void DetermineFitnessVarBounds()
    {
        bool boundsModified = false;
        for (int i = 0; i < numberOfFitnessVariables; i++) // pra cada variavel do fitness
        {
            (int max, int min) = allFitness.MaxAndMin(fitness => fitness.Value[i]);

            if (i < boundsOfFitnessVars.Count) // se tiver elementos naquela posicao
            {
                if (max > boundsOfFitnessVars[i].max)
                {
                    boundsOfFitnessVars[i].max = max;
                    boundsModified = true;
                }
                if (min < boundsOfFitnessVars[i].min)
                {
                    boundsOfFitnessVars[i].min = min;
                    boundsModified = true;
                }
            }
            else
            {
                boundsOfFitnessVars.Add(new Range { max = max, min = min });
            }
        }

       areBoundsModified = boundsModified;
    }

    /// <summary>
    /// Calculates fitness variables based on the characteristics of the room individual.
    /// </summary>
    /// <param name="individual">The room individual for which to calculate fitness variables.</param>
    /// <returns>A array of calculated fitness variables.</returns>
    int[] CalculeAllFitnessVars(RoomIndividual individual)
    {
        List<int> groups = GroupCounter.CountGroups(individual.RoomMatrix.Values, individual.RoomMatrix.EnemiesPositions);
        double media = groups.Average();

        double averageDistanceFromDoorsToEnemies = RoomOperations.AverageDistanceFromDoorsToEnemies(individual.RoomMatrix.EnemiesPositions);
        float valueWhenDifficultyIsMinimal = (float)averageDistanceFromDoorsToEnemies; // maximizar a distancia entre os inimigos e as portas
        float valueWhenDifficultyIsMaximal = (float)-averageDistanceFromDoorsToEnemies; // minimizar a distancia entre os inimigos e as portas

        //double minimunDistanceFromDoorsToEnemies = RoomOperations.MinimumDistanceBetweenDoorsAndEnemies(individual.RoomMatrix.EnemiesPositions);
        //float valueWhenDifficultyIsMinimal = (float)minimunDistanceFromDoorsToEnemies; // maximizar a minima distancia entre os inimigos e as portas
        //float valueWhenDifficultyIsMaximal = (float)-minimunDistanceFromDoorsToEnemies; // minimizar a minima distancia entre os inimigos e as portas

        // Calculate the final value by interpolating between minimal and maximal values based on the difficulty.
        float value = Mathf.Lerp(valueWhenDifficultyIsMinimal, valueWhenDifficultyIsMaximal, GeneticAlgorithmConstants.ROOM.Difficulty);

        int[] vars = new int[numberOfFitnessVariables];
        vars[(int)FitnessVariable.NumGroups] = -groups.Count; // minimizar a quantidade de grupos
        vars[(int)FitnessVariable.EnemiesPerGroupAverage] = -(int)media; // minimizar a media de inimigos por grupos
        vars[(int)FitnessVariable.EnemyDoorDistance] = (int)value; // maximizar o value
        return vars;
    }

    /// <summary>
    /// Determines whether the fitness should be recalculated for a specific individual.
    /// This is based on whether the individual has been modified or if the bounds of fitness variables have been modified.
    /// </summary>
    /// <param name="individual">The individual for which fitness is being evaluated.</param>
    /// <returns>True if fitness should be recalculated, false otherwise.</returns>
    bool ShouldRecalculateFitness(RoomIndividual individual)
    {
        if (individual.ItWasModified)
        {
            return true;
        }
        if (areBoundsModified)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Calculates fitness variables for all individuals within a population if recalculation is necessary.
    /// If an individual has been modified or the bounds of fitness variables have changed, their fitness variables are recalculated.
    /// </summary>
    /// <param name="population">The population of individuals for which to calculate fitness variables.</param>
    void CalculeAllFitnessVarsOfPopulation(RoomIndividual[] population)
    {
        for (int i = 0; i < population.Length; i++)
        {
            if (!ShouldRecalculateFitness(population[i]))
            {
                continue;
            }

            allFitness[i] = CalculeAllFitnessVars(population[i]);
        }
    }

    /// <summary>
    /// Evaluates the fitness of an entire population of individuals.
    /// Fitness variables are calculated for each individual, and bounds of fitness variables are determined.
    /// If an individual has been modified or the bounds of fitness variables have changed, their fitness is recalculated.
    /// </summary>
    /// <param name="population">The population of individuals to evaluate.</param>
    public void EvaluatePopulation(RoomIndividual[] population)
    {
        CalculeAllFitnessVarsOfPopulation(population);
        DetermineFitnessVarBounds();

        // avaliar o individual se ele foi modificado
        for (int i = 0; i < population.Length; i++)
        {
            if (!ShouldRecalculateFitness(population[i]))
            {
                continue;
            }

            Evaluate(population[i], allFitness[i]);
            population[i].ItWasModified = false;
        }
    }

    /// <summary>
    /// Calculates and assigns the fitness value for an individual based on provided fitness variables.
    /// If the individual is considered "monstrous," their fitness value is set to the minimum possible integer value.
    /// </summary>
    /// <param name="individual">The individual to evaluate.</param>
    /// <param name="allFitnessVars">An array of fitness variables used in the evaluation.</param>
    public void Evaluate(RoomIndividual individual, int[] allFitnessVars)
    {
        if (IsMonstrous(individual))
        {
            individual.Value = int.MinValue;
            return;
        }

        individual.Value = CalculateNormalizedFitnessValue(allFitnessVars);
        //Value = - groups.Count - (int)media + (int)value; // + qntInimigosProximosDeObstaculos;
    }

    /// <summary>
    /// Calculates and assigns the fitness value for an individual.
    /// Fitness variables are determined internally for the individual before evaluation.
    /// If the individual is considered "monstrous," their fitness value is set to the minimum possible integer value.
    /// </summary>
    /// <param name="individual">The individual to evaluate.</param>
    public void Evaluate(RoomIndividual individual)
    {
        if (IsMonstrous(individual))
        {
            individual.Value = int.MinValue;
            return;
        }

        individual.Value = CalculateNormalizedFitnessValue(CalculeAllFitnessVars(individual));
    }

    /// <summary>
    /// Calculates the normalized fitness value based on a set of fitness variables.
    /// Normalization ensures that the fitness value falls within specified bounds.
    /// </summary>
    /// <param name="fitnessVars">An array of fitness variables used to calculate the fitness value.</param>
    /// <returns>The normalized fitness value.</returns>
    int CalculateNormalizedFitnessValue(int[] fitnessVars)
    {
        int value = 0;
        for (int i = 0; i < numberOfFitnessVariables; i++)
        {
            double normalizedValue = Utils.Normalization(fitnessVars[i], boundsOfFitnessVars[i].min, boundsOfFitnessVars[i].max);
            value += (int)(normalizedValue * importances[(FitnessVariable)i]);
        }
        return value;
    }

    /// <summary>
    /// Determines if a given room individual is considered "monstrous" based on certain criteria.
    /// </summary>
    /// <param name="individual">The room individual to evaluate.</param>
    /// <returns>True if the individual is monstrous; otherwise, false.</returns>
    bool IsMonstrous(RoomIndividual individual)
    {
        if (!PathFinder.IsAPathBetweenDoors(individual.RoomMatrix.Values))
        {
            return true;
        }

        if (!PathFinder.IsAPathBetweenDoorAndEnemies(individual.RoomMatrix))
        {
            //Debug.Log("Mostro por causa do caminho ate inimigos");
            return true;
        }

        bool hasTheRightAmountOfEnemies = individual.RoomMatrix.EnemiesPositions.Count == GeneticAlgorithmConstants.ROOM.Enemies.Length;
        if (!hasTheRightAmountOfEnemies)
        {
            //Debug.Log("Mostro por causa da quantidade de inimigos");
            return true;
        }

        bool hasTheRightAmountOfObstacles = individual.RoomMatrix.ObstaclesPositions.Count == GeneticAlgorithmConstants.ROOM.Obstacles.Length;
        if (!hasTheRightAmountOfObstacles)
        {
            //Debug.Log("Mostro por causa da quantidade de obstaculos");
            return true;
        }

        /*
        if (!RoomOperations.HasTheRightObjects(roomMatrix, typeof(Enemies), enemiesPositions, room.enemies.Cast<object>().ToList()))
        {
            //Debug.Log("Mostro por causa dos inimigos");
            return true;
        }

        if (!RoomOperations.HasTheRightObjects(roomMatrix, typeof(Obstacles), obstaclesPositions, room.obstacles.Cast<object>().ToList()))
        {
            //Debug.Log("Mostro por causa dos obstaculo");
            return true;
        }
        */

        return false;
    }
}
