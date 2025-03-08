using System.Text;
using Phaeyz.Marshalling;
using Phaeyz.Png.Chunks;

namespace Phaeyz.Png.Test;

internal class CompressableTextTest
{
    private const string c_testDeserializedText = "The quick brown fox jumps over the lazy dog.";

    private static readonly byte[] s_testSerializedCompressedText =
    [
        0x78, 0x9C, 0x0B, 0xC9, 0x48, 0x55, 0x28, 0x2C,
        0xCD, 0x4C, 0xCE, 0x56, 0x48, 0x2A, 0xCA, 0x2F,
        0xCF, 0x53, 0x48, 0xCB, 0xAF, 0x50, 0xC8, 0x2A,
        0xCD, 0x2D, 0x28, 0x56, 0xC8, 0x2F, 0x4B, 0x2D,
        0x52, 0x28, 0xC9, 0x48, 0x55, 0xC8, 0x49, 0xAC,
        0xAA, 0x54, 0x48, 0xC9, 0x4F, 0xD7, 0x03, 0x00,
        0x6B, 0xE4, 0x10, 0x08
    ];

    [Test]
    public async Task FromStreamAsync_CompressedSerializedBytes_DeserializesCorrectly()
    {
        CompressableText text = new(s_testSerializedCompressedText, true, CompressionMethod.Deflate);
        await Assert.That(text.Text).IsEqualTo(c_testDeserializedText);
    }

    [Test]
    public async Task FromStreamAsync_UncompressedSerializedBytes_DeserializesCorrectly()
    {
        CompressableText text = new(Encoding.UTF8.GetBytes(c_testDeserializedText), false, CompressionMethod.Deflate);
        await Assert.That(text.Text).IsEqualTo(c_testDeserializedText);
    }

    [Test]
    public async Task FromStreamAsync_RoundTripCompressedFromSerializedBytes_RoundTripsCorrectly()
    {
        CompressableText start = new(s_testSerializedCompressedText, true, CompressionMethod.Deflate);
        using MemoryStream baseStream = new();
        using (MarshalStream marshalStream = new(baseStream, false))
        {
            await start.WriteTextToStreamAsync(marshalStream).ConfigureAwait(false);
        }
        await Assert.That(baseStream.ToArray()).IsEquivalentTo(s_testSerializedCompressedText);
        baseStream.Position = 0;
        CompressableText end = await CompressableText.FromStreamAsync(
            baseStream,
            (int)baseStream.Length,
            true,
            CompressionMethod.Deflate).ConfigureAwait(false);
        await Assert.That(end.Text).IsEqualTo(c_testDeserializedText);
    }

    [Test]
    public async Task FromStreamAsync_RoundTripCompressedFromString_RoundTripsCorrectly()
    {
        CompressableText start = new()
        {
            IsCompressed = true,
            Text = c_testDeserializedText
        };
        using MemoryStream baseStream = new();
        using (MarshalStream marshalStream = new(baseStream, false))
        {
            await start.WriteTextToStreamAsync(marshalStream).ConfigureAwait(false);
        }
        await Assert.That(baseStream.ToArray()).IsEquivalentTo(s_testSerializedCompressedText);
        baseStream.Position = 0;
        CompressableText end = await CompressableText.FromStreamAsync(
            baseStream,
            (int)baseStream.Length,
            true,
            CompressionMethod.Deflate).ConfigureAwait(false);
        await Assert.That(end.Text).IsEqualTo(c_testDeserializedText);
    }

    [Test]
    public async Task FromStreamAsync_RoundTripUncompressedFromSerializedBytes_RoundTripsCorrectly()
    {
        CompressableText start = new(Encoding.UTF8.GetBytes(c_testDeserializedText), false, CompressionMethod.Deflate);
        using MemoryStream baseStream = new();
        using (MarshalStream marshalStream = new(baseStream, false))
        {
            await start.WriteTextToStreamAsync(marshalStream).ConfigureAwait(false);
        }
        await Assert.That(baseStream.ToArray()).IsEquivalentTo(Encoding.UTF8.GetBytes(c_testDeserializedText));
        baseStream.Position = 0;
        CompressableText end = await CompressableText.FromStreamAsync(
            baseStream,
            (int)baseStream.Length,
            false,
            CompressionMethod.Deflate).ConfigureAwait(false);
        await Assert.That(end.Text).IsEqualTo(c_testDeserializedText);
    }

    [Test]
    public async Task FromStreamAsync_RoundTripUncompressedFromString_RoundTripsCorrectly()
    {
        CompressableText start = new()
        {
            IsCompressed = false,
            Text = c_testDeserializedText
        };
        using MemoryStream baseStream = new();
        using (MarshalStream marshalStream = new(baseStream, false))
        {
            await start.WriteTextToStreamAsync(marshalStream).ConfigureAwait(false);
        }
        await Assert.That(baseStream.ToArray()).IsEquivalentTo(Encoding.UTF8.GetBytes(c_testDeserializedText));
        baseStream.Position = 0;
        CompressableText end = await CompressableText.FromStreamAsync(
            baseStream,
            (int)baseStream.Length,
            false,
            CompressionMethod.Deflate).ConfigureAwait(false);
        await Assert.That(end.Text).IsEqualTo(c_testDeserializedText);
    }

    [Test]
    public async Task FromStreamAsync_StartCompressedSerializedBytesChangeIsCompressed_RoundTripsCorrectly()
    {
        CompressableText start = new(s_testSerializedCompressedText, true, CompressionMethod.Deflate);
        start.IsCompressed = false;
        using MemoryStream baseStream = new();
        using (MarshalStream marshalStream = new(baseStream, false))
        {
            await start.WriteTextToStreamAsync(marshalStream).ConfigureAwait(false);
        }
        await Assert.That(baseStream.ToArray()).IsEquivalentTo(Encoding.UTF8.GetBytes(c_testDeserializedText));
        baseStream.Position = 0;
        CompressableText end = await CompressableText.FromStreamAsync(
            baseStream,
            (int)baseStream.Length,
            false,
            CompressionMethod.Deflate).ConfigureAwait(false);
        await Assert.That(end.Text).IsEqualTo(c_testDeserializedText);
    }
}