using System;
using System.Collections.Generic;
using System.Linq;

public partial class SpeciesData
{
    public enum GenderDisrtibution { OnlyMale, OnlyFemale, Default }
    public GenderDisrtibution GenderDistribution = GenderDisrtibution.Default;

    public enum Reprodution { Normal, Dominant, Passive }
    private readonly Reprodution _reproductionType = Reprodution.Normal;

    private readonly Dictionary<Type, object> _genePools = new();

    public bool ForceTheSameSpecies => _reproductionType is Reprodution.Dominant;
    public bool SpeciesNonInheritable => _reproductionType is Reprodution.Passive;

    public readonly Range StatRangeHP = new();
    public readonly Range StatRangeAttack = new();
    public readonly Range StatRangeDefence = new();

    public SpeciesData(string json)
    {
        //
    }

    public bool IsGenePure<T>(T gene) where T : Gene
    {
        if (_genePools.TryGetValue(typeof(T), out var pool) && pool is T[] array)
            return array.Contains(gene);
        return false;
    }

    public T GetRandomGene<T>() where T : Gene
    {
        if (_genePools.TryGetValue(typeof(T), out var pool) && pool is T[] array && array.Length > 0)
            return array[UnityEngine.Random.Range(0, array.Length)];

        throw new InvalidOperationException(
            $"No available genes for type {typeof(T).Name}");
    }

    public bool HasGene<T>()
    {
        return _genePools.TryGetValue(typeof(T), out var pool) && pool is T[] array && array.Length > 0;
    }
}