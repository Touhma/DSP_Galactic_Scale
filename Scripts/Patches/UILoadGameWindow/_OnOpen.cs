using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnUILoadGameWindow
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILoadGameWindow), "_OnOpen")]
        public static bool UILoadGameWindow_OnOpen()
        {
            GS2.SaveOrLoadWindowOpen = true; // Prevents GSSettings getting overwritten when loading a save for purposes of displaying thumbnail
            return true;
        }

    }
}