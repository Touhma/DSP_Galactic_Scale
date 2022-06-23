using HarmonyLib;

namespace GalacticScale
{
   
    public partial class PatchOnUISaveGameWindow
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISaveGameWindow), "_OnOpen")]
        public static bool UISaveGameWindow_OnOpen()
        {
            //GS2.Warn("Disabled Import");

            GS2.SaveOrLoadWindowOpen = true;
            return true;
        }
    }
}