using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnBuildTool_Reform
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BuildTool_Reform), "PrepareFlattenPoints")]
        [HarmonyPatch(typeof(BuildTool_Reform), "PrepareRestorePoints")]
        public static IEnumerable<CodeInstruction> RaiseClamp(IEnumerable<CodeInstruction> instructions)
        {
            var ins = instructions.ToList();
            var minMethod = AccessTools.Method(typeof(System.Math), nameof(System.Math.Min), new[] { typeof(int), typeof(int) });
            for (int i = 0; i < ins.Count; i++)
            {
                if (ins[i].opcode == OpCodes.Ldc_I4_S && ins[i].operand is sbyte v && v == 10
                    && i + 1 < ins.Count && ins[i + 1].Calls(minMethod))
                {
                    ins[i].operand = (sbyte)MaxBrushSize;
                }
            }
            return ins;
        }
    }
}