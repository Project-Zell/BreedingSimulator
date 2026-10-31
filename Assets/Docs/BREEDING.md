# Breeding and Inheritance

## Entry point

The breeding operation is:

```csharp
Unit offspring = breed.GetOffspring(breedContainer);
```

`BreedContainer` receives two tuples, each containing a `Unit` and its corresponding `SpeciesData`:

```csharp
new BreedContainer(
    (fatherUnit, fatherSpeciesData),
    (motherUnit, motherSpeciesData));
```

The container exposes four readonly references:

- `FatherUnit`
- `MotherUnit`
- `FatherData`
- `MotherData`

The constructor parameter order is father first, mother second.

## Breeding eligibility

`Breed.CannotResultOffspring` currently prevents breeding when either condition is true:

1. Both parent species data objects have `SpeciesNonInheritable == true`.
2. Both parent units have the same gender.

If breeding is rejected, `GetOffspring` throws `InvalidOperationException`.

This is the exact behavior in the supplied implementation. It does not independently check age, health, fertility, relationship, or any other eligibility conditions.

## Offspring species selection

`GetSpecies` applies these rules in order:

1. If the father has `ForceTheSameSpecies`, or the mother has `SpeciesNonInheritable`, choose the father's species.
2. Otherwise, if the mother has `ForceTheSameSpecies`, or the father has `SpeciesNonInheritable`, choose the mother's species.
3. Otherwise, if both parents have the same species identifier, use that species.
4. Otherwise, choose the father's species when a random float is greater than `0.6`; otherwise choose the mother's species.

For different-species parents in the final case, the implementation therefore gives the father a 40% chance and the mother a 60% chance, assuming `RandomFloat()` is uniformly distributed over `[0, 1)`.

## Offspring gender

The selected offspring species' `GenderDistribution` controls the result:

- `OnlyMale`: Male.
- `OnlyFemale`: Female.
- `Default`: Female for a random roll below `0.55`, otherwise Male.

This is a 55% Female / 45% Male split under a uniform random distribution.

## Gene inheritance

`Breed` dynamically creates a delegate for `GetMorphedGene<T>()` for each type in `Gene.AllTypes`. It then evaluates those gene types and injects any non-null result into the offspring.

The inheritance method first checks whether the offspring species has a pool for that gene type. If not, that gene type is omitted from the offspring.

### Neither parent has the gene

The intended behavior is to choose a random gene from the offspring species' pool. **There is a likely bug in the supplied condition:** the code currently tests `motherHasGene` twice:

```csharp
if (motherHasGene is false && motherHasGene is false)
```

The second check likely should be `fatherHasGene`. Until corrected, the no-parent-gene case may not follow the intended behavior.

### Only one parent has the gene

The method compares the active parent's gene with the offspring species' pool:

- If the parent gene is considered pure in the parent's species, the parent gene receives weight 3 and a random offspring-pool gene receives weight 1.
- If the parent gene is not pure, the parent gene receives weight 2 and a random offspring-pool gene receives weight 2.

A weighted random roll selects one of these candidates.

### Both parents have the gene

The method considers:

- whether the father's gene is pure in the father's species;
- whether the mother's gene is pure in the mother's species;
- whether the offspring species matches each parent's species.

It calculates separate integer weights for the father's gene, mother's gene, and a random gene from the offspring species' pool, then selects one candidate using a weighted random roll.

The exact weight formulas are implemented in `GetGeneFromBothParents` in `Breed.cs`. They are intentionally documented here as algorithmic behavior rather than as Mendelian inheritance: this is a custom weighted selection model.

## Stat inheritance

HP, Attack, and Defence are calculated independently through `StatInherit`.

For each stat, the method builds a candidate interval from:

- the minimum and maximum of the offspring species' configured stat range;
- the father's stat;
- the mother's stat.

Every integer value in that interval receives a weight composed of:

- base weight `4` if the value is inside the offspring species' stat range, otherwise `1`;
- parent-range weight `2` if the value lies between the two parent values, otherwise `0`;
- father match weight `2` if it equals the father's stat;
- mother match weight `2` if it equals the mother's stat.

A weighted random roll chooses the resulting stat value.

Because the parent range is constructed using `new Range(fatherStat, motherStat)`, it includes both parental values and all integer values between them.

## Randomness and testing

All random choices use `UnityEngine.Random`. To make breeding tests repeatable, set the Unity random seed before the operation and test multiple cases, including:

- same-species and cross-species parents;
- each reproduction mode;
- each gender distribution;
- both parents, one parent, or neither parent having a gene;
- pure and non-pure parental genes;
- stat values below, within, and above the offspring species' stat range.

## Limitations

The breeding algorithm assumes that parent units and species data are internally consistent. For example, when a species reports that it has a gene type, the corresponding parent unit is expected to contain a gene of that type. Missing gene assignments can cause `Unit.GetGene<T>()` to throw.
