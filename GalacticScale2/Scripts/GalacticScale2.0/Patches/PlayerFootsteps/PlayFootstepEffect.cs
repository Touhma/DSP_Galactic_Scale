using HarmonyLib;

// Fix error when localPlanet not defined
namespace GalacticScale
{
    public partial class PatchOnPlayerFootsteps
    {
        /*
         * theese can get nulled for whatever reason when in a multiplayer session and beeing the client.
         * resetting them solves the issue.
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerFootsteps), "PlayFootstepEffect")]
        public static bool PlayFootstepEffect(ref PlayerFootsteps __instance)
        {
            if (__instance.player == null) __instance.player = GameMain.mainPlayer;

            if (__instance.player.planetData == null) __instance.player.planetData = GameMain.localPlanet;

            if (__instance.player.planetData.ambientDesc == null)
            {
                GS2.Error("Player planetData ambientDesc null");
                return false;
            }

            return true;
        }
    }
}