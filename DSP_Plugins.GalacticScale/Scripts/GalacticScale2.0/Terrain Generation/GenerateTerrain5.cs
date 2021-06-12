﻿using System;
using UnityEngine;

namespace GalacticScale {
    public static partial class TerrainAlgorithms {
        private static GS2.Random random;
        public static void GenerateTerrain5(GSPlanet gsPlanet, double modX = 0.0, double modY = 0.0) {
            random = new GS2.Random(gsPlanet.Seed);
            GS2.Log($"USING GSTA5 FOR {gsPlanet.Name} with seed {GSSettings.Seed}");
            GSTerrainSettings t = GS2.ThemeLibrary[gsPlanet.Theme].TerrainSettings;
            //GS2.Log("Generate 2 Terrain ");
            int seed = random.Next();
            int seed2 = random.Next();
            SimplexNoise simplexNoise = new SimplexNoise(seed);
            SimplexNoise simplexNoise2 = new SimplexNoise(seed2);
            PlanetRawData data = gsPlanet.planetData.data;
            for (int i = 0; i < data.dataLength; i++) {
                double num = (double)(data.vertices[i].x * gsPlanet.planetData.radius + t.xFactor);
                double num2 = (double)(data.vertices[i].y * gsPlanet.planetData.radius + t.yFactor);
                double num3 = (double)(data.vertices[i].z * gsPlanet.planetData.radius + t.zFactor);
                double hdEnd = 0.0;
                double num5 = Maths.Levelize(num * 0.007, 1.0, 0.0);
                double num6 = Maths.Levelize(num2 * 0.007, 1.0, 0.0);
                double num7 = Maths.Levelize(num3 * 0.007, 1.0, 0.0);
                num5 += simplexNoise.Noise(num * 0.05, num2 * 0.05, num3 * 0.05) * 0.04 * (1 + t.LandModifier);
                num6 += simplexNoise.Noise(num2 * 0.05, num3 * 0.05, num * 0.05) * 0.04 * (1 + t.LandModifier);
                num7 += simplexNoise.Noise(num3 * 0.05, num * 0.05, num2 * 0.05) * 0.04 * (1 + t.LandModifier);
                double num8 = Math.Abs(simplexNoise2.Noise(num5, num6, num7));
                double num9 = (0.16 - num8) * 10.0;
                num9 = ((num9 <= 0.0) ? 0.0 : ((num9 <= 1.0) ? num9 : 1.0));
                num9 *= num9;
                double num10 = simplexNoise.Noise3DFBM(num2 * 0.005, num3 * 0.005, num * 0.005, 4, 0.5, 2.0);
                double num11 = (num10 + 0.22) * 5.0;
                num11 = ((num11 <= 0.0) ? 0.0 : ((num11 <= 1.0) ? num11 : 1.0));
                double num12 = Math.Abs(simplexNoise2.Noise3DFBM(num5 * 1.5, num6 * 1.5, num7 * 1.5, 2, 0.5, 2.0)) * t.RandomFactor;
                double num13 = simplexNoise.Noise3DFBM(num3 * 0.06, num2 * 0.06, num * 0.06, 2, 0.5, 2.0);
                hdEnd -= num9 * 1.2 * num11;
                if (hdEnd >= 0.0) {
                    hdEnd += num8 * 0.25 + num12 * 0.6;
                }
                hdEnd -= 0.1;
                double bdEnd = num8 * 2.1;
                if (bdEnd < 0.0) {
                    bdEnd *= 5.0;
                }
                bdEnd = ((bdEnd <= -1.0) ? -1.0 : ((bdEnd <= 2.0) ? bdEnd : 2.0));
                bdEnd += num13 * 0.6 * bdEnd;
                double num15 = -0.3 - hdEnd;
                num15 = num15 * (1 + t.LandModifier);
                if (num15 > 0.0) {
                    num15 = ((num15 <= 1.0) ? num15 : 1.0);
                    num15 = (3.0 - num15 - num15) * num15 * num15;
                    hdEnd = -0.3 - num15 * 3.700000047683716;
                }
                data.heightData[i] = (ushort)((gsPlanet.planetData.radius + hdEnd + 0.2 + t.BaseHeight) * 100.0 * t.HeightMulti);
                data.biomoData[i] = (byte)Mathf.Clamp((float)((bdEnd + t.BiomeHeightModifier) * 100.0 * t.BiomeHeightMulti), 0f, 200f);

            }
        }
    }
}