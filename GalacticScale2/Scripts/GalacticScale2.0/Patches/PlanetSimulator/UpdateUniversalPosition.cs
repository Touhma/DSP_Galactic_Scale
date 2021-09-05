using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;

namespace GalacticScale
{
    public partial class PatchOnPlanetSimulator
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetSimulator), "UpdateUniversalPosition")]
        public static bool UpdateUniversalPosition(ref PlanetSimulator __instance, ref StarSimulator ___star, ref bool ___isLocal, ref Transform ___lookCamera, Vector3 playerLPos, VectorLF3 playerUPos, Vector3 cameraPos)
        {
            if (__instance.planetData == null || __instance.planetData.loading || __instance.planetData.factoryLoading || __instance.planetData == PlanetModelingManager.currentModelingPlanet) return false;

            __instance.SetLayers();
            var localPlanet = GameMain.localPlanet;
            var flag1 = ___isLocal != (localPlanet == __instance.planetData);
            ___isLocal = localPlanet == __instance.planetData;
            var flag2 = PlanetSimulator.sOptionCastShadow != __instance.optionCastShadow;
            if (flag1 || flag2)
            {
                foreach (var renderer in __instance.surfaceRenderer)
                {
                    renderer.receiveShadows = ___isLocal;
                    renderer.shadowCastingMode = !___isLocal || !PlanetSimulator.sOptionCastShadow ? ShadowCastingMode.Off : ShadowCastingMode.On;
                }

                __instance.optionCastShadow = PlanetSimulator.sOptionCastShadow;
            }

            __instance.reformRenderer.receiveShadows = ___isLocal;
            var flag3 = ___isLocal && __instance.planetData.type != EPlanetType.Gas;
            if (__instance.sphereCollider.enabled == flag3)
            {
                foreach (var collider in __instance.surfaceCollider) collider.enabled = flag3;

                if (__instance.oceanCollider != null) __instance.oceanCollider.enabled = flag3;

                __instance.sphereCollider.enabled = !flag3;
            }

            var uPosition = __instance.planetData.uPosition;
            var quaternion = Quaternion.identity;
            VectorLF3 vectorLf3;
            if (localPlanet != null)
            {
                quaternion = localPlanet.runtimeRotation;
                if (localPlanet == __instance.planetData)
                {
                    vectorLf3 = VectorLF3.zero;
                }
                else
                {
                    var v = uPosition - localPlanet.uPosition;
                    vectorLf3 = Maths.QInvRotateLF(quaternion, v);
                }
            }
            else
            {
                vectorLf3 = uPosition - playerUPos;
            }

            var vscale = 1f;
            Vector3 vpos;
            UniverseSimulator.VirtualMapping(vectorLf3.x, vectorLf3.y, vectorLf3.z, cameraPos, out vpos, out vscale);
            var vector3_1 = Vector3.one * vscale;
            if (__instance.transform.localPosition != vpos) __instance.transform.localPosition = vpos;

            if (__instance.planetData == localPlanet)
            {
                if (__instance.transform.localPosition.sqrMagnitude > 0.0)
                    __instance.transform.localPosition = Vector3.zero;

                if (__instance.transform.localRotation != Quaternion.identity)
                    __instance.transform.localRotation = Quaternion.identity;
            }
            else
            {
                __instance.transform.localRotation = Quaternion.Inverse(quaternion) * __instance.planetData.runtimeRotation;
            }

            if (__instance.transform.localScale != vector3_1) __instance.transform.localScale = vector3_1;

            var vector3_2 = Quaternion.Inverse(quaternion) * (__instance.planetData.star.uPosition - __instance.planetData.uPosition).normalized;
            var lhs = ___lookCamera.localPosition - __instance.transform.localPosition;
            var magnitude = lhs.magnitude;
            var localRotation = __instance.transform.localRotation;
            var vector4_1 = new Vector4(localRotation.x, localRotation.y, localRotation.z, localRotation.w);
            if (__instance.surfaceRenderer != null && __instance.surfaceRenderer.Length > 0)
            {
                var sharedMaterial = __instance.surfaceRenderer[0].sharedMaterial;
                sharedMaterial.SetVector("_SunDir", vector3_2);
                sharedMaterial.SetVector("_Rotation", new Vector4(localRotation.x, localRotation.y, localRotation.z, localRotation.w));
                sharedMaterial.SetFloat("_Distance", magnitude);
            }

            if (__instance.reformRenderer != null && __instance.reformMat0 != null && __instance.reformMat0 != null && __instance.planetData.factory != null)
            {
                var platformSystem = __instance.planetData.factory.platformSystem;
                var reformOffsetsBuffer = platformSystem.reformOffsetsBuffer;
                var reformDataBuffer = platformSystem.reformDataBuffer;
                __instance.reformMat0.SetFloat("_LatitudeCount", platformSystem.latitudeCount);
                __instance.reformMat0.SetVector("_SunDir", vector3_2);
                __instance.reformMat0.SetFloat("_Distance", magnitude);
                __instance.reformMat0.SetVector("_Rotation", vector4_1);
                __instance.reformMat0.SetTexture("_ColorsTexture", platformSystem.reformColorsTex);
                __instance.reformMat1.SetFloat("_LatitudeCount", platformSystem.latitudeCount);
                __instance.reformMat1.SetVector("_SunDir", vector3_2);
                __instance.reformMat1.SetFloat("_Distance", magnitude);
                __instance.reformMat1.SetVector("_Rotation", vector4_1);
                __instance.reformMat1.SetTexture("_ColorsTexture", platformSystem.reformColorsTex);
                if (platformSystem.reformData != null && reformDataBuffer != null)
                {
                    reformOffsetsBuffer.SetData(platformSystem.reformOffsets);
                    reformDataBuffer.SetData(platformSystem.reformData);
                    __instance.reformMat0.SetBuffer("_OffsetsBuffer", reformOffsetsBuffer);
                    __instance.reformMat0.SetBuffer("_DataBuffer", reformDataBuffer);
                    __instance.reformMat1.SetBuffer("_OffsetsBuffer", reformOffsetsBuffer);
                    __instance.reformMat1.SetBuffer("_DataBuffer", reformDataBuffer);
                }
            }

            if (!(__instance.atmoTrans0 != null)) return false;

            __instance.atmoTrans0.rotation = ___lookCamera.localRotation;
            Vector4 vector4_2 = !(GameCamera.generalTarget == null) ? GameCamera.generalTarget.position : Vector3.zero;

            // ********************************* Fix Here for release :) 
            var positionOffset = 0;
            if (localPlanet != null)
                if (localPlanet.type != EPlanetType.Gas)
                    positionOffset = Mathf.RoundToInt(Mathf.Abs(localPlanet.radius - 200) / 2);

            __instance.atmoTrans1.localPosition = new Vector3(0, 0, Mathf.Clamp(Vector3.Dot(lhs, ___lookCamera.forward) + 10f / __instance.planetData.GetScaleFactored(), 0.0f, Math.Max(320f, 320f * __instance.planetData.GetScaleFactored())));
            var num1 = Mathf.Clamp01(8000f * __instance.planetData.GetScaleFactored() / magnitude);
            var num2 = Mathf.Clamp01(4000f * __instance.planetData.GetScaleFactored() / magnitude);
            var atmoMatRadiusParam = __instance.atmoMatRadiusParam;
            atmoMatRadiusParam.z = atmoMatRadiusParam.x + (float)((__instance.atmoMatRadiusParam.z - (double)__instance.atmoMatRadiusParam.x) * (2.70000004768372 - num2 * 1.70000004768372));
            var vector4_3 = atmoMatRadiusParam * vscale * __instance.planetData.GetScaleFactored();
            __instance.atmoMat.SetVector("_PlanetPos", __instance.transform.localPosition);
            __instance.atmoMat.SetVector("_SunDir", vector3_2);
            __instance.atmoMat.SetVector("_PlanetRadius", vector4_3);
            __instance.atmoMat.SetColor("_Color4", ___star.sunAtmosColor);
            __instance.atmoMat.SetColor("_Sky4", ___star.sunriseAtmosColor);
            __instance.atmoMat.SetVector("_LocalPos", vector4_2);
            __instance.atmoMat.SetFloat("_SunRiseScatterPower", Mathf.Max(60f * __instance.planetData.GetScaleFactored(), (float)((magnitude - __instance.planetData.realRadius * 2.0) * 0.180000007152557)));
            __instance.atmoMat.SetFloat("_IntensityControl", num1);
            __instance.atmoMat.renderQueue = __instance.planetData != localPlanet ? 2989 : 2991;

            return false;
        }
    }
}