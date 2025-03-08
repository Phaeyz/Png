# Phaeyz.Png assembly

## Phaeyz.Png namespace

| public type | description |
| --- | --- |
| abstract class [Chunk](./Phaeyz.Png/Chunk.md) | The base class for all chunks. |
| class [ChunkAttribute](./Phaeyz.Png/ChunkAttribute.md) | Chunk classes must be decorated with this attribute to indicate the chunk type. |
| class [ChunkDefinitions](./Phaeyz.Png/ChunkDefinitions.md) | Used by chunk readers to determine which classes to use for chunk types. |
| class [ChunkReader](./Phaeyz.Png/ChunkReader.md) | A reader for PNG chunks. |
| class [ChunkType](./Phaeyz.Png/ChunkType.md) | The type of a chunk. |
| class [ChunkWriter](./Phaeyz.Png/ChunkWriter.md) | A writer for PNG chunks. |
| class [CompressableText](./Phaeyz.Png/CompressableText.md) | Supports a delayed deserialization of non-null terminated and potentially compressed UTF-8 text within chunks. Also supports efficiently reserializing. |
| class [OrderAfterChunksAttribute](./Phaeyz.Png/OrderAfterChunksAttribute.md) | When a chunk class is decorated with this, it indicates which other chunks it must be ordered after. |
| class [OrderBeforeChunksAttribute](./Phaeyz.Png/OrderBeforeChunksAttribute.md) | When a chunk class is decorated with this, it indicates which other chunks it must be ordered before. |
| class [PngException](./Phaeyz.Png/PngException.md) | This exception is thrown if there is a serialization or deserialization issue. |
| class [PngMetadata](./Phaeyz.Png/PngMetadata.md) | Encapsulates a series of PNG chunks which make up a valid PNG file. |
| class [UnknownChunk](./Phaeyz.Png/UnknownChunk.md) | A chunk for which there is no defined class for the chunk type. The chunk data is available as a byte array. |

## Phaeyz.Png.Chunks namespace

| public type | description |
| --- | --- |
| class [AnimationControl](./Phaeyz.Png.Chunks/AnimationControl.md) | Declares that this is an animated PNG image, gives the number of frames, and the number of times to loop. |
| class [BackgroundColor](./Phaeyz.Png.Chunks/BackgroundColor.md) | Specifies a default background color to present the image against. |
| class [CodingIndependentCodePoints](./Phaeyz.Png.Chunks/CodingIndependentCodePoints.md) | The color space (primaries), transfer function, matrix coefficients and scaling factor of the image using the code points specified in [ITU-T-H.273]. |
| [Flags] enum [ColorType](./Phaeyz.Png.Chunks/ColorType.md) | The color types used in the PNG image. |
| class [CompressedTextualData](./Phaeyz.Png.Chunks/CompressedTextualData.md) | Stores a keyword and compressed text string. |
| enum [CompressionMethod](./Phaeyz.Png.Chunks/CompressionMethod.md) | The method used to compress the image data. |
| class [ContentLightLevelInformation](./Phaeyz.Png.Chunks/ContentLightLevelInformation.md) | Adds static metadata which provides an opportunity to optimize tone mapping of the associated content to a specific target display. This is accomplished by tailoring the tone mapping of the content itself to the specific peak brightness capabilities of the target display to prevent clipping. The method of tone-mapping optimization is currently subjective. |
| class [Exif](./Phaeyz.Png.Chunks/Exif.md) | Contains EXIF metadata. |
| enum [FilterMethod](./Phaeyz.Png.Chunks/FilterMethod.md) | The preprocessing method applied to the image data before compression. |
| enum [FrameAreaDisposal](./Phaeyz.Png.Chunks/FrameAreaDisposal.md) | The type of frame area disposal to be done after rendering this frame; in other words, it specifies how the output buffer should be changed at the end of the delay (before rendering the next frame). |
| enum [FrameBlendBehavior](./Phaeyz.Png.Chunks/FrameBlendBehavior.md) | Specifies whether the frame is to be alpha blended into the current output buffer content, or whether it should completely replace its region in the output buffer. |
| class [FrameControl](./Phaeyz.Png.Chunks/FrameControl.md) | The dimensions, position, delay and disposal of an individual frame. |
| class [FrameData](./Phaeyz.Png.Chunks/FrameData.md) | The set of fdAT chunks contains the image data for all frames (or, for animations which include the static image as first frame, for all frames after the first one). |
| class [Gamma](./Phaeyz.Png.Chunks/Gamma.md) | Specifies a gamma value. |
| class [Histogram](./Phaeyz.Png.Chunks/Histogram.md) | Gives the approximate usage frequency of each color in the palette. |
| class [IccProfile](./Phaeyz.Png.Chunks/IccProfile.md) | If the iCCP chunk is present, the image samples conform to the color space represented by the embedded ICC profile as defined by the International Color Consortium [ICC][ISO_15076-1]. |
| class [ImageData](./Phaeyz.Png.Chunks/ImageData.md) | Contains the actual image data which is the output stream of the compression algorithm. |
| class [ImageEnd](./Phaeyz.Png.Chunks/ImageEnd.md) | Marks the end of the PNG datastream. There must not be anymore chunks after this one. |
| class [ImageHeader](./Phaeyz.Png.Chunks/ImageHeader.md) | Image header. |
| enum [InterlaceMethod](./Phaeyz.Png.Chunks/InterlaceMethod.md) | The transmission order of the image data. |
| class [InternationalTextualData](./Phaeyz.Png.Chunks/InternationalTextualData.md) | Stores text data that can include international characters and language information. The iTXt chunk allows for more flexibility and supports text encoding in UTF-8, which can represent characters from virtually any language. |
| class [MasteringDisplayColorVolume](./Phaeyz.Png.Chunks/MasteringDisplayColorVolume.md) | Provides informative static metadata which allows a target (consumer) display to potentially optimize its tone mapping decisions on a comparison of its inherent capabilities versus the original mastering display's capabilities. |
| class [Palette](./Phaeyz.Png.Chunks/Palette.md) | A chunk for which there is no defined class for the chunk type. The chunk data is available as a byte array. |
| class [PhysicalPixelDimensions](./Phaeyz.Png.Chunks/PhysicalPixelDimensions.md) | The intended pixel size or aspect ratio for display of the image. |
| class [PngImageType](./Phaeyz.Png.Chunks/PngImageType.md) | A PNG image type. |
| class [PrimaryChromaticitiesAndWhitePoint](./Phaeyz.Png.Chunks/PrimaryChromaticitiesAndWhitePoint.md) | Used to specify the 1931 CIE x,y chromaticities of the red, green, and blue display primaries used in the PNG image, and the referenced white point. |
| class [SignificantBits](./Phaeyz.Png.Chunks/SignificantBits.md) | Defines the original number of significant bits (which can be less than or equal to the sample depth). This allows PNG decoders to recover the original data losslessly even if the data had a sample depth not directly supported by PNG. |
| class [StandardRgb](./Phaeyz.Png.Chunks/StandardRgb.md) | If the sRGB chunk is present, the image samples conform to the sRGB color space [SRGB] and should be displayed using the specified rendering intent defined by the International Color Consortium [ICC] or [ICC-2]. |
| enum [StandardRgbRenderingIntent](./Phaeyz.Png.Chunks/StandardRgbRenderingIntent.md) | The specified rendering intent defined by the International Color Consortium [ICC] or [ICC-2]. |
| class [SuggestedPalette](./Phaeyz.Png.Chunks/SuggestedPalette.md) | Provides a suggested palette for indexed-color images. This chunk is optional and is primarily used to improve the appearance of images when they are displayed on devices with limited color capabilities or when the image is converted to a different format that uses a limited color palette. |
| struct [SuggestedPaletteEntry](./Phaeyz.Png.Chunks/SuggestedPaletteEntry.md) | An entry within a [`SuggestedPalette`](./Phaeyz.Png.Chunks/SuggestedPalette.md) chunk. |
| class [TextualData](./Phaeyz.Png.Chunks/TextualData.md) | Stores a keyword and text string. |
| class [Time](./Phaeyz.Png.Chunks/Time.md) | The UTC time of the last image modification (not the time of initial image creation). |
| class [Transparency](./Phaeyz.Png.Chunks/Transparency.md) | Specifies either alpha values that are associated with palette entries (for indexed-color images) or a single transparent color (for greyscale and truecolor images). |
| enum [UnitSpecifier](./Phaeyz.Png.Chunks/UnitSpecifier.md) | Specifies the physical pixel unit specifier. |

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Png.dll -->
