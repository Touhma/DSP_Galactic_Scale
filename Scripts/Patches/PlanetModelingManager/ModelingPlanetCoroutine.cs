using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), "ModelingPlanetCoroutine")]
        public static bool ModelingPlanetCoroutine()
        {
            if (GS2.IsMenuDemo || GS2.Vanilla) return true;
            // GS2.Log("ModelingPlanetCoroutine");
            Modeler.ModelingCoroutine();
            return false;
        }
    }
}