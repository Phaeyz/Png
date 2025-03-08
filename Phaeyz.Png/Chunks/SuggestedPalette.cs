using System.Text;
using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// Provides a suggested palette for indexed-color images. This chunk is optional and is primarily used to
/// improve the appearance of images when they are displayed on devices with limited color capabilities or
/// when the image is converted to a different format that uses a limited color palette.
/// </summary>
[Chunk("sPLT", AllowMultiple = true)]
[OrderBeforeChunks("IDAT")]
public class SuggestedPalette : Chunk
{
    /// <summary>
    /// The palette name can be any convenient name for referring to the palette.
    /// </summary>
    /// <remarks>
    /// Example convenient palette names may include "256 color including Macintosh default",
    /// "256 color including Windows-3.1 default", and "Optimal 512". The palette name may aid the
    /// choice of the appropriate suggested palette when more than one appears in a PNG datastream.
    /// </remarks>
    public string PaletteName { get; set; } = string.Empty;

    /// <summary>
    /// The number of bits used to represent each color component (Red, Green, Blue, and optionally Alpha)
    /// in the palette entries. The sample depth can be either <c>8</c> bits or <c>16</c> bits per component.
    /// </summary>
    public byte SampleDepth { get; set; }

    /// <summary>
    /// The suggested palette entries.
    /// </summary>
    public SuggestedPaletteEntry[] Entries { get; set; } = [];

    /// <summary>
    /// Reads the chunk from the stream, hydrating the properties of the chunk.
    /// </summary>
    /// <param name="stream">
    /// The stream to read from.
    /// </param>
    /// <param name="chunkLength">
    /// The length of the chunk data on the stream.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the chunk has been read.
    /// </returns>
    /// <remarks>
    /// It is expected that the method reads exactly <paramref name="chunkLength"/> bytes from the stream.
    /// If less or more is read, it will lead to corruption.
    /// </remarks>
    public override async ValueTask ReadFromStreamAsync(MarshalStream stream, int chunkLength, CancellationToken cancellationToken)
    {
        var result = await stream.ReadStringAsync(
            Encoding.Latin1,
            chunkLength,
            MarshalStreamNullTerminatorBehavior.Stop,
            cancellationToken).ConfigureAwait(false);
        PaletteName = result.Value;

        chunkLength -= result.BytesRead;
        if (chunkLength < 1)
        {
            throw new PngException("Unexpected chunk length.");
        }

        SampleDepth = await stream.ReadUInt8Async(cancellationToken);
        int entrySize = SampleDepth switch
        {
            8 => SuggestedPaletteEntry.StructSizeForSampleDepth8,
            16 => SuggestedPaletteEntry.StructSizeForSampleDepth16,
            _ => throw new PngException("The SampleDepth must be either 8 or 16."),
        };

        if (--chunkLength % entrySize != 0)
        {
            throw new PngException("The remaining count does not align to the palette entry size");
        }

        int entryCount = chunkLength / entrySize;
        Entries = new SuggestedPaletteEntry[entryCount];

        for (int i = 0; i < entryCount; i++)
        {
            Entries[i] = new SuggestedPaletteEntry
            {
                Red = SampleDepth == 8
                    ? await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false)
                    : await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false),
                Green = SampleDepth == 8
                    ? await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false)
                    : await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false),
                Blue = SampleDepth == 8
                    ? await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false)
                    : await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false),
                Alpha = SampleDepth == 8
                    ? await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false)
                    : await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false),
                Frequency = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false)
            };
        }
    }

    /// <summary>
    /// Validates all properties of the chunk such that the chunk is ready for serialization,
    /// and computes the data length of the chunk.
    /// </summary>
    /// <returns>
    /// The data length of the chunk. It must match exactly the number of bytes that
    /// <see cref="Phaeyz.Png.Chunk.WriteToStreamAsync"/> would write.
    /// </returns>
    /// <remarks>
    /// This method is not called if the chunk does not have length.
    /// </remarks>
    public override int ValidateAndComputeLength()
    {
        int entrySize = SampleDepth switch
        {
            8 => SuggestedPaletteEntry.StructSizeForSampleDepth8,
            16 => SuggestedPaletteEntry.StructSizeForSampleDepth16,
            _ => throw new PngException("The SampleDepth must be either 8 or 16."),
        };

        if (SampleDepth == 8)
        {
            if (Entries.Any(entry => entry.Red > 0xFF || entry.Green > 0xFF || entry.Blue > 0xFF || entry.Alpha > 0xFF))
            {
                throw new PngException("A palette entry has a color value taking more than 8-bits.");
            }
        }

        return PaletteName.Length + 2 + (entrySize * Entries.Length);
    }

    /// <summary>
    /// Writes the chunk to the stream.
    /// </summary>
    /// <param name="stream">
    /// The stream to write to.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the chunk has been written.
    /// </returns>
    /// <remarks>
    /// This method is not called if the chunk does not have length.
    /// </remarks>
    public override async ValueTask WriteToStreamAsync(MarshalStream stream, CancellationToken cancellationToken)
    {
        await stream.WriteStringAsync(Encoding.Latin1, PaletteName, true, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt8Async(SampleDepth, cancellationToken).ConfigureAwait(false);

        for (int i = 0; i < Entries.Length; i++)
        {
            SuggestedPaletteEntry entry = Entries[i];
            if (SampleDepth == 8)
            {
                await stream.WriteUInt8Async((byte)entry.Red, cancellationToken).ConfigureAwait(false);
                await stream.WriteUInt8Async((byte)entry.Green, cancellationToken).ConfigureAwait(false);
                await stream.WriteUInt8Async((byte)entry.Blue, cancellationToken).ConfigureAwait(false);
                await stream.WriteUInt8Async((byte)entry.Alpha, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await stream.WriteUInt16Async(entry.Red, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
                await stream.WriteUInt16Async(entry.Green, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
                await stream.WriteUInt16Async(entry.Blue, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
                await stream.WriteUInt16Async(entry.Alpha, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
            }
            await stream.WriteUInt16Async(entry.Frequency, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        }
    }
}