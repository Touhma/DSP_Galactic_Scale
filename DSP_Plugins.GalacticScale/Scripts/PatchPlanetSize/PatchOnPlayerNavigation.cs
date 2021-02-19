using GalacticScale.Scripts.PatchStarSystemGeneration;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlayerNavigation))]
    public class PatchOnPlayerNavigation {
        [HarmonyPrefix]
        [HarmonyPatch("DetermineHighOperation")]
        public static bool DetermineHighOperation(ref PlayerNavigation __instance, ref float ___arriveFactor,
            float speedWanted, ref bool lift, ref bool drop) {
            var getPlayer = Traverse.Create(__instance).Method("get_player").GetValue<Player>();

            double num1 = PlayerNavigation.SphericalDistance((VectorLF3) getPlayer.position, __instance.naviTarget,
                (double) getPlayer.planetData.realRadius * getPlayer.planetData.GetScaleFactored(), false);
            Debug.Log("num1"+ num1);

            float num2 = ___arriveFactor / speedWanted;
            if (__instance.useSailFinally && num1 > 80.0) {
                lift = true;
            }

            if ((double) num2 >= 2.0) {
                {
                    return false;
                }
            }

            drop = true;
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch("DetermineLowVelocity")]
        public static bool DetermineLowVelocity(
            float speedWanted,
            float turningRatio,
            ref Vector3 vel,
            ref bool fly,
            ref PlayerNavigation __instance
         )
        {
            var getPlayer = Traverse.Create(__instance).Method("get_player").GetValue<Player>();
            float realRadius = getPlayer.planetData.realRadius;
            
            Debug.Log("realRadius"+ realRadius);
            Vector3 vector3_1 = getPlayer.position.normalized * realRadius;
            Debug.Log("vector3_1"+ vector3_1);
            Vector3 vector3_2 = (Vector3) (__instance.naviTarget.normalized * (double) realRadius);
            Debug.Log("vector3_2"+ vector3_2);
            Vector3 normalized1 = vector3_1.normalized;
            Debug.Log("normalized1"+ normalized1);
            Vector3 rhs = vector3_2 - vector3_1;
            Debug.Log("rhs"+ rhs);
            float magnitude = rhs.magnitude;
            Debug.Log("magnitude"+ magnitude);
            Vector3 normalized2 = Vector3.Cross(Vector3.Cross(normalized1, rhs).normalized, normalized1).normalized;
            Debug.Log("normalized2"+ normalized2);
            vel = Vector3.Slerp(vel, normalized2 * speedWanted, turningRatio);
            if (!__instance.useFlyFinally || (double) magnitude <= 20.0) {
                return false;
            }
            fly = true;
            return false;
        }
        
    }
}