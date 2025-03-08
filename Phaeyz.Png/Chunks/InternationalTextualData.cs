using System.Text;
using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// Stores text data that can include international characters and language information. The iTXt
/// chunk allows for more flexibility and supports text encoding in UTF-8, which can represent
/// characters from virtually any language.
/// </summary>
[Chunk("iTXt", AllowMultiple = true)]
public class InternationalTextualData : Chunk
{
    private CompressableText _text = new();

    /// <summary>
    /// Describes the type of text.
    /// </summary>
    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// If <c>true</c>, the data in text data is compressed using the compression method.
    /// </summary>
    public bool IsTextCompressed
    {
        get => _text.IsCompressed;
        set => _text.IsCompressed = value;
    }

    /// <summary>
    /// The compression method used on the text data if it is compressed.
    /// </summary>
    public CompressionMethod CompressionMethod
    {
        get => _text.CompressionMethod;
        set => _text.CompressionMethod = value;
    }

    /// <summary>
    /// Specifies the language of the text according to ISO 639-1 or 639-2 language codes.
    /// </summary>
    public string LanguageTag { get; set; } = string.Empty;

    /// <summary>
    /// Provides a translation of the keyword in the specified language.
    /// </summary>
    public string TranslatedKeyword { get; set; } = string.Empty;

    /// <summary>
    /// The text content.
    /// </summary>
    public string Text
    {
        get => _text.Text;
        set => _text.Text = value;
    }

    /// <summary>
    /// Reads the chunk from the stream, hydrating the properties of the chunk.
    /// </summary>
    /// <param name="stream">
    /// The stream to read from.
    /// </param>
    /// <param name="chunkLength">
    /// The length of the chunk data on the stream.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the chunk has been read.
    /// </returns>
    /// <remarks>
    /// It is expected that the method reads exactly <paramref name="chunkLength"/> bytes from the stream.
    /// If less or more is read, it will lead to corruption.
    /// </remarks>
    public override async ValueTask ReadFromStreamAsync(MarshalStream stream, int chunkLength, CancellationToken cancellationToken)
    {
        if (chunkLength < 5)
        {
            throw new PngException("Unexpected chunk length.");
        }

        var result = await stream.ReadStringAsync(
            Encoding.Latin1,
            chunkLength,
            MarshalStreamNullTerminatorBehavior.Stop,
            cancellationToken).ConfigureAwait(false);
        Keyword = result.Value;
        chunkLength -= result.BytesRead;
        if (chunkLength < 4)
        {
            throw new PngException("Unexpected chunk length.");
        }

        byte isTextCompressed = await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false);
        chunkLength--;

        CompressionMethod compressionMethod = (CompressionMethod)await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false);
        chunkLength--;

        result = await stream.ReadStringAsync(
            Encoding.ASCII,
            chunkLength,
            MarshalStreamNullTerminatorBehavior.Stop,
            cancellationToken).ConfigureAwait(false);
        LanguageTag = result.Value;
        chunkLength -= result.BytesRead;
        if (chunkLength < 1)
        {
            throw new PngException("Unexpected chunk length.");
        }

        result = await stream.ReadStringAsync(
            Encoding.UTF8,
            chunkLength,
            MarshalStreamNullTerminatorBehavior.Stop,
            cancellationToken).ConfigureAwait(false);
        TranslatedKeyword = result.Value;
        chunkLength -= result.BytesRead;

        _text = await CompressableText.FromStreamAsync(
            stream,
            chunkLength,
            isTextCompressed != 0,
            compressionMethod,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Validates all properties of the chunk such that the chunk is ready for serialization,
    /// and computes the data length of the chunk.
    /// </summary>
    /// <returns>
    /// The data length of the chunk. It must match exactly the number of bytes that
    /// <see cref="Phaeyz.Png.Chunk.WriteToStreamAsync"/> would write.
    /// </returns>
    /// <remarks>
    /// This method is not called if the chunk does not have length.
    /// </remarks>
    public override int ValidateAndComputeLength()
    {
        int byteCount = Keyword.Length +
            LanguageTag.Length +
            Encoding.UTF8.GetByteCount(TranslatedKeyword) +
            _text.ValidateAndComputeLength() + 5;
        return byteCount;
    }

    /// <summary>
    /// Writes the chunk to the stream.
    /// </summary>
    /// <param name="stream">
    /// The stream to write to.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the chunk has been written.
    /// </returns>
    /// <remarks>
    /// This method is not called if the chunk does not have length.
    /// </remarks>
    public override async ValueTask WriteToStreamAsync(MarshalStream stream, CancellationToken cancellationToken)
    {
        await stream.WriteStringAsync(Encoding.Latin1, Keyword, true, cancellationToken).ConfigureAwait(false);
        await stream.WriteByteAsync(IsTextCompressed ? (byte)1 : (byte)0, cancellationToken).ConfigureAwait(false);
        await stream.WriteByteAsync((byte)CompressionMethod, cancellationToken).ConfigureAwait(false);
        await stream.WriteStringAsync(Encoding.ASCII, LanguageTag, true, cancellationToken).ConfigureAwait(false);
        await stream.WriteStringAsync(Encoding.UTF8, TranslatedKeyword, true, cancellationToken).ConfigureAwait(false);
        await _text.WriteTextToStreamAsync(stream, cancellationToken).ConfigureAwait(false);
    }
}