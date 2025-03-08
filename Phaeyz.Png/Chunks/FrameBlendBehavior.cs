namespace Phaeyz.Png.Chunks;

/// <summary>
/// Specifies whether the frame is to be alpha blended into the current output buffer content,
/// or whether it should completely replace its region in the output buffer.
/// </summary>
/// <remarks>
/// For the first frame, the two blend modes are functionally equivalent due to the clearing of
/// the output buffer at the beginning of each play.
/// </remarks>
public enum FrameBlendBehavior : byte
{
    /// <summary>
    /// All color components of the frame, including alpha, overwrite the current contents of the
    /// frame's output buffer region.
    /// </summary>
    /// <remarks>
    /// APNG_BLEND_OP_SOURCE
    /// </remarks>
    Overwrite       = 0,

    /// <summary>
    /// The frame should be composited onto the output buffer based on its alpha, using a simple "OVER" operation as described in
    /// (<a href="https://www.w3.org/TR/png-3/#13Alpha-channel-processing">Alpha Channel Processing</a>). Note that the second
    /// variation of the sample code is applicable.
    /// </summary>
    /// <remarks>
    /// APNG_BLEND_OP_OVER
    /// </remarks>
    AlphaBlend = 1,
}