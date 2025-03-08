using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// Specifies a default background color to present the image against.
/// </summary>
[Chunk("bKGD")]
[OrderAfterChunks("PLTE")]
[OrderBeforeChunks("IDAT")]
public class BackgroundColor : Chunk
{
    /// <summary>
    /// The color data for the background image.
    /// </summary>
    /// <remarks>
    /// The layout and length of the data buffer is dependent upon the PNG image type:
    /// <code>
    /// PngImageType.Greyscale and PngImageType.GreyscaleWithAlpha:
    ///     Greyscale                   2 bytes
    ///
    /// PngImageType.Truecolor and PngImageType.TruecolorWithAlpha:
    ///     Red	                        2 bytes
    ///     Green                       2 bytes
    ///     Blue                        2 bytes
    ///
    /// PngImageType.IndexedColor:
    ///     Palette index               1 byte
    /// </code>
    /// </remarks>
    public byte[] ColorData { get; set; } = [];

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
        ColorData = new byte[chunkLength];
        await stream.ReadExactlyAsync(ColorData, cancellationToken).ConfigureAwait(false);
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
        switch (ColorData.Length)
        {
            case 1:
            case 2:
            case 6:
                break;
            default:
                throw new PngException("Invalid ColorData.");
        }
        return ColorData.Length;
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
        await stream.WriteAsync(ColorData, cancellationToken).ConfigureAwait(false);
    }
}