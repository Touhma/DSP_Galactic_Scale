using System;
using UnityEngine;

namespace GalacticScale
{
    public static partial class TerrainAlgorithms
    {
        /// <summary>
        /// Custom terrain generation algorithm for large planets (>200 radius)
        /// Uses full-precision height data storage via SetHeightData method
        /// Based on vanilla algorithm 10 but modified for large planets
        /// </summary>
        public static void GenerateTerrainLarge(GSPlanet gsPlanet, double modX = 0.0, double modY = 0.0)
        {
            if (gsPlanet == null)
            {
                GS2.Warn("gsPlanet Null");
                return;
            }

            random = new GS2.Random(gsPlanet.Seed);
            GS2.Log($"GenerateTerrainLarge: {gsPlanet.Name} (radius {gsPlanet.Radius})");
            
            var t = gsPlanet.GsTheme.TerrainSettings;
            var xPrecision = t.xFactor;
            var yPrecision = t.yFactor;
            var zPrecision = t.zFactor;
            var heightMulti = t.HeightMulti; // Height Factor default 3.0
            var baseHeight = t.BaseHeight; // Base Height offset -2.5 - 2.5
            var landModifier = t.LandModifier;
            var biomeHeightMulti = t.BiomeHeightMulti;
            var biomeHeightModifier = t.BiomeHeightModifier;

            var seed1 = random.Next();
            var seed2 = random.Next();
            var simplexNoise1 = new SimplexNoise(seed1);
            var simplexNoise2 = new SimplexNoise(seed2);
            
            if (gsPlanet.planetData == null)
            {
                GS2.Warn("gsPlanet.planetData Null");
                return;
            }

            var data = gsPlanet.planetData.data;
            if (data == null) return;
            
            float radius = gsPlanet.Radius;
            var planet = gsPlanet.planetData;
            
            // CRITICAL: Use planet.precision instead of planet.radius for height calculations
            // This ensures terrain generation matches mesh generation expectations
            float precision = planet.precision;
            
            GS2.Log($"GenerateTerrainLarge: Using precision={precision} for radius={radius}");
            
            for (var i = 0; i < data.dataLength; ++i)
            {
                if (data.vertices == null) return;
                
                // Use precision for terrain calculations (not radius)
                var num1 = data.vertices[i].x * (double)precision;
                var num2 = data.vertices[i].y * (double)precision;
                var num3 = data.vertices[i].z * (double)precision;
                var num4 = 0.0;
                var num5 = Maths.Levelize(num1 * 0.007);
                var num6 = Maths.Levelize(num2 * 0.007);
                var num7 = Maths.Levelize(num3 * 0.007);
                var xin = num5 + simplexNoise1.Noise(num1 * xPrecision, num2 * xPrecision, num3 * xPrecision) * 0.04 * t.RandomFactor;
                var yin = num6 + simplexNoise1.Noise(num2 * yPrecision, num3 * yPrecision, num1 * yPrecision) * 0.04 * t.RandomFactor;
                var zin = num7 + simplexNoise1.Noise(num3 * zPrecision, num1 * zPrecision, num2 * zPrecision) * 0.04 * t.RandomFactor;
                var num8 = Math.Abs(simplexNoise2.Noise(xin, yin, zin));
                var num9 = (0.16 - num8) * 10.0 * (1 + t.LandModifier);
                var num10 = num9 <= 0.0 ? 0.0 : num9 <= 1.0 ? num9 : 1.0;
                var num11 = num10 * num10;
                var num12 = (simplexNoise1.Noise3DFBM(num2 * 0.005, num3 * 0.005, num1 * 0.005, 4) + 0.22) * 5.0;
                var num13 = num12 <= 0.0 ? 0.0 : num12 <= 1.0 ? num12 : 1.0;
                var num14 = Math.Abs(simplexNoise2.Noise3DFBM(xin * 1.5, yin * 1.5, zin * 1.5, 2));
                var num15 = num4 - num11 * 1.2 * num13;
                if (num15 >= 0.0) num15 += num8 * 0.25 + num14 * 0.6;

                var num16 = num15 - 0.1;
                var num17 = -0.3 - num16;
                if (num17 > 0.0)
                {
                    var num18 = num17 <= 1.0 ? num17 : 1.0;
                    num16 = -0.3 - (3.0 - num18 - num18) * num18 * num18 * 3.70000004768372;
                }

                var num19 = Maths.Levelize(num11 <= 0.300000011920929 ? 0.300000011920929 : num11, 0.7);
                var num20 = num16 <= -0.800000011920929 ? (-num19 - num8) * 0.899999976158142 : num16;
                var hdEndValue = num20 <= -1.20000004768372 ? -1.20000004768372 : num20;
                hdEndValue = hdEndValue * t.HeightMulti + t.BaseHeight;
                var bEndValue = hdEndValue * num11 + (num8 * 2.1 * t.BiomeHeightMulti + 0.800000011920929 + t.BiomeHeightModifier);
                if (bEndValue > 1.70000004768372 && bEndValue < 2.0) bEndValue = 2.0;
                
                if (data.heightData == null) return;
                
                // CRITICAL: Calculate height using precision, then store with SetHeightData
                // This ensures the full-precision dictionary gets populated
                double heightValue = (precision + hdEndValue + 0.2) * 100.0;
                int heightInt = (int)heightValue;
                
                // Use our custom SetHeightData method for full precision
                data.SetHeightData(i, heightInt);
                
                // Also store in vanilla array for compatibility
                data.heightData[i] = (ushort)Mathf.Clamp(heightInt, 0, 65535);
                data.biomoData[i] = (byte)Mathf.Clamp((float)(bEndValue * 100.0), 0.0f, 200f);
                
                // Debug logging for first few vertices
                if (i < 5)
                {
                    GS2.Log($"GenerateTerrainLarge[{i}]: heightValue={heightValue:F2}, heightInt={heightInt}, " +
                           $"precision={precision}, hdEndValue={hdEndValue:F2}");
                }
            }
            
            GS2.Log($"GenerateTerrainLarge: Completed for {gsPlanet.Name}");
        }
    }
}
