using System;
using UnityEngine;

namespace GalacticScale
{
    public static partial class TerrainAlgorithms
    {
        public static void GenerateTerrain3(GSPlanet gsPlanet, double modX = 0.0, double modY = 0.0)
        {
            if (gsPlanet == null)
            {
                GS2.Warn("gsPlanet Null");
                return;
            }

            random = new GS2.Random(gsPlanet.Seed);
            GS2.Log($"USING GSTA3 FOR {gsPlanet.Name} with seed {GSSettings.Seed}");
            var t = gsPlanet?.GsTheme.TerrainSettings;
            GS2.Log("Generate Terrain for " + gsPlanet.Name + " 3 ");
            var planet = gsPlanet.planetData;
            if (planet == null)
            {
                GS2.Warn("planet Null");
                return;
            }

            var num1 = 0.007;
            var num2 = 0.007;
            var num3 = 0.007;
            //GS2.Random random = new GS2.Random(planet.seed);
            var seed1 = random.Next();
            var seed2 = random.Next();
            var simplexNoise1 = new SimplexNoise(seed1);
            var simplexNoise2 = new SimplexNoise(seed2);
            var data = planet.data;
            for (var index = 0; index < data.dataLength; ++index)
            {
                var num4 = data.vertices[index].x * (double)planet.radius;
                var num5 = data.vertices[index].y * (double)planet.radius;
                var num6 = data.vertices[index].z * (double)planet.radius;
                var num7 = num4 + Math.Sin(num5 * 0.15) * 3.0;
                var num8 = num5 + Math.Sin(num6 * 0.15) * 3.0;
                var num9 = num6 + Math.Sin(num7 * 0.15) * 3.0;
                var num10 = simplexNoise1.Noise3DFBM(num7 * num1 * 1.0, num8 * num2 * 1.1, num9 * num3 * 1.0, 6, deltaWLen: 1.8);
                var num11 = simplexNoise2.Noise3DFBM(num7 * num1 * 1.3 + 0.5, num8 * num2 * 2.8 + 0.2, num9 * num3 * 1.3 + 0.7, 3) * 2.0;
                var num12 = simplexNoise2.Noise3DFBM(num7 * num1 * 6.0, num8 * num2 * 12.0, num9 * num3 * 6.0, 2) * 2.0;
                var num13 = simplexNoise2.Noise3DFBM(num7 * num1 * 0.8, num8 * num2 * 0.8, num9 * num3 * 0.8, 2) * 2.0;
                var f = num10 * 2.0 + 0.92 + Mathf.Clamp01((float)(num11 * Mathf.Abs((float)num13 + 0.5f) - 0.35) * 1f);
                //GS2.Log(f.ToString());
                if (f < 0.0) f *= 2.0;

                var num14 = Maths.Levelize2(f);
                if (num14 > 0.0) num14 = Maths.Levelize4(Maths.Levelize2(f));

                var num15 = num14 <= 0.0 ? Mathf.Lerp(-4f, 0.0f, (float)num14 + 1f) : num14 <= 1.0 ? Mathf.Lerp(0.0f, 0.3f, (float)num14) + num12 * 0.1 : num14 <= 2.0 ? Mathf.Lerp(0.3f, 1.4f, (float)num14 - 1f) + num12 * 0.12 : Mathf.Lerp(1.4f, 2.7f, (float)num14 - 2f) + num12 * 0.12;
                if (f < 0.0) f *= 2.0;

                if (f < 1.0) f = Maths.Levelize(f);

                double num17 = Mathf.Abs((float)f);
                var num18 = num17 <= 0.0 ? 0.0 : num17 <= 2.0 ? num17 : 2.0;
                var num19 = num18 + (num18 <= 1.8 ? num12 * 0.2 : -num12 * 0.8);
                data.heightData[index] = (ushort)((planet.radius + num15 * t.HeightMulti + 0.2 + t.BaseHeight) * 100.0);
                data.biomoData[index] = (byte)Mathf.Clamp((float)(num19 * 100.0 * t.BiomeHeightMulti + t.BiomeHeightModifier), 0.0f, 200f);
            }
            GS2.Log("--------------------");
        }
    }
}