using RoomGeneticAlgorithm;
using System;
using UnityEngine;

public class FitnessVar
{
    public Range<int> CurrentBound { get; set; } = new(int.MaxValue, int.MinValue);
    public Range<float> Range { get; private set; }
    public float Importance { get; private set; }
    public float Ideal { get; private set; }
    public bool IncreaseWithDifficulty { get; private set; }
    public Func<RoomIndividual, float> FitnessVarValue { get; private set; }

    public FitnessVar(Range<float> range, float importance, bool increaseWithDifficulty, Func<RoomIndividual, float> FitnessVarValue)
    {
        Range = range;
        Importance = importance;
        IncreaseWithDifficulty = increaseWithDifficulty;
        this.FitnessVarValue = FitnessVarValue;
    }

    public void SetIdealValue(float difficulty)
    {
        if (IncreaseWithDifficulty)
        {
            Ideal = Mathf.Lerp(Range.Min, Range.Max, difficulty);
        }
        else
        {
            Ideal = Mathf.Lerp(Range.Max, Range.Min, difficulty);
        }
    }

    public void UpdateExistingBound(Range<int> varBound, ref bool boundsModified)
    {
        if (varBound.Max > CurrentBound.Max)
        {
            //Debug.LogWarning("UPDATE MAX BOUND");
            CurrentBound.Max = varBound.Max;
            boundsModified = true;
        }
        if (varBound.Min < CurrentBound.Min)
        {
            //Debug.LogWarning("UPDATE MIN BOUND");
            CurrentBound.Min = varBound.Min;
            boundsModified = true;
        }
    }
}
