using Phaeyz.Marshalling;

namespace Phaeyz.Png;

/// <summary>
/// Computes a CRC while a <see cref="Phaeyz.Marshalling.MarshalStream"/> is being read or written to.
/// </summary>
internal class Crc32Processor : IMarshalStreamProcessor
{
    /// <summary>
    /// The algorithm to compute the CRC.
    /// </summary>
    private Crc32 _crc32 = new();

    /// <summary>
    /// Processes bytes being read or written to a <see cref="Phaeyz.Marshalling.MarshalStream"/> instance.
    /// </summary>
    /// <param name="bytes">
    /// The bytes being processed.
    /// </param>
    /// <remarks>
    /// If this method throws an exception, the state of the <see cref="Phaeyz.Marshalling.MarshalStream"/>
    /// will be corrupted and invalid.
    /// </remarks>
    void IMarshalStreamProcessor.Process(ReadOnlySpan<byte> bytes) => _crc32.Update(bytes);

    /// <summary>
    /// The current CRC value.
    /// </summary>
    public uint Value => _crc32.Value;
}