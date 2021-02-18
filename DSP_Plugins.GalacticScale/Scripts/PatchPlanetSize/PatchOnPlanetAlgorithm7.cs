using System;
using BepInEx.Logging;
using GalacticScale.Scripts.PatchStarSystemGeneration;
using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
using Random = System.Random;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetAlgorithm7))]
    public class PatchOnPlanetAlgorithm7 { 
        [HarmonyPrefix]
        [HarmonyPatch("GenerateVegetables")]
        public static bool PatchGenerateVegetables(ref PlanetData ___planet) {
            Patch.Debug("GenerateVegetables 7:  " + ___planet.radius + " for : " + ___planet.name,
                LogLevel.Debug, Patch.DebugPlanetAlgorithm7);
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
            System.Random random1 = new System.Random(___planet.seed);
            random1.Next();
            random1.Next();
            random1.Next();
            System.Random random2 = new System.Random(random1.Next());
            SimplexNoise simplexNoise1 = new SimplexNoise(random2.Next());
            SimplexNoise simplexNoise2 = new SimplexNoise(random2.Next());
            PlanetRawData data = ___planet.data;
            int stride = data.stride;
            int num10 = stride / 2;
            float num11 =
                (float) ((double) ___planet.radius * 3.1415901184082 * 2.0 / ((double) data.precision * 4.0));
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
                    double num14 = (double) data.vertices[index1].x * (double) ___planet.radius;
                    double num15 = (double) data.vertices[index1].y * (double) ___planet.radius;
                    double num16 = (double) data.vertices[index1].z * (double) ___planet.radius;
                    float a = (float) data.heightData[index1] * 0.01f;
                    float b1 = (float) data.heightData[index1 + 1 + stride] * 0.01f;
                    float b2 = (float) data.heightData[index1 - 1 + stride] * 0.01f;
                    float b3 = (float) data.heightData[index1 + 1 - stride] * 0.01f;
                    float b4 = (float) data.heightData[index1 - 1 - stride] * 0.01f;
                    float num17 = (float) data.biomoData[index1] * 0.01f;
                    bool flag = true;
                    if ((double) diff(a, b1) > 0.200000002980232)
                        flag = false;
                    if ((double) diff(a, b2) > 0.200000002980232)
                        flag = false;
                    if ((double) diff(a, b3) > 0.200000002980232)
                        flag = false;
                    if ((double) diff(a, b4) > 0.200000002980232)
                        flag = false;
                    double num18 = random2.NextDouble();
                    double num19 = num18 * num18;
                    double num20 = random2.NextDouble();
                    float num21 = (float) random2.NextDouble() - 0.5f;
                    float num22 = (float) random2.NextDouble() - 0.5f;
                    float num23 = (float) Math.Sqrt(random2.NextDouble());
                    float angle1 = (float) random2.NextDouble() * 360f;
                    float num24 = (float) random2.NextDouble();
                    float num25 = (float) random2.NextDouble();
                    int[] numArray1;
                    float num26;
                    float num27;
                    float num28;
                    if ((double) num17 < 0.800000011920929) {
                        numArray1 = vegetables0;
                        num26 = num1;
                        num27 = num2;
                        num28 = num3;
                    }
                    else if ((double) num17 < 2.0) {
                        numArray1 = vegetables1;
                        num26 = num4;
                        num27 = num5;
                        num28 = num6;
                    }
                    else {
                        numArray1 = vegetables5;
                        num26 = num4;
                        num27 = num5;
                        num28 = num6;
                    }

                    double num29 = simplexNoise1.Noise(num14 * 0.07, num15 * 0.07, num16 * 0.07) * (double) num26 +
                                   (double) num27 + 0.5;
                    double num30 = simplexNoise2.Noise(num14 * 0.4, num15 * 0.4, num16 * 0.4) * (double) num7 +
                                   (double) num8 + 0.5;
                    double num31 = num30 - 0.55;
                    int[] numArray2;
                    double num32;
                    int num33;
                    if ((double) num17 > 1.0) {
                        numArray2 = vegetables2;
                        num32 = num30;
                        num33 = 4;
                    }
                    else if ((double) num17 > 0.5) {
                        numArray2 = vegetables3;
                        num32 = num31;
                        num33 = 1;
                    }
                    else if ((double) num17 > 0.0) {
                        numArray2 = vegetables4;
                        num32 = num31;
                        num33 = 1;
                    }
                    else {
                        numArray2 = (int[]) null;
                        num32 = num30;
                        num33 = 1;
                    }

                    if (flag && num20 < num29 && (numArray1 != null && numArray1.Length > 0)) {
                        vege.protoId = (short) numArray1[(int) (num19 * (double) numArray1.Length)];
                        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, vertex);
                        Vector3 vector3_1 = rotation * Vector3.forward;
                        Vector3 vector3_2 = rotation * Vector3.right;
                        Vector4 vector4 = vegeScaleRanges[(int) vege.protoId];
                        Vector3 vector3_3 = vertex * a;
                        Vector3 vector3_4 = (vector3_2 * num21 + vector3_1 * num22).normalized *
                                            (num23 * num28 * num11);
                        float y = (float) ((double) num25 * ((double) vector4.x + (double) vector4.y) +
                                           (1.0 - (double) vector4.x));
                        float num34 = (float) ((double) num24 * ((double) vector4.z + (double) vector4.w) +
                                               (1.0 - (double) vector4.z)) * y;
                        vege.pos = (vector3_3 + vector3_4).normalized;
                        a = data.QueryHeight(vege.pos);
                        vege.pos *= a *  ___planet.GetScaleFactored();
                        vege.rot = Quaternion.FromToRotation(Vector3.up, vege.pos.normalized) *
                                   Quaternion.AngleAxis(angle1, Vector3.up);
                        vege.scl = new Vector3(num34, y, num34);
                        vege.modelIndex = (short) vegeProtos[(int) vege.protoId].ModelIndex;
                        vege.hp = vegeHps[(int) vege.protoId];
                        int num35 = data.AddVegeData(vege);
                        data.vegeIds[index1] = (ushort) num35;
                    }

                    if (num20 < num32 && numArray2 != null && numArray2.Length > 0) {
                        vege.protoId = (short) numArray2[(int) (num19 * (double) numArray2.Length)];
                        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, vertex);
                        Vector3 vector3_1 = rotation * Vector3.forward;
                        Vector3 vector3_2 = rotation * Vector3.right;
                        Vector4 vector4 = vegeScaleRanges[(int) vege.protoId];
                        for (int index2 = 0; index2 < num33; ++index2) {
                            float num34 = (float) random2.NextDouble() - 0.5f;
                            float num35 = (float) random2.NextDouble() - 0.5f;
                            float num36 = (float) Math.Sqrt(random2.NextDouble());
                            float angle2 = (float) random2.NextDouble() * 360f;
                            float num37 = (float) random2.NextDouble();
                            float num38 = (float) random2.NextDouble();
                            Vector3 vector3_3 = vertex * a;
                            Vector3 vector3_4 = (vector3_2 * num34 + vector3_1 * num35).normalized *
                                                (num36 * num9 * num11);
                            float y = (float) ((double) num38 * ((double) vector4.x + (double) vector4.y) +
                                               (1.0 - (double) vector4.x));
                            float num39 = (float) ((double) num37 * ((double) vector4.z + (double) vector4.w) +
                                                   (1.0 - (double) vector4.z)) * y;
                            vege.pos = (vector3_3 + vector3_4).normalized;
                            a = data.QueryHeight(vege.pos);
                            vege.pos *= a *  ___planet.GetScaleFactored();
                            vege.rot = Quaternion.FromToRotation(Vector3.up, vege.pos.normalized) *
                                       Quaternion.AngleAxis(angle2, Vector3.up);
                            vege.scl = new Vector3(num39, y, num39);
                            vege.modelIndex = (short) vegeProtos[(int) vege.protoId].ModelIndex;
                            vege.hp = (short) 1;
                            int num40 = data.AddVegeData(vege);
                            data.vegeIds[index1] = (ushort) num40;
                        }
                    }
                }
            }

            return false;
        }

        private static float diff(float a, float b) => (double) a > (double) b ? a - b : b - a;
    }
}