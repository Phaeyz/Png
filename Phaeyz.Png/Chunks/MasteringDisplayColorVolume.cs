using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// Provides informative static metadata which allows a target (consumer) display to
/// potentially optimize its tone mapping decisions on a comparison of its inherent
/// capabilities versus the original mastering display's capabilities.
/// </summary>
[Chunk("mDCV")]
[OrderBeforeChunks("PLTE", "IDAT")]
public class MasteringDisplayColorVolume : Chunk
{
    /// <summary>
    /// The chunk's data is always this length.
    /// </summary>
    public const int FixedLength = 24;

    /// <summary>
    /// Mastering display color for red X or first primary chromaticities X. Divisor value is <c>0.00002</c>.
    /// </summary>
    public ushort RedOrPrimaryFirstChromaticityX { get; set; }

    /// <summary>
    /// Mastering display color for red Y or first primary chromaticities Y. Divisor value is <c>0.00002</c>.
    /// </summary>
    public ushort RedOrPrimaryFirstChromaticityY { get; set; }

    /// <summary>
    /// Mastering display color for blue X or second primary chromaticities X. Divisor value is <c>0.00002</c>.
    /// </summary>
    public ushort BlueOrPrimarySecondChromaticityX { get; set; }

    /// <summary>
    /// Mastering display color for blue Y or second primary chromaticities Y. Divisor value is <c>0.00002</c>.
    /// </summary>
    public ushort BlueOrPrimarySecondChromaticityY { get; set; }

    /// <summary>
    /// Mastering display color for green X or third primary chromaticities X. Divisor value is <c>0.00002</c>.
    /// </summary>
    public ushort GreenOrPrimaryThirdChromaticityX { get; set; }

    /// <summary>
    /// Mastering display color for green Y or third primary chromaticities Y. Divisor value is <c>0.00002</c>.
    /// </summary>
    public ushort GreenOrPrimaryThirdChromaticityY { get; set; }

    /// <summary>
    /// Mastering display white point chromaticity. Divisor value is <c>0.00002</c>.
    /// </summary>
    public ushort WhitePointChromaticityX { get; set; }

    /// <summary>
    /// Mastering display white point chromaticity. Divisor value is <c>0.00002</c>.
    /// </summary>
    public ushort WhitePointChromaticityY { get; set; }

    /// <summary>
    /// Mastering display maximum luminance. Divisor value is <c>0.0001</c>.
    /// </summary>
    public uint MaximumLuminance { get; set; }

    /// <summary>
    /// Mastering display maximum luminance. Divisor value is <c>0.0001</c>.
    /// </summary>
    public uint MinimumLuminance { get; set; }

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

        RedOrPrimaryFirstChromaticityX = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken);
        RedOrPrimaryFirstChromaticityY = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken);
        BlueOrPrimarySecondChromaticityX = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken);
        BlueOrPrimarySecondChromaticityY = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken);
        GreenOrPrimaryThirdChromaticityX = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken);
        GreenOrPrimaryThirdChromaticityY = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken);
        WhitePointChromaticityX = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken);
        WhitePointChromaticityY = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken);
        MaximumLuminance = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
        MinimumLuminance = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
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
        await stream.WriteUInt16Async(RedOrPrimaryFirstChromaticityX, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt16Async(RedOrPrimaryFirstChromaticityY, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt16Async(BlueOrPrimarySecondChromaticityX, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt16Async(BlueOrPrimarySecondChromaticityY, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt16Async(GreenOrPrimaryThirdChromaticityX, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt16Async(GreenOrPrimaryThirdChromaticityY, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt16Async(WhitePointChromaticityX, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt16Async(WhitePointChromaticityY, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(MaximumLuminance, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(MinimumLuminance, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
    }
}