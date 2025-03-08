namespace Phaeyz.Png.Chunks;

/// <summary>
/// An entry within a <see cref="Phaeyz.Png.Chunks.SuggestedPalette"/> chunk.
/// </summary>
public struct SuggestedPaletteEntry
{
    /// <summary>
    /// The size of this struct when the sPLT sample depth is <c>8</c>.
    /// </summary>
    public const int StructSizeForSampleDepth8 = 6;

    /// <summary>
    /// The size of this struct when the sPLT sample depth is <c>16</c>.
    /// </summary>
    public const int StructSizeForSampleDepth16 = 10;

    /// <summary>
    /// Red.
    /// </summary>
    /// <remarks>
    /// The minimum allowed value is <c>0</c>. The maximum allowed value is <c>255</c> (when the sPLT
    /// sample depth is <c>8</c>) or <c>65535</c> (when the sPLT sample depth is <c>16</c>).
    /// Values are not pre-multiplied by alpha, nor are they pre-composited against any background.
    /// </remarks>
    public ushort Red { get; set; }

    /// <summary>
    /// Green.
    /// </summary>
    /// <remarks>
    /// The minimum allowed value is <c>0</c>. The maximum allowed value is <c>255</c> (when the sPLT
    /// sample depth is <c>8</c>) or <c>65535</c> (when the sPLT sample depth is <c>16</c>).
    /// Values are not pre-multiplied by alpha, nor are they pre-composited against any background.
    /// </remarks>
    public ushort Green { get; set; }

    /// <summary>
    /// Blue.
    /// </summary>
    /// <remarks>
    /// The minimum allowed value is <c>0</c>. The maximum allowed value is <c>255</c> (when the sPLT
    /// sample depth is <c>8</c>) or <c>65535</c> (when the sPLT sample depth is <c>16</c>).
    /// Values are not pre-multiplied by alpha, nor are they pre-composited against any background.
    /// </remarks>
    public ushort Blue { get; set; }

    /// <summary>
    /// Alpha.
    /// </summary>
    /// <remarks>
    /// An alpha value of <c>0</c> means fully transparent. An alpha value of <c>255</c> (when the sPLT
    /// sample depth is <c>8</c>) or <c>65535</c> (when the sPLT sample depth is <c>16</c>) means fully opaque.
    /// </remarks>
    public ushort Alpha { get; set; }

    /// <summary>
    /// The frequency value indicates the relative importance or frequency of use of each color in the image.
    /// Higher frequency values suggest that the color is used more frequently.
    /// </summary>
    /// <remarks>
    /// Each frequency value is proportional to the fraction of the pixels in the image for which that palette
    /// entry is the closest match in RGBA space, before the image has been composited against any background.
    /// The exact scale factor is chosen by the PNG encoder; it is recommended that the resulting range of
    /// individual values reasonably fills the range <c>0</c> to <c>65535</c>. A PNG encoder may artificially
    /// inflate the frequencies for colors considered to be "important", for example the colors used in a
    /// logo or the facial features of a portrait. Zero is a valid frequency meaning that the color is "least
    /// important" or that it is rarely, if ever, used. When all the frequencies are zero, they are
    /// meaningless, that is to say, nothing may be inferred about the actual frequencies with which the
    /// colors appear in the PNG image.
    /// </remarks>
    public ushort Frequency { get; set; }
}