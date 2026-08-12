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
            var matcher = new CodeMatcher(instructions).MatchForward(true, new CodeMatch(OpCodes.Ldloc_S), new CodeMatch(OpCodes.Ldflda), new CodeMatch(OpCodes.Call), new CodeMatch(OpCodes.Ldc_R4, 200.2f));
            if (matcher.IsInvalid)
            {
                GS2.Error("BuildTool_BlueprintPaste.CheckBuildConditions transpiler: 200.2f pattern not found (game update changed the method?). Returning original code - blueprint paste height checks will assume a radius-200 planet.");
                return instructions;
            }

            return matcher.SetInstruction(Transpilers.EmitDelegate<Func<float>>(() =>
            {
                return GameMain.localPlanet?.realRadius + 0.2f??200.2f;
                // return planet == null ? 200.2f : planet.realRadius + 0.2f;
            })).InstructionEnumeration();
        }
    }
}