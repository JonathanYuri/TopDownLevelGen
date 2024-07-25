using RoomGeneticAlgorithm;
using System;
using UnityEngine;

public class FitnessVar
{
    public string Name { get; }

    public Range<float> IdealRange { get; }
    public float MaxDeviation { get; }
    public float Importance { get; }
    public float Ideal { get; private set; }
    public bool IncreaseWithDifficulty { get; private set; }
    public Func<RoomIndividual, float> FitnessVarValue { get; private set; }

    public FitnessVar(string name, Range<float> range, float maxDeviation, float importance,
        bool increaseWithDifficulty, Func<RoomIndividual, float> FitnessVarValue)
    {
        Name = name;
        IdealRange = range;
        MaxDeviation = maxDeviation;
        Importance = importance;
        IncreaseWithDifficulty = increaseWithDifficulty;
        this.FitnessVarValue = FitnessVarValue;
    }

    public void SetIdealValue(float difficulty)
    {
        if (IncreaseWithDifficulty)
        {
            Ideal = Mathf.Lerp(IdealRange.Min, IdealRange.Max, difficulty);
        }
        else
        {
            Ideal = Mathf.Lerp(IdealRange.Max, IdealRange.Min, difficulty);
        }
    }

    public float Normalize(float distance)
    {
        if (distance > MaxDeviation)
        {
            return 0f;
        }
        else
        {
            // se distance == 0 -> normalizedDistance = 100f
            // se distance == MaxDeviation -> normalizedDistance = 0
            float normalizedDistance = Mathf.Lerp(100f, 0f, distance / MaxDeviation);
            return normalizedDistance;
        }
    }
}
