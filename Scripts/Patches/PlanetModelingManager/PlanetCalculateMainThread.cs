using HarmonyLib;
using static PlanetModelingManager;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), "PlanetCalculateThreadMain")]
        public static bool PlanetCalculateThreadMain()
        {
            Modeler.Calculate();
            GS2.Log("Loop end. ThreadFlag: " + planetCalculateThreadFlag);
            return false;
        }
    }
}