using HarmonyLib;
using NebulaCompatibility;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "EnterGame")]
        public static void EnterGame(ref GameDesc ___gameDesc)
        {
            // GS2.Warn("Entergame...");
            if (GS2.Config.SkipPrologue && !NebulaCompat.IsMultiplayerActive) DSPGame.StartGameSkipPrologue(___gameDesc);
        }
    }
}