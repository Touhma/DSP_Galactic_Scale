# GenerateTerrain1 - Dual-Layer Fractal Terrain Algorithm

## Overview
A sophisticated terrain generation algorithm using dual-layer simplex noise with Fractal Brownian Motion (FBM). Creates realistic planetary surfaces with mountains, valleys, plains, and varied biomes. This is one of the primary algorithms for Earth-like terrestrial planets.

## Algorithm Type
**Procedural Noise-Based** - Uses layered simplex noise with FBM

## Characteristics

### Visual Appearance
- Realistic continental landmasses
- Mountain ranges and valley systems
- Varied elevation changes
- Natural-looking coastlines
- Multiple biome types
- Asymmetric terrain (deeper oceans, softer mountains)

### Technical Details

#### Noise Generation
The algorithm uses **two independent simplex noise generators** with different seeds:

1. **Primary Noise Generator**: Creates large-scale terrain features
2. **Secondary Noise Generator**: Adds detail and variation

## Mathematical Breakdown

### 1. Primary Terrain Noise (Large Features)
```csharp
primaryTerrainNoise = primaryNoiseGenerator.Noise3DFBM(
    worldX * (xFactor + 0.01),
    worldY * (yFactor + 0.012), 
    worldZ * (zFactor + 0.01),
    6  // octaves
) * 3 * HeightMulti + (-0.2 + BaseHeight)
```

**Purpose**: Creates base landscape with mountains and valleys

**Key Elements**:
- **FBM with 6 octaves**: High detail, natural-looking features
- **Scale factors**: Control feature size (xFactor, yFactor, zFactor)
- **Amplitude**: 3 × HeightMulti (controls maximum height variation)
- **Offset**: -0.2 + BaseHeight (shifts baseline elevation)

### 2. Secondary Terrain Noise (Detail Layer)
```csharp
secondaryTerrainNoise = secondaryNoiseGenerator.Noise3DFBM(
    worldX * (1.0 / 400.0),
    worldY * (1.0 / 400.0), 
    worldZ * (1.0 / 400.0),
    3  // octaves
) * 3 * HeightMulti * (RandomFactor + 0.9) + (LandModifier + 0.5)
```

**Purpose**: Adds broader, smoother variations

**Key Elements**:
- **Larger scale** (1/400): Creates continental-sized features
- **3 octaves**: Less detail than primary (smoother)
- **RandomFactor**: Controls variation intensity
- **LandModifier**: Adjusts land/ocean ratio

### 3. Terrain Combination & Asymmetry
```csharp
// Dampen positive secondary noise
secondaryTerrainNoise = (secondaryTerrainNoise <= 0.0) ? 
    secondaryTerrainNoise : 
    secondaryTerrainNoise * 0.5;

// Combine layers
combinedTerrainNoise = primaryTerrainNoise + secondaryTerrainNoise;

// Apply asymmetric scaling
scaledTerrainValue = (combinedTerrainNoise <= 0.0) ? 
    combinedTerrainNoise * 1.6 :  // Deeper oceans
    combinedTerrainNoise * 0.5;   // Softer mountains
```

**Effect**:
- Oceans are **1.6× deeper** than mountains are tall
- Creates more realistic Earth-like topography
- Prevents extreme mountain peaks

### 4. Leveling Functions
```csharp
leveledTerrainHeight = (scaledTerrainValue <= 0.0) ? 
    Maths.Levelize2(scaledTerrainValue, 0.5) :  // Ocean floor smoothing
    Maths.Levelize3(scaledTerrainValue, 0.7);   // Mountain smoothing
```

**Purpose**: Redistributes height values for more natural appearance

- **Levelize2**: Smooths underwater terrain
- **Levelize3**: Creates plateaus and reduces extreme peaks

### 5. Biome Generation
```csharp
// Biome detail noise
biomeDetailNoise = secondaryNoiseGenerator.Noise3DFBM(
    worldX * (xFactor + 0.01) * 2.5,
    worldY * (yFactor + 0.012) * 8.0,  // Stretched vertically
    worldZ * (zFactor + 0.01) * 2.5,
    2  // octaves
) * 0.6 - 0.3;

// Combine with terrain for ecological zones
rawBiomeHeight = scaledTerrainValue * BiomeHeightMulti + 
                 biomeDetailNoise + 
                 BiomeHeightModifier * 2.5 + 0.3;

// Apply soft cap for high values
scaledBiomeHeight = (rawBiomeHeight >= 1.0) ? 
    (rawBiomeHeight - 1.0) * 0.8 + 1.0 :  // Soften extreme biomes
    rawBiomeHeight;
```

**Biome Features**:
- Vertically stretched (8× on Y-axis) for latitude-like zones
- Correlated with terrain height
- Soft-capped to prevent extreme values

## Parameters

### Theme Settings Used

| Parameter | Default | Effect |
|-----------|---------|--------|
| **xFactor** | 0.01 | Horizontal terrain scale (X-axis) |
| **yFactor** | 0.012 | Vertical terrain scale (Y-axis) |
| **zFactor** | 0.01 | Horizontal terrain scale (Z-axis) |
| **HeightMulti** | 3.0 | Overall terrain amplitude |
| **BaseHeight** | -0.2 | Baseline elevation offset |
| **RandomFactor** | 0.9 | Detail variation intensity |
| **LandModifier** | 0.5 | Land/ocean ratio adjustment |
| **BiomeHeightMulti** | 1.0 | Biome variation amplitude |
| **BiomeHeightModifier** | 0.0 | Biome baseline offset |

## Data Output

### Height Data
```csharp
heightDataValue = (radius + leveledTerrainHeight + 0.2) * 100.0
```
- Stored as `ushort` (0-65535)
- Clamped to prevent overflow
- Units: centimeters (×100)

### Biome Data
```csharp
biomeValue = scaledBiomeHeight * 100.0
```
- Stored as `byte` (0-200)
- Determines vegetation/climate type
- 0 = coldest/lowest, 200 = hottest/highest

## Use Cases

### Ideal For:
- ✅ Earth-like terrestrial planets
- ✅ Rocky worlds with continents and oceans
- ✅ Habitable zone planets
- ✅ Planets with varied biomes
- ✅ Realistic topography

### Not Suitable For:
- ❌ Gas giants
- ❌ Completely flat worlds
- ❌ Exotic/alien terrain patterns
- ❌ Extreme vertical terrain

## Performance

**Computational Cost**: ⭐⭐⭐ Moderate-High

- **2 simplex noise generators**
- **Multiple FBM calculations** per vertex
- **6 octaves** for primary noise (expensive)
- **3 octaves** for secondary noise
- Estimated: ~100ms for 200k vertices

## Visual Examples

### Typical Planet Appearance:
```
┌─────────────────────────────┐
│   🏔️  Mountains            │
│  🌲🌲 Forests               │
│ ~~~~ Coastlines             │
│🌊🌊🌊 Oceans                │
│   🏜️  Varied biomes        │
└─────────────────────────────┘
```

## Algorithm Flow

```
1. Initialize random generator with planet seed
2. Create two simplex noise generators (different seeds)
3. Get terrain settings from theme
4. For each vertex in planet mesh:
   ├─ Convert to world coordinates (×radius)
   ├─ Generate primary terrain noise (6 octaves FBM)
   ├─ Generate secondary terrain noise (3 octaves FBM)
   ├─ Dampen positive secondary noise
   ├─ Combine noise layers
   ├─ Apply asymmetric scaling (oceans deeper)
   ├─ Apply leveling function
   ├─ Generate biome detail noise (2 octaves FBM)
   ├─ Calculate biome value with soft cap
   ├─ Store height data (clamped ushort)
   └─ Store biome data (clamped byte 0-200)
5. Track min/max for diagnostics
6. Complete
```

## Comparison with Other Algorithms

| Feature | Terrain1 | Terrain3 | Terrain5 | Terrain6 |
|---------|----------|----------|----------|----------|
| Noise Layers | 2 | 2 | 2 | 2 |
| Max Octaves | 6 | 6 | 4 | 4 |
| Asymmetry | Yes | No | No | Yes |
| Land/Ocean | Natural | Extreme | Natural | Natural |
| Biome Zones | Smooth | Varied | Simple | Complex |
| Best For | Earth-like | Alien | Simple | Islands |

## Special Features

### 1. **Asymmetric Terrain**
Mountains rise gently (0.5× scaling) while oceans drop steeply (1.6× scaling), mimicking real planetary physics.

### 2. **Dual-Scale Noise**
Combines fine detail (primary) with broad features (secondary) for realistic appearance.

### 3. **Adaptive Leveling**
Different leveling functions for above/below sea level create natural-looking distributions.

### 4. **Latitude-Aware Biomes**
Vertical stretching (8× on Y-axis) creates climate bands similar to Earth's latitude zones.

## Configuration Example

```json
{
  "algorithm": "GenerateTerrain1",
  "settings": {
    "xFactor": 0.01,
    "yFactor": 0.012,
    "zFactor": 0.01,
    "HeightMulti": 3.0,
    "BaseHeight": -0.2,
    "RandomFactor": 0.9,
    "LandModifier": 0.5,
    "BiomeHeightMulti": 1.0,
    "BiomeHeightModifier": 0.0
  },
  "description": "Earth-like planet with continents and varied biomes"
}
```

## Notes

- The algorithm includes a **checkerboard pattern generator** (`GenerateTerrainA`) as an alternative/debugging tool
- Height/biome tracking enables diagnostics and validation
- Seed-based generation ensures consistency across game sessions
- FBM (Fractal Brownian Motion) creates natural-looking self-similar terrain at multiple scales


