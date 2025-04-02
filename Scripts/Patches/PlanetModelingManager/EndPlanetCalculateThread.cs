using System.Collections.Generic;
using HarmonyLib;
using static PlanetModelingManager;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetModelingManager), nameof(PlanetModelingManager.EndPlanetScanThread))]
        public static bool EndPlanetScanThread()
        {
            Queue<PlanetData> queue = PlanetModelingManager.scnPlanetReqList;
            lock (queue)
            {
                PlanetModelingManager.scnPlanetReqList.Clear();
            }
            PlanetModelingManager.ThreadFlagLock threadFlagLock = PlanetModelingManager.planetScanThreadFlagLock;
            lock (threadFlagLock)
            {
                PlanetModelingManager.planetScanThreadFlag = PlanetModelingManager.ThreadFlag.Ending;
            }
            return false;
        }
    }
}
