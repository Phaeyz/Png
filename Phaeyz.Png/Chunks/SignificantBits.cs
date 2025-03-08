using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// Defines the original number of significant bits (which can be less than or equal to the sample depth).
/// This allows PNG decoders to recover the original data losslessly even if the data had a sample depth
/// not directly supported by PNG.
/// </summary>
[Chunk("sBIT")]
[OrderBeforeChunks("PLTE", "IDAT")]
public class SignificantBits : Chunk
{
    /// <summary>
    /// Defines the original number of significant bits (which can be less than or equal to the sample depth).
    /// </summary>
    /// <remarks>
    /// The layout and length of the data buffer is dependent upon the PNG image type:
    /// <code>
    /// PngImageType.Greyscale:
    ///     Significant greyscale bits  1 byte
    ///
    /// PngImageType.Truecolor and PngImageType.IndexedColor:
    ///     Significant red bits        1 byte
    ///     Significant green bits      1 byte
    ///     Significant blue bits       1 byte
    ///
    /// PngImageType.GreyscaleWithAlpha:
    ///     Significant greyscale bits  1 byte
    ///     Significant alpha bits      1 byte
    ///
    /// PngImageType.TruecolorWithAlpha:
    ///     Significant red bits        1 byte
    ///     Significant green bits      1 byte
    ///     Significant blue bits       1 byte
    ///     Significant alpha bits      1 byte
    /// </code>
    /// </remarks>
    public byte[] Bits { get; set; } = [];

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
        Bits = new byte[chunkLength];
        await stream.ReadExactlyAsync(Bits, cancellationToken).ConfigureAwait(false);
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
        if (Bits.Length == 0 || Bits.Length > 4)
        {
            throw new PngException("Invalid Bits.");
        }
        return Bits.Length;
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
        await stream.WriteAsync(Bits, cancellationToken).ConfigureAwait(false);
    }
}