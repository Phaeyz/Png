using Phaeyz.Marshalling;

namespace Phaeyz.Png;

/// <summary>
/// A reader for PNG chunks.
/// </summary>
public class ChunkReader
{
    /// <summary>
    /// All PNG streams begin with this signature.
    /// </summary>
    public static readonly ReadOnlyMemory<byte> PngSignature = "\x89PNG\r\n\x1A\n".Select(c => (byte)c).ToArray();

    private readonly ChunkDefinitions _chunkDefinitions;
    private readonly MarshalStream _stream;

    /// <summary>
    /// Initializes a new instance of the <see cref="Phaeyz.Png.ChunkReader"/> class.
    /// </summary>
    /// <param name="stream">
    /// The stream containing PNG chunks. The buffer size of this stream must be at least the length of
    /// <see cref="Phaeyz.Png.ChunkReader.PngSignature"/>.
    /// </param>
    /// <param name="chunkDefinitions">
    /// A set of chunk definitions used for mapping to chunk classes. If not provided, the default set will be used.
    /// </param>
    /// <exception cref="System.ArgumentException">
    /// The stream is not readable or the buffer size of <paramref name="stream"/> is less than
    /// <see cref="Phaeyz.Png.ChunkReader.PngSignature"/>.
    /// </exception>
    public ChunkReader(MarshalStream stream, ChunkDefinitions? chunkDefinitions = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
        {
            throw new ArgumentException("The stream is not readable.", nameof(stream));
        }

        if (!stream.IsFixedBuffer && stream.TotalBufferSize < PngSignature.Length)
        {
            throw new ArgumentException("The stream buffer size is too small for parsing.", nameof(stream));
        }

        _chunkDefinitions = chunkDefinitions ?? ChunkDefinitions.Default;
        _stream = stream;
    }

    /// <summary>
    /// Reads the next chunk from the stream.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding the next chunk from the stream.
    /// </returns>
    /// <exception cref="Phaeyz.Png.PngException">
    /// The chunk length is invalid or the chunk CRC is invalid.
    /// </exception>
    public async ValueTask<Chunk> ReadAsync(CancellationToken cancellationToken = default)
    {
        // Get the length of the stream.
        int length = await _stream.ReadInt32Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        if (length < 0)
        {
            throw new PngException("The PNG chunk has an invalid length.");
        }

        // The chunk type and chunk data are computed as part of the CRC.
        Crc32Processor crc = new();
        Chunk chunk;
        using (_stream.AddReadProcessor(crc))
        {
            // Get the chunk type.
            ChunkType chunkType = new(await _stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false));

            // Create a chunk instance of the chunk type to deserialize the chunk data.
            Type? chunkClassType = _chunkDefinitions.GetMapping(chunkType);
            chunk = chunkClassType is null ? new UnknownChunk(chunkType) : (Chunk)Activator.CreateInstance(chunkClassType)!;

            // Deserialize the chunk data.
            await chunk.ReadFromStreamAsync(_stream, length, cancellationToken).ConfigureAwait(false);
        }

        // Validate the CRC.
        uint expectedCrc = await _stream.ReadUInt32Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        if (expectedCrc != crc.Value)
        {
            throw new PngException("The PNG chunk has an invalid CRC.");
        }

        return chunk;
    }

    /// <summary>
    /// Reads the PNG signature at the current position in the stream.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used cancel the operation.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the signature was successfully found and the underlying <see cref="Phaeyz.Marshalling.MarshalStream"/>
    /// position is now immediately after the signature. Otherwise the method returns <c>false</c> because a valid signature was not
    /// found and the underlying <see cref="Phaeyz.Marshalling.MarshalStream"/> position is unchanged.
    /// </returns>
    public async ValueTask<bool> ReadPngSignatureAsync(CancellationToken cancellationToken = default)
    {
        MarshalStreamMatchResult result = await _stream.MatchAsync(PngSignature, cancellationToken).ConfigureAwait(false);
        return result.Success;
    }
}