using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), "ModelingPlanetCoroutine")]
        public static bool ModelingPlanetCoroutine()
        {
            // GS2.Log("ModelingPlanetCoroutine");
            Modeler.ModelingCoroutine();
            return false;
        }
    }
}