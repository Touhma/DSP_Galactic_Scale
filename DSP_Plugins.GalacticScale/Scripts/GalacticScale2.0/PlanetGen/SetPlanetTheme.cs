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
            ThemeProto themeProto = LDB.themes.Select(gsTheme.LDBThemeId);
            GS2.prog("SetPlanetTheme");
            if (gsPlanet.Seed > -1) seed = gsPlanet.Seed;
            GS2.prog("SetPlanetTheme"); 
            System.Random rand = new System.Random(seed);
            GS2.prog("SetPlanetTheme");
            int set_algo = -1;
            
            double rand2 = rand.NextDouble();
            double rand3 = rand.NextDouble();
            double rand4 = rand.NextDouble();
            GS2.prog("SetPlanetTheme");
            Log(themeProto.ToString());
            planet.theme = themeProto.ID;

            GS2.prog("SetPlanetTheme");
            if (set_algo > 0)
            {
                planet.algoId = set_algo;
            }
            else
            {
                planet.algoId = 0;
                if (themeProto != null && themeProto.Algos != null && themeProto.Algos.Length > 0)
                {
                    planet.algoId = themeProto.Algos[(int)(rand2 * (double)themeProto.Algos.Length) % themeProto.Algos.Length];
                    planet.mod_x = (double)themeProto.ModX.x + rand3 * ((double)themeProto.ModX.y - (double)themeProto.ModX.x);
                    planet.mod_y = (double)themeProto.ModY.x + rand4 * ((double)themeProto.ModY.y - (double)themeProto.ModY.x);
                }
            }
            GS2.prog("SetPlanetTheme");
            if (themeProto == null)
                return;
            GS2.prog("SetPlanetTheme");
            planet.type = themeProto.PlanetType;
            planet.ionHeight = themeProto.IonHeight;
            planet.windStrength = themeProto.Wind;
            Log("planet.windStrength = " + planet.windStrength);
            planet.waterHeight = themeProto.WaterHeight;
            Log("waterheight = " + planet.waterHeight);
            planet.waterItemId = themeProto.WaterItemId;
            Log("waterItemId = " + planet.waterItemId);
            planet.levelized = themeProto.UseHeightForBuild;
            GS2.prog("SetPlanetTheme");
            if (planet.type != EPlanetType.Gas)
                return;
            GS2.prog("SetPlanetTheme");
            int length1 = themeProto.GasItems.Length;
            int length2 = themeProto.GasSpeeds.Length;
            int[] numArray1 = new int[length1];
            float[] numArray2 = new float[length2];
            float[] numArray3 = new float[length1];
            GS2.prog("SetPlanetTheme");
            for (int index = 0; index < length1; ++index)
                numArray1[index] = themeProto.GasItems[index];
            double num1 = 0.0;
            GS2.prog("SetPlanetTheme");
            for (int index = 0; index < length2; ++index)
            {
                float num2 = themeProto.GasSpeeds[index] * (float)(rand2 * 0.190909147262573 + 0.909090876579285);
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