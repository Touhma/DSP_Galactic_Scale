using System;
using System.Collections.Generic;

namespace GalacticScale
{
    public static class PlanetRawDataExtension
    {
        private static readonly Dictionary<PlanetRawData, float> FactoredRadius = new();

        public static void AddFactoredRadius(this PlanetRawData planetRawData, PlanetData planet)
        {
            //GS2.Log("PlanetRawDataExtension|AddFactoredRadius|" + planet.name + " planetRawData:" + ((planetRawData != null)?"PlanetRawData Exists":"PlanetRawData Null"));
            if (planet == null)
            {
                GS2.Warn("planet Null");
                return;
            }

            if (planetRawData == null)
            {
                if (!UIRoot.instance.backToMainMenu) GS2.Warn($"RawData Null for planet {planet.name} of radius {planet.radius}");
                return;
            }

            var scaleFactored = planet.GetScaleFactored();
            //GS2.Log($"Trying to add to dict:{scaleFactored}");
            try
            {
                FactoredRadius[planetRawData] = scaleFactored;
            }
            catch (Exception e)
            {
                GS2.Error(e.Message);
            }
        }

        public static float GetFactoredScale(this PlanetRawData planetRawData)
        {
            //GS2.Warn($"Trying to get factored scale. {FactoredRadius.TryGetValue(planetRawData, out var result)}");
            return FactoredRadius.TryGetValue(planetRawData, out var result) ? result : 1f; //return FactoredRadius.TryGetValue(planetRawData, out var result) ? result : 1f;
        }

        public static int GetModPlaneInt(this PlanetRawData planetRawData, int index)
        {
            float baseHeight = 20;

            baseHeight += planetRawData.GetFactoredScale() * 20000;

            return (int)(((planetRawData.modData[index >> 1] >> (((index & 1) << 2) + 2)) & 3) * 133 + baseHeight);
        }
    }
}