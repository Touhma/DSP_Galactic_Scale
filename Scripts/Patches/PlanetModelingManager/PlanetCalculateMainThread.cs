using System.Threading;
using HarmonyLib;
using static PlanetModelingManager;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), "PlanetCalculateThreadMain")]
        public static bool PlanetCalculateThreadMain(ref ThreadFlag ___planetCalculateThreadFlag, ref ThreadFlagLock ___planetCalculateThreadFlagLock, ref Thread ___planetCalculateThread)
        {
            Modeler.Calculate();
            return false;
        }
    }
}