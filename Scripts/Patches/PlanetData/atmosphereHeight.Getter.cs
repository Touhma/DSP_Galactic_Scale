using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetData), "atmosphereHeight", MethodType.Getter)]
        public static bool atmosphereHeightGetter(ref PlanetData __instance, ref float __result)
        {
            __result = 0f;
            if (GS2.Vanilla || GS2.IsMenuDemo)
            {
                if (!(__instance.atmosMaterial == null))
                {
                    __result = __instance.atmosMaterial.GetVector("_PlanetRadius").z - __instance.realRadius;
                    return false;
                }
                return false;
            }
            if (VFInput.shift)
            {
                // GS2.Warn($"Overriding AtmosHeight to {__instance.realRadius+50f}");
                __result = __instance.realRadius+50f;
                return false;
            }
            if (!(__instance.atmosMaterial == null))
                {
                    // GSPlanet gsPlanet = GS2.GetGSPlanet(__instance);
                    // GS2.Warn($"Z Component of atmosMat planetRadius =  {__instance.atmosMaterial.GetVector("_PlanetRadius").z} for theme {gsPlanet.GsTheme.Name} AlgoID: {gsPlanet.GsTheme.Algo} Vector:{__instance.atmosMaterial.GetVector("_PlanetRadius")}");
                    // if (__instance.type == EPlanetType.Gas) __result = __instance.atmosMaterial.GetVector("_PlanetRadius").z - __instance.realRadius;
                    // else __result = __instance.atmosMaterial.GetVector("_PlanetRadius").z - __instance.realRadius;
                    __result = __instance.temperatureBias;
                }

               // GS2.Warn($"Atmosphere Height Set to {__result} for planet:{__instance.name}");
            
            return false;
        }
    }
}