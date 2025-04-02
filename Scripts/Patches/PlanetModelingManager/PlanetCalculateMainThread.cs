using HarmonyLib;
using static PlanetModelingManager;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager),nameof(PlanetModelingManager.PlanetScanThreadMain))]
        public static bool PlanetCalculateThreadMain()
        {
            Modeler.Calculate();
            return false;
        }
    }
}