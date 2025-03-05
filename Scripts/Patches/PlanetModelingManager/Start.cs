using System.Collections.Generic;
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
            PlanetModelingManager.calPlanetReqList = new Queue<PlanetData>(100);
            GS2.Log($"ThreadFlag: {PlanetModelingManager.planetComputeThreadFlag},{PlanetModelingManager.planetCalculateThreadFlag} => Running");
            return true;
        }
    }
}