using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    public static class PatchOnLocalLaserContinuous
    
    {
        [HarmonyTranspiler]   
        [HarmonyPatch(typeof(LocalLaserContinuous),  nameof(LocalLaserContinuous.TickSkillLogic))]
        public static IEnumerable<CodeInstruction> Aim_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var code = new List<CodeInstruction>(instructions);
                var matcher = new CodeMatcher(code)
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
                        var mi = matcher.GetRadiusFromAstroId();
                        matcher.Advance(1);
                        matcher.InsertAndAdvance(new CodeInstruction(Ldarg_0));
                        matcher.InsertAndAdvance(Utils.LoadField(typeof(LocalLaserContinuous), nameof(LocalLaserContinuous.astroId)));
                        matcher.InsertAndAdvance(new CodeInstruction(Call, mi));
                    });
                    code = new List<CodeInstruction>(matcher.InstructionEnumeration());
                }

                var damageMethod = AccessTools.Method(typeof(SkillSystem), nameof(SkillSystem.DamageGroundObjectByLocalCaster));
                var safeDamageMethod = AccessTools.Method(typeof(PatchOnLocalLaserContinuous), nameof(SafeDamageGroundObjectByLocalCaster));
                foreach (var instruction in code)
                {
                    if (instruction.Calls(damageMethod))
                    {
                        instruction.opcode = Call;
                        instruction.operand = safeDamageMethod;
                    }
                }

                return code;
            }
            catch
            {
                Bootstrap.Logger.LogInfo("PatchOnLocalLaserContinuous.Aim_Transpiler failed");
                return instructions;
            }
        }

        public static ref CombatStat SafeDamageGroundObjectByLocalCaster(
            SkillSystem skillSystem,
            PlanetFactory factory,
            int damage,
            int slice,
            ref SkillTargetLocal target,
            ref SkillTargetLocal caster)
        {
            if (factory == null)
            {
                CombatStat.temp.Reset();
                return ref CombatStat.temp;
            }

            return ref skillSystem.DamageGroundObjectByLocalCaster(factory, damage, slice, ref target, ref caster);
        }

    }
}