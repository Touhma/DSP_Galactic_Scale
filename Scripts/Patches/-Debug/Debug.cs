using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection.Emit;
using System.Linq;
using HarmonyLib;
using static GalacticScale.GS2;
using UnityEngine;
using UnityEngine.UI;
using static System.Reflection.Emit.OpCodes;
using Logger = BepInEx.Logging.Logger;

// using BCE;
namespace GalacticScale
{
    public static class PatchOnUnspecified_Debug
    {

        [HarmonyPrefix, HarmonyPatch(typeof(PlanetATField), "UpdateGeneratorMatrix")]
        public static bool PlanetATField_UpdateGeneratorMatrix_Prefix(ref PlanetATField __instance)
        {



            // Calculate total generators needed first
            double shieldRadius = 80.0;
            double realRadius = __instance.planet.realRadius;
            int totalGenerators = 1; // First generator at pole

            int rCount = (int)Math.Ceiling(Math.PI * realRadius / (2.0 * shieldRadius));
            totalGenerators += rCount;

            // Calculate generators along each latitude ring
            for (int i = 1; i <= rCount; i++)
            {
                double sita = (double)i * Math.PI / (double)rCount;
                double r2 = realRadius * Math.Sin(sita);
                int r2Count = (int)Math.Ceiling(Math.PI * r2 * 2.0 / (2.0 * shieldRadius));
                totalGenerators += r2Count - 1;
            }

            totalGenerators += 1;

            //Original Code
            DataPool<FieldGeneratorComponent> fieldGenerators = __instance.defense.fieldGenerators;
            int count = fieldGenerators.count;
            __instance.generatorCount = 0;
            if (__instance.generatorMatrix == null || __instance.generatorMatrix.Length != totalGenerators)
            {
                __instance.generatorMatrix = new Vector4[totalGenerators];
            }
            else if (__instance.generatorMatrix.Length == totalGenerators)
            {
                Array.Clear(__instance.generatorMatrix, 0, __instance.generatorMatrix.Length);
            }
            if (count > 0)
            {
                if (__instance.generatorMatrixPresent == null)
                {
                    __instance.generatorMatrixPresent = new Vector4[totalGenerators];
                }
                FieldGeneratorComponent[] buffer = fieldGenerators.buffer;
                int cursor = fieldGenerators.cursor;
                for (int i = 1; i < cursor; i++)
                {
                    if (buffer[i].id == i)
                    {
                        __instance.generatorMatrix[__instance.generatorCount] = buffer[i].holder;
                        __instance.generatorCount++;
                        if (__instance.generatorCount == totalGenerators)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                return false;
            }
            // lltcggie.DSP.plugin.PlanetwidePlanetaryShieldGenerator , modified
            
            
            
            // 惑星内の惑星シールドジェネレータで最も高いサポート率をサポート率として採用
            float maxW = 0.0f;
            for (int i = 0; i < count; i++)
            {
                maxW = Math.Max(__instance.generatorMatrix[i].w, maxW);
            }

            __instance.generatorCount = 0;



            // 緯度方向に等間隔に配置する
            Vector4 vec;
            vec.x = 0.0f;
            vec.y = (float)realRadius;
            vec.z = 0.0f;
            vec.w = maxW;
            __instance.generatorMatrix[__instance.generatorCount] = vec;
            __instance.generatorCount++;
            for (int i = 1;
                 i <= rCount && __instance.generatorCount < totalGenerators;
                 i++)
            {
                double sita = (double)i * Math.PI / (double)rCount;
                vec.x = 0.0f;
                vec.y = (float)(realRadius * Math.Cos(sita));
                vec.z = (float)(realRadius * Math.Sin(sita));
                vec.w = maxW;
                __instance.generatorMatrix[__instance.generatorCount] = vec;
                __instance.generatorCount++;

                // 緯度方向に配置したのを原点として経度方向に等間隔に配置する
                double r2 = realRadius * Math.Sin(sita);
                int r2Count = (int)Math.Ceiling((Math.PI * r2 * 2.0) / (2.0 * shieldRadius));
                for (int j = 1; j < r2Count && __instance.generatorCount < totalGenerators; j++)
                {
                    double sita2 = (double)j * 2.0 * Math.PI / (double)r2Count;
                    vec.x = (float)(r2 * Math.Sin(sita2));
                    vec.z = (float)(r2 * Math.Cos(sita2));
                    __instance.generatorMatrix[__instance.generatorCount] = vec;
                    __instance.generatorCount++;
                }
            }

            return false;
        }


        [HarmonyPrefix, HarmonyPatch(typeof(DysonSphere), nameof(DysonSphere.RocketGameTick), new[]
        {
            typeof(int), typeof(int), typeof(int)
        })]
        public static bool RocketGameTick(ref DysonSphere __instance, int _usedThreadCnt, int _curThreadIdx, int _minimumMissionCnt)
        {
            AstroData[] astrosData = __instance.starData.galaxy.astrosData;
            double timefactor = 0.016666666666666666;
            float orbitRadiusFactor = Mathf.Max(1f, (float)Math.Pow((double)__instance.defOrbitRadius / 40000.0 * 4.0, 0.4));
            float constantValue = 7.5f;
            float scaleOrbitRadius1 = 18f * orbitRadiusFactor;
            float scaleOrbitRadius2 = 280f * orbitRadiusFactor;
            long gameTick = GameMain.gameTick;
            int startindex;
            int endIndex;
            if (!WorkerThreadExecutor.CalculateMissionIndex(1, __instance.rocketCursor - 1, _usedThreadCnt, _curThreadIdx, _minimumMissionCnt, out startindex, out endIndex))
            {
                return false;
            }

            VectorLF3 starUPosition = __instance.starData.uPosition;
            for (int i = startindex; i < endIndex; i++)
            {
                if (__instance.rocketPool[i].id == i)
                {
                    ref DysonRocket ptr = ref __instance.rocketPool[i];
                    if (ptr.node == null)
                    {
                        __instance.RemoveDysonRocket(i);
                    }
                    else
                    {
                        DysonSphereLayer dysonSphereLayer = __instance.layersIdBased[ptr.node.layerId];
                        ref AstroData ptr2 = ref astrosData[ptr.planetId];
                        VectorLF3 vectorLF;
                        var radius = ptr2.uRadius;
                        vectorLF.x = ptr.uPos.x - ptr2.uPos.x;
                        vectorLF.y = ptr.uPos.y - ptr2.uPos.y;
                        vectorLF.z = ptr.uPos.z - ptr2.uPos.z;
                        double distanceFromPlanetSurface = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)ptr2.uRadius;
                        if (ptr.t <= 0f)
                        {
                            if (distanceFromPlanetSurface < radius)
                            {
                                float num9 = (float)distanceFromPlanetSurface / radius;
                                if (num9 < 0f)
                                {
                                    num9 = 0f;
                                }

                                float num10 = num9 * num9 * 600f + 15f;
                                ptr.uSpeed = ptr.uSpeed * 0.9f + num10 * 0.1f;
                                ptr.t = (num9 - 1f) * 1.2f;
                                if (ptr.t < -1f)
                                {
                                    ptr.t = -1f;
                                }
                            }
                            else
                            {
                                VectorLF3 vectorLF2;
                                dysonSphereLayer.NodeEnterUPos(ptr.node, out vectorLF2);
                                VectorLF3 vectorLF3;
                                vectorLF3.x = vectorLF2.x - ptr.uPos.x;
                                vectorLF3.y = vectorLF2.y - ptr.uPos.y;
                                vectorLF3.z = vectorLF2.z - ptr.uPos.z;
                                double num11 = Math.Sqrt(vectorLF3.x * vectorLF3.x + vectorLF3.y * vectorLF3.y + vectorLF3.z * vectorLF3.z);
                                if (num11 < 50.0)
                                {
                                    ptr.t = 0.0001f;
                                }
                                else
                                {
                                    ptr.t = 0f;
                                }

                                double num12 = num11 / ((double)ptr.uSpeed + 0.1) * 0.382;
                                double num13 = num11 / (double)scaleOrbitRadius2;
                                float num14 = (float)((double)ptr.uSpeed * num12) + 150f;
                                if (num14 > scaleOrbitRadius2)
                                {
                                    num14 = scaleOrbitRadius2;
                                }

                                if (ptr.uSpeed < num14 - constantValue)
                                {
                                    ptr.uSpeed += constantValue;
                                }
                                else if (ptr.uSpeed > num14 + scaleOrbitRadius1)
                                {
                                    ptr.uSpeed -= scaleOrbitRadius1;
                                }
                                else
                                {
                                    ptr.uSpeed = num14;
                                }

                                int num15 = -1;
                                double rhs = 0.0;
                                double num16 = 1E+40;
                                int starID = ptr.planetId / 100 * 100;
                                for (int j = starID; j < starID + 99; j++) // Added a 99 here
                                {
                                    float uRadius = astrosData[j].uRadius;
                                    if (uRadius >= 1f && j != ptr.planetId)
                                    {
                                        float num18 = (j == starID) ? (dysonSphereLayer.orbitRadius + 8000f) : (uRadius + 6500f);
                                        VectorLF3 vectorLF4;
                                        vectorLF4.x = ptr.uPos.x - astrosData[j].uPos.x;
                                        vectorLF4.y = ptr.uPos.y - astrosData[j].uPos.y;
                                        vectorLF4.z = ptr.uPos.z - astrosData[j].uPos.z;
                                        double num19 = vectorLF4.x * vectorLF4.x + vectorLF4.y * vectorLF4.y + vectorLF4.z * vectorLF4.z;
                                        double num20 = -((double)ptr.uVel.x * vectorLF4.x + (double)ptr.uVel.y * vectorLF4.y + (double)ptr.uVel.z * vectorLF4.z);
                                        if ((num20 > 0.0 || num19 < (double)(uRadius * uRadius * 7f)) && num19 < num16 && num19 < (double)(num18 * num18))
                                        {
                                            rhs = ((num20 < 0.0) ? 0.0 : num20);
                                            num15 = j;
                                            num16 = num19;
                                        }
                                    }
                                }

                                VectorLF3 rhs2 = VectorLF3.zero;
                                float num21 = 0f;
                                if (num15 > 0)
                                {
                                    float num22 = astrosData[num15].uRadius;
                                    bool flag = num15 % 100 == 0;
                                    if (flag)
                                    {
                                        num22 = dysonSphereLayer.orbitRadius - 400f;
                                    }

                                    double num23 = 1.25;
                                    VectorLF3 vectorLF5 = (VectorLF3)ptr.uPos + (VectorLF3)ptr.uVel * rhs - astrosData[num15].uPos;
                                    double num24 = vectorLF5.magnitude / (double)num22;
                                    if (num24 < num23)
                                    {
                                        double num25 = Math.Sqrt(num16) - (double)num22 * 0.82;
                                        if (num25 < 1.0)
                                        {
                                            num25 = 1.0;
                                        }

                                        double num26 = (num24 - 1.0) / (num23 - 1.0);
                                        if (num26 < 0.0)
                                        {
                                            num26 = 0.0;
                                        }

                                        num26 = 1.0 - num26 * num26;
                                        double num27 = (double)(ptr.uSpeed - 6f) / num25 * 2.5 - 0.01;
                                        if (num27 > 1.5)
                                        {
                                            num27 = 1.5;
                                        }
                                        else if (num27 < 0.0)
                                        {
                                            num27 = 0.0;
                                        }

                                        num27 = num27 * num27 * num26;
                                        num21 = (float)(flag ? 0.0 : (num27 * 0.5));
                                        rhs2 = vectorLF5.normalized * num27 * 2.0;
                                    }
                                }

                                int num28 = (num15 > 0 || distanceFromPlanetSurface < 2000.0 || num11 < 2000.0) ? 1 : 6;
                                if (num28 == 1 || (gameTick + (long)i) % (long)num28 == 0L)
                                {
                                    float num29 = 1f / (float)num13 - 0.05f;
                                    num29 += num21;
                                    float t = Mathf.Lerp(0.005f, 0.08f, num29) * (float)num28;
                                    ptr.uVel = Vector3.Slerp(ptr.uVel, vectorLF3.normalized + rhs2, t).normalized;
                                    Quaternion b;
                                    if (num11 < 350.0)
                                    {
                                        float t2 = ((float)num11 - 50f) / 300f;
                                        b = Quaternion.Slerp(dysonSphereLayer.NodeURot(ptr.node), Quaternion.LookRotation(ptr.uVel), t2);
                                    }
                                    else
                                    {
                                        b = Quaternion.LookRotation(ptr.uVel);
                                    }

                                    ptr.uRot = Quaternion.Slerp(ptr.uRot, b, 0.2f);
                                }
                            }
                        }
                        else
                        {
                            VectorLF3 vectorLF6;
                            dysonSphereLayer.NodeSlotUPos(ptr.node, out vectorLF6);
                            VectorLF3 vectorLF7;
                            vectorLF7.x = vectorLF6.x - ptr.uPos.x;
                            vectorLF7.y = vectorLF6.y - ptr.uPos.y;
                            vectorLF7.z = vectorLF6.z - ptr.uPos.z;
                            double num30 = Math.Sqrt(vectorLF7.x * vectorLF7.x + vectorLF7.y * vectorLF7.y + vectorLF7.z * vectorLF7.z);
                            if (num30 < 2.0)
                            {
                                __instance.ConstructSp(ptr.node);
                                __instance.RemoveDysonRocket(i);
                                goto IL_E26;
                            }

                            float num31 = (float)(num30 * 0.75 + 15.0);
                            if (num31 > scaleOrbitRadius2)
                            {
                                num31 = scaleOrbitRadius2;
                            }

                            if (ptr.uSpeed < num31 - constantValue)
                            {
                                ptr.uSpeed += constantValue;
                            }
                            else if (ptr.uSpeed > num31 + scaleOrbitRadius1)
                            {
                                ptr.uSpeed -= scaleOrbitRadius1;
                            }
                            else
                            {
                                ptr.uSpeed = num31;
                            }

                            if ((gameTick + (long)i) % 2L == 0L)
                            {
                                ptr.uVel = Vector3.Slerp(ptr.uVel, vectorLF7.normalized, 0.15f);
                                ptr.uRot = Quaternion.Slerp(ptr.uRot, dysonSphereLayer.NodeURot(ptr.node), 0.2f);
                            }

                            ptr.t = (350f - (float)num30) / 330f;
                            if (ptr.t > 1f)
                            {
                                ptr.t = 1f;
                            }
                            else if (ptr.t < 0.0001f)
                            {
                                ptr.t = 0.0001f;
                            }
                        }

                        VectorLF3 vectorLF8 = new VectorLF3(0f, 0f, 0f);
                        bool flag2 = false;
                        double num32 = (double)(2f - (float)distanceFromPlanetSurface / radius);
                        if (num32 > 1.0)
                        {
                            num32 = 1.0;
                        }
                        else if (num32 < 0.0)
                        {
                            num32 = 0.0;
                        }

                        if (num32 > 0.0)
                        {
                            VectorLF3 v;
                            Maths.QInvRotateLF_refout(ref ptr2.uRot, ref vectorLF, out v);
                            VectorLF3 lhs = Maths.QRotateLF(ptr2.uRotNext, v) + ptr2.uPosNext;
                            Quaternion quaternion;
                            Maths.QInvMultiply_ref(ref ptr2.uRot, ref ptr.uRot, out quaternion);
                            Quaternion quaternion2;
                            Maths.QMultiply_ref(ref ptr2.uRotNext, ref quaternion, out quaternion2);
                            num32 = (3.0 - num32 - num32) * num32 * num32;
                            vectorLF8 = (lhs - ptr.uPos) * num32;
                            ptr.uRot = ((num32 == 1.0) ? quaternion2 : Quaternion.Slerp(ptr.uRot, quaternion2, (float)num32));
                            flag2 = true;
                        }

                        if (!flag2)
                        {
                            VectorLF3 vectorLF9;
                            vectorLF9.x = ptr.uPos.x - starUPosition.x;
                            vectorLF9.y = ptr.uPos.y - starUPosition.y;
                            vectorLF9.z = ptr.uPos.z - starUPosition.z;
                            double num33 = Math.Abs(Math.Sqrt(vectorLF9.x * vectorLF9.x + vectorLF9.y * vectorLF9.y + vectorLF9.z * vectorLF9.z) - (double)dysonSphereLayer.orbitRadius);
                            double num34 = 1.5 - (double)((float)num33 / 1800f);
                            if (num34 > 1.0)
                            {
                                num34 = 1.0;
                            }
                            else if (num34 < 0.0)
                            {
                                num34 = 0.0;
                            }

                            if (num34 > 0.0)
                            {
                                VectorLF3 v2;
                                Maths.QInvRotateLF_refout(ref dysonSphereLayer.currentRotation, ref vectorLF9, out v2);
                                VectorLF3 lhs2 = Maths.QRotateLF(dysonSphereLayer.nextRotation, v2) + starUPosition;
                                Quaternion quaternion3;
                                Maths.QInvMultiply_ref(ref dysonSphereLayer.currentRotation, ref ptr.uRot, out quaternion3);
                                Quaternion quaternion4;
                                Maths.QMultiply_ref(ref dysonSphereLayer.nextRotation, ref quaternion3, out quaternion4);
                                num34 = (3.0 - num34 - num34) * num34 * num34;
                                vectorLF8 = (lhs2 - ptr.uPos) * num34;
                                ptr.uRot = ((num34 == 1.0) ? quaternion4 : Quaternion.Slerp(ptr.uRot, quaternion4, (float)num34));
                            }
                        }

                        double num35 = (double)ptr.uSpeed * timefactor;
                        ptr.uPos.x = ptr.uPos.x + (double)ptr.uVel.x * num35 + vectorLF8.x;
                        ptr.uPos.y = ptr.uPos.y + (double)ptr.uVel.y * num35 + vectorLF8.y;
                        ptr.uPos.z = ptr.uPos.z + (double)ptr.uVel.z * num35 + vectorLF8.z;
                        if (distanceFromPlanetSurface < radius + 50f)
                        {
                            vectorLF.x = ptr2.uPos.x - ptr.uPos.x;
                            vectorLF.y = ptr2.uPos.y - ptr.uPos.y;
                            vectorLF.z = ptr2.uPos.z - ptr.uPos.z;
                            distanceFromPlanetSurface = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)ptr2.uRadius;
                            if (distanceFromPlanetSurface < radius - 20f)
                            {
                                ptr.uPos = ptr2.uPos + Maths.QRotateLF(ptr2.uRot, (VectorLF3)ptr.launch * ((double)ptr2.uRadius + distanceFromPlanetSurface));
                                ptr.uRot = ptr2.uRot * Quaternion.LookRotation(ptr.launch);
                            }
                        }
                    }
                }

            IL_E26: ;
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpaceSector), nameof(SpaceSector.Import))]
        public static bool Import(ref SpaceSector __instance, BinaryReader r)
        {
            if (GS2.IsMenuDemo) return true;
            int num = r.ReadInt32();
            int astroCapacity = r.ReadInt32();
            __instance.SetAstroCapacity(astroCapacity);
            __instance.astroCursor = r.ReadInt32();
            for (int i = 1; i < __instance.astroCursor; i++)
            {
                __instance.astros[i].id = r.ReadInt32();
                if (__instance.astros[i].id > 0)
                {
                    __instance.astros[i].type = (EAstroType)r.ReadInt32();
                    __instance.astros[i].parentId = r.ReadInt32();
                    __instance.astros[i].uPos.x = r.ReadDouble();
                    __instance.astros[i].uPos.y = r.ReadDouble();
                    __instance.astros[i].uPos.z = r.ReadDouble();
                    __instance.astros[i].uPosNext.x = r.ReadDouble();
                    __instance.astros[i].uPosNext.y = r.ReadDouble();
                    __instance.astros[i].uPosNext.z = r.ReadDouble();
                    __instance.astros[i].uRot.x = r.ReadSingle();
                    __instance.astros[i].uRot.y = r.ReadSingle();
                    __instance.astros[i].uRot.z = r.ReadSingle();
                    __instance.astros[i].uRot.w = r.ReadSingle();
                    __instance.astros[i].uRotNext.x = r.ReadSingle();
                    __instance.astros[i].uRotNext.y = r.ReadSingle();
                    __instance.astros[i].uRotNext.z = r.ReadSingle();
                    __instance.astros[i].uRotNext.w = r.ReadSingle();
                    __instance.astros[i].uRadius = r.ReadSingle();
                }
                else
                {
                    __instance.astros[i].SetEmpty();
                }
            }

            astroCapacity = r.ReadInt32();
            __instance.SetEnemyCapacity(astroCapacity);
            __instance.enemyCursor = r.ReadInt32();
            __instance.enemyRecycleCursor = r.ReadInt32();
            for (int j = 1; j < __instance.enemyCursor; j++)
            {
                __instance.enemyPool[j].Import(r);
            }

            for (int k = 0; k < __instance.enemyRecycleCursor; k++)
            {
                __instance.enemyRecycle[k] = r.ReadInt32();
            }

            for (int l = 1; l < __instance.enemyCursor; l++)
            {
                __instance.enemyAnimPool[l].time = r.ReadSingle();
                __instance.enemyAnimPool[l].prepare_length = r.ReadSingle();
                __instance.enemyAnimPool[l].working_length = r.ReadSingle();
                __instance.enemyAnimPool[l].state = r.ReadUInt32();
                __instance.enemyAnimPool[l].power = r.ReadSingle();
            }

            astroCapacity = r.ReadInt32();
            __instance.SetCraftCapacity(astroCapacity);
            __instance.craftCursor = r.ReadInt32();
            __instance.craftRecycleCursor = r.ReadInt32();
            for (int m = 1; m < __instance.craftCursor; m++)
            {
                __instance.craftPool[m].Import(r);
            }

            for (int n = 0; n < __instance.craftRecycleCursor; n++)
            {
                __instance.craftRecycle[n] = r.ReadInt32();
            }

            for (int num2 = 1; num2 < __instance.craftCursor; num2++)
            {
                __instance.craftAnimPool[num2].time = r.ReadSingle();
                __instance.craftAnimPool[num2].prepare_length = r.ReadSingle();
                __instance.craftAnimPool[num2].working_length = r.ReadSingle();
                __instance.craftAnimPool[num2].state = r.ReadUInt32();
                __instance.craftAnimPool[num2].power = r.ReadSingle();
            }

            if (num >= 1)
            {
                __instance.spaceRuins.Import(r);
            }

            __instance.skillSystem.Import(r);
            int num3 = r.ReadInt32();
            if (num3 > 0)
            {
                if (num3 > 65535)
                {
                    throw new Exception("invalid dfcnt!");
                }

                GS2.Random ra = new GS2.Random(num3);
                __instance.dfHives = new EnemyDFHiveSystem[num3];
                for (int num4 = 0; num4 < num3; num4++)
                {
                    __instance.dfHives[num4] = null;
                    var possibleOrbits = GS2.GeneratePossibleHiveOrbits(GetGSStar(__instance.galaxy.stars[num4]));
                    EnemyDFHiveSystem enemyDFHiveSystem = null;
                    while (r.ReadInt32() == 19884)
                    {
                        EnemyDFHiveSystem enemyDFHiveSystem2 = new EnemyDFHiveSystem();
                        enemyDFHiveSystem2.Init(__instance.gameData, __instance.galaxy.stars[num4].id, 0);
                        enemyDFHiveSystem2.Import(r);
                        if (enemyDFHiveSystem2.hiveAstroOrbit.orbitRadius > __instance.galaxy.stars[num4].systemRadius)
                        {
                            Warn($"DFHive orbit radius is larger than {__instance.galaxy.stars[num4].name} system radius {enemyDFHiveSystem2.hiveAstroOrbit.orbitRadius} > {__instance.galaxy.stars[num4].systemRadius}");
                            var orbit = ra.ItemAndRemove(possibleOrbits);
                            Warn($"New Orbit Radius = {orbit}");
                            enemyDFHiveSystem2.hiveAstroOrbit.orbitRadius = orbit;
                        }


                        if (enemyDFHiveSystem == null)
                        {
                            __instance.dfHives[num4] = enemyDFHiveSystem2;
                        }
                        else
                        {
                            enemyDFHiveSystem.nextSibling = enemyDFHiveSystem2;
                        }

                        enemyDFHiveSystem2.prevSibling = enemyDFHiveSystem;
                        enemyDFHiveSystem = enemyDFHiveSystem2;
                    }
                }
            }

            __instance.combatSpaceSystem = new CombatSpaceSystem(__instance);
            __instance.combatSpaceSystem.Import(r);
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.GameTickLogic))]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.KeyTickLogic))]
        public static bool EDFHGTL(ref EnemyDFHiveSystem __instance)
        {
            if (Config.SkipDFHiveLogic)
            {
                if ((__instance.starData.uPosition - GameMain.mainPlayer.uPosition).magnitude > Config.SkipDFHiveDistance * 2400000f)
                {
                    return false;
                }

                // return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyDFGroundSystem), nameof(EnemyDFGroundSystem.GameTickLogic))]
        [HarmonyPatch(typeof(EnemyDFGroundSystem), nameof(EnemyDFGroundSystem.KeyTickLogic))]
        public static bool EDFHGTL(ref EnemyDFGroundSystem __instance)
        {
            if (Config.SkipDFHiveLogic)
            {
                if ((__instance.planet.uPosition - GameMain.mainPlayer.uPosition).magnitude > Config.SkipDFHiveDistance * 2400000f)
                {
                    return false;
                }

                // return false;
            }

            return true;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(SpaceColliderLogic), nameof(SpaceColliderLogic.Init))]
        public static IEnumerable<CodeInstruction> InitTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(i => i.opcode == Ldc_I4 && Convert.ToInt32(i.operand) == 512)
                );
            if (matcher.IsValid)
            {
                matcher.Repeat(matcher => { matcher.SetOperandAndAdvance(8192); });
            }
            else
            {
                Error("Failed to patch SpaceColliderLogic.Init");
            }

            return instructions;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.Init))]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.Import))]
        public static void EnemyDFHiveSystemInitImport(ref EnemyDFHiveSystem __instance)
        {
            if (__instance.idleRelayIds.Length < 2048)
            {
                var newArray = new int[2048];
                Array.Copy(__instance.idleRelayIds, newArray, __instance.idleRelayIds.Length);
                __instance.idleRelayIds = newArray;
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerAction_Inspect), nameof(PlayerAction_Inspect.GetObjectSelectDistance))]
        public static void GetObjectSelectDistance(ref PlayerAction_Inspect __instance, ref float __result, EObjectType objType, int objid)
        {
            if (objid == 0)
            {
                return;
            }

            if (__instance.player.factory == null)
            {
                return;
            }

            if (objType != EObjectType.Entity) return;
            var id = __instance.player.factory.entityPool[objid].protoId;
            if (id == 2107 || id == 2103 || id == 2104) __result = 2000f;
            if (id == 2105) __result = 15000f;
            if (__result == 35f) __result = 50f;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(DefenseSystem), nameof(DefenseSystem.AfterTurretsImport))]
        private static void AfterTurretsImport(ref DefenseSystem __instance)
        {
            int cursor = __instance.turrets.cursor;
            TurretComponent[] buffer = __instance.turrets.buffer;
            for (int i = 1; i < cursor; i++)
            {
                ref TurretComponent ptr = ref buffer[i];
                if (ptr.id == 1) TurretComponentTranspiler.AddTurret(__instance, ref ptr);
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.ApplyGravity))]
        private static bool ApplyGravity(ref PlayerController __instance)
        {
            if (Config.NewGravity) return true;
            VectorLF3 v = VectorLF3.zero;
            if (GameMain.localStar != null)
            {
                StarData localStar = GameMain.localStar;
                double num = 0.0;
                VectorLF3 lhs = VectorLF3.zero;
                for (int i = 0; i < localStar.planetCount; i++)
                {
                    PlanetData planetData = localStar.planets[i];
                    VectorLF3 lhs2 = planetData.uPosition - __instance.player.uPosition;
                    double magnitude = lhs2.magnitude;
                    if (magnitude > 1.0)
                    {
                        double y = (double)Math.Max((800f - (float)magnitude) / 150f, 0f);
                        double num2 = Math.Pow(10.0, y);
                        VectorLF3 lhs3 = lhs2 / magnitude;
                        double num3 = magnitude / (double)planetData.realRadius;
                        double num4 = num3 * 0.800000011920929;
                        double num5 = ((num3 < 1.0) ? num3 : (1.0 / (num4 * num4)));
                        if (num5 > 1.0)
                        {
                            num5 = 1.0;
                        }

                        double num6 = Math.Sqrt((double)planetData.realRadius) * 3.5;
                        lhs += lhs3 * (num6 * num5 * num2);
                        num += num2;
                    }
                }

                VectorLF3 lhs4 = localStar.uPosition - __instance.player.uPosition;
                double magnitude2 = lhs4.magnitude;
                if (magnitude2 > 1.0)
                {
                    double num7 = 1.0;
                    VectorLF3 lhs5 = lhs4 / magnitude2;
                    double num8 = magnitude2 / (double)(localStar.orbitScaler * 800f);
                    double num9 = num8 * 0.10000000149011612;
                    double num10 = ((num8 < 1.0) ? num8 : (1.0 / (num9 * num9)));
                    if (num10 > 1.0)
                    {
                        num10 = 1.0;
                    }

                    double num11 = 26.7;
                    lhs += lhs5 * (num11 * num10 * num7);
                    num += num7;
                }

                v = lhs / num;
            }

            if (v.sqrMagnitude > 1E-06)
            {
                __instance.universalGravity = v;
                __instance.localGravity = Maths.QInvRotateLF(__instance.gameData.relativeRot, v);
            }
            else
            {
                __instance.universalGravity = VectorLF3.zero;
                __instance.localGravity = Vector3.zero;
            }

            if (!__instance.player.sailing && !__instance.gameData.disableController && __instance.player.isAlive)
            {
                __instance.AddLocalForce(__instance.localGravity);
            }

            Debug.DrawRay(__instance.transform.localPosition, __instance.localGravity * 0.1f, Color.white);
            if (__instance.gameData.localPlanet != null)
            {
                Vector3 forward = __instance.transform.forward;
                Vector3 normalized = __instance.transform.localPosition.normalized;
                Vector3 normalized2 = Vector3.Cross(Vector3.Cross(normalized, forward).normalized, normalized).normalized;
                __instance.transform.localRotation = Quaternion.LookRotation(normalized2, normalized);
                return false;
            }

            __instance.transform.localRotation = Quaternion.identity;
            return false;
        }
    }
}