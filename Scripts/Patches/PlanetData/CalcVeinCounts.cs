using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetData
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetData), nameof(PlanetData.SummarizeVeinCountsByFilter))]
        public static bool SummarizeVeinCountsByFilter(PlanetData __instance)
        {
            if (__instance.runtimeVeinGroups == null)
            {
                bool hadData = __instance.data != null;
                
                PlanetModelingManager.Algorithm(__instance).GenerateVeins();
                
                // Clean up heavy data for non-local planets to prevent memory leak
                // Only if we're not the local planet AND we didn't have data before
                if (__instance != GameMain.localPlanet && !hadData && __instance.data != null)
                {
                    // Free the PlanetRawData we just created
                    __instance.data.Free();
                    __instance.data = null;
                    
                    // Free aux data too
                    if (__instance.aux != null)
                    {
                        __instance.aux.Free();
                        __instance.aux = null;
                    }
                }
            }
            return true;
        }
    }
}