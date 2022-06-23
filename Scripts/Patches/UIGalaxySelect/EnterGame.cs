using HarmonyLib;
using NebulaCompatibility;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), "EnterGame")]
        public static bool EnterGame(ref GameDesc ___gameDesc)
        {
            // GS2.Warn("Entergame...");
            UIRoot.instance.uiGame.planetDetail.gameObject.SetActive(false);
            UIRoot.instance.uiGame.starDetail.gameObject.SetActive(false);
            if (!GS2.Vanilla && !NebulaCompat.IsMultiplayerActive) SystemDisplay.ResetView();
            if (GS2.Config.SkipPrologue && !NebulaCompat.IsMultiplayerActive)
            {
                GS2.Warn("Starting Game, Skipping Prologue.");
                DSPGame.StartGameSkipPrologue(___gameDesc);
            }
            else if (!NebulaCompat.IsMultiplayerActive)
            {
                DSPGame.StartGame(___gameDesc);
            }

            return false;
        }
    }
}