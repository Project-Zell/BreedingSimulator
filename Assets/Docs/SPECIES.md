# Species Data and Gene Pools

## SpeciesData

`SpeciesData` represents species-specific rules used during breeding.

### Configuration represented in the class

| Member | Type | Purpose |
|---|---|---|
| `GenderDistribution` | `GenderDisrtibution` | Determines whether offspring are always male, always female, or use the default random distribution. |
| `_reproductionType` | `Reprodution` | Controls species inheritance behavior. |
| `StatRangeHP` | `Range` | Intended valid/base range for offspring HP. |
| `StatRangeAttack` | `Range` | Intended valid/base range for offspring Attack. |
| `StatRangeDefence` | `Range` | Intended valid/base range for offspring Defence. |
| `_genePools` | `Dictionary<Type, object>` | Stores gene arrays keyed by concrete gene type. |

### Gender distribution

The enum currently defines:

- `OnlyMale`
- `OnlyFemale`
- `Default`

`Breed` uses this value to determine the offspring's gender. For `Default`, the current implementation chooses Female when the random roll is below `0.55`; otherwise it chooses Male.

### Reproduction behavior

The enum currently defines:

- `Normal`
- `Dominant`
- `Passive`

The public read-only properties map these values as follows:

- `ForceTheSameSpecies` is true for `Dominant`.
- `SpeciesNonInheritable` is true for `Passive`.

The field `_reproductionType` is currently initialized to `Normal` and is `readonly`; the supplied source does not show a constructor or loader assigning another value. As supplied, the two special behaviors cannot be configured through the public API.

### Gene-pool queries

The class exposes generic methods:

- `HasGene<T>()`: true if a non-empty `T[]` exists in `_genePools`.
- `IsGenePure<T>(T gene)`: true if the supplied gene is contained in the species' `T[]` pool.
- `GetRandomGene<T>()`: returns a random member of the species' `T[]` pool; throws `InvalidOperationException` if no non-empty pool exists.

The current implementation expects the values stored in `_genePools` to be arrays of the concrete gene type (`T[]`). The supplied constructor is empty and does not populate the dictionary.

## Range

`Range` is a readonly struct with inclusive `Min` and `Max` bounds.

- `new Range(a, b)` normalizes the values so `Min` is the lower endpoint and `Max` is the higher endpoint.
- `InRange(value)` checks whether a value is between the endpoints, inclusive.
- `Clamp(value)` clamps an integer to the endpoints.
- `GetRandom()` returns a random integer in the inclusive interval. It uses `Random.Range(Min, Max + 1)` to include `Max`.

The default value of `Range` is `(0, 0)`. The supplied `SpeciesData` field initializers use `new Range()` and therefore currently produce zero-only ranges until replaced with configured values.

## SpeciesCollection

`SpeciesCollection` declares a read-only dictionary of species data, but the constructor taking a JSON string is empty in the supplied archive. No species lookup API is currently implemented in the class.
