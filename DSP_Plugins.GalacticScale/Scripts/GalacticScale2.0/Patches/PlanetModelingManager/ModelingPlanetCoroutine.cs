using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), "ModelingPlanetCoroutine")]
        public static bool ModelingPlanetCoroutine()
        {
            Modeler.ModelingCoroutine();
            return false;
        }
    }
}