using UnityEngine;

public readonly struct Range
{
    public readonly int Min { get; }
    public readonly int Max { get; }

    public Range(int min, int max)
    {
        Min = min < max ? min : max;
        Max = max > min ? max : min;
    }

    public bool InRange(int value) => value >= Min && value <= Max;
    public int Clamp(int value) => Mathf.Clamp(value, Min, Max);

    public int GetRandom() => Random.Range(Min, Max + 1);
}