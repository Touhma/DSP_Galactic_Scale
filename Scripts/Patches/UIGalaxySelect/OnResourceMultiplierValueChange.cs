using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "OnResourceMultiplierValueChange")]
        public static void OnResourceMultiplierValueChange(UIGalaxySelect __instance, float val)
        {
            if (__instance.gameDesc != null) GS2.Config.SetResourceMultiplier(__instance.gameDesc.resourceMultiplier);
        }
    }
}