using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnUILoadGameWindow
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILoadGameWindow), "LoadSelectedGame")]
        public static bool UILoadGameWindow_LoadSelectedGame()
        {
            //GS2.Warn("Enabled Import");
            GS2.SaveOrLoadWindowOpen = false;
            return true;
        }
    }
}