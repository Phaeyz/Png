using Phaeyz.Marshalling;

namespace Phaeyz.Png;

/// <summary>
/// A writer for PNG chunks.
/// </summary>
public class ChunkWriter
{
    private readonly MarshalStream _stream;

    /// <summary>
    /// Initializes a new instance of the <see cref="Phaeyz.Png.ChunkWriter"/> class.
    /// </summary>
    /// <param name="stream">
    /// The stream to which PNG chunks will be written.
    /// </param>
    /// <exception cref="System.ArgumentException">
    /// The stream is not read-only.
    /// </exception>
    public ChunkWriter(MarshalStream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanWrite)
        {
            throw new ArgumentException("The stream is read-only.", nameof(stream));
        }

        _stream = stream;
    }

    /// <summary>
    /// Writes a PNG chunk to the stream.
    /// </summary>
    /// <param name="chunk">
    /// The chunk to write.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the operation completes.
    /// </returns>
    /// <exception cref="Phaeyz.Png.PngException">
    /// The length of the chunk computed to be less than zero.
    /// </exception>
    public async ValueTask WriteAsync(Chunk chunk, CancellationToken cancellationToken = default)
    {
        // Compute the chunk length so the length field may be written.
        int length = chunk.ValidateAndComputeLength();
        if (length < 0)
        {
            throw new PngException("The PNG chunk has an invalid length.");
        }

        // Write the length.
        await _stream.WriteInt32Async(length, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);

        // The chunk type and chunk data are computed as part of the CRC.
        Crc32Processor crc = new();
        using (_stream.AddWriteProcessor(crc))
        {
            // Write chunk type.
            await _stream.WriteUInt32Async(chunk.ChunkType.Value, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);

            // Write the chunk data.
            if (length > 0)
            {
                await chunk.WriteToStreamAsync(_stream, cancellationToken).ConfigureAwait(false);
            }
        }

        // Write the CRC.
        await _stream.WriteUInt32Async(crc.Value, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Writes the PNG signature at the current position in the stream.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the operation is complete.
    /// </returns>
    public ValueTask WritePngSignatureAsync(CancellationToken cancellationToken = default) =>
        _stream.WriteAsync(ChunkReader.PngSignature, cancellationToken);
}