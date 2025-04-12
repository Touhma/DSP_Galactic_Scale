using HarmonyLib;
using static PlanetModelingManager;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetModelingManager), nameof(PlanetModelingManager.EndPlanetComputeThread))]
        public static bool EndPlanetComputeThread()
        {	
            lock (genPlanetReqList)
            {
                genPlanetReqList.Clear();
            }
            lock (modPlanetReqList)
            {
                modPlanetReqList.Clear();
            }
            lock (fctPlanetReqList)
            {
                fctPlanetReqList.Clear();
            }
            lock (planetComputeThreadFlagLock)
            {
                // Change: If the ThreadFlag is already ended, don't change it to ending
                if (planetComputeThreadFlag == ThreadFlag.Running)
                {
                    planetComputeThreadFlag = ThreadFlag.Ending;
                    GS2.Log("ThreadFlag: Running => Ending");
                }
            }
            return false;
        }
    }
}
