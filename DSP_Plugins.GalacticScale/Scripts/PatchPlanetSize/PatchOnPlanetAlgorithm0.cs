using System;
using BepInEx.Logging;
using GalacticScale.Scripts.PatchStarSystemGeneration;
using HarmonyLib;
using UnityEngine;
using Random = System.Random;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize  {
    [HarmonyPatch(typeof(PlanetAlgorithm0))]
    public class PatchOnPlanetAlgorithm0 {
        [HarmonyPrefix]
        [HarmonyPatch("GenerateTerrain")]
        public static bool PatchGenerateTerrain(ref PlanetAlgorithm1 __instance, ref PlanetData ___planet,
            double modX, double modY) {
            if (___planet.type == EPlanetType.Gas) {
                Patch.Debug("GenerateTerrain" + ___planet.radius + " for : " + ___planet.name, LogLevel.Debug,
                    Patch.DebugPlanetAlgorithm0);
            
                PlanetRawData data = ___planet.data;
                for (int index = 0; index < data.dataLength; ++index)
                {
                    data.heightData[index] = (ushort) ((double) ___planet.radius * 100.0);
                    Patch.Debug("data.heightData[index]" + data.heightData[index] , LogLevel.Debug,
                        Patch.DebugPlanetAlgorithm0);
                    data.biomoData[index] = (byte) 0;
                }
            
                return false;
            }

            return true;
        }

        private static float diff(float a, float b) => (double) a > (double) b ? a - b : b - a;
    }
}