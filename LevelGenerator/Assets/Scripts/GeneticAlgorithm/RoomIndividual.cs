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
}
