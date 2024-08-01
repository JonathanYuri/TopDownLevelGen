using RoomGeneticAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FitnessVar
{
    public string Name { get; }

    int count = 0;
    double sumOfSquares = 0.0;
    public int MaxValue { get; private set; } = int.MinValue;
    public int MinValue { get; private set; } = int.MaxValue;
    public double Mean { get; private set; } = 0.0;
    public double StdDev
    {
        get { return count > 1 ? Math.Sqrt(sumOfSquares / (count - 1)) : 0.0; }
    }

    public Range<float> IdealRange { get; }
    public float MaxDeviation { get; }
    public float Importance { get; }
    public float Ideal { get; private set; }
    public Func<RoomIndividual, float> FitnessVarValue { get; private set; }

    public FitnessVar(string name, Range<float> range, float maxDeviation, float importance, Func<RoomIndividual, float> FitnessVarValue)
    {
        Name = name;
        IdealRange = range;
        MaxDeviation = maxDeviation;
        Importance = importance;
        this.FitnessVarValue = FitnessVarValue;
    }

    public void SetIdealValue(float difficulty)
    {
        Ideal = Mathf.Lerp(IdealRange.Min, IdealRange.Max, difficulty);
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

    public void AddValue(int value)
    {
        count++;
        double delta = value - Mean;
        Mean += delta / count;
        sumOfSquares += delta * (value - Mean);

        MaxValue = Mathf.Max(value, MaxValue);
        MinValue = Mathf.Min(value, MinValue);
    }
}
