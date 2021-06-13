using UnityEngine;
namespace GalacticScale {
    public static partial class GS2 {
        public static void SetPlanetTheme(
         PlanetData planet,
         GSPlanet gsPlanet) {
            Log("Start|" + gsPlanet.Name);
            int seed = 0;
            GSTheme gsTheme = GSSettings.ThemeLibrary.Find(gsPlanet.Theme);
            int ThemeID = gsTheme.UpdateThemeProtoSet();
            if (gsPlanet.Seed > -1) {
                seed = gsPlanet.Seed;
            }

            GS2.Random rand = new GS2.Random(seed);
            double rand2 = rand.NextDouble();
            double rand3 = rand.NextDouble();
            double rand4 = rand.NextDouble();
            planet.theme = ThemeID;
            planet.algoId = gsTheme.Algo;
            planet.mod_x = gsTheme.ModX.x + rand3 * (gsTheme.ModX.y - (double)gsTheme.ModX.x);
            planet.mod_y = gsTheme.ModY.x + rand4 * (gsTheme.ModY.y - (double)gsTheme.ModY.x);
            planet.type = gsTheme.PlanetType;
            planet.ionHeight = gsTheme.IonHeight;
            planet.windStrength = gsTheme.Wind;
            planet.waterHeight = gsTheme.WaterHeight;
            planet.waterItemId = gsTheme.WaterItemId;
            planet.levelized = gsTheme.UseHeightForBuild;
            if (planet.type != EPlanetType.Gas) {
                return;
            }

            int length1 = gsTheme.GasItems.Length;
            int length2 = gsTheme.GasSpeeds.Length;
            int[] numArray1 = new int[length1];
            float[] numArray2 = new float[length2];
            float[] numArray3 = new float[length1];
            for (int index = 0; index < length1; ++index) {
                numArray1[index] = gsTheme.GasItems[index];
            }

            double num1 = 0.0;
            for (int index = 0; index < length2; ++index) {
                float num2 = gsTheme.GasSpeeds[index] * (float)(rand2 * 0.190909147262573 + 0.909090876579285);
                numArray2[index] = num2 * Mathf.Pow(planet.star.resourceCoef, 0.3f);
                ItemProto itemProto = LDB.items.Select(numArray1[index]);
                numArray3[index] = itemProto.HeatValue;
                num1 += numArray3[index] * (double)numArray2[index];
            }
            planet.gasItems = numArray1;
            planet.gasSpeeds = numArray2;
            planet.gasHeatValues = numArray3;
            planet.gasTotalHeat = num1;
            Log("End");
        }
    }
}