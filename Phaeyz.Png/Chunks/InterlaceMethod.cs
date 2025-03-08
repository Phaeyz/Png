namespace Phaeyz.Png.Chunks;

/// <summary>
/// The transmission order of the image data.
/// </summary>
public enum InterlaceMethod : byte
{
    /// <summary>
    /// Pixels are extracted sequentially from left to right, and scanlines sequentially
    /// from top to bottom. The interlaced PNG image is a single reduced image.
    /// </summary>
    Null = 0,

    /// <summary>
    /// Defines seven distinct passes over the image. Each pass transmits a subset of the pixels in the
    /// reference image. The pass in which each pixel is transmitted (numbered from 1 to 7) is
    /// defined by replicating the following 8-by-8 pattern over the entire image, starting at the
    /// upper left corner.
    /// </summary>
    Adam7 = 1,
}