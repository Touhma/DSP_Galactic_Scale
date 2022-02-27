using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnBuildTool_BlueprintPaste
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "segment", MethodType.Getter)]
        public static bool get_segment(ref BuildTool_BlueprintPaste __instance, ref int __result)
        {
            if (__instance.planet?.aux?.activeGrid != null)
            {
                __result = __instance.planet.aux.activeGrid.segment;
                return false;
            }

            GS2.Warn($"ActiveGrid not found for planet {__instance.planet?.name}");
            __result = 200;
            return false;
        }
    }
}