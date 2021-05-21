using System;
using UnityEngine;

namespace GalacticScale
{
    public static partial class VeinAlgorithms
    {
        public static void GenerateTerrain6(GSPlanet gsPlanet, double modX = 0.0, double modY = 0.0)
        {

            GSTerrainSettings t = GS2.ThemeLibrary[gsPlanet.Theme].TerrainSettings;
            //GS2.Log("Generate Terrain for "+gsPlanet.Name + " " + t.landModifier );
            double xPrecision = t.xFactor;
            double yPrecision = t.yFactor;
            double zPrecision = t.zFactor;
            double heightMulti = t.HeightMulti; //Height Factor default 3.0
            double baseHeight = t.BaseHeight; //Base Height offset -2.5 - 2.5
            double landModifier = t.LandModifier;
            double biomeHeightMulti = t.BiomeHeightMulti;
            double biomeHeightModifier = t.BiomeHeightModifier;


            System.Random random = new System.Random(gsPlanet.Seed);
            int seed1 = random.Next();
            int seed2 = random.Next();
            SimplexNoise simplexNoise1 = new SimplexNoise(seed1);
            SimplexNoise simplexNoise2 = new SimplexNoise(seed2);
            PlanetRawData data = gsPlanet.planetData.data;
            float radius = gsPlanet.Radius;
            for (int index = 0; index < data.dataLength; ++index)
            {
                double num1 = (double)data.vertices[index].x * (double)radius;
                double num2 = (double)data.vertices[index].y * (double)radius;
                double num3 = (double)data.vertices[index].z * (double)radius;
                double num4 = 0.0;
                double num5 = Maths.Levelize(num1 * 0.007);
                double num6 = Maths.Levelize(num2 * 0.007);
                double num7 = Maths.Levelize(num3 * 0.007);
                double xin = num5 + simplexNoise1.Noise(num1 * xPrecision, num2 * xPrecision, num3 * xPrecision) * 0.04 * t.RandomFactor;
                double yin = num6 + simplexNoise1.Noise(num2 * yPrecision, num3 * yPrecision, num1 * yPrecision) * 0.04 * t.RandomFactor;
                double zin = num7 + simplexNoise1.Noise(num3 * zPrecision, num1 * zPrecision, num2 * zPrecision) * 0.04 * t.RandomFactor;
                double num8 = Math.Abs(simplexNoise2.Noise(xin, yin, zin));
                double num9 = (0.16 - num8) * 10.0 * (1+t.LandModifier);
                double num10 = num9 <= 0.0 ? 0.0 : (num9 <= 1.0 ? num9 : 1.0);
                double num11 = num10 * num10;
                double num12 = (simplexNoise1.Noise3DFBM(num2 * 0.005, num3 * 0.005, num1 * 0.005, 4) + 0.22) * 5.0;
                double num13 = num12 <= 0.0 ? 0.0 : (num12 <= 1.0 ? num12 : 1.0);
                double num14 = Math.Abs(simplexNoise2.Noise3DFBM(xin * 1.5, yin * 1.5, zin * 1.5, 2));
                double num15 = num4 - num11 * 1.2 * num13;
                if (num15 >= 0.0)
                    num15 += num8 * 0.25 + num14 * 0.6;
                double num16 = num15 - 0.1;
                double num17 = -0.3 - num16;
                if (num17 > 0.0)
                {
                    double num18 = num17 <= 1.0 ? num17 : 1.0;
                    num16 = -0.3 - (3.0 - num18 - num18) * num18 * num18 * 3.70000004768372;
                }
                double num19 = Maths.Levelize(num11 <= 0.300000011920929 ? 0.300000011920929 : num11, 0.7);
                double num20 = num16 <= -0.800000011920929 ? (-num19 - num8) * 0.899999976158142 : num16;
                double hdEndValue = num20 <= -1.20000004768372 ? -1.20000004768372 : num20;
                hdEndValue = hdEndValue * t.HeightMulti + t.BaseHeight;
                double bEndValue = hdEndValue * num11 + (num8 * 2.1 * t.BiomeHeightMulti + 0.800000011920929 + t.BiomeHeightModifier);
                if (bEndValue > 1.70000004768372 && bEndValue < 2.0)
                    bEndValue = 2.0;
                data.heightData[index] = (ushort)(((double)radius + hdEndValue + 0.2) * 100.0);
                data.biomoData[index] = (byte)Mathf.Clamp((float)(bEndValue * 100.0), 0.0f, 200f);
            }
        }
    }
}