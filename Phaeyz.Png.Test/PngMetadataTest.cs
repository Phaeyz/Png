using Phaeyz.Marshalling;
using Phaeyz.Png.Chunks;
using TUnit.Assertions.AssertConditions.Throws;

namespace Phaeyz.Png.Test;

internal class PngMetadataTest
{
    [Test]
    public async Task FindAll_MultipleMatching_MatchesYielded()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageData());
        pngMetadata.Chunks.Add(new ImageData());
        List<ImageData> chunks = pngMetadata.FindAll<ImageData>().ToList();
        await Assert.That(chunks.Count).IsEqualTo(2);
    }

    [Test]
    public async Task FindAll_NoMatching_EmptyEnumerable()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        List<Gamma> chunks = pngMetadata.FindAll<Gamma>().ToList();
        await Assert.That(chunks).IsEmpty();
    }

    [Test]
    public async Task FindFirst_MultipleMatching_FirstReturned()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageData { Data = new byte[1] });
        pngMetadata.Chunks.Add(new ImageData { Data = new byte[2] });
        pngMetadata.Chunks.Add(new ImageData { Data = new byte[3] });
        ImageData? chunk = pngMetadata.FindFirst<ImageData>();
        await Assert.That(chunk?.Data.Length).IsEqualTo(1);
    }

    [Test]
    public async Task FindFirst_MultipleMatching_FirstReturnedWithCorrectIndex()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageData { Data = new byte[1] });
        pngMetadata.Chunks.Add(new ImageData { Data = new byte[2] });
        pngMetadata.Chunks.Add(new ImageData { Data = new byte[3] });
        ImageData? chunk = pngMetadata.FindFirst<ImageData>(out int index);
        await Assert.That(chunk?.Data.Length).IsEqualTo(1);
        await Assert.That(index).IsEqualTo(2);
    }

    [Test]
    public async Task FindFirst_NoMatching_NullReturned()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        Gamma? chunk = pngMetadata.FindFirst<Gamma>();
        await Assert.That(chunk).IsNull();
    }

    [Test]
    public async Task FindFirstIndex_MultipleMatching_FirstCorrectIndexReturned()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageData());
        pngMetadata.Chunks.Add(new ImageData());
        pngMetadata.Chunks.Add(new ImageData());
        int index = pngMetadata.FindFirstIndex(ChunkType.ImageData);
        await Assert.That(index).IsEqualTo(2);
    }

    [Test]
    public async Task FindFirstIndex_NoMatching_NegativeOneReturned()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        int index = pngMetadata.FindFirstIndex(ChunkType.ImageData);
        await Assert.That(index).IsEqualTo(-1);
    }

    [Test]
    public async Task GetFirstOrCreate_CreateSegment_SegmentCreated()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageEnd());
        ImageData chunk = pngMetadata.GetFirstOrCreate<ImageData>(
            false,
            out bool created,
            out int index,
            ChunkType.ImageHeader,
            ChunkType.Gamma,
            ChunkType.FrameData);
        await Assert.That(created).IsEqualTo(true);
        await Assert.That(index).IsEqualTo(2);
        await Assert.That(pngMetadata.Chunks.IndexOf(chunk)).IsEqualTo(2);
    }

    [Test]
    public async Task GetFirstOrCreate_ExistingSegmentInProperLocation_SegmentNotMoved()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        ImageData chunk1 = new();
        pngMetadata.Chunks.Add(chunk1);
        pngMetadata.Chunks.Add(new ImageEnd());
        ImageData chunk2 = pngMetadata.GetFirstOrCreate<ImageData>(
            false,
            out bool created,
            out int index,
            ChunkType.ImageHeader,
            ChunkType.Gamma,
            ChunkType.FrameData);
        await Assert.That(created).IsEqualTo(false);
        await Assert.That(index).IsEqualTo(2);

        await Assert.That(pngMetadata.Chunks.IndexOf(chunk1)).IsEqualTo(2);
        await Assert.That(ReferenceEquals(chunk1, chunk2)).IsTrue();
    }

    [Test]
    public async Task GetFirstOrCreate_ExistingSegmentInWrongLocation_SegmentNotMoved()
    {
        PngMetadata pngMetadata = new();
        ImageData chunk1 = new();
        pngMetadata.Chunks.Add(chunk1);
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageEnd());
        ImageData chunk2 = pngMetadata.GetFirstOrCreate<ImageData>(
            false,
            out bool created,
            out int index,
            ChunkType.ImageHeader,
            ChunkType.Gamma,
            ChunkType.FrameData);
        await Assert.That(created).IsEqualTo(false);
        await Assert.That(index).IsEqualTo(0);
        await Assert.That(pngMetadata.Chunks.IndexOf(chunk1)).IsEqualTo(0);
        await Assert.That(ReferenceEquals(chunk1, chunk2)).IsTrue();
    }

    [Test]
    public async Task GetFirstOrCreate_ExistingSegmentInWrongLocation_SegmentMoved()
    {
        PngMetadata pngMetadata = new();
        ImageData chunk1 = new();
        pngMetadata.Chunks.Add(chunk1);
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageEnd());
        ImageData chunk2 = pngMetadata.GetFirstOrCreate<ImageData>(
            true,
            out bool created,
            out int index,
            ChunkType.ImageHeader,
            ChunkType.Gamma,
            ChunkType.FrameData);
        await Assert.That(created).IsEqualTo(false);
        await Assert.That(index).IsEqualTo(2);
        await Assert.That(pngMetadata.Chunks.IndexOf(chunk1)).IsEqualTo(2);
        await Assert.That(ReferenceEquals(chunk1, chunk2)).IsTrue();
    }

    [Test]
    public async Task GetIndexAfter_EmptyList_ReturnsZero()
    {
        PngMetadata pngMetadata = new();
        int index = pngMetadata.GetIndexAfter(ChunkType.ImageHeader, ChunkType.Gamma);
        await Assert.That(index).IsEqualTo(0);
    }

    [Test]
    public async Task GetIndexAfter_EndOfList_ReturnsListLength()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageEnd());
        int index = pngMetadata.GetIndexAfter(ChunkType.ImageEnd);
        await Assert.That(index).IsEqualTo(3);
    }

    [Test]
    public async Task GetIndexAfter_Middle_ReturnsOne()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageEnd());
        int index = pngMetadata.GetIndexAfter(ChunkType.ImageHeader, ChunkType.Gamma);
        await Assert.That(index).IsEqualTo(1);
    }

    [Test]
    public async Task GetIndexAfter_NotFound_ReturnsZero()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageEnd());
        int index = pngMetadata.GetIndexAfter(ChunkType.ImageData);
        await Assert.That(index).IsEqualTo(0);
    }

    [Test]
    public async Task Insert_EmptyList_ReturnsZero()
    {
        PngMetadata pngMetadata = new();
        int index = pngMetadata.Insert(new FrameData(), ChunkType.ImageHeader, ChunkType.Gamma);
        await Assert.That(index).IsEqualTo(0);
    }

    [Test]
    public async Task Insert_EndOfList_ReturnsListLength()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageEnd());
        int index = pngMetadata.Insert(new ImageData(), ChunkType.ImageEnd);
        await Assert.That(index).IsEqualTo(3);
    }

    [Test]
    public async Task Insert_Middle_ReturnsOne()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageEnd());
        int index = pngMetadata.Insert(new ImageData(), ChunkType.ImageHeader, ChunkType.Gamma);
        await Assert.That(index).IsEqualTo(1);
    }

    [Test]
    public async Task Insert_NotFound_ReturnsZero()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new Gamma());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageEnd());
        int index = pngMetadata.Insert(new ImageData(), ChunkType.ImageData);
        await Assert.That(index).IsEqualTo(0);
    }

    [Test]
    public async Task ReadAllFromStreamAsync_HasUnknownChunk_RoundTripsCorrectly()
    {
        byte[] imageBytes =
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG Signature

            0x00, 0x00, 0x00, 0x0D, // Length (13)
            0x49, 0x48, 0x44, 0x52, // Type (ImageHeader)
            0x00, 0x00, 0x00, 0x0A, // Width
            0x00, 0x00, 0x00, 0x0B, // Height
            0x08,                   // BitDepth
            0x02,                   // ColorType
            0x00,                   // CompressionMethod
            0x00,                   // FilterMethod
            0x01,                   // InterlaceMethod
            0xBE, 0x0B, 0xBB, 0xD9, // CRC

            0x00, 0x00, 0x00, 0x06, // Length
            0x74, 0x65, 0x73, 0x74, // Type (test)
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, // Data
            0x00, 0x10, 0x3D, 0xB3, // CRC

            0x00, 0x00, 0x00, 0x00, // Length (0)
            0x49, 0x45, 0x4E, 0x44, // Type (ImageEnd)
            0xAE, 0x42, 0x60, 0x82, // CRC
        ];

        using MarshalStream marshalStream = new(imageBytes);
        PngMetadata? pngMetadata = await PngMetadata.ReadFromStreamAsync(marshalStream);
        using MemoryStream memoryStream = new();
        using (MarshalStream targetStream = new(memoryStream, false))
        {
            await pngMetadata!.WriteToStreamAsync(targetStream);
        }
        byte[] testBytes = memoryStream.ToArray();
        await Assert.That(imageBytes.SequenceEqual(testBytes)).IsTrue();
    }

    [Test]
    public async Task ReadAllFromStreamAsync_MultipleMetadatasMultipleChunks_DeserializedCorrectly()
    {
        byte[] imageBytes =
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG Signature

            0x00, 0x00, 0x00, 0x0D, // Length (13)
            0x49, 0x48, 0x44, 0x52, // Type (ImageHeader)
            0x00, 0x00, 0x00, 0x0A, // Width
            0x00, 0x00, 0x00, 0x0B, // Height
            0x08,                   // BitDepth
            0x02,                   // ColorType
            0x00,                   // CompressionMethod
            0x00,                   // FilterMethod
            0x01,                   // InterlaceMethod
            0xBE, 0x0B, 0xBB, 0xD9, // CRC

            0x00, 0x00, 0x00, 0x00, // Length (0)
            0x49, 0x45, 0x4E, 0x44, // Type (ImageEnd)
            0xAE, 0x42, 0x60, 0x82, // CRC

            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG Signature

            0x00, 0x00, 0x00, 0x0D, // Length (13)
            0x49, 0x48, 0x44, 0x52, // Type (ImageHeader)
            0x00, 0x00, 0x00, 0x0C, // Width
            0x00, 0x00, 0x00, 0x0D, // Height
            0x08,                   // BitDepth
            0x02,                   // ColorType
            0x00,                   // CompressionMethod
            0x00,                   // FilterMethod
            0x01,                   // InterlaceMethod
            0x65, 0x4C, 0x28, 0x83, // CRC

            0x00, 0x00, 0x00, 0x00, // Length (0)
            0x49, 0x45, 0x4E, 0x44, // Type (ImageEnd)
            0xAE, 0x42, 0x60, 0x82  // CRC
        ];

        using MarshalStream marshalStream = new(imageBytes);
        List<PngMetadata> pngMetadataList = await PngMetadata.ReadAllFromStreamAsync(marshalStream);

        await Assert.That(pngMetadataList.Count).IsEqualTo(2);
        await Assert.That(pngMetadataList[0].Chunks.Select(s => s.GetType()).ToList()).IsEquivalentTo([
            typeof(ImageHeader),
            typeof(ImageEnd)
            ]);
        await Assert.That(pngMetadataList[1].Chunks.Select(s => s.GetType()).ToList()).IsEquivalentTo([
            typeof(ImageHeader),
            typeof(ImageEnd)
            ]);

        ImageHeader imageHeader1 = (ImageHeader)pngMetadataList[0].Chunks[0];
        ImageHeader imageHeader2 = (ImageHeader)pngMetadataList[1].Chunks[0];
        await Assert.That(imageHeader1.Width).IsEqualTo((byte)10);
        await Assert.That(imageHeader1.Height).IsEqualTo((byte)11);
        await Assert.That(imageHeader2.Width).IsEqualTo((byte)12);
        await Assert.That(imageHeader2.Height).IsEqualTo((byte)13);
        foreach (ImageHeader imageHeader in new List<ImageHeader>() { imageHeader1, imageHeader2 })
        {
            await Assert.That(imageHeader.BitDepth).IsEqualTo((byte)8);
            await Assert.That(imageHeader.ColorType).IsEqualTo(ColorType.TruecolorUsed);
            await Assert.That(imageHeader.CompressionMethod).IsEqualTo(CompressionMethod.Deflate);
            await Assert.That(imageHeader.FilterMethod).IsEqualTo(FilterMethod.Adaptive);
            await Assert.That(imageHeader.InterlaceMethod).IsEqualTo(InterlaceMethod.Adam7);
        }
    }

    [Test]
    public async Task ReadFromStreamAsync_BasicImageChunks_DeserializesCorrectly()
    {
        byte[] imageBytes =
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG Signature

            0x00, 0x00, 0x00, 0x0D, // Length (13)
            0x49, 0x48, 0x44, 0x52, // Type (ImageHeader)
            0x00, 0x00, 0x00, 0x0A, // Width
            0x00, 0x00, 0x00, 0x0B, // Height
            0x08,                   // BitDepth
            0x02,                   // ColorType
            0x00,                   // CompressionMethod
            0x00,                   // FilterMethod
            0x01,                   // InterlaceMethod
            0xBE, 0x0B, 0xBB, 0xD9, // CRC

            0x00, 0x00, 0x00, 0x00, // Length (0)
            0x49, 0x45, 0x4E, 0x44, // Type (ImageEnd)
            0xAE, 0x42, 0x60, 0x82  // CRC
        ];
        using MarshalStream marshalStream = new(imageBytes);
        PngMetadata? pngMetadata = await PngMetadata.ReadFromStreamAsync(marshalStream);
        await Assert.That(pngMetadata).IsNotNull();
        await Assert.That(pngMetadata!.Chunks.Count).IsEqualTo(2);
        await Assert.That(pngMetadata!.Chunks[0].GetType()).IsEqualTo(typeof(ImageHeader));
        await Assert.That(pngMetadata!.Chunks[1].GetType()).IsEqualTo(typeof(ImageEnd));

        ImageHeader imageHeader = (ImageHeader)pngMetadata!.Chunks[0];
        await Assert.That(imageHeader.Width).IsEqualTo((byte)10);
        await Assert.That(imageHeader.Height).IsEqualTo((byte)11);
        await Assert.That(imageHeader.BitDepth).IsEqualTo((byte)8);
        await Assert.That(imageHeader.ColorType).IsEqualTo(ColorType.TruecolorUsed);
        await Assert.That(imageHeader.CompressionMethod).IsEqualTo(CompressionMethod.Deflate);
        await Assert.That(imageHeader.FilterMethod).IsEqualTo(FilterMethod.Adaptive);
        await Assert.That(imageHeader.InterlaceMethod).IsEqualTo(InterlaceMethod.Adam7);
    }

    [Test]
    public async Task ReadFromStreamAsync_EmptyStream_DeserializedCorrectly()
    {
        byte[] imageBytes = [];
        using MarshalStream marshalStream = new(imageBytes);
        PngMetadata? pngMetadata = await PngMetadata.ReadFromStreamAsync(marshalStream);
        await Assert.That(pngMetadata).IsNull();
    }

    [Test]
    public async Task ReadFromStreamAsync_MultipleMetadatasMultipleChunks_StreamReadingStopsAfterFirst()
    {
        byte[] imageBytes =
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG Signature

            0x00, 0x00, 0x00, 0x0D, // Length (13)
            0x49, 0x48, 0x44, 0x52, // Type (ImageHeader)
            0x00, 0x00, 0x00, 0x0A, // Width
            0x00, 0x00, 0x00, 0x0B, // Height
            0x08,                   // BitDepth
            0x02,                   // ColorType
            0x00,                   // CompressionMethod
            0x00,                   // FilterMethod
            0x01,                   // InterlaceMethod
            0xBE, 0x0B, 0xBB, 0xD9, // CRC

            0x00, 0x00, 0x00, 0x00, // Length (0)
            0x49, 0x45, 0x4E, 0x44, // Type (ImageEnd)
            0xAE, 0x42, 0x60, 0x82, // CRC

            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG Signature

            0x00, 0x00, 0x00, 0x0D, // Length (13)
            0x49, 0x48, 0x44, 0x52, // Type (ImageHeader)
            0x00, 0x00, 0x00, 0x0C, // Width
            0x00, 0x00, 0x00, 0x0D, // Height
            0x08,                   // BitDepth
            0x02,                   // ColorType
            0x00,                   // CompressionMethod
            0x00,                   // FilterMethod
            0x01,                   // InterlaceMethod
            0xBE, 0x0B, 0xBB, 0xD9, // CRC

            0x00, 0x00, 0x00, 0x00, // Length (0)
            0x49, 0x45, 0x4E, 0x44, // Type (ImageEnd)
            0xAE, 0x42, 0x60, 0x82, // CRC
        ];

        using MarshalStream marshalStream = new(imageBytes);
        PngMetadata? pngMetadata = await PngMetadata.ReadFromStreamAsync(marshalStream);

        await Assert.That(marshalStream.Position).IsEqualTo(imageBytes.Length / 2);
        await Assert.That(pngMetadata).IsNotNull();
        await Assert.That(pngMetadata!.Chunks.Select(c => c.GetType()).ToList()).IsEquivalentTo([
            typeof(ImageHeader),
            typeof(ImageEnd)
            ]);
        ImageHeader imageHeader = (ImageHeader)pngMetadata!.Chunks[0];
        await Assert.That(imageHeader.Width).IsEqualTo((byte)10);
        await Assert.That(imageHeader.Height).IsEqualTo((byte)11);
        await Assert.That(imageHeader.BitDepth).IsEqualTo((byte)8);
        await Assert.That(imageHeader.ColorType).IsEqualTo(ColorType.TruecolorUsed);
        await Assert.That(imageHeader.CompressionMethod).IsEqualTo(CompressionMethod.Deflate);
        await Assert.That(imageHeader.FilterMethod).IsEqualTo(FilterMethod.Adaptive);
        await Assert.That(imageHeader.InterlaceMethod).IsEqualTo(InterlaceMethod.Adam7);
    }

    [Test]
    public async Task ReadFromStreamAsync_NoImageEndMarker_ThrowsEndOfStreamException()
    {
        byte[] imageBytes =
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG Signature

            0x00, 0x00, 0x00, 0x0D, // Length (13)
            0x49, 0x48, 0x44, 0x52, // Type (ImageHeader)
            0x00, 0x00, 0x00, 0x0A, // Width
            0x00, 0x00, 0x00, 0x0B, // Height
            0x08,                   // BitDepth
            0x02,                   // ColorType
            0x00,                   // CompressionMethod
            0x00,                   // FilterMethod
            0x01,                   // InterlaceMethod
            0xBE, 0x0B, 0xBB, 0xD9, // CRC
        ];
        using MarshalStream marshalStream = new(imageBytes);
        await Assert.That(async () => await PngMetadata.ReadFromStreamAsync(marshalStream)).Throws<EndOfStreamException>();
    }

    [Test]
    public async Task RemoveAll_NoneFound_ReturnsZero()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageEnd());
        int count = pngMetadata.RemoveAll<ImageData>();
        await Assert.That(count).IsEqualTo(0);
        await Assert.That(pngMetadata.Chunks.Select(s => s.GetType())).IsEquivalentTo([
            typeof(ImageHeader),
            typeof(FrameData),
            typeof(ImageEnd)]);
    }

    [Test]
    public async Task RemoveAll_OneFound_ReturnsOne()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageData());
        pngMetadata.Chunks.Add(new ImageEnd());
        int count = pngMetadata.RemoveAll<ImageData>();
        await Assert.That(count).IsEqualTo(1);
        await Assert.That(pngMetadata.Chunks.Select(s => s.GetType())).IsEquivalentTo([
            typeof(ImageHeader),
            typeof(FrameData),
            typeof(ImageEnd)]);
    }

    [Test]
    public async Task RemoveAll_TwoFound_ReturnsTwo()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageData());
        pngMetadata.Chunks.Add(new ImageEnd());
        pngMetadata.Chunks.Add(new ImageData());
        int count = pngMetadata.RemoveAll<ImageData>();
        await Assert.That(count).IsEqualTo(2);
        await Assert.That(pngMetadata.Chunks.Select(s => s.GetType())).IsEquivalentTo([
            typeof(ImageHeader),
            typeof(FrameData),
            typeof(ImageEnd)]);
    }

    [Test]
    public async Task RemoveFirst_NoneFound_ReturnsFalse()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageEnd());
        bool result = pngMetadata.RemoveFirst<ImageData>();
        await Assert.That(result).IsEqualTo(false);
        await Assert.That(pngMetadata.Chunks.Select(s => s.GetType())).IsEquivalentTo([
            typeof(ImageHeader),
            typeof(FrameData),
            typeof(ImageEnd)]);
    }

    [Test]
    public async Task RemoveFirst_OneFound_ReturnsTrue()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageData());
        pngMetadata.Chunks.Add(new ImageEnd());
        bool result = pngMetadata.RemoveFirst<ImageData>();
        await Assert.That(result).IsEqualTo(true);
        await Assert.That(pngMetadata.Chunks.Select(s => s.GetType())).IsEquivalentTo([
            typeof(ImageHeader),
            typeof(FrameData),
            typeof(ImageEnd)]);
    }

    [Test]
    public async Task RemoveFirst_TwoFound_ReturnsTrue()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageHeader());
        pngMetadata.Chunks.Add(new FrameData());
        pngMetadata.Chunks.Add(new ImageData());
        pngMetadata.Chunks.Add(new ImageEnd());
        pngMetadata.Chunks.Add(new ImageData());
        bool result = pngMetadata.RemoveFirst<ImageData>();
        await Assert.That(result).IsEqualTo(true);
        await Assert.That(pngMetadata.Chunks.Select(s => s.GetType())).IsEquivalentTo([
            typeof(ImageHeader),
            typeof(FrameData),
            typeof(ImageEnd),
            typeof(ImageData)]);
    }

    [Test]
    public async Task ValidateAndReorderChunks_Cycle_Throws()
    {
        List<Chunk> chunks = [
            new ImageHeader(),
            new Cyclical2Chunk(),
            new Cyclical3Chunk(),
            new Cyclical1Chunk(),
            new ImageEnd()
        ];
        await Assert.That(() => PngMetadata.ValidateAndReorderChunks(chunks)).Throws<PngException>();
    }

    [Test]
    public async Task ValidateAndReorderChunks_EmptyList_ThrowsDueToNoHeader()
    {
        List<Chunk> chunks = [];
        await Assert.That(() => PngMetadata.ValidateAndReorderChunks(chunks)).Throws<PngException>();
    }

    [Test]
    public async Task ValidateAndReorderChunks_ExemptFirstAtMiddle_OrderedProperly()
    {
        List<Chunk> chunks = [
            new ImageData(),
            new FrameControl() { SequenceNumber = 1 },
            new FrameControl(),
            new ImageData(),
            new ImageEnd(),
            new ImageHeader()
        ];
        List<Chunk> orderedChunks = PngMetadata.ValidateAndReorderChunks(chunks);
        await Assert.That(orderedChunks.Select(c => c.ChunkType).ToList()).IsEquivalentTo(new List<ChunkType>() {
            ChunkType.ImageHeader,
            ChunkType.ImageData,
            ChunkType.FrameControl,
            ChunkType.ImageData,
            ChunkType.FrameControl,
            ChunkType.ImageEnd
        });
        await Assert.That(((FrameControl)orderedChunks[2]).SequenceNumber).IsEqualTo(1U);
    }

    [Test]
    public async Task ValidateAndReorderChunks_ExemptFirstAtTop_OrderedProperly()
    {
        List<Chunk> chunks = [
            new FrameControl() { SequenceNumber = 1 },
            new FrameControl(),
            new ImageData(),
            new ImageData(),
            new ImageEnd(),
            new ImageHeader()
        ];
        List<Chunk> orderedChunks = PngMetadata.ValidateAndReorderChunks(chunks);
        await Assert.That(orderedChunks.Select(c => c.ChunkType).ToList()).IsEquivalentTo(new List<ChunkType>() {
            ChunkType.ImageHeader,
            ChunkType.FrameControl,
            ChunkType.ImageData,
            ChunkType.ImageData,
            ChunkType.FrameControl,
            ChunkType.ImageEnd
        });
        await Assert.That(((FrameControl)orderedChunks[1]).SequenceNumber).IsEqualTo(1U);
    }

    [Test]
    public async Task ValidateAndReorderChunks_MultipleEnds_Throws()
    {
        List<Chunk> chunks = [
            new ImageHeader(),
            new ImageEnd(),
            new ImageEnd()
        ];
        await Assert.That(() => PngMetadata.ValidateAndReorderChunks(chunks)).Throws<PngException>();
    }

    [Test]
    public async Task ValidateAndReorderChunks_MultipleHeaders_Throws()
    {
        List<Chunk> chunks = [
            new ImageHeader(),
            new ImageHeader(),
            new ImageEnd()
        ];
        await Assert.That(() => PngMetadata.ValidateAndReorderChunks(chunks)).Throws<PngException>();
    }

    [Test]
    public async Task ValidateAndReorderChunks_MultipleOfSameChunkType_OrderedProperly()
    {
        List<Chunk> chunks = [
            new ImageData() {
                Data = [1]
            },
            new ImageEnd(),
            new ImageData() {
                Data = [2]
            },
            new ImageHeader(),
            new ImageData() {
                Data = [3]
            },
        ];
        List<Chunk> orderedChunks = PngMetadata.ValidateAndReorderChunks(chunks);
        await Assert.That(orderedChunks.Select(c => c.ChunkType).ToList()).IsEquivalentTo(new List<ChunkType>() {
            ChunkType.ImageHeader,
            ChunkType.ImageData,
            ChunkType.ImageData,
            ChunkType.ImageData,
            ChunkType.ImageEnd
        });
        await Assert.That(((ImageData)orderedChunks[1]).Data[0]).IsEqualTo((byte)1);
        await Assert.That(((ImageData)orderedChunks[2]).Data[0]).IsEqualTo((byte)2);
        await Assert.That(((ImageData)orderedChunks[3]).Data[0]).IsEqualTo((byte)3);
    }

    [Test]
    public async Task ValidateAndReorderChunks_MultipleOnChunkWhichDisallowsIt_Throws()
    {
        List<Chunk> chunks = [
            new ImageHeader(),
            new Exif(),
            new Exif(),
            new ImageEnd()
        ];
        await Assert.That(() => PngMetadata.ValidateAndReorderChunks(chunks)).Throws<PngException>();
    }

    [Test]
    public async Task ValidateAndReorderChunks_OnlyHeaderAndEnd_OrderedProperly()
    {
        List<Chunk> chunks = [
            new ImageEnd(),
            new ImageHeader()
        ];
        List<Chunk> orderedChunks = PngMetadata.ValidateAndReorderChunks(chunks);
        await Assert.That(orderedChunks.Select(c => c.ChunkType).ToList()).IsEquivalentTo(new List<ChunkType>() {
            ChunkType.ImageHeader,
            ChunkType.ImageEnd
        });
    }

    [Test]
    public async Task ValidateAndReorderChunks_OrderAfterIEndChunk_Throws()
    {
        List<Chunk> chunks = [
            new ImageHeader(),
            new OrderAfterIEndChunk(),
            new ImageEnd()
        ];
        await Assert.That(() => PngMetadata.ValidateAndReorderChunks(chunks)).Throws<PngException>();
    }

    [Test]
    public async Task ValidateAndReorderChunks_OrderBeforeIHeaderChunk_Throws()
    {
        List<Chunk> chunks = [
            new ImageHeader(),
            new OrderBeforeIHeaderChunk(),
            new ImageEnd()
        ];
        await Assert.That(() => PngMetadata.ValidateAndReorderChunks(chunks)).Throws<PngException>();
    }

    [Test]
    public async Task ValidateAndReorderChunks_SelfReferencingOrderAfterChunk_Throws()
    {
        List<Chunk> chunks = [
            new ImageHeader(),
            new SelfReferencingOrderAfterChunk(),
            new ImageEnd()
        ];
        await Assert.That(() => PngMetadata.ValidateAndReorderChunks(chunks)).Throws<PngException>();
    }

    [Test]
    public async Task ValidateAndReorderChunks_SelfReferencingOrderBeforeChunk_Throws()
    {
        List<Chunk> chunks = [
            new ImageHeader(),
            new SelfReferencingOrderBeforeChunk(),
            new ImageEnd()
        ];
        await Assert.That(() => PngMetadata.ValidateAndReorderChunks(chunks)).Throws<PngException>();
    }

    [Test]
    public async Task ValidateAndReorderChunks_VarietyOfChunks_OrderedProperly()
    {
        List<Chunk> chunks = [
            new ImageData(),
            new ImageEnd(),
            new Histogram(),
            new Palette(),
            new ImageHeader(),
            new Gamma()
        ];
        List<Chunk> orderedChunks = PngMetadata.ValidateAndReorderChunks(chunks);
        await Assert.That(orderedChunks.Select(c => c.ChunkType).ToList()).IsEquivalentTo(new List<ChunkType>() {
            ChunkType.ImageHeader,
            ChunkType.Gamma,
            ChunkType.Palette,
            ChunkType.Histogram,
            ChunkType.ImageData,
            ChunkType.ImageEnd
        });
    }

    [Test]
    public async Task WriteAllToStreamAsync_MultipleMetadataWithMultipleChunks_WritesCorrectOutput()
    {
        PngMetadata pngMetadata1 = new();
        pngMetadata1.Chunks.Add(new ImageHeader
        {
            Width = 10,
            Height = 11,
            BitDepth = PngImageType.Truecolor.AllowedBitDepths.First(),
            ColorType = PngImageType.Truecolor.ColorType,
            CompressionMethod = CompressionMethod.Deflate,
            FilterMethod = FilterMethod.Adaptive,
            InterlaceMethod = InterlaceMethod.Adam7
        });
        pngMetadata1.Chunks.Add(new ImageEnd());

        PngMetadata pngMetadata2 = new();
        pngMetadata2.Chunks.Add(new ImageHeader
        {
            Width = 12,
            Height = 13,
            BitDepth = PngImageType.Truecolor.AllowedBitDepths.First(),
            ColorType = PngImageType.Truecolor.ColorType,
            CompressionMethod = CompressionMethod.Deflate,
            FilterMethod = FilterMethod.Adaptive,
            InterlaceMethod = InterlaceMethod.Adam7
        });
        pngMetadata2.Chunks.Add(new ImageEnd());
        using MemoryStream memoryStream = new();
        using MarshalStream stream = new(memoryStream, false);
        await PngMetadata.WriteAllToStreamAsync(stream, [pngMetadata1, pngMetadata2]);
        stream.Flush();
        byte[] bytes = memoryStream.ToArray();
        byte[] expectedBytes =
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG Signature

            0x00, 0x00, 0x00, 0x0D, // Length (13)
            0x49, 0x48, 0x44, 0x52, // Type (ImageHeader)
            0x00, 0x00, 0x00, 0x0A, // Width
            0x00, 0x00, 0x00, 0x0B, // Height
            0x08,                   // BitDepth
            0x02,                   // ColorType
            0x00,                   // CompressionMethod
            0x00,                   // FilterMethod
            0x01,                   // InterlaceMethod
            0xBE, 0x0B, 0xBB, 0xD9, // CRC

            0x00, 0x00, 0x00, 0x00, // Length (0)
            0x49, 0x45, 0x4E, 0x44, // Type (ImageEnd)
            0xAE, 0x42, 0x60, 0x82, // CRC

            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG Signature

            0x00, 0x00, 0x00, 0x0D, // Length (13)
            0x49, 0x48, 0x44, 0x52, // Type (ImageHeader)
            0x00, 0x00, 0x00, 0x0C, // Width
            0x00, 0x00, 0x00, 0x0D, // Height
            0x08,                   // BitDepth
            0x02,                   // ColorType
            0x00,                   // CompressionMethod
            0x00,                   // FilterMethod
            0x01,                   // InterlaceMethod
            0x65, 0x4C, 0x28, 0x83, // CRC

            0x00, 0x00, 0x00, 0x00, // Length (0)
            0x49, 0x45, 0x4E, 0x44, // Type (ImageEnd)
            0xAE, 0x42, 0x60, 0x82  // CRC
        ];
        await Assert.That(bytes).IsEquivalentTo(expectedBytes);
    }

    [Test]
    public async Task WriteAllToStreamAsync_NoMetadatas_WritesCorrectOutput()
    {
        using MemoryStream memoryStream = new();
        using MarshalStream stream = new(memoryStream, false);
        await PngMetadata.WriteAllToStreamAsync(stream, []);
        stream.Flush();
        byte[] bytes = memoryStream.ToArray();
        byte[] expectedBytes = [];
        await Assert.That(bytes).IsEquivalentTo(expectedBytes);
    }

    [Test]
    public async Task WriteAllToStreamAsync_SingleMetadataAndSegment_WritesCorrectOutput()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageEnd());
        using MemoryStream memoryStream = new();
        using MarshalStream stream = new(memoryStream, false);
        await PngMetadata.WriteAllToStreamAsync(stream, [pngMetadata]);
        stream.Flush();
        byte[] bytes = memoryStream.ToArray();
        byte[] expectedBytes =
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG Signature

            0x00, 0x00, 0x00, 0x00, // Length (0)
            0x49, 0x45, 0x4E, 0x44, // Type (ImageEnd)
            0xAE, 0x42, 0x60, 0x82  // CRC
        ];
        await Assert.That(bytes).IsEquivalentTo(expectedBytes);
    }

    [Test]
    public async Task WriteAllToStreamAsync_TwoEmptyMetadata_WritesCorrectOutput()
    {
        PngMetadata pngMetadata1 = new();
        PngMetadata pngMetadata2 = new();
        using MemoryStream memoryStream = new();
        using MarshalStream stream = new(memoryStream, false);
        await PngMetadata.WriteAllToStreamAsync(stream, [pngMetadata1, pngMetadata2]);
        stream.Flush();
        byte[] bytes = memoryStream.ToArray();
        byte[] expectedBytes = [];
        await Assert.That(bytes).IsEquivalentTo(expectedBytes);
    }

    [Test]
    public async Task WriteToStreamAsync_EndToEnd_WritesCorrectOutput()
    {
        PngMetadata pngMetadata1 = new();
        pngMetadata1.Chunks.Add(new ImageHeader
        {
            Width = 10,
            Height = 11,
            BitDepth = PngImageType.Truecolor.AllowedBitDepths.First(),
            ColorType = PngImageType.Truecolor.ColorType,
            CompressionMethod = CompressionMethod.Deflate,
            FilterMethod = FilterMethod.Adaptive,
            InterlaceMethod = InterlaceMethod.Adam7
        });
        pngMetadata1.Chunks.Add(new ImageEnd());
        using MemoryStream memoryStream = new();
        using MarshalStream stream = new(memoryStream, false);
        await PngMetadata.WriteAllToStreamAsync(stream, [pngMetadata1]);
        stream.Flush();
        byte[] bytes = memoryStream.ToArray();
        byte[] expectedBytes =
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG Signature

            0x00, 0x00, 0x00, 0x0D, // Length (13)
            0x49, 0x48, 0x44, 0x52, // Type (ImageHeader)
            0x00, 0x00, 0x00, 0x0A, // Width
            0x00, 0x00, 0x00, 0x0B, // Height
            0x08,                   // BitDepth
            0x02,                   // ColorType
            0x00,                   // CompressionMethod
            0x00,                   // FilterMethod
            0x01,                   // InterlaceMethod
            0xBE, 0x0B, 0xBB, 0xD9, // CRC

            0x00, 0x00, 0x00, 0x00, // Length (0)
            0x49, 0x45, 0x4E, 0x44, // Type (ImageEnd)
            0xAE, 0x42, 0x60, 0x82, // CRC
        ];
        await Assert.That(bytes).IsEquivalentTo(expectedBytes);
    }

    [Test]
    public async Task WriteToStreamAsync_NoChunks_WritesNothing()
    {
        PngMetadata pngMetadata = new();
        using MemoryStream memoryStream = new();
        using MarshalStream stream = new(memoryStream, false);
        await pngMetadata.WriteToStreamAsync(stream);
        stream.Flush();
        byte[] bytes = memoryStream.ToArray();
        byte[] expectedBytes = [];
        await Assert.That(bytes).IsEquivalentTo(expectedBytes);
    }

    [Test]
    public async Task WriteToStreamAsync_SingleSegment_WritesCorrectOutput()
    {
        PngMetadata pngMetadata = new();
        pngMetadata.Chunks.Add(new ImageEnd());
        using MemoryStream memoryStream = new();
        using MarshalStream stream = new(memoryStream, false);
        await pngMetadata.WriteToStreamAsync(stream);
        stream.Flush();
        byte[] bytes = memoryStream.ToArray();
        byte[] expectedBytes =
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG Signature

            0x00, 0x00, 0x00, 0x00, // Length (0)
            0x49, 0x45, 0x4E, 0x44, // Type (ImageEnd)
            0xAE, 0x42, 0x60, 0x82  // CRC
        ];
        await Assert.That(bytes).IsEquivalentTo(expectedBytes);
    }
}

[Chunk("cyca")]
[OrderAfterChunks("cycb")]
public class Cyclical1Chunk : Chunk
{
    public override ValueTask ReadFromStreamAsync(MarshalStream stream, int chunkLength, CancellationToken cancellationToken) => throw new NotSupportedException();

    public override int ValidateAndComputeLength() => throw new NotSupportedException();

    public override ValueTask WriteToStreamAsync(MarshalStream stream, CancellationToken cancellationToken) => throw new NotSupportedException();
}

[Chunk("cycb")]
[OrderAfterChunks("cycc")]
public class Cyclical2Chunk : Chunk
{
    public override ValueTask ReadFromStreamAsync(MarshalStream stream, int chunkLength, CancellationToken cancellationToken) => throw new NotSupportedException();

    public override int ValidateAndComputeLength() => throw new NotSupportedException();

    public override ValueTask WriteToStreamAsync(MarshalStream stream, CancellationToken cancellationToken) => throw new NotSupportedException();
}

[Chunk("cycc")]
[OrderAfterChunks("cyca")]
public class Cyclical3Chunk : Chunk
{
    public override ValueTask ReadFromStreamAsync(MarshalStream stream, int chunkLength, CancellationToken cancellationToken) => throw new NotSupportedException();

    public override int ValidateAndComputeLength() => throw new NotSupportedException();

    public override ValueTask WriteToStreamAsync(MarshalStream stream, CancellationToken cancellationToken) => throw new NotSupportedException();
}

[Chunk("aend")]
[OrderAfterChunks("IEND")]
public class OrderAfterIEndChunk : Chunk
{
    public override ValueTask ReadFromStreamAsync(MarshalStream stream, int chunkLength, CancellationToken cancellationToken) => throw new NotSupportedException();

    public override int ValidateAndComputeLength() => throw new NotSupportedException();

    public override ValueTask WriteToStreamAsync(MarshalStream stream, CancellationToken cancellationToken) => throw new NotSupportedException();
}

[Chunk("bhdr")]
[OrderBeforeChunks("IHDR")]
public class OrderBeforeIHeaderChunk : Chunk
{
    public override ValueTask ReadFromStreamAsync(MarshalStream stream, int chunkLength, CancellationToken cancellationToken) => throw new NotSupportedException();

    public override int ValidateAndComputeLength() => throw new NotSupportedException();

    public override ValueTask WriteToStreamAsync(MarshalStream stream, CancellationToken cancellationToken) => throw new NotSupportedException();
}

[Chunk("aslf")]
[OrderAfterChunks("aslf")]
public class SelfReferencingOrderAfterChunk : Chunk
{
    public override ValueTask ReadFromStreamAsync(MarshalStream stream, int chunkLength, CancellationToken cancellationToken) => throw new NotSupportedException();

    public override int ValidateAndComputeLength() => throw new NotSupportedException();

    public override ValueTask WriteToStreamAsync(MarshalStream stream, CancellationToken cancellationToken) => throw new NotSupportedException();
}

[Chunk("bslf")]
[OrderAfterChunks("bslf")]
public class SelfReferencingOrderBeforeChunk : Chunk
{
    public override ValueTask ReadFromStreamAsync(MarshalStream stream, int chunkLength, CancellationToken cancellationToken) => throw new NotSupportedException();

    public override int ValidateAndComputeLength() => throw new NotSupportedException();

    public override ValueTask WriteToStreamAsync(MarshalStream stream, CancellationToken cancellationToken) => throw new NotSupportedException();
}