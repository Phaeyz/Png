namespace Phaeyz.Png.Chunks;

/// <summary>
/// The preprocessing method applied to the image data before compression.
/// </summary>
public enum FilterMethod : byte
{
    /// <summary>
    /// Adaptive filtering with five basic filter types.
    /// </summary>
    Adaptive = 0
}