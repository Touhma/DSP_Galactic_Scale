using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    public class PlanetCountTransipler
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlayerAction_Combat), nameof(PlayerAction_Combat.CalculateBombCurvePoints))]
        [HarmonyPatch(typeof(PlayerAction_Combat), nameof(PlayerAction_Combat.CanAttackHatredTarget))]
        [HarmonyPatch(typeof(PlayerAction_Combat), nameof(PlayerAction_Combat.SearchTarget))]
        [HarmonyPatch(typeof(PlayerAction_Combat), nameof(PlayerAction_Combat.SearchTargetManually))]
        [HarmonyPatch(typeof(Bomb_Explosive), nameof(Bomb_Explosive.TickSkillLogic))]
        [HarmonyPatch(typeof(Bomb_EMCapsule), nameof(Bomb_EMCapsule.TickSkillLogic))]
        [HarmonyPatch(typeof(Bomb_Liquid), nameof(Bomb_Liquid.TickSkillLogic))]
        public static IEnumerable<CodeInstruction> FixPlanetCountLoop_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
        {
            try
            {
                // Change: for (int i = starAstroId + 1; i <= starAstroId + 8; i++)
                // To:     for (int i = starAstroId + 1; i <= starAstroId + GameMain.galaxy.StarById(starAstroId / 100).planetCount; i++)
                var matcher = new CodeMatcher(instructions)
                        .MatchForward(true,
                            new CodeMatch(Ldloc_S),
                            new CodeMatch(Ldc_I4_1),
                            new CodeMatch(Add),
                            new CodeMatch(Stloc_S),
                            new CodeMatch(i => i.IsLdloc()),
                            new CodeMatch(i => i.IsLdloc()),
                            new CodeMatch(Ldc_I4_8), // replace target 
                            new CodeMatch(Add),      // replace target
                            new CodeMatch(i => i.opcode == Ble || i.opcode == Blt))
                        .Advance(-2)
                        .RemoveInstructions(2)
                        .Insert(Transpilers.EmitDelegate<Func<int, int>>(starAstroId =>
                        {
                            return starAstroId + GameMain.galaxy.StarById(starAstroId / 100)?.planetCount ?? starAstroId + 8;
                        }));
                return matcher.InstructionEnumeration();
            }
            catch (Exception e)
            {
                GS2.Warn("FixPlanetCountLoop_Transpiler failed!" + __originalMethod.Name);
                GS2.Warn(e.ToString());
                return instructions;
            }
        }
    }
}
