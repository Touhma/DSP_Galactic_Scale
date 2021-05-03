using System;
using System.Collections.Generic;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.Bootstrap;
using System.IO;
namespace GalacticScale
{
    public static partial class GS2
    {
        public static void SetPlanetTheme(
         PlanetData planet,
         GSPlanet gsPlanet)
        {
            GS2.prog("SetPlanetTheme("+ gsPlanet.Theme + ")");
            int seed = 0;
            GSTheme gsTheme = GS2.planetThemes[gsPlanet.Theme];
            GS2.prog("SetPlanetTheme");
            //ThemeProto themeProto = LDB.themes.Select(gsTheme.LDBThemeId);
            GS2.prog("SetPlanetTheme");
            if (gsPlanet.Seed > -1) seed = gsPlanet.Seed;
            GS2.prog("SetPlanetTheme"); 
            System.Random rand = new System.Random(seed);
            GS2.prog("SetPlanetTheme");
         //int set_algo = -1;
            double rand2 = rand.NextDouble();
            double rand3 = rand.NextDouble();
            double rand4 = rand.NextDouble();
            GS2.prog("SetPlanetTheme");
           // Log(themeProto.ToString());
            planet.theme = gsTheme.LDBThemeId;// themeProto.ID;

            GS2.prog("SetPlanetTheme");
            //if (set_algo > 0)
            //{
            planet.algoId = gsTheme.algo; //set_algo;
            //}
            planet.mod_x = (double)gsTheme.ModX.x + rand3 * ((double)gsTheme.ModX.y - (double)gsTheme.ModX.x);
            planet.mod_y = (double)gsTheme.ModY.x + rand4 * ((double)gsTheme.ModY.y - (double)gsTheme.ModY.x);

            GS2.prog("SetPlanetTheme");
            planet.type = gsTheme.type;
            planet.ionHeight = gsTheme.IonHeight;
            planet.windStrength = gsTheme.Wind;
            Log("planet.windStrength = " + planet.windStrength);
            planet.waterHeight = gsTheme.WaterHeight;
            Log("waterheight = " + planet.waterHeight);
            planet.waterItemId = gsTheme.WaterItemId;
            Log("waterItemId = " + planet.waterItemId);
            planet.levelized = gsTheme.UseHeightForBuild;
            GS2.prog("SetPlanetTheme");
            if (planet.type != EPlanetType.Gas)
                return;
            GS2.prog("SetPlanetTheme");
            int length1 = gsTheme.GasItems.Length;
            int length2 = gsTheme.GasSpeeds.Length;
            int[] numArray1 = new int[length1];
            float[] numArray2 = new float[length2];
            float[] numArray3 = new float[length1];
            GS2.prog("SetPlanetTheme");
            for (int index = 0; index < length1; ++index)
                numArray1[index] = gsTheme.GasItems[index];
            double num1 = 0.0;
            GS2.prog("SetPlanetTheme");
            for (int index = 0; index < length2; ++index)
            {
                float num2 = gsTheme.GasSpeeds[index] * (float)(rand2 * 0.190909147262573 + 0.909090876579285);
                numArray2[index] = num2 * Mathf.Pow(planet.star.resourceCoef, 0.3f);
                ItemProto itemProto = LDB.items.Select(numArray1[index]);
                numArray3[index] = (float)itemProto.HeatValue;
                num1 += (double)numArray3[index] * (double)numArray2[index];
            }
            GS2.prog("SetPlanetTheme");
            planet.gasItems = numArray1;
            planet.gasSpeeds = numArray2;
            planet.gasHeatValues = numArray3;
            planet.gasTotalHeat = num1;
            GS2.prog("SetPlanetTheme", true);
        }
    }
}