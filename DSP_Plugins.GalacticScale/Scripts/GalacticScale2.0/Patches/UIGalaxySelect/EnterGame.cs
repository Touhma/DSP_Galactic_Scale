using HarmonyLib;

namespace GalacticScale {
    public partial class PatchOnUIGalaxySelect {
        [HarmonyPostfix, HarmonyPatch(typeof(UIGalaxySelect), "EnterGame")]
        public static void EnterGame(ref GameDesc ___gameDesc) {
            if (GS2.SkipPrologue) {
                DSPGame.StartGameSkipPrologue(___gameDesc);
            }
        }
    }
}