using System.Threading;
using HarmonyLib;
using static PlanetModelingManager;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), nameof(PlanetModelingManager.PlanetComputeThreadMain))]
        public static bool PlanetComputeThreadMain(ref ThreadFlag ___planetComputeThreadFlag, ref ThreadFlagLock ___planetComputeThreadFlagLock, ref Thread ___planetComputeThread)
        {
            Modeler.Compute(ref ___planetComputeThreadFlag, ref ___planetComputeThreadFlagLock, ref ___planetComputeThread);
            return false;
        }
    }
}