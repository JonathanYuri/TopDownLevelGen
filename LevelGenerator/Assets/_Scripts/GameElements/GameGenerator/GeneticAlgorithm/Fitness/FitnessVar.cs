using RoomGeneticAlgorithm;
using System;
using UnityEngine;

public class FitnessVar
{
    public Range<int> CurrentBound { get; set; } = new(int.MaxValue, int.MinValue);
    public Range<float> IdealRange { get; private set; }
    public float Importance { get; private set; }
    public float Ideal { get; private set; }
    public bool IncreaseWithDifficulty { get; private set; }
    public Func<RoomIndividual, float> FitnessVarValue { get; private set; }

    public FitnessVar(Range<float> range, float importance, bool increaseWithDifficulty, Func<RoomIndividual, float> FitnessVarValue)
    {
        IdealRange = range;
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

    public bool UpdateExistingBound(int value)
    {
        if (value > CurrentBound.Max)
        {
            //Debug.LogWarning("UPDATE MAX BOUND");
            CurrentBound.Max = value;
            return true;
        }
        if (value < CurrentBound.Min)
        {
            //Debug.LogWarning("UPDATE MIN BOUND");
            CurrentBound.Min = value;
            return true;
        }
        return false;
    }
}
