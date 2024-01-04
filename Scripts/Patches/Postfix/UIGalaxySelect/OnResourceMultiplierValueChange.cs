using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), nameof(UIGalaxySelect.OnResourceMultiplierValueChange))]
        public static void OnResourceMultiplierValueChange(UIGalaxySelect __instance, float val)
        {
            if (__instance.gameDesc != null) GS3.Config.SetResourceMultiplier(__instance.gameDesc.resourceMultiplier);
        }
    }
}