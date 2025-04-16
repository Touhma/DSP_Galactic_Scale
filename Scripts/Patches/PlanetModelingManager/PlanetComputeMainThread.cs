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
            GS2.Warn("Loop start. ThreadFlag: " + planetComputeThreadFlag);
            Modeler.Compute();
            GS2.Warn("Loop end. ThreadFlag: " + planetComputeThreadFlag);
            return false;
        }
    }
}
