using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomIndividual
{
    RoomContents[,] roomValues;
    int value;
    bool itWasModified = true;

    HashSet<Position> enemiesPositions;
    HashSet<Position> obstaclesPositions;

    public RoomContents[,] RoomValues { get => roomValues; set => roomValues = value; }
    public int Value { get => value; set => this.value = value; }
    public bool ItWasModified { get => itWasModified; set => itWasModified = value; }

    // TODO: refactor VVV
    public HashSet<Position> EnemiesPositions { get => enemiesPositions; set => enemiesPositions = value; }
    public HashSet<Position> ObstaclesPositions { get => obstaclesPositions; set => obstaclesPositions = value; }

    public RoomIndividual(bool generateRandomly = true)
    {
        Value = default;
        RoomValues = (RoomContents[,])GeneticAlgorithmConstants.Room.Values.Clone();
        EnemiesPositions = new();
        ObstaclesPositions = new();

        if (generateRandomly)
        {
            GenerateRoomRandomly();
        }
    }

    public RoomIndividual(RoomIndividual individual)
    {
        Value = individual.Value;
        RoomValues = individual.RoomValues;
        EnemiesPositions = individual.EnemiesPositions;
        ObstaclesPositions = individual.ObstaclesPositions;
    }

    public void PutEnemyInPosition(RoomContents enemy, Position position)
    {
        RoomValues[position.X, position.Y] = enemy;
        EnemiesPositions.Add(position);
    }

    public void PutObstacleInPosition(RoomContents obstacle, Position position)
    {
        RoomValues[position.X, position.Y] = obstacle;
        ObstaclesPositions.Add(position);
    }

    public void RemoveFromPosition(HashSet<Position> positions, Position position)
    {
        positions.Remove(position);
    }

    void GenerateRoomRandomly()
    {
        List<Position> avaliablePositions = new(GeneticAlgorithmConstants.Room.changeablesPositions);
        int qntObjects = GeneticAlgorithmConstants.Room.Enemies.Length + GeneticAlgorithmConstants.Room.Obstacles.Length;
        Position[] chosenPositions = avaliablePositions.SelectRandomDistinctElements(qntObjects);

        int count = 0;
        foreach (RoomContents enemy in GeneticAlgorithmConstants.Room.Enemies)
        {
            PutEnemyInPosition(enemy, chosenPositions[count]);
            count++;
        }

        foreach (RoomContents obstacle in GeneticAlgorithmConstants.Room.Obstacles)
        {
            PutObstacleInPosition(obstacle, chosenPositions[count]);
            count++;
        }
    }

    void ChangePlaceOf(HashSet<Position> positionsOf, Position position1)
    {
        int idx2 = Random.Range(0, GeneticAlgorithmConstants.Room.changeablesPositions.Count);
        Position position2 = GeneticAlgorithmConstants.Room.changeablesPositions[idx2];

        RoomContents content1 = RoomValues[position1.X, position1.Y];
        RoomContents content2 = RoomValues[position2.X, position2.Y];

        // Colocar na posicao 1 o conteudo da posicao 2
        if (GeneticAlgorithmConstants.Room.Enemies.Contains(RoomValues[position2.X, position2.Y]))
        {
            RemoveFromPosition(EnemiesPositions, position2);
            PutEnemyInPosition(content2, position1);
        }
        else if (GeneticAlgorithmConstants.Room.Obstacles.Contains(RoomValues[position2.X, position2.Y]))
        {
            RemoveFromPosition(ObstaclesPositions, position2);
            PutObstacleInPosition(content2, position1);
        }
        else
        {
            RoomValues[position1.X, position1.Y] = content2;
        }

        // Colocar na posicao 2 o conteudo da posicao 1
        RemoveFromPosition(positionsOf, position1);
        if (positionsOf.Equals(EnemiesPositions))
        {
            PutEnemyInPosition(content1, position2);
        }
        else if (positionsOf.Equals(ObstaclesPositions))
        {
            PutObstacleInPosition(content1, position2);
        }
    }

    public void Mutate()
    {
        // escolher inimigos ou obstaculos para mudar
        if (Random.value < 0.5f)
        {
            Position[] positionsToChange = EnemiesPositions.SelectRandomDistinctElements(Random.Range(0, EnemiesPositions.Count));
            foreach (Position position in positionsToChange)
            {
                ChangePlaceOf(EnemiesPositions, position);
            }
        }
        else
        {
            Position[] positionsToChange = ObstaclesPositions.SelectRandomDistinctElements(Random.Range(0, ObstaclesPositions.Count));
            foreach (Position position in positionsToChange)
            {
                ChangePlaceOf(ObstaclesPositions, position);
            }
        }
    }

    bool IsMonstrous()
    {
        if (!PathFinder.IsAPathBetweenDoors(RoomValues, GeneticAlgorithmConstants.Room.doorsPositions))
        {
            return true;
        }

        if (!PathFinder.IsAPathBetweenDoorAndEnemies(RoomValues, GeneticAlgorithmConstants.Room.doorsPositions, EnemiesPositions))
        {
            //Debug.Log("Mostro por causa do caminho ate inimigos");
            return true;
        }

        bool hasTheRightAmountOfEnemies = EnemiesPositions.Count == GeneticAlgorithmConstants.Room.Enemies.Length;
        if (!hasTheRightAmountOfEnemies)
        {
            //Debug.Log("Mostro por causa da quantidade de inimigos");
            return true;
        }

        bool hasTheRightAmountOfObstacles = ObstaclesPositions.Count == GeneticAlgorithmConstants.Room.Obstacles.Length;
        if (!hasTheRightAmountOfObstacles)
        {
            //Debug.Log("Mostro por causa da quantidade de obstaculos");
            return true;
        }

        /*
        if (!RoomOperations.HasTheRightObjects(roomMatrix, typeof(Enemies), enemiesPositions, sala.enemies.Cast<object>().ToList()))
        {
            //Debug.Log("Mostro por causa dos inimigos");
            return true;
        }

        if (!RoomOperations.HasTheRightObjects(roomMatrix, typeof(Obstacles), obstaclesPositions, sala.obstacles.Cast<object>().ToList()))
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
