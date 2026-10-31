# Genes and Units

## Gene

`Gene` is an abstract base class for a genetic value.

```csharp
public abstract class Gene
{
    public readonly byte Value;
    public readonly string Key;
    public readonly Color ReferenceColor;
}
```

### Fields

| Field | Type | Meaning |
|---|---|---|
| `Value` | `byte` | Compact numeric value associated with the gene. |
| `Key` | `string` | Identifier/name associated with the gene. |
| `ReferenceColor` | `Color` | Color parsed from the supplied hexadecimal string. |

The constructor accepts `(byte value, string key, string hexColor)`. If `hexColor` cannot be parsed by `ColorUtility.TryParseHtmlString`, construction throws `ArgumentException`.

`ToString()` returns `Key`.

### Reflection helpers

- `Gene.GetAll<T>()`: reads public static fields declared directly on `T` and casts them to `T`.
- `Gene.GetAllKeys<T>()`: returns the `Key` values from those fields.
- `Gene.GetGeneByIndex<T>(int index)`: returns the first gene whose `Value` matches `index`.
- `Gene.GetAllDerivedTypes()`: discovers non-abstract subclasses of `Gene` in loaded assemblies.
- `Gene.AllTypes`: returns the currently registered gene types.
- `Gene.Register<T>()`: protected helper to register a concrete gene type.

**Initialization note:** `GetAll<T>()` depends on the static fields of the concrete type. `AllTypes` depends on registration having occurred. Ensure gene definitions are initialized before the breeding system enumerates registered types.

## Concrete gene classes

`EyeColor` is an example concrete subclass. In the supplied source, it declares a static `Genes` array and a `LoadGenes(string json)` method, but the array is not initialized and the loader body is empty. It is therefore a placeholder rather than a working JSON-backed implementation.

## Unit

`Unit` is a runtime individual.

### Identity

| Member | Description |
|---|---|
| `Species` | Immutable species identifier string. |
| `Gender` | Immutable `GenderType` (`Female` or `Male`). |
| `Nickname` | Private-set nickname, changed through `ChangeNickname`. |

### Stats

`UnitStats` contains three integer fields:

- `HP`
- `Attack`
- `Defence`

A new unit starts with a new `UnitStats` instance. `SetStats` replaces the current stats reference.

### Gene storage

The unit stores genes in a private `Dictionary<Type, Gene>`.

- `InjectGene(Gene gene)`: adds the gene only if that concrete type is not already present. A null argument throws `ArgumentNullException`.
- `OverrideGene(Gene gene)`: adds or replaces the gene for its concrete type. A null argument throws `ArgumentNullException`.
- `GetGene<T>()`: returns the gene stored under `typeof(T)`, or throws `InvalidOperationException` if no matching gene is present.

The storage model allows one gene per concrete gene class on a unit. For example, a unit can have one `EyeColor`, one `FurColor`, and one `BodyPattern`, assuming each is represented by a distinct subclass of `Gene`.
