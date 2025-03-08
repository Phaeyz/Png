using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// Specifies either alpha values that are associated with palette entries
/// (for indexed-color images) or a single transparent color (for greyscale and truecolor images).
/// </summary>
[Chunk("tRNS")]
[OrderAfterChunks("PLTE")]
[OrderBeforeChunks("IDAT")]
public class Transparency : Chunk
{
    /// <summary>
    /// Defines the alpha sample values or palette references of the transparency.
    /// </summary>
    /// <remarks>
    /// The data is dependent upon the PNG image type:
    /// <code>
    /// PngImageType.Greyscale:
    ///     Grey sample value           2 bytes
    ///
    /// PngImageType.Truecolor:
    ///     Red sample value            2 bytes
    ///     Green sample value          2 bytes
    ///     Blue sample value           2 bytes
    ///
    /// PngImageType.IndexedColor:
    ///     Alpha for palette index 0   1 byte
    ///     Alpha for palette index 1   1 byte
    ///     ...etc...                   1 byte
    /// </code>
    /// </remarks>
    public byte[] Data { get; set; } = [];

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
        Data = new byte[chunkLength];
        await stream.ReadExactlyAsync(Data, cancellationToken).ConfigureAwait(false);
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
    public override int ValidateAndComputeLength() => Data.Length;

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
        await stream.WriteAsync(Data, cancellationToken).ConfigureAwait(false);
    }
}