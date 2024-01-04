﻿using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetSimulator
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetSimulator), nameof(PlanetSimulator.SetPlanetData))]
        public static bool SetPlanetData(ref PlanetSimulator __instance, ref Transform ___lookCamera, ref UniverseSimulator ___universe, ref StarSimulator ___star, PlanetData planet)
        {
            __instance.planetData = planet;
            if (__instance.planetData.atmosMaterial != null)
            {
                __instance.atmoTrans0 = new GameObject("Atmosphere")
                {
                    layer = 31
                }.transform;
                __instance.atmoTrans0.parent = __instance.transform;
                __instance.atmoTrans0.localPosition = Vector3.zero;
                if (planet.GetScaleFactored() >= 1)
                    __instance.atmoTrans0.localScale *= planet.GetScaleFactored();
                else
                    __instance.atmoTrans0.localScale /= planet.GetScaleFactored();
                //__instance.cloudSimulator.nephogram.transform.localScale

                var primitive = GameObject.CreatePrimitive(PrimitiveType.Quad);
                primitive.layer = 31;
                __instance.atmoTrans1 = primitive.transform;
                __instance.atmoTrans1.parent = __instance.atmoTrans0;
                __instance.atmoTrans1.localPosition = Vector3.zero;
                Object.Destroy(primitive.GetComponent<Collider>());
                var component = primitive.GetComponent<Renderer>();
                var atmosMaterial = __instance.planetData.atmosMaterial;
                var atmosMaterialLate = __instance.planetData.atmosMaterialLate;
                component.sharedMaterials = new Material[2] {atmosMaterial, atmosMaterialLate};
                __instance.atmoMat = atmosMaterial;
                __instance.atmoMatLate = atmosMaterialLate;
                component.shadowCastingMode = ShadowCastingMode.Off;
                component.receiveShadows = false;
                component.lightProbeUsage = LightProbeUsage.Off;
                component.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                __instance.atmoTrans1.localScale = Vector3.one * (planet.realRadius * 5f * planet.GetScaleFactored());
                __instance.atmoMatRadiusParam = __instance.atmoMat.GetVector("_PlanetRadius");
                __instance.oceanMat = __instance.planetData.oceanMaterial;
                if (__instance.oceanMat != null)
                {
                    __instance.oceanRenderQueue = __instance.oceanMat.renderQueue;
                }

                if (__instance.cloudSimulator != null)
                {
                    __instance.cloudSimulator.OnEnable();
                }
            }

            ___lookCamera = Camera.main.transform;
            ___universe = GameMain.universeSimulator;
            ___star = ___universe.FindStarSimulator(planet.star);

            return false;
        }
    }
}