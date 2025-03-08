using Phaeyz.Marshalling;

namespace Phaeyz.Png.Chunks;

/// <summary>
/// The UTC time of the last image modification (not the time of initial image creation).
/// </summary>
/// <remarks>
/// The tIME chunk is intended for use as an automatically-applied time stamp that is
/// updated whenever the image data are changed.
/// </remarks>
[Chunk("tIME")]
public class Time : Chunk
{
    /// <summary>
    /// The chunk's data is always this length.
    /// </summary>
    public const int FixedLength = 7;

    /// <summary>
    /// The year. It should be complete and not short (i.e. <c>1995</c> and not <c>95</c>).
    /// </summary>
    public ushort Year { get; set; }

    /// <summary>
    /// The month (<c>1</c>-<c>12</c>).
    /// </summary>
    public byte Month { get; set; }

    /// <summary>
    /// The day (<c>1</c>-<c>31</c>).
    /// </summary>
    public byte Day { get; set; }

    /// <summary>
    /// The hour (<c>0</c>-<c>23</c>).
    /// </summary>
    public byte Hour { get; set; }

    /// <summary>
    /// The minute (<c>0</c>-<c>59</c>).
    /// </summary>
    public byte Minute { get; set; }

    /// <summary>
    /// THe second (<c>0</c>-<c>60</c>). <c>60</c> allowed for leap seconds.
    /// </summary>
    public byte Second { get; set; }

    /// <summary>
    /// A <c>System.DateTime</c> version of the chunk.
    /// </summary>
    public DateTime DateTime
    {
        get => new(Year, Month, Day, Hour, Minute, Second, DateTimeKind.Utc);
        set
        {
            value = value.ToUniversalTime();
            Year = (ushort)value.Year;
            Month = (byte)value.Month;
            Day = (byte)value.Day;
            Hour = (byte)value.Hour;
            Minute = (byte)value.Minute;
            Second = (byte)value.Second;
        }
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
        if (chunkLength != FixedLength)
        {
            throw new PngException("Unexpected chunk length.");
        }

        Year = await stream.ReadUInt16Async(ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        Month = await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false);
        Day = await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false);
        Hour = await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false);
        Minute = await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false);
        Second = await stream.ReadUInt8Async(cancellationToken).ConfigureAwait(false);
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
    public override int ValidateAndComputeLength() => FixedLength;

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
        await stream.WriteUInt16Async(Year, ByteConverter.BigEndian, cancellationToken).ConfigureAwait(false);
        await stream.WriteUInt8Async(Month, cancellationToken);
        await stream.WriteUInt8Async(Day, cancellationToken);
        await stream.WriteUInt8Async(Hour, cancellationToken);
        await stream.WriteUInt8Async(Minute, cancellationToken);
        await stream.WriteUInt8Async(Second, cancellationToken);
    }
}