using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Exception thrown when an issue occurs during reproduction in a genetic algorithm.
/// </summary>
public class ReproductionException : Exception
{
    public string EnemiesPositionsInFather { get; }
    public string EnemiesPositionsInMother { get; }
    public string EnemiesPositionsInChild { get; }

    public string ObstaclesPositionsInFather { get; }
    public string ObstaclesPositionsInMother { get; }
    public string ObstaclesPositionsInChild { get; }

    public ReproductionException(string message)
        : base(message) { }

    public ReproductionException(string message, Exception innerException)
        : base(message, innerException) { }

    public ReproductionException(
        string message,
        HashSet<Position> enemiesPositionsInFather,
        HashSet<Position> enemiesPositionsInMother,
        HashSet<Position> enemiesPositionsInChild,

        HashSet<Position> obstaclesPositionsInFather,
        HashSet<Position> obstaclesPositionsInMother,
        HashSet<Position> obstaclesPositionsInChild)
        : base(
            BuildErrorMessage(
                message,
                enemiesPositionsInFather,
                enemiesPositionsInMother,
                enemiesPositionsInChild,
                obstaclesPositionsInFather,
                obstaclesPositionsInMother,
                obstaclesPositionsInChild)
        )
    {

    }

    /// <summary>
    /// Transforms a collection of positions into a formatted string for error messaging.
    /// </summary>
    /// <param name="positions">The collection of positions to transform.</param>
    /// <param name="nameOfPositionsHashSet">The name of the position collection for identification.</param>
    /// <returns>A formatted string containing the positions in the specified collection.</returns>
    static string TransformPositionsInString(HashSet<Position> positions, string nameOfPositionsHashSet)
    {
        StringBuilder messageBuilder = new();
        messageBuilder.AppendLine($"Elementos em {nameOfPositionsHashSet}:");

        foreach (Position position in positions)
        {
            string formattedPosition = $"{position.X} x {position.Y}";
            messageBuilder.AppendLine(formattedPosition);
        }

        return messageBuilder.ToString();
    }

    static string BuildErrorMessage(
        string message,
        HashSet<Position> enemiesPositionsInFather,
        HashSet<Position> enemiesPositionsInMother,
        HashSet<Position> enemiesPositionsInChild,

        HashSet<Position> obstaclesPositionsInFather,
        HashSet<Position> obstaclesPositionsInMother,
        HashSet<Position> obstaclesPositionsInChild)
    {
        StringBuilder messageBuilder = new();
        messageBuilder.AppendLine(message);
        messageBuilder.AppendLine(TransformPositionsInString(enemiesPositionsInFather, "enemiesPositionsInFather"));
        messageBuilder.AppendLine(TransformPositionsInString(enemiesPositionsInMother, "enemiesPositionsInMother"));
        messageBuilder.AppendLine(TransformPositionsInString(enemiesPositionsInChild, "enemiesPositionsInChild"));
        messageBuilder.AppendLine(TransformPositionsInString(obstaclesPositionsInFather, "obstaclesPositionsInFather"));
        messageBuilder.AppendLine(TransformPositionsInString(obstaclesPositionsInMother, "obstaclesPositionsInMother"));
        messageBuilder.AppendLine(TransformPositionsInString(obstaclesPositionsInChild, "obstaclesPositionsInChild"));
        return messageBuilder.ToString();
    }
}
