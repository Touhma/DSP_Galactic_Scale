using HarmonyLib;
using static PlanetModelingManager;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), nameof(PlanetModelingManager.PlanetComputeThreadMain))]
        public static bool PlanetComputeThreadMain()
        {
            Modeler.Compute();
            GS2.Log("Loop end. ThreadFlag: " + planetComputeThreadFlag);
            return false;
        }
    }
}