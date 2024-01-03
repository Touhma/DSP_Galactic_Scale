using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace GalacticScale
{
    public class PatchOnCloudSimulator
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CloudSimulator), nameof(CloudSimulator.OnEnable))]
        public static void OnEnable(CloudSimulator __instance)
        {
            // GS2.Log($"CloudSim.OnEnable for {__instance.planet.name}: cloudHeight = {__instance.cloudHeight}, realRadius = {__instance.planet.realRadius}, atmosphereHeight = {__instance.planet.atmosphereHeight} atmosMaterialPlanetRadius = {__instance.planet.atmosMaterial.GetVector("_PlanetRadius").z} nephogram.localScale: {__instance.nephogram.transform.localScale}");
            
        }
    }
}
