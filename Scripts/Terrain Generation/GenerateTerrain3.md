# GenerateTerrain3 - Warped Multi-Layer Terrain Algorithm

## Overview
An advanced terrain generation algorithm featuring **spatial warping** and **multi-scale noise layering**. Creates dramatic, alien-looking landscapes with extreme vertical variation and complex terrain features. This algorithm produces some of the most visually striking and unusual planetary surfaces.

## Algorithm Type
**Warped Procedural Noise** - Uses sinusoidal domain warping with multi-layer FBM

## Characteristics

### Visual Appearance
- Dramatic, alien landscapes
- Extreme height variations
- Complex, swirling terrain patterns
- Sharp ridges and deep valleys
- Highly varied biome distribution
- Non-uniform, exotic topography

### Key Innovation: **Domain Warping**

The defining feature of this algorithm is the **sinusoidal warping** applied before noise generation:

```csharp
var num7 = num4 + Math.Sin(num5 * 0.15) * 3.0;  // Warp X with Y
var num8 = num5 + Math.Sin(num6 * 0.15) * 3.0;  // Warp Y with Z
var num9 = num6 + Math.Sin(num7 * 0.15) * 3.0;  // Warp Z with X
```

**Effect**: Creates swirling, flowing patterns that don't align with simple coordinate axes, resulting in more organic and unpredictable terrain.

## Mathematical Breakdown

### 1. Coordinate Warping (Pre-Processing)
```csharp
worldX = vertex.x * radius
worldY = vertex.y * radius
worldZ = vertex.z * radius

warpedX = worldX + sin(worldY * 0.15) * 3.0
warpedY = worldY + sin(worldZ * 0.15) * 3.0
warpedZ = worldZ + sin(warpedX * 0.15) * 3.0
```

**Parameters**:
- **Frequency**: 0.15 (controls how tight the swirls are)
- **Amplitude**: 3.0 units (strength of warping effect)
- **Circular dependency**: Z depends on X, creating complex patterns

**Visual Result**: Terrain features curve and swirl instead of following regular patterns

### 2. Primary Terrain Layer (6 Octaves)
```csharp
noise1 = simplexNoise1.Noise3DFBM(
    warpedX * 0.007 * 1.0,
    warpedY * 0.007 * 1.1,  // Slightly different scale
    warpedZ * 0.007 * 1.0,
    6,                       // High detail
    deltaWLen: 1.8          // Custom wavelength decrease
)
```

**Purpose**: Creates primary terrain structure

**Features**:
- **Custom deltaWLen (1.8)**: Controls how quickly detail decreases at each octave
  - Standard is 2.0
  - 1.8 makes higher octaves more prominent
  - Results in more persistent fine details

### 3. Secondary Detail Layer (3 Octaves)
```csharp
noise2 = simplexNoise2.Noise3DFBM(
    warpedX * 0.007 * 1.3 + 0.5,  // Different scale + offset
    warpedY * 0.007 * 2.8 + 0.2,  // Much different on Y
    warpedZ * 0.007 * 1.3 + 0.7,
    3
) * 2.0
```

**Purpose**: Adds medium-scale features

**Key Points**:
- **Non-uniform scaling** (1.3, 2.8, 1.3)
- **Phase offsets** (0.5, 0.2, 0.7) prevent alignment with primary layer
- **Amplitude × 2**: Strong influence on final terrain

### 4. Fine Detail Layer (2 Octaves)
```csharp
noise3 = simplexNoise2.Noise3DFBM(
    warpedX * 0.007 * 6.0,   // High frequency
    warpedY * 0.007 * 12.0,  // Very high on Y
    warpedZ * 0.007 * 6.0,
    2
) * 2.0
```

**Purpose**: Adds small-scale roughness

**Characteristics**:
- **6-12× higher frequency** than primary layer
- Creates fine surface texture
- Used selectively in final combination

### 5. Continental Scale Layer (2 Octaves)
```csharp
noise4 = simplexNoise2.Noise3DFBM(
    warpedX * 0.007 * 0.8,  // Lower frequency
    warpedY * 0.007 * 0.8,
    warpedZ * 0.007 * 0.8,
    2
) * 2.0
```

**Purpose**: Creates large-scale terrain variations

**Effect**: Subtle, broad elevation changes

### 6. Complex Terrain Combination

```csharp
// Combine layers with conditional logic
baseValue = noise1 * 2.0 + 0.92 + 
            Clamp01(noise2 * Abs(noise4 + 0.5) - 0.35)

// Apply double-negative scaling to underwater areas
if (baseValue < 0.0) {
    baseValue *= 2.0
}

// Apply leveling
leveledValue = Levelize2(baseValue)
if (leveledValue > 0.0) {
    leveledValue = Levelize4(Levelize2(baseValue))  // Double-leveling!
}
```

**Key Features**:
- **Conditional amplification**: Noise2 only affects where noise4 is significant
- **Asymmetric scaling**: Negative values (oceans) are amplified
- **Double-leveling**: Terrain above sea level is leveled twice for dramatic effect

### 7. Height Calculation with Stepped Transitions

```csharp
finalHeight = 
    if (leveledValue <= 0.0):
        Lerp(-4.0, 0.0, leveledValue + 1.0)
    else if (leveledValue <= 1.0):
        Lerp(0.0, 0.3, leveledValue) + noise3 * 0.1
    else if (leveledValue <= 2.0):
        Lerp(0.3, 1.4, leveledValue - 1.0) + noise3 * 0.12
    else:
        Lerp(1.4, 2.7, leveledValue - 2.0) + noise3 * 0.12
```

**Terrain Zones**:
- **< 0**: Deep underwater (interpolated from -4 to 0)
- **0-1**: Coastal/lowlands (0 to 0.3 height)
- **1-2**: Hills/mountains (0.3 to 1.4 height)  
- **> 2**: High mountains (1.4 to 2.7 height)

**Note**: Fine detail (noise3) is added progressively more to higher elevations

### 8. Biome Calculation

```csharp
// Reuse base value but with different processing
biomeBase = Abs(baseValue)
clampedBiome = (biomeBase <= 0.0) ? 0.0 : 
               (biomeBase <= 2.0) ? biomeBase : 2.0

biomeValue = clampedBiome + 
             (clampedBiome <= 1.8) ? noise3 * 0.2 : -noise3 * 0.8
```

**Features**:
- **Absolute value** of terrain base creates different biome from height
- **Conditional noise**: Adds roughness to lower biomes, smooths high ones
- Results in biomes that don't perfectly match elevation

## Parameters

### Hard-Coded Constants

| Parameter | Value | Effect |
|-----------|-------|--------|
| **Base scale** | 0.007 | Overall terrain frequency |
| **Warp frequency** | 0.15 | How tight the swirls are |
| **Warp amplitude** | 3.0 | Strength of warping |
| **DeltaWLen** | 1.8 | Octave wavelength decrease rate |

### Theme Settings Used

| Parameter | Default | Effect |
|-----------|---------|--------|
| **HeightMulti** | 1.0 | Final height amplitude multiplier |
| **BaseHeight** | 0.0 | Vertical offset for entire terrain |
| **BiomeHeightMulti** | 1.0 | Biome value amplitude |
| **BiomeHeightModifier** | 0.0 | Biome value offset |

## Data Output

### Height Data
```csharp
height = (radius + finalHeight * HeightMulti + 0.2 + BaseHeight) * 100.0
```

### Biome Data
```csharp
biome = Clamp(biomeValue * 100.0 * BiomeHeightMulti + BiomeHeightModifier, 0, 200)
```

## Use Cases

### Ideal For:
- ✅ Alien/exotic worlds
- ✅ High-contrast dramatic landscapes  
- ✅ Visually striking planets
- ✅ Unusual/memorable terrain
- ✅ Science fiction settings
- ✅ Showcase/beauty shots

### Not Suitable For:
- ❌ Realistic Earth-like planets
- ❌ Subtle, gentle terrain
- ❌ Low-variation worlds
- ❌ Performance-critical scenarios

## Performance

**Computational Cost**: ⭐⭐⭐⭐ High

- **2 simplex noise generators**
- **4 separate FBM calculations** per vertex
- **6 + 3 + 2 + 2 = 13 total octaves** processed
- **Sinusoidal warping** (3 sin calculations)
- **Complex conditional logic**
- **Double-leveling** operations
- Estimated: ~150ms for 200k vertices

## Visual Characteristics

```
Typical Terrain Profile:
        ⛰️
       /||\
      / || \      ← Sharp peaks
     /  ||  \
    /   ||   \
~~~~/   ||    \~~~~  ← Dramatic transitions
  Deep  ||  Valleys
  Ocean ||
```

## Algorithm Flow

```
1. Initialize random generator with seed
2. Create two simplex noise generators
3. Get terrain settings from theme
4. For each vertex:
   ├─ Convert to world coordinates
   ├─ Apply sinusoidal domain warping
   │  ├─ Warp X with sin(Y)
   │  ├─ Warp Y with sin(Z)
   │  └─ Warp Z with sin(X)
   ├─ Generate primary noise (6 octaves, deltaWLen=1.8)
   ├─ Generate secondary noise (3 octaves)
   ├─ Generate fine detail noise (2 octaves, high frequency)
   ├─ Generate continental noise (2 octaves, low frequency)
   ├─ Combine with complex conditional logic
   ├─ Apply double-negative amplification
   ├─ Apply leveling (sometimes double-leveling)
   ├─ Calculate height with zone-based interpolation
   ├─ Calculate biome with conditional noise
   └─ Store clamped values
5. Complete
```

## Comparison with Other Algorithms

| Feature | Terrain3 | Terrain1 | Terrain5 | Terrain6 |
|---------|----------|----------|----------|----------|
| Domain Warping | ✅ Yes | ❌ No | ❌ No | ❌ No |
| Total Octaves | 13 | 9 | 8 | 8 |
| Complexity | Very High | Moderate | Moderate | High |
| Drama Level | Extreme | Balanced | Balanced | High |
| Realism | Low | High | Moderate | High |
| CPU Cost | Highest | Moderate | Low | Moderate |

## Special Features

### 1. **Sinusoidal Domain Warping**
The circular dependency in warping creates unique, flowing patterns impossible with simple noise.

### 2. **Custom deltaWLen**
Using 1.8 instead of standard 2.0 makes terrain more "rough" at all scales.

### 3. **Zone-Based Height Mapping**
Different elevation ranges use different interpolation schemes, creating distinct terrain types.

### 4. **Double-Leveling**
Positive terrain is leveled twice, creating plateau-like structures.

### 5. **Independent Biomes**
Biomes use absolute values and different combinations, creating variety.

## Configuration Example

```json
{
  "algorithm": "GenerateTerrain3",
  "settings": {
    "HeightMulti": 2.0,
    "BaseHeight": 0.5,
    "BiomeHeightMulti": 1.5,
    "BiomeHeightModifier": 0.2
  },
  "description": "Dramatic alien world with extreme terrain"
}
```

## Notes

- **Most computationally expensive** standard terrain algorithm
- Creates the **most dramatic and alien** landscapes
- Best used for **memorable showcase planets**
- **Domain warping** is the signature feature that sets this apart
- Can create terrain that's **difficult to navigate** due to extreme variations
- **Biomes don't correlate directly with elevation**, adding to alien feel


