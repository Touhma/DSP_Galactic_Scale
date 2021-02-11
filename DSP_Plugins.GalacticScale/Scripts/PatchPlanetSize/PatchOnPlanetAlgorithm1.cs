using System;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Random = System.Random;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize  {
    [HarmonyPatch(typeof(PlanetAlgorithm1))]
    public class PatchOnPlanetAlgorithm1 {
        [HarmonyPrefix]
        [HarmonyPatch("GenerateTerrain")]
        public static bool PatchGenerateTerrain(ref PlanetAlgorithm1 __instance, ref PlanetData ___planet,
            double modX, double modY) {
            if (___planet.name == "Luna") {
                Patch.Debug("GenerateTerrain Luna", LogLevel.Debug,
                    Patch.DebugPlanetAlgorithm1);
                double constant1 = 0.01;
                double constant2 = 0.012;
                double constant3 = 0.01;
                double constant4 = 3.0;
                double constant5 = -0.2;
                double constant6 = 0.9;
                double constant7 = 0.5;
                double constant8 = 2.5;
                double constant9 = 0.3;
                Random random = new Random(___planet.seed);
                int seed1 = random.Next();
                int seed2 = random.Next();
                SimplexNoise simplexNoise1 = new SimplexNoise(seed1);
                SimplexNoise simplexNoise2 = new SimplexNoise(seed2);
                PlanetRawData data = ___planet.data;
                for (int index = 0; index < data.dataLength; ++index) {
                    double xAxis = data.vertices[index].x * (double) ___planet.radius;
                    double yAxis = data.vertices[index].y * (double) ___planet.radius;
                    double zAxis = data.vertices[index].z * (double) ___planet.radius;
                    double noiseSimplex3D =
                        simplexNoise1.Noise3DFBM(xAxis * constant1, yAxis * constant2, zAxis * constant3, 6) *
                        constant4 + constant5;
                    double secondNoiseSimplex3D =
                        simplexNoise2.Noise3DFBM(xAxis * (1.0 / 400.0), yAxis * (1.0 / 400.0),
                            zAxis * (1.0 / 400.0),
                            3) * constant4 * constant6 + constant7;
                    double noiseFactor =
                        secondNoiseSimplex3D <= 0.0 ? secondNoiseSimplex3D : secondNoiseSimplex3D * 0.5;
                    double noiseOffset = noiseSimplex3D + noiseFactor;
                    double f = noiseOffset <= 0.0 ? noiseOffset * 1.6 : noiseOffset * 0.5;
                    double fLevelized = f <= 0.0 ? Maths.Levelize2(f, 0.5) : Maths.Levelize3(f, 0.7);
                    double noiseFactor2 =
                        simplexNoise2.Noise3DFBM(xAxis * constant1 * 2.5, yAxis * constant2 * 8.0,
                            zAxis * constant3 * 2.5, 2) * 0.6 -
                        0.3;
                    double fNoisified = f * constant8 + noiseFactor2 + constant9;
                    double fNoisifiedAdjusted = fNoisified >= 1.0 ? (fNoisified - 1.0) * 0.8 + 1.0 : fNoisified;

                    // I can increase that for more irregular terrain
                    float amplitude = 100f;

                    data.heightData[index] = (ushort) (((___planet.radius + fLevelized + 0.2) * amplitude));

                    Patch.Debug("___planet.radius : " + ___planet.radius, LogLevel.Debug,
                        Patch.DebugPlanetAlgorithm1);
                    Patch.Debug("scaleFactor : " + Patch.scaleFactor, LogLevel.Debug,
                        Patch.DebugPlanetAlgorithm1);
                    Patch.Debug(
                        "(___planet.radius + fLevelized + 0.2) * amplitude) * scaleFactor : " +
                        ((___planet.radius + fLevelized + 0.2) * amplitude) * Patch.scaleFactor,
                        LogLevel.Debug, Patch.DebugPlanetAlgorithm1);
                    Patch.Debug("data.heightData[" + index + "] : " + data.heightData[index],
                        LogLevel.Debug, Patch.DebugPlanetAlgorithm1);

                    data.biomoData[index] = (byte) Mathf.Clamp((float) (fNoisifiedAdjusted * 100.0), 0.0f, 200f);

                    Patch.Debug("data.biomoData[" + index + "] : " + data.biomoData[index], LogLevel.Debug,
                        Patch.DebugPlanetAlgorithm1);
                }

                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GenerateVegetables")]
        public static bool PatchGenerateVegetables(ref PlanetData ___planet) {
            Patch.Debug("GenerateVegetables :  " + ___planet.radius + " for : " + ___planet.name,
                LogLevel.Debug, Patch.DebugPlanetAlgorithm1);
            ThemeProto themeProto = LDB.themes.Select(___planet.theme);
            if (themeProto == null)
                return false;
            int[] vegetables0 = themeProto.Vegetables0;
            int[] vegetables1 = themeProto.Vegetables1;
            int[] vegetables2 = themeProto.Vegetables2;
            int[] vegetables3 = themeProto.Vegetables3;
            int[] vegetables4 = themeProto.Vegetables4;
            int[] vegetables5 = themeProto.Vegetables5;
            float num1 = 1.3f;
            float num2 = -0.5f;
            float num3 = 2.5f;
            float num4 = 4f;
            float num5 = 0.5f;
            float num6 = 1f;
            float num7 = 2f;
            float num8 = -0.2f;
            float num9 = 1.4f;
            Random random1 = new Random(___planet.seed);
            random1.Next();
            random1.Next();
            random1.Next();
            Random random2 = new Random(random1.Next());
            SimplexNoise simplexNoise1 = new SimplexNoise(random2.Next());
            SimplexNoise simplexNoise2 = new SimplexNoise(random2.Next());
            PlanetRawData data = ___planet.data;
            int stride = data.stride;
            int num10 = stride / 2;
            float num11 = (float) (___planet.radius * 3.1415901184082 * 2.0 /
                                   (data.precision * 4.0));
            VegeData vege = new VegeData();
            VegeProto[] vegeProtos = PlanetModelingManager.vegeProtos;
            Vector4[] vegeScaleRanges = PlanetModelingManager.vegeScaleRanges;
            short[] vegeHps = PlanetModelingManager.vegeHps;
            for (int index1 = 0; index1 < data.dataLength; ++index1) {
                int num12 = index1 % stride;
                int num13 = index1 / stride;
                if (num12 > num10)
                    --num12;
                if (num13 > num10)
                    --num13;
                if (num12 % 2 == 1 && num13 % 2 == 1) {
                    Vector3 vertex = data.vertices[index1];
                    double num14 = data.vertices[index1].x * (double) ___planet.radius;
                    double num15 = data.vertices[index1].y * (double) ___planet.radius;
                    double num16 = data.vertices[index1].z * (double) ___planet.radius;
                    float a = data.heightData[index1] * 0.01f;
                    float b1 = data.heightData[index1 + 1 + stride] * 0.01f;
                    float b2 = data.heightData[index1 - 1 + stride] * 0.01f;
                    float b3 = data.heightData[index1 + 1 - stride] * 0.01f;
                    float b4 = data.heightData[index1 - 1 - stride] * 0.01f;
                    float num17 = data.heightData[index1 + 1] * 0.01f;
                    float num18 = data.heightData[index1 - 1] * 0.01f;
                    float num19 = data.heightData[index1 + stride] * 0.01f;
                    float num20 = data.heightData[index1 - stride] * 0.01f;
                    float num21 = data.biomoData[index1] * 0.01f;
                    float num22 = ___planet.radius + 0.15f;
                    bool flag1 = false;
                    if (a < (double) num22)
                        flag1 = true;
                    else if (b1 < (double) num22)
                        flag1 = true;
                    else if (b2 < (double) num22)
                        flag1 = true;
                    else if (b3 < (double) num22)
                        flag1 = true;
                    else if (b4 < (double) num22)
                        flag1 = true;
                    else if (num17 < (double) num22)
                        flag1 = true;
                    else if (num18 < (double) num22)
                        flag1 = true;
                    else if (num19 < (double) num22)
                        flag1 = true;
                    else if (num20 < (double) num22)
                        flag1 = true;
                    if (!flag1 || vegetables5 != null && vegetables5.Length != 0) {
                        bool flag2 = true;
                        if (diff(a, b1) > 0.200000002980232)
                            flag2 = false;
                        if (diff(a, b2) > 0.200000002980232)
                            flag2 = false;
                        if (diff(a, b3) > 0.200000002980232)
                            flag2 = false;
                        if (diff(a, b4) > 0.200000002980232)
                            flag2 = false;
                        double num23 = random2.NextDouble();
                        double num24 = num23 * num23;
                        double num25 = random2.NextDouble();
                        float num26 = (float) random2.NextDouble() - 0.5f;
                        float num27 = (float) random2.NextDouble() - 0.5f;
                        float num28 = (float) Math.Sqrt(random2.NextDouble());
                        float angle1 = (float) random2.NextDouble() * 360f;
                        float num29 = (float) random2.NextDouble();
                        float num30 = (float) random2.NextDouble();
                        float num31 = 1f;
                        float num32 = 0.5f;
                        float num33 = 1f;
                        int[] numArray1;
                        if (!flag1) {
                            if (num21 < 0.800000011920929) {
                                numArray1 = vegetables0;
                                num31 = num1;
                                num32 = num2;
                                num33 = num3;
                            }
                            else {
                                numArray1 = vegetables1;
                                num31 = num4;
                                num32 = num5;
                                num33 = num6;
                            }
                        }
                        else
                            numArray1 = null;

                        double num34 =
                            simplexNoise1.Noise(num14 * 0.07, num15 * 0.07, num16 * 0.07) * num31 +
                            num32 + 0.5;
                        double num35 = simplexNoise2.Noise(num14 * 0.4, num15 * 0.4, num16 * 0.4) * num7 +
                                       num8 + 0.5;
                        double num36 = num35 - 0.55;
                        double num37 = num35 - 1.1;
                        int[] numArray2;
                        double num38;
                        int num39;
                        if (!flag1) {
                            if (num21 > 1.0) {
                                numArray2 = vegetables2;
                                num38 = num35;
                                num39 = vegetables5 == null || vegetables5.Length == 0 ? 4 : 2;
                            }
                            else if (num21 > 0.5) {
                                numArray2 = vegetables3;
                                num38 = num36;
                                num39 = 1;
                            }
                            else if (num21 > 0.0) {
                                numArray2 = vegetables4;
                                num38 = num36;
                                num39 = 1;
                            }
                            else {
                                numArray2 = null;
                                num38 = num35;
                                num39 = 1;
                            }
                        }
                        else if (a < num22 - 1.0 &&
                                 a > num22 - 2.20000004768372) {
                            numArray2 = vegetables5;
                            num38 = num37;
                            num39 = 1;
                        }
                        else
                            continue;

                        if (flag2 && num25 < num34 && (numArray1 != null && numArray1.Length > 0)) {
                            vege.protoId = (short) numArray1[(int) (num24 * numArray1.Length)];
                            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, vertex);
                            Vector3 vector3_1 = rotation * Vector3.forward;
                            Vector3 vector3_2 = rotation * Vector3.right;
                            Vector4 vector4 = vegeScaleRanges[vege.protoId];
                            Vector3 vector3_3 = vertex * a;
                            Vector3 vector3_4 = (vector3_2 * num26 + vector3_1 * num27).normalized *
                                                (num28 * num33 * num11);
                            float y = (float) (num30 * (vector4.x + (double) vector4.y) +
                                               (1.0 - vector4.x));
                            float num40 = (float) (num29 * (vector4.z + (double) vector4.w) +
                                                   (1.0 - vector4.z)) * y;
                            vege.pos = (vector3_3 + vector3_4).normalized;
                            a = data.QueryHeight(vege.pos);
                            vege.pos *= a;
                            vege.rot = Quaternion.FromToRotation(Vector3.up, vege.pos.normalized) *
                                       Quaternion.AngleAxis(angle1, Vector3.up);
                            vege.scl = new Vector3(num40, y, num40);
                            vege.modelIndex = (short) vegeProtos[vege.protoId].ModelIndex;
                            vege.hp = vegeHps[vege.protoId];
                            int num41 = data.AddVegeData(vege);
                            data.vegeIds[index1] = (ushort) num41;
                        }

                        if (num25 < num38 && numArray2 != null && numArray2.Length > 0) {
                            vege.protoId = (short) numArray2[(int) (num24 * numArray2.Length)];
                            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, vertex);
                            Vector3 vector3_1 = rotation * Vector3.forward;
                            Vector3 vector3_2 = rotation * Vector3.right;
                            Vector4 vector4 = vegeScaleRanges[vege.protoId];
                            for (int index2 = 0; index2 < num39; ++index2) {
                                float num40 = (float) random2.NextDouble() - 0.5f;
                                float num41 = (float) random2.NextDouble() - 0.5f;
                                float num42 = (float) Math.Sqrt(random2.NextDouble());
                                float angle2 = (float) random2.NextDouble() * 360f;
                                float num43 = (float) random2.NextDouble();
                                float num44 = (float) random2.NextDouble();
                                Vector3 vector3_3 = vertex * a;
                                Vector3 vector3_4 = (vector3_2 * num40 + vector3_1 * num41).normalized *
                                                    (num42 * num9 * num11);
                                float y = (float) (num44 * (vector4.x + (double) vector4.y) +
                                                   (1.0 - vector4.x));
                                float num45 = (float) (num43 * (vector4.z + (double) vector4.w) +
                                                       (1.0 - vector4.z)) * y;
                                vege.pos = (vector3_3 + vector3_4).normalized;
                                a = !flag1 ? data.QueryHeight(vege.pos) : num22;

                                // Patched Vegetation position depending on the scaleFactor
                                vege.pos *= a * Patch.scaleFactor;
                                vege.rot = Quaternion.FromToRotation(Vector3.up, vege.pos.normalized) *
                                           Quaternion.AngleAxis(angle2, Vector3.up);
                                vege.scl = new Vector3(num45, y, num45);
                                vege.modelIndex = (short) vegeProtos[vege.protoId].ModelIndex;
                                vege.hp = 1;
                                int num46 = data.AddVegeData(vege);
                                data.vegeIds[index1] = (ushort) num46;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static float diff(float a, float b) => (double) a > (double) b ? a - b : b - a;
    }
}