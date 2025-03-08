namespace Phaeyz.Png.Chunks;

/// <summary>
/// The color types used in the PNG image.
/// </summary>
[Flags]
public enum ColorType
{
    /// <summary>
    /// Greyscale.
    /// </summary>
    Greyscale       = 0,

    /// <summary>
    /// Palette used.
    /// </summary>
    PaletteUsed     = 1,

    /// <summary>
    /// Truecolor used.
    /// </summary>
    TruecolorUsed   = 2,

    /// <summary>
    /// Alpha used.
    /// </summary>
    AlphaUsed       = 4,
}