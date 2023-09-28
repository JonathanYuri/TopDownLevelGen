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
        RoomMatrix = new RoomMatrix((RoomContents[,])GeneticAlgorithmConstants.Room.Values.Clone());

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
        List<Position> avaliablePositions = new(GeneticAlgorithmConstants.Room.changeablesPositions);
        int qntObjects = GeneticAlgorithmConstants.Room.Enemies.Length + GeneticAlgorithmConstants.Room.Obstacles.Length;
        Position[] chosenPositions = avaliablePositions.SelectRandomDistinctElements(qntObjects);

        int count = 0;
        foreach (RoomContents enemy in GeneticAlgorithmConstants.Room.Enemies)
        {
            RoomMatrix.PutEnemyInPosition(enemy, chosenPositions[count]);
            count++;
        }

        foreach (RoomContents obstacle in GeneticAlgorithmConstants.Room.Obstacles)
        {
            RoomMatrix.PutObstacleInPosition(obstacle, chosenPositions[count]);
            count++;
        }
    }

    void ChangePlaceOf(HashSet<Position> positionsOf, Position position1)
    {
        int idx2 = Random.Range(0, GeneticAlgorithmConstants.Room.changeablesPositions.Count);
        Position position2 = GeneticAlgorithmConstants.Room.changeablesPositions[idx2];

        RoomContents content1 = RoomMatrix.Values[position1.X, position1.Y];
        RoomContents content2 = RoomMatrix.Values[position2.X, position2.Y];

        // Colocar na posicao 1 o conteudo da posicao 2
        if (GeneticAlgorithmConstants.Room.Enemies.Contains(RoomMatrix.Values[position2.X, position2.Y]))
        {
            RoomMatrix.RemoveFromPosition(RoomMatrix.EnemiesPositions, position2);
            RoomMatrix.PutEnemyInPosition(content2, position1);
        }
        else if (GeneticAlgorithmConstants.Room.Obstacles.Contains(RoomMatrix.Values[position2.X, position2.Y]))
        {
            RoomMatrix.RemoveFromPosition(RoomMatrix.ObstaclesPositions, position2);
            RoomMatrix.PutObstacleInPosition(content2, position1);
        }
        else
        {
            RoomMatrix.Values[position1.X, position1.Y] = content2;
        }

        // Colocar na posicao 2 o conteudo da posicao 1
        RoomMatrix.RemoveFromPosition(positionsOf, position1);
        if (positionsOf.Equals(RoomMatrix.EnemiesPositions))
        {
            RoomMatrix.PutEnemyInPosition(content1, position2);
        }
        else if (positionsOf.Equals(RoomMatrix.ObstaclesPositions))
        {
            RoomMatrix.PutObstacleInPosition(content1, position2);
        }
    }

    void Mutate(HashSet<Position> positionsToMutate)
    {
        Position[] positionsToChange = positionsToMutate.SelectRandomDistinctElements(Random.Range(0, positionsToMutate.Count));
        foreach (Position position in positionsToChange)
        {
            ChangePlaceOf(positionsToMutate, position);
        }
    }

    public void Mutate()
    {
        // escolher inimigos ou obstaculos para mudar
        if (Random.value < 0.5f)
        {
            Mutate(RoomMatrix.EnemiesPositions);
        }
        else
        {
            Mutate(RoomMatrix.ObstaclesPositions);
        }
    }

    bool IsMonstrous()
    {
        if (!PathFinder.IsAPathBetweenDoors(RoomMatrix.Values, GeneticAlgorithmConstants.Room.doorsPositions))
        {
            return true;
        }

        if (!PathFinder.IsAPathBetweenDoorAndEnemies(RoomMatrix.Values, GeneticAlgorithmConstants.Room.doorsPositions, RoomMatrix.EnemiesPositions))
        {
            //Debug.Log("Mostro por causa do caminho ate inimigos");
            return true;
        }

        bool hasTheRightAmountOfEnemies = RoomMatrix.EnemiesPositions.Count == GeneticAlgorithmConstants.Room.Enemies.Length;
        if (!hasTheRightAmountOfEnemies)
        {
            //Debug.Log("Mostro por causa da quantidade de inimigos");
            return true;
        }

        bool hasTheRightAmountOfObstacles = RoomMatrix.ObstaclesPositions.Count == GeneticAlgorithmConstants.Room.Obstacles.Length;
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
            double normalizedValue = Utils.MinMaxNormalization(vars[i], bounds[i].min, bounds[i].max);
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
