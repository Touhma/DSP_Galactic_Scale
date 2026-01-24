using System;
using System.Collections.Generic;

namespace GalacticScale
{
    public static class PlanetRawDataExtension
    {
        private static readonly Dictionary<PlanetRawData, float> FactoredRadius = new();
        private static readonly Dictionary<PlanetRawData, int[]> FullPrecisionHeightData = new();
        private static readonly Dictionary<PlanetRawData, float> PlanetRadius = new();
        private static readonly Dictionary<PlanetRawData, float> PlanetScale = new();
        
        // Backup storage using planet ID as key (survives PlanetRawData recreation)
        private static readonly Dictionary<int, int[]> FullPrecisionHeightDataByPlanetId = new();
        private static readonly Dictionary<int, float> PlanetRadiusByPlanetId = new();
        
        // Map PlanetRawData objects to their planet IDs for fallback lookup
        private static readonly Dictionary<PlanetRawData, int> PlanetRawDataToPlanetId = new();

        public static void AddFactoredRadius(this PlanetRawData planetRawData, PlanetData planet)
        {
            //GS2.Log("PlanetRawDataExtension|AddFactoredRadius|" + planet.name + " planetRawData:" + ((planetRawData != null)?"PlanetRawData Exists":"PlanetRawData Null"));
            if (planet == null)
            {
                GS2.Warn("planet Null");
                return;
            }

            if (planetRawData == null)
            {
                if (!UIRoot.instance.backToMainMenu) GS2.Warn($"RawData Null for planet {planet.name} of radius {planet.radius}");
                return;
            }

            var scaleFactored = planet.GetScaleFactored();
            //GS2.Log($"Trying to add to dict:{scaleFactored}");
            try
            {
                FactoredRadius[planetRawData] = scaleFactored;
                PlanetRadius[planetRawData] = planet.radius;
                PlanetScale[planetRawData] = planet.scale;
                
                // Store by planet ID as backup (survives PlanetRawData recreation)
                PlanetRadiusByPlanetId[planet.id] = planet.radius;
                PlanetRawDataToPlanetId[planetRawData] = planet.id;
                
                // For large planets, allocate full precision height storage
                if (planet.radius > 200)
                {
                    // Validate scale is 1.0 for terran worlds
                    if (planet.scale != 1.0f && planet.type != EPlanetType.Gas)
                    {
                        GS2.Warn($"WARNING: Planet {planet.name} has scale={planet.scale:F2} but should be 1.0 for terran worlds!");
                    }
                    
                    // Store in both dictionaries for redundancy
                    if (!FullPrecisionHeightData.ContainsKey(planetRawData))
                    {
                        FullPrecisionHeightData[planetRawData] = new int[planetRawData.dataLength];
                    }
                    if (!FullPrecisionHeightDataByPlanetId.ContainsKey(planet.id))
                    {
                        FullPrecisionHeightDataByPlanetId[planet.id] = new int[planetRawData.dataLength];
                    }
                    GS2.Log($"Allocated full precision heightData for planet {planet.name} (radius {planet.radius}, scale {planet.scale:F2})");
                }
            }
            catch (Exception e)
            {
                GS2.Error(e.Message);
            }
        }

        public static float GetFactoredScale(this PlanetRawData planetRawData)
        {
            //GS2.Warn($"Trying to get factored scale. {FactoredRadius.TryGetValue(planetRawData, out var result)}");
            return FactoredRadius.TryGetValue(planetRawData, out var result) ? result : 1f;
        }
        
        public static float GetPlanetRadius(this PlanetRawData planetRawData)
        {
            return PlanetRadius.TryGetValue(planetRawData, out var result) ? result : 200f;
        }
        
        public static bool UsesFullPrecisionHeight(this PlanetRawData planetRawData)
        {
            return FullPrecisionHeightData.ContainsKey(planetRawData);
        }
        
        public static void SetHeightData(this PlanetRawData planetRawData, int index, int value)
        {
            // Try object-based dictionary first
            if (FullPrecisionHeightData.TryGetValue(planetRawData, out var fullData))
            {
                fullData[index] = value;
                // Also store clamped version in ushort array for compatibility
                planetRawData.heightData[index] = (ushort)UnityEngine.Mathf.Clamp(value, 0, 65535);
                
                // DUAL-WRITE: Also store in ID-based dictionary for redundancy
                if (PlanetRawDataToPlanetId.TryGetValue(planetRawData, out int planetId) && 
                    FullPrecisionHeightDataByPlanetId.TryGetValue(planetId, out var backupData))
                {
                    backupData[index] = value;
                }
                
                // Debug logging for first few writes
                if (index < 5)
                {
                    GS2.Log($"SetHeightData[{index}]: value={value}, stored={fullData[index]}, clamped={planetRawData.heightData[index]}, dictHash={planetRawData.GetHashCode()}");
                }
            }
            else
            {
                // Fallback: try to find planet ID using the mapping
                if (PlanetRawDataToPlanetId.TryGetValue(planetRawData, out int planetId) && 
                    FullPrecisionHeightDataByPlanetId.TryGetValue(planetId, out var backupData))
                {
                    backupData[index] = value;
                    planetRawData.heightData[index] = (ushort)UnityEngine.Mathf.Clamp(value, 0, 65535);
                    
                    if (index < 5 && !DSPGame.IsMenuDemo)
                    {
                        GS2.Log($"SetHeightData[{index}]: value={value}, stored in backup dict, planetId={planetId}, clamped={planetRawData.heightData[index]}");
                    }
                }
                else
                {
                    planetRawData.heightData[index] = (ushort)UnityEngine.Mathf.Clamp(value, 0, 65535);
                    if (index < 5 && !DSPGame.IsMenuDemo)
                    {
                        GS2.Log($"SetHeightData[{index}]: value={value}, no dict, clamped={planetRawData.heightData[index]}, rawDataHash={planetRawData.GetHashCode()}");
                    }
                }
            }
        }
        
        public static int GetHeightDataInt(this PlanetRawData planetRawData, int index)
        {
            // Try object-based dictionary first
            if (FullPrecisionHeightData.TryGetValue(planetRawData, out var fullData))
            {
                int dictValue = fullData[index];
                int vanillaValue = planetRawData.heightData[index];
                if (index < 5 && !DSPGame.IsMenuDemo)
                {
                    GS2.Log($"GetHeightDataInt[{index}]: dict={dictValue}, vanilla={vanillaValue}, using=dict, rawDataHash={planetRawData.GetHashCode()}");
                }
                return dictValue;
            }
            
            // Fallback: try to find planet ID using the mapping
            if (PlanetRawDataToPlanetId.TryGetValue(planetRawData, out int planetId) && 
                FullPrecisionHeightDataByPlanetId.TryGetValue(planetId, out var backupData))
            {
                int backupValue = backupData[index];
                int vanillaValue = planetRawData.heightData[index];
                if (index < 5 && !DSPGame.IsMenuDemo)
                {
                    GS2.Log($"GetHeightDataInt[{index}]: backup={backupValue}, vanilla={vanillaValue}, using=backup, planetId={planetId}");
                }
                return backupValue;
            }
            
            // Fall back to vanilla behavior for main menu / uninitialized planets
            if (planetRawData?.heightData != null && index >= 0 && index < planetRawData.heightData.Length)
            {
                if (index < 5 && !DSPGame.IsMenuDemo)
                {
                    GS2.Log($"GetHeightDataInt[{index}]: no dict, vanilla={planetRawData.heightData[index]}, using=vanilla, rawDataHash={planetRawData.GetHashCode()}");
                }
                return planetRawData.heightData[index];
            }
            
            return 0; // Safe fallback if data is completely missing
        }
        
        public static float GetHeightDataFloatSafe(this PlanetRawData planetRawData, int index)
        {
            // Log first few calls to verify this method is actually being called
            if (index < 3)
            {
                GS2.Log($"GetHeightDataFloatSafe[{index}] called: isMenuDemo={DSPGame.IsMenuDemo}, planet={planetRawData?.GetHashCode()}");
            }
            
            // Runtime guard: only use custom logic for large planets in actual gameplay
            if (DSPGame.IsMenuDemo)
            {
                // In main menu, use vanilla behavior
                if (planetRawData?.heightData != null && index >= 0 && index < planetRawData.heightData.Length)
                {
                    float result = planetRawData.heightData[index] * 0.01f;
                    if (index < 3)
                    {
                        GS2.Log($"GetHeightDataFloatSafe[{index}] menu: heightData[{index}]={planetRawData.heightData[index]}, result={result:F2}");
                    }
                    return result;
                }
                else
                {
                    // If heightData is null or index out of bounds, return a small random height
                    // This prevents degenerate meshes while maintaining some variation
                    float fallbackHeight = (float)((index * 0.1f) % 1.0f) * 0.01f; // Small variation based on index
                    if (index < 3)
                    {
                        GS2.Log($"GetHeightDataFloatSafe[{index}] menu: using fallback height={fallbackHeight:F4}");
                    }
                    return fallbackHeight;
                }
            }
            
            // Check if we have custom height data for this planet
            bool hasCustomData = FullPrecisionHeightData.ContainsKey(planetRawData);
            
            // If we don't have custom data, use vanilla behavior (terrain not generated yet)
            if (!hasCustomData)
            {
                if (planetRawData?.heightData != null && index >= 0 && index < planetRawData.heightData.Length)
                {
                    return planetRawData.heightData[index] * 0.01f;
                }
                return 0f;
            }
            
            // In actual gameplay with custom data, use the full logic
            return planetRawData.GetHeightDataFloat(index);
        }
        
        public static float GetHeightDataFloat(this PlanetRawData planetRawData, int index)
        {
            // Safety check for null or invalid data
            if (planetRawData == null || index < 0)
            {
                return 0f;
            }
            
            // Auto-allocate dictionary if missing (transpiler might call this before prefix)
            if (!FullPrecisionHeightData.ContainsKey(planetRawData))
            {
                // Try to find the planet data to get radius - check all planets, not just local
                PlanetData foundPlanet = null;
                if (GameMain.data != null && GameMain.data.galaxy != null && !DSPGame.IsMenuDemo)
                {
                    foreach (var star in GameMain.data.galaxy.stars)
                    {
                        if (star != null && star.planets != null)
                        {
                            foreach (var planet in star.planets)
                            {
                                if (planet != null && planet.data == planetRawData)
                                {
                                    foundPlanet = planet;
                                    break;
                                }
                            }
                        }
                        if (foundPlanet != null) break;
                    }
                }
                
                if (foundPlanet != null && foundPlanet.radius > 200)
                {
                    planetRawData.AddFactoredRadius(foundPlanet);
                    GS2.Log($"GetHeightDataFloat: Auto-allocated dictionary for planet {foundPlanet.name} (radius {foundPlanet.radius})");
                }
                else
                {
                    // For small planets or when planet not found, use vanilla behavior
                    // Don't log warnings for every call - this is expected behavior
                    if (planetRawData.heightData != null && index < planetRawData.heightData.Length)
                    {
                        return planetRawData.heightData[index] * 0.01f;
                    }
                    else
                    {
                        // Fallback if heightData is null or index out of bounds
                        return 0f;
                    }
                }
            }
            
            // Log first 3 calls to verify this method is actually being called
            bool usesDict = FullPrecisionHeightData.ContainsKey(planetRawData);
            if (index < 3 && !DSPGame.IsMenuDemo)
            {
                GS2.Log($"GetHeightDataFloat[{index}]: usesDict={usesDict}, dictHash={planetRawData.GetHashCode()}");
            }
            
            int heightInt = planetRawData.GetHeightDataInt(index);
            float result = heightInt * 0.01f;
            
            if (index < 3 && !DSPGame.IsMenuDemo)
            {
                GS2.Log($"GetHeightDataFloat[{index}] result: heightInt={heightInt}, result={result:F2}");
            }
            
            return result;
        }

        public static int GetModPlaneInt(this PlanetRawData planetRawData, int index)
        {
            float baseHeight = 20;

            baseHeight += planetRawData.GetFactoredScale() * 20000;

            return (int)(((planetRawData.modData[index >> 1] >> (((index & 1) << 2) + 2)) & 3) * 133 + baseHeight);
        }
        
        /// <summary>
        /// Converts heightData value to world height.
        /// For large planets, uses full precision storage.
        /// </summary>
        public static float GetWorldHeight(this PlanetRawData data, int index)
        {
            return data.GetHeightDataFloat(index);
        }
    }
}