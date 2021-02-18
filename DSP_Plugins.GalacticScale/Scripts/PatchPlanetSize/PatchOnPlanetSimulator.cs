using System;
using BepInEx.Logging;
using GalacticScale.Scripts.PatchStarSystemGeneration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;


namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetSimulator))]
    public class PatchOnPlanetSimulator {
        [HarmonyPrefix]
        [HarmonyPatch("SetPlanetData")]
        public static bool SetPlanetData(ref PlanetSimulator __instance, ref Transform ___lookCamera,
            ref UniverseSimulator ___universe, ref StarSimulator ___star, PlanetData planet) {
            __instance.planetData = planet;
            if ((UnityEngine.Object) __instance.planetData.atmosMaterial != (UnityEngine.Object) null) {
                __instance.atmoTrans0 = new GameObject("Atmosphere") {
                    layer = 31
                }.transform;
                __instance.atmoTrans0.parent = __instance.transform;
                __instance.atmoTrans0.localPosition = Vector3.zero;
                GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Quad);
                primitive.layer = 31;
                __instance.atmoTrans1 = primitive.transform;
                __instance.atmoTrans1.parent = __instance.atmoTrans0;
                __instance.atmoTrans1.localPosition = Vector3.zero;
                UnityEngine.Object.Destroy((UnityEngine.Object) primitive.GetComponent<Collider>());
                Renderer component = primitive.GetComponent<Renderer>();
                Material atmosMaterial = __instance.planetData.atmosMaterial;
                component.sharedMaterial = atmosMaterial;
                __instance.atmoMat = atmosMaterial;
                component.shadowCastingMode = ShadowCastingMode.Off;
                component.receiveShadows = false;
                component.lightProbeUsage = LightProbeUsage.Off;
                component.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                __instance.atmoTrans1.localScale = Vector3.one * (planet.realRadius * 5f * planet.GetScaleFactored());
                __instance.atmoMatRadiusParam = __instance.atmoMat.GetVector("_PlanetRadius");
            }

            ___lookCamera = Camera.main.transform;
            ___universe = GameMain.universeSimulator;
            ___star = ___universe.FindStarSimulator(planet.star);

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdateUniversalPosition")]
        public static bool UpdateUniversalPosition(ref PlanetSimulator __instance, ref StarSimulator ___star, ref bool ___isLocal , ref Transform ___lookCamera ,Vector3 playerLPos, VectorLF3 playerUPos, Vector3 cameraPos) {
            if (__instance.planetData == null ||__instance.planetData.loading ||__instance.planetData.factoryLoading) {
                return false;
            }
           __instance.SetLayers();
            PlanetData localPlanet = GameMain.localPlanet;
            bool flag1 =___isLocal != (localPlanet ==__instance.planetData);
           ___isLocal = localPlanet ==__instance.planetData;
            bool flag2 = PlanetSimulator.sOptionCastShadow !=__instance.optionCastShadow;
            if (flag1 || flag2) {
                foreach (Renderer renderer in __instance.surfaceRenderer) {
                    renderer.receiveShadows =___isLocal;
                    renderer.shadowCastingMode = !___isLocal || !PlanetSimulator.sOptionCastShadow
                        ? ShadowCastingMode.Off
                        : ShadowCastingMode.On;
                }

               __instance.optionCastShadow = PlanetSimulator.sOptionCastShadow;
            }

           __instance.reformRenderer.receiveShadows =___isLocal;
            bool flag3 =___isLocal &&__instance.planetData.type != EPlanetType.Gas;
            if (__instance.sphereCollider.enabled == flag3) {
                foreach (Collider collider in __instance.surfaceCollider) {
                    collider.enabled = flag3;
                }
                if ((UnityEngine.Object)__instance.oceanCollider != (UnityEngine.Object) null) {
                    __instance.oceanCollider.enabled = flag3;
                }
               __instance.sphereCollider.enabled = !flag3;
            }

            VectorLF3 uPosition =__instance.planetData.uPosition;
            Quaternion quaternion = Quaternion.identity;
            VectorLF3 vectorLf3;
            if (localPlanet != null) {
                quaternion = localPlanet.runtimeRotation;
                if (localPlanet ==__instance.planetData) {
                    vectorLf3 = VectorLF3.zero;
                }
                else {
                    VectorLF3 v = uPosition - localPlanet.uPosition;
                    vectorLf3 = Maths.QInvRotateLF(quaternion, v);
                }
            }
            else {
                vectorLf3 = uPosition - playerUPos;
            }

            float vscale = 1f;
            Vector3 vpos;
            UniverseSimulator.VirtualMapping(vectorLf3.x, vectorLf3.y, vectorLf3.z, cameraPos, out vpos, out vscale);
            Vector3 vector3_1 = Vector3.one * vscale;
            if (__instance.transform.localPosition != vpos) {
                __instance.transform.localPosition = vpos;
            }
            if (__instance.planetData == localPlanet) {
                if ((double)__instance.transform.localPosition.sqrMagnitude > 0.0) {
                    __instance.transform.localPosition = Vector3.zero;
                }
                if (__instance.transform.localRotation != Quaternion.identity) {
                    __instance.transform.localRotation = Quaternion.identity;
                }
            }
            else {
                __instance.transform.localRotation =
                    Quaternion.Inverse(quaternion) * __instance.planetData.runtimeRotation;
            }

            if (__instance.transform.localScale != vector3_1) {
                __instance.transform.localScale = vector3_1;
            }
            Vector3 vector3_2 = Quaternion.Inverse(quaternion) *
                                (Vector3) (__instance.planetData.star.uPosition -__instance.planetData.uPosition).normalized;
            Vector3 lhs =___lookCamera.localPosition -__instance.transform.localPosition;
            float magnitude = lhs.magnitude;
            Quaternion localRotation =__instance.transform.localRotation;
            Vector4 vector4_1 = new Vector4(localRotation.x, localRotation.y, localRotation.z, localRotation.w);
            if (__instance.surfaceRenderer != null &&__instance.surfaceRenderer.Length > 0) {
                Material sharedMaterial =__instance.surfaceRenderer[0].sharedMaterial;
                sharedMaterial.SetVector("_SunDir", (Vector4) vector3_2);
                sharedMaterial.SetVector("_Rotation",
                    new Vector4(localRotation.x, localRotation.y, localRotation.z, localRotation.w));
                sharedMaterial.SetFloat("_Distance", magnitude);
            }

            if ((UnityEngine.Object)__instance.reformRenderer != (UnityEngine.Object) null &&
                (UnityEngine.Object)__instance.reformMat != (UnityEngine.Object) null &&__instance.planetData.factory != null) {
                PlatformSystem platformSystem =__instance.planetData.factory.platformSystem;
                ComputeBuffer reformOffsetsBuffer = platformSystem.reformOffsetsBuffer;
                ComputeBuffer reformDataBuffer = platformSystem.reformDataBuffer;
               __instance.reformMat.SetFloat("_LatitudeCount", (float) platformSystem.latitudeCount);
               __instance.reformMat.SetVector("_SunDir", (Vector4) vector3_2);
               __instance.reformMat.SetFloat("_Distance", magnitude);
               __instance.reformMat.SetVector("_Rotation", vector4_1);
                if (platformSystem.reformData != null && reformDataBuffer != null) {
                    reformOffsetsBuffer.SetData((Array) platformSystem.reformOffsets);
                    reformDataBuffer.SetData((Array) platformSystem.reformData);
                   __instance.reformMat.SetBuffer("_OffsetsBuffer", reformOffsetsBuffer);
                   __instance.reformMat.SetBuffer("_DataBuffer", reformDataBuffer);
                }
            }

            if (!((UnityEngine.Object)__instance.atmoTrans0 != (UnityEngine.Object) null)) {
                return false;
            }
           __instance.atmoTrans0.rotation =___lookCamera.localRotation;
            Vector4 vector4_2 = (Vector4) (!((UnityEngine.Object) GameCamera.generalTarget == (UnityEngine.Object) null)
                ? GameCamera.generalTarget.position
                : Vector3.zero);
            
            // ********************************* Fix Here for release :) 
            int positionOffset = 0;
            if (localPlanet != null) {
                if (localPlanet.type != EPlanetType.Gas) {
                    positionOffset = Mathf.RoundToInt(Mathf.Abs(localPlanet.radius - 200) / 2);
                }
            }
            Patch.Debug("positionOffset "+positionOffset , LogLevel.Debug, Patch.DebugAtmoBlur);

    
           __instance.atmoTrans1.localPosition = new Vector3(30,30,
                Mathf.Clamp(Vector3.Dot(lhs,___lookCamera.forward) + 10f, 0.0f, 320f /   __instance.planetData.GetScaleFactored()));
            float num1 = Mathf.Clamp01(8000f * __instance.planetData.GetScaleFactored() / magnitude);
            float num2 = Mathf.Clamp01(4000f  * __instance.planetData.GetScaleFactored() / magnitude);
            Vector4 atmoMatRadiusParam =__instance.atmoMatRadiusParam;
            atmoMatRadiusParam.z = atmoMatRadiusParam.x +
                                   (float) (((double)__instance.atmoMatRadiusParam.z - (double)__instance.atmoMatRadiusParam.x) *
                                            (2.70000004768372 - (double) num2 * 1.70000004768372));
            Vector4 vector4_3 = atmoMatRadiusParam * vscale * __instance.planetData.GetScaleFactored();
           __instance.atmoMat.SetVector("_PlanetPos", (Vector4)__instance.transform.localPosition);
           __instance.atmoMat.SetVector("_SunDir", (Vector4) vector3_2);
           __instance.atmoMat.SetVector("_PlanetRadius", vector4_3);
           __instance.atmoMat.SetColor("_Color4",___star.sunAtmosColor);
           __instance.atmoMat.SetColor("_Sky4",___star.sunriseAtmosColor);
           __instance.atmoMat.SetVector("_LocalPos", vector4_2);
           __instance.atmoMat.SetFloat("_SunRiseScatterPower",
                Mathf.Max(60f  * __instance.planetData.GetScaleFactored(),
                    (float) (((double) magnitude - (double)__instance.planetData.realRadius * 2.0) * 0.180000007152557)));
           __instance.atmoMat.SetFloat("_IntensityControl", num1);
           __instance.atmoMat.renderQueue =__instance.planetData != localPlanet ? 2989 : 2991;
           
           return false;
        }
        
    }
}