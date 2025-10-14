# GenerateTerrain00 - Flat/Smooth Sphere Algorithm

## Overview
The simplest terrain generation algorithm that creates a perfectly smooth sphere with no terrain variation. This algorithm is typically used for gas giants or featureless planets.

## Algorithm Type
**Geometric** - No noise generation involved

## Characteristics

### Visual Appearance
- Perfectly smooth spherical surface
- No mountains, valleys, or terrain features
- Uniform biome distribution (value: 0)
- Constant radius across entire planet

### Technical Details

#### Height Generation
```csharp
data.heightData[i] = (ushort)(gsPlanet.planetData.radius * 100.1);
```

- **Height Calculation**: Planet radius × 100.1
- **Variation**: None (completely uniform)
- **Scale Factor**: 100.1 (slight offset above base radius)

#### Biome Generation
```csharp
data.biomoData[i] = 0;
```

- **Biome Value**: Always 0
- **Meaning**: Uniform biome type across entire surface
- **Variation**: None

## Parameters

| Parameter | Value | Effect |
|-----------|-------|--------|
| Height Multiplier | 100.1 | Constant across all vertices |
| Biome Value | 0 | Single biome type |
| Noise | None | No procedural variation |

## Use Cases

### Ideal For:
- ✅ Gas giants (Jupiter-like planets)
- ✅ Smooth ice planets
- ✅ Featureless ocean worlds
- ✅ Testing/debugging terrain systems
- ✅ Placeholder planets

### Not Suitable For:
- ❌ Rocky terrestrial planets
- ❌ Earth-like worlds with continents
- ❌ Planets requiring varied terrain

## Performance

**Computational Cost**: ⭐ Minimal (fastest algorithm)

- No noise calculations
- Simple arithmetic only
- O(n) complexity where n = vertex count
- Negligible CPU usage

## Code Flow

```
1. Validate planet object exists
2. Get planet data reference
3. For each vertex:
   └─ Set height to radius × 100.1
   └─ Set biome to 0
4. Complete
```

## Mathematical Formula

### Height at any point:
```
h = r × 100.1
```

Where:
- `h` = height data value
- `r` = planet radius

### Biome at any point:
```
b = 0 (constant)
```

## Comparison with Other Algorithms

| Feature | Terrain00 | Terrain1 | Terrain6 |
|---------|-----------|----------|----------|
| Complexity | Minimal | High | High |
| Noise Layers | 0 | 2 | 2 |
| Terrain Variety | None | High | High |
| Computation Time | ~1ms | ~100ms | ~80ms |
| Memory Usage | Minimal | Moderate | Moderate |

## Example Configuration

```json
{
  "algorithm": "GenerateTerrain00",
  "planetType": "GasGiant",
  "description": "Smooth featureless sphere"
}
```

## Notes

- This algorithm ignores all theme terrain settings (HeightMulti, BaseHeight, etc.)
- The 100.1 multiplier creates a slight offset above the base planet radius
- Perfect for scenarios where terrain detail is not needed
- Commonly used as a fallback when terrain generation fails


