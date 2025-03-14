# PngMetadata.Insert&lt;T&gt; method

Inserts a chunk after the last chunk with any of the specified types.

```csharp
public int Insert<T>(T chunk, IEnumerable<ChunkType> precedingChunkTypes)
    where T : Chunk, new()
```

| parameter | description |
| --- | --- |
| T | The type of chunk being inserted. |
| chunk | The chunk to insert. |
| precedingChunkTypes | Inserts the chunk after the last chunk matching any of the specified types. If no types are specified, the chunk will be placed at the beginning. An image-header chunk is always implied to be in this array. |

## Return Value

The index which the chunk was inserted at.

## See Also

* class [ChunkType](../ChunkType.md)
* class [Chunk](../Chunk.md)
* class [PngMetadata](../PngMetadata.md)
* namespace [Phaeyz.Png](../../Phaeyz.Png.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Png.dll -->
