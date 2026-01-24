using UnityEngine;
using System;

namespace GalacticScale
{
    public static partial class TerrainAlgorithms
    {
        /// <summary>
        /// Generates terrain for a planet using simplex noise algorithms.
        /// This method creates realistic landscapes with mountains, valleys, and varied biomes.
        /// </summary>
        /// <param name="planet">The planet to generate terrain for</param>
        /// <param name="modX">Optional X-axis modifier for terrain generation</param>
        /// <param name="modY">Optional Y-axis modifier for terrain generation</param>
        public static void GenerateTerrain1(GSPlanet planet, double modX = 0.0, double modY = 0.0)
        {
            // Validate planet object exists
            if (planet == null)
            {
                GS2.Warn("Planet object is null, cannot generate terrain");
                return;
            }

            // Initialize random generator with planet's seed for consistent terrain generation
            random = new GS2.Random(planet.Seed);
            
            // Get terrain settings from the planet's theme
            var terrainSettings = planet.GsTheme.TerrainSettings;
            
            // Generate random seeds for the two simplex noise generators
            var primaryNoiseSeed = random.Next();
            var secondaryNoiseSeed = random.Next();
            
            // Create simplex noise generators with different seeds for varied terrain patterns
            var primaryNoiseGenerator = new SimplexNoise(primaryNoiseSeed);
            var secondaryNoiseGenerator = new SimplexNoise(secondaryNoiseSeed);
            
            // Validate planet data exists
            if (planet.planetData == null)
            {
                return;
            }

            var planetVertexData = planet.planetData.data;
            
            // Track min/max values for height and biome data for debugging purposes
            var maxHeightValue = -999;
            var minHeightValue = 999999;
            var maxBiomeValue = -999;
            var minBiomeValue = 999999;
            
            // Ensure planet vertex data exists
            if (planetVertexData == null) return;
            
            // Process each vertex in the planet mesh
            for (var vertexIndex = 0; vertexIndex < planetVertexData.dataLength; ++vertexIndex)
            {
                // Scale vertex coordinates by planet radius to get world-space coordinates
                var worldX = planetVertexData.vertices[vertexIndex].x * (double)planet.planetData.radius;
                var worldY = planetVertexData.vertices[vertexIndex].y * (double)planet.planetData.radius;
                var worldZ = planetVertexData.vertices[vertexIndex].z * (double)planet.planetData.radius;
                
                // Generate primary terrain shape using fractal Brownian motion (FBM) noise
                // This creates the base landscape with large features (mountains, valleys)
                var primaryTerrainNoise = primaryNoiseGenerator.Noise3DFBM(
                    worldX * (terrainSettings.xFactor + 0.01), 
                    worldY * (0.012 + terrainSettings.yFactor), 
                    worldZ * (0.01 + terrainSettings.zFactor), 
                    6  // Octaves - higher values mean more detail
                ) * 3 * terrainSettings.HeightMulti + (-0.2 + terrainSettings.BaseHeight);
                
                // Generate secondary terrain details using different scale and settings
                // This adds smaller features and variations to the landscape
                var secondaryTerrainNoise = secondaryNoiseGenerator.Noise3DFBM(
                    worldX * (1.0 / 400.0),  // Larger scale for broader features
                    worldY * (1.0 / 400.0), 
                    worldZ * (1.0 / 400.0), 
                    3  // Fewer octaves for smoother secondary features
                ) * 3 * terrainSettings.HeightMulti * (terrainSettings.RandomFactor + 0.9) + (terrainSettings.LandModifier + 0.5);
                
                // Dampen positive values of secondary noise to prevent excessive peaks
                secondaryTerrainNoise = secondaryTerrainNoise <= 0.0 ? secondaryTerrainNoise : secondaryTerrainNoise * 0.5;
                
                // Combine primary and secondary noise for complex terrain
                var combinedTerrainNoise = primaryTerrainNoise + secondaryTerrainNoise;
                
                // Apply different scaling to positive vs negative values
                // This creates asymmetric terrain with deeper oceans and softer mountains
                var scaledTerrainValue = combinedTerrainNoise <= 0.0 ? combinedTerrainNoise * 1.6 : combinedTerrainNoise * 0.5;
                
                // Apply different leveling functions based on height
                // Levelize functions modify the distribution of heights to create more natural-looking terrain
                var leveledTerrainHeight = scaledTerrainValue <= 0.0 ? 
                    Maths.Levelize2(scaledTerrainValue, 0.5) : 
                    Maths.Levelize3(scaledTerrainValue, 0.7);
                
                // Generate biome-specific noise for varied ecological zones
                var biomeDetailNoise = secondaryNoiseGenerator.Noise3DFBM(
                    worldX * (terrainSettings.xFactor + 0.01) * 2.5, 
                    worldY * (0.012 + terrainSettings.yFactor) * 8.0, 
                    worldZ * (0.01 + terrainSettings.zFactor) * 2.5, 
                    2  // Lower octaves for broader biome transitions
                ) * 0.6 - 0.3;
                
                // Calculate raw biome height value combining terrain and biome factors
                var rawBiomeHeight = scaledTerrainValue * terrainSettings.BiomeHeightMulti + 
                                  biomeDetailNoise + 
                                  terrainSettings.BiomeHeightModifier * 2.5 + 0.3;
                
                // Scale biome height with a soft cap for high values to prevent extreme biomes
                var scaledBiomeHeight = rawBiomeHeight >= 1.0 ? 
                    (rawBiomeHeight - 1.0) * 0.8 + 1.0 : 
                    rawBiomeHeight;
                
                // Calculate final height data value in integer format for mesh vertices
                // Important: Use PRECISION not radius, because mesh will be scaled by planet.scale
                // For clamped planets: precision=1250, scale=1.12 → visual radius = 1400
                var heightDataValue = (int)((planet.planetData.precision + leveledTerrainHeight + 0.2) * 100.0);
                
                // Ensure height data array exists
                if (planetVertexData.heightData == null) return;
                
                // Use full precision storage for large planets
                planetVertexData.SetHeightData(vertexIndex, heightDataValue);
                
                // Calculate biome data value and clamp to byte range
                var biomeValue = (byte)Mathf.Clamp((float)(scaledBiomeHeight * 100.0), 0.0f, 200f);
                planetVertexData.biomoData[vertexIndex] = biomeValue;
                
                // Track min/max values for diagnostics
                if (heightDataValue > maxHeightValue) maxHeightValue = heightDataValue;
                if (heightDataValue < minHeightValue) minHeightValue = heightDataValue;
                if (biomeValue > maxBiomeValue) maxBiomeValue = biomeValue;
                if (biomeValue < minBiomeValue) minBiomeValue = biomeValue;
            }
            
            // Debugging can be uncommented if needed
            // GS2.Log($"Planet: {planet.Name} - Radius: {planet.Radius} - Height Range: {minHeightValue} to {maxHeightValue} - Biome Range: {minBiomeValue} to {maxBiomeValue}");
        }

        /// <summary>
        /// Generates a checkerboard pattern terrain on the planet.
        /// Creates alternating high and low regions in a grid-like pattern across the planet surface.
        /// </summary>
        /// <param name="planet">The planet to generate terrain for</param>
        /// <param name="gridSize">Size of the checkerboard squares (lower values = more squares)</param>
        /// <param name="heightDifference">Height difference between high and low regions</param>
        public static void GenerateTerrainA(GSPlanet planet, double gridSize = 0.5, double heightDifference = 10)
        {
            // Validate planet object exists
            if (planet == null)
            {
                GS2.Warn("Planet object is null, cannot generate checkerboard terrain");
                return;
            }

            // Initialize random generator with planet's seed for consistent terrain generation
            random = new GS2.Random(planet.Seed);
            
            // Get terrain settings from the planet's theme
            var terrainSettings = planet.GsTheme.TerrainSettings;
            
            // Validate planet data exists
            if (planet.planetData == null)
            {
                return;
            }

            var planetVertexData = planet.planetData.data;
            
            // Track min/max values for height and biome data for debugging purposes
            var maxHeightValue = -999;
            var minHeightValue = 999999;
            var maxBiomeValue = -999;
            var minBiomeValue = 999999;
            
            // Ensure planet vertex data exists
            if (planetVertexData == null) return;
            
            // Create simplex noise for subtle variations and biome data
            var noiseGenerator = new SimplexNoise(random.Next());
            
            // Process each vertex in the planet mesh
            for (var vertexIndex = 0; vertexIndex < planetVertexData.dataLength; ++vertexIndex)
            {
                // Get normalized vertex coordinates (direction from center)
                var nx = planetVertexData.vertices[vertexIndex].x;
                var ny = planetVertexData.vertices[vertexIndex].y;
                var nz = planetVertexData.vertices[vertexIndex].z;
                
                // Convert to spherical coordinates to determine position on planetary surface
                // Calculate longitude and latitude from normalized coordinates
                double longitude = Math.Atan2(nz, nx);  
                double latitude = Math.Asin(ny);
                
                // Scale to create grid pattern
                // Divide by gridSize to control the number of squares
                double scaledLon = longitude / gridSize;
                double scaledLat = latitude / gridSize;
                
                // Determine if this point is in a "high" or "low" square
                // By checking if the integer parts of scaled coordinates sum to an even number
                bool isHighSquare = (int)Math.Floor(scaledLon) % 2 == (int)Math.Floor(scaledLat) % 2;
                
                // Calculate base terrain height based on checkerboard pattern
                double terrainHeight = isHighSquare ? heightDifference : 0.0;
                
                // Add slight noise to avoid completely flat surfaces
                double noiseVariation = noiseGenerator.Noise3DFBM(
                    nx * 5.0, 
                    ny * 5.0, 
                    nz * 5.0, 
                    2
                ) * 0.03;  // Small variation (3% of base height)
                
                // Apply base height from terrain settings
                terrainHeight += terrainSettings.BaseHeight + noiseVariation;
                
                // Calculate biome value based on a different noise pattern
                // This creates more natural biome transitions
                var biomeNoise = noiseGenerator.Noise3DFBM(
                    nx * 3.0,
                    ny * 3.0, 
                    nz * 3.0,
                    3
                );
                
                // Scale biome value and add base value
                var scaledBiomeHeight = biomeNoise * 0.5 + 0.5;  // Range 0.0-1.0
                
                // Calculate final height data value for mesh vertices
                // Important: Use PRECISION not radius, because mesh will be scaled by planet.scale
                var heightDataValue = (int)((planet.planetData.precision + terrainHeight) * 100.0);
                
                // Ensure height data array exists
                if (planetVertexData.heightData == null) return;
                
                // Use full precision storage for large planets
                planetVertexData.SetHeightData(vertexIndex, heightDataValue);
                
                // Calculate biome data value and clamp to byte range (0-200)
                var biomeValue = (byte)Mathf.Clamp((float)(scaledBiomeHeight * 100.0), 0.0f, 200f);
                planetVertexData.biomoData[vertexIndex] = biomeValue;
                
                // Track min/max values for diagnostics
                if (heightDataValue > maxHeightValue) maxHeightValue = heightDataValue;
                if (heightDataValue < minHeightValue) minHeightValue = heightDataValue;
                if (biomeValue > maxBiomeValue) maxBiomeValue = biomeValue;
                if (biomeValue < minBiomeValue) minBiomeValue = biomeValue;
            }
            
            GS2.Log($"Checkerboard terrain generated for planet: {planet.Name} - Grid size: {gridSize} - Height difference: {heightDifference}");
            // Uncomment for detailed diagnostics
            // GS2.Log($"Height Range: {minHeightValue} to {maxHeightValue} - Biome Range: {minBiomeValue} to {maxBiomeValue}");
        }
    }
}