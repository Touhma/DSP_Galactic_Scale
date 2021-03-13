using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [HarmonyPatch(typeof(UIStarmapPlanet))]
    public class PatchOnUIStarmapPlanet {



        [HarmonyPostfix]
        [HarmonyPatch("_OnInit")]
        public static void _OnInit(ref UIStarmapPlanet __instance, LineRenderer ___orbitRenderer, Material ___orbitMat, Material ___planetMat) {
            ___orbitRenderer.transform.localScale = new Vector3((__instance.planet.orbitRadius * 10f)* 0.75f ,(__instance.planet.orbitRadius * 10f) ,(__instance.planet.orbitRadius * 10f)* 1.25f);

        }
        
        [HarmonyPostfix]
        [HarmonyPatch("_OnCreate")]
        public static void _OnCreate(ref UIStarmapPlanet __instance, LineRenderer ___orbitRenderer, Material ___orbitMat, Material ___planetMat) {
            for (int index = 0; index < 60; ++index)
            {
                float f = (float) ((double) index * (Math.PI / 180.0) * 6.0);
                Vector3 position = new Vector3(Mathf.Cos(f) * 0.75f , 0.0f, Mathf.Sin(f) * 1.25f);
                ___orbitRenderer.SetPosition(index, position);
            }
        }
    }
}