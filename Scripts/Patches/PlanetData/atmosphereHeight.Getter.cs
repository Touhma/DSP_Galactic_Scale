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
                if (!(__instance.atmosMaterial == null))
                {
                    if (__instance.type == EPlanetType.Gas) __result = __instance.atmosMaterial.GetVector("_PlanetRadius").z - 800;
                    else __result = __instance.atmosMaterial.GetVector("_PlanetRadius").z - 200;
                }

               
            
            return false;
        }
    }
}