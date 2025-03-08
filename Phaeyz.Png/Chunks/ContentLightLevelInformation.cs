using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// Adds static metadata which provides an opportunity to optimize tone mapping of the associated
/// content to a specific target display. This is accomplished by tailoring the tone mapping of
/// the content itself to the specific peak brightness capabilities of the target display to prevent
/// clipping. The method of tone-mapping optimization is currently subjective.
/// </summary>
[Chunk("cLLI")]
[OrderBeforeChunks("PLTE", "IDAT")]
public class ContentLightLevelInformation : Chunk
{
    /// <summary>
    /// The chunk's data is always this length.
    /// </summary>
    public const int FixedLength = 8;

    /// <summary>
    /// Maximum Content Light Level (MaxCLL). Divisor value is <c>0.0001 cd/m^2</c>
    /// </summary>
    /// <remarks>
    /// MaxCLL (Maximum Content Light Level) uses a static metadata value to indicate the maximum light
    /// level of any single pixel (in cd/m^2, also known as nits) of the entire playback sequence.
    /// There is often an algorithmic filter to eliminate false values occurring from processing or
    /// noise that could adversely affect intended downstream tone mapping.
    /// </remarks>
    public uint MaximumContentLightLevel { get; set; }

    /// <summary>
    /// Maximum Frame-Average Light Level (MaxFALL). Divisor value is <c>0.0001 cd/m^2</c>
    /// </summary>
    /// <remarks>
    /// MaxFALL (Maximum Frame Average Light Level) uses a static metadata value to indicate the maximum
    /// value of the frame average light level (in cd/m^2, also known as nits) of the entire playback sequence.
    /// MaxFALL is calculated by first averaging the decoded luminance values of all the pixels in each frame,
    /// and then using the value for the frame with the highest value.
    /// </remarks>
    public uint MaximumFrameAverageLightLevel { get; set; }

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

        MaximumContentLightLevel = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
        MaximumFrameAverageLightLevel = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
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
        await stream.WriteUInt32Async(MaximumContentLightLevel, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(MaximumFrameAverageLightLevel, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
    }
}