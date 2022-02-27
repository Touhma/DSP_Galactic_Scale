using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnBuildTool_BlueprintPaste
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "CheckBuildConditions")]
        public static IEnumerable<CodeInstruction> CheckBuildConditions(IEnumerable<CodeInstruction> instructions)
        {
            instructions = new CodeMatcher(instructions).MatchForward(true, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldflda), new CodeMatch(OpCodes.Call), new CodeMatch(OpCodes.Ldc_R4, 200.2f)).SetInstruction(Transpilers.EmitDelegate<Func<float>>(() =>
            {
                var planet = GameMain.localPlanet;
                return planet == null ? 200.2f : planet.realRadius + 0.2f;
            })).InstructionEnumeration();

            return instructions;
        }
    }
}