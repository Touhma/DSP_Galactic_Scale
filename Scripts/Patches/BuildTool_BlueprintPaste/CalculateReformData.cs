using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnBuildTool_BlueprintPaste
    {
        // Game 0.10.34 added foundation data to blueprints. BuildTool_BlueprintPaste.CalculateReformData
        // walks the whole previewReform buffer (sized by platformSystem.maxReformCount) while stepping a
        // latitude-band index through platformSystem.reformOffsets with no bounds check.
        //
        // GS2's ComputeMaxReformCount prefix sets maxReformCount from the keyed-LUT total
        // (sum of segCount * 25 * 2), which can exceed the cumulative total reformOffsets was built
        // from, so on GS2 planets the band index runs off the end of the table and the game throws
        // IndexOutOfRangeException on every tick of the paste preview (and disables autosave).
        //
        // Cells beyond the reformOffsets total are always zero and are skipped by the walk, so clamping
        // the read is behavior-preserving: int.MaxValue out of range just stops further band advances.
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "CalculateReformData")]
        public static IEnumerable<CodeInstruction> CalculateReformData(IEnumerable<CodeInstruction> instructions)
        {
            var clampedGet = AccessTools.Method(typeof(PatchOnBuildTool_BlueprintPaste), nameof(ClampedIntArrayGet));
            var patched = 0;
            foreach (var ins in instructions)
            {
                if (ins.opcode == OpCodes.Ldelem_I4)
                {
                    var call = new CodeInstruction(OpCodes.Call, clampedGet);
                    call.labels.AddRange(ins.labels);
                    call.blocks.AddRange(ins.blocks);
                    yield return call;
                    patched++;
                }
                else
                {
                    yield return ins;
                }
            }

            // Zero replacements is not necessarily an error (another mod's transpiler may have
            // replaced the loads first, which is a safe no-op for us) but after a game update it
            // can also mean the method was rewritten - worth a note in the log either way.
            if (patched == 0)
                GS2.Warn("BuildTool_BlueprintPaste.CalculateReformData transpiler: no int-array loads found to clamp (another mod patched them first, or a game update rewrote the method).");
        }

        public static int ClampedIntArrayGet(int[] arr, int idx)
        {
            if (arr != null && idx >= 0 && idx < arr.Length) return arr[idx];
            return int.MaxValue;
        }
    }
}
