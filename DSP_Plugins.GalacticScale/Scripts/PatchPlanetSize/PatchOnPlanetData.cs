using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetData))]
    public class PatchOnPlanetData {

        //Strategy: 1) Remove checks for PlanetData.scale; 2) Convert GetModPlane to use our Int version
        // 1) find all calls to ldArg.0
        // if the following instruction is ldfld float32 PlanetData::scale
        //    change ldarg.0 to OpCodes.Nop
        //    change the following instruction to ldc.r4 1
        // 2) find all calls to GetModPlane
        [HarmonyPatch("UpdateDirtyMesh")]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Ldarg_0 && i < codes.Count - 1)
                    // This condition removes references to this.scale. First we just check for the IL equivalent of "this."
                    // We stop checking this 1 early because we operate on both the current AND following line when we do anything
                {
                    // Check if the field we're reading from "this." is scale
                    if (codes[i + 1].LoadsField(typeof(PlanetData).GetField("scale"))) {
                        // Prevent "this." from being added to the stack (ordinarily, the field reference would remove it from the stack)
                        codes[i] = new CodeInstruction(OpCodes.Nop);
                        // Instead load the fixed value 1, as a float32
                        codes[i + 1] = new CodeInstruction(OpCodes.Ldc_R4, 1f);
                    }

                }
                else if (codes[i].Calls(typeof(PlanetRawData).GetMethod("GetModPlane")))
                    // This condition finds calls to PlanetRawData.GetModPlane (which returns a short)
                {
                    // We instead call PlanetRawDataExtension.GetModPlaneInt (which returns an int)
                    // All existing calls to GetModPlane cast the result to a float, anyway...
                    codes[i] = new CodeInstruction(OpCodes.Call, typeof(PlanetRawDataExtension).GetMethod("GetModPlaneInt"));
                }
            return codes.AsEnumerable();
        }

        [HarmonyPrefix]
        [HarmonyPatch("realRadius", MethodType.Getter)]
        public static bool RealRadiusGetter(ref PlanetData __instance, ref float __result) {
            if (__instance.type != EPlanetType.Gas) {
                __result = __instance.radius;
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GenBirthPoints")]
        public static bool GenBirthPoints(PlanetRawData rawData, int _birthSeed) {
            return true;
        }
        
         [HarmonyPrefix]
        [HarmonyPatch("UpdateRuntimePose")]
        public static bool UpdateRuntimePose(ref PlanetData __instance, double time) {
            double num1 = time / __instance.orbitalPeriod + (double) __instance.orbitPhase / 360.0;
            int num2 = (int) (num1 + 0.1);
            double num3 = num1 - (double) num2;
            __instance.runtimeOrbitPhase = (float) num3 * 360f;
            double num4 = num3 * (2.0 * Math.PI);
            double num5 = time / __instance.rotationPeriod + (double) __instance.rotationPhase / 360.0;
            int num6 = (int) (num5 + 0.1);
            double num7 = (num5 - (double) num6) * 360.0;
            __instance.runtimeRotationPhase = (float) num7;
            VectorLF3 vectorLf3_1 = Maths.QRotateLF(__instance.runtimeOrbitRotation, new VectorLF3((float) Math.Cos(num4) * __instance.orbitRadius * 0.75, 0.0f, (float) Math.Sin(num4) * __instance.orbitRadius * 1.25));
            if (__instance.orbitAroundPlanet != null) {
                vectorLf3_1.x += __instance.orbitAroundPlanet.runtimePosition.x * 0.75;
                vectorLf3_1.y += __instance.orbitAroundPlanet.runtimePosition.y;
                vectorLf3_1.z += __instance.orbitAroundPlanet.runtimePosition.z * 1.25;
            }
            __instance.runtimePosition = vectorLf3_1;
            __instance.runtimeRotation = __instance.runtimeSystemRotation * Quaternion.AngleAxis((float) num7, Vector3.down);
            __instance.uPosition.x = __instance.star.uPosition.x + vectorLf3_1.x * 40000.0;
            __instance.uPosition.y = __instance.star.uPosition.y + vectorLf3_1.y * 40000.0;
            __instance.uPosition.z = __instance.star.uPosition.z + vectorLf3_1.z * 40000.0;
            __instance.runtimeLocalSunDirection = Maths.QInvRotate(__instance.runtimeRotation, -vectorLf3_1);
            double num8 = time + 1.0 / 60.0;
            double num9 = num8 / __instance.orbitalPeriod + (double) __instance.orbitPhase / 360.0;
            int num10 = (int) (num9 + 0.1);
            double num11 = (num9 - (double) num10) * (2.0 * Math.PI);
            double num12 = num8 / __instance.rotationPeriod + (double) __instance.rotationPhase / 360.0;
            int num13 = (int) (num12 + 0.1);
            double num14 = (num12 - (double) num13) * 360.0;
            VectorLF3 vectorLf3_2 = Maths.QRotateLF(__instance.runtimeOrbitRotation, new VectorLF3((float) Math.Cos(num11) * __instance.orbitRadius * 0.75f, 0.0f, (float) Math.Sin(num11) * __instance.orbitRadius * 1.25f));
            if (__instance.orbitAroundPlanet != null) {
                vectorLf3_2.x += __instance.orbitAroundPlanet.runtimePositionNext.x;
                vectorLf3_2.y += __instance.orbitAroundPlanet.runtimePositionNext.y;
                vectorLf3_2.z += __instance.orbitAroundPlanet.runtimePositionNext.z;
            }
            __instance.runtimePositionNext = vectorLf3_2;
            __instance.runtimeRotationNext = __instance.runtimeSystemRotation * Quaternion.AngleAxis((float) num14, Vector3.down);
            __instance.uPositionNext.x = __instance.star.uPosition.x + vectorLf3_2.x * 40000.0;
            __instance.uPositionNext.y = __instance.star.uPosition.y + vectorLf3_2.y * 40000.0;
            __instance.uPositionNext.z = __instance.star.uPosition.z + vectorLf3_2.z * 40000.0;
            __instance.galaxy.astroPoses[__instance.id].uPos = __instance.uPosition;
            __instance.galaxy.astroPoses[__instance.id].uRot = __instance.runtimeRotation;
            __instance.galaxy.astroPoses[__instance.id].uPosNext = __instance.uPositionNext;
            __instance.galaxy.astroPoses[__instance.id].uRotNext = __instance.runtimeRotationNext;

            return false;
        }
    }

}