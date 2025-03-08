using Phaeyz.Marshalling;

namespace Phaeyz.Png;

/// <summary>
/// Encapsulates a series of PNG chunks which make up a valid PNG file.
/// </summary>
public class PngMetadata
{
    /// <summary>
    /// The chunks in the PNG metadata. It is up to the caller to ensure that the correct chunks
    /// exist and that they are in the correct order.
    /// </summary>
    public List<Chunk> Chunks { get; set; } = [];

    /// <summary>
    /// Finds all chunks of the specified type in the PNG metadata.
    /// </summary>
    /// <typeparam name="T">
    /// The type of chunk to find.
    /// </typeparam>
    /// <returns>
    /// An enumerable of chunks of the specified type.
    /// </returns>
    /// <exception cref="Phaeyz.Png.PngException">
    /// A chunk was found, but the class type did not match the expected type.
    /// </exception>
    public IEnumerable<T> FindAll<T>() where T : Chunk, new()
    {
        ChunkType chunkType = ChunkAttribute.Get<T>().ChunkType;
        return Chunks
            .Where(o => o.ChunkType == chunkType)
            .Select(o =>
            {
                if (o is not T)
                {
                    throw new PngException(
                        $"The existing chunk which matches the chunk type is of class type " +
                        $"'{o.GetType()}' but '{typeof(T)}' is expected.");
                }
                return (T)o;
            });
    }

    /// <summary>
    /// Finds the first chunk of the specified type in the PNG metadata.
    /// </summary>
    /// <typeparam name="T">
    /// The type of chunk to find.
    /// </typeparam>
    /// <returns>
    /// The first chunk of the specified type in the PNG metadata, or <c>null</c> if it wasn't found.
    /// </returns>
    /// <exception cref="Phaeyz.Png.PngException">
    /// A chunk was found, but the class type did not match the expected type.
    /// </exception>
    public T? FindFirst<T>() where T : Chunk, new() => FindFirst<T>(out _);

    /// <summary>
    /// Finds the first chunk of the specified type in the PNG metadata.
    /// </summary>
    /// <typeparam name="T">
    /// The type of chunk to find.
    /// </typeparam>
    /// <param name="index">
    /// Receives the zero-based index of the chunk which has been found, or <c>-1</c> if it was not found.
    /// </param>
    /// <returns>
    /// The first chunk of the specified type in the PNG metadata, or <c>null</c> if it wasn't found.
    /// </returns>
    /// <exception cref="Phaeyz.Png.PngException">
    /// A chunk was found, but the class type did not match the expected type.
    /// </exception>
    public T? FindFirst<T>(out int index) where T : Chunk, new()
    {
        index = FindFirstIndex(ChunkAttribute.Get<T>().ChunkType);
        if (index == -1)
        {
            return null;
        }

        if (Chunks[index] is not T)
        {
            throw new PngException(
                $"The existing chunk which matches the chunk type is of class type " +
                $"'{Chunks[index].GetType()}' but '{typeof(T)}' is expected.");
        }

        return (T)Chunks[index];
    }

    /// <summary>
    /// Finds the first chunk with the specified type in the PNG metadata.
    /// </summary>
    /// <param name="chunkType">
    /// The chunk type to find.
    /// </param>
    /// <returns>
    /// Returns the index of the first chunk with the specified type, or <c>-1</c> if it was not found.
    /// </returns>
    public int FindFirstIndex(ChunkType chunkType)
    {
        for (int i = 0; i < Chunks.Count; i++)
        {
            if (Chunks[i].ChunkType == chunkType)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Gets the first chunk of the specified type in the PNG metadata, or creates a new one if it does not exist.
    /// </summary>
    /// <typeparam name="T">
    /// The type of chunk to get or create.
    /// </typeparam>
    /// <param name="repositionExistingChunk">
    /// If <c>true</c> and the chunk already exists, it will be repositioned according to the
    /// <paramref name="precedingChunkTypes"/> array. If <c>false</c>, the existing chunk will not be repositioned.
    /// An image-header chunk is always implied to be in this array.
    /// </param>
    /// <param name="precedingChunkTypes">
    /// When creating or repositioning, ensures the chunk is placed after the last chunk
    /// matching any of the types.
    /// </param>
    /// <returns>
    /// A chunk which was found or created of the requested type.
    /// </returns>
    /// <exception cref="Phaeyz.Png.PngException">
    /// A chunk was found, but the class type did not match the expected type.
    /// </exception>
    public T GetFirstOrCreate<T>(
        bool repositionExistingChunk,
        params IEnumerable<ChunkType> precedingChunkTypes) where T : Chunk, new() =>
            GetFirstOrCreate<T>(repositionExistingChunk, out _, out _, precedingChunkTypes);

    /// <summary>
    /// Gets the first chunk of the specified type in the PNG metadata, or creates a new one if it does not exist.
    /// </summary>
    /// <typeparam name="T">
    /// The type of chunk to get or create.
    /// </typeparam>
    /// <param name="repositionExistingChunk">
    /// If <c>true</c> and the chunk already exists, it will be repositioned according to the
    /// <paramref name="precedingChunkTypes"/> array. If <c>false</c>, the existing chunk will not be repositioned.
    /// An image-header chunk is always implied to be in this array.
    /// </param>
    /// <param name="created">
    /// Receives a boolean indicating whether or not the returned chunk was created during this
    /// call because it was not found.
    /// </param>
    /// <param name="index">
    /// Receives the index of the returned chunk.
    /// </param>
    /// <param name="precedingChunkTypes">
    /// When creating or repositioning, ensures the chunk is placed after the last chunk
    /// matching any of the specified types. If no types are specified, the chunk will be placed at the beginning.
    /// </param>
    /// <returns>
    /// A chunk which was found or created of the requested type.
    /// </returns>
    /// <exception cref="Phaeyz.Png.PngException">
    /// A chunk was found, but the class type did not match the expected type.
    /// </exception>
    public T GetFirstOrCreate<T>(
        bool repositionExistingChunk,
        out bool created,
        out int index,
        params IEnumerable<ChunkType> precedingChunkTypes) where T : Chunk, new()
    {
        // Find the existing chunk (if it exists).
        T? chunk = FindFirst<T>(out index);
        if (chunk is null)
        {
            // If the chunk didn't exist, so create one.
            chunk = new();
            // Get the index after all chunks which match the specified types.
            index = GetIndexAfter(
                precedingChunkTypes is null
                ? [ChunkType.ImageHeader]
                : precedingChunkTypes.Concat([ChunkType.ImageHeader]));
            // Insert and return.
            Chunks.Insert(index, chunk);
            created = true;
            return chunk;
        }

        created = false;

        // Reposition an existing chunk if requested.
        if (repositionExistingChunk)
        {
            // Get the index after all chunks which match the specified types.
            int targetIndex = GetIndexAfter(
                precedingChunkTypes is null
                ? [ChunkType.ImageHeader]
                : precedingChunkTypes.Concat([ChunkType.ImageHeader]));

            // If the index is before the target index, it means it is before one of the preceding chunks.
            if (index < targetIndex)
            {
                Chunks.RemoveAt(index);
                index = targetIndex - 1;
                Chunks.Insert(index, chunk);
            }
        }

        return chunk;
    }

    /// <summary>
    /// Gets the index after the last chunk with any of the specified types.
    /// </summary>
    /// <param name="chunkTypes">
    /// The types of the chunks to find.
    /// </param>
    /// <returns>
    /// Returns the index after the last chunk with any of the specified types, or <c>0</c> if none were found.
    /// </returns>
    public int GetIndexAfter(
        params IEnumerable<ChunkType> chunkTypes)
    {
        if (chunkTypes is not null)
        {
            for (int i = Chunks.Count - 1; i >= 0; i--)
            {
                if (chunkTypes.Contains(Chunks[i].ChunkType))
                {
                    return i + 1;
                }
            }
        }

        return 0;
    }

    /// <summary>
    /// Inserts a chunk after the last chunk with any of the specified types.
    /// </summary>
    /// <typeparam name="T">
    /// The type of chunk being inserted.
    /// </typeparam>
    /// <param name="chunk">
    /// The chunk to insert.
    /// </param>
    /// <param name="precedingChunkTypes">
    /// Inserts the chunk after the last chunk matching any of the specified types.
    /// If no types are specified, the chunk will be placed at the beginning.
    /// An image-header chunk is always implied to be in this array.
    /// </param>
    /// <returns>
    /// The index which the chunk was inserted at.
    /// </returns>
    public int Insert<T>(
        T chunk,
        params IEnumerable<ChunkType> precedingChunkTypes) where T : Chunk, new()
    {
        int index = GetIndexAfter(precedingChunkTypes is null
            ? [ChunkType.ImageHeader]
            : precedingChunkTypes.Concat([ChunkType.ImageHeader]));
        Chunks.Insert(index, chunk);
        return index;
    }

    /// <summary>
    /// Reads all PNG metadatas from the stream.
    /// </summary>
    /// <param name="stream">
    /// The stream to read the PNG metadata from.
    /// </param>
    /// <param name="chunkDefinitions">
    /// Optionally chunk definitions which maps chunk types to chunk class types.
    /// Specify <c>null</c> to use the default chunk definitions.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns a list of <see cref="Phaeyz.Png.PngMetadata"/> objects based on what has been discovered
    /// in the stream.
    /// </returns>
    /// <remarks>
    /// If the stream contains multiple back-to-back PNGs, this method will return all of them.
    /// </remarks>
    public static async ValueTask<List<PngMetadata>> ReadAllFromStreamAsync(
        MarshalStream stream,
        ChunkDefinitions? chunkDefinitions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        // Loop for each PNG metadata sequence.
        List<PngMetadata> pngMetadatas = [];
        while (true)
        {
            // Read the sequence.
            PngMetadata? pngMetadata = await ReadFromStreamAsync(
                stream,
                chunkDefinitions,
                cancellationToken).ConfigureAwait(false);

            // If a new sequence was not found, end the loop.
            if (pngMetadata is null)
            {
                break;
            }
            pngMetadatas.Add(pngMetadata);
        }

        return pngMetadatas;
    }

    /// <summary>
    /// Read a PNG metadata from the stream.
    /// </summary>
    /// <param name="stream">
    /// The stream to read the PNG metadata from.
    /// </param>
    /// <param name="pngDefinitions">
    /// Optionally chunk definitions which maps chunk keys to chunk class types.
    /// Specify <c>null</c> to use the default chunk definitions.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns a <see cref="Phaeyz.Png.PngMetadata"/> if the stream contains valid PNG metadata,
    /// otherwise <c>null</c> if a PNG signature could not be detected.
    /// </returns>
    /// <remarks>
    /// It is expected the last chunk be image-end.
    /// </remarks>
    public static async ValueTask<PngMetadata?> ReadFromStreamAsync(
        MarshalStream stream,
        ChunkDefinitions? pngDefinitions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        // Read the PNG signature.
        ChunkReader reader = new(stream, pngDefinitions);
        if (!await reader.ReadPngSignatureAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        // Read chunk until an image-end chunk is discovered.
        PngMetadata pngMetadata = new();
        for (Chunk? chunk = null; chunk is null || chunk.ChunkType != ChunkType.ImageEnd; pngMetadata.Chunks.Add(chunk!))
        {
            chunk = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        }
        return pngMetadata;
    }

    /// <summary>
    /// Removes all chunks of the specified type from the PNG metadata.
    /// </summary>
    /// <typeparam name="T">
    /// The type of chunk to remove.
    /// </typeparam>
    /// <returns>
    /// The number of matching chunks removed.
    /// </returns>
    public int RemoveAll<T>() where T : Chunk, new() => RemoveAll(ChunkAttribute.Get<T>().ChunkType);

    /// <summary>
    /// Removes all chunks with the specified chunk type from the PNG metadata.
    /// </summary>
    /// <param name="chunkType">
    /// The type of the chunk to remove.
    /// </param>
    /// <returns>
    /// The number of matching chunks removed.
    /// </returns>
    public int RemoveAll(ChunkType chunkType)
    {
        int countRemoved = 0;
        for (int i = 0; i < Chunks.Count;)
        {
            if (Chunks[i].ChunkType == chunkType)
            {
                Chunks.RemoveAt(i);
                countRemoved++;
            }
            else
            {
                i++;
            }
        }
        return countRemoved;
    }

    /// <summary>
    /// Removes the first chunk of the specified type from the PNG metadata.
    /// </summary>
    /// <typeparam name="T">
    /// The type of chunk to remove.
    /// </typeparam>
    /// <returns>
    /// Returns <c>true</c> if a chunk was removed, otherwise <c>false</c>.
    /// </returns>
    public bool RemoveFirst<T>() where T : Chunk, new() => RemoveFirst(ChunkAttribute.Get<T>().ChunkType);

    /// <summary>
    /// Removes the first chunk with the specified type from the PNG metadata.
    /// </summary>
    /// <param name="chunkType">
    /// The type of the chunk to remove.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if a chunk was removed, otherwise <c>false</c>.
    /// </returns>
    public bool RemoveFirst(ChunkType chunkType)
    {
        int index = FindFirstIndex(chunkType);
        if (index >= 0)
        {
            Chunks.RemoveAt(index);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Validates the chunks in the metadata matches the policies defined on the class, and if valid the order
    /// of the chunks is updated to adheres to required ordering rules.
    /// </summary>
    /// <exception cref="PngException">
    /// A validation error occurred on the list of chunks.
    /// </exception>
    public void ValidateAndReorderChunks() => Chunks = ValidateAndReorderChunks(Chunks);

    /// <summary>
    /// Validates the list of chunks matches the policies defined on the class, and if valid the order
    /// of the chunks is updated into a new list where the order adheres to required ordering rules.
    /// </summary>
    /// <param name="chunks">
    /// The chunks to validate and order.
    /// </param>
    /// <returns>
    /// A new list containing a new order of the chunks. The order may be the same if reordering was not necessary.
    /// </returns>
    /// <exception cref="PngException">
    /// A validation error occurred on the list of chunks.
    /// </exception>
    public static List<Chunk> ValidateAndReorderChunks(List<Chunk> chunks)
    {
        // Iterate all chunks, creating a table of chunk types with policy constraints.
        // While iterating, validate the chunks and their policies.
        Chunk? imageHeader = null;
        Chunk? imageEnd = null;
        Dictionary<ChunkType, ChunkTypeInfo> chunkTypeToInfo = [];
        foreach (var chunk in chunks)
        {
            // Only one IHDR may exist.
            if (chunk.ChunkType == ChunkType.ImageHeader)
            {
                if (imageHeader is not null)
                {
                    throw new PngException("The chunk list has multiple 'IHDR' chunks.");
                }
                imageHeader = chunk;
            }
            // Only one IEND may exist.
            else if (chunk.ChunkType == ChunkType.ImageEnd)
            {
                if (imageEnd is not null)
                {
                    throw new PngException("The chunk list has multiple 'IEND' chunks.");
                }
                imageEnd = chunk;
            }

            // Get the type and main attribute.
            var type = chunk.GetType();
            ChunkAttribute chunkAttr = type
                .GetCustomAttributes(typeof(ChunkAttribute), false)
                .Cast<ChunkAttribute>()
                .First();

            // Check to see if this chunk type has already been registered.
            if (chunkTypeToInfo.TryGetValue(chunk.ChunkType, out ChunkTypeInfo? info))
            {
                // info.AllowMultiple will be null until a chunk is registered to the chunk type.
                if (info.AllowMultiple.HasValue)
                {
                    // Since the chunk type has already been registered, the chunk type must allow multiple instances.
                    if (info.AllowMultiple == false || info.AllowMultiple != chunkAttr.AllowMultiple)
                    {
                        throw new PngException($"The chunk list has multiple '{chunk.ChunkType}' instances but only a single is allowed.");
                    }
                    continue;
                }
                info.AllowMultiple = chunkAttr.AllowMultiple;
            }
            else
            {
                // The chunk type was not registered, so register it now.
                info = new ChunkTypeInfo
                {
                    ChunkType = chunkAttr.ChunkType,
                    AllowMultiple = chunkAttr.AllowMultiple
                };
                // Store this chunk type info in the mapping.
                chunkTypeToInfo[chunkAttr.ChunkType] = info;
            }

            // Track all the chunks the current chunk must be ordered after.
            foreach (OrderAfterChunksAttribute orderAfter in type
                .GetCustomAttributes(typeof(OrderAfterChunksAttribute), false)
                .Cast<OrderAfterChunksAttribute>())
            {
                foreach (ChunkType orderAfterChunkType in orderAfter.ChunkTypes)
                {
                    if (orderAfterChunkType == info.ChunkType)
                    {
                        throw new PngException($"A '{chunk.ChunkType}' chunk may not reference itself in an order-after constraint.");
                    }
                    else if (orderAfterChunkType == ChunkType.ImageEnd)
                    {
                        throw new PngException("A chunk type may not have an OrderAfter constraint of 'IEND'.");
                    }
                    else if (info.ChunkType == ChunkType.ImageHeader)
                    {
                        throw new PngException("A 'IHDR' chunk may not have OrderAfter constraints.");
                    }

                    if (!info.OrderAfter.TryGetValue(orderAfterChunkType, out bool firstExempt) ||
                        (!firstExempt && orderAfter.FirstChunkIsExempt))
                    {
                        info.OrderAfter[orderAfterChunkType] = orderAfter.FirstChunkIsExempt;
                    }
                }
            }

            // If the current chunk says it must be ordered before another chunk type,
            // instead go to that chunk type and track it must be ordered after the current chunk type.
            foreach (OrderBeforeChunksAttribute orderBefore in type
                .GetCustomAttributes(typeof(OrderBeforeChunksAttribute), false)
                .Cast<OrderBeforeChunksAttribute>())
            {
                foreach (ChunkType orderBeforeChunkType in orderBefore.ChunkTypes)
                {
                    if (orderBeforeChunkType == info.ChunkType)
                    {
                        throw new PngException($"A '{chunk.ChunkType}' chunk may not reference itself in an order-before constraint.");
                    }
                    else if (orderBeforeChunkType == ChunkType.ImageHeader)
                    {
                        throw new PngException("A chunk type may not have an OrderBefore constraint of 'IHDR'.");
                    }
                    else if (info.ChunkType == ChunkType.ImageEnd)
                    {
                        throw new PngException("A 'IEND' chunk may not have OrderBefore constraints.");
                    }

                    if (chunkTypeToInfo.TryGetValue(orderBeforeChunkType, out ChunkTypeInfo? beforeInfo))
                    {
                        // An OrderBefore removes any exemption requested on OrderAfter.
                        beforeInfo.OrderAfter[info.ChunkType] = false;
                    }
                    else
                    {
                        // The referenced chunk type hasn't been encountered yet, so create a reference.
                        chunkTypeToInfo[orderBeforeChunkType] = new ChunkTypeInfo
                        {
                            ChunkType = orderBeforeChunkType,
                            OrderAfter = new Dictionary<ChunkType, bool>
                            {
                                [info.ChunkType] = false,
                            }
                        };
                    }
                }
            }
        }

        // Fail if IHDR or IEND was not found
        if (imageHeader is null)
        {
            throw new PngException("The chunk list does not have an 'IHDR' chunk.");
        }
        else if (imageEnd is null)
        {
            throw new PngException("The chunk list does not have an 'IEND' chunk.");
        }

        // Implements a Depth-First Search (DFS) algorithm to check for cycles in the chunk type graph.
        static bool HasCycle(
            Dictionary<ChunkType, ChunkTypeInfo> chunkTypeToInfo,
            ChunkType currentChunkType,
            HashSet<ChunkType> visitedChunkTypes,
            HashSet<ChunkType> chunkTypesInContext)
        {
            if (chunkTypesInContext.Contains(currentChunkType))
            {
                return true;
            }

            if (visitedChunkTypes.Contains(currentChunkType))
            {
                return false;
            }

            visitedChunkTypes.Add(currentChunkType);
            chunkTypesInContext.Add(currentChunkType);

            if (chunkTypeToInfo.TryGetValue(currentChunkType, out var info))
            {
                foreach (ChunkType orderAfterChunkType in info.OrderAfter.Keys)
                {
                    if (HasCycle(chunkTypeToInfo, orderAfterChunkType, visitedChunkTypes, chunkTypesInContext))
                    {
                        return true;
                    }
                }
            }

            chunkTypesInContext.Remove(currentChunkType);
            return false;
        }

        // Check to see if there any cycles in the chunk type graph.
        var visitedChunkTypes = new HashSet<ChunkType>();
        var chunkTypesInContext = new HashSet<ChunkType>();
        if (chunkTypeToInfo.Keys.Any(chunkType => HasCycle(
            chunkTypeToInfo,
            chunkType,
            visitedChunkTypes,
            chunkTypesInContext)))
        {
            throw new PngException("The chunk types in the list contain a cycle in the ordering constraints.");
        }

        // Begin the process of fixing the order based on the chunk type information.
        // Pair each chunk with its original index.
        LinkedList<Chunk> chunksToPlace = new(
            chunks.Where(chunk => chunk.ChunkType != ChunkType.ImageHeader && chunk.ChunkType != ChunkType.ImageEnd));

        List<Chunk> result = new(chunks.Count)
        {
            imageHeader
        };

        // Track number of instances placed per type.
        Dictionary<ChunkType, (int PlacedCount, int TotalCount)> placedChunkCounts = chunksToPlace
            .Select(chunk => chunk.ChunkType)
            .GroupBy(chunkType => chunkType)
            .ToDictionary(group => group.Key, group => (0, group.Count()));

        // Loop until all chunks are placed.
        while (chunksToPlace.Count != 0)
        {
            bool chunkPlaced = false;

            // Find the next placeable chunk and place it.
            for (LinkedListNode<Chunk>? chunkNode = chunksToPlace.First; chunkNode is not null; chunkNode = chunkNode.Next)
            {
                bool canPlace = true;
                ChunkType chunkType = chunkNode.Value.ChunkType;

                // Check to see if the current chunk can be placed.
                foreach ((ChunkType orderAfterChunkType, bool orderAfterFirstExempt) in chunkTypeToInfo[chunkType].OrderAfter)
                {
                    // The header is always placed first. Also, do not enforce policies for non-existent chunks.
                    if (!placedChunkCounts.ContainsKey(orderAfterChunkType))
                    {
                        continue;
                    }
                    // If the current chunk cannot be placed yet due to constraints, skip and move to the next.
                    (int placedCount, _) = placedChunkCounts[chunkType];
                    (int orderAfterPlacedCount, int orderAfterTotalCount) = placedChunkCounts[orderAfterChunkType];
                    bool isFirstAndExempt = placedCount == 0 && orderAfterFirstExempt;
                    bool constraintAlreadyPlaced = orderAfterPlacedCount == orderAfterTotalCount;
                    if (!isFirstAndExempt && !constraintAlreadyPlaced)
                    {
                        canPlace = false;
                        break;
                    }
                }

                // Place the chunk as necessary.
                if (canPlace)
                {
                    result.Add(chunkNode.Value);
                    (int placedCount, int totalCount) = placedChunkCounts[chunkType];
                    placedChunkCounts[chunkType] = (placedCount + 1, totalCount);
                    chunksToPlace.Remove(chunkNode);
                    chunkPlaced = true;
                    break;
                }
            }

            // This should never happen because the configuration was thoroughly validated.
            if (!chunkPlaced)
            {
                throw new PngException("An unknown error occurred while ordering chunks.");
            }
        }

        result.Add(imageEnd);
        return result;
    }

    /// <summary>
    /// Writes all PNG metadatas to the stream.
    /// </summary>
    /// <param name="stream">
    /// The stream to write the PNG metadata to.
    /// </param>
    /// <param name="pngMetadatas">
    /// An enumerable of <see cref="Phaeyz.Png.PngMetadata"/> objects to write to the stream.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when all PNG metadata has been written to the stream.
    /// </returns>
    public static async ValueTask WriteAllToStreamAsync(
        MarshalStream stream,
        IEnumerable<PngMetadata> pngMetadatas,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(pngMetadatas);
        foreach (PngMetadata pngMetadata in pngMetadatas)
        {
            ArgumentNullException.ThrowIfNull(pngMetadatas, nameof(pngMetadatas));
            await pngMetadata.WriteToStreamAsync(stream, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Writes the PNG metadata to the stream.
    /// </summary>
    /// <param name="stream">
    /// The stream to write the PNG metadata to.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the PNG metadata has been written to the stream.
    /// </returns>
    public async ValueTask WriteToStreamAsync(
        MarshalStream stream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ChunkWriter writer = new(stream);
        if (Chunks.Count > 0)
        {
            await writer.WritePngSignatureAsync(cancellationToken).ConfigureAwait(false);
            foreach (Chunk chunk in Chunks)
            {
                await writer.WriteAsync(chunk, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}

/// <summary>
/// Used internally for validating and ordering chunks.
/// </summary>
file class ChunkTypeInfo
{
    /// <summary>
    /// The chunk type for this instance.
    /// </summary>
    public required ChunkType ChunkType { get; set; }

    /// <summary>
    /// When a chunk is first registered to this type, it hydrates this value to whether or not multiple are allowed.
    /// </summary>
    public bool? AllowMultiple { get; set; }

    /// <summary>
    /// A dictionary where keys are the chunks to be ordered after, and the values are boolean indicating if the
    /// first chunk is exempt from the policy.
    /// </summary>
    public Dictionary<ChunkType, bool> OrderAfter { get; set; } = [];
}