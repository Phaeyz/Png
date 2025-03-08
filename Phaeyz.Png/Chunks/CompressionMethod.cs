namespace Phaeyz.Png.Chunks;

/// <summary>
/// The method used to compress the image data.
/// </summary>
public enum CompressionMethod : byte
{
    /// <summary>
    /// Deflate compression with a sliding window of at most 32768 bytes.
    /// </summary>
    Deflate = 0,
}