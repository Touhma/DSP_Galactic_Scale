# GenerateTerrain6 - Island-Based Terrain Algorithm

## Overview
A highly customizable terrain generation algorithm that creates **archipelago-style planets** with island chains, varied landmasses, and smooth ocean transitions. Features the most comprehensive theme settings integration and creates dramatic coastlines with natural-looking biome distributions.

## Algorithm Type
**Advanced Threshold-Masked Noise** - Uses configurable noise layers with sharp land/ocean masking

## Characteristics

### Visual Appearance
- Island chains and archipelagos
- Clear land/ocean boundaries
- Dramatic coastal features
- Smooth underwater terrain
- Highly varied biomes
- Natural-looking coastlines
- Customizable land coverage

### Key Innovation: **Fully Parameterized Terrain**

Unlike other algorithms, Terrain6 exposes **all major parameters** through theme settings, making it the most customizable algorithm in the system.

## Mathematical Breakdown

### 1. Coordinate Processing with Theme Factors
```csharp
worldX = vertex.x * radius
worldY = vertex.y * radius
worldZ = vertex.z * radius

// Levelize with standard 0.007 scale
x' = Levelize(worldX * 0.007)
y' = Levelize(worldY * 0.007)
z' = Levelize(worldZ * 0.007)

// Add noise wobble with full theme control
x'' = x' + Noise(world * xFactor) * 0.04 * RandomFactor
y'' = y' + Noise(world * yFactor) * 0.04 * RandomFactor
z'' = z' + Noise(world * zFactor) * 0.04 * RandomFactor
```

**Key Difference**: Uses **theme factors** (xFactor, yFactor, zFactor) for noise scaling instead of hard-coded values.

### 2. Land/Ocean Threshold Mask
```csharp
maskNoise = Abs(simplexNoise2.Noise(x'', y'', z''))

// Configurable threshold
rawLandValue = (0.16 - maskNoise) * 10.0 * (1 + LandModifier)

// Clamp and square for sharp boundaries
landMask = Clamp(rawLandValue, 0, 1)²
```

**Enhanced Features**:
- **LandModifier integration**: Directly affects threshold steepness
  - Higher LandModifier = more land, sharper boundaries
  - Lower LandModifier = more ocean, softer boundaries

### 3. Continental Scale Features
```csharp
continentalNoise = simplexNoise.Noise3DFBM(
    worldY * 0.005,
    worldZ * 0.005,
    worldX * 0.005,
    4  // octaves
)

continentalHeight = (continentalNoise + 0.22) * 5.0
continentalHeight = Clamp(continentalHeight, 0, 1)
```

**Purpose**: Creates broad elevation variations across landmasses

### 4. Fine Detail Layer
```csharp
detailNoise = Abs(simplexNoise2.Noise3DFBM(
    x'' * 1.5,
    y'' * 1.5,
    z'' * 1.5,
    2
))
```

**Purpose**: Adds surface texture and small-scale features

### 5. Height Calculation
```csharp
height = 0.0

// Depress land based on mask and continental features
height -= landMask * 1.2 * continentalHeight

// Add roughness to above-water terrain
if (height >= 0.0) {
    height += maskNoise * 0.25 + detailNoise * 0.6
}

// Baseline adjustment
height -= 0.1

// Advanced ocean floor smoothing
underwaterDepth = -0.3 - height
if (underwaterDepth > 0.0) {
    underwaterDepth = Clamp(underwaterDepth, 0, 1)
    smoothed = (3.0 - 2*underwaterDepth) * underwaterDepth²
    height = -0.3 - smoothed * 3.7
}

// Apply all theme settings
finalHeight = height * HeightMulti + BaseHeight
```

**Final Output**:
```csharp
heightData = (radius + finalHeight + 0.2) * 100.0
```

### 6. Advanced Biome Calculation
```csharp
// Base biome from terrain
biomeBase = height * landMask + 
            (maskNoise * 2.1 * BiomeHeightMulti + 
             0.8 + BiomeHeightModifier)

// Prevent mid-range bunching
if (biomeBase > 1.7 && biomeBase < 2.0) {
    biomeBase = 2.0  // Jump over the gap
}

finalBiome = Clamp(biomeBase * 100.0, 0, 200)
```

**Special Feature**: The 1.7-2.0 gap prevents "bunching" of biome values in that range, creating more distinct biome transitions.

## Parameters

### Theme Settings Used (Most Comprehensive)

| Parameter | Default | Effect |
|-----------|---------|--------|
| **xFactor** | Variable | Noise frequency X-axis |
| **yFactor** | Variable | Noise frequency Y-axis |
| **zFactor** | Variable | Noise frequency Z-axis |
| **HeightMulti** | 3.0 | Terrain amplitude multiplier |
| **BaseHeight** | -2.5 to 2.5 | Vertical offset |
| **LandModifier** | -1.0 to 1.0 | Land/ocean ratio AND boundary sharpness |
| **RandomFactor** | 0.0 to 2.0 | Surface detail intensity |
| **BiomeHeightMulti** | 1.0 | Biome variation amplitude |
| **BiomeHeightModifier** | -1.0 to 1.0 | Biome baseline shift |

### Hard-Coded Constants

| Constant | Value | Purpose |
|----------|-------|---------|
| **Base threshold** | 0.16 | Land/ocean split point |
| **Threshold multiplier** | 10.0 | Boundary sharpness base |
| **Ocean shelf** | -0.3 | Coastal shelf depth |
| **Max ocean depth** | 3.7 | Deep ocean multiplier |
| **Land depression** | 1.2 | How much continents depress terrain |

## Terrain Customization Examples

### Island Archipelago
```json
{
  "LandModifier": -0.5,
  "HeightMulti": 2.0,
  "BaseHeight": 0.0,
  "RandomFactor": 1.5
}
```
**Result**: Many small islands, moderate mountains, high surface detail

### Continental World
```json
{
  "LandModifier": 0.5,
  "HeightMulti": 3.0,
  "BaseHeight": 0.2,
  "RandomFactor": 0.8
}
```
**Result**: Large continents, tall mountains, smoother terrain

### Ocean World with Islands
```json
{
  "LandModifier": -0.8,
  "HeightMulti": 1.5,
  "BaseHeight": -0.3,
  "RandomFactor": 1.0
}
```
**Result**: Mostly water, scattered islands, moderate terrain

## Data Output

### Height Data
```csharp
heightValue = (radius + height * HeightMulti + BaseHeight + 0.2) * 100.0
```

### Biome Data
```csharp
biomeValue = Clamp(
    (height * landMask + maskNoise * 2.1 * BiomeHeightMulti + 
     0.8 + BiomeHeightModifier) * 100.0,
    0,
    200
)
```

## Use Cases

### Ideal For:
- ✅ Archipelago planets (island chains)
- ✅ Highly customizable worlds
- ✅ Planets with specific land/ocean ratios
- ✅ Realistic coastal features
- ✅ Theme-driven procedural generation
- ✅ Varied biome distributions

### Not Suitable For:
- ❌ Featureless planets
- ❌ Extreme alien terrain
- ❌ All-land or all-ocean worlds (use Terrain0)
- ❌ Ultra-high detail requirements

## Performance

**Computational Cost**: ⭐⭐⭐ Moderate

- **2 simplex noise generators**
- **8 total octaves** (4 + 2 + 2)
- **Cubic ocean smoothing**
- **Multiple conditional branches**
- **Theme parameter integration**
- Estimated: ~80ms for 200k vertices

## Visual Examples

### Typical Appearance
```
     🏔️       🏔️      ← Island peaks
    🌲🌳     🌴🌴
   ~~~~~~  ~~~~~~~~    ← Sharp coastlines
 🌊     🌊🌊     🌊🌊  ← Island chains
🌊🌊🌊🌊🌊🌊🌊🌊🌊🌊   ← Smooth ocean
```

### Biome Distribution
The algorithm creates **distinct biome zones**:
- **0-70**: Ocean/underwater biomes
- **70-100**: Coastal/lowland biomes
- **100-140**: Mid-elevation biomes
- **140-170**: Highland biomes
- **Gap at 170-200**: Jumped to prevent bunching
- **200**: Highest mountain biomes

## Algorithm Flow

```
1. Initialize random generator with seed
2. Create two simplex noise generators
3. Get comprehensive terrain settings from theme
4. For each vertex:
   ├─ Convert to world coordinates
   ├─ Levelize coordinates (0.007 scale)
   ├─ Add theme-controlled noise wobble
   │  └─ Uses xFactor, yFactor, zFactor, RandomFactor
   ├─ Calculate land mask
   │  ├─ Get absolute noise value
   │  ├─ Apply threshold with LandModifier
   │  ├─ Clamp and square for sharp edges
   ├─ Generate continental features (4 octaves)
   ├─ Generate surface detail (2 octaves)
   ├─ Calculate height
   │  ├─ Apply land depression
   │  ├─ Add roughness if above water
   │  ├─ Baseline adjustment
   │  ├─ Cubic ocean floor smoothing
   │  └─ Apply HeightMulti and BaseHeight
   ├─ Calculate biome
   │  ├─ Combine height and mask
   │  ├─ Add maskNoise component
   │  ├─ Apply BiomeHeightMulti and Modifier
   │  └─ Jump 1.7-2.0 gap
   └─ Store clamped values
5. Complete
```

## Comparison with All Algorithms

| Feature | T0 | T1 | T3 | T5 | T6 |
|---------|----|----|----|----|-----|
| **Customization** | None | Medium | Low | Medium | ✅ **Highest** |
| **Land/Ocean** | N/A | Soft | Mixed | Sharp | Sharp |
| **Octaves** | 0 | 9 | 13 | 8 | 8 |
| **Theme Integration** | None | Partial | Minimal | Partial | ✅ **Full** |
| **Island Formation** | No | Poor | No | Moderate | ✅ **Excellent** |
| **CPU Cost** | Minimal | Moderate | Highest | Moderate | Moderate |
| **Realism** | N/A | Very High | Low | High | High |
| **Biome Variety** | None | High | Extreme | Moderate | ✅ **High** |

## Special Features

### 1. **Comprehensive Theme Integration**
Every major parameter exposed through theme settings for maximum customization.

### 2. **LandModifier Dual Effect**
Controls both land coverage AND boundary sharpness in one parameter.

### 3. **Biome Gap Jumping**
```csharp
if (biomeBase > 1.7 && biomeBase < 2.0) biomeBase = 2.0;
```
Prevents mid-range biome bunching for clearer distinctions.

### 4. **Island Chain Formation**
The combination of threshold masking and continental noise creates natural archipelago patterns.

### 5. **Cubic Ocean Smoothing**
Same gentle underwater slopes as Terrain5:
```csharp
smoothed = (3 - 2*depth) * depth²
```

## Advanced Configuration

### Parameter Interactions

**LandModifier Effects**:
- `-1.0`: Maximum ocean coverage, softest boundaries
- `0.0`: Balanced land/ocean, moderate boundaries  
- `+1.0`: Maximum land coverage, sharpest boundaries

**HeightMulti × BaseHeight**:
- High HeightMulti + Low BaseHeight = Deep valleys, tall peaks
- Low HeightMulti + High BaseHeight = Plateau-like terrain
- High both = Floating mountains
- Low both = Flat marshlands

**BiomeHeightMulti × BiomeHeightModifier**:
- Control biome diversity independently from terrain height
- Higher multiplier = more biome variation
- Positive modifier = shift toward warmer/higher biomes
- Negative modifier = shift toward colder/lower biomes

## Configuration Example

```json
{
  "algorithm": "GenerateTerrain6",
  "settings": {
    "xFactor": 0.01,
    "yFactor": 0.012,
    "zFactor": 0.01,
    "HeightMulti": 3.0,
    "BaseHeight": -0.5,
    "LandModifier": 0.0,
    "RandomFactor": 1.2,
    "BiomeHeightMulti": 1.5,
    "BiomeHeightModifier": 0.2
  },
  "description": "Balanced archipelago world with varied biomes"
}
```

## Notes

- **Most flexible** terrain algorithm in the system
- Perfect for **procedural theme systems** that need fine control
- **Island formation** is a natural outcome of the threshold masking approach
- The **biome gap jump** (1.7-2.0) was likely discovered through experimentation
- **Cubic smoothing** creates more realistic oceans than linear interpolation
- Works exceptionally well with **custom planet themes**
- Similar to Terrain5 but with **enhanced customization**


