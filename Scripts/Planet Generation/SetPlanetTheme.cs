using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void SetPlanetTheme(PlanetData planet, GSPlanet gsPlanet)
        {
            //Log("Start|" + gsPlanet.Name);
            // var highStopwatch = new HighStopwatch();                                highStopwatch.Begin();


            var seed = 0;
            var gsTheme = GSSettings.ThemeLibrary.Find(gsPlanet.Theme);
            // Log($"Finding Theme {gsTheme.Name} took {highStopwatch.duration:F5} s\r\n");highStopwatch.Begin();
            // var ThemeID = gsTheme.UpdateThemeProtoSet();
            var ThemeID = gsTheme.AddToThemeProtoSet();
            if (gsPlanet.Seed > -1) seed = gsPlanet.Seed;
            // Log($"UpdateProtoSet for {gsTheme.Name} took {highStopwatch.duration:F5} s\r\n");highStopwatch.Begin();

            var rand = new Random(seed);
            var rand2 = rand.NextDouble();
            var rand3 = rand.NextDouble();
            var rand4 = rand.NextDouble();
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
            if (planet.type != EPlanetType.Gas)
                //Log("End");
                // Log($"Rest of setting planetTheme {gsTheme.Name} took {highStopwatch.duration:F5} s\r\n");
                return;

            var length1 = gsTheme.GasItems.Length;
            var length2 = gsTheme.GasSpeeds.Length;
            var numArray1 = new int[length1];
            var numArray2 = new float[length2];
            var numArray3 = new float[length1];
            for (var index = 0; index < length1; ++index) numArray1[index] = gsTheme.GasItems[index];

            var num1 = 0.0;
            for (var index = 0; index < length2; ++index)
            {
                var num2 = gsTheme.GasSpeeds[index] * (float)(rand2 * 0.190909147262573 + 0.909090876579285);
                numArray2[index] = num2 * Mathf.Pow(planet.star.resourceCoef, 0.3f);
                var itemProto = LDB.items.Select(numArray1[index]);
                numArray3[index] = itemProto.HeatValue;
                num1 += numArray3[index] * (double)numArray2[index];
            }

            planet.gasItems = numArray1;
            planet.gasSpeeds = numArray2;
            planet.gasHeatValues = numArray3;
            planet.gasTotalHeat = num1;
            //Log("End");
            // Log($"Rest of setting planetTheme {gsTheme.Name} took {highStopwatch.duration:F5} s\r\n");
        }
    }
}