# ChunkReader.ReadPngSignatureAsync method

Reads the PNG signature at the current position in the stream.

```csharp
public ValueTask<bool> ReadPngSignatureAsync(CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| cancellationToken | A cancellation token which may be used cancel the operation. |

## Return Value

Returns `true` if the signature was successfully found and the underlying MarshalStream position is now immediately after the signature. Otherwise the method returns `false` because a valid signature was not found and the underlying MarshalStream position is unchanged.

## See Also

* class [ChunkReader](../ChunkReader.md)
* namespace [Phaeyz.Png](../../Phaeyz.Png.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Png.dll -->
