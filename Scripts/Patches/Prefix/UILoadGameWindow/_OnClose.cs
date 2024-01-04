using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnUILoadGameWindow
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILoadGameWindow), nameof(UILoadGameWindow._OnClose))]
        public static bool UILoadGameWindow_OnClose()
        {
            //GS3.Warn("Enabled Import");

            GS3.SaveOrLoadWindowOpen = false;
            return true;
        }
    }
}