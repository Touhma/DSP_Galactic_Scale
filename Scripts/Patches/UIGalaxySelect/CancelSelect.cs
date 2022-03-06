using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), "CancelSelect")]
        public static bool CancelSelect(UIGalaxySelect __instance)
        {
            SystemDisplay.OnBackClick(__instance);
            return false;
        }
    }
}