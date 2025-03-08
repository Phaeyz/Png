using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// A chunk for which there is no defined class for the chunk type. The chunk data is available
/// as a byte array.
/// </summary>
[Chunk("PLTE")]
[OrderBeforeChunks("IDAT")]
public class Palette : Chunk
{
    /// <summary>
    /// A series of three-byte-triples (R, G, and B) accounting for palette data.
    /// </summary>
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
    public override int ValidateAndComputeLength()
    {
        if (Data.Length % 3 == 0)
        {
            throw new PngException("The palette data must be divisible by three.");
        }

        int entryCount = Data.Length / 3;
        if (entryCount < 1 || entryCount > 256)
        {
            throw new PngException("There must be between 1 and 256 palette entries.");
        }

        return Data.Length;
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
        await stream.WriteAsync(Data, cancellationToken).ConfigureAwait(false);
    }
}