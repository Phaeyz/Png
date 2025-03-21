# ImageData class

Contains the actual image data which is the output stream of the compression algorithm.

```csharp
public class ImageData : Chunk
```

## Public Members

| name | description |
| --- | --- |
| [ImageData](ImageData/ImageData.md)() | The default constructor. |
| [Data](ImageData/Data.md) { get; set; } | Image data which is the output stream of the compression algorithm. |
| override [ReadFromStreamAsync](ImageData/ReadFromStreamAsync.md)(…) | Reads the chunk from the stream, hydrating the properties of the chunk. |
| override [ValidateAndComputeLength](ImageData/ValidateAndComputeLength.md)() | Validates all properties of the chunk such that the chunk is ready for serialization, and computes the data length of the chunk. |
| override [WriteToStreamAsync](ImageData/WriteToStreamAsync.md)(…) | Writes the chunk to the stream. |

## See Also

* class [Chunk](../Phaeyz.Png/Chunk.md)
* namespace [Phaeyz.Png.Chunks](../Phaeyz.Png.md)
* [ImageData.cs](https://github.com/Phaeyz/Png/blob/main/Phaeyz.Png/Chunks/ImageData.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Png.dll -->
