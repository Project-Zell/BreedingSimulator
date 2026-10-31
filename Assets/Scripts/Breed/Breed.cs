using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Unit;

public class Breed
{
    public Unit GetOffspring(BreedContainer container)
    {
        if (CannotResultOffspring(container))
            throw new InvalidOperationException("These parents cannot produce offspring.");

        var offspringSpecies = GetSpecies(container);
        var offspringData = offspringSpecies == container.FatherUnit.Species ? container.FatherData : container.MotherData;

        var offspringUnit = new Unit(species: offspringSpecies, gender: GetOffspringGender(offspringData));
        var offspringContext = (offspringUnit, offspringData);

        RefreshGeneMorphers();

        foreach (var morpher in GeneMorphers.Values)
        {
            var gene = morpher(container, offspringContext);

            if (gene != null)
            {
                offspringUnit.InjectGene(gene);
            } 
        }

        offspringUnit.SetStats(stats: GetStats(container, offspringContext));

        return offspringUnit;
    }

    private string GetSpecies(BreedContainer container)
    {
        var fatherData = container.FatherData;
        var motherData = container.MotherData;

        var fatherUnit = container.FatherUnit;
        var motherUnit = container.MotherUnit;

        if (fatherData.ForceTheSameSpecies || motherData.SpeciesNonInheritable)
            return fatherUnit.Species;

        if (motherData.ForceTheSameSpecies || fatherData.SpeciesNonInheritable)
            return motherUnit.Species;

        if (fatherUnit.Species == motherUnit.Species)
            return fatherUnit.Species;

        return RandomFloat() > 0.6f ? fatherUnit.Species : motherUnit.Species;
    }

    private GenderType GetOffspringGender(SpeciesData offspringSpeciesData)
    {
        if (offspringSpeciesData.GenderDistribution == SpeciesData.GenderDisrtibution.OnlyMale)
            return GenderType.Male;

        if (offspringSpeciesData.GenderDistribution == SpeciesData.GenderDisrtibution.OnlyFemale)
            return GenderType.Female;

        var roll = RandomFloat();
        return roll < 0.55f ? GenderType.Female : GenderType.Male;
    }

    private bool CannotResultOffspring(BreedContainer container)
    {
        var isParentsNotBreedable = container.FatherData.SpeciesNonInheritable && container.MotherData.SpeciesNonInheritable;        
        return isParentsNotBreedable || container.FatherUnit.Gender == container.MotherUnit.Gender;
    }

    private Gene GetMorphedGene<GeneType>(BreedContainer container, (Unit unit, SpeciesData data) offspring) where GeneType : Gene
    {
        if (offspring.data.HasGene<GeneType>() is false)
            return null;

        var fatherHasGene = container.FatherData.HasGene<GeneType>();
        var motherHasGene = container.MotherData.HasGene<GeneType>();

        if(motherHasGene is false && motherHasGene is false)
            return offspring.data.GetRandomGene<GeneType>();

        if (fatherHasGene && motherHasGene)
            return GetGeneFromBothParents<GeneType>(container, offspring);

        var activeParent = container.FatherData.HasGene<GeneType>() ? container.FatherUnit : container.MotherUnit;
        var parentInfo = container.FatherData.HasGene<GeneType>() ? container.FatherData : container.MotherData;

        var activeGene = activeParent.GetGene<GeneType>();
        var mutationGene = offspring.data.GetRandomGene<GeneType>();

        var isParentPure = parentInfo.IsGenePure(activeGene);

        int parentGenes = 2 + (isParentPure ? 1 : 0);
        int mutationGenes = 1 + (isParentPure ? 0 : 1);

        var totalWeight = parentGenes + mutationGenes;
        var roll = UnityEngine.Random.Range(0, totalWeight);

        if (roll < parentGenes)
            return activeGene;
        return mutationGene;
    }

    private Gene GetGeneFromBothParents<GeneType>(BreedContainer container, (Unit unit, SpeciesData data) offspring) where GeneType : Gene
    {
        var fatherUit = container.FatherUnit;
        var motherUnit = container.MotherUnit;
        var offspringUnit = offspring.unit;

        var fatherGene = container.FatherUnit.GetGene<GeneType>();
        var motherGene = container.MotherUnit.GetGene<GeneType>();
        var mutationGene = offspring.data.GetRandomGene<GeneType>();

        var isFatherPure = container.FatherData.IsGenePure(fatherGene);
        var isMotherPure = container.MotherData.IsGenePure(motherGene);
        var isParentsPure = isFatherPure && isMotherPure;

        var isFatherAndChildSameSpecies = fatherUit.Species == offspringUnit.Species;
        var isMotherAndChildSameSpecies = motherUnit.Species == offspringUnit.Species;

        var fatherGenes = 2 + (isFatherPure ? 1 : 0) + (isFatherAndChildSameSpecies ? 1 : 0) + (isParentsPure ? 0 : 1);
        var motherGenes = 2 + (isMotherPure ? 1 : 0) + (isMotherAndChildSameSpecies ? 1 : 0) + (isParentsPure ? 0 : 1);
        var mutationGenes = 1 + (isMotherAndChildSameSpecies ? 0 : 1) + (isFatherAndChildSameSpecies ? 0 : 1) + (isFatherPure is false && isMotherPure is false ? 1 : 0);

        var totalWeight = fatherGenes + motherGenes + mutationGenes;
        var roll = UnityEngine.Random.Range(0, totalWeight);

        if (roll < fatherGenes)
            return fatherGene;

        roll -= fatherGenes;
        if (roll < motherGenes)
            return motherGene;

        return mutationGene;
    }

    private UnitStats GetStats(BreedContainer container, (Unit unit, SpeciesData data) offspring)
    {
        var fatherStats = container.FatherUnit.Stats;
        var motherStats = container.MotherUnit.Stats;
        var offspringData = offspring.data;

        return new UnitStats
        {
            HP = StatInherit(fatherStats.HP, motherStats.HP, offspringData.StatRangeHP),
            Attack = StatInherit(fatherStats.Attack, motherStats.Attack, offspringData.StatRangeAttack),
            Defence = StatInherit(fatherStats.Defence, motherStats.Defence, offspringData.StatRangeDefence)
        };
    }

    private int StatInherit(int fatherStat, int motherStat, Range offspringSpecieBaseStatRange)
    {
        var statValueToWeight = new Dictionary<int, int>();

        var upperLimit = Math.Max(offspringSpecieBaseStatRange.Max, Math.Max(fatherStat, motherStat));
        var lowerLimit = Math.Min(offspringSpecieBaseStatRange.Min, Math.Min(fatherStat, motherStat));

        for (int statValue = lowerLimit; statValue <= upperLimit; statValue++)
        {
            var parentStatRange = new Range(fatherStat, motherStat);

            var baseWeight = offspringSpecieBaseStatRange.InRange(statValue) ? 4 : 1;
            var parentRangeWeight = parentStatRange.InRange(statValue) ? 2 : 0;

            var fatherWeight = fatherStat == statValue ? 2 : 0;
            var motherWeight = motherStat == statValue ? 2 : 0;

            var statWeight = baseWeight + parentRangeWeight + fatherWeight + motherWeight;
            statValueToWeight.Add(statValue, statWeight);
        }

        var totalWeight = statValueToWeight.Values.Sum();
        var random = UnityEngine.Random.Range(0, totalWeight);

        var sum = 0;
        foreach (var statValueToWeightPair in statValueToWeight.OrderBy(statValue => statValue.Key))
        {
            sum += statValueToWeightPair.Value;
            if (random < sum)
            {
                return statValueToWeightPair.Key;
            }
        }
        return statValueToWeight.Keys.Last();
    }
    private float RandomFloat() => UnityEngine.Random.Range(0, 1f);

    private delegate Gene GeneMorpher(BreedContainer container, (Unit unit, SpeciesData data) offspring);
    private readonly Dictionary<Type, GeneMorpher> GeneMorphers = new();

    private void RefreshGeneMorphers()
    {
        var method = typeof(Breed).GetMethod(nameof(GetMorphedGene), BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (Type geneType in Gene.AllTypes)
        {
            if (GeneMorphers.ContainsKey(geneType))
                continue;

            var genericMethod = method.MakeGenericMethod(geneType);
            var morpher = (GeneMorpher)Delegate.CreateDelegate(typeof(GeneMorpher), this, genericMethod);

            GeneMorphers.Add(geneType, morpher);
        }
    }
}
