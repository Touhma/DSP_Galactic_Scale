using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnUIResearchResultsWindow
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIResearchResultWindow), "SetTechId")]
        public static bool SetTechId()
        {
            if (GS2.Config.SkipTutorials) return false;

            return true;
        }
    }
}