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
        [HarmonyPatch(typeof(CloudSimulator), "OnEnable")]
        public static void OnEnable(CloudSimulator __instance)
        {
            GS2.Warn($"CloudSim.OnEnable for {__instance.planet.name}: cloudHeight = {__instance.cloudHeight}, realRadius = {__instance.planet.realRadius}");
        }
    }
}
