using System;
using System.Collections.Generic;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;
using System.IO;
namespace GalacticScale
{
    public static partial class GS2
    {
        public static void SetPlanetTheme(
         PlanetData planet,
         StarData star,
         GameDesc game_desc,
         ThemeProto set_theme,
         int set_algo,
         double rand1,
         double rand2,
         double rand3,
         double rand4,
         int theme_seed)
        {
            planet.theme = set_theme.ID;

            //else
            //{
            //    if (PlanetGen.tmp_theme == null)
            //        PlanetGen.tmp_theme = new List<int>();
            //    else
            //        PlanetGen.tmp_theme.Clear();
            //    int[] themeIds = game_desc.themeIds;
            //    int length = themeIds.Length;
            //    for (int index1 = 0; index1 < length; ++index1)
            //    {
            //        ThemeProto themeProto = LDB.themes.Select(themeIds[index1]);
            //        Patch.Debug("Theme " + themeProto.DisplayName + " " + index1);
            //        bool flag = false;
            //        if (planet.star.index == 0 && planet.type == EPlanetType.Ocean)
            //        {
            //            if (themeProto.Distribute == EThemeDistribute.Birth)
            //                flag = true;
            //        }
            //        else if (themeProto.PlanetType == planet.type && (double)themeProto.Temperature * (double)planet.temperatureBias >= -0.100000001490116)
            //        {
            //            if (planet.star.index == 0)
            //            {
            //                if (themeProto.Distribute == EThemeDistribute.Default)
            //                    flag = true;
            //            }
            //            else if (themeProto.Distribute != EThemeDistribute.Birth)
            //                flag = true;
            //        }
            //        if (flag)
            //        {
            //            for (int index2 = 0; index2 < planet.index; ++index2)
            //            {
            //                if (planet.star.planets[index2].theme == themeProto.ID)
            //                {
            //                    flag = false;
            //                    break;
            //                }
            //            }
            //        }
            //        if (flag)
            //            PlanetGen.tmp_theme.Add(themeProto.ID);
            //    }
            //    if (PlanetGen.tmp_theme.Count == 0)
            //    {
            //        for (int index1 = 0; index1 < length; ++index1)
            //        {
            //            ThemeProto themeProto = LDB.themes.Select(themeIds[index1]);
            //            bool flag = false;
            //            if (themeProto.PlanetType == EPlanetType.Desert)
            //                flag = true;
            //            if (flag)
            //            {
            //                for (int index2 = 0; index2 < planet.index; ++index2)
            //                {
            //                    if (planet.star.planets[index2].theme == themeProto.ID)
            //                    {
            //                        flag = false;
            //                        break;
            //                    }
            //                }
            //            }
            //            if (flag)
            //                PlanetGen.tmp_theme.Add(themeProto.ID);
            //        }
            //    }
            //    if (PlanetGen.tmp_theme.Count == 0)
            //    {
            //        for (int index = 0; index < length; ++index)
            //        {
            //            ThemeProto themeProto = LDB.themes.Select(themeIds[index]);
            //            if (themeProto.PlanetType == EPlanetType.Desert)
            //                PlanetGen.tmp_theme.Add(themeProto.ID);
            //        }
            //    }
            //    planet.theme = PlanetGen.tmp_theme[(int)(rand1 * (double)PlanetGen.tmp_theme.Count) % PlanetGen.tmp_theme.Count];
            //}
            ThemeProto themeProto1 = LDB.themes.Select(planet.theme);
            //DumpObjectToJson(Path.Combine(DataDir , themeProto1.displayName + ".json"), themeProto1);
            //if (themeProto1.oceanMat) { 
            //    Patch.Debug(themeProto1.oceanMat.color + "ocean"); 
            //    themeProto1.oceanMat.color = Color.red; 
            //}
            //if (themeProto1.atmosMat) { 
            //    Patch.Debug(themeProto1.atmosMat.color + "at"); 
            //    themeProto1.atmosMat.color = Color.green;
            //}
            //if (themeProto1.terrainMat) { Patch.Debug(themeProto1.terrainMat.color + "tera"); themeProto1.terrainMat.color = Color.red;}
            //if (themeProto1.lowMat) { Patch.Debug(themeProto1.lowMat.color + "ocean"); themeProto1.lowMat.color = Color.magenta;}
            //if (themeProto1.thumbMat) { Patch.Debug(themeProto1.thumbMat.color + "thumb"); if (themeProto1.thumbMat) themeProto1.thumbMat.color = Color.yellow;}
            //if (themeProto1.minimapMat) {  Patch.Debug(themeProto1.minimapMat.color + "mini"); themeProto1.minimapMat.color = Color.red;}
            
            if (set_algo > 0)
            {
                planet.algoId = set_algo;
            }
            else
            {
                planet.algoId = 0;
                if (themeProto1 != null && themeProto1.Algos != null && themeProto1.Algos.Length > 0)
                {
                    planet.algoId = themeProto1.Algos[(int)(rand2 * (double)themeProto1.Algos.Length) % themeProto1.Algos.Length];
                    planet.mod_x = (double)themeProto1.ModX.x + rand3 * ((double)themeProto1.ModX.y - (double)themeProto1.ModX.x);
                    planet.mod_y = (double)themeProto1.ModY.x + rand4 * ((double)themeProto1.ModY.y - (double)themeProto1.ModY.x);
                }
            }
            if (themeProto1 == null)
                return;
            planet.type = themeProto1.PlanetType;
            planet.ionHeight = themeProto1.IonHeight;
            planet.windStrength = themeProto1.Wind;
            planet.waterHeight = themeProto1.WaterHeight;
            planet.waterItemId = themeProto1.WaterItemId;
            planet.levelized = themeProto1.UseHeightForBuild;
            if (planet.type != EPlanetType.Gas)
                return;
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
                float num2 = themeProto1.GasSpeeds[index] * (float)(random.NextDouble() * 0.190909147262573 + 0.909090876579285);
                numArray2[index] = num2 * Mathf.Pow(star.resourceCoef, 0.3f);
                ItemProto itemProto = LDB.items.Select(numArray1[index]);
                numArray3[index] = (float)itemProto.HeatValue;
                num1 += (double)numArray3[index] * (double)numArray2[index];
            }
            planet.gasItems = numArray1;
            planet.gasSpeeds = numArray2;
            planet.gasHeatValues = numArray3;
            planet.gasTotalHeat = num1;
        }
    }
}