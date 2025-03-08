using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// Gives the approximate usage frequency of each color in the palette.
/// </summary>
[Chunk("hIST")]
[OrderAfterChunks("PLTE")]
[OrderBeforeChunks("IDAT")]
public class Histogram : Chunk
{
    /// <summary>
    /// The approximate usage frequency of each color in the palette.
    /// </summary>
    public ushort[] Frequency { get; set; } = [];

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
        if (chunkLength % 2 != 0)
        {
            throw new PngException("Cannot deserialize the histogram because length does not align to the element size.");
        }
        int elementCount = chunkLength / 2;
        Frequency = new ushort[elementCount];
        for (int i = 0; i < elementCount; i++)
        {
            Frequency[i] = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
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
    public override int ValidateAndComputeLength() => Frequency.Length * sizeof(ushort);

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
        for (int i = 0; i < Frequency.Length; i++)
        {
            await stream.WriteUInt16Async(Frequency[i], ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        }
    }
}