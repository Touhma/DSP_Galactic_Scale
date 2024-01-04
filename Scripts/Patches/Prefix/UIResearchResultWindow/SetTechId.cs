using HarmonyLib;

namespace GalacticScale.Patches
{
    public class PatchOnUIResearchResultsWindow
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIResearchResultWindow), nameof(UIResearchResultWindow.SetTechId))]
        public static bool SetTechId()
        {
            if (GS3.Config.SkipTutorials) return false;

            return true;
        }
    }
}