using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetData), nameof(PlanetData.atmosphereHeight), MethodType.Getter)]
        public static bool atmosphereHeightGetter(ref PlanetData __instance, ref float __result)
        {
            __result = 0f;
            if (GS3.IsMenuDemo)
            {
                if (!(__instance.atmosMaterial == null))
                {
                    __result = __instance.atmosMaterial.GetVector("_PlanetRadius").z - __instance.realRadius;
                    return false;
                }
                return false;
            }
            if (!(__instance.atmosMaterial == null))
            {
                __result = __instance.temperatureBias;
            }
            return false;
        }
    }
}