using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnUISaveGameWindow
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISaveGameWindow), nameof(UISaveGameWindow._OnClose))]
        public static bool UISaveGameWindow_OnClose()
        {
            //GS3.Warn("Enabled Import");

            GS3.SaveOrLoadWindowOpen = false;
            return true;
        }
    }
}