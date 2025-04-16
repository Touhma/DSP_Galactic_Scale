using System.Collections.Generic;
using System.Threading;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetModelingManager), "Start")]
        public static bool Start()
        {
            PlanetModelingManager.genPlanetReqList = new Queue<PlanetData>(100);
            PlanetModelingManager.modPlanetReqList = new Queue<PlanetData>(100);
            PlanetModelingManager.fctPlanetReqList = new Queue<PlanetData>(100);
            PlanetModelingManager.scnPlanetReqList = new Queue<PlanetData>(100);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), nameof(PlanetModelingManager.StartPlanetScanThread))]
        public static bool StartPlanetScanThread_Prefix()
        {
            if (PlanetModelingManager.scnPlanetReqList == null)
            {
                PlanetModelingManager.scnPlanetReqList = new Queue<PlanetData>(100);
            }
            lock (PlanetModelingManager.planetScanThreadFlagLock)
            {
                GS2.Log("ThreadFlag: " + PlanetModelingManager.planetScanThreadFlag);
                if (PlanetModelingManager.planetScanThreadFlag == PlanetModelingManager.ThreadFlag.Ending)
                {
                    GS2.Warn("PlanetScanThread is in Ending state. Starting a new thread");
                    PlanetModelingManager.planetScanThread = new Thread(new ThreadStart(PlanetModelingManager.PlanetScanThreadMain));
                    PlanetModelingManager.planetScanThread.Name = "Planet Scan Thread";
                    PlanetModelingManager.planetScanThread.Start();
                    PlanetModelingManager.planetScanThreadFlag = PlanetModelingManager.ThreadFlag.Running;
                }
            }
            return true;
        }
    }
}