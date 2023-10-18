using System;
using System.Collections.Generic;

/// <summary>
/// Represents an individual in a genetic algorithm, specifically designed for room layout optimization.
/// </summary>
public class RoomIndividual
{
    RoomMatrix roomMatrix;
    int value;
    bool itWasModified = true;

    public int Value { get => value; set => this.value = value; }
    public bool ItWasModified { get => itWasModified; set => itWasModified = value; }
    public RoomMatrix RoomMatrix { get => roomMatrix; set => roomMatrix = value; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoomIndividual"/> class, optionally generating it randomly.
    /// </summary>
    /// <param name="generateRandomly">Determines whether to generate the room layout randomly.</param>
    public RoomIndividual(bool generateRandomly = true)
    {
        Value = default;
        RoomMatrix = new RoomMatrix((RoomContents[,])GeneticAlgorithmConstants.ROOM.Values.Clone());

        if (generateRandomly)
        {
            GenerateRoomRandomly();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoomIndividual"/> class as a copy of another individual.
    /// </summary>
    /// <param name="individual">The individual to copy.</param>
    public RoomIndividual(RoomIndividual individual)
    {
        Value = individual.Value;
        RoomMatrix = individual.RoomMatrix;
    }

    /// <summary>
    /// Generates a random room layout by placing enemies and obstacles in available positions.
    /// </summary>
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
