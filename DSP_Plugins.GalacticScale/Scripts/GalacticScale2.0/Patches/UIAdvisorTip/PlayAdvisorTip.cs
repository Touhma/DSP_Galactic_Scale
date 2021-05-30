using HarmonyLib;

namespace GalacticScale
{
    public class PatchUITutorialTip
    {
        [HarmonyPrefix, HarmonyPatch(typeof(UITutorialTip), "PopupTutorialTip")]
        public static bool PopupTutorialTip()
        {
            if (GS2.tutorialsOff) return false;
            return true;
        }
    }
}