using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetData), "realRadius", MethodType.Getter)]
        public static bool RealRadiusGetter(ref PlanetData __instance, ref float __result)
        {
            if (__instance.type == EPlanetType.Gas) return true;

            __result = __instance.radius;
            return false;
        }
    }
}