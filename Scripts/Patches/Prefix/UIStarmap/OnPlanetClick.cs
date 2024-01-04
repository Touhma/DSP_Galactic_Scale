using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIStarmapPlanet
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarmap), nameof(UIStarmap.OnPlanetClick))]
        public static bool OnPlanetClick(ref UIStarmapPlanet ___focusPlanet, UIStarmapPlanet planet)
        {
            var id = planet.planet.id;
            if (planet.planet.scale < 1 && planet.planet.scale > 0) return false;
            return true;
        }
    }
}