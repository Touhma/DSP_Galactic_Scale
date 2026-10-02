using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnBuildTool_Reform
    {
        public const int MaxBrushSize = 20;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildTool_Reform), MethodType.Constructor)]
        public static void Constructor(ref BuildTool_Reform __instance)
        {
            int size = MaxBrushSize * MaxBrushSize;
            if (__instance.cursorIndices.Length < size) __instance.cursorIndices = new int[size];
            if (__instance.cursorPoints.Length < size) __instance.cursorPoints = new Vector3[size];
        }
    }
}