# Chunk class

The base class for all chunks.

```csharp
public abstract class Chunk
```

## Public Members

| name | description |
| --- | --- |
| [ChunkType](Chunk/ChunkType.md) { get; } | The chunk type. |
| abstract [ReadFromStreamAsync](Chunk/ReadFromStreamAsync.md)(…) | Reads the chunk from the stream, hydrating the properties of the chunk. |
| override [ToString](Chunk/ToString.md)() | Returns a friendly string representation of the chunk. |
| abstract [ValidateAndComputeLength](Chunk/ValidateAndComputeLength.md)() | Validates all properties of the chunk such that the chunk is ready for serialization, and computes the data length of the chunk. |
| abstract [WriteToStreamAsync](Chunk/WriteToStreamAsync.md)(…) | Writes the chunk to the stream. |

## Protected Members

| name | description |
| --- | --- |
| [Chunk](Chunk/Chunk.md)() | Called by base classes when initializing a chunk. |
| [Chunk](Chunk/Chunk.md)(…) | Used internally to initialize an unknown chunk's type. |

## See Also

* namespace [Phaeyz.Png](../Phaeyz.Png.md)
* [Chunk.cs](https://github.com/Phaeyz/Png/blob/main/Phaeyz.Png/Chunk.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Png.dll -->
