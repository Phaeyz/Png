# OrderAfterChunksAttribute constructor

When a chunk class is decorated with this, it indicates which other chunks it must be ordered after.

```csharp
public OrderAfterChunksAttribute(params string[] chunkTypeLabels)
```

| parameter | description |
| --- | --- |
| chunkTypeLabels | Labels of the chunk types which a decorated chunk must be ordered after. |

## Remarks

Initializes a new instance of the [`OrderAfterChunksAttribute`](../OrderAfterChunksAttribute.md) class.

## See Also

* class [OrderAfterChunksAttribute](../OrderAfterChunksAttribute.md)
* namespace [Phaeyz.Png](../../Phaeyz.Png.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Png.dll -->
