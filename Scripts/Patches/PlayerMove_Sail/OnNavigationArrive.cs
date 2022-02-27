using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlayerMove_Sail
    {
        [HarmonyPatch(typeof(PlayerMove_Fly), "OnNavigationArrive")]
        [HarmonyPrefix]
        public static bool OnNavigationArrive(ref PlayerMove_Fly __instance)
        {
            if (__instance.player.movementState != EMovementState.Fly) return false;

            var planetData = __instance.player.planetData;
            if (planetData == null || planetData.type == EPlanetType.Gas && planetData.scale >= 1) return false;

            __instance.controller.movementStateInFrame = EMovementState.Walk;
            __instance.controller.softLandingTime = 1.2f;
            return false;
        }
    }
}