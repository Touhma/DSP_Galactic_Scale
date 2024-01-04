using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnUILoadGameWindow
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILoadGameWindow), nameof(UILoadGameWindow.LoadSelectedGame))]
        public static bool UILoadGameWindow_LoadSelectedGame()
        {
            //GS3.Warn("Enabled Import");
            GS3.SaveOrLoadWindowOpen = false;
            return true;
        }
    }
}