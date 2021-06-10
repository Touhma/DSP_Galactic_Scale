using System;
using System.Collections.Generic;

namespace GalacticScale {
    public static class PlanetRawDataExtension {
        private static readonly Dictionary<PlanetRawData, float>
            FactoredRadius = new Dictionary<PlanetRawData, float>();

        public static void AddFactoredRadius(this PlanetRawData planetRawData, PlanetData planet) {
            //GS2.Log("PlanetRawDataExtension|AddFactoredRadius|" + planet.name + " planetRawData:" + ((planetRawData != null)?"PlanetRawData Exists":"PlanetRawData Null"));
            if (planet == null) { GS2.Warn("planet Null"); return; }
            if (planetRawData == null) { GS2.Warn($"RawData Null for planet {planet.name} of radius {planet.radius}"); return; }
            float scaleFactored = planet.GetScaleFactored();
            try
            {
                FactoredRadius[planetRawData] = scaleFactored;
            }
            catch (Exception e)
            {
                GS2.Error(e.Message);
            }
        }

        public static float GetFactoredScale(this PlanetRawData planetRawData) {
            return FactoredRadius.TryGetValue(planetRawData, out var result) ? result : 1f;
        }

        public static int GetModPlaneInt(this PlanetRawData planetRawData, int index) {
            float baseHeight = 20;

            baseHeight += planetRawData.GetFactoredScale() * 200 * 100;

            return (int) (((planetRawData.modData[index >> 1] >> (((index & 1) << 2) + 2)) & 3) * 133 + baseHeight);
        }
    }
}