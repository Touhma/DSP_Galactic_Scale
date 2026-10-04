using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public static partial class PatchOnPlanetATField
    {
        private const float ReferenceRadius = 200f;
        private const float MinFactor = 0.5f;
        private const float MaxFactor = 2f;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetATField), "UpdateGeneratorMatrix")]
        public static void UpdateGeneratorMatrix_ScaleReach(PlanetATField __instance)
        {
            if (ShieldCompat.PlanetwideShieldInstalled) return;
            var small = GS2.Config.ShieldScaleStrength;
            var large = GS2.Config.ShieldScaleLargeStrength;
            if ((small <= 0f && large <= 0f) || __instance.planet == null || __instance.generatorMatrix == null) return;
            var factor = GetShieldReachFactor(__instance.planet.realRadius, small, large);
            if (Mathf.Approximately(factor, 1f)) return;
            for (var i = 0; i < __instance.generatorCount; i++) __instance.generatorMatrix[i].w *= factor;
        }

        public static float GetShieldReachFactor(float radius, float smallStrength, float largeStrength)
        {
            if (radius <= 0f) return 1f;
            var strength = radius < ReferenceRadius ? smallStrength : largeStrength;
            if (strength <= 0f) return 1f;
            return Mathf.Clamp(Mathf.Pow(ReferenceRadius / radius, strength), MinFactor, MaxFactor);
        }
    }
}