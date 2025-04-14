using HarmonyLib;

namespace GalacticScale
{
    // ReSharper disable once InconsistentNaming
    public partial class PatchOnPlayerMove_Sail
    {
        [HarmonyPostfix, HarmonyPatch(typeof(PlayerMove_Sail), "GameTick")]
        public static void GameTick_Postfix(ref PlayerMove_Sail __instance)
        {
            // Check if fast travel was active but is now finished
            if (GS2.fastTravelTargetPlanet != null && !__instance.fastTravelling)
            {
                GS2.Log($"Fast travel to {GS2.fastTravelTargetPlanet.name} finished. Clearing flag.");
                GS2.fastTravelTargetPlanet = null; // Clear the flag
            }
        }
    }
}