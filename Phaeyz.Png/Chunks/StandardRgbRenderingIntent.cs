namespace Phaeyz.Png.Chunks;

/// <summary>
/// The specified rendering intent defined by the International Color Consortium [ICC] or [ICC-2].
/// </summary>
public enum StandardRgbRenderingIntent : byte
{
    /// <summary>
    /// For images preferring good adaptation to the output device gamut at the expense of colorimetric
    /// accuracy, such as photographs.
    /// </summary>
    Perceptual              = 0,

    /// <summary>
    /// For images requiring color appearance matching (relative to the output device white point), such as logos.
    /// </summary>
    RelativeColorimetric    = 1,

    /// <summary>
    /// For images preferring preservation of saturation at the expense of hue and lightness, such as charts and graphs.
    /// </summary>
    Saturation              = 2,

    /// <summary>
    /// For images requiring preservation of absolute colorimetry, such as previews of images destined for a different
    /// output device (proofs).
    /// </summary>
    AbsoluteColorimetric    = 3,
}