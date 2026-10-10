using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetData), nameof(PlanetData.RegenerateName))]
        public static bool RegenerateName_KeepGSName(PlanetData __instance, bool notifychange)
        {
            if (GS2.gsPlanets == null || !GS2.gsPlanets.TryGetValue(__instance.id, out var gsPlanet)) return true;
            if (gsPlanet == null || string.IsNullOrEmpty(gsPlanet.Name)) return true;
            __instance.name = gsPlanet.Name;
            if (notifychange && string.IsNullOrEmpty(__instance.overrideName)) __instance.galaxy.NotifyAstroNameChange(__instance.astroId);
            return false;
        }
    }
}