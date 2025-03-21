# SuggestedPaletteEntry.Frequency property

The frequency value indicates the relative importance or frequency of use of each color in the image. Higher frequency values suggest that the color is used more frequently.

```csharp
public ushort Frequency { get; set; }
```

## Remarks

Each frequency value is proportional to the fraction of the pixels in the image for which that palette entry is the closest match in RGBA space, before the image has been composited against any background. The exact scale factor is chosen by the PNG encoder; it is recommended that the resulting range of individual values reasonably fills the range `0` to `65535`. A PNG encoder may artificially inflate the frequencies for colors considered to be "important", for example the colors used in a logo or the facial features of a portrait. Zero is a valid frequency meaning that the color is "least important" or that it is rarely, if ever, used. When all the frequencies are zero, they are meaningless, that is to say, nothing may be inferred about the actual frequencies with which the colors appear in the PNG image.

## See Also

* struct [SuggestedPaletteEntry](../SuggestedPaletteEntry.md)
* namespace [Phaeyz.Png.Chunks](../../Phaeyz.Png.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Png.dll -->
