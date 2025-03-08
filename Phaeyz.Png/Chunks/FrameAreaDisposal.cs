namespace Phaeyz.Png.Chunks;

/// <summary>
/// The type of frame area disposal to be done after rendering this frame; in other words,
/// it specifies how the output buffer should be changed at the end of the delay (before rendering
/// the next frame).
/// </summary>
public enum FrameAreaDisposal : byte
{
    /// <summary>
    /// No disposal is done on this frame before rendering the next; the contents of the output buffer are left as is.
    /// </summary>
    /// <remarks>
    /// APNG_DISPOSE_OP_NONE
    /// </remarks>
    None        = 0,

    /// <summary>
    /// The frame's region of the output buffer is to be cleared to fully transparent black before rendering the next frame.
    /// </summary>
    /// <remarks>
    /// APNG_DISPOSE_OP_BACKGROUND
    /// </remarks>
    Background  = 1,

    /// <summary>
    /// The frame's region of the output buffer is to be reverted to the previous contents before rendering the next frame.
    /// </summary>
    /// <remarks>
    /// APNG_DISPOSE_OP_PREVIOUS
    /// </remarks>
    Previous    = 2,
}