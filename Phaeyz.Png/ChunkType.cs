using System.Diagnostics.CodeAnalysis;

namespace Phaeyz.Png;

/// <summary>
/// The type of a chunk.
/// </summary>
public class ChunkType : IEquatable<ChunkType>
{
    /// <summary>
    /// Image header, which is the first chunk in a PNG datastream.
    /// </summary>
    public static readonly ChunkType ImageHeader = "IHDR";

    /// <summary>
    /// Palette table associated with indexed PNG images.
    /// </summary>
    public static readonly ChunkType Palette = "PLTE";

    /// <summary>
    /// Image data chunks.
    /// </summary>
    public static readonly ChunkType ImageData = "IDAT";

    /// <summary>
    /// Image trailer, which is the last chunk in a PNG datastream.
    /// </summary>
    public static readonly ChunkType ImageEnd = "IEND";

    /// <summary>
    /// Transparency.
    /// </summary>
    public static readonly ChunkType Transparency = "tRNS";

    /// <summary>
    /// Primary chromaticities and white point.
    /// </summary>
    public static readonly ChunkType Chromaticities = "cHRM";

    /// <summary>
    /// Image gamma.
    /// </summary>
    public static readonly ChunkType Gamma = "gAMA";

    /// <summary>
    /// Embedded ICC profile.
    /// </summary>
    public static readonly ChunkType IccProfile = "iCCP";

    /// <summary>
    /// Significant bits.
    /// </summary>
    public static readonly ChunkType SignificantBits = "sBIT";

    /// <summary>
    /// Standard RGB color space.
    /// </summary>
    public static readonly ChunkType RgbRendering = "sRGB";

    /// <summary>
    /// Coding-independent code points for video signal type identification.
    /// </summary>
    public static readonly ChunkType CodingIndependentCodePoints = "cICP";

    /// <summary>
    /// Mastering Display Color Volume.
    /// </summary>
    public static readonly ChunkType MasteringDisplayColorVolume = "mDCV";

    /// <summary>
    /// International textual data.
    /// </summary>
    public static readonly ChunkType InternationalTextualData = "iTXt";

    /// <summary>
    /// Textual data.
    /// </summary>
    public static readonly ChunkType TextualData = "tEXt";

    /// <summary>
    /// Compressed textual data.
    /// </summary>
    public static readonly ChunkType CompressedTextualData = "zTXt";

    /// <summary>
    /// Background color.
    /// </summary>
    public static readonly ChunkType BackgroundColor = "bKGD";

    /// <summary>
    /// Image histogram.
    /// </summary>
    public static readonly ChunkType Histogram = "hIST";

    /// <summary>
    /// Physical pixel dimensions.
    /// </summary>
    public static readonly ChunkType PhysicalPixelDimensions = "pHYs";

    /// <summary>
    /// Suggested palette.
    /// </summary>
    public static readonly ChunkType SuggestedPalette = "sPLT";

    /// <summary>
    /// Exchangeable Image File (EXIF) Profile.
    /// </summary>
    public static readonly ChunkType Exif = "eXIf";

    /// <summary>
    /// Image last-modification time.
    /// </summary>
    public static readonly ChunkType LastModificationTime = "tIME";

    /// <summary>
    /// Animation Control Chunk.
    /// </summary>
    public static readonly ChunkType AnimationControlChunk = "acTL";

    /// <summary>
    /// Frame Control Chunk.
    /// </summary>
    public static readonly ChunkType FrameControl = "fcTL";

    /// <summary>
    /// Frame Data Chunk.
    /// </summary>
    public static readonly ChunkType FrameData = "fdAT";

    /// <summary>
    /// Creates a new <see cref="Phaeyz.Png.ChunkType"/> instance using a chunk type value.
    /// </summary>
    /// <param name="value">
    /// The chunk type value.
    /// </param>
    /// <exception cref="ArgumentException">
    /// The chunk type value is not a valid compliant chunk type value.
    /// </exception>
    public ChunkType(uint value)
    {
        if (!ValidateChar((char)(byte)((value >> 24) & 0xFF), out char c1))
        {
            throw new ArgumentException("Invalid chunk type value.", nameof(value));
        }
        if (!ValidateChar((char)(byte)((value >> 16) & 0xFF), out char c2))
        {
            throw new ArgumentException("Invalid chunk type value.", nameof(value));
        }
        if (!ValidateChar((char)(byte)((value >> 8) & 0xFF), out char c3))
        {
            throw new ArgumentException("Invalid chunk type value.", nameof(value));
        }
        if (!ValidateChar((char)(byte)(value & 0xFF), out char c4))
        {
            throw new ArgumentException("Invalid chunk type value.", nameof(value));
        }
        Value = value;
        Label = $"{c1}{c2}{c3}{c4}";
    }

    /// <summary>
    /// Creates a new <see cref="Phaeyz.Png.ChunkType"/> instance using a chunk type label.
    /// </summary>
    /// <param name="label">
    /// The chunk type label.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The chunk type label is null or empty.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The chunk type label is not a valid compliant chunk type label.
    /// </exception>
    public ChunkType(string label)
    {
        if (string.IsNullOrEmpty(label))
        {
            throw new ArgumentNullException(nameof(label));
        }

        if (label.Length != 4)
        {
            throw new ArgumentException("Invalid chunk type label.", nameof(label));
        }
        if (!ValidateChar(label[0], out char c1))
        {
            throw new ArgumentException("Invalid chunk type label.", nameof(label));
        }
        if (!ValidateChar(label[1], out char c2))
        {
            throw new ArgumentException("Invalid chunk type label.", nameof(label));
        }
        if (!ValidateChar(label[2], out char c3))
        {
            throw new ArgumentException("Invalid chunk type label.", nameof(label));
        }
        if (!ValidateChar(label[3], out char c4))
        {
            throw new ArgumentException("Invalid chunk type label.", nameof(label));
        }

        Value = ((uint)c1 << 24) | ((uint)c2 << 16) | ((uint)c3 << 8) | c4;
        Label = label;
    }

    /// <summary>
    /// Gets whether the chunk is ancillary or critical.
    /// </summary>
    /// <remarks>
    /// Critical chunks are necessary for successful display of the contents of the datastream,
    /// for example the image header chunk (IHDR). A decoder trying to extract the image, upon
    /// encountering an unknown chunk type in which is not ancillary (critical), shall indicate
    /// to the user that the image contains information it cannot safely interpret.
    /// <br/>
    /// Ancillary chunks are not strictly necessary in order to meaningfully display the contents of
    /// the datastream, for example the time chunk (tIME). A decoder encountering an unknown chunk type
    /// which is ancillary can safely ignore the chunk and proceed to display the image.
    /// </remarks>
    public bool IsAncillary => (Value & (1u << 29)) != 0;

    /// <summary>
    /// Gets whether or not the chunk is a private chunk.
    /// </summary>
    /// <remarks>
    /// Public chunks are reserved for definition by the W3C. Encoders may use private chunks to carry
    /// information that need not be understood by other applications.
    /// </remarks>
    public bool IsPrivate => (Value & (1u << 21)) != 0;

    /// <summary>
    /// This flag is reserved for future use of the PNG specification.
    /// </summary>
    /// <remarks>
    /// PNGs containing chunks with a reserved chunk type currently do not conform to the PNG specification.
    /// </remarks>
    public bool IsReserved => (Value & (1u << 13)) != 0;

    /// <summary>
    /// Indicates the chunk may be safely copied to a new PNG data stream even if the image data is changed
    /// and the encoder does not understand this chunk type.
    /// </summary>
    /// <remarks>
    /// If this value is <c>false</c> it indicates that if the image data is changed, it will impact any
    /// data within the chunk, and therefore the chunk should not be blindly copied to a new PNG data stream.
    /// If an encoder understands this chunk, on the other hand, it is free to make necessary modifications
    /// and copy the chunk.
    /// </remarks>
    public bool IsSafeToCopy => (Value & (1u << 5)) != 0;

    /// <summary>
    /// The chunk type label.
    /// </summary>
    public string Label { get; private set; }

    /// <summary>
    /// The chunk type value.
    /// </summary>
    public uint Value { get; private set; }

    /// <summary>
    /// Tests two chunk types for equality.
    /// </summary>
    /// <param name="other">
    /// The other chunk type to compare.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the chunk types are the same; <c>false</c> otherwise.
    /// </returns>
    public bool Equals(ChunkType? other) => other is not null && other.Value == Value;

    /// <summary>
    /// Tests two chunk types for equality.
    /// </summary>
    /// <param name="obj">
    /// The other chunk type to compare.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the chunk types are the same; <c>false</c> otherwise.
    /// </returns>
    public override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as ChunkType);

    /// <summary>
    /// Gets a hash code for the current instance.
    /// </summary>
    /// <returns>
    /// A hash code for the current instance.
    /// </returns>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Gets the chunk type label.
    /// </summary>
    /// <returns>
    /// The chunk type label.
    /// </returns>
    public override string ToString() => Label;

    /// <summary>
    /// Validates an individual byte of the chunk type value.
    /// </summary>
    /// <param name="c">
    /// The input byte.
    /// </param>
    /// <param name="d">
    /// On output, receives the input byte.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if byte is valid; <c>false</c> otherwise.
    /// </returns>
    private static bool ValidateChar(char c, out char d)
    {
        if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
        {
            d = c;
            return true;
        }
        d = '\0';
        return false;
    }

    /// <summary>
    /// Tests two chunk types for equality.
    /// </summary>
    /// <param name="left">
    /// The first chunk type.
    /// </param>
    /// <param name="right">
    /// The second chunk type.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the chunk types are equal; <c>false</c> otherwise.
    /// </returns>
    public static bool operator ==(ChunkType left, ChunkType right) => left.Equals(right);

    /// <summary>
    /// Tests two chunk types for inequality.
    /// </summary>
    /// <param name="left">
    /// The first chunk type.
    /// </param>
    /// <param name="right">
    /// The second chunk type.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the chunk types are not equal; <c>false</c> otherwise.
    /// </returns>
    public static bool operator !=(ChunkType left, ChunkType right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts a chunk type to a label.
    /// </summary>
    /// <param name="chunkType">
    /// The chunk type to convert to a label.
    /// </param>
    public static implicit operator string(ChunkType chunkType) => chunkType.Label;

    /// <summary>
    /// Implicitly converts a label to a chunk type.
    /// </summary>
    /// <param name="label">
    /// The label to convert to a chunk type.
    /// </param>
    public static implicit operator ChunkType(string label) => new(label);
}