# Phaeyz

Phaeyz is a set of libraries created and polished over time for use with other projects, and made available here for convenience.

All Phaeyz libraries may be found [here](https://github.com/Phaeyz).

# Phaeyz.Png

API documentation for **Phaeyz.Png** library is [here](https://github.com/Phaeyz/Png/blob/main/docs/Phaeyz.Png.md).

This library contains classes which allow for deserializing PNG (Portable Network Graphics), editing and adding PNG chunks, as well as serializing it back out. Additionally there is utility for validating and correcting chunk order, and deserializing and serializing compressed strings within chunks. The deserializer and serializer was written such that if chunk types are not built-in or supported by Phaeyz.Png, they are stored as generic chunks, and chunk types can be extended to make them strongly typed.

Note this library does not protect you from creating non-standard PNG. Some chunks may be required and in certain orders in some contexts. Furthermore, this library only provides raw structural data for PNG, and currently does not provide decoding and encoding of image data -- though image data will be readily available through this library.

For more information on PNG, see [PNG on Wikipedia](https://en.wikipedia.org/wiki/PNG) and/or [PNG Specification on W3](https://www.w3.org/TR/png/).

Here are some highlights of this library.

## PngMetadata (deserializing)

```C#
using FileStream fileStream = File.OpenRead(filePath);
using MarshalStream inputStream = new MarshalStream(fileStream, false); // Used to efficiently read file
// Some files have back-to-back PNG streams, where the second is a thumbnail or grayscale version.
// Instead of using ReadFromStreamAsync, ReadAllFromStreamAsync may extract them all into a list.
List<PngMetadata> pngMetadatas = await PngMetadata.ReadAllFromStreamAsync(inputStream);
foreach (PngMetadata pngMetadata in pngMetadatas)
{
    Exif? exif = pngMetadata.FindFirst<Exif>(); // Get the Exif segment
}
```

## PngMetadata (serializing)

```C#
using FileStream fileStream = File.OpenRead(filePath);
using MarshalStream inputStream = new MarshalStream(fileStream, false); // Used to efficiently read file
// Some files have back-to-back PNG streams, where the second is a thumbnail or grayscale version.
// Instead of using ReadFromStreamAsync, ReadAllFromStreamAsync may extract them all into a list.
List<PngMetadata> pngMetadatas = await PngMetadata.ReadAllFromStreamAsync(inputStream);
// Make an output stream
using MemoryStream memoryStream = new();
using MarshalStream outputStream = new MarshalStream(memoryStream, false); // Used to efficient write file
await PngMetadata.WriteAllToStreamAsync(outputStream, pngMetadatas); // Can write all PNG metadata
// Can also write independent PNG metadata
foreach (PngMetadata pngMetadata in pngMetadatas)
{
    await pngMetadata.WriteToStreamAsync(outputStream);
}
```

## PngMetadata (removing chunks)

```C#
using FileStream fileStream = File.OpenRead(filePath);
using MarshalStream inputStream = new MarshalStream(fileStream, false); // Used to efficiently read file
// Some files have back-to-back PNG streams, where the second is a thumbnail or grayscale version.
// Instead of using ReadFromStreamAsync, ReadAllFromStreamAsync may extract them all into a list.
List<PngMetadata> pngMetadatas = await PngMetadata.ReadAllFromStreamAsync(inputStream);
foreach (PngMetadata pngMetadata in pngMetadatas)
{
    // PngMetadata is basically a collection of chunks, and chunks may be removed
    pngMetadata.RemoveAll<TextualData>(); // Remove all chunks of this type
    pngMetadata.RemoveAll<CompressedTextualData>(); // Remove all chunks of this type
    pngMetadata.RemoveAll(ChunkType.InternationalTextualData); // May also use ChunkType enum.
}
```

## PngMetadata (automatically fix chunk order)

```C#
PngMetadata pngMetadata = new();
// Add chunks
pngMetadata.Chunks.Add(new ImageEnd());
pngMetadata.Chunks.Add(new ImageHeader());
pngMetadata.Chunks.Add(new ImageData());
// Fix the order - will change to head, data, then end.
// Prefer to always do this before serializing to ensure PNG chunk order is to spec.
pngMetadata.ValidateAndReorderChunks();
```

## ChunkDefinitions (custom segments)

```C#
// Define custom chunk.
[Chunk("tESt")] // Define the chunk type. May also specify AllowMultiple = true.
[OrderBeforeChunks("IDAT")] // These hints are used during automatic reordering.
public class CustomChunk : Chunk
{
    public byte[] Data { get; set; } = [];
    public override async ValueTask ReadFromStreamAsync(MarshalStream stream, int chunkLength, CancellationToken cancellationToken)
    {
        Data = new byte[chunkLength];
        await stream.ReadExactlyAsync(Data, cancellationToken).ConfigureAwait(false);
    }
    public override int ValidateAndComputeLength() => Data.Length;
    public override async ValueTask WriteToStreamAsync(MarshalStream stream, CancellationToken cancellationToken)
    {
        await stream.WriteAsync(Data, cancellationToken).ConfigureAwait(false);
    }
}

// Define the chunk
ChunkDefinitions chunkDefinitions = new(ChunkDefinitions.Default);
chunkDefinitions.Add<CustomChunk>();
// Now deserialize a stream with support for that chunk.
using FileStream fileStream = File.OpenRead(filePath);
using MarshalStream inputStream = new MarshalStream(fileStream, false); // Used to efficiently read file
// Some files have back-to-back PNG streams, where the second is a thumbnail or grayscale version.
// Instead of using ReadFromStreamAsync, ReadAllFromStreamAsync may extract them all into a list.
// Notice chunkDefinitions is passed in here.
List<PngMetadata> pngMetadatas = await PngMetadata.ReadAllFromStreamAsync(inputStream, chunkDefinitions);
foreach (PngMetadata pngMetadata in pngMetadatas)
{
    CustomChunk? customChunk = pngMetadata.FindFirst<CustomChunk>(); // Now fetch the custom chunk
}
```

# Licensing

This project is licensed under GNU General Public License v3.0, which means you can use it for personal or educational purposes for free. However, donations are always encouraged to support the ongoing development of adding new features and resolving issues.

If you plan to use this code for commercial purposes or within an organization, we kindly ask for a donation to support the project's development. Any reasonably sized donation amount which reflects the perceived value of using Phaeyz in your product or service is accepted.

## Donation Options

There are several ways to support Phaeyz with a donation. Perhaps the best way is to use Patreon so that recurring small donations continue to support the development of Phaeyz.

- **Patreon**: [https://www.patreon.com/phaeyz](https://www.patreon.com/phaeyz)
- **Bitcoin**: Send funds to address: ```bc1qdzdahz8d7jkje09fg7s7e8xedjsxm6kfhvsgsw```
- **PayPal**: Send funds to ```phaeyz@pm.me``` ([directions](https://www.paypal.com/us/cshelp/article/how-do-i-send-money-help293))

Your support is greatly appreciated and helps me continue to improve and maintain Phaeyz!