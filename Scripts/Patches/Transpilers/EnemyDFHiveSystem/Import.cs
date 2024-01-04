using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale.Patches

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
                    // GS3.12.8 which didnt touch hives. As GS3 doesnt store the hive orbits, they might be missing
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
                    return GalacticScale.GS3.LogTranspilerError(instructions,
                        "Transpiler EnemyDFHiveSystem:Import failed.");
                }

                GS3.Log("Transpiler EnemyDFHiveSystem:Import matcher succeeded.");

                codeMatcher.InsertAndAdvance(new CodeInstruction(Ldarg_0));
                codeMatcher.InsertAndAdvance(
                    Transpilers.EmitDelegate<Action<EnemyDFHiveSystem>>(DarkFog.GenerateMissingHiveOrbits));
                return codeMatcher.InstructionEnumeration();
            }
            catch
            {
                return GS3.LogTranspilerError(instructions, $"Transpiler EnemyDFHiveSystem:Import failed.");
            }
        }

        
 
    }
}