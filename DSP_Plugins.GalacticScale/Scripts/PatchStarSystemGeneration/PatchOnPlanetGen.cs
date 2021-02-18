using System;
using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Random = System.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [HarmonyPatch(typeof(PlanetGen))]
    public class PatchOnPlanetGen {
        /*
                 * PlanetGen.CreatePlanet(galaxy, star, gameDesc, i, 0, 3, 1, false, info_seed, gen_seed)
                 * galaxy , star, gameDesc : internal data
                 * index : index of the planet
                 * orbit around : if it's a moon then decide around what index it's orbiting around if it's a planet : put 0 --> 0 is the index of the star
                 * orbit index : the index of the orbit chosen, here it's the index in the _orbitRadiusArrayPlanetList
                 * number : index on the local system. if it's a moon with number 1 : it's the first moon around the planet on orbitIndex  
                 * gasGiant : is it a gas Giant ?
                 * info_seed, gen_seed : internal random numbers
                 */

        [HarmonyPrefix]
        [HarmonyPatch("CreatePlanet")]
        public static bool CreatePlanet(
            GalaxyData galaxy,
            StarData star,
            GameDesc gameDesc,
            int index,
            int orbitAround,
            int orbitIndex,
            int number,
            bool gasGiant,
            int info_seed,
            int gen_seed,
            ref PlanetData __result) {
            Patch.Debug("CreatePlanet -----", LogLevel.Debug, Patch.DebugPlanetGen);
            __result = ReworkPlanetGen.ReworkCreatePlanet(ref galaxy, ref star, ref gameDesc, index, orbitAround, orbitIndex, number, gasGiant, info_seed, gen_seed);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetPlanetTheme")]
        public static bool SetPlanetTheme(
            PlanetData planet,
            StarData star,
            GameDesc game_desc,
            int set_theme,
            int set_algo,
            double rand1,
            double rand2,
            double rand3,
            double rand4,
            int theme_seed,
            ref List<int> ___tmp_theme) {
            Patch.Debug("SetPlanetTheme -----", LogLevel.Debug, Patch.DebugPlanetGen);
            if (set_theme > 0) {
                planet.theme = set_theme;
                Patch.Debug("1 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                    Patch.DebugPlanetGen);
            }
            else {
                Patch.Debug("2 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                    Patch.DebugPlanetGen);
                if (___tmp_theme == null) {
                    ___tmp_theme = new List<int>();
                }
                else {
                    ___tmp_theme.Clear();
                }

                Patch.Debug("3 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                    Patch.DebugPlanetGen);
                int[] themeIds = game_desc.themeIds;
                int length = themeIds.Length;
                Patch.Debug("4 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                    Patch.DebugPlanetGen);
                for (int index1 = 0; index1 < length; ++index1) {
                    ThemeProto themeProto = LDB.themes.Select(themeIds[index1]);
                    Patch.Debug("4-1 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                        Patch.DebugPlanetGen);
                    bool flag = false;
                    if (planet.star.index == 0 && planet.type == EPlanetType.Ocean) {
                        if (themeProto.Distribute == EThemeDistribute.Birth)
                            flag = true;
                    }
                    else if (themeProto.PlanetType == planet.type &&
                             (double) themeProto.Temperature * (double) planet.temperatureBias >= -0.100000001490116) {
                        if (planet.star.index == 0) {
                            if (themeProto.Distribute == EThemeDistribute.Default)
                                flag = true;
                        }
                        else if (themeProto.Distribute != EThemeDistribute.Birth)
                            flag = true;
                    }
                    Patch.Debug("4-2 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                        Patch.DebugPlanetGen);
                    Patch.Debug("planet.star.planets --> "+ planet.star.planets.Length, LogLevel.Debug,
                        Patch.DebugPlanetGen);
                    if (flag) {
                        for (int index2 = 0; index2 < planet.index; ++index2) {
                            Patch.Debug("index : --> "+index2, LogLevel.Debug,
                                Patch.DebugPlanetGen);
                            Patch.Debug("planet.star.planets[index2]: --> "+planet.star.planets[index2], LogLevel.Debug,
                             Patch.DebugPlanetGen);
                            
                            Patch.Debug("planet.star.planets[index2] name: --> "+planet.star.planets[index2].name, LogLevel.Debug,
                             Patch.DebugPlanetGen); 
                            Patch.Debug("planet.star.planets[index2] type: --> "+planet.star.planets[index2].type, LogLevel.Debug,
                             Patch.DebugPlanetGen); 
                            
                            Patch.Debug("planet.star.planets[index2] theme: --> "+planet.star.planets[index2].theme, LogLevel.Debug,
                             Patch.DebugPlanetGen);
                            if (planet.star.planets[index2].theme == themeProto.ID) {
                                flag = false;
                                break;
                            }
                        }
                    }
                    Patch.Debug("4-3 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                        Patch.DebugPlanetGen);

                    if (flag) {
                        ___tmp_theme.Add(themeProto.ID);
                    }
                    Patch.Debug("4-4 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                        Patch.DebugPlanetGen);
                }

                Patch.Debug("5 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                    Patch.DebugPlanetGen);

                if (___tmp_theme.Count == 0) {
                    for (int index1 = 0; index1 < length; ++index1) {
                        ThemeProto themeProto = LDB.themes.Select(themeIds[index1]);
                        bool flag = themeProto.PlanetType == EPlanetType.Desert;
                        if (flag) {
                            for (int index2 = 0; index2 < planet.index; ++index2) {
                                if (planet.star.planets[index2].theme == themeProto.ID) {
                                    flag = false;
                                    break;
                                }
                            }
                        }

                        if (flag) {
                            ___tmp_theme.Add(themeProto.ID);
                        }
                    }
                }

                Patch.Debug("6 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                    Patch.DebugPlanetGen);

                if (___tmp_theme.Count == 0) {
                    for (int index = 0; index < length; ++index) {
                        ThemeProto themeProto = LDB.themes.Select(themeIds[index]);
                        if (themeProto.PlanetType == EPlanetType.Desert) {
                            ___tmp_theme.Add(themeProto.ID);
                        }
                    }
                }

                Patch.Debug("7 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                    Patch.DebugPlanetGen);

                planet.theme =
                    ___tmp_theme[(int) (rand1 * (double) ___tmp_theme.Count) % ___tmp_theme.Count];
                Patch.Debug("8 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                    Patch.DebugPlanetGen);
            }

            ThemeProto themeProto1 = LDB.themes.Select(planet.theme);
            Patch.Debug("9 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                Patch.DebugPlanetGen);
            if (set_algo > 0) {
                planet.algoId = set_algo;
            }
            else {
                planet.algoId = 0;
                if (themeProto1 != null && themeProto1.Algos != null && themeProto1.Algos.Length > 0) {
                    planet.algoId =
                        themeProto1.Algos[(int) (rand2 * (double) themeProto1.Algos.Length) % themeProto1.Algos.Length];
                    planet.mod_x = (double) themeProto1.ModX.x +
                                   rand3 * ((double) themeProto1.ModX.y - (double) themeProto1.ModX.x);
                    planet.mod_y = (double) themeProto1.ModY.x +
                                   rand4 * ((double) themeProto1.ModY.y - (double) themeProto1.ModY.x);
                }
            }

            Patch.Debug("10 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                Patch.DebugPlanetGen);

            if (themeProto1 == null) {
                return false;
            }

            planet.type = themeProto1.PlanetType;
            planet.ionHeight = themeProto1.IonHeight;
            planet.windStrength = themeProto1.Wind;
            planet.waterHeight = themeProto1.WaterHeight;
            planet.waterItemId = themeProto1.WaterItemId;
            planet.levelized = themeProto1.UseHeightForBuild;
            if (planet.type != EPlanetType.Gas) {
                return false;
            }

            int length1 = themeProto1.GasItems.Length;
            int length2 = themeProto1.GasSpeeds.Length;
            int[] numArray1 = new int[length1];
            float[] numArray2 = new float[length2];
            float[] numArray3 = new float[length1];
            for (int index = 0; index < length1; ++index) {
                numArray1[index] = themeProto1.GasItems[index];
            }

            double num1 = 0.0;
            System.Random random = new System.Random(theme_seed);
            for (int index = 0; index < length2; ++index) {
                float num2 = themeProto1.GasSpeeds[index] *
                             (float) (random.NextDouble() * 0.190909147262573 + 0.909090876579285);
                numArray2[index] = num2 * Mathf.Pow(star.resourceCoef, 0.3f);
                ItemProto itemProto = LDB.items.Select(numArray1[index]);
                numArray3[index] = (float) itemProto.HeatValue;
                num1 += (double) numArray3[index] * (double) numArray2[index];
            }

            Patch.Debug("11 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                Patch.DebugPlanetGen);
            planet.gasItems = numArray1;
            planet.gasSpeeds = numArray2;
            planet.gasHeatValues = numArray3;
            planet.gasTotalHeat = num1;
            Patch.Debug("12 /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                Patch.DebugPlanetGen);
            return false;
        }
    }
}