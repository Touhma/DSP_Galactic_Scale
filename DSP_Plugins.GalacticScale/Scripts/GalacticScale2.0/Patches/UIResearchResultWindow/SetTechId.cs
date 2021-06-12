using HarmonyLib;

namespace GalacticScale {
    public partial class PatchOnUIResearchResultsWindow {
        [HarmonyPrefix, HarmonyPatch(typeof(UIResearchResultWindow), "SetTechId")]
        public static bool SetTechId() {
            if (GS2.tutorialsOff) {
                return false;
            }

            return true;
        }
    }
}