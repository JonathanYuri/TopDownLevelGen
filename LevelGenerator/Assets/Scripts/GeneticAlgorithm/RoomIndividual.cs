using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomIndividual
{
    RoomMatrix roomMatrix;
    int value;
    bool itWasModified = true;

    public int Value { get => value; set => this.value = value; }
    public bool ItWasModified { get => itWasModified; set => itWasModified = value; }
    public RoomMatrix RoomMatrix { get => roomMatrix; set => roomMatrix = value; }

    public RoomIndividual(bool generateRandomly = true)
    {
        Value = default;
        RoomMatrix = new RoomMatrix((RoomContents[,])GeneticAlgorithmConstants.ROOM.Values.Clone());

        if (generateRandomly)
        {
            GenerateRoomRandomly();
        }
    }

    public RoomIndividual(RoomIndividual individual)
    {
        Value = individual.Value;
        RoomMatrix = individual.RoomMatrix;
    }

    void GenerateRoomRandomly()
    {
        List<Position> avaliablePositions = new(GeneticAlgorithmConstants.ROOM.ChangeablesPositions);
        int qntObjects = GeneticAlgorithmConstants.ROOM.Enemies.Length + GeneticAlgorithmConstants.ROOM.Obstacles.Length;
        Position[] chosenPositions = avaliablePositions.SelectRandomDistinctElements(qntObjects);

        int count = 0;
        foreach (RoomContents enemy in GeneticAlgorithmConstants.ROOM.Enemies)
        {
            RoomMatrix.PutContentInPosition(enemy, RoomMatrix.EnemiesPositions, chosenPositions[count]);
            count++;
        }

        foreach (RoomContents obstacle in GeneticAlgorithmConstants.ROOM.Obstacles)
        {
            RoomMatrix.PutContentInPosition(obstacle, RoomMatrix.ObstaclesPositions, chosenPositions[count]);
            count++;
        }
    }

    bool IsMonstrous()
    {
        if (!PathFinder.IsAPathBetweenDoors(RoomMatrix.Values))
        {
            return true;
        }

        if (!PathFinder.IsAPathBetweenDoorAndEnemies(RoomMatrix))
        {
            //Debug.Log("Mostro por causa do caminho ate inimigos");
            return true;
        }

        bool hasTheRightAmountOfEnemies = RoomMatrix.EnemiesPositions.Count == GeneticAlgorithmConstants.ROOM.Enemies.Length;
        if (!hasTheRightAmountOfEnemies)
        {
            //Debug.Log("Mostro por causa da quantidade de inimigos");
            return true;
        }

        bool hasTheRightAmountOfObstacles = RoomMatrix.ObstaclesPositions.Count == GeneticAlgorithmConstants.ROOM.Obstacles.Length;
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

    public void Evaluate(List<int> vars, List<Range> bounds)
    {
        if (IsMonstrous())
        {
            Value = int.MinValue;
            return;
        }

        //Debug.Log("Total de grupos de Enemies na matriz: " + groups.Count);
        //Debug.Log("Média do tamanho dos grupos: " + media);

        //int qntInimigosProximosDeObstaculos = Utils.CountEnemiesNextToObstacles(roomMatrix);

        Value = 0;
        for (int i = 0; i < vars.Count; i++)
        {
            double normalizedValue = Utils.Normalization(vars[i], bounds[i].min, bounds[i].max);
            Value += (int)normalizedValue;

            /*
            if (i == 2)
            {
                Debug.Log("DISTANCIA: " + vars[i] + " valor: " + normalizedValue);
            }
            */
            //Debug.Log("Var: " + i);
            //Debug.Log("NormalizedValue: " + normalizedValue + " var: " + vars[i] + " bounds: " + bounds[i].min + " x " + bounds[i].max);
        }

        //Value = - groups.Count - (int)media + (int)value; // + qntInimigosProximosDeObstaculos;
    }
}
