using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIAdvisorTip
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIAdvisorTip), nameof(UIAdvisorTip.PlayAdvisorTip))]
        public static bool PlayAdvisorTip()
        {
            if (GS3.Config.SkipTutorials) return false;

            return true;
        }
    }
}