# Genetics & Breeding System

An isolated, simplified prototype of the breeding system from **Daron Breeding Game**, implemented in Unity/C#. It is intended as a flexible experimentation framework rather than a complete reproduction of the original game's systems.

The central design goal is to give game designers room to adjust species rules, gene pools, and inheritance behavior, then quickly test different combinations and observe the resulting offspring. The system separates genetic data, individual units, species configuration, and breeding logic so that these parts can be changed and evaluated independently.

In the original game, genes and species were defined in JSON files, loaded at runtime, and parsed into the game's native components. This prototype preserves that data-driven direction: JSON acts as the editable source for gene and species definitions, while the runtime works with C# objects and collections.

> **Documentation scope:** This documentation describes the source files present in the supplied `Assets(4).zip` archive. The archive contains several implementation stubs and does not include the JSON data files or the `GeneCollection` implementation discussed separately. Consequently, this is documentation of the code currently present in the archive—not a claim that every planned feature is implemented.

## Design goals

- **Designer-facing flexibility:** make species configurations, available genes, and inheritance rules straightforward to modify and compare.
- **Data-driven definitions:** keep gene and species definitions in JSON rather than hard-coding every configuration into gameplay code.
- **Separation of concerns:** distinguish gene definitions, individual units, species rules, and the breeding procedure.
- **Rapid experimentation:** allow designers to create test species and vary inheritance parameters without rewriting the core breeding flow.
- **Runtime-native data:** parse external JSON definitions into C# objects and collections used by the game.

## Features represented in the source

- **Gene definitions:** a base `Gene` class provides a byte value, a key, and a reference color parsed from a hexadecimal color string.
- **Gene-type discovery:** concrete subclasses can be discovered through reflection; registered gene types are exposed through `Gene.AllTypes`.
- **Unit data:** a `Unit` stores its species identifier, gender, nickname, stats, and at most one gene instance per concrete gene type.
- **Species gene pools:** `SpeciesData` contains generic methods for checking whether a gene is available, checking whether a gene is pure, and selecting a random gene from a pool.
- **Breeding:** `Breed` selects an offspring species and gender, resolves inherited genes, applies mutation candidates, and calculates inherited HP, Attack, and Defence values using weighted random selection.
- **Numeric ranges:** the `Range` struct provides inclusive range checks, clamping, and random integer selection.

## Project layout

```text
Assets/
└── Scripts/
    ├── Breed/
    │   ├── Breed.cs
    │   └── BreedContainer.cs
    ├── Generic/
    │   └── Range.cs
    ├── Genes/
    │   ├── Gene.cs
    │   └── EyeColor.cs
    ├── New Folder/
    │   └── Unit.cs
    └── Species/
        ├── SpeciesCollection.cs
        └── SpeciesData.cs
```

The archive also contains a sample Unity scene, Input System action asset, and URP settings. They are not part of the genetics/breeding domain model described here.

## Main concepts

| Concept | Responsibility |
|---|---|
| `Gene` | Base class for a genetic trait value. |
| Concrete gene class (for example, `EyeColor`) | Defines a particular gene category and its available values. |
| `Unit` | Runtime individual with species, gender, stats, and selected genes. |
| `SpeciesData` | Species rules, stat ranges, reproduction behavior, and allowed gene pools. |
| `BreedContainer` | Bundles the two parent units with their corresponding species data. |
| `Breed` | Produces an offspring from a `BreedContainer`. |
| `Range` | Inclusive integer range utility. |
| `SpeciesCollection` | Intended registry for species data; loading is currently a stub in the supplied source. |

## How the current code is intended to be used

At a high level, a caller would:

1. Create or load gene definitions.
2. Create species definitions and populate each species' gene pools and stat ranges.
3. Create two `Unit` instances and assign their genes and stats.
4. Pair each unit with its `SpeciesData` in a `BreedContainer`.
5. Call `Breed.GetOffspring(container)` to create the child.

The supplied archive does not yet contain a complete end-to-end loader/setup path, so steps 1 and 2 require implementation before this workflow can run from external data.

## Important implementation gaps

The following members are placeholders in the supplied archive:

- `EyeColor.LoadGenes(string json)` has an empty body.
- `SpeciesCollection.SpeciesCollection(string json)` has an empty body.
- `SpeciesData.SpeciesData(string json)` has an empty body.
- `SpeciesData`'s gene-pool dictionary is private and is not populated by the code currently supplied.
- The archive does not contain the JSON definition files under `Assets/StreamingAssets`.
- The archive does not contain the `GeneCollection` class discussed in the accompanying conversation.

As a result, the data-loading pipeline and species initialization are not complete in this snapshot. In particular, `Breed.GetOffspring` depends on populated `SpeciesData` gene pools and valid parent gene assignments.

## Important code-review notes

Before treating this as production-ready, review these points:

1. **Parent gene check:** `Breed.GetMorphedGene` currently checks `motherHasGene` twice in the condition intended to detect when neither parent has the gene. The second check likely should be `fatherHasGene`.
2. **Gene registration:** `Gene.RegisteredGeneTypes` is populated by the base constructor and `Register<T>()`. Ensure concrete gene definitions are initialized before breeding so all expected types are registered.
3. **`EyeColor.Genes`:** this static array is declared but not initialized in the supplied file.
4. **Species initialization:** the species constructor and collection loader are stubs, so `HasGene`, `IsGenePure`, and `GetRandomGene` will not return useful results until `_genePools` is populated.
5. **Default ranges:** `new Range()` produces the default value of the struct (`Min = 0`, `Max = 0`), not a configured species stat range.
6. **Duplicate gene types:** `Unit.InjectGene` does not replace an existing gene of the same type; `OverrideGene` does.
7. **Namespace and folder consistency:** `Unit` is in a folder named `New Folder` but declares no namespace. Consider moving it into a domain folder and applying consistent namespaces.
8. **Spelling and naming:** consider correcting `GenderDisrtibution` to `GenderDistribution` and `Reprodution` to `Reproduction` before these names become part of a larger API.

## Requirements

- Unity project with the UnityEngine API available.
- C# language/runtime support for the syntax used by the project (including target-typed `new()` and pattern matching).
- Unity's `UnityEngine.Random` and `UnityEngine.ColorUtility` APIs.

## Documentation

- [Architecture and data flow](docs/ARCHITECTURE.md)
- [Genes and units](docs/GENES_AND_UNITS.md)
- [Species data and gene pools](docs/SPECIES.md)
- [Breeding and inheritance](docs/BREEDING.md)
- [API reference](docs/API_REFERENCE.md)
