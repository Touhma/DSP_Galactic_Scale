# Terrain Generation Algorithms - Complete Guide

## Overview

The GalacticScale3 mod includes **5 distinct terrain generation algorithms**, each designed for different planetary types and visual aesthetics. This document provides a comprehensive comparison and selection guide.

## Quick Algorithm Selection Guide

### Choose Your Algorithm Based On:

| Your Goal | Recommended Algorithm |
|-----------|----------------------|
| Gas giant / featureless planet | **Terrain00** |
| Earth-like planet with continents | **Terrain1** or **Terrain5** |
| Dramatic alien landscapes | **Terrain3** |
| Island archipelagos | **Terrain6** |
| Maximum customization | **Terrain6** |
| Vanilla DSP-style terrain | **Terrain5** |
| Performance critical | **Terrain00** |

## Algorithm Comparison Table

| Algorithm | Type | Octaves | CPU Cost | Realism | Customization | Land/Ocean |
|-----------|------|---------|----------|---------|---------------|------------|
| **Terrain00** | Geometric | 0 | ⭐ Minimal | N/A | None | N/A |
| **Terrain1** | Dual-Layer FBM | 9 | ⭐⭐⭐ Moderate | Very High | Medium | Soft |
| **Terrain3** | Warped Multi-Layer | 13 | ⭐⭐⭐⭐ Highest | Low | Low | Mixed |
| **Terrain5** | Threshold-Based | 8 | ⭐⭐ Moderate | High | Medium | Sharp |
| **Terrain6** | Advanced Threshold | 8 | ⭐⭐⭐ Moderate | High | Highest | Sharp |

## Detailed Algorithm Summaries

### Terrain00 - Flat Sphere
- **File**: `GenerateTerrain00.cs`
- **Documentation**: [GenerateTerrain00.md](GenerateTerrain00.md)
- **Best For**: Gas giants, featureless planets, testing
- **Key Feature**: No noise generation, perfectly smooth sphere
- **Performance**: Fastest possible (1-2ms)
- **Complexity**: Minimal

**When to Use**: Gas giants, placeholder planets, or when terrain detail isn't needed.

---

### Terrain1 - Dual-Layer Fractal Terrain
- **File**: `GenerateTerrain1.cs`
- **Documentation**: [GenerateTerrain1.md](GenerateTerrain1.md)
- **Best For**: Earth-like terrestrial planets with natural continents
- **Key Features**:
  - 6-octave primary noise (high detail)
  - 3-octave secondary noise (continental features)
  - Asymmetric terrain (deeper oceans, softer mountains)
  - Latitude-aware biomes
- **Performance**: ~100ms for 200k vertices
- **Complexity**: High

**When to Use**: Default choice for rocky, Earth-like planets with varied terrain and multiple biomes.

**Visual Style**: Natural mountains, valleys, coastlines, and varied ecosystems.

---

### Terrain3 - Warped Multi-Layer
- **File**: `GenerateTerrain3.cs`
- **Documentation**: [GenerateTerrain3.md](GenerateTerrain3.md)
- **Best For**: Alien/exotic worlds with extreme features
- **Key Features**:
  - Sinusoidal domain warping (unique!)
  - 13 total octaves (most detailed)
  - Custom deltaWLen (1.8) for persistent detail
  - Double-leveling for plateaus
  - Zone-based height mapping
- **Performance**: ~150ms for 200k vertices (slowest)
- **Complexity**: Very High

**When to Use**: Memorable showcase planets, alien worlds, or when you want dramatic, unusual terrain.

**Visual Style**: Swirling patterns, extreme height variations, sharp ridges, alien landscapes.

---

### Terrain5 - Continent/Ocean
- **File**: `GenerateTerrain5.cs`
- **Documentation**: [GenerateTerrain5.md](GenerateTerrain5.md)
- **Best For**: Vanilla DSP-style planets with clear land/ocean boundaries
- **Key Features**:
  - Threshold-based land masking (0.16 threshold)
  - Cubic ocean floor smoothing
  - Clear continent formation
  - Coordinate levelizing
  - 8 total octaves
- **Performance**: ~60ms for 200k vertices (fast)
- **Complexity**: Moderate

**When to Use**: When you want clear continents and oceans, similar to vanilla DSP terrain.

**Visual Style**: Distinct landmasses, smooth ocean floors, realistic coastlines, moderate terrain variation.

---

### Terrain6 - Island-Based (Most Customizable)
- **File**: `GenerateTerrain6.cs`
- **Documentation**: [GenerateTerrain6.md](GenerateTerrain6.md)
- **Best For**: Archipelago planets, highly customized worlds
- **Key Features**:
  - Full theme parameter integration (most customizable!)
  - Threshold-based land masking with LandModifier
  - Biome gap jumping (prevents bunching)
  - Cubic ocean smoothing
  - Island chain formation
  - 8 total octaves
- **Performance**: ~80ms for 200k vertices
- **Complexity**: Moderate-High

**When to Use**: When you need fine control over terrain characteristics, or want island-based geography.

**Visual Style**: Island chains, archipelagos, customizable land/ocean ratios, varied biomes.

---

## Feature Comparison Matrix

### Noise Generation

| Algorithm | Generators | Primary Octaves | Secondary Octaves | Total Octaves | Special Processing |
|-----------|-----------|-----------------|-------------------|---------------|-------------------|
| Terrain00 | 0 | - | - | 0 | None |
| Terrain1 | 2 | 6 | 3 | 9 | Asymmetric scaling |
| Terrain3 | 2 | 6 | 3+2+2 | 13 | Domain warping |
| Terrain5 | 2 | 4 | 2+2 | 8 | Threshold masking |
| Terrain6 | 2 | 4 | 2+2 | 8 | Threshold + gap jumping |

### Theme Parameter Support

| Parameter | T00 | T1 | T3 | T5 | T6 |
|-----------|-----|----|----|----|----|
| xFactor | ❌ | ✅ | ❌ | ✅ | ✅ |
| yFactor | ❌ | ✅ | ❌ | ✅ | ✅ |
| zFactor | ❌ | ✅ | ❌ | ✅ | ✅ |
| HeightMulti | ❌ | ✅ | ✅ | ✅ | ✅ |
| BaseHeight | ❌ | ✅ | ✅ | ✅ | ✅ |
| LandModifier | ❌ | ✅ | ❌ | ✅ | ✅ |
| RandomFactor | ❌ | ✅ | ❌ | ✅ | ✅ |
| BiomeHeightMulti | ❌ | ✅ | ✅ | ✅ | ✅ |
| BiomeHeightModifier | ❌ | ✅ | ✅ | ✅ | ✅ |

**Winner**: Terrain6 (full integration)

### Terrain Characteristics

| Feature | T00 | T1 | T3 | T5 | T6 |
|---------|-----|----|----|----|----|
| Mountains | ❌ | ✅✅✅ | ✅✅✅✅ | ✅✅ | ✅✅ |
| Valleys | ❌ | ✅✅✅ | ✅✅✅✅ | ✅✅ | ✅✅ |
| Continents | ❌ | ✅✅ | ✅ | ✅✅✅ | ✅✅ |
| Islands | ❌ | ✅ | ❌ | ✅✅ | ✅✅✅✅ |
| Sharp Coastlines | ❌ | ✅ | ✅ | ✅✅✅ | ✅✅✅ |
| Smooth Oceans | ✅ | ✅ | ❌ | ✅✅✅ | ✅✅✅ |
| Biome Variety | ❌ | ✅✅✅ | ✅✅✅✅ | ✅✅ | ✅✅✅ |

### Performance Metrics (200k vertices)

| Algorithm | Estimated Time | Noise Calls | Math Operations | Memory Usage |
|-----------|---------------|-------------|-----------------|--------------|
| Terrain00 | ~1ms | 0 | Minimal | Minimal |
| Terrain1 | ~100ms | ~1.8M | High | Moderate |
| Terrain3 | ~150ms | ~2.6M | Very High | Moderate |
| Terrain5 | ~60ms | ~1.6M | Moderate | Moderate |
| Terrain6 | ~80ms | ~1.6M | Moderate-High | Moderate |

## Visual Comparison

### Cross-Section Views

```
Terrain00: ═══════════════════════ (Flat)

Terrain1:      ▲
              /│\
             / │ \
~~~~~~~~~~~~~  │  ~~~~~~~~~~~~~~
              \│/
               ▼

Terrain3:   ⚡️  ▲⚡️
           /⚡️/|\⚡️\
          /  ⚡️│⚡️  \
~~~~~~~~~    ⚡️│⚡️    ~~~~~~~~~
         \   │   /
          \  │  /

Terrain5:    ▲   ▲
           /│\ /│\
~~~~~~~~~~  │   │  ~~~~~~~~~~~
         \  │   │  /
          \_│___│_/

Terrain6:  ▲     ▲     ▲
          /│\   /│\   /│\
~~~~~~~~~  │ ~~~ │ ~~~ │ ~~~~~
        \  │     │     │  /
         \_│_____│_____│_/
```

## Technical Details

### Data Structures

All algorithms output two arrays:
1. **Height Data**: `ushort[]` (0-65535)
   - Units: centimeters
   - Formula: `(radius + terrainHeight) × 100`
   - Clamped to prevent overflow

2. **Biome Data**: `byte[]` (0-200)
   - Determines vegetation/climate type
   - 0 = coldest/lowest
   - 200 = hottest/highest

### Noise Functions Used

- **SimplexNoise**: Primary noise generator
- **Noise3DFBM**: Fractal Brownian Motion (layered noise)
  - Parameters: x, y, z, octaves, persistence, lacunarity, deltaWLen
- **Maths.Levelize**: Height distribution functions (2, 3, 4 variants)

### Common Processing Steps

1. **Initialize RNG** with planet seed
2. **Create noise generators** (typically 2)
3. **Get theme settings**
4. **For each vertex**:
   - Convert to world coordinates
   - Apply preprocessing (warping, levelizing, etc.)
   - Generate noise layers
   - Combine layers
   - Apply leveling/smoothing
   - Calculate final height and biome
   - Store clamped values

## Usage Examples

### Setting Algorithm in Code

```csharp
// Set algorithm for a specific planet theme
myTheme.TerrainSettings.Algorithm = "GenerateTerrain1";

// Or use the algorithm directly
TerrainAlgorithms.GenerateTerrain1(gsPlanet, modX: 0, modY: 0);
```

### Customizing Theme Settings for Terrain6

```csharp
var settings = myTheme.TerrainSettings;
settings.xFactor = 0.01;
settings.yFactor = 0.012;
settings.zFactor = 0.01;
settings.HeightMulti = 3.0;
settings.BaseHeight = -0.5;
settings.LandModifier = 0.2;      // More land
settings.RandomFactor = 1.5;       // High detail
settings.BiomeHeightMulti = 1.2;   // Varied biomes
settings.BiomeHeightModifier = 0.1; // Slightly warmer
```

## Best Practices

### For Performance
1. Use **Terrain00** for gas giants
2. Avoid **Terrain3** on low-end systems
3. Consider **Terrain5** for balanced performance

### For Realism
1. **Terrain1** for Earth-like worlds
2. **Terrain5** for vanilla-style planets
3. **Terrain6** for customized realism

### For Variety
1. Mix different algorithms across solar systems
2. **Terrain3** for memorable alien worlds
3. **Terrain6** with varied theme settings

### For Customization
1. **Terrain6** provides maximum control
2. Adjust **LandModifier** to control land/ocean ratio
3. Use **HeightMulti** and **BaseHeight** together for dramatic effects

## Algorithm Selection Flowchart

```
Start
  │
  ├─ Need terrain variation?
  │   NO → Terrain00 (gas giant)
  │   YES ↓
  │
  ├─ Want islands/archipelagos?
  │   YES → Terrain6 (most customizable)
  │   NO ↓
  │
  ├─ Need extreme/alien terrain?
  │   YES → Terrain3 (dramatic)
  │   NO ↓
  │
  ├─ Want vanilla DSP-style?
  │   YES → Terrain5 (continent/ocean)
  │   NO ↓
  │
  └─ Default: Terrain1 (Earth-like)
```

## Performance Optimization Tips

1. **Reduce vertex count** for lower-importance planets
2. **Cache terrain data** - don't regenerate unnecessarily
3. **Use Terrain00** for distant/unvisited planets
4. **Thread terrain generation** (already implemented in Modeler.Compute.cs)
5. **Clean up unused PlanetRawData** after generation

## Common Issues & Solutions

### Issue: Terrain too flat
- **Solution**: Increase `HeightMulti`
- **Algorithms affected**: All except Terrain00

### Issue: Too much ocean
- **Solution**: Increase `LandModifier` (T5, T6) or adjust `LandModifier` (T1)
- **Algorithms affected**: Terrain1, Terrain5, Terrain6

### Issue: Terrain too rough/noisy
- **Solution**: Decrease `RandomFactor`
- **Algorithms affected**: Terrain1, Terrain5, Terrain6

### Issue: Performance problems
- **Solution**: Use simpler algorithm (T5 instead of T3) or reduce vertex count
- **Algorithms affected**: Terrain3 especially

## Future Development

### Potential Enhancements
- Add erosion simulation
- Implement tectonic plate simulation
- Add crater generation for airless worlds
- Create hybrid algorithms
- Implement terrain caching system

### Modding Support
All algorithms support:
- Custom theme settings
- Seed-based generation (deterministic)
- Real-time parameter adjustment
- Integration with vegetation/vein generation

## References

### Related Files
- **Modeler.Compute.cs**: Threading and generation management
- **PlanetRawData.cs**: Data structure definitions  
- **SimplexNoise.cs**: Noise generation functions
- **Maths.cs**: Levelize and other math utilities
- **TerrainSettings.cs**: Theme parameter definitions

### External Resources
- [Simplex Noise](https://en.wikipedia.org/wiki/Simplex_noise)
- [Fractal Brownian Motion](https://thebookofshaders.com/13/)
- [Domain Warping](https://www.iquilezles.org/www/articles/warp/warp.htm)

---

## Quick Reference Card

| Algorithm | One-Line Description | Best Use Case |
|-----------|---------------------|---------------|
| **Terrain00** | Smooth sphere | Gas giants |
| **Terrain1** | Natural fractal terrain | Earth-like worlds |
| **Terrain3** | Warped alien landscapes | Exotic showcase planets |
| **Terrain5** | Clear continents/oceans | Vanilla DSP style |
| **Terrain6** | Island archipelagos | Maximum customization |

---

*Last Updated: 2025-10-14*  
*GalacticScale3 Mod - Terrain Generation Documentation*

