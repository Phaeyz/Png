# CompressableText.FromStreamAsync method

Reads a specific number of bytes from a stream and assigns to a new [`CompressableText`](../CompressableText.md) instance.

```csharp
public static Task<CompressableText> FromStreamAsync(Stream stream, int byteCount, 
    bool isCompressed, CompressionMethod compressionMethod, 
    CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| stream | The stream to read bytes from. |
| byteCount | The number of bytes to read from the stream. |
| isCompressed | Determines whether or not the string within the stream is compressed. |
| compressionMethod | Determines the compression method of the compressed string within the stream. |
| cancellationToken | A cancellation token which may be used to cancel the operation. |

## Return Value

A task yielding a new [`CompressableText`](../CompressableText.md) instance.

## See Also

* enum [CompressionMethod](../../Phaeyz.Png.Chunks/CompressionMethod.md)
* class [CompressableText](../CompressableText.md)
* namespace [Phaeyz.Png](../../Phaeyz.Png.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Png.dll -->
