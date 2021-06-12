using HarmonyLib;

namespace GalacticScale {
    public partial class PatchOnUIAdvisorTip {
        [HarmonyPrefix, HarmonyPatch(typeof(UIAdvisorTip), "PlayAdvisorTip")]
        public static bool PlayAdvisorTip() {
            if (GS2.tutorialsOff) {
                return false;
            }

            return true;
        }
    }
}