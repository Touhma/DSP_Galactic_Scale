using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchUIAdvisorTip
    {
        [HarmonyPrefix, HarmonyPatch(typeof(UIAdvisorTip), "RunAdvisorTip")]
        public static bool RunAdvisorTip()
        {
            if (GS2.tutorialsOff) return false;
            return true;
        }
    }
}