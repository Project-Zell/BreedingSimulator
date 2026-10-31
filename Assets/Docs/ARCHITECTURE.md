# Architecture and Data Flow

## Design intent

This project is an isolated, simplified prototype of the breeding system from **Daron Breeding Game**. It is not intended to reproduce the entire original game. Its purpose is to make the breeding rules independently testable and flexible enough for game designers to experiment with different species, gene pools, and inheritance behaviors.

In the original game, JSON files defined genes and species. Those files were loaded and parsed into native game components. The prototype follows the same general data-driven pattern: author definitions as JSON, convert them into runtime C# data, and let the breeding logic operate on those runtime objects.

The important boundary is between **editable configuration** and **runtime behavior**. Designers should be able to change data and tune rules without needing to restructure the unit model or rewrite unrelated systems.

## Overview

The code separates an individual creature (`Unit`), species-level rules (`SpeciesData`), genetic values (`Gene` subclasses), and the breeding operation (`Breed`).

```text
                   ┌─────────────────────┐
                   │     SpeciesData     │
                   │ gene pools / rules  │
                   └──────────┬──────────┘
                              │
┌──────────────┐              │              ┌──────────────┐
│  Father Unit │──────────────┼──────────────│  Mother Unit │
└──────┬───────┘              │              └──────┬───────┘
       │                      ▼                     │
       └──────────────┌────────────────┐────────────┘
                      │ BreedContainer │
                      └───────┬────────┘
                              ▼
                      ┌───────────────┐
                      │     Breed     │
                      │ species, genes│
                      │ stats, gender │
                      └───────┬───────┘
                              ▼
                         Offspring Unit
```

`BreedContainer` stores both parent `Unit` references and the `SpeciesData` associated with each parent. `Breed` consumes this context and constructs a new `Unit`.

## Responsibilities

### Gene layer

`Gene` is the abstract base class for trait values. Each instance stores:

- `Value`: byte-sized value/index.
- `Key`: string identifier or display key.
- `ReferenceColor`: a Unity `Color` parsed from a hexadecimal string.

The base constructor validates the color string with `ColorUtility.TryParseHtmlString`. Invalid color input causes an `ArgumentException`.

The class also provides reflection helpers to enumerate static gene definitions and discover concrete subclasses.

### Unit layer

`Unit` represents one individual. It stores:

- immutable `Species` and `Gender`;
- mutable `Nickname` through `ChangeNickname`;
- `UnitStats` (`HP`, `Attack`, `Defence`);
- a private dictionary mapping concrete gene `Type` to a `Gene` instance.

A unit can hold no more than one gene instance per concrete gene class in `_genePool`.

### Species layer

`SpeciesData` is intended to hold species-level configuration:

- gender distribution;
- reproduction behavior;
- allowed gene pools;
- HP, Attack, and Defence ranges.

Its query methods operate on `_genePools`, a dictionary keyed by gene `Type`. The supplied source does not include the code that populates this dictionary.

`SpeciesCollection` is intended to provide a lookup collection of species, but its constructor is currently empty.

### Breeding layer

`Breed` performs the offspring-generation process:

1. Check whether the parents can breed under the current rules.
2. Choose the offspring species.
3. Choose the offspring gender based on the selected species' gender distribution.
4. Create the offspring `Unit`.
5. Resolve a gene inheritance result for each registered concrete gene type.
6. Calculate inherited stats.
7. Return the offspring.

## Intended initialization flow

The intended data-driven runtime flow is:

1. Read the gene and species JSON definitions.
2. Deserialize the JSON into serializable DTOs.
3. Validate that referenced gene types and gene IDs exist.
4. Convert the validated definitions into runtime C# objects and collections.
5. Instantiate parent units and assign their species, genes, and stats.
6. Construct a `BreedContainer`.
7. Call `Breed.GetOffspring` and inspect the result.

This pipeline mirrors the original game's JSON-to-native-component approach while keeping this prototype isolated from the rest of the game. It also creates a useful designer workflow: edit a JSON definition or a breeding rule, reload the data, and run another test.

The current archive does not implement a complete loader for this sequence. `EyeColor.LoadGenes`, `SpeciesCollection` construction, and `SpeciesData` construction are placeholders.

## Randomness

The breeding system uses `UnityEngine.Random`. Results are therefore governed by Unity's random state. For reproducible tests, control the Unity random seed before invoking the breeding operation.
