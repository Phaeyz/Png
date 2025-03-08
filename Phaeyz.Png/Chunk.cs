using Phaeyz.Marshalling;

namespace Phaeyz.Png;

/// <summary>
/// The base class for all chunks.
/// </summary>
public abstract class Chunk
{
    /// <summary>
    /// Called by base classes when initializing a chunk.
    /// </summary>
    protected Chunk() => ChunkType = ChunkAttribute.Get(GetType()).ChunkType;

    /// <summary>
    /// Used internally to initialize an unknown chunk's type.
    /// </summary>
    /// <param name="chunkType">
    /// The chunk's type.
    /// </param>
    internal protected Chunk(ChunkType chunkType) => ChunkType = chunkType;

    /// <summary>
    /// The chunk type.
    /// </summary>
    public ChunkType ChunkType { get; private set; }

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
    public abstract ValueTask ReadFromStreamAsync(
        MarshalStream stream,
        int chunkLength,
        CancellationToken cancellationToken);

    /// <summary>
    /// Returns a friendly string representation of the chunk.
    /// </summary>
    /// <returns>
    /// A friendly string representation of the chunk.
    /// </returns>
    public override string ToString() => $"{ChunkType} => {GetType().Name}";

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
    public abstract int ValidateAndComputeLength();

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
    public abstract ValueTask WriteToStreamAsync(
        MarshalStream stream,
        CancellationToken cancellationToken);
}