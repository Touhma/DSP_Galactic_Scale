using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;


namespace GalacticScale
{
    public static partial class PatchOnBuildTool_PathAddon
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BuildTool_Addon), nameof(BuildTool_Addon.FindPotentialBelt))]
        public static IEnumerable<CodeInstruction> FindPotentialBeltTranspiler(
            IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions).MatchForward(true,
                    new CodeMatch(i => i.opcode == Ldloc_S),
                    new CodeMatch(i => i.opcode == Ldloc_S),
                    new CodeMatch(i => i.opcode == Ldelem && i.operand.ToString().Contains("UnityEngine.Vector3")),
                    new CodeMatch(i => i.opcode == Ldc_R4 && (float)i.operand == 2f));
            if (matcher.IsInvalid)
            {
                GS2.Error("BuildTool_Addon.FindPotentialBelt transpiler: Vector3/2f pattern not found (game update changed the method?). Returning original code - addon belt snapping will assume a radius-200 planet.");
                return instructions;
            }

            return matcher
                .SetInstructionAndAdvance(new CodeInstruction(Call,
                    typeof(Utils).GetMethod(nameof(Utils.GetPlanetSizeRatio2))))
                .InstructionEnumeration();
        }
    }
}