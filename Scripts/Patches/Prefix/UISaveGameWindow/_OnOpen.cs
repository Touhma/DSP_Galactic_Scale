using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnUISaveGameWindow
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISaveGameWindow), nameof(UISaveGameWindow._OnOpen))]
        public static bool UISaveGameWindow_OnOpen()
        {
            //GS3.Warn("Disabled Import");

            GS3.SaveOrLoadWindowOpen = true;
            return true;
        }
    }
}