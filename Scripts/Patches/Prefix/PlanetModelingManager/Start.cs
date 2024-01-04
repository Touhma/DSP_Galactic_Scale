using System.Collections.Generic;
using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetModelingManager), nameof(PlanetModelingManager.Start))]
        public static bool Start()
        {
            PlanetModelingManager.genPlanetReqList = new Queue<PlanetData>(100);
            PlanetModelingManager.modPlanetReqList = new Queue<PlanetData>(100);
            PlanetModelingManager.fctPlanetReqList = new Queue<PlanetData>(100);
            PlanetModelingManager.calPlanetReqList = new Queue<PlanetData>(100);
            return true;
        }
    }
}