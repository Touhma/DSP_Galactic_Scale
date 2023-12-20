using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using static GalacticScale.Utils;

namespace GalacticScale

{
    // [HarmonyPatch(typeof(EnemyData), "Formation", new Type[] {typeof(int), typeof(EnemyData), typeof(float), typeof(VectorLF3), typeof(Quaternion), typeof(Vector3)}, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Ref, ArgumentType.Ref})]
    // [HarmonyPatch(typeof(TurretComponent), "CheckEnemyIsInAttackRange")]
    // [HarmonyPatch(typeof(DFGTurretComponent), "Aim")]
    // [HarmonyPatch(typeof(EnemyUnitComponent), "Attack_SLancer")]
    // [HarmonyPatch(typeof(GrowthTool_Node_DFGround), "CreateNode7")]
    // [HarmonyPatch(typeof(PlayerNavigation), "DetermineArrive")]
    // [HarmonyPatch(typeof(DFRelayComponent), "RelaySailLogic")]
    // [HarmonyPatch(typeof(PlayerAction_Navigate), "GameTick")]
    // [HarmonyPatch(typeof(FleetComponent), "GetUnitOrbitingAstroPose")]
    // [HarmonyPatch(typeof(PlayerNavigation), "Init")]
    // [HarmonyPatch(typeof(PlanetEnvironment), "LateUpdate")]
    // [HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Engage_GRaider")]
    // [HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Engage_GRanger")]
    // [HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Engage_SHumpback")]
    // // [HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_OrbitTarget_SLancer")]
    // [HarmonyPatch(typeof(UnitComponent), "RunBehavior_Engage_SAttackLaser_Large")]
    // [HarmonyPatch(typeof(UnitComponent), "RunBehavior_Engage_SAttackPlasma_Small")]
    // // [HarmonyPatch(typeof(EnemyUnitComponent), "SeekToHive_Space_FollowLeader")]
    // // [HarmonyPatch(typeof(EnemyUnitComponent), "SeekToTargetPoint_Space_FollowLeader")]
    // // [HarmonyPatch(typeof(FleetComponent), "SensorLogic_Ground")]
    // [HarmonyPatch(typeof(TurretComponent), "SetStateToAim_Default")]
    // [HarmonyPatch(typeof(PlayerAction_Combat), "Shoot_Gauss_Space")]
    // [HarmonyPatch(typeof(TurretComponent), "Shoot_Plasma")]
    // [HarmonyPatch(typeof(PlayerAction_Combat), "Shoot_Plasma")]
    // [HarmonyPatch(typeof(DFSTurretComponent), "Shoot_Plasma")]
    // [HarmonyPatch(typeof(DFGTurretComponent), "Shoot_Plasma")]
    // [HarmonyPatch(typeof(DFGTurretComponent), "Shoot_Laser")]
    // [HarmonyPatch(typeof(DFTinderComponent), "TinderSailLogic")]
    // [HarmonyPatch(typeof(PlayerAction_Plant), "UpdateRaycast")]
    public class PatchOnEnemyStuff
    {
        [HarmonyPrefix, HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Engage_GRaider")]
        // EnemyUnitComponent
// Token: 0x060006BD RID: 1725 RVA: 0x0004BF78 File Offset: 0x0004A178
        public static bool RunBehavior_Engage_GRaider(ref EnemyUnitComponent __instance, PlanetFactory factory,
            ref EnemyData enemy)
        {
            Vector3 vector;
            float num;
            if (__instance.GetTargetPosition_Ground(factory, __instance.hatred.max.target, out vector, out num))
            {
                int num2 = (int)(enemy.port - 1);
                int num3 = num2 / 12;
                int num4 = num2 * 7 % 12;
                ref VectorLF3 ptr = ref enemy.pos;
                ref Quaternion ptr2 = ref enemy.rot;
                ref Vector3 ptr3 = ref enemy.vel;
                PrefabDesc prefabDesc = SpaceSector.PrefabDescByModelIndex[(int)enemy.modelIndex];
                if ((GameMain.gameTick + (long)num2) % 2L == 0L)
                {
                    HashSystem hashSystemStatic = factory.hashSystemStatic;
                    float cellSize = HashSystem.cellSize;
                    int num5 = (int)((ptr.x + 270.0) / (double)cellSize);
                    int num6 = (int)((ptr.y + 270.0) / (double)cellSize);
                    int num7 = (int)((ptr.z + 270.0) / (double)cellSize);
                    num5 = ((num5 < 99) ? ((num5 < 0) ? 0 : num5) : 99);
                    num6 = ((num6 < 99) ? ((num6 < 0) ? 0 : num6) : 99);
                    num7 = ((num7 < 99) ? ((num7 < 0) ? 0 : num7) : 99);
                    int num8 = num7 * 10000 + num6 * 100 + num5;
                    int bucketIndex = (int)HashSystem.bucketMap[num8 * HashSystem.areaBucketsCount].bucketIndex;
                    int num9 = hashSystemStatic.bucketOffsets[bucketIndex];
                    int num10 = num9 + hashSystemStatic.bucketCursors[bucketIndex];
                    int[] hashPool = hashSystemStatic.hashPool;
                    EntityData[] entityPool = factory.entityPool;
                    for (int i = num9; i < num10; i++)
                    {
                        int num11 = hashPool[i];
                        if (num11 != 0 && num11 >> 28 == 0)
                        {
                            int num12 = num11 & 268435455;
                            ref EntityData ptr4 = ref entityPool[num12];
                            if (ptr4.id == num12 && ptr4.protoId >= 2013)
                            {
                                float num13 = SkillSystem.RoughWidthByModelIndex[(int)ptr4.modelIndex];
                                if (num13 >= 1.5f)
                                {
                                    float num14 = num13 * num13 / 1.5f;
                                    float num15 = 0f;
                                    float num16 = ptr4.pos.x - (float)ptr.x;
                                    num15 += num16 * num16;
                                    if (num15 <= num14)
                                    {
                                        float num17 = ptr4.pos.y - (float)ptr.y;
                                        num15 += num17 * num17;
                                        if (num15 <= num14)
                                        {
                                            float num18 = ptr4.pos.z - (float)ptr.z;
                                            num15 += num18 * num18;
                                            if (num15 <= num14)
                                            {
                                                float num19 = ptr4.alt;
                                                if (num19 < getPlanetSize())
                                                {
                                                    num19 = getPlanetSize();
                                                }

                                                float num20 = ptr4.pos.x / num19;
                                                float num21 = ptr4.pos.y / num19;
                                                float num22 = ptr4.pos.z / num19;
                                                float num23 = num16 * num20 + num17 * num21 + num18 * num22;
                                                num16 -= num23 * num20;
                                                num17 -= num23 * num21;
                                                num18 -= num23 * num22;
                                                float num24 =
                                                    Mathf.Sqrt(num16 * num16 + num17 * num17 + num18 * num18) + 1E-05f;
                                                num16 /= num24;
                                                num17 /= num24;
                                                num18 /= num24;
                                                float num25 = num16 * ptr3.x + num17 * ptr3.y + num18 * ptr3.z;
                                                float num26 = 1f - num15 / num14;
                                                float num27 = 0.78f + 0.17f * num15 / num14;
                                                if (num25 > 0.1f)
                                                {
                                                    ptr3.x -= num25 * num16 * num26;
                                                    ptr3.y -= num25 * num17 * num26;
                                                    ptr3.z -= num25 * num18 * num26;
                                                }

                                                if (ptr3.x * ptr3.x + ptr3.y * ptr3.y + ptr3.z * ptr3.z > 14f)
                                                {
                                                    ptr3.x *= num27;
                                                    ptr3.y *= num27;
                                                    ptr3.z *= num27;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                float num28 = (float)__instance.level * 0.03f + 0.7f;
                if (num28 > 1f)
                {
                    num28 = 1f;
                }

                float num29 = prefabDesc.unitMaxMovementSpeed * num28;
                float num30 = prefabDesc.unitMaxMovementAcceleration;
                float unitAttackRange = prefabDesc.unitAttackRange0;
                float num31 = prefabDesc.unitEngageArriveRange;
                if (num >= 4f)
                {
                    num31 += 5f;
                }

                float num32 = vector.x - (float)ptr.x;
                float num33 = vector.y - (float)ptr.y;
                float num34 = vector.z - (float)ptr.z;
                float num35 = (float)(ptr.x * ptr.x + ptr.y * ptr.y + ptr.z * ptr.z);
                float num36 = vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
                float num37 = Mathf.Sqrt(num35);
                float num38 = (num36 - num35) / (num37 * 2f);
                bool flag = num38 > (unitAttackRange + num) * 0.9f;
                if (num37 + num38 > getPlanetSize(6f))
                {
                    num38 = getPlanetSize(6f) - num37;
                }
                else if (num37 + num38 < getPlanetSize(2f))
                {
                    num38 = getPlanetSize(2f) - num37;
                }

                float num39 = (float)ptr.x / num37;
                float num40 = (float)ptr.y / num37;
                float num41 = (float)ptr.z / num37;
                float num42 = num32 * num39 + num33 * num40 + num34 * num41;
                num42 -= num38;
                num32 -= num39 * num42;
                num33 -= num40 * num42;
                num34 -= num41 * num42;
                float num43 = Mathf.Sqrt(num32 * num32 + num33 * num33 + num34 * num34);
                float fx = num32 / num43;
                float fy = num33 / num43;
                float fz = num34 / num43;
                num43 = ((num43 > -num42) ? num43 : (-num42));
                float num44 = num43 - num;
                if (num44 < 0f)
                {
                    num44 = 0f;
                }

                float num45 = (num44 - num31 * 0.5f) / num31;
                float num46 = num45 * 0.7f;
                num45 = ((num45 < 1f) ? ((num45 < 0f) ? 0f : num45) : 1f);
                num45 = 2f * num45 - num45 * num45;
                num46 = ((num46 < 1f) ? ((num46 < 0f) ? 0f : num46) : 1f);
                float num47 = (num + unitAttackRange) * num46;
                float num48 = ((float)num4 - 5.5f) / 5.5f;
                float num49 = num40 * num34 - num33 * num41;
                float num50 = num41 * num32 - num34 * num39;
                float num51 = num39 * num33 - num32 * num40;
                float num52 = num47 * num48 / Mathf.Sqrt(num49 * num49 + num50 * num50 + num51 * num51);
                num49 *= num52;
                num50 *= num52;
                num51 *= num52;
                num32 += num49;
                num33 += num50;
                num34 += num51;
                float num53 = Mathf.Sqrt(num32 * num32 + num33 * num33 + num34 * num34);
                float num54 = num29 * num45 / num53;
                num32 *= num54;
                num33 *= num54;
                num34 *= num54;
                num30 /= 1f + num46 * 1f;
                float num55 = num32 - ptr3.x;
                float num56 = num33 - ptr3.y;
                float num57 = num34 - ptr3.z;
                float num58 = Mathf.Sqrt(num55 * num55 + num56 * num56 + num57 * num57) / (num30 * 0.016666668f);
                if (num58 > 1f)
                {
                    num55 /= num58;
                    num56 /= num58;
                    num57 /= num58;
                }

                ptr3.x += num55;
                ptr3.y += num56;
                ptr3.z += num57;
                __instance.repulsion.x = __instance.repulsion.x * 0.87f;
                __instance.repulsion.y = __instance.repulsion.y * 0.87f;
                __instance.repulsion.z = __instance.repulsion.z * 0.87f;
                DataPool<EnemyUnitComponent> units = factory.enemySystem.units;
                if (units.cursor > 1)
                {
                    __instance.stateTick++;
                    int num59 = __instance.stateTick % units.cursor;
                    if (num59 != 0 && units.buffer[num59].protoId == __instance.protoId)
                    {
                        ref EnemyUnitComponent ptr5 = ref units.buffer[num59];
                        ref VectorLF3 ptr6 = ref factory.enemyPool[ptr5.enemyId].pos;
                        double num60 = 0.0;
                        double num61 = ptr6.x - ptr.x;
                        num60 += num61 * num61;
                        if (num60 <= 16.0)
                        {
                            num61 = ptr6.y - ptr.y;
                            num60 += num61 * num61;
                            if (num60 <= 16.0)
                            {
                                num61 = ptr6.z - ptr.z;
                                num60 += num61 * num61;
                                if (num60 <= 16.0)
                                {
                                    num60 /= 4.0;
                                    num60 = 1.0 / num60;
                                    if (num60 > 1.0)
                                    {
                                        num60 = 1.0;
                                    }
                                    else if (num60 < 0.5)
                                    {
                                        num60 = 0.0;
                                    }

                                    if (num60 > 0.0)
                                    {
                                        __instance.repulsion.x =
                                            __instance.repulsion.x + (float)((ptr.x - ptr6.x) * num60);
                                        __instance.repulsion.y =
                                            __instance.repulsion.y + (float)((ptr.y - ptr6.y) * num60);
                                        __instance.repulsion.z =
                                            __instance.repulsion.z + (float)((ptr.z - ptr6.z) * num60);
                                        float num62 = __instance.repulsion.x * num39 + __instance.repulsion.y * num40 +
                                                      __instance.repulsion.z * num41;
                                        __instance.repulsion.x = __instance.repulsion.x - num39 * num62;
                                        __instance.repulsion.y = __instance.repulsion.y - num40 * num62;
                                        __instance.repulsion.z = __instance.repulsion.z - num41 * num62;
                                        float num63 = __instance.repulsion.x * __instance.repulsion.x +
                                                      __instance.repulsion.y * __instance.repulsion.y +
                                                      __instance.repulsion.z * __instance.repulsion.z;
                                        if (num63 > 1.44f)
                                        {
                                            float num64 = 1.2f / Mathf.Sqrt(num63);
                                            __instance.repulsion.x = __instance.repulsion.x * num64;
                                            __instance.repulsion.y = __instance.repulsion.y * num64;
                                            __instance.repulsion.z = __instance.repulsion.z * num64;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                ptr3.x += __instance.repulsion.x;
                ptr3.y += __instance.repulsion.y;
                ptr3.z += __instance.repulsion.z;
                ptr.x += (double)ptr3.x * 0.016666666666666666;
                ptr.y += (double)ptr3.y * 0.016666666666666666;
                ptr.z += (double)ptr3.z * 0.016666666666666666;
                float f = ptr3.x * ptr3.x + ptr3.y * ptr3.y + ptr3.z * ptr3.z;
                __instance.speed = Mathf.Sqrt(f);
                if (__instance.speed > 0.1f)
                {
                    Maths.LookRotation(ptr3.x / __instance.speed, ptr3.y / __instance.speed, ptr3.z / __instance.speed,
                        num39, num40, num41, out ptr2);
                }

                float num65 = 1f - (num44 - unitAttackRange * 0.8f) / (unitAttackRange * 1.5f);
                num65 = ((num65 < 1f) ? ((num65 < 0f) ? 0f : num65) : 1f);
                if (num65 > 0f)
                {
                    Quaternion b;
                    Maths.LookRotation(fx, fy, fz, num39, num40, num41, out b);
                    ptr2 = Quaternion.Slerp(ptr2, b, num65 * 0.15f);
                }

                if (!flag && num44 < unitAttackRange && __instance.fire0 <= 0 && __instance.heat >= 0)
                {
                    ref LocalLaserOneShot ptr7 = ref GameMain.data.spaceSector.skillSystem.raiderLasers.Add();
                    ptr7.astroId = factory.planetId;
                    ptr7.hitIndex = 2;
                    ptr7.beginPos = enemy.pos;
                    ptr7.target = __instance.hatred.max.skillTargetLocal;
                    if (ptr7.target.type == ETargetType.None)
                    {
                        ptr7.endPos = vector + vector.normalized *
                            (SkillSystem.RoughHeightByModelIndex[(int)factory.entityPool[ptr7.target.id].modelIndex] *
                             0.4f);
                    }
                    else if (ptr7.target.type == ETargetType.Player)
                    {
                        ptr7.endPos = vector;
                    }
                    else if (ptr7.target.type == ETargetType.Craft)
                    {
                        ptr7.endPos = vector;
                    }

                    ptr7.caster.type = ETargetType.Enemy;
                    ptr7.caster.id = enemy.id;
                    ptr7.damage = prefabDesc.unitAttackDamage0 + prefabDesc.unitAttackDamageInc0 * __instance.level;
                    ptr7.mask = (ETargetTypeMask.Entity | ETargetTypeMask.Craft | ETargetTypeMask.Player);
                    ptr7.life = 15;
                    __instance.fire0 = prefabDesc.unitAttackInterval0;
                    __instance.heat += prefabDesc.unitAttackHeat0;
                }

                __instance.anim = (1f - (float)__instance.fire0 / (float)prefabDesc.unitAttackInterval0 * 2f) * num65;
                __instance.steering = 0f;
                return false;
            }

            __instance.RunBehavior_Engage_EmptyHatred(ref enemy);
            return false;
        }
    }
}