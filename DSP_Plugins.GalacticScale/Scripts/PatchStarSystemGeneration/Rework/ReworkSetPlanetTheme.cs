using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    public class ReworkSetPlanetTheme {
        public static int[] GasThemes { get; } = { 2, 3, 4, 5 };

        public static void SetPlanetTheme(
            ref PlanetData planet,
            ref StarData star,
            int theme_seed
        ) {
            planet.theme = GasThemes[UnityRandom.Range(0, GasThemes.Length - 1)];
            ThemeProto themeProto1 = LDB.themes.Select(planet.theme);
            int length1 = themeProto1.GasItems.Length;
            int length2 = themeProto1.GasSpeeds.Length;
            int[] numArray1 = new int[length1];
            float[] numArray2 = new float[length2];
            float[] numArray3 = new float[length1];
            for (int index = 0; index < length1; ++index)
                numArray1[index] = themeProto1.GasItems[index];
            double num1 = 0.0;
            System.Random random = new System.Random(theme_seed);
            for (int index = 0; index < length2; ++index)
            {
                float num2 = themeProto1.GasSpeeds[index] * (float) (random.NextDouble() * 0.190909147262573 + 0.909090876579285);
                numArray2[index] = num2 * Mathf.Pow(star.resourceCoef, 0.3f);
                ItemProto itemProto = LDB.items.Select(numArray1[index]);
                numArray3[index] = (float) itemProto.HeatValue;
                num1 += (double) numArray3[index] * (double) numArray2[index];
            }
            planet.gasItems = numArray1;
            planet.gasSpeeds = numArray2;
            planet.gasHeatValues = numArray3;
            planet.gasTotalHeat = num1;
        }
    }
}