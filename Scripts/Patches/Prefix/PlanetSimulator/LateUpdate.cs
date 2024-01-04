using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
// ReSharper disable PossibleNullReferenceException

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetSimulator
    {
	    private static readonly int SunDir = Shader.PropertyToID("_SunDir");
	    private static readonly int PlanetPos = Shader.PropertyToID("_PlanetPos");
	    private static readonly int PlanetWaterAmbientColor0 = Shader.PropertyToID("_Planet_WaterAmbientColor0");
	    private static readonly int PlanetWaterAmbientColor1 = Shader.PropertyToID("_Planet_WaterAmbientColor1");
	    private static readonly int PlanetWaterAmbientColor2 = Shader.PropertyToID("_Planet_WaterAmbientColor2");
	    private static readonly int PlanetRadius = Shader.PropertyToID("_PlanetRadius");
	    private static readonly int Color4 = Shader.PropertyToID("_Color4");
	    private static readonly int Sky4 = Shader.PropertyToID("_Sky4");
	    private static readonly int LocalPos = Shader.PropertyToID("_LocalPos");
	    private static readonly int SunRiseScatterPower = Shader.PropertyToID("_SunRiseScatterPower");
	    private static readonly int IntensityControl = Shader.PropertyToID("_IntensityControl");
	    private static readonly int DistanceControl = Shader.PropertyToID("_DistanceControl");
	    private static readonly int StencilRef = Shader.PropertyToID("_StencilRef");
	    private static readonly int StencilComp = Shader.PropertyToID("_StencilComp");
	    
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetSimulator), nameof(PlanetSimulator.LateUpdate))]
        public static bool LateUpdate(PlanetSimulator __instance)
        {
	        PlanetData localPlanet = GameMain.localPlanet;
			Quaternion rotation = localPlanet?.runtimeRotation ?? Quaternion.identity;

			PlanetData planetData = __instance.planetData;
			Vector3 sunDir = Quaternion.Inverse(rotation) * (planetData.star.uPosition - planetData.uPosition).normalized;
			if (FactoryModel.whiteMode0)
			{
				sunDir = -GameCamera.instance.camLight.transform.forward;
			}

			Material atmoMat = __instance.atmoMat;
			Material atmoMatLate = __instance.atmoMatLate;
			
			if (localPlanet == planetData && localPlanet != null)
			{
				if (__instance.surfaceRenderer != null && __instance.surfaceRenderer.Length != 0)
				{
					__instance.surfaceRenderer[0].sharedMaterial.SetVector(SunDir, sunDir);
				}
				if (__instance.reformMat0 != null)
				{
					__instance.reformMat0.SetVector(SunDir, sunDir);
				}
				if (__instance.reformMat1 != null)
				{
					__instance.reformMat1.SetVector(SunDir, sunDir);
				}
				if (atmoMat != null)
				{
					atmoMat.SetVector(SunDir, sunDir);
					atmoMat.SetVector(PlanetPos, __instance.transform.localPosition);
				}
				if (atmoMatLate != null)
				{
					atmoMatLate.SetVector(SunDir, sunDir);
					atmoMatLate.SetVector(PlanetPos, __instance.transform.localPosition);
				}
			}
			if (__instance.cloudSimulator != null)
			{
				__instance.cloudSimulator.OnLateUpdate();
			}
			if (__instance.oceanMat != null)
			{
				if (__instance.oceanRenderQueue > 2987)
				{
					__instance.oceanMat.renderQueue = localPlanet == planetData ? __instance.oceanRenderQueue : 2988;
				}
				__instance.oceanMat.SetColor(PlanetWaterAmbientColor0, planetData.ambientDesc.waterAmbientColor0);
				__instance.oceanMat.SetColor(PlanetWaterAmbientColor1, planetData.ambientDesc.waterAmbientColor1);
				__instance.oceanMat.SetColor(PlanetWaterAmbientColor2, planetData.ambientDesc.waterAmbientColor2);
			}
			if (__instance.atmoTrans0 != null && planetData != null && !planetData.loading && !planetData.factoryLoading)
			{
				__instance.atmoTrans0.rotation = __instance.lookCamera.localRotation;
				Vector4 localPos = GameCamera.generalTarget == null ? Vector3.zero : GameCamera.generalTarget.position;
				Vector3 position = GameCamera.main.transform.position;
				if (localPos.sqrMagnitude == 0f)
				{
					localPos = !GameCamera.instance.isPlanetMode ? (Vector4)GameMain.mainPlayer.position : ((Vector4)(position + GameCamera.main.transform.forward * 30f)).normalized * planetData.realRadius;
				}
				Vector3 posToCam = __instance.lookCamera.localPosition - __instance.transform.localPosition;
				float distancePosToCam = posToCam.magnitude;
				VectorLF3 uPos = planetData.uPosition;
				if (localPlanet != null)
				{
					rotation = localPlanet.runtimeRotation;
					if (localPlanet == planetData)
					{
						uPos = VectorLF3.zero;
					}
					else
					{
						uPos -= localPlanet.uPosition;
						uPos = Maths.QInvRotateLF(rotation, uPos);
					}
				}
				else if (GameMain.mainPlayer != null)
				{
					uPos -= GameMain.mainPlayer.uPosition;
				}
				UniverseSimulator.VirtualMapping(uPos.x, uPos.y, uPos.z, position, out _, out var vscale);
				float scaleFactored = planetData.GetScaleFactored();
				__instance.atmoTrans1.localPosition = new Vector3(0, 0, Mathf.Clamp(Vector3.Dot(posToCam, __instance.lookCamera.forward) + 10f / scaleFactored, 0.0f, Math.Max(320f, 320f * scaleFactored)));
				float intensityControl = Mathf.Clamp01(8000f / distancePosToCam);
				float scaleRadiusControl = Mathf.Clamp01(4000f / distancePosToCam);
				float distanceControl = Mathf.Max(0f, distancePosToCam / 6000f - 1f);
				Vector4 radiusParam = __instance.atmoMatRadiusParam;
				radiusParam.z = radiusParam.x + (radiusParam.z - radiusParam.x) * (2.7f - scaleRadiusControl * 1.7f);
				radiusParam = radiusParam * vscale * __instance.planetData.GetScaleFactored();
				
				
				atmoMat.SetVector(PlanetPos, __instance.transform.localPosition);
				atmoMat.SetVector(SunDir, sunDir);
				atmoMat.SetVector(PlanetRadius, radiusParam);
				atmoMat.SetColor(Color4, __instance.star.sunAtmosColor);
				atmoMat.SetColor(Sky4, __instance.star.sunriseAtmosColor);
				atmoMat.SetVector(LocalPos, localPos);
				atmoMat.SetFloat(SunRiseScatterPower, Mathf.Max(60f * scaleFactored, (distancePosToCam - planetData.realRadius * 2f) * 0.18f));
				atmoMat.SetFloat(IntensityControl, intensityControl);
				atmoMat.SetFloat(DistanceControl, distanceControl);
				atmoMat.renderQueue = planetData == localPlanet ? 2991 : 2989;
				
				atmoMatLate.SetVector(PlanetPos, __instance.transform.localPosition);
				atmoMatLate.SetVector(SunDir, sunDir);
				atmoMatLate.SetVector(PlanetRadius, radiusParam);
				atmoMatLate.SetColor(Color4, __instance.star.sunAtmosColor);
				atmoMatLate.SetColor(Sky4, __instance.star.sunriseAtmosColor);
				atmoMatLate.SetVector(LocalPos, localPos);
				atmoMatLate.SetFloat(SunRiseScatterPower, Mathf.Max(60f * scaleFactored, (distancePosToCam - planetData.realRadius * 2f) * 0.18f));
				atmoMatLate.SetFloat(IntensityControl, intensityControl);
				atmoMatLate.SetFloat(DistanceControl, distanceControl);
				if (planetData == localPlanet)
				{
					atmoMatLate.renderQueue = 3200;
					atmoMatLate.SetInt(StencilRef, 2);
					atmoMatLate.SetInt(StencilComp, 3);
				}
				else
				{
					atmoMatLate.renderQueue = 2989;
					atmoMatLate.SetInt(StencilRef, 0);
					atmoMatLate.SetInt(StencilComp, 1);
				}
			}
			__instance.GpuAnalysis();

			return false;
		
        }
    }
}