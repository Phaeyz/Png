using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// The set of fdAT chunks contains the image data for all frames (or, for animations which include
/// the static image as first frame, for all frames after the first one).
/// </summary>
[Chunk("fdAT", AllowMultiple = true)]
[OrderAfterChunks("IDAT")]
public class FrameData : Chunk
{
    /// <summary>
    /// The frame sequence number.
    /// </summary>
    public uint SequenceNumber { get; set; }

    /// <summary>
    /// The frame data.
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
        SequenceNumber = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        chunkLength -= sizeof(uint);

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
    public override int ValidateAndComputeLength() => sizeof(uint) + Data.Length;

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
        await stream.WriteUInt32Async(SequenceNumber, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteAsync(Data, cancellationToken).ConfigureAwait(false);
    }
}