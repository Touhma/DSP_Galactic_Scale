using HarmonyLib;
using static PlanetModelingManager;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager),nameof(PlanetModelingManager.PlanetScanThreadMain))]
        public static bool PlanetScanThreadMain()
        {
            GS2.Warn("Loop start. ThreadFlag: " + planetScanThreadFlag);
            Modeler.Scan();
            GS2.Warn("Loop end. ThreadFlag: " + planetScanThreadFlag);
            return false;
        }
    }
}
