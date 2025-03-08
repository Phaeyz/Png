using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// If the sRGB chunk is present, the image samples conform to the sRGB color space [SRGB]
/// and should be displayed using the specified rendering intent defined by the
/// International Color Consortium [ICC] or [ICC-2].
/// </summary>
/// <remarks>
/// If the sRGB chunk is present, the iCCP chunk should not be present.
/// </remarks>
[Chunk("sRGB")]
[OrderBeforeChunks("PLTE", "IDAT")]
public class StandardRgb : Chunk
{
    /// <summary>
    /// The chunk's data is always this length.
    /// </summary>
    public const int FixedLength = 1;

    /// <summary>
    /// The specified rendering intent defined by the International Color Consortium [ICC] or [ICC-2].
    /// </summary>
    public StandardRgbRenderingIntent RenderingIntent { get; set; }

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

        RenderingIntent = (StandardRgbRenderingIntent)await stream.ReadUInt8Async(cancellationToken);
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
        await stream.WriteUInt8Async((byte)RenderingIntent, cancellationToken).ConfigureAwait(false);
    }
}