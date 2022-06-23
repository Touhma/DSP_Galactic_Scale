using HarmonyLib;

namespace GalacticScale
{
   
    public partial class PatchOnUISaveGameWindow
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISaveGameWindow), "_OnClose")]
        public static bool UISaveGameWindow_OnClose()
        {
            //GS2.Warn("Enabled Import");

            GS2.SaveOrLoadWindowOpen = false;
            return true;
        }
    }
}