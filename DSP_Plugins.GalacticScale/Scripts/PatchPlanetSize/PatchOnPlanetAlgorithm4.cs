using System;
using BepInEx.Logging;
using GalacticScale.Scripts.PatchStarSystemGeneration;
using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
using Random = System.Random;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetAlgorithm4))]
    public class PatchOnPlanetAlgorithm4 {
        [HarmonyPrefix]
        [HarmonyPatch("GenerateVegetables")]
        public static bool PatchGenerateVegetables(ref PlanetData ___planet) {
            Patch.Debug("GenerateVegetables 4:  " + ___planet.radius + " for : " + ___planet.name,
                LogLevel.Debug, Patch.DebugPlanetAlgorithm4);
            ThemeProto themeProto = LDB.themes.Select(___planet.theme);
            if (themeProto == null)
                return false;
            int[] vegetables0 = themeProto.Vegetables0;
            int[] vegetables1 = themeProto.Vegetables1;
            double num1 = 0.005;
            double num2 = 0.02;
            double num3 = 0.005;
            float num4 = 0.18f;
            float num5 = -0.45f;
            float num6 = 2.5f;
            float num7 = 0.25f;
            float num8 = -0.45f;
            float num9 = 1f;
            Random random1 = new Random(___planet.seed);
            random1.Next();
            random1.Next();
            random1.Next();
            Random random2 = new Random(random1.Next());
            SimplexNoise simplexNoise = new SimplexNoise(random2.Next());
            PlanetRawData data = ___planet.data;
            int stride = data.stride;
            int num10 = stride / 2;
            float num11 =
                (float) (___planet.radius * 3.1415901184082 * 2.0 / (data.precision * 4.0));
            VegeData vege = new VegeData();
            VegeProto[] vegeProtos = PlanetModelingManager.vegeProtos;
            Vector4[] vegeScaleRanges = PlanetModelingManager.vegeScaleRanges;
            short[] vegeHps = PlanetModelingManager.vegeHps;
            for (int index = 0; index < data.dataLength; ++index) {
                int num12 = index % stride;
                int num13 = index / stride;
                if (num12 > num10)
                    --num12;
                if (num13 > num10)
                    --num13;
                if (num12 % 2 == 1 && num13 % 2 == 1) {
                    Vector3 vertex = data.vertices[index];
                    double num14 = data.vertices[index].x * (double) ___planet.radius;
                    double num15 = data.vertices[index].y * (double) ___planet.radius;
                    double num16 = data.vertices[index].z * (double) ___planet.radius;
                    float num17 = data.heightData[index] * 0.01f;
                    float num18 = data.biomoData[index] * 0.01f;
                    double num19 = random2.NextDouble();
                    double num20 = num19 * num19;
                    double num21 = random2.NextDouble();
                    float num22 = (float) random2.NextDouble() - 0.5f;
                    float num23 = (float) random2.NextDouble() - 0.5f;
                    float num24 = (float) Math.Sqrt(random2.NextDouble());
                    float angle = (float) random2.NextDouble() * 360f;
                    float num25 = (float) random2.NextDouble();
                    float num26 = (float) random2.NextDouble();
                    int[] numArray;
                    float num27;
                    float num28;
                    float num29;
                    if (num18 < 0.800000011920929) {
                        numArray = vegetables0;
                        num27 = num4;
                        num28 = num5;
                        num29 = num6;
                    }
                    else {
                        numArray = vegetables1;
                        num27 = num7;
                        num28 = num8;
                        num29 = num9;
                    }

                    double num30 =
                        simplexNoise.Noise3DFBM(num14 * num1, num15 * num2, num16 * num3, 2) * num27 +
                        num28 + 0.5;
                    if (num21 <= num30 && numArray != null && numArray.Length > 0) {
                        vege.protoId = (short) numArray[(int) (num20 * numArray.Length)];
                        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, vertex);
                        Vector3 vector3_1 = rotation * Vector3.forward;
                        Vector3 vector3_2 = rotation * Vector3.right;
                        Vector3 vector3_3 = vertex * num17;
                        Vector3 vector3_4 = (vector3_2 * num22 + vector3_1 * num23).normalized *
                                            (num24 * num29 * num11);
                        Vector4 vector4 = vegeScaleRanges[vege.protoId];
                        float y = (float) (num26 * (vector4.x + (double) vector4.y) +
                                           (1.0 - vector4.x));
                        float num31 = (float) (num25 * (vector4.z + (double) vector4.w) +
                                               (1.0 - vector4.z)) * y;
                        vege.pos = (vector3_3 + vector3_4).normalized;
                        float num32 = data.QueryHeight(vege.pos);
                        vege.pos *= num32 *  ___planet.GetScaleFactored();
                        vege.rot = Quaternion.FromToRotation(Vector3.up, vege.pos.normalized) *
                                   Quaternion.AngleAxis(angle, Vector3.up);
                        vege.scl = new Vector3(num31, y, num31);
                        vege.modelIndex = (short) vegeProtos[vege.protoId].ModelIndex;
                        vege.hp = vegeHps[vege.protoId];
                        int num33 = data.AddVegeData(vege);
                        data.vegeIds[index] = (ushort) num33;
                    }
                }
            }

            return false;
        }

        private static float diff(float a, float b) => (double) a > (double) b ? a - b : b - a;
    }
}