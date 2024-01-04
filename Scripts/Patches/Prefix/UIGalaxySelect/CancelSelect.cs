using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), nameof(UIGalaxySelect.CancelSelect))]
        public static bool CancelSelect(UIGalaxySelect __instance)
        {
            SystemDisplay.OnBackClick(__instance);
            return false;
        }
    }
}