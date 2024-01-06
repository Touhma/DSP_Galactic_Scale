using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale.Patches

{
    public static class PatchOnLocalLaserContinuous
    
    {
        [HarmonyTranspiler]   
        [HarmonyPatch(typeof(LocalLaserContinuous),  nameof(LocalLaserContinuous.TickSkillLogic))]
        public static IEnumerable<CodeInstruction> Aim_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var matcher = new CodeMatcher(instructions)
                    .MatchForward(
                        true,
                        new CodeMatch(i => (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) &&
                                           (Convert.ToDouble(i.operand ?? 0.0) == 197.5 ||
                                            Convert.ToDouble(i.operand ?? 0.0) == 198.5)
                    ));
                if (!matcher.IsInvalid)
                {
                    matcher.Repeat(matcher =>
                    {
                        // var mi = AccessTools.Method(typeof(PatchOnDFRelayComponent), nameof(Utils.GetRadiusFromAstroId)).MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
                        matcher.LogILPre();
                        var mi = matcher.GetRadiusFromAstroId();
                        matcher.Advance(1);
                        matcher.InsertAndAdvance(new CodeInstruction(Ldarg_0));
                        matcher.InsertAndAdvance(Utils.LoadField(typeof(LocalLaserContinuous), nameof(LocalLaserContinuous.astroId)));
                        matcher.InsertAndAdvance(new CodeInstruction(Call, mi));
                        matcher.LogILPost(4);
                    });
                    instructions = matcher.InstructionEnumeration();
                    return instructions;
                }
                return instructions;
            }
            catch
            {
                Bootstrap.Logger.LogInfo("PatchOnLocalLaserContinuous.Aim_Transpiler failed");
                return instructions;
            }
        }

    }
}