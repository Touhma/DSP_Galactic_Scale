using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnBuildTool_BlueprintCopy
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BuildTool_BlueprintCopy), nameof(BuildTool_BlueprintCopy.DetermineDivideLine))]
        public static IEnumerable<CodeInstruction> DetermineDivideLine(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);
            var equatorWidthField = AccessTools.Field(typeof(BuildTool_BlueprintCopy), nameof(BuildTool_BlueprintCopy.equatorWidth));
            var matchingLoads = new List<CodeInstruction>();

            foreach (var instruction in code)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand is FieldInfo field && field == equatorWidthField)
                    matchingLoads.Add(instruction);
            }

            if (matchingLoads.Count != 3)
            {
                GS2.Error($"BuildTool_BlueprintCopy.DetermineDivideLine transpiler expected 3 equatorWidth loads, found {matchingLoads.Count}; leaving the original method unchanged.");
                return code;
            }

            var reformMarkScale = matchingLoads[2];
            reformMarkScale.opcode = OpCodes.Pop;
            reformMarkScale.operand = null;
            code.Insert(code.IndexOf(reformMarkScale) + 1, new CodeInstruction(OpCodes.Ldc_I4, 1000));
            return code;
        }
    }
}
