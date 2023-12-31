using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    public partial class PatchOnEnemyDFHiveSystem
    {
        [HarmonyTranspiler, HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.Import))]
        public static IEnumerable<CodeInstruction> TranspilerEnemyDFHiveSystemImport(
            IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var codeMatcher = new CodeMatcher(instructions)
                    //----------------------------------------------------------------------------------------------------
                    // Line 54-61
                    // this.hiveOrbitIndex = this.hiveAstroId - 1000000 - (this.starData.id - 1) * 8 - 1;
                    //----------------------------------------------------------------------------------------------------
                    // /* 0x0009B9B3 02           */ IL_002B: ldarg.0
                    // /* 0x0009B9B4 02           */ IL_002C: ldarg.0
                    // /* 0x0009B9B5 7BBF0B0004   */ IL_002D: ldfld     int32 EnemyDFHiveSystem::hiveAstroId
                    // /* 0x0009B9BA 2040420F00   */ IL_0032: ldc.i4    1000000
                    // /* 0x0009B9BF 59           */ IL_0037: sub
                    // /* 0x0009B9C0 02           */ IL_0038: ldarg.0
                    // /* 0x0009B9C1 7BBD0B0004   */ IL_0039: ldfld     class StarData EnemyDFHiveSystem::starData
                    // /* 0x0009B9C6 7B771C0004   */ IL_003E: ldfld     int32 StarData::id
                    // /* 0x0009B9CB 17           */ IL_0043: ldc.i4.1
                    // /* 0x0009B9CC 59           */ IL_0044: sub
                    // /* 0x0009B9CD 1E           */ IL_0045: ldc.i4.8
                    // /* 0x0009B9CE 5A           */ IL_0046: mul
                    // /* 0x0009B9CF 59           */ IL_0047: sub
                    // /* 0x0009B9D0 17           */ IL_0048: ldc.i4.1
                    // /* 0x0009B9D1 59           */ IL_0049: sub
                    // /* 0x0009B9D2 7DC20B0004   */ IL_004A: stfld     int32 EnemyDFHiveSystem::hiveOrbitIndex
                    //----------------------------------------------------------------------------------------------------
                    // Find the above code, and insert our patch before it.
                    // This creates some generic Hive orbits for the star in case the game was created before
                    // GS2.12.8 which didnt touch hives. As GS2 doesnt store the hive orbits, they might be missing
                    // from subsequent generations
                    //----------------------------------------------------------------------------------------------------
                    .MatchForward(
                        false,
                        new CodeMatch(i => i.opcode == Ldarg_0),
                        new CodeMatch(i => i.opcode == Ldarg_0),
                        new CodeMatch(i => i.opcode == Ldfld && (FieldInfo)i.operand ==
                            AccessTools.Field(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.hiveAstroId))),
                        new CodeMatch(i => i.opcode == Ldc_I4 && Convert.ToInt32(i.operand) == 1000000),
                        new CodeMatch(i => i.opcode == Sub),
                        new CodeMatch(i => i.opcode == Ldarg_0),
                        new CodeMatch(i => i.opcode == Ldfld && (FieldInfo)i.operand ==
                            AccessTools.Field(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.starData))),
                        new CodeMatch(i => i.opcode == Ldfld && (FieldInfo)i.operand ==
                            AccessTools.Field(typeof(StarData), nameof(StarData.id))),
                        new CodeMatch(i => i.opcode == Ldc_I4_1),
                        new CodeMatch(i => i.opcode == Sub),
                        new CodeMatch(i => i.opcode == Ldc_I4_8),
                        new CodeMatch(i => i.opcode == Mul),
                        new CodeMatch(i => i.opcode == Sub),
                        new CodeMatch(i => i.opcode == Ldc_I4_1),
                        new CodeMatch(i => i.opcode == Sub),
                        new CodeMatch(i => i.opcode == Stfld && (FieldInfo)i.operand ==
                            AccessTools.Field(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.hiveOrbitIndex)))
                    );
                if (codeMatcher.IsInvalid)
                {
                    return GalacticScale.GS2.LogTranspilerError(instructions,
                        "Transpiler EnemyDFHiveSystem:Import failed.");
                }

                GS2.Log("Transpiler EnemyDFHiveSystem:Import matcher succeeded.");

                codeMatcher.InsertAndAdvance(new CodeInstruction(Ldarg_0));
                codeMatcher.InsertAndAdvance(
                    Transpilers.EmitDelegate<Action<EnemyDFHiveSystem>>(GenerateMissingHiveOrbits));
                return codeMatcher.InstructionEnumeration();
            }
            catch
            {
                return GS2.LogTranspilerError(instructions, $"Transpiler EnemyDFHiveSystem:Import failed.");
            }
        }

        public static void GenerateMissingHiveOrbits(EnemyDFHiveSystem __instance)
        {
            if (__instance.starData.hiveAstroOrbits == null || __instance.starData.hiveAstroOrbits.Length == 0)
            {
                __instance.starData.hiveAstroOrbits = new AstroOrbitData[8];
            }

            if (__instance.starData.hiveAstroOrbits.Length - 1 < __instance.hiveOrbitIndex)
            {
                var temp = new AstroOrbitData[8];
                Array.Copy(__instance.starData.hiveAstroOrbits, temp, __instance.starData.hiveAstroOrbits.Length);
                __instance.starData.hiveAstroOrbits = temp;
            }

            if (__instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex] == null)
            {
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex] = new AstroOrbitData();
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitRadius =
                    10f * __instance.hiveOrbitIndex;
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitInclination = 0f;
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitLongitude = 0f;
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitPhase = 0f;
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitalPeriod =
                    Utils.CalculateOrbitPeriod(__instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex]
                        .orbitRadius);
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitRotation =
                    Quaternion.AngleAxis(__instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitLongitude,
                        Vector3.up) *
                    Quaternion.AngleAxis(
                        __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitInclination,
                        Vector3.forward);
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitNormal =
                    Maths.QRotateLF(__instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitRotation,
                        new VectorLF3(0f, 1f, 0f)).normalized;
            }
        }
        // [HarmonyPrefix, HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.Import))]
        // public static bool Import(ref EnemyDFHiveSystem __instance, BinaryReader r)
        // {
        //     int num = r.ReadInt32();
        //     __instance.hiveAstroId = r.ReadInt32();
        //     __instance.seed = r.ReadInt32();
        //     __instance.rtseed = r.ReadInt32();
        //     __instance.hiveOrbitIndex = __instance.hiveAstroId - 1000000 - (__instance.starData.id - 1) * 8 - 1;
        //     
        //     
        //
        //     __instance.hiveAstroOrbit = __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex];
        //     __instance.orbitRadius = (double)__instance.hiveAstroOrbit.orbitRadius * 40000.0;
        //     GS2.Log("1");
        //     int num2 = r.ReadInt32();
        //     __instance.pbuilders = new GrowthPattern_DFSpace.Builder[num2];
        //     for (int i = 0; i < num2; i++)
        //     {
        //         __instance.pbuilders[i].Import(r);
        //     }
        //
        //     GS2.Log("2");
        //     __instance.GenerateDocks();
        //     __instance.realized = r.ReadBoolean();
        //     __instance.isEmpty = r.ReadBoolean();
        //     __instance.ticks = r.ReadInt32();
        //     __instance.turboTicks = r.ReadInt32();
        //     __instance.turboRepress = r.ReadInt32();
        //     __instance.matterStatComplete = r.ReadBoolean();
        //     __instance.matterProductStat = r.ReadInt32();
        //     __instance.matterConsumeStat = r.ReadInt32();
        //     __instance.matterProduction = r.ReadInt32();
        //     __instance.matterConsumption = r.ReadInt32();
        //     __instance.rootEnemyId = r.ReadInt32();
        //     __instance.isCarrierRealized = r.ReadBoolean();
        //     __instance.tindersInTransit = r.ReadInt32();
        //     if (num >= 1)
        //     {
        //         __instance.lancerAssaultCountBase = r.ReadSingle();
        //         if (num == 1)
        //         {
        //             r.ReadInt32();
        //         }
        //     }
        //     else
        //     {
        //         __instance.lancerAssaultCountBase =
        //             (float)__instance.GetLancerAssaultCountInitial(__instance.history.combatSettings.aggressiveLevel);
        //     }
        //
        //     if (num >= 3)
        //     {
        //         __instance.relayNeutralizedCounter = r.ReadInt32();
        //     }
        //     else
        //     {
        //         __instance.relayNeutralizedCounter = 0;
        //     }
        //
        //     GS2.Log("3");
        //     if (__instance.hiveAstroId > 1000000)
        //     {
        //         __instance.sector.dfHivesByAstro[__instance.hiveAstroId - 1000000] = __instance;
        //     }
        //
        //     __instance.builders.Import(r);
        //     __instance.cores.Import(r);
        //     __instance.nodes.Import(r);
        //     __instance.connectors.Import(r);
        //     __instance.replicators.Import(r);
        //     __instance.gammas.Import(r);
        //     __instance.turrets.Import(r);
        //     __instance.relays.Import(r);
        //     __instance.tinders.Import(r);
        //     __instance.units.Import(r);
        //     GS2.Log("4");
        //     for (int j = 1; j < __instance.relays.cursor; j++)
        //     {
        //         DFRelayComponent dfrelayComponent = __instance.relays.buffer[j];
        //         if (dfrelayComponent != null)
        //         {
        //             dfrelayComponent.hive = __instance;
        //             dfrelayComponent.SetDockIndex(dfrelayComponent.dockIndex);
        //         }
        //     }
        //
        //     GS2.Log("5");
        //     for (int k = 1; k < __instance.tinders.cursor; k++)
        //     {
        //         ref DFTinderComponent ptr = ref __instance.tinders.buffer[k];
        //         if (ptr.id == k)
        //         {
        //             ptr.SetDockIndex(__instance, ptr.dockIndex);
        //         }
        //     }
        //
        //     GS2.Log("6");
        //     int num3 = r.ReadInt32();
        //     __instance.idleRelayIds = new int[num3];
        //     __instance.idleRelayCount = r.ReadInt32();
        //     for (int l = 0; l < __instance.idleRelayCount; l++)
        //     {
        //         __instance.idleRelayIds[l] = r.ReadInt32();
        //     }
        //
        //     GS2.Log("7");
        //     num3 = r.ReadInt32();
        //     __instance.idleTinderIds = new int[num3];
        //     __instance.idleTinderCount = r.ReadInt32();
        //     for (int m = 0; m < __instance.idleTinderCount; m++)
        //     {
        //         __instance.idleTinderIds[m] = r.ReadInt32();
        //     }
        //
        //     GS2.Log("8");
        //     __instance.forms = new EnemyFormation[3];
        //     __instance.forms[0] = new EnemyFormation();
        //     __instance.forms[1] = new EnemyFormation();
        //     __instance.forms[2] = new EnemyFormation();
        //     __instance.forms[0].Import(r);
        //     __instance.forms[1].Import(r);
        //     __instance.forms[2].Import(r);
        //     __instance.evolve.Import(r);
        //     __instance.hatred.Import(r);
        //     __instance.hatredAstros.Import(r);
        //     __instance.CalcFormsSupply();
        //     __instance.CreateAntSegmentBuffer();
        //     GS2.Log("9");
        //     return false;
        // }
    }
}