using Phaeyz.Marshalling;
using System.Text;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// If the iCCP chunk is present, the image samples conform to the color space represented by the
/// embedded ICC profile as defined by the International Color Consortium [ICC][ISO_15076-1].
/// </summary>
/// <remarks>
/// If the iCCP chunk is present, the sRGB chunk should not be present.
/// </remarks>
[Chunk("iCCP")]
[OrderBeforeChunks("PLTE", "IDAT")]
public class IccProfile : Chunk
{
    /// <summary>
    /// The profile name may be any convenient name for referring to the profile. It is case-sensitive.
    /// </summary>
    public string ProfileName { get; set; } = string.Empty;

    /// <summary>
    /// The method used to compress the profile.
    /// </summary>
    public CompressionMethod CompressionMethod { get; set; }

    /// <summary>
    /// The compressed profile. Decompression of this datastream yields the embedded ICC profile.
    /// </summary>
    public byte[] CompressionProfile { get; set; } = [];

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
        var result = await stream.ReadStringAsync(
            Encoding.Latin1,
            chunkLength,
            MarshalStreamNullTerminatorBehavior.Stop,
            cancellationToken).ConfigureAwait(false);
        ProfileName = result.Value;

        chunkLength -= result.BytesRead;
        if (chunkLength < 1)
        {
            throw new PngException("Unexpected chunk length.");
        }

        CompressionMethod = (CompressionMethod)await stream.ReadUInt8Async(cancellationToken);
        CompressionProfile = new byte[chunkLength - 1];
        await stream.ReadExactlyAsync(CompressionProfile, 0, CompressionProfile.Length, cancellationToken).ConfigureAwait(false);
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
    public override int ValidateAndComputeLength() => ProfileName.Length + 2 + CompressionProfile.Length;

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
        await stream.WriteStringAsync(Encoding.Latin1, ProfileName, true, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt8Async((byte)CompressionMethod, cancellationToken).ConfigureAwait(false);
        await stream.WriteAsync(CompressionProfile, cancellationToken).ConfigureAwait(false);
    }
}