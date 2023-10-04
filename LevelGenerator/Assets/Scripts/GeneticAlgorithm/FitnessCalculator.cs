using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FitnessCalculator
{
    Dictionary<int, List<int>> allFitness;
    List<Range> boundsOfFitnessVars; // pra normalizar as variaveis da fitness do individual

    public FitnessCalculator()
    {
        boundsOfFitnessVars = new();
        allFitness = new();
    }

    void DetermineFitnessVarBounds()
    {
        for (int i = 0; i < allFitness[0].Count; i++) // pra cada variavel do fitness
        {
            int max = allFitness.Max(fitness => fitness.Value[i]);
            int min = allFitness.Min(fitness => fitness.Value[i]);

            if (i < boundsOfFitnessVars.Count) // se tiver elementos naquela posicao
            {
                boundsOfFitnessVars[i].max = Mathf.Max(max, boundsOfFitnessVars[i].max);
                boundsOfFitnessVars[i].min = Mathf.Min(min, boundsOfFitnessVars[i].min);
            }
            else
            {
                boundsOfFitnessVars.Add(new Range { max = max, min = min });
            }
        }
    }

    void CalculeAllFitnessVars(RoomIndividual[] population)
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

    public void EvaluatePopulation(RoomIndividual[] population)
    {
        CalculeAllFitnessVars(population);
        DetermineFitnessVarBounds();

        // avaliar o individual se ele foi modificado
        for (int i = 0; i < population.Length; i++)
        {
            if (population[i].ItWasModified)
            {
                Evaluate(population[i], allFitness[i], boundsOfFitnessVars);
                population[i].ItWasModified = false;
            }
        }
    }

    public void Evaluate(RoomIndividual individual, List<int> vars, List<Range> bounds)
    {
        if (IsMonstrous(individual))
        {
            individual.Value = int.MinValue;
            return;
        }

        //Debug.Log("Total de grupos de Enemies na matriz: " + groups.Count);
        //Debug.Log("Média do tamanho dos grupos: " + media);

        //int qntInimigosProximosDeObstaculos = Utils.CountEnemiesNextToObstacles(roomMatrix);

        int value = 0;
        for (int i = 0; i < vars.Count; i++)
        {
            double normalizedValue = Utils.Normalization(vars[i], bounds[i].min, bounds[i].max);
            value += (int)normalizedValue;

            /*
            if (i == 2)
            {
                Debug.Log("DISTANCIA: " + vars[i] + " valor: " + normalizedValue);
            }
            */
            //Debug.Log("Var: " + i);
            //Debug.Log("NormalizedValue: " + normalizedValue + " var: " + vars[i] + " bounds: " + bounds[i].min + " x " + bounds[i].max);
        }

        individual.Value = value;
        //Value = - groups.Count - (int)media + (int)value; // + qntInimigosProximosDeObstaculos;
    }

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
