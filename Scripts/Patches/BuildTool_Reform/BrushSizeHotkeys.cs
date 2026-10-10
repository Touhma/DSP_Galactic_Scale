using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnBuildTool_Reform
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Reform), nameof(BuildTool_Reform.UpdateRaycastAndReform))]
        public static void StoreBrushSizeForHotkeys(BuildTool_Reform __instance, out int __state)
        {
            __state = __instance.brushSize;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildTool_Reform), nameof(BuildTool_Reform.UpdateRaycastAndReform))]
        public static void ExtendBrushSizeHotkeys(BuildTool_Reform __instance, int __state)
        {
            if (VFInput._cursorPlusKey.onDown && __state >= 10)
            {
                __instance.brushSize = Mathf.Min(__state + 1, MaxBrushSize);
            }
            else if (VFInput._cursorMinusKey.onDown && __state > 10)
            {
                __instance.brushSize = Mathf.Max(__state - 1, 10);
            }
        }
    }
}
