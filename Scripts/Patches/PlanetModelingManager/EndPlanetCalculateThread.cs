using HarmonyLib;
using static PlanetModelingManager;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetModelingManager), "EndPlanetCalculateThread")]
        public static bool EndPlanetCalculateThread()
        {
            lock (planetCalculateThreadFlagLock)
            {
                // Change: If the ThreadFlag is already ended, don't change it to ending
                if (planetCalculateThreadFlag == ThreadFlag.Running)
                {
                    planetCalculateThreadFlag = ThreadFlag.Ending;
                    GS2.Log("ThreadFlag: Running => Ending");
                }
            }
            return false;
        }
    }
}
