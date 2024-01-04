using HarmonyLib;

namespace GalacticScale.Patches
{
    public class PatchOnUITutorialTip
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UITutorialTip), nameof(UITutorialTip.PopupTutorialTip))]
        public static bool PopupTutorialTip()
        {
            if (GS3.Config.SkipTutorials) return false;

            return true;
        }
    }
}