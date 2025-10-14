# GenerateTerrain5 - Continent/Ocean Terrain Algorithm

## Overview
A specialized terrain generation algorithm focused on creating clear **land/ocean boundaries** with realistic continental formations. Uses a threshold-based approach to generate distinct landmasses and ocean basins, similar to vanilla DSP terrain but with enhanced customization.

## Algorithm Type
**Threshold-Based Procedural Noise** - Uses noise masking to create distinct land/ocean regions

## Characteristics

### Visual Appearance
- Clear continent/ocean boundaries
- Realistic landmass shapes
- Smooth coastal transitions
- Moderate terrain variation on land
- Gentle ocean floors
- Earth-like appearance

### Key Innovation: **Threshold Masking**

The core of this algorithm is using noise thresholds to create binary land/ocean decisions:

```csharp
landMask = Abs(noise(position))
landValue = (0.16 - landMask) * 10.0
// Clamp to 0-1, then square for sharp boundaries
landValue = Clamp(landValue, 0, 1)²
```

**Effect**: When `landMask < 0.16`, terrain becomes land; otherwise, it becomes ocean. The squaring creates sharp, clear coastlines.

## Mathematical Breakdown

### 1. Coordinate Pre-Processing with Noise Offset
```csharp
worldX = vertex.x * radius + xFactor
worldY = vertex.y * radius + yFactor  
worldZ = vertex.z * radius + zFactor

// Levelize coordinates
x' = Levelize(worldX * 0.007)
y' = Levelize(worldY * 0.007)
z' = Levelize(worldZ * 0.007)

// Add small-scale noise wobble
x'' = x' + Noise(world * 0.05) * 0.04 * (1 + LandModifier)
y'' = y' + Noise(world * 0.05) * 0.04 * (1 + LandModifier)
z'' = z' + Noise(world * 0.05) * 0.04 * (1 + LandModifier)
```

**Purpose**: 
- **Levelize**: Redistributes coordinates for better noise distribution
- **Noise wobble**: Prevents perfectly regular patterns
- **LandModifier**: Controls how irregular coastlines are

### 2. Land/Ocean Threshold Calculation
```csharp
maskNoise = Abs(simplexNoise2.Noise(x'', y'', z''))

// Create threshold-based land value
rawLandValue = (0.16 - maskNoise) * 10.0

// Clamp to 0-1 range
landValue = Clamp(rawLandValue, 0, 1)

// Square it for sharper boundaries
landMask = landValue²
```

**Breakdown**:
1. **Threshold: 0.16** - Critical value separating land from ocean
2. **Multiply by 10**: Creates steep transition at threshold
3. **Square**: Makes coastlines even sharper and more defined

**Result**:
- `maskNoise < 0.16`: Land (landMask approaches 1)
- `maskNoise > 0.16`: Ocean (landMask = 0)
- Near threshold: Coastline (landMask 0-1)

### 3. Continental Scale Features
```csharp
continentalNoise = simplexNoise.Noise3DFBM(
    worldY * 0.005,  // Note: uses Y,Z,X instead of X,Y,Z
    worldZ * 0.005,
    worldX * 0.005,
    4  // octaves
)

continentalFeature = (continentalNoise + 0.22) * 5.0
continentalFeature = Clamp(continentalFeature, 0, 1)
```

**Purpose**: Creates large-scale elevation variations across continents

**Key Points**:
- **Low frequency** (0.005): Continental-sized features
- **Rotated coordinates**: Different pattern from land mask
- **4 octaves**: Moderate detail level

### 4. Detail Noise Layer
```csharp
detailNoise = Abs(simplexNoise2.Noise3DFBM(
    x'' * 1.5,
    y'' * 1.5,
    z'' * 1.5,
    2
)) * RandomFactor
```

**Purpose**: Adds small-scale terrain roughness

**Characteristics**:
- **1.5× higher frequency** than mask
- **2 octaves**: Light detail
- **RandomFactor**: Controls detail intensity
- **Absolute value**: Always positive contribution

### 5. Secondary Detail for Biomes
```csharp
biomeNoise = simplexNoise.Noise3DFBM(
    worldZ * 0.06,  // Again, rotated
    worldY * 0.06,
    worldX * 0.06,
    2
)
```

**Purpose**: Creates biome variation independent of terrain height

### 6. Height Calculation
```csharp
height = 0.0
height -= landMask * 1.2 * continentalFeature  // Land depression

if (height >= 0.0) {
    height += maskNoise * 0.25 + detailNoise * 0.6  // Add roughness
}

height -= 0.1  // Lower baseline

// Smooth ocean floors
oceanDepth = -0.3 - height
oceanDepth *= (1 + LandModifier)
if (oceanDepth > 0.0) {
    oceanDepth = Clamp(oceanDepth, 0, 1)
    oceanDepth = (3 - 2*oceanDepth) * oceanDepth²  // Cubic smoothing
    height = -0.3 - oceanDepth * 3.7
}
```

**Step-by-Step**:
1. **Land formation**: Where landMask is high, terrain is lowered by continents
2. **Above zero**: Add surface roughness
3. **Baseline shift**: -0.1 overall
4. **Ocean smoothing**: Cubic function creates gentle underwater slopes
   - **-0.3**: Ocean "shelf" depth
   - **3.7**: Maximum ocean depth multiplier

**Result**: Land has varied terrain; oceans are smooth and gentle

### 7. Biome Calculation  
```csharp
biome = maskNoise * 2.1  // Base from land mask

// Amplify negative biomes
if (biome < 0.0) {
    biome *= 5.0
}

// Clamp range
biome = Clamp(biome, -1.0, 2.0)

// Add noise variation
biome += (biome <= 1.8) ? 
    biomeNoise * 0.6 * biome :   // Add to lower biomes
    0.0                          // None to high biomes
```

**Features**:
- **Ocean biomes**: Highly negative (×5)
- **Land biomes**: 0 to 2.1 range
- **Conditional noise**: More variation in typical biomes

## Parameters

### Theme Settings Used

| Parameter | Default | Effect |
|-----------|---------|--------|
| **xFactor** | 0.0 | World coordinate offset (X) |
| **yFactor** | 0.0 | World coordinate offset (Y) |
| **zFactor** | 0.0 | World coordinate offset (Z) |
| **HeightMulti** | 1.0 | Final height amplitude |
| **BaseHeight** | 0.0 | Vertical baseline offset |
| **LandModifier** | 0.0 | Affects coastline irregularity and ocean depth |
| **RandomFactor** | 1.0 | Detail noise intensity |
| **BiomeHeightMulti** | 1.0 | Biome value amplitude |
| **BiomeHeightModifier** | 0.0 | Biome baseline offset |

### Hard-Coded Constants

| Constant | Value | Purpose |
|----------|-------|---------|
| **Land threshold** | 0.16 | Noise value below which land forms |
| **Threshold steepness** | 10.0 | How sharp land/ocean boundary is |
| **Ocean shelf** | -0.3 | Coastal shelf depth |
| **Max ocean depth** | 3.7 | Deep ocean floor multiplier |
| **Continental scale** | 0.005 | Size of continental features |

## Data Output

### Height Data
```csharp
finalHeight = (radius + height + 0.2 + BaseHeight) * 100.0 * HeightMulti
```

### Biome Data
```csharp
finalBiome = Clamp((biome + BiomeHeightModifier) * 100.0 * BiomeHeightMulti, 0, 200)
```

## Use Cases

### Ideal For:
- ✅ Earth-like planets with clear continents
- ✅ Ocean worlds with archipelagos
- ✅ Planets with strong land/water contrast
- ✅ Vanilla DSP-style terrain
- ✅ Moderate terrain variation
- ✅ Realistic planetary geography

### Not Suitable For:
- ❌ All-land or all-ocean worlds
- ❌ Extreme vertical terrain
- ❌ Alien/exotic landscapes
- ❌ High-detail surface features

## Performance

**Computational Cost**: ⭐⭐ Moderate

- **2 simplex noise generators**
- **8 total octaves** (4 + 2 + 2)
- **Simple conditional logic**
- **Cubic smoothing** calculation
- Estimated: ~60ms for 200k vertices

## Visual Examples

### Typical Planet Appearance
```
    🏔️ Mountains (continental features)
  🌲🌲🌳 Forests (varied biomes)
 ~~~~~~~~ Sharp coastlines
🌊🌊🌊🌊 Smooth ocean floors
```

### Height Profile
```
        ▲
       /|\      ← Moderate peaks
      / | \
~~~~~   |  ~~~~ ← Sharp coastline
     \  |  /
      \ | /     ← Smooth ocean
       \|/
        ▼
```

## Algorithm Flow

```
1. Initialize random generator with seed
2. Create two simplex noise generators
3. Get terrain settings from theme
4. For each vertex:
   ├─ Convert to world coordinates + offsets
   ├─ Levelize coordinates
   ├─ Add noise wobble to coordinates
   ├─ Calculate land mask from threshold
   │  ├─ Get absolute noise value
   │  ├─ Apply threshold (0.16)
   │  ├─ Multiply by 10 for steepness
   │  ├─ Clamp and square
   ├─ Generate continental features (4 octaves)
   ├─ Generate detail noise (2 octaves)
   ├─ Generate biome noise (2 octaves)
   ├─ Calculate height
   │  ├─ Apply land depression
   │  ├─ Add surface roughness if above zero
   │  ├─ Subtract baseline
   │  ├─ Apply ocean floor smoothing
   ├─ Calculate biome
   │  ├─ Base from mask noise
   │  ├─ Amplify ocean biomes
   │  ├─ Add conditional noise
   ├─ Apply theme multipliers
   └─ Store clamped values
5. Complete
```

## Comparison with Other Algorithms

| Feature | Terrain5 | Terrain1 | Terrain3 | Terrain6 |
|---------|----------|----------|----------|----------|
| Land/Ocean Clarity | ✅ Sharp | Moderate | Low | Sharp |
| Total Octaves | 8 | 9 | 13 | 8 |
| Ocean Smoothing | ✅ Cubic | Linear | None | Cubic |
| CPU Cost | Moderate | Moderate | High | Moderate |
| Realism | High | Very High | Low | High |
| Vanilla-Like | ✅ Yes | No | No | Somewhat |

## Special Features

### 1. **Threshold-Based Land Mask**
Creates clear continents vs oceans rather than gradual elevation changes.

### 2. **Cubic Ocean Floor Smoothing**
```csharp
smoothed = (3 - 2*depth) * depth²
```
Creates natural-looking underwater slopes.

### 3. **Coordinate Rotation**
Continental and biome noise use Y,Z,X instead of X,Y,Z, creating different patterns.

### 4. **Levelize Pre-Processing**
Redistributes coordinate values for better noise characteristics.

### 5. **Conditional Detail**
Roughness only added to above-water terrain.

## Configuration Example

```json
{
  "algorithm": "GenerateTerrain5",
  "settings": {
    "HeightMulti": 1.0,
    "BaseHeight": 0.0,
    "LandModifier": 0.2,
    "RandomFactor": 1.0,
    "BiomeHeightMulti": 1.0,
    "BiomeHeightModifier": 0.0
  },
  "description": "Earth-like world with clear continents and oceans"
}
```

## Notes

- Most similar to **vanilla DSP terrain generation**
- **LandModifier** has dual effect: coastline irregularity AND ocean depth
- **Cubic smoothing** creates more realistic underwater topography than linear
- **0.16 threshold** was likely tuned to create ~30-40% land coverage
- Coordinate **levelize** is a signature feature for better noise distribution
- Works well for **habitable zone** planets in the game


