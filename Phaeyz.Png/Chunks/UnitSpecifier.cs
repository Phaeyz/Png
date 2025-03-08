namespace Phaeyz.Png.Chunks;

/// <summary>
/// Specifies the physical pixel unit specifier.
/// </summary>
public enum UnitSpecifier : byte
{
    /// <summary>
    /// Unit is unknown and unspecified.
    /// </summary>
    Unknown     = 0,

    /// <summary>
    /// Unit is pixels-per-meter.
    /// </summary>
    Meter       = 1,
}