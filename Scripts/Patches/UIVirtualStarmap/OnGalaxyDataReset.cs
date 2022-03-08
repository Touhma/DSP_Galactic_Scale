using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnUIVirtualStarmap
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIVirtualStarmap), "OnGalaxyDataReset")]
        public static bool OnGalaxyDataReset(ref UIVirtualStarmap __instance)
        {
            return SystemDisplay.OnGalaxyDataReset(__instance);
        }
    }
}