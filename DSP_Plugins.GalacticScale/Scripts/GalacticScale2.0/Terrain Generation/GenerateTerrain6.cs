﻿using System;
using UnityEngine;

namespace GalacticScale {
    public static partial class TerrainAlgorithms {
        public static void GenerateTerrain6(GSPlanet gsPlanet, double modX = 0.0, double modY = 0.0) {
            random = new GS2.Random(gsPlanet.Seed);
            GS2.Log($"USING GSTA6 FOR {gsPlanet.Name} with seed {GSSettings.Seed}");
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


            //GS2.Random random = new GS2.Random(gsPlanet.Seed);
            int seed1 = random.Next();
            int seed2 = random.Next();
            SimplexNoise simplexNoise1 = new SimplexNoise(seed1);
            SimplexNoise simplexNoise2 = new SimplexNoise(seed2);
            PlanetRawData data = gsPlanet.planetData.data;
            float radius = gsPlanet.Radius;
            PlanetData planet = gsPlanet.planetData;
            for (int i = 0; i < data.dataLength; ++i) {
                double num1 = data.vertices[i].x * (double)radius;
                double num2 = data.vertices[i].y * (double)radius;
                double num3 = data.vertices[i].z * (double)radius;
                double num4 = 0.0;
                double num5 = Maths.Levelize(num1 * 0.007);
                double num6 = Maths.Levelize(num2 * 0.007);
                double num7 = Maths.Levelize(num3 * 0.007);
                double xin = num5 + simplexNoise1.Noise(num1 * xPrecision, num2 * xPrecision, num3 * xPrecision) * 0.04 * t.RandomFactor;
                double yin = num6 + simplexNoise1.Noise(num2 * yPrecision, num3 * yPrecision, num1 * yPrecision) * 0.04 * t.RandomFactor;
                double zin = num7 + simplexNoise1.Noise(num3 * zPrecision, num1 * zPrecision, num2 * zPrecision) * 0.04 * t.RandomFactor;
                double num8 = Math.Abs(simplexNoise2.Noise(xin, yin, zin));
                double num9 = (0.16 - num8) * 10.0 * (1 + t.LandModifier);
                double num10 = num9 <= 0.0 ? 0.0 : (num9 <= 1.0 ? num9 : 1.0);
                double num11 = num10 * num10;
                double num12 = (simplexNoise1.Noise3DFBM(num2 * 0.005, num3 * 0.005, num1 * 0.005, 4) + 0.22) * 5.0;
                double num13 = num12 <= 0.0 ? 0.0 : (num12 <= 1.0 ? num12 : 1.0);
                double num14 = Math.Abs(simplexNoise2.Noise3DFBM(xin * 1.5, yin * 1.5, zin * 1.5, 2));
                double num15 = num4 - num11 * 1.2 * num13;
                if (num15 >= 0.0) {
                    num15 += num8 * 0.25 + num14 * 0.6;
                }

                double num16 = num15 - 0.1;
                double num17 = -0.3 - num16;
                if (num17 > 0.0) {
                    double num18 = num17 <= 1.0 ? num17 : 1.0;
                    num16 = -0.3 - (3.0 - num18 - num18) * num18 * num18 * 3.70000004768372;
                }
                double num19 = Maths.Levelize(num11 <= 0.300000011920929 ? 0.300000011920929 : num11, 0.7);
                double num20 = num16 <= -0.800000011920929 ? (-num19 - num8) * 0.899999976158142 : num16;
                double hdEndValue = num20 <= -1.20000004768372 ? -1.20000004768372 : num20;
                hdEndValue = hdEndValue * t.HeightMulti + t.BaseHeight;
                double bEndValue = hdEndValue * num11 + (num8 * 2.1 * t.BiomeHeightMulti + 0.800000011920929 + t.BiomeHeightModifier);
                if (bEndValue > 1.70000004768372 && bEndValue < 2.0) {
                    bEndValue = 2.0;
                }

                data.heightData[i] = (ushort)((radius + hdEndValue + 0.2) * 100.0);
                data.biomoData[i] = (byte)Mathf.Clamp((float)(bEndValue * 100.0), 0.0f, 200f);
                //double num3 = data.vertices[i].x * planet.radius;
                //double num4 = data.vertices[i].y * planet.radius;
                //double num5 = data.vertices[i].z * planet.radius;
                //double num6 = 0.0;
                //double num7 = 0.0;
                //double num8 = Maths.Levelize(num3 * 0.007);
                //double num9 = Maths.Levelize(num4 * 0.007);
                //double num10 = Maths.Levelize(num5 * 0.007);
                //num8 += simplexNoise.Noise(num3 * 0.05, num4 * 0.05, num5 * 0.05) * 0.04;
                //num9 += simplexNoise.Noise(num4 * 0.05, num5 * 0.05, num3 * 0.05) * 0.04;
                //num10 += simplexNoise.Noise(num5 * 0.05, num3 * 0.05, num4 * 0.05) * 0.04;
                //double num11 = Math.Abs(simplexNoise2.Noise(num8, num9, num10));
                //double num12 = (0.16 - num11) * 10.0;
                //num12 = ((!(num12 > 0.0)) ? 0.0 : ((!(num12 > 1.0)) ? num12 : 1.0));
                //num12 *= num12;
                //double num13 = simplexNoise.Noise3DFBM(num4 * 0.005, num5 * 0.005, num3 * 0.005, 4);
                //double num14 = (num13 + 0.22) * 5.0;
                //num14 = ((!(num14 > 0.0)) ? 0.0 : ((!(num14 > 1.0)) ? num14 : 1.0));
                //double num15 = Math.Abs(simplexNoise2.Noise3DFBM(num8 * 1.5, num9 * 1.5, num10 * 1.5, 2));
                //num6 -= num12 * 1.2 * num14;
                //if (num6 >= 0.0)
                //{
                //    num6 += num11 * 0.25 + num15 * 0.6;
                //}
                //num6 -= 0.1;
                //double num16 = -0.3 - num6;
                //if (num16 > 0.0)
                //{
                //    num16 = ((!(num16 > 1.0)) ? num16 : 1.0);
                //    num16 = (3.0 - num16 - num16) * num16 * num16;
                //    num6 = -0.3 - num16 * 3.7000000476837158;
                //}
                //double f = ((!(num12 > 0.30000001192092896)) ? 0.30000001192092896 : num12);
                //f = Maths.Levelize(f, 0.7);
                //num6 = ((!(num6 > -0.800000011920929)) ? ((0.0 - f - num11) * 0.89999997615814209) : num6);
                //num6 = ((!(num6 > -1.2000000476837158)) ? (-1.2000000476837158) : num6);
                //num7 = num6 * num12;
                //num7 += num11 * 2.1 + 0.800000011920929;
                //if (num7 > 1.7000000476837158 && num7 < 2.0)
                //{
                //    num7 = 2.0;
                //}
                //data.heightData[i] = (ushort)(((double)planet.radius + num6 + 0.2) * 100.0);
                //data.biomoData[i] = (byte)Mathf.Clamp((float)(num7 * 100.0), 0f, 200f);
            }
        }
    }
}