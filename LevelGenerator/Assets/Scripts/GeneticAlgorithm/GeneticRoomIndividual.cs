using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneticRoomIndividual
{
    RoomContents[,] roomValues;
    int? value;
    bool itWasModified = true;

    HashSet<Position> enemiesPositions;
    HashSet<Position> obstaclesPositions;

    public RoomContents[,] RoomValues { get => roomValues; set => roomValues = value; }
    public int? Value { get => value; set => this.value = value; }
    public bool ItWasModified { get => itWasModified; set => itWasModified = value; }
    public HashSet<Position> EnemiesPositions { get => enemiesPositions; set => enemiesPositions = value; }
    public HashSet<Position> ObstaclesPositions { get => obstaclesPositions; set => obstaclesPositions = value; }

    public GeneticRoomIndividual(Sala sala, bool generateRandomly = true)
    {
        Value = null;
        RoomValues = (RoomContents[,])sala.Values.Clone();
        EnemiesPositions = new();
        ObstaclesPositions = new();

        if (generateRandomly)
        {
            GenerateRoomRandomly();
        }
    }

    public GeneticRoomIndividual(GeneticRoomIndividual individual)
    {
        Value = individual.Value;
        RoomValues = individual.RoomValues;
        EnemiesPositions = individual.EnemiesPositions;
        ObstaclesPositions = individual.ObstaclesPositions;
    }

    public void PutEnemyInPosition(RoomContents enemy, Position position)
    {
        RoomValues[position.Row, position.Column] = enemy;
        EnemiesPositions.Add(position);
    }

    public void PutObstacleInPosition(RoomContents obstacle, Position position)
    {
        RoomValues[position.Row, position.Column] = obstacle;
        ObstaclesPositions.Add(position);
    }

    public void RemoveFromPosition(HashSet<Position> positions, Position position)
    {
        positions.Remove(position);
    }

    void GenerateRoomRandomly()
    {
        List<Position> avaliablePositions = new(GeneticAlgorithmConstants.Sala.changeablesPositions);
        int qntObjects = GeneticAlgorithmConstants.Sala.Enemies.Length + GeneticAlgorithmConstants.Sala.Obstacles.Length;
        Position[] chosenPositions = avaliablePositions.SelectRandomDistinctElements(qntObjects);

        int count = 0;
        foreach (RoomContents enemy in GeneticAlgorithmConstants.Sala.Enemies)
        {
            PutEnemyInPosition(enemy, chosenPositions[count]);
            count++;
        }

        foreach (RoomContents obstacle in GeneticAlgorithmConstants.Sala.Obstacles)
        {
            PutObstacleInPosition(obstacle, chosenPositions[count]);
            count++;
        }
    }

    void ChangePlaceOf(HashSet<Position> positionsOf)
    {
        // escolher
        int idx1 = Random.Range(0, positionsOf.Count);
        Position position1 = positionsOf.ElementAt(idx1);

        int idx2 = Random.Range(0, GeneticAlgorithmConstants.Sala.changeablesPositions.Count);
        Position position2 = GeneticAlgorithmConstants.Sala.changeablesPositions[idx2];

        RoomContents content1 = RoomValues[position1.Row, position1.Column];
        RoomContents content2 = RoomValues[position2.Row, position2.Column];

        // Colocar na posicao 1 o conteudo da posicao 2
        if (GeneticAlgorithmConstants.Sala.Enemies.Contains(RoomValues[position2.Row, position2.Column]))
        {
            RemoveFromPosition(EnemiesPositions, position2);
            PutEnemyInPosition(content2, position1);
        }
        else if (GeneticAlgorithmConstants.Sala.Obstacles.Contains(RoomValues[position2.Row, position2.Column]))
        {
            RemoveFromPosition(ObstaclesPositions, position2);
            PutObstacleInPosition(content2, position1);
        }
        else
        {
            RoomValues[position1.Row, position1.Column] = content2;
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
        // escolher um inimigo ou um obstaculo para mudar
        if (Random.value < 0.5f)
        {
            ChangePlaceOf(EnemiesPositions);
        }
        else
        {
            ChangePlaceOf(ObstaclesPositions);
        }
    }

    bool IsMonstrous()
    {
        int qntCaminhosEntrePortas = PathFinder.CountPathsBetweenDoors(RoomValues, GeneticAlgorithmConstants.Sala.doorsPositions);
        if (qntCaminhosEntrePortas == int.MinValue)
        {
            //Debug.Log("Mostro por causa do caminho entre portas");
            return true;
        }

        bool isPath = PathFinder.IsAPathBetweenDoorAndEnemies(RoomValues, GeneticAlgorithmConstants.Sala.doorsPositions, EnemiesPositions);
        if (!isPath)
        {
            //Debug.Log("Mostro por causa do caminho ate inimigos");
            return true;
        }

        bool hasTheRightAmountOfEnemies = EnemiesPositions.Count == GeneticAlgorithmConstants.Sala.Enemies.Length;
        if (!hasTheRightAmountOfEnemies)
        {
            //Debug.Log("Mostro por causa da quantidade de inimigos");
            return true;
        }

        bool hasTheRightAmountOfObstacles = ObstaclesPositions.Count == GeneticAlgorithmConstants.Sala.Obstacles.Length;
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

    public void Evaluate()
    {
        if (IsMonstrous())
        {
            Value = int.MinValue;
            return;
        }

        List<int> groups = GroupCounter.CountGroups(RoomValues, EnemiesPositions);
        double media = groups.Average();

        //Debug.Log("Total de grupos de Enemies na matriz: " + groups.Count);
        //Debug.Log("Média do tamanho dos grupos: " + media);

        //int qntInimigosProximosDeObstaculos = Utils.CountEnemiesNextToObstacles(roomMatrix);
        Value = - groups.Count - (int)media; // + qntInimigosProximosDeObstaculos;
    }
}
