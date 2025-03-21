# PngMetadata.RemoveAll method (1 of 2)

Removes all chunks with the specified chunk type from the PNG metadata.

```csharp
public int RemoveAll(ChunkType chunkType)
```

| parameter | description |
| --- | --- |
| chunkType | The type of the chunk to remove. |

## Return Value

The number of matching chunks removed.

## See Also

* class [ChunkType](../ChunkType.md)
* class [PngMetadata](../PngMetadata.md)
* namespace [Phaeyz.Png](../../Phaeyz.Png.md)

---

# PngMetadata.RemoveAll&lt;T&gt; method (2 of 2)

Removes all chunks of the specified type from the PNG metadata.

```csharp
public int RemoveAll<T>()
    where T : Chunk, new()
```

| parameter | description |
| --- | --- |
| T | The type of chunk to remove. |

## Return Value

The number of matching chunks removed.

## See Also

* class [Chunk](../Chunk.md)
* class [PngMetadata](../PngMetadata.md)
* namespace [Phaeyz.Png](../../Phaeyz.Png.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Png.dll -->
