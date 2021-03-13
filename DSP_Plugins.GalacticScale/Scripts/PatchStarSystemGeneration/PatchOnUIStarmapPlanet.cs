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
            ___orbitMat.SetFloat("_Radius", __instance.planet.orbitRadius * 10f * 1.25f);

        }
        
        [HarmonyPostfix]
        [HarmonyPatch("_OnCreate")]
        public static void _OnCreate(ref UIStarmapPlanet __instance, LineRenderer ___orbitRenderer, Material ___orbitMat, Material ___planetMat) {
            if (___orbitRenderer.positionCount != 60)
                ___orbitRenderer.positionCount = 60;
            for (int index = 0; index < 60; ++index)
            {
                float f = (float) ((double) index * (Math.PI / 180.0) * 6.0);
                Vector3 position = new Vector3(Mathf.Cos(f) * 0.75f , 0.0f, Mathf.Sin(f) * 1.25f);
                ___orbitRenderer.SetPosition(index, position);
            }
        }
    
        
        [HarmonyPostfix]
        [HarmonyPatch("_OnLateUpdate")]
        public static void _OnLateUpdate(ref UIStarmapPlanet __instance, LineRenderer ___orbitRenderer, Material ___orbitMat, Material ___planetMat) {
            Vector3 vector3 = __instance.planet.orbitAroundPlanet == null ? (Vector3) ((__instance.planet.star.uPosition - __instance.starmap.viewTargetUPos) * 0.00025) : (Vector3) ((__instance.planet.orbitAroundPlanet.uPosition - __instance.starmap.viewTargetUPos) * 0.00025);
            Quaternion quaternion = __instance.planet.runtimeOrbitRotation;
            ___orbitRenderer.transform.localRotation = quaternion;
            ___orbitMat.SetVector("_Position", new Vector4(vector3.x, vector3.y, vector3.z, 1f));
            ___orbitMat.SetVector("_Rotation", new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w));

        }
        
      
        
        
    }
}