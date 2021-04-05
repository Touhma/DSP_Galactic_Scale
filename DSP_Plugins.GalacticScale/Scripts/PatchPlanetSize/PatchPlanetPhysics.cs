using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize
{
    [HarmonyPatch(typeof(PlanetPhysics))]
    public static class PatchPlanetPhysics
    {
        [HarmonyPrefix, HarmonyPatch("Init")]
        public static void Init(ref PlanetPhysics __instance, ref ColliderContainer[] ___colChunks, ref NearColliderLogic ___nearColliderLogic, ref RaycastLogic ___raycastLogic)
        {
            ___colChunks = new ColliderContainer[600];
            for (int index = 0; index < 600; ++index)
                ___colChunks[index] = new ColliderContainer();
            ___nearColliderLogic = new NearColliderLogic();
            ___nearColliderLogic.Init(__instance.planet);
            ___raycastLogic = new RaycastLogic();
            ___raycastLogic.Init(__instance.planet);
        }

        public static void Free(ref ColliderContainer[] ___colChunks, ref NearColliderLogic ___nearColliderLogic, ref RaycastLogic ___raycastLogic)
        {
            for (int index = 0; index < 600; ++index)
            {
                ___colChunks[index].Free();
                ___colChunks[index] = (ColliderContainer)null;
            }
            ___colChunks = (ColliderContainer[])null;
            if (___nearColliderLogic != null)
            {
                ___nearColliderLogic.Free();
                ___nearColliderLogic = (NearColliderLogic)null;
            }
            if (___raycastLogic == null)
                return;
            ___raycastLogic.Free();
            ___raycastLogic = (RaycastLogic)null;
        }
    }
}