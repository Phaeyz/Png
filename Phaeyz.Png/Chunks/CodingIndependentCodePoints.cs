using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
///  The color space (primaries), transfer function, matrix coefficients and scaling factor of
///  the image using the code points specified in [ITU-T-H.273].
/// </summary>
[Chunk("cICP")]
[OrderBeforeChunks("PLTE", "IDAT")]
public class CodingIndependentCodePoints : Chunk
{
    /// <summary>
    /// The chunk's data is always this length.
    /// </summary>
    public const int FixedLength = 4;

    /// <summary>
    /// Color Primaries.
    /// </summary>
    public byte ColorPrimaries { get; set; }

    /// <summary>
    /// Transfer Function.
    /// </summary>
    public byte TransferFunction { get; set; }

    /// <summary>
    /// Matrix Coefficients.
    /// </summary>
    public byte MatrixCoefficients { get; set; }

    /// <summary>
    /// Video Full Range Flag.
    /// </summary>
    public byte VideoFullRangeFlag { get; set; }

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

        ColorPrimaries = await stream.ReadUInt8Async(cancellationToken);
        TransferFunction = await stream.ReadUInt8Async(cancellationToken);
        MatrixCoefficients = await stream.ReadUInt8Async(cancellationToken);
        VideoFullRangeFlag = await stream.ReadUInt8Async(cancellationToken);
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
        await stream.WriteUInt8Async(ColorPrimaries, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt8Async(TransferFunction, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt8Async(MatrixCoefficients, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt8Async(VideoFullRangeFlag, cancellationToken).ConfigureAwait(false);
    }
}