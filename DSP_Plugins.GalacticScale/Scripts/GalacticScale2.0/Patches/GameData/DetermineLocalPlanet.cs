using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "DetermineLocalPlanet")]
        public static bool DetermineLocalPlanet(ref bool __result, ref GameData __instance)
        {
            if (GS2.Vanilla || GS2.IsMenuDemo) return true;
            if (__instance.mainPlayer == null)
            {
                GS2.Error("MainPlayer Null");
                return false;
            }

            __result = HandleLocalStarPlanets.Update();
            return false;
        }
    }
}