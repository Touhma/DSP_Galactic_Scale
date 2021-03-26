
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
using System;
using UnityEngine;
using System.Threading;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetAlgorithm))]
    static class PatchPlanetAlgorithm
    {
        [HarmonyPrefix]
        [HarmonyPatch("GenerateVeins")]
        public static bool GenerateVeins(PlanetAlgorithm __instance, bool sketchOnly, ref PlanetData ___planet, ref Vector3[] ___veinVectors, ref EVeinType[] ___veinVectorTypes, ref int ___veinVectorCount, ref List<Vector2> ___tmp_vecs)
        {
            //lock ((object)___planet)
            //{
                ThemeProto themeProto = LDB.themes.Select(___planet.theme);
                if (themeProto == null)
                    return false;
                System.Random random1 = new System.Random(___planet.seed);
                random1.Next();
                random1.Next();
                random1.Next();
                random1.Next();
                int _birthSeed = random1.Next();
                System.Random random2 = new System.Random(random1.Next());
                PlanetRawData data = ___planet.data;
                float num1 = 2.1f / ___planet.radius;
                VeinProto[] veinProtos = PlanetModelingManager.veinProtos;
                int[] veinModelIndexs = PlanetModelingManager.veinModelIndexs;
                int[] veinModelCounts = PlanetModelingManager.veinModelCounts;
                int[] veinProducts = PlanetModelingManager.veinProducts;
                int[] numArray1 = new int[veinProtos.Length];
                float[] numArray2 = new float[veinProtos.Length];
                float[] numArray3 = new float[veinProtos.Length];
                if (themeProto.VeinSpot != null)
                    Array.Copy((Array)themeProto.VeinSpot, 0, (Array)numArray1, 1, Math.Min(themeProto.VeinSpot.Length, numArray1.Length - 1));
                if (themeProto.VeinCount != null)
                    Array.Copy((Array)themeProto.VeinCount, 0, (Array)numArray2, 1, Math.Min(themeProto.VeinCount.Length, numArray2.Length - 1));
                if (themeProto.VeinOpacity != null)
                    Array.Copy((Array)themeProto.VeinOpacity, 0, (Array)numArray3, 1, Math.Min(themeProto.VeinOpacity.Length, numArray3.Length - 1));
                float p = 1f;
                ESpectrType spectr = ___planet.star.spectr;
                switch (___planet.star.type)
                {
                    case EStarType.MainSeqStar:
                        switch (spectr)
                        {
                            case ESpectrType.M:
                                p = 2.5f;
                                break;
                            case ESpectrType.K:
                                p = 1f;
                                break;
                            case ESpectrType.G:
                                p = 0.7f;
                                break;
                            case ESpectrType.F:
                                p = 0.6f;
                                break;
                            case ESpectrType.A:
                                p = 1f;
                                break;
                            case ESpectrType.B:
                                p = 0.4f;
                                break;
                            case ESpectrType.O:
                                p = 1.6f;
                                break;
                        }
                        break;
                    case EStarType.GiantStar:
                        p = 2.5f;
                        break;
                    case EStarType.WhiteDwarf:
                        p = 3.5f;
                        ++numArray1[9];
                        ++numArray1[9];
                        for (int index = 1; index < 12 && random1.NextDouble() < 0.449999988079071; ++index)
                            ++numArray1[9];
                        numArray2[9] = 0.7f;
                        numArray3[9] = 1f;
                        ++numArray1[10];
                        ++numArray1[10];
                        for (int index = 1; index < 12 && random1.NextDouble() < 0.449999988079071; ++index)
                            ++numArray1[10];
                        numArray2[10] = 0.7f;
                        numArray3[10] = 1f;
                        ++numArray1[12];
                        for (int index = 1; index < 12 && random1.NextDouble() < 0.5; ++index)
                            ++numArray1[12];
                        numArray2[12] = 0.7f;
                        numArray3[12] = 0.3f;
                        break;
                    case EStarType.NeutronStar:
                        p = 4.5f;
                        ++numArray1[14];
                        for (int index = 1; index < 12 && random1.NextDouble() < 0.649999976158142; ++index)
                            ++numArray1[14];
                        numArray2[14] = 0.7f;
                        numArray3[14] = 0.3f;
                        break;
                    case EStarType.BlackHole:
                        p = 5f;
                        ++numArray1[14];
                        for (int index = 1; index < 12 && random1.NextDouble() < 0.649999976158142; ++index)
                            ++numArray1[14];
                        numArray2[14] = 0.7f;
                        numArray3[14] = 0.3f;
                        break;
                }
                for (int index1 = 0; index1 < themeProto.RareVeins.Length; ++index1)
                {
                    int rareVein = themeProto.RareVeins[index1];
                    float num2 = ___planet.star.index != 0 ? themeProto.RareSettings[index1 * 4 + 1] : themeProto.RareSettings[index1 * 4];
                    float rareSetting1 = themeProto.RareSettings[index1 * 4 + 2];
                    float rareSetting2 = themeProto.RareSettings[index1 * 4 + 3];
                    float num3 = rareSetting2;
                    float num4 = 1f - Mathf.Pow(1f - num2, p);
                    float num5 = 1f - Mathf.Pow(1f - rareSetting2, p);
                    float num6 = 1f - Mathf.Pow(1f - num3, p);
                    if (random1.NextDouble() < (double)num4)
                    {
                        ++numArray1[rareVein];
                        numArray2[rareVein] = num5;
                        numArray3[rareVein] = num5;
                        for (int index2 = 1; index2 < 12 && random1.NextDouble() < (double)rareSetting1; ++index2)
                            ++numArray1[rareVein];
                    }
                }
                bool flag1 = ___planet.galaxy.birthPlanetId == ___planet.id;
                if (flag1 && !sketchOnly)
                    ___planet.GenBirthPoints(data, _birthSeed);
                float resourceCoef = ___planet.star.resourceCoef;
                bool flag2 = (double)DSPGame.GameDesc.resourceMultiplier >= 99.5;
                if (flag1)
                    resourceCoef *= 0.6666667f;
                float num7 = 1f * 1.1f;
                Array.Clear((Array)___veinVectors, 0, ___veinVectors.Length);
                Array.Clear((Array)___veinVectorTypes, 0, ___veinVectorTypes.Length);
                ___veinVectorCount = 0;
                Vector3 vector3_1;
                if (flag1)
                {
                    vector3_1 = ___planet.birthPoint;
                    vector3_1.Normalize();
                    vector3_1 *= 0.75f;
                }
                else
                {
                    Vector3 vector3_2;
                    vector3_2.x = (float)(random2.NextDouble() * 2.0 - 1.0);
                    vector3_2.y = (float)random2.NextDouble() - 0.5f;
                    vector3_2.z = (float)(random2.NextDouble() * 2.0 - 1.0);
                    vector3_2.Normalize();
                    vector3_1 = vector3_2 * (float)(random2.NextDouble() * 0.4 + 0.2);
                }
                ___planet.veinSpotsSketch = numArray1;
                if (sketchOnly)
                    return false;
                if (flag1)
                {
                    ___veinVectorTypes[0] = EVeinType.Iron;
                    ___veinVectors[0] = ___planet.birthResourcePoint0;
                    ___veinVectorTypes[1] = EVeinType.Copper;
                    ___veinVectors[1] = ___planet.birthResourcePoint1;
                    ___veinVectorCount = 2;
                }
                for (int index1 = 1; index1 < 15 && ___veinVectorCount < ___veinVectors.Length; ++index1)
                {
                    EVeinType eveinType = (EVeinType)index1;
                    int num2 = numArray1[index1];
                    if (num2 > 1)
                        num2 += random2.Next(-1, 2);
                    for (int index2 = 0; index2 < num2; ++index2)
                    {
                        int num3 = 0;
                        Vector3 zero = Vector3.zero;
                        bool flag3 = false;
                        while (num3++ < ___planet.radius)
                        {
                            zero.x = (float)(random2.NextDouble() * 2.0 - 1.0);
                            zero.y = (float)(random2.NextDouble() * 2.0 - 1.0);
                            zero.z = (float)(random2.NextDouble() * 2.0 - 1.0);
                            if (eveinType != EVeinType.Oil)
                                zero += vector3_1;
                            zero.Normalize();
                            float num4 = data.QueryHeight(zero);
                        Patch.Log("num4 = " + num4);
                            if ((double)num4 >= (double)___planet.radius && (eveinType != EVeinType.Oil || (double)num4 >= (double)___planet.radius + 0.5))
                            {
                                bool flag4 = false;
                                float num5 = eveinType != EVeinType.Oil ? 196f : 100f;
                                for (int index3 = 0; index3 < ___veinVectorCount; ++index3)
                                {
                                    if ((double)(___veinVectors[index3] - zero).sqrMagnitude < (double)num1 * (double)num1 * (double)num5)
                                    {
                                        flag4 = true;
                                    Patch.Log("flag4 = " + flag4.ToString());
                                        break;
                                    }
                                }
                                if (!flag4)
                                {
                                    flag3 = true;
                                Patch.Log("flag3 = " + flag3.ToString());
                                break;
                                }
                            }
                        }
                        if (flag3)
                        {
                            ___veinVectors[___veinVectorCount] = zero;
                            ___veinVectorTypes[___veinVectorCount] = eveinType;
                            ++___veinVectorCount;
                            if (___veinVectorCount == ___veinVectors.Length)
                                break;
                        }
                    }
                }
                Array.Clear((Array)___planet.veinAmounts, 0, ___planet.veinAmounts.Length);
                data.veinCursor = 1;
                ___planet.veinGroups = new PlanetData.VeinGroup[___veinVectorCount];
                ___tmp_vecs.Clear();
                VeinData vein = new VeinData();
                for (int index1 = 0; index1 < ___veinVectorCount; ++index1)
                {
                    ___tmp_vecs.Clear();
                    Vector3 normalized = ___veinVectors[index1].normalized;
                    EVeinType veinVectorType = ___veinVectorTypes[index1];
                    int index2 = (int)veinVectorType;
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normalized);
                    Vector3 vector3_2 = rotation * Vector3.right;
                    Vector3 vector3_3 = rotation * Vector3.forward;
                    ___planet.veinGroups[index1].type = veinVectorType;
                    ___planet.veinGroups[index1].pos = normalized;
                    ___planet.veinGroups[index1].count = 0;
                    ___planet.veinGroups[index1].amount = 0L;
                    ___tmp_vecs.Add(Vector2.zero);
                    int num2 = Mathf.RoundToInt(numArray2[index2] * (float)random2.Next(20, 25));
                    if (veinVectorType == EVeinType.Oil)
                        num2 = 1;
                    float num3 = numArray3[index2];
                    if (flag1 && index1 < 2)
                    {
                        num2 = 6;
                        num3 = 0.2f;
                    }
                    int num4 = 0;
                    while (num4++ < 20)
                    {
                        int count = ___tmp_vecs.Count;
                        for (int index3 = 0; index3 < count && ___tmp_vecs.Count < num2; ++index3)
                        {
                            if ((double)___tmp_vecs[index3].sqrMagnitude <= 36.0)
                            {
                                double num5 = random2.NextDouble() * Math.PI * 2.0;
                                Vector2 vector2_1 = new Vector2((float)Math.Cos(num5), (float)Math.Sin(num5));
                                vector2_1 += ___tmp_vecs[index3] * 0.2f;
                                vector2_1.Normalize();
                                Vector2 vector2_2 = ___tmp_vecs[index3] + vector2_1;
                                bool flag3 = false;
                                for (int index4 = 0; index4 < ___tmp_vecs.Count; ++index4)
                                {
                                    if ((double)(___tmp_vecs[index4] - vector2_2).sqrMagnitude < 0.850000023841858)
                                    {
                                        flag3 = true;
                                        break;
                                    }
                                }
                                if (!flag3)
                                    ___tmp_vecs.Add(vector2_2);
                            }
                        }
                        if (___tmp_vecs.Count >= num2)
                            break;
                    }
                    int num6 = Mathf.RoundToInt(num3 * 100000f * resourceCoef);
                    if (num6 < 20)
                        num6 = 20;
                    int num8 = num6 >= 16000 ? 15000 : Mathf.FloorToInt((float)num6 * (15f / 16f));
                    int minValue = num6 - num8;
                    int maxValue = num6 + num8 + 1;
                    for (int index3 = 0; index3 < ___tmp_vecs.Count; ++index3)
                    {
                        Vector3 vector3_4 = (___tmp_vecs[index3].x * vector3_2 + ___tmp_vecs[index3].y * vector3_3) * num1;
                        vein.type = veinVectorType;
                        vein.groupIndex = (short)index1;
                        vein.modelIndex = (short)random2.Next(veinModelIndexs[index2], veinModelIndexs[index2] + veinModelCounts[index2]);
                        vein.amount = Mathf.RoundToInt((float)random2.Next(minValue, maxValue) * num7);
                        if (___planet.veinGroups[index1].type != EVeinType.Oil)
                            vein.amount = Mathf.RoundToInt((float)vein.amount * DSPGame.GameDesc.resourceMultiplier);
                        if (vein.amount < 1)
                            vein.amount = 1;
                        if (flag2 && vein.type != EVeinType.Oil)
                            vein.amount = 1000000000;
                        vein.productId = veinProducts[index2];
                        vein.pos = normalized + vector3_4;
                        if (vein.type == EVeinType.Oil)
                            vein.pos = ___planet.aux.RawSnap(vein.pos);
                        vein.minerCount = 0;
                        float num5 = data.QueryHeight(vein.pos);
                        data.EraseVegetableAtPoint(vein.pos);
                        vein.pos = vein.pos.normalized * num5;
                        if (___planet.waterItemId == 0 || (double)num5 >= (double)___planet.radius)
                        {
                            ___planet.veinAmounts[(int)veinVectorType] += (long)vein.amount;
                            ++___planet.veinGroups[index1].count;
                            ___planet.veinGroups[index1].amount += (long)vein.amount;
                            data.AddVeinData(vein);
                        }
                    }
                }
                ___tmp_vecs.Clear();
            //}
            foreach (Vector3 v in ___veinVectors)
            {
                Patch.Log("vv" + v);
            }
            foreach (PlanetData.VeinGroup v in ___planet.veinGroups)
            {
                Patch.Log("vv" + v.pos);
            }
            return false;
        }
    }

}