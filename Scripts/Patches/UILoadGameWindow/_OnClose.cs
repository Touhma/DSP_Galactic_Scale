using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnUILoadGameWindow
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILoadGameWindow), "_OnClose")]
        public static bool UILoadGameWindow_OnClose()
        {
            //GS2.Warn("Enabled Import");

            GS2.SaveOrLoadWindowOpen = false;
            return true;
        }
    }
}