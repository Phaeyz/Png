using System.Diagnostics.CodeAnalysis;

namespace Phaeyz.Png;

/// <summary>
/// Chunk classes must be decorated with this attribute to indicate the chunk type.
/// </summary>
/// <remarks>
/// Classes decorated with this attribute must also have a parameterless constructor.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="Phaeyz.Png.ChunkAttribute"/> class.
/// </remarks>
/// <param name="chunkTypeLabel">
/// The chunk type label.
/// </param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class ChunkAttribute(string chunkTypeLabel) : Attribute
{
    /// <summary>
    /// The chunk type.
    /// </summary>
    public ChunkType ChunkType { get; private set; } = chunkTypeLabel;

    /// <summary>
    /// If <c>true</c>, the chunk may exist multiple times in a single PNG image.
    /// </summary>
    public bool AllowMultiple { get; set; } = false;

    /// <summary>
    /// Gets the <see cref="Phaeyz.Png.ChunkAttribute"/> of a chunk class type.
    /// </summary>
    /// <typeparam name="T">
    /// The chunk class type.
    /// </typeparam>
    /// <returns>
    /// The <see cref="Phaeyz.Png.ChunkAttribute"/> of a chunk class type.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// The chunk class type does not implement <see cref="Phaeyz.Png.Chunk"/>,
    /// or the type does not have a public default parameterless constructor,
    /// or the type does not have a <see cref="Phaeyz.Png.ChunkAttribute"/>.
    /// </exception>
    public static ChunkAttribute Get<T>() where T : Chunk, new() => Get(typeof(T));

    /// <summary>
    /// Gets the <see cref="Phaeyz.Png.ChunkAttribute"/> of a chunk class type.
    /// </summary>
    /// <param name="chunkClassType">
    /// The chunk class type.
    /// </param>
    /// <returns>
    /// The <see cref="Phaeyz.Png.ChunkAttribute"/> of a chunk class type.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// The type associated with <paramref name="chunkClassType"/> does not implement <see cref="Phaeyz.Png.Chunk"/>,
    /// or the type does not have a public default parameterless constructor,
    /// or the type does not have a <see cref="Phaeyz.Png.ChunkAttribute"/>.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="chunkClassType"/> is <c>null</c>.
    /// </exception>
    public static ChunkAttribute Get(Type chunkClassType)
    {
        ArgumentNullException.ThrowIfNull(chunkClassType);

        if (!typeof(Chunk).IsAssignableFrom(chunkClassType))
        {
            throw new ArgumentException($"Type '{chunkClassType.Name}' does not implement Chunk.", nameof(chunkClassType));
        }

        if (chunkClassType.GetConstructor(Type.EmptyTypes) is null)
        {
            throw new ArgumentException($"Type '{chunkClassType.Name}' does not have a public default parameterless constructor.", nameof(chunkClassType));
        }

        ChunkAttribute? chunkAttribute = chunkClassType
            .GetCustomAttributes(typeof(ChunkAttribute), false)
            .Cast<ChunkAttribute>()
            .FirstOrDefault();

        return chunkAttribute is null
            ? throw new ArgumentException($"Type '{chunkClassType.Name}' does not have a ChunkAttribute.", nameof(chunkClassType))
            : chunkAttribute;
    }

    /// <summary>
    /// Gets the <see cref="Phaeyz.Png.ChunkAttribute"/> of a chunk class type.
    /// </summary>
    /// <typeparam name="T">
    /// The chunk class type.
    /// </typeparam>
    /// <param name="chunkAttribute">
    /// If the method returns <c>true</c> this will receive the chunk attribute. Otherwise it will receive <c>null</c>.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the chunk class type is valid and the <paramref name="chunkAttribute"/> received the
    /// attribute; <c>false</c> otherwise.
    /// </returns>
    public static bool TryGet<T>([MaybeNullWhen(false)] out ChunkAttribute chunkAttribute) where T : Chunk, new() =>
        TryGet(typeof(T), out chunkAttribute);

    /// <summary>
    /// Gets the <see cref="Phaeyz.Png.ChunkAttribute"/> of a chunk class type.
    /// </summary>
    /// <param name="chunkClassType">
    /// The chunk class type.
    /// </param>
    /// <param name="chunkAttribute">
    /// If the method returns <c>true</c> this will receive the chunk attribute. Otherwise it will receive <c>null</c>.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the chunk class type is valid and the <paramref name="chunkAttribute"/> received the
    /// attribute; <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// This function always returns <c>false</c> if the chunk class type does not derive from
    /// <see cref="Phaeyz.Png.Chunk"/> or if the chunk class type does not have a public default parameterless constructor.
    /// </remarks>
    public static bool TryGet(Type chunkClassType, [MaybeNullWhen(false)] out ChunkAttribute chunkAttribute)
    {
        if (chunkClassType is null ||
            !typeof(Chunk).IsAssignableFrom(chunkClassType) ||
            chunkClassType.GetConstructor(Type.EmptyTypes) is not null)
        {
            chunkAttribute = null;
            return false;
        }
        chunkAttribute = chunkClassType
            .GetCustomAttributes(typeof(ChunkAttribute), false)
            .Cast<ChunkAttribute>()
            .FirstOrDefault();
        return chunkAttribute is not null;
    }
}