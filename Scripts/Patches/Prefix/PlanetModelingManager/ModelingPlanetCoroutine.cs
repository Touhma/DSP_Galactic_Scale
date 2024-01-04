using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), nameof(PlanetModelingManager.ModelingPlanetCoroutine))]
        public static bool ModelingPlanetCoroutine()
        {
            if (GS3.IsMenuDemo) return true;
            Modeler.ModelingCoroutine();
            return false;
        }
    }
}