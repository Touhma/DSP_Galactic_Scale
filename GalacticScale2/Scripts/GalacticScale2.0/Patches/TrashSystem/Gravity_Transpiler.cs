using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace GalacticScale
{
    [HarmonyPatch(typeof(TrashSystem))]
    class PatchOnTrashSystem
    {
        [HarmonyTranspiler]
        [HarmonyPatch("Gravity")]
        public static IEnumerable<CodeInstruction> TrashSystem_Gravity_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            instructions = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Add),
                    new CodeMatch(OpCodes.Stloc_2),
                    new CodeMatch(OpCodes.Ldloc_S),
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(OpCodes.Add),
                    new CodeMatch(OpCodes.Stloc_S),
                    new CodeMatch(OpCodes.Ldloc_S),
                    new CodeMatch(OpCodes.Ldloc_1),
                    new CodeMatch(OpCodes.Ldc_I4_8))
                .SetAndAdvance(OpCodes.Ldarg_S, 6)
                .Insert(HarmonyLib.Transpilers.EmitDelegate<Func<int, int>>(localPlanetId =>
                {
                    PlanetData planet = GameMain.galaxy.PlanetById(localPlanetId);
                    return (planet == null) ? 8 : planet.star.planetCount;
                }))
                .InstructionEnumeration();

            return instructions;
        }
    }
}
