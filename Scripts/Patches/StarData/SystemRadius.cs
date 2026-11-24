using HarmonyLib;

namespace GalacticScale
{
    public static class PatchOnStarData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StarData), nameof(StarData.systemRadius), MethodType.Getter)]
        public static bool SystemRadius(StarData __instance, ref float __result)
        {
            if (__instance.galaxy == null || __instance.galaxy != GS2.galaxy ||
                !GS2.gsStars.TryGetValue(__instance.id, out var star) || star.PlanetCount == 0)
                return true;

            __result = star.OutermostOrbitRadius;
            return false;
        }
    }
}
