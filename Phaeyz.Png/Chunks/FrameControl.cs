using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// The dimensions, position, delay and disposal of an individual frame.
/// </summary>
[Chunk("fcTL", AllowMultiple = true)]
[OrderAfterChunks("IDAT", FirstChunkIsExempt = true)]
public class FrameControl : Chunk
{
    /// <summary>
    /// The chunk's data is always this length.
    /// </summary>
    public const int FixedLength = 26;

    /// <summary>
    /// The sequence number of the animation chunk, starting from <c>0</c>.
    /// </summary>
    public uint SequenceNumber { get; set; }

    /// <summary>
    /// The width of the following frame.
    /// </summary>
    public uint Width { get; set; }

    /// <summary>
    /// The height of the following frame.
    /// </summary>
    public uint Height { get; set; }

    /// <summary>
    /// The X-position of the following frame.
    /// </summary>
    public uint OffsetX { get; set; }

    /// <summary>
    /// The Y-position of the following frame.
    /// </summary>
    public uint OffsetY { get; set; }

    /// <summary>
    /// The numerator of the delay fraction; indicating the time to display the current frame, in seconds.
    /// </summary>
    /// <remarks>
    /// If the the value of the numerator is <c>0</c> the decoder should render the next frame as quickly
    /// as possible, though viewers may impose a reasonable lower bound.
    /// </remarks>
    public ushort DelayNumerator { get; set; }

    /// <summary>
    /// The denominator of the delay fraction; indicating the time to display the current frame, in seconds.
    /// </summary>
    /// <remarks>
    /// If the denominator is <c>0</c>, it is to be treated as if it were <c>100</c> (that is, the numerator then
    /// specifies 1/100ths of a second).
    /// </remarks>
    public ushort DelayDenominator { get; set; }

    /// <summary>
    /// The type of frame area disposal to be done after rendering this frame; in other words, it
    /// specifies how the output buffer should be changed at the end of the delay (before rendering
    /// the next frame).
    /// </summary>
    public FrameAreaDisposal DisposeOperation { get; set; }

    /// <summary>
    /// Specifies whether the frame is to be alpha blended into the current output buffer content,
    /// or whether it should completely replace its region in the output buffer.
    /// </summary>
    public FrameBlendBehavior BlendOperation { get; set; }

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

        SequenceNumber = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        Width = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        Height = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        OffsetX = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        OffsetY = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        DelayNumerator = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        DelayDenominator = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        DisposeOperation = (FrameAreaDisposal)await stream.ReadUInt8Async(cancellationToken);
        BlendOperation = (FrameBlendBehavior)await stream.ReadUInt8Async(cancellationToken);
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
        await stream.WriteUInt32Async(SequenceNumber, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(Width, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(Height, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(OffsetX, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(OffsetY, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt16Async(DelayNumerator, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt16Async(DelayDenominator, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt8Async((byte)DisposeOperation, cancellationToken);
        await stream.WriteUInt8Async((byte)BlendOperation, cancellationToken);
    }
}