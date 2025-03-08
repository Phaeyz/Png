using System.Reflection;

namespace Phaeyz.Png;

/// <summary>
/// Used by chunk readers to determine which classes to use for chunk types.
/// </summary>
public class ChunkDefinitions
{
    /// <summary>
    /// A dictionary of chunk types based on their type.
    /// </summary>
    private readonly Dictionary<ChunkType, Type> _chunkTypes;

    /// <summary>
    /// Initializes a value containing the default chunk definitions.
    /// </summary>
    private static readonly Lazy<ChunkDefinitions> s_default = new(() => CreateDefault().MakeReadOnly());

    /// <summary>
    /// Gets the default chunk definitions.
    /// </summary>
    public static ChunkDefinitions Default => s_default.Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Phaeyz.Png.ChunkDefinitions"/> class.
    /// </summary>
    public ChunkDefinitions()
    {
        _chunkTypes = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Phaeyz.Png.ChunkDefinitions"/> class, and clones a preexisting instance.
    /// </summary>
    /// <param name="chunkDefinitions">
    /// The chunk definitions to clone into the new instance. For example, this could be the default chunk definitions.
    /// </param>
    public ChunkDefinitions(ChunkDefinitions chunkDefinitions)
    {
        _chunkTypes = new Dictionary<ChunkType, Type>(chunkDefinitions._chunkTypes);
    }

    /// <summary>
    /// Gets a value indicating whether the current object is read-only.
    /// </summary>
    public bool IsReadOnly { get; private set; }

    /// <summary>
    /// Adds a mappings for a chunk type to a chunk class.
    /// </summary>
    /// <typeparam name="T">
    /// The chunk type to add a mapping for.
    /// </typeparam>
    /// <param name="overrideExistingMappings">
    /// If <c>true</c> and there is an existing mapping for the chunk type to a chunk class, it will be removed and replaced.
    /// If <c>false</c>, an exception will be thrown if a mapping for the chunk type already exists.
    /// The default value is <c>false</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> is a mapping for the chunk type to a chunk class type was added;
    /// <c>false</c> if a mapping already exists and <paramref name="overrideExistingMappings"/> is <c>false</c>.
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    /// The current object is read-only.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// A configuration issue occurred with the chunk class, or a mapping for the chunk type already exists.
    /// </exception>
    /// <remarks>
    /// The chunk class must implement <see cref="Phaeyz.Png.Chunk"/> and be decorated with <see cref="Phaeyz.Png.ChunkAttribute"/>.
    /// </remarks>
    public void Add<T>(bool overrideExistingMappings = false) where T : Chunk, new() =>
        Add(typeof(T), overrideExistingMappings);

    /// <summary>
    /// Adds a mappings for a chunk type to a chunk class.
    /// </summary>
    /// <param name="chunkClassType">
    /// The chunk type to add a mapping for.
    /// </param>
    /// <param name="overrideExistingMappings">
    /// If <c>true</c> and there is an existing mapping for the chunk type to a chunk class, it will be removed and replaced.
    /// If <c>false</c>, an exception will be thrown if a mapping for the chunk type already exists.
    /// The default value is <c>false</c>.
    /// </param>
    /// <exception cref="System.InvalidOperationException">
    /// The current object is read-only.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// A configuration issue occurred with the chunk class, or a mapping for the chunk type already exists.
    /// </exception>
    /// <remarks>
    /// The chunk class must implement <see cref="Phaeyz.Png.Chunk"/> and be decorated with <see cref="Phaeyz.Png.ChunkAttribute"/>.
    /// </remarks>
    public void Add(Type chunkClassType, bool overrideExistingMappings = false)
    {
        if (IsReadOnly)
        {
            throw new InvalidOperationException("The current object is read-only.");
        }

        ChunkAttribute attribute = ChunkAttribute.Get(chunkClassType);

        if (overrideExistingMappings || !_chunkTypes.ContainsKey(attribute.ChunkType))
        {
            _chunkTypes[attribute.ChunkType] = chunkClassType;
        }
        else
        {
            throw new ArgumentException($"A mapping for chunk type '{attribute.ChunkType}' already exists.", nameof(chunkClassType));
        }
    }

    /// <summary>
    /// Adds all eligible mappings for chunk types to chunk classes in an assembly.
    /// </summary>
    /// <param name="assembly">
    /// The assembly containing the chunk classes to add mappings for.
    /// </param>
    /// <param name="overrideExistingMappings">
    /// If <c>true</c> and there is an existing mapping for a chunk type to a chunk class, it will be removed and replaced.
    /// If <c>false</c>, an exception will be thrown if a mapping for a chunk type already exists.
    /// The default value is <c>false</c>.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current object is read-only.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// A mapping for a chunk type already exists.
    /// </exception>
    public void AddAllInAssembly(Assembly assembly, bool overrideExistingMappings = false)
    {
        var chunkType = typeof(Chunk);
        var chunkAttributeType = typeof(ChunkAttribute);

        foreach (Type type in assembly
            .GetTypes()
            .Where(t =>
                chunkType.IsAssignableFrom(t) &&
                t.GetConstructor(Type.EmptyTypes) is not null &&
                t.GetCustomAttributes(chunkAttributeType, false).Length != 0))
        {
            Add(type, overrideExistingMappings);
        }
    }

    /// <summary>
    /// Used internally to create the default chunk definitions.
    /// </summary>
    /// <returns>
    /// The default chunk definitions.
    /// </returns>
    private static ChunkDefinitions CreateDefault()
    {
        ChunkDefinitions chunkDefinitions = new();
        chunkDefinitions.AddAllInAssembly(Assembly.GetExecutingAssembly());
        return chunkDefinitions;
    }

    /// <summary>
    /// Gets the chunk class type for a chunk type.
    /// </summary>
    /// <param name="chunkType">
    /// The chunk type to map to a chunk class type.
    /// </param>
    /// <returns>
    /// The chunk class type, or <c>null</c> if a mapping is not found.
    /// </returns>
    public Type? GetMapping(ChunkType chunkType) =>
        _chunkTypes.TryGetValue(chunkType, out Type? chunkClassType) ? chunkClassType : null;

    /// <summary>
    /// Prevents further changes to the current chunk definitions object.
    /// </summary>
    /// <returns>
    /// The current chunk definitions object.
    /// </returns>
    public ChunkDefinitions MakeReadOnly()
    {
        IsReadOnly = true;
        return this;
    }
}
