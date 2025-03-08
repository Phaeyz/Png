using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// Declares that this is an animated PNG image, gives the number of frames, and the number of times to loop.
/// </summary>
[Chunk("acTL")]
[OrderBeforeChunks("IDAT")]
public class AnimationControl : Chunk
{
    /// <summary>
    /// The chunk's data is always this length.
    /// </summary>
    public const int FixedLength = 8;

    /// <summary>
    /// The total number of frames in the animation.
    /// </summary>
    /// <remarks>
    /// This must equal the number of fcTL chunks. <c>0</c> is not a valid value. <c>1</c> is a valid value,
    /// for a single-frame PNG. If this value does not equal the actual number of frames it should be treated
    /// as an error.
    /// </remarks>
    public uint FrameCount;

    /// <summary>
    /// The number of times that this animation should play.
    /// </summary>
    /// <remarks>
    /// If it is <c>0</c>, the animation should play indefinitely. If non-zero, the animation should come to
    /// rest on the final frame at the end of the last play.
    /// </remarks>
    public uint PlayCount;

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

        FrameCount = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
        PlayCount = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken);
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
        if (FrameCount == 0)
        {
            throw new PngException("Invalid FrameCount.");
        }
        return FixedLength;
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
        await stream.WriteUInt32Async(FrameCount, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(PlayCount, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
    }
}