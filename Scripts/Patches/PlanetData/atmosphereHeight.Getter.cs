using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetData), "atmosphereHeight", MethodType.Getter)]
        public static bool atmosphereHeightGetter(ref PlanetData __instance, ref float __result)
        {
            if (GS2.Vanilla || GS2.IsMenuDemo)
                return true;

            __result = __instance.atmosMaterial == null ? 0f : __instance.temperatureBias * __instance.GetAtmosphereScale();
            return false;
        }
    }
}
