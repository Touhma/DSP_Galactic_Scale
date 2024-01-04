using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnUILoadGameWindow
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILoadGameWindow), nameof(UILoadGameWindow._OnOpen))]
        public static bool UILoadGameWindow_OnOpen()
        {
            GS3.SaveOrLoadWindowOpen = true; // Prevents GSSettings getting overwritten when loading a save for purposes of displaying thumbnail
            return true;
        }
    }
}