using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneticRoomIndividual
{
    public RoomContents[,] roomMatrix;
    public int? value;
    public bool itWasModified = true;

    public HashSet<Position> enemiesPositions;
    public HashSet<Position> obstaclesPositions;

    // TODO: mudar a sala daq, pq n precisa q todos individuos tenham ela, pode ser uma so pro algoritmo genetico
    readonly Sala sala;

    public GeneticRoomIndividual(Sala sala, bool generateRandomly = true)
    {
        this.sala = sala;
        value = null;
        roomMatrix = (RoomContents[,])sala.matriz.Clone();
        enemiesPositions = new();
        obstaclesPositions = new();

        if (generateRandomly)
        {
            GenerateRoomRandomly();
        }
    }

    public GeneticRoomIndividual(GeneticRoomIndividual individual)
    {
        sala = individual.sala;
        value = individual.value;
        roomMatrix = individual.roomMatrix;
        enemiesPositions = individual.enemiesPositions;
        obstaclesPositions = individual.obstaclesPositions;
    }

    public void PutEnemyInPosition(RoomContents enemy, Position position)
    {
        roomMatrix[position.Row, position.Column] = enemy;
        enemiesPositions.Add(position);
    }

    public void PutObstacleInPosition(RoomContents obstacle, Position position)
    {
        roomMatrix[position.Row, position.Column] = obstacle;
        obstaclesPositions.Add(position);
    }

    public void RemoveFromPosition(HashSet<Position> positions, Position position)
    {
        positions.Remove(position);
    }

    void GenerateRoomRandomly()
    {
        List<Position> avaliablePositions = new(sala.changeablesPositions);
        Position[] chosenPositions = avaliablePositions.SelectRandomDistinctElements(sala.Enemies.Length + sala.Obstacles.Length);

        int count = 0;
        foreach (RoomContents enemy in sala.Enemies)
        {
            PutEnemyInPosition(enemy, chosenPositions[count]);
            count++;
        }

        foreach (RoomContents obstacle in sala.Obstacles)
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

        int idx2 = Random.Range(0, sala.changeablesPositions.Count);
        Position position2 = sala.changeablesPositions[idx2];

        // switchPositions

        RoomContents wasInPosition1 = roomMatrix[position1.Row, position1.Column];
        RoomContents wasInPosition2 = roomMatrix[position2.Row, position2.Column];

        RemoveFromPosition(positionsOf, position1);
        
        if (sala.Enemies.Contains(roomMatrix[position2.Row, position2.Column])) // se a posicao 2 for um inimigo
        {
            RemoveFromPosition(enemiesPositions, position2);
            PutEnemyInPosition(wasInPosition2, position1); // colocar na posicao 1 o que tinha na 2
        }
        else if (sala.Obstacles.Contains(roomMatrix[position2.Row, position2.Column])) // se a posicao 2 for um obstaculo
        {
            RemoveFromPosition(obstaclesPositions, position2);
            PutObstacleInPosition(wasInPosition2, position1); // colocar na posicao 1 o que tinha na 2
        }
        else
        {
            roomMatrix[position1.Row, position1.Column] = wasInPosition2;
        }

        // colocar na posicao 2 o que tinha na 1
        if (positionsOf.Equals(enemiesPositions))
        {
            PutEnemyInPosition(wasInPosition1, position2);
        }
        else if (positionsOf.Equals(obstaclesPositions))
        {
            PutObstacleInPosition(wasInPosition1, position2);
        }
    }

    public void Mutate()
    {
        // escolher um inimigo ou um obstaculo para mudar
        if (Random.value < 0.5f)
        {
            ChangePlaceOf(enemiesPositions);
        }
        else
        {
            ChangePlaceOf(obstaclesPositions);
        }
    }

    bool IsMonstrousIndividual()
    {
        int qntCaminhosEntrePortas = PathFinder.CountPathsBetweenDoors(roomMatrix, sala.doorsPositions);
        if (qntCaminhosEntrePortas == int.MinValue)
        {
            //Debug.Log("Mostro por causa do caminho entre portas");
            return true;
        }

        bool isPath = PathFinder.IsAPathBetweenDoorAndEnemies(roomMatrix, sala.doorsPositions, enemiesPositions);
        if (!isPath)
        {
            //Debug.Log("Mostro por causa do caminho ate inimigos");
            return true;
        }

        bool hasTheRightAmountOfEnemies = enemiesPositions.Count == sala.Enemies.Length;
        if (!hasTheRightAmountOfEnemies)
        {
            //Debug.Log("Mostro por causa da quantidade de inimigos");
            return true;
        }

        bool hasTheRightAmountOfObstacles = obstaclesPositions.Count == sala.Obstacles.Length;
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
        if (IsMonstrousIndividual())
        {
            value = int.MinValue;
            return;
        }

        List<int> groups = GroupCounter.CountGroups(roomMatrix, enemiesPositions);
        double media = groups.Average();

        //Debug.Log("Total de grupos de Enemies na matriz: " + groups.Count);
        //Debug.Log("Média do tamanho dos grupos: " + media);

        //int qntInimigosProximosDeObstaculos = Utils.CountEnemiesNextToObstacles(roomMatrix);
        value = - groups.Count - (int)media; // + qntInimigosProximosDeObstaculos;
    }
}
