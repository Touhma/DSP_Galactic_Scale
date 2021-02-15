using System.Collections.Generic;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    public static class PlanetRawDataExtension {
        private static readonly Dictionary<PlanetRawData, float>
            FactoredRadius = new Dictionary<PlanetRawData, float>();

        public static void AddFactoredRadius(this PlanetRawData planetRawData, PlanetData planet) {
            FactoredRadius[planetRawData] = planet.GetScaleFactored();
        }

        public static float GetFactoredScale(this PlanetRawData planetRawData) {
            return FactoredRadius.TryGetValue(planetRawData, out var result) ? result : 1f;
        }
    }
}