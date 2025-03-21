# Exif class

Contains EXIF metadata.

```csharp
public class Exif : Chunk
```

## Public Members

| name | description |
| --- | --- |
| [Exif](Exif/Exif.md)() | The default constructor. |
| [Data](Exif/Data.md) { get; set; } | The EXIF metadata. |
| override [ReadFromStreamAsync](Exif/ReadFromStreamAsync.md)(…) | Reads the chunk from the stream, hydrating the properties of the chunk. |
| override [ValidateAndComputeLength](Exif/ValidateAndComputeLength.md)() | Validates all properties of the chunk such that the chunk is ready for serialization, and computes the data length of the chunk. |
| override [WriteToStreamAsync](Exif/WriteToStreamAsync.md)(…) | Writes the chunk to the stream. |

## See Also

* class [Chunk](../Phaeyz.Png/Chunk.md)
* namespace [Phaeyz.Png.Chunks](../Phaeyz.Png.md)
* [Exif.cs](https://github.com/Phaeyz/Png/blob/main/Phaeyz.Png/Chunks/Exif.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Png.dll -->
