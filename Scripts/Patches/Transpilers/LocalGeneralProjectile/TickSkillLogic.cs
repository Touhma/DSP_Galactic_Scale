using System;
using System.Collections.Generic;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    public static class PatchOnLocalGeneralProjectile
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(LocalGeneralProjectile), nameof(LocalGeneralProjectile.TickSkillLogic))] //225f 212f
        public static IEnumerable<CodeInstruction> Fix39000(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i =>
                    {
                        return (i.opcode == Ldc_R4) &&
                               (
                                   Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 39006.25) < 1
                               );
                    })
                );
            if (matcher.IsInvalid)
            {
                GS2.Warn("Transpiler LocalGeneralProjectile.TickSkillLogic fail!\nCan't find constant 39006.25");
                return instructions;
            }
            matcher.Repeat(matcher =>
            {
                matcher.InsertAndAdvance(new CodeInstruction(Ldarg_0));
                matcher.InsertAndAdvance(new CodeInstruction(Utils.LoadField(typeof(LocalGeneralProjectile),
                    nameof(LocalGeneralProjectile.astroId))));
                matcher.SetInstruction(new CodeInstruction(Call, matcher.GetSquareRadiusFromAstroFactoryId()));
            });

            return matcher.InstructionEnumeration();
        }
    }
}
