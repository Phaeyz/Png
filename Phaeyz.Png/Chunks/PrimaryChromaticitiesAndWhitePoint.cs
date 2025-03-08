using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// Used to specify the 1931 CIE x,y chromaticities of the red, green, and blue display primaries
/// used in the PNG image, and the referenced white point.
/// </summary>
[Chunk("cHRM")]
[OrderBeforeChunks("PLTE", "IDAT")]
public class PrimaryChromaticitiesAndWhitePoint : Chunk
{
    /// <summary>
    /// The chunk's data is always this length.
    /// </summary>
    public const int FixedLength = 32;

    /// <summary>
    /// White point x.
    /// </summary>
    public uint WhitePointX { get; set; }

    /// <summary>
    /// White point y.
    /// </summary>
    public uint WhitePointY { get; set; }

    /// <summary>
    /// Red x.
    /// </summary>
    public uint RedX { get; set; }

    /// <summary>
    /// Red y.
    /// </summary>
    public uint RedY { get; set; }

    /// <summary>
    /// Green x.
    /// </summary>
    public uint GreenX { get; set; }

    /// <summary>
    /// Green y.
    /// </summary>
    public uint GreenY { get; set; }

    /// <summary>
    /// Blue x.
    /// </summary>
    public uint BlueX { get; set; }

    /// <summary>
    /// Blue y.
    /// </summary>
    public uint BlueY { get; set; }

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
        if (chunkLength != FixedLength)
        {
            throw new PngException("Unexpected chunk length.");
        }

        WhitePointX = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
        WhitePointY = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
        RedX = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
        RedY = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
        GreenX = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
        GreenY = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
        BlueX = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
        BlueY = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
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
    public override int ValidateAndComputeLength() => FixedLength;

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
        await stream.WriteUInt32Async(WhitePointX, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(WhitePointY, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(RedX, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(RedY, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(GreenX, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(GreenY, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(BlueX, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(BlueY, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
    }
}