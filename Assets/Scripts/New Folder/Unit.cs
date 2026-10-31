using System;
using System.Collections.Generic;

public class Unit
{
    public enum GenderType { Female, Male }
    public readonly GenderType Gender;
    public readonly string Species; 
    public string Nickname { get; private set; }
    public UnitStats Stats { get; private set; }

    private readonly Dictionary<Type, Gene> _genePool = new();

    public class UnitStats
    {
        public int HP;
        public int Attack;
        public int Defence;
    }

    public Unit(string species, GenderType gender)
    {
        Species = species;
        Gender = gender;

        Stats = new UnitStats();
    }
    public void ChangeNickname(string nickname) => Nickname = nickname;

    public void InjectGene(Gene gene)
    {
        if (gene == null)
            throw new ArgumentNullException(nameof(gene));

        _genePool.TryAdd(gene.GetType(), gene);
    }

    public void OverrideGene(Gene gene)
    {
        if (gene == null)
            throw new ArgumentNullException(nameof(gene));

        _genePool[gene.GetType()] = gene;
    }

    public GeneType GetGene<GeneType>() where GeneType : Gene
    {
        if (_genePool.TryGetValue(typeof(GeneType), out var gene) && gene is GeneType cast)
            return cast;

        throw new InvalidOperationException($"Gene of type {typeof(GeneType).Name} not found or invalid.");
    }

    public void SetStats(UnitStats stats)
    {
        Stats = stats;
    }
}