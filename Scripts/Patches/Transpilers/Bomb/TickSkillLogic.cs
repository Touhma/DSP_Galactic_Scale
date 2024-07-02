using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    public static class PatchOnBomb    
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Bomb_Explosive),  nameof(Bomb_Explosive.TickSkillLogic))]
        [HarmonyPatch(typeof(Bomb_Explosive), nameof(Bomb_Explosive.BombFactoryObjects))]
        [HarmonyPatch(typeof(Bomb_EMCapsule), nameof(Bomb_EMCapsule.TickSkillLogic))]
        [HarmonyPatch(typeof(Bomb_EMCapsule), nameof(Bomb_EMCapsule.BombFactoryObjects))]
        [HarmonyPatch(typeof(Bomb_Liquid), nameof(Bomb_Explosive.TickSkillLogic))]
        public static IEnumerable<CodeInstruction> FixRadius_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
        {
            try
            {
                var matcher = new CodeMatcher(instructions)
                    .MatchForward(
                        true,
                        new CodeMatch(i => (i.opcode == Ldc_R4 &&
                            (i.OperandIs(200f) || i.OperandIs(250f) || i.OperandIs(270f)))
                    ));
                var mi = matcher.GetRadiusFromAstroId();
                matcher.Repeat(matcher =>
                {
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(new CodeInstruction(Ldarg_0));
                    matcher.InsertAndAdvance(Utils.LoadField(__originalMethod.DeclaringType, "nearPlanetAstroId"));
                    matcher.InsertAndAdvance(new CodeInstruction(Call, mi));
                });
                return matcher.InstructionEnumeration();
            }
            catch (Exception e)
            {
                GS2.Warn("FixRadius_Transpiler failed!" + __originalMethod.Name);
                GS2.Warn(e.ToString());
                return instructions;
            }
        }
    }
}
