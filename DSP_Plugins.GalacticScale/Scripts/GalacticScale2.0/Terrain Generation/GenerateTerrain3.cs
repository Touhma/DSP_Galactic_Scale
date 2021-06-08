using System;
using UnityEngine;

namespace GalacticScale
{
    public static partial class TerrainAlgorithms
    {
        public static void GenerateTerrain3(GSPlanet gsPlanet, double modX = 0.0, double modY = 0.0)
        {
            random = new GS2.Random(gsPlanet.Seed);
            GS2.Warn($"USING GSTA3 FOR {gsPlanet.Name} with seed {GSSettings.Seed}");
            GSTerrainSettings t = GS2.ThemeLibrary[gsPlanet.Theme].TerrainSettings;
            //GS2.Log("Generate Terrain for " + gsPlanet.Name + " 3 ");
            PlanetData planet = gsPlanet.planetData;
            double num1 = 0.007;
            double num2 = 0.007;
            double num3 = 0.007;
            //GS2.Random random = new GS2.Random(planet.seed);
            int seed1 = random.Next();
            int seed2 = random.Next();
            SimplexNoise simplexNoise1 = new SimplexNoise(seed1);
            SimplexNoise simplexNoise2 = new SimplexNoise(seed2);
            PlanetRawData data = planet.data;
            for (int index = 0; index < data.dataLength; ++index)
            {
                double num4 = (double)data.vertices[index].x * (double)planet.radius;
                double num5 = (double)data.vertices[index].y * (double)planet.radius;
                double num6 = (double)data.vertices[index].z * (double)planet.radius;
                double num7 = num4 + Math.Sin(num5 * 0.15) * 3.0;
                double num8 = num5 + Math.Sin(num6 * 0.15) * 3.0;
                double num9 = num6 + Math.Sin(num7 * 0.15) * 3.0;
                double num10 = simplexNoise1.Noise3DFBM(num7 * num1 * 1.0, num8 * num2 * 1.1, num9 * num3 * 1.0, 6, deltaWLen: 1.8);
                double num11 = simplexNoise2.Noise3DFBM(num7 * num1 * 1.3 + 0.5, num8 * num2 * 2.8 + 0.2, num9 * num3 * 1.3 + 0.7, 3) * 2.0;
                double num12 = simplexNoise2.Noise3DFBM(num7 * num1 * 6.0, num8 * num2 * 12.0, num9 * num3 * 6.0, 2) * 2.0;
                double num13 = simplexNoise2.Noise3DFBM(num7 * num1 * 0.8, num8 * num2 * 0.8, num9 * num3 * 0.8, 2) * 2.0;
                double f = num10 * 2.0 + 0.92 + (double)Mathf.Clamp01((float)(num11 * (double)Mathf.Abs((float)num13 + 0.5f) - 0.35) * 1f);
                //GS2.Log(f.ToString());
                if (f < 0.0)
                    f *= 2.0;
                double num14 = Maths.Levelize2(f);
                if (num14 > 0.0)
                    num14 = Maths.Levelize4(Maths.Levelize2(f));
                double num15 = num14 <= 0.0 ? (double)Mathf.Lerp(-4f, 0.0f, (float)num14 + 1f) : (num14 <= 1.0 ? (double)Mathf.Lerp(0.0f, 0.3f, (float)num14) + num12 * 0.1 : (num14 <= 2.0 ? (double)Mathf.Lerp(0.3f, 1.4f, (float)num14 - 1f) + num12 * 0.12 : (double)Mathf.Lerp(1.4f, 2.7f, (float)num14 - 2f) + num12 * 0.12));
                if (f < 0.0)
                    f *= 2.0;
                if (f < 1.0)
                    f = Maths.Levelize(f);
                double num17 = (double)Mathf.Abs((float)f);
                double num18 = num17 <= 0.0 ? 0.0 : (num17 <= 2.0 ? num17 : 2.0);
                double num19 = num18 + (num18 <= 1.8 ? num12 * 0.2 : -num12 * 0.8);
                data.heightData[index] = (ushort)(((double)planet.radius + (num15 * t.HeightMulti) + 0.2 + t.BaseHeight) * 100.0);
                data.biomoData[index] = (byte)Mathf.Clamp((float)(num19 * 100.0 * t.BiomeHeightMulti + t.BiomeHeightModifier), 0.0f, 200f);
            }
            //GS2.Log("--------------------");
           
        }
    }
}