using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "EnterGame")]
        public static void EnterGame(ref GameDesc ___gameDesc)
        {
            if (GS2.Config.SkipPrologue) DSPGame.StartGameSkipPrologue(___gameDesc);
        }
    }
}