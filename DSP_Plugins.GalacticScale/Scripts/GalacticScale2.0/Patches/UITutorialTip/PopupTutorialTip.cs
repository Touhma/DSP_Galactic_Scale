using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnUITutorialTip
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UITutorialTip), "PopupTutorialTip")]
        public static bool PopupTutorialTip()
        {
            if (GS2.tutorialsOff) return false;

            return true;
        }
    }
}