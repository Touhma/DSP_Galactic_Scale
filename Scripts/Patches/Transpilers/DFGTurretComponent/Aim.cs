using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale.Patches

{
    public static class PatchOnDFGTurretComponent
    {
        [HarmonyTranspiler]   
        [HarmonyPatch(typeof(DFGTurretComponent),  nameof(DFGTurretComponent.Aim))]
        public static IEnumerable<CodeInstruction> Aim_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var matcher = new CodeMatcher(instructions)
                    .MatchForward(
                        true,
                        new CodeMatch(i => (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) &&
                                           (Convert.ToDouble(i.operand ?? 0.0) == 200.0 ||
                                            Convert.ToDouble(i.operand ?? 0.0) == 197.6)
                    ));
                if (!matcher.IsInvalid)
                {
                    matcher.Repeat(matcher =>
                    {
                        // var mi = AccessTools.Method(typeof(EnemyUnitComponentTranspiler), nameof(Utils.GetRadiusFromFactory)).MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
                        var mi = matcher.GetRadiusFromFactory();
                        matcher.Advance(1);
                        matcher.InsertAndAdvance(new CodeInstruction(Ldarg_1));
                        matcher.Insert(new CodeInstruction(Call, mi));
                    });
                    instructions = matcher.InstructionEnumeration();
                    return instructions;
                }
                return instructions;
            }
            catch
            {
                Bootstrap.Logger.LogInfo("TurretComponentTranspiler.Shoot_PlasmaTranspiler failed");
                return instructions;
            }
        }

        

        // [HarmonyPatch(typeof(FleetComponent),  nameof(FleetComponent.GetUnitOrbitingAstroPose))]

        // [HarmonyPatch(typeof(SkillSystem),  nameof(SkillSystem.AddSpaceEnemyHatred), new[]
        // {
        //     typeof(EnemyDFHiveSystem), 
        //     typeof(EnemyData), 
        //     typeof(ETargetType), 
        //     typeof(int), 
        //     typeof(int)
        // }, new[]
        // {
        //     ArgumentType.Normal, 
        //     ArgumentType.Ref, 
        //     ArgumentType.Normal, 
        //     ArgumentType.Normal, 
        //     ArgumentType.Normal
        // })]
    }
}