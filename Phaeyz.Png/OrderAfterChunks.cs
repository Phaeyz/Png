namespace Phaeyz.Png;

/// <summary>
/// When a chunk class is decorated with this, it indicates which other chunks it must be ordered after.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Phaeyz.Png.OrderAfterChunksAttribute"/> class.
/// </remarks>
/// <param name="chunkTypeLabels">
/// Labels of the chunk types which a decorated chunk must be ordered after.
/// </param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class OrderAfterChunksAttribute(params string[] chunkTypeLabels) : Attribute
{
    /// <summary>
    /// The chunk types which a decorated chunk must be ordered after.
    /// </summary>
    public IReadOnlySet<ChunkType> ChunkTypes { get; private set; } =
        new HashSet<ChunkType>(chunkTypeLabels.Select(l => new ChunkType(l)));

    /// <summary>
    /// The first chunk is exempt from this policy.
    /// </summary>
    public bool FirstChunkIsExempt { get; set; } = false;
}