# ChunkAttribute.Get method (1 of 2)

Gets the [`ChunkAttribute`](../ChunkAttribute.md) of a chunk class type.

```csharp
public static ChunkAttribute Get(Type chunkClassType)
```

| parameter | description |
| --- | --- |
| chunkClassType | The chunk class type. |

## Return Value

The [`ChunkAttribute`](../ChunkAttribute.md) of a chunk class type.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentException | The type associated with *chunkClassType* does not implement [`Chunk`](../Chunk.md), or the type does not have a public default parameterless constructor, or the type does not have a [`ChunkAttribute`](../ChunkAttribute.md). |
| ArgumentNullException | *chunkClassType* is `null`. |

## See Also

* class [ChunkAttribute](../ChunkAttribute.md)
* namespace [Phaeyz.Png](../../Phaeyz.Png.md)

---

# ChunkAttribute.Get&lt;T&gt; method (2 of 2)

Gets the [`ChunkAttribute`](../ChunkAttribute.md) of a chunk class type.

```csharp
public static ChunkAttribute Get<T>()
    where T : Chunk, new()
```

| parameter | description |
| --- | --- |
| T | The chunk class type. |

## Return Value

The [`ChunkAttribute`](../ChunkAttribute.md) of a chunk class type.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentException | The chunk class type does not implement [`Chunk`](../Chunk.md), or the type does not have a public default parameterless constructor, or the type does not have a [`ChunkAttribute`](../ChunkAttribute.md). |

## See Also

* class [Chunk](../Chunk.md)
* class [ChunkAttribute](../ChunkAttribute.md)
* namespace [Phaeyz.Png](../../Phaeyz.Png.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Png.dll -->
