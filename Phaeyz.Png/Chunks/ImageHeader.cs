using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// Image header.
/// </summary>
[Chunk("IHDR")]
public class ImageHeader : Chunk
{
    /// <summary>
    /// The chunk's data is always this length.
    /// </summary>
    public const int FixedLength = 13;

    /// <summary>
    /// The image width. Must be greater than zero.
    /// </summary>
    public uint Width { get; set; }

    /// <summary>
    /// The image height. Must be greater than zero.
    /// </summary>
    public uint Height { get; set; }

    /// <summary>
    /// The number of bits per sample or per palette index (not per pixel). Valid values depend
    /// on the PNG image type.
    /// </summary>
    public byte BitDepth { get; set; }

    /// <summary>
    /// The color type used in the PNG image.
    /// </summary>
    public ColorType ColorType { get; set; }

    /// <summary>
    /// The method used to compress the image data.
    /// </summary>
    public CompressionMethod CompressionMethod { get; set; }

    /// <summary>
    /// The preprocessing method applied to the image data before compression.
    /// </summary>
    public FilterMethod FilterMethod { get; set; }

    /// <summary>
    /// The transmission order of the image data.
    /// </summary>
    public InterlaceMethod InterlaceMethod { get; set; }

    /// <summary>
    /// The PNG image type.
    /// </summary>
    public PngImageType PngImageType => PngImageType.FromColorType(ColorType);

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

        Width = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        Height = await stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        BitDepth = await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false);
        ColorType = (ColorType)await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false);
        CompressionMethod = (CompressionMethod)await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false);
        FilterMethod = (FilterMethod)await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false);
        InterlaceMethod = (InterlaceMethod)await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false);
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
        if (Width == 0)
        {
            throw new PngException("Invalid Width.");
        }

        if (Height == 0)
        {
            throw new PngException("Invalid Height.");
        }

        if (!PngImageType.AllowedBitDepths.Contains(BitDepth))
        {
            throw new PngException("Invalid BitDepth.");
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
        await stream.WriteUInt32Async(Width, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt32Async(Height, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt8Async(BitDepth, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt8Async((byte)ColorType, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt8Async((byte)CompressionMethod, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt8Async((byte)FilterMethod, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt8Async((byte)InterlaceMethod, cancellationToken).ConfigureAwait(false);
    }
}