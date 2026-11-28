using HarmonyLib;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnCloudSimulator
    {
        private static readonly int TileSettings = Shader.PropertyToID("_TileSettings");
        private static readonly int SizeMultiplier = Shader.PropertyToID("_SizeMultiplier");
        private static readonly ConditionalWeakTable<Material, CloudMaterialSettings> MaterialSettings = new();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CloudSimulator), nameof(CloudSimulator.OnEnable))]
        public static void OnEnable(CloudSimulator __instance)
        {
            if (GS2.Vanilla || GS2.IsMenuDemo) return;

            var planet = __instance.planet;
            float scale = planet.GetAtmosphereScale();
            if (scale == 1f) return;

            ScaleMaterial(planet.nephogramMaterial, scale);
            ScaleMaterial(planet.cloudMaterial, scale);

            // CloudRenderer scatters particles around the shell with a fixed radial spread.
            float shellRadius = planet.realRadius + __instance.cloudHeight;
            var particles = __instance.cloudr.instArr;
            for (int i = 0; i < particles.Length; i++)
            {
                ref var position = ref particles[i].pos;
                float radius = position.magnitude;
                if (radius > 0f)
                    position *= (shellRadius + (radius - shellRadius) * scale) / radius;
            }
        }

        private static void ScaleMaterial(Material material, float scale)
        {
            var settings = MaterialSettings.GetValue(material, source => new CloudMaterialSettings(source));
            if (settings.Tiling.HasValue)
            {
                // The sphere shader and particle culling shader sample in planet-local distance units.
                var tiling = settings.Tiling.Value;
                tiling.w /= scale;
                material.SetVector(TileSettings, tiling);
            }

            if (settings.Size.HasValue)
                material.SetFloat(SizeMultiplier, settings.Size.Value * scale);
        }

        private sealed class CloudMaterialSettings
        {
            public readonly Vector4? Tiling;
            public readonly float? Size;

            public CloudMaterialSettings(Material material)
            {
                if (material.HasProperty(TileSettings)) Tiling = material.GetVector(TileSettings);
                if (material.HasProperty(SizeMultiplier)) Size = material.GetFloat(SizeMultiplier);
            }
        }
    }
}
