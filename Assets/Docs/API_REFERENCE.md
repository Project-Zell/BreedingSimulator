# API Reference

This reference covers the public members visible in the supplied source archive.

## `Gene`

Abstract base class.

| Member | Signature / type | Description |
|---|---|---|
| `Value` | `byte` | Numeric gene value. |
| `Key` | `string` | Gene key. |
| `ReferenceColor` | `Color` | Parsed reference color. |
| `AllTypes` | `IReadOnlyCollection<Type>` | Registered gene types. |
| Constructor | `Gene(byte value, string key, string hexColor)` | Initializes a gene and validates its color. |
| `ToString` | `override string ToString()` | Returns `Key`. |
| `GetAll<T>` | `static IEnumerable<T>` | Enumerates public static fields declared on `T`. |
| `GetAllKeys<T>` | `static string[]` | Gets keys from those static fields. |
| `GetGeneByIndex<T>` | `static T` | Finds a gene with the specified `Value`. |
| `GetAllDerivedTypes` | `static IEnumerable<Type>` | Finds concrete subclasses in loaded assemblies. |

## `Unit`

Represents an individual.

| Member | Signature / type | Description |
|---|---|---|
| `GenderType` | enum | `Female`, `Male`. |
| `Gender` | `GenderType` | Immutable gender. |
| `Species` | `string` | Immutable species identifier. |
| `Nickname` | `string` | Private-set nickname. |
| `Stats` | `UnitStats` | Current stats. |
| Constructor | `Unit(string species, GenderType gender)` | Creates a unit with empty stats. |
| `ChangeNickname` | `void ChangeNickname(string nickname)` | Sets the nickname. |
| `InjectGene` | `void InjectGene(Gene gene)` | Adds a gene only if that type is not already present. |
| `OverrideGene` | `void OverrideGene(Gene gene)` | Adds or replaces a gene of the same concrete type. |
| `GetGene<T>` | `GeneType GetGene<GeneType>() where GeneType : Gene` | Gets the gene of the requested type. |
| `SetStats` | `void SetStats(UnitStats stats)` | Replaces the stats object. |

`UnitStats` has public integer fields `HP`, `Attack`, and `Defence`.

## `SpeciesData`

| Member | Description |
|---|---|
| `GenderDisrtibution` | Enum: `OnlyMale`, `OnlyFemale`, `Default`. |
| `GenderDistribution` | Current gender-distribution setting. |
| `Reprodution` | Enum: `Normal`, `Dominant`, `Passive`. |
| `ForceTheSameSpecies` | True when reproduction type is `Dominant`. |
| `SpeciesNonInheritable` | True when reproduction type is `Passive`. |
| `StatRangeHP` | HP range. |
| `StatRangeAttack` | Attack range. |
| `StatRangeDefence` | Defence range. |
| `IsGenePure<T>(T gene)` | Checks whether the gene is in the species pool. |
| `GetRandomGene<T>()` | Gets a random gene from the species pool. |
| `HasGene<T>()` | Checks whether a non-empty pool exists for the gene type. |

The constructor `SpeciesData(string json)` is present but empty in the supplied source.

## `BreedContainer`

Constructor:

```csharp
BreedContainer(
    (Unit unit, SpeciesData data) father,
    (Unit unit, SpeciesData data) mother)
```

Exposes `FatherUnit`, `MotherUnit`, `FatherData`, and `MotherData` as readonly fields.

## `Breed`

Public entry point:

```csharp
Unit GetOffspring(BreedContainer container)
```

Returns a newly created `Unit` or throws `InvalidOperationException` if the parents cannot breed under the current rules.

## `Range`

Readonly struct.

| Member | Description |
|---|---|
| `Min` | Lower endpoint. |
| `Max` | Upper endpoint. |
| `Range(int min, int max)` | Creates a normalized range. |
| `InRange(int value)` | Inclusive membership test. |
| `Clamp(int value)` | Clamps a value to the range. |
| `GetRandom()` | Returns a random integer in the inclusive range. |

## `SpeciesCollection`

Declares a constructor taking a JSON string. The constructor is currently empty, and no public lookup methods are present in the supplied source.

## `EyeColor`

Concrete `Gene` subclass. The supplied class currently has an uninitialized static `Genes` array and an empty `LoadGenes(string json)` method.
