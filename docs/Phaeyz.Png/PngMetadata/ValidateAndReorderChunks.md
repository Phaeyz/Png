# PngMetadata.ValidateAndReorderChunks method (1 of 2)

Validates the chunks in the metadata matches the policies defined on the class, and if valid the order of the chunks is updated to adheres to required ordering rules.

```csharp
public void ValidateAndReorderChunks()
```

## Exceptions

| exception | condition |
| --- | --- |
| [PngException](../PngException.md) | A validation error occurred on the list of chunks. |

## See Also

* class [PngMetadata](../PngMetadata.md)
* namespace [Phaeyz.Png](../../Phaeyz.Png.md)

---

# PngMetadata.ValidateAndReorderChunks method (2 of 2)

Validates the list of chunks matches the policies defined on the class, and if valid the order of the chunks is updated into a new list where the order adheres to required ordering rules.

```csharp
public static List<Chunk> ValidateAndReorderChunks(List<Chunk> chunks)
```

| parameter | description |
| --- | --- |
| chunks | The chunks to validate and order. |

## Return Value

A new list containing a new order of the chunks. The order may be the same if reordering was not necessary.

## Exceptions

| exception | condition |
| --- | --- |
| [PngException](../PngException.md) | A validation error occurred on the list of chunks. |

## See Also

* class [Chunk](../Chunk.md)
* class [PngMetadata](../PngMetadata.md)
* namespace [Phaeyz.Png](../../Phaeyz.Png.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Png.dll -->
