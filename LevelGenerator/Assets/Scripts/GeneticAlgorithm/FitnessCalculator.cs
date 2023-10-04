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
        for (int i = 0; i < allFitness[0].Count; i++)
        {
            int max = allFitness.Max(fitness => fitness.Value[i]);
            int min = allFitness.Min(fitness => fitness.Value[i]);

            if (i < boundsOfFitnessVars.Count) // se tiver elementos naquela posicao
            {
                max = Mathf.Max(max, boundsOfFitnessVars[i].max);
                min = Mathf.Min(min, boundsOfFitnessVars[i].min);
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
                population[i].Evaluate(allFitness[i], boundsOfFitnessVars);
                population[i].ItWasModified = false;
            }
        }
    }
}
