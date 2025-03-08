namespace Phaeyz.Png.Chunks;

/// <summary>
/// A PNG image type.
/// </summary>
public class PngImageType
{
    /// <summary>
    /// Greyscale.
    /// </summary>
    public static readonly PngImageType Greyscale =
        new("Greyscale", ColorType.Greyscale, new HashSet<byte> { 1, 2, 4, 8, 16 });

    /// <summary>
    /// Truecolor.
    /// </summary>
    public static readonly PngImageType Truecolor =
        new("Truecolor", ColorType.TruecolorUsed, new HashSet<byte> { 8, 16 });

    /// <summary>
    /// Indexed-color.
    /// </summary>
    public static readonly PngImageType IndexedColor =
        new("IndexedColor", ColorType.PaletteUsed | ColorType.TruecolorUsed, new HashSet<byte> { 1, 2, 4, 8 });

    /// <summary>
    /// Greyscale with alpha.
    /// </summary>
    public static readonly PngImageType GreyscaleWithAlpha =
        new("GreyscaleWithAlpha", ColorType.Greyscale | ColorType.AlphaUsed, new HashSet<byte> { 8, 16 });

    /// <summary>
    /// Truecolor with alpha.
    /// </summary>
    public static readonly PngImageType TruecolorWithAlpha =
        new("TruecolorWithAlpha", ColorType.TruecolorUsed | ColorType.AlphaUsed, new HashSet<byte> { 8, 16 });

    /// <summary>
    /// Initializes a new <see cref="PngImageType"/> instance.
    /// </summary>
    /// <param name="name">
    /// A friendly name for the image type.
    /// </param>
    /// <param name="colorType">
    /// The image color type.
    /// </param>
    /// <param name="allowedBitDepths">
    /// Allowed bit depths for the image type.
    /// </param>
    private PngImageType(string name, ColorType colorType, IReadOnlySet<byte> allowedBitDepths)
    {
        Name = name;
        ColorType = colorType;
        AllowedBitDepths = allowedBitDepths;
    }

    /// <summary>
    /// The image color type.
    /// </summary>
    public ColorType ColorType { get; private set; }

    /// <summary>
    /// Allowed bit depths for the image type.
    /// </summary>
    public IReadOnlySet<byte> AllowedBitDepths { get; private set; }

    /// <summary>
    /// A friendly name for the image type.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Iterates all possible PNG image types.
    /// </summary>
    /// <returns>
    /// An enumerable which yields all possible PNG image types.
    /// </returns>
    public static IEnumerable<PngImageType> EnumerateAll()
    {
        yield return Greyscale;
        yield return Truecolor;
        yield return IndexedColor;
        yield return GreyscaleWithAlpha;
        yield return TruecolorWithAlpha;
    }

    /// <summary>
    /// Gets the PNG image type from a color type configuration.
    /// </summary>
    /// <param name="colorType">
    /// The color type configuration to map to a PNG image type.
    /// </param>
    /// <returns>
    /// A PNG image type.
    /// </returns>
    /// <exception cref="PngException">
    /// The specified color type configuration is not valid.
    /// </exception>
    public static PngImageType FromColorType(ColorType colorType)
    {
        PngImageType? pngImageType = EnumerateAll().FirstOrDefault(o => o.ColorType == colorType);
        return pngImageType ?? throw new PngException("The PNG color type is invalid.");
    }

    /// <summary>
    /// Gets a friendly name for the image type.
    /// </summary>
    /// <returns>
    /// A friendly name for the image type.
    /// </returns>
    public override string ToString() => Name;
}