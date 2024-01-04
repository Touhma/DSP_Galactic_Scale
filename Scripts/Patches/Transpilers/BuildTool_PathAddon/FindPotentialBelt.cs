using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;


namespace GalacticScale.Patches
{
    public static partial class PatchOnBuildTool_PathAddon
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BuildTool_Addon), nameof(BuildTool_Addon.FindPotentialBelt))]
        public static IEnumerable<CodeInstruction> FindPotentialBeltTranspiler(
            IEnumerable<CodeInstruction> instructions)
        {
            instructions = new CodeMatcher(instructions).MatchForward(true,
                    new CodeMatch(i => i.opcode == Ldloc_S),
                    new CodeMatch(i => i.opcode == Ldloc_S),
                    new CodeMatch(i => i.opcode == Ldelem && i.operand.ToString().Contains("UnityEngine.Vector3")),
                    new CodeMatch(i => i.opcode == Ldc_R4 && (float)i.operand == 2f))
                .SetInstructionAndAdvance(new CodeInstruction(Call,
                    typeof(Utils).GetMethod(nameof(Utils.GetPlanetSizeRatio2))))
                .InstructionEnumeration();

            return instructions;
        }
    }
}