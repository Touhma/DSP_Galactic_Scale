using HarmonyLib;
using NebulaAPI;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "EnterGame")]
        public static void EnterGame(ref GameDesc ___gameDesc)
        {
            if (GS2.Config.SkipPrologue && NebulaModAPI.MultiplayerSession == null) DSPGame.StartGameSkipPrologue(___gameDesc);
        }
    }
}