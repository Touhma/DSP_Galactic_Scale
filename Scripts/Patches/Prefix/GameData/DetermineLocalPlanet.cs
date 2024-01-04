using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnGameData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), nameof(GameData.DetermineLocalPlanet))]
        public static bool DetermineLocalPlanet(ref bool __result, ref GameData __instance)
        {
            if (GS3.IsMenuDemo) return true;
            if (__instance.mainPlayer == null)
            {
                GS3.Error("MainPlayer Null");
                return false;
            }

            __result = HandleLocalStarPlanets.Update();
            return false;
        }
    }
}