using System.Buffers;
using System.IO.Compression;
using System.Text;
using Phaeyz.Marshalling;
using Phaeyz.Png.Chunks;

namespace Phaeyz.Png;

/// <summary>
/// Supports a delayed deserialization of non-null terminated and potentially compressed UTF-8 text within chunks.
/// Also supports efficiently reserializing.
/// </summary>
/// <remarks>
/// If the text has embedded NIL characters, the NIL and any text afterward will be ignored when writing to a stream.
/// </remarks>
public class CompressableText
{
    // The zlib header used by this class for DeflateStream and normal compression. The header
    // is mostly informational and not necessary for DeflateStream to deserialize, so the header
    // values are only used for serialization. The only important value used for deserialization
    // is the compression method used, but if it is not deflate, DeflateStream will fail anyway.
    private static readonly byte[] s_zlibHeader = [0x78, 0x9C];
    // A CRC is at the end of the zlib deflate stream.
    private const int c_crcLengthInBytes = sizeof(uint);

    private bool _deserialized = false;
    private byte[]? _serializedText = null;
    private string _text = string.Empty;
    private bool _isCompressed = false;
    private CompressionMethod _compressionMethod = CompressionMethod.Deflate;

    /// <summary>
    /// Initializes a new <see cref="CompressableText"/> instance.
    /// </summary>
    public CompressableText() { }

    /// <summary>
    /// Initializes a new <see cref="CompressableText"/> instance.
    /// </summary>
    /// <param name="serializedText">
    /// The initial value for serialized text. It is only deserialized if the user requests the raw string.
    /// </param>
    /// <param name="isCompressed">
    /// Indicates whether or not the text should be compressed when serialized.
    /// </param>
    /// <param name="compressionMethod">
    /// The compression method.
    /// </param>
    public CompressableText(byte[] serializedText, bool isCompressed, CompressionMethod compressionMethod)
    {
        _serializedText = serializedText;
        _isCompressed = isCompressed;
        _compressionMethod = compressionMethod;
    }

    /// <summary>
    /// The compression method.
    /// </summary>
    public CompressionMethod CompressionMethod
    {
        get => _compressionMethod;
        set
        {
            if (_compressionMethod != value)
            {
                if (_isCompressed)
                {
                    Deserialize();
                    _serializedText = null;
                }
                _compressionMethod = value;
            }
        }
    }

    /// <summary>
    /// Indicates whether or not the text should be compressed when serialized.
    /// </summary>
    public bool IsCompressed
    {
        get => _isCompressed;
        set
        {
            if (_isCompressed != value)
            {
                Deserialize();
                _serializedText = null;
                _isCompressed = value;
            }
        }
    }

    /// <summary>
    /// The text field.
    /// </summary>
    public string Text
    {
        get
        {
            Deserialize();
            return _text;
        }
        set
        {
            if (!ReferenceEquals(value, _text))
            {
                _text = value;
                _serializedText = null;
            }
        }
    }

    /// <summary>
    /// Deserializes the serialized text if it has not been deserialized yet.
    /// </summary>
    /// <exception cref="PngException">
    /// The compression method value is invalid, or the CRC is invalid.
    /// </exception>
    private void Deserialize()
    {
        // If the serialized text is null, or deserialization has already occurred, no text data is available for deserialization.
        if (_serializedText is null || _deserialized)
        {
            return;
        }

        // Default to empty string for empty buffers, even for compressed.
        if (_serializedText.Length == 0 )
        {
            _text = string.Empty;
            _deserialized = true;
            return;
        }

        // If the text data is not compressed, the UTF-8 string can be ready right from the buffer.
        if (!_isCompressed)
        {
            _text = Encoding.UTF8.GetNullTerminatedString(_serializedText, out _);
            _deserialized = true;
            return;
        }

        // Validate the compression method flag.
        if (_compressionMethod != CompressionMethod.Deflate)
        {
            throw new PngException("CompressionMethod has an unexpected value.");
        }

        // The compressed data must be at least s_zlibHeader.Length + crcLengthInBytes bytes long.
        if (_serializedText.Length < s_zlibHeader.Length + c_crcLengthInBytes)
        {
            throw new PngException("Invalid compressed data length.");
        }

        // Decompress into a memory stream.
        using MemoryStream sourceMemoryStream = new(_serializedText);
        sourceMemoryStream.Position = s_zlibHeader.Length;
        using ScopedReadStream scopedReadStream = new(sourceMemoryStream, _serializedText.Length - s_zlibHeader.Length - c_crcLengthInBytes);
        using MemoryStream targetMemoryStream = new(_serializedText.Length * 2);
        using (DeflateStream decompressionStream = new(scopedReadStream, CompressionMode.Decompress, true))
        {
            decompressionStream.CopyTo(targetMemoryStream);
        }
        byte[] buffer = targetMemoryStream.ToArray();

        // Validate the CRC
        uint crcExpectedValue = ByteConverter.BigEndian.ToUInt32(_serializedText.AsSpan(^c_crcLengthInBytes..));
        uint crcActualValue = ComputeAdler32(buffer);
        if (crcActualValue != crcExpectedValue)
        {
            throw new PngException("The text CRC value is invalid.");
        }

        // Read the string from the memory stream.
        _text = Encoding.UTF8.GetNullTerminatedString(buffer, out _);
        _deserialized = true;
    }

    /// <summary>
    /// Reads a specific number of bytes from a stream and assigns to a
    /// new <see cref="CompressableText"/> instance.
    /// </summary>
    /// <param name="stream">
    /// The stream to read bytes from.
    /// </param>
    /// <param name="byteCount">
    /// The number of bytes to read from the stream.
    /// </param>
    /// <param name="isCompressed">
    /// Determines whether or not the string within the stream is compressed.
    /// </param>
    /// <param name="compressionMethod">
    /// Determines the compression method of the compressed string within the stream.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding a new <see cref="CompressableText"/> instance.
    /// </returns>
    public async static Task<CompressableText> FromStreamAsync(
        Stream stream,
        int byteCount,
        bool isCompressed,
        CompressionMethod compressionMethod,
        CancellationToken cancellationToken = default)
    {
        if (byteCount == 0)
        {
            return new CompressableText
            {
                IsCompressed = isCompressed,
                CompressionMethod = compressionMethod,
            };
        }

        byte[] buffer = new byte[byteCount];
        await stream.ReadExactlyAsync(buffer, 0, byteCount, cancellationToken).ConfigureAwait(false);

        return new CompressableText(buffer, isCompressed, compressionMethod);
    }

    /// <summary>
    /// Serializes the text into a byte array if it has not been serialized yet.
    /// </summary>
    /// <exception cref="Phaeyz.Png.PngException">
    /// The compression method value is invalid.
    /// </exception>
    private void Serialize()
    {
        // Only serialize if needed.
        if (_serializedText is not null)
        {
            return;
        }

        // Get the UTF-8 bytes.
        int nilIndex = _text.IndexOf('\0');
        ReadOnlySequence<char> charsToWrite = new(nilIndex == -1 ? _text.AsMemory() : _text.AsMemory(..nilIndex));
        byte[] utf8Bytes = charsToWrite.Length > 0 ? Encoding.UTF8.GetBytes(charsToWrite) : [];

        // If the text is not compressed, simply write the text string to the stream.
        if (!_isCompressed)
        {
            _serializedText = utf8Bytes;
            // The serialized text now matches the text, so we can consider it deserialized as well.
            _deserialized = true;
            return;
        }

        // Validate the compression flag.
        if (_compressionMethod != CompressionMethod.Deflate)
        {
            throw new PngException("CompressionMethod has an unexpected value.");
        }

        // Create a stream where the zlib deflate stream shall be written.
        const int excessBufferSize = 64; // Short text may be longer when compressed, account for that.
        using MemoryStream memoryStream = new(s_zlibHeader.Length + utf8Bytes.Length + excessBufferSize + c_crcLengthInBytes);

        // Write the zlib header.
        memoryStream.Write(s_zlibHeader);

        // Write the compressed the text to the stream.
        using (DeflateStream compressionStream = new(memoryStream, CompressionMode.Compress, true))
        {
            compressionStream.Write(utf8Bytes);
        }

        // Write the CRC
        uint crcValue = ComputeAdler32(utf8Bytes);
        memoryStream.WriteUInt32(crcValue, ByteConverter.BigEndian);

        // Store the resulting buffer.
        _serializedText = memoryStream.ToArray();
        // The serialized text now matches the text, so we can consider it deserialized as well.
        _deserialized = true;
    }

    /// <summary>
    /// Validates the text is ready for serialization, and computes the data length of the chunk.
    /// </summary>
    /// <returns>
    /// The data length of the text.
    /// </returns>
    /// <exception cref="Phaeyz.Png.PngException">
    /// The compression method value is invalid.
    /// </exception>
    public int ValidateAndComputeLength()
    {
        Serialize();
        return _serializedText!.Length;
    }

    /// <summary>
    /// Writes the text data to the stream.
    /// </summary>
    /// <param name="stream">
    /// The stream to write to.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the text has been written.
    /// </returns>
    /// <exception cref="Phaeyz.Png.PngException">
    /// The compression method value is invalid.
    /// </exception>
    public async ValueTask WriteTextToStreamAsync(MarshalStream stream, CancellationToken cancellationToken = default)
    {
        Serialize();
        await stream.WriteAsync(_serializedText, cancellationToken).ConfigureAwait(false);
    }


    /// <summary>
    /// Computes the CRC persisted after a deflate stream.
    /// </summary>
    /// <param name="utf8Bytes">
    /// The UTF-8 bytes to compute a CRC for.
    /// </param>
    /// <returns>
    /// The text CRC.
    /// </returns>
    private static uint ComputeAdler32(ReadOnlySpan<byte> utf8Bytes)
    {
        const uint modAdler = 65521;
        uint a = 1, b = 0;
        for (int i = 0; i < utf8Bytes.Length; i++)
        {
            a = (a + utf8Bytes[i]) % modAdler;
            b = (b + a) % modAdler;
        }
        return (b << 16) | a;
    }
}