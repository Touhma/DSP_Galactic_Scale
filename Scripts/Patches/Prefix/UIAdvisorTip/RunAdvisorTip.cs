using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIAdvisorTip
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIAdvisorTip), nameof(UIAdvisorTip.RunAdvisorTip))]
        public static bool RunAdvisorTip()
        {
            if (GS3.Config.SkipTutorials) return false;

            return true;
        }
    }
}