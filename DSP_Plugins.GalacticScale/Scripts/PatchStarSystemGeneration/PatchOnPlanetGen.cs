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
                 * number : index on the local system. if it's a moon with number 1 : it's the first moon around the planet on orbitIndex ( don't start at 0 --> it's the star  ) 
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
            
            __result = ReworkPlanetGen.ReworkCreatePlanet(ref galaxy, ref star, ref gameDesc, index, orbitAround, orbitIndex, number, gasGiant, info_seed, gen_seed);
            return false;
            
            Patch.Debug("CreatePlanet", LogLevel.Debug,
                Patch.DebugPlanetGen);

            PlanetData planet = new PlanetData();
            Random random = new Random(info_seed);


            // we create a planet A
            planet.index = index;
            planet.galaxy = star.galaxy;
            planet.star = star;
            planet.seed = gen_seed;

            // Number of the planet it's orbiting around ( it's a moon of the planet with the number == orbitAround )
            planet.orbitAround = orbitAround;
            // Number of moon of the planet 
            planet.orbitIndex = orbitIndex;
            // index of the planet in the system ? 
            planet.number = number;
            //Entity Id global of the planetoid ? 
            planet.id = star.id * 100 + planet.index + 1;
            StarData[] stars = galaxy.stars;
            //Represent the number of stars in the galaxy 
            // old : num1
            int numberOfPlanetInGalaxy = 0;

            //For all Stars in Galaxy
            foreach (var starData in stars) {
                numberOfPlanetInGalaxy += starData.planetCount;
            }

            // Represent the position of the planet in the entity id array ?
            // old : num2
            int indexOfPlanetInGlobalContext = numberOfPlanetInGalaxy + planet.index;
            // is planet A a Moon of an another planet ?
            if (orbitAround > 0) {
                foreach (var planetData in star.planets) {
                    // planetData.number --> from the func seem to represent the id of the planet 
                    // for planet A we check all planetoids in the system , planet A == planetData.number
                    // if True : Planet A is orbiting around planetData
                    if (orbitAround == planetData.number && planetData.orbitAround == 0) {
                        // planet.orbitAroundPlanet represent the planet of wich planetA is a moon of 
                        planet.orbitAroundPlanet = planetData;
                        if (orbitIndex > 1) {
                            // check here if more that one moon
                            planet.orbitAroundPlanet.singularity |= EPlanetSingularity.MultipleSatellites;
                        }

                        break;
                    }
                }

                Assert.NotNull(planet.orbitAroundPlanet);
            }

            // Name management of the planet A
            string str = star.planetCount > 20 ? (planet.index + 1).ToString() : NameGen.roman[planet.index + 1];
            planet.name = star.name + " " + str + "号星".Translate();

            // Random numbers Party

            //old num3
            double randomNumber1 = random.NextDouble();
            //old num4
            double randomNumber2 = random.NextDouble();
            //old num5
            double randomNumber3 = random.NextDouble();
            //old num6
            double randomNumber4 = random.NextDouble();
            //old num7
            double randomNumber5 = random.NextDouble();
            //old num8
            double randomNumber6 = random.NextDouble();
            //old num9
            double randomNumber7 = random.NextDouble();
            //old num10
            double randomNumber8 = random.NextDouble();
            //old num11
            double randomNumber9 = random.NextDouble();
            //old num12
            double randomNumber10 = random.NextDouble();
            //old num13
            double randomNumber11 = random.NextDouble();
            //old num14
            double randomNumber12 = random.NextDouble();
            //old num15
            double randomNumber13 = random.NextDouble();


            double rand1 = random.NextDouble();
            double rand2 = random.NextDouble();
            double rand3 = random.NextDouble();
            double rand4 = random.NextDouble();

            // Random planet biome ?
            int theme_seed = random.Next();
            // Randomizer of the orbit ?
            float baselineOrbitVariation = Mathf.Pow(1.2f, (float) (randomNumber1 * (randomNumber2 - 0.5) * 0.5));

            // Management of the orbits
            float orbitRadius;
            if (star.index == 0) {
                Patch.Debug("-------------------", LogLevel.Debug, Patch.DebugPlanetGen);
                Patch.Debug("planet.index : " + planet.index, LogLevel.Debug, Patch.DebugPlanetGen);
                Patch.Debug("is a moon : " + (orbitAround > 0), LogLevel.Debug, Patch.DebugPlanetGen);
                Patch.Debug("orbitIndex : " + orbitIndex, LogLevel.Debug, Patch.DebugPlanetGen);
                Patch.Debug("-------------------", LogLevel.Debug, Patch.DebugPlanetGen);
            }

            if (orbitAround == 0) {
                // If PlanetA is orbiting around the star
                // predefined orbit size for x star size and orbit index x


                float baselineOrbitSize = Patch.OrbitRadiusPlanetArray[orbitIndex] * star.orbitScaler;
                float orbitSize = (float) ((baselineOrbitVariation - 1.0) / Mathf.Max(1f, baselineOrbitSize) + 1.0);
                orbitRadius = baselineOrbitSize * orbitSize;
            }
            else {
                // If PlanetA is a moon of planet.orbitAroundPlanet
                orbitRadius = (float) (((1600.0 * orbitIndex + 200.0) * Mathf.Pow(star.orbitScaler, 0.3f) *
                    Mathf.Lerp(baselineOrbitVariation, 1f, 0.5f) + planet.orbitAroundPlanet.realRadius) / 40000.0);
            }


            planet.orbitRadius = orbitRadius;
            planet.orbitInclination = (float) (randomNumber3 * 16.0 - 8.0);
            // if it's a moon the orbit inclinaison is bigger
            if (orbitAround > 0) {
                planet.orbitInclination *= 2.2f;
            }

            planet.orbitLongitude = (float) (randomNumber4 * 360.0);
            if (star.type >= EStarType.NeutronStar) {
                // more inclinaison if around neutronStar
                if (planet.orbitInclination > 0.0) {
                    planet.orbitInclination += 3f;
                }
                else {
                    planet.orbitInclination -= 3f;
                }
            }

            // calculation of the orbitalPeriod of the planet , first case if it's a moon , second if it's in orbit around the star
            planet.orbitalPeriod = planet.orbitAroundPlanet != null
                ? Math.Sqrt(39.4784176043574 * orbitRadius * orbitRadius * orbitRadius / 1.08308421068537E-08)
                : Math.Sqrt(39.4784176043574 * orbitRadius * orbitRadius * orbitRadius /
                            (1.35385519905204E-06 * star.mass));
            planet.orbitPhase = (float) (randomNumber5 * 360.0);

            // Definition of the obliquity of the orbit of the planet 
            if (randomNumber13 < 0.0399999991059303) {
                planet.obliquity = (float) (randomNumber6 * (randomNumber7 - 0.5) * 39.9);
                if (planet.obliquity < 0.0) {
                    planet.obliquity -= 70f;
                }
                else {
                    planet.obliquity += 70f;
                }

                planet.singularity |= EPlanetSingularity.LaySide;
            }
            else if (randomNumber13 < 0.100000001490116) {
                planet.obliquity = (float) (randomNumber6 * (randomNumber7 - 0.5) * 80.0);
                if (planet.obliquity < 0.0) {
                    planet.obliquity -= 30f;
                }
                else {
                    planet.obliquity += 30f;
                }
            }
            else {
                planet.obliquity = (float) (randomNumber6 * (randomNumber7 - 0.5) * 60.0);
            }

            planet.rotationPeriod = (randomNumber8 * randomNumber9 * 1000.0 + 400.0) *
                                    (orbitAround != 0 ? 1.0 : Mathf.Pow(orbitRadius, 0.25f)) *
                                    (!gasGiant ? 1.0 : 0.200000002980232);


            if (!gasGiant) {
                if (star.type == EStarType.WhiteDwarf) {
                    planet.rotationPeriod *= 0.5;
                }
                else if (star.type == EStarType.NeutronStar) {
                    planet.rotationPeriod *= 0.200000002980232;
                }
                else if (star.type == EStarType.BlackHole) {
                    planet.rotationPeriod *= 0.150000005960464;
                }
            }

            planet.rotationPhase = (float) (randomNumber10 * 360.0);
            planet.sunDistance = orbitAround != 0 ? planet.orbitAroundPlanet.orbitRadius : planet.orbitRadius;

            // if planetA is a moon we take the "mother" planet orbit if no we take planetA orbit
            // old num17
            double orbitalPeriodAroundStar =
                orbitAround != 0 ? planet.orbitAroundPlanet.orbitalPeriod : planet.orbitalPeriod;

            planet.rotationPeriod = 1.0 / (1.0 / orbitalPeriodAroundStar + 1.0 / planet.rotationPeriod);

            
            // Define if tidally locked
            if (orbitAround == 0 && orbitIndex <= 4 && !gasGiant) {
                if (randomNumber13 > 0.959999978542328) {
                    planet.obliquity *= 0.01f;
                    planet.rotationPeriod = planet.orbitalPeriod;
                    planet.singularity |= EPlanetSingularity.TidalLocked;
                }
                else if (randomNumber13 > 0.930000007152557) {
                    planet.obliquity *= 0.1f;
                    planet.rotationPeriod = planet.orbitalPeriod * 0.5;
                    planet.singularity |= EPlanetSingularity.TidalLocked2;
                }
                else if (randomNumber13 > 0.899999976158142) {
                    planet.obliquity *= 0.2f;
                    planet.rotationPeriod = planet.orbitalPeriod * 0.25;
                    planet.singularity |= EPlanetSingularity.TidalLocked4;
                }
            }

            // Define if the orbit is reverse of the rest of the system

            if (randomNumber13 > 0.85 && randomNumber13 <= 0.9) {
                planet.rotationPeriod = -planet.rotationPeriod;
                planet.singularity |= EPlanetSingularity.ClockwiseRotate;
            }

            planet.runtimeOrbitRotation = Quaternion.AngleAxis(planet.orbitLongitude, Vector3.up) *
                                          Quaternion.AngleAxis(planet.orbitInclination, Vector3.forward);
            if (planet.orbitAroundPlanet != null) {
                planet.runtimeOrbitRotation =
                    planet.orbitAroundPlanet.runtimeOrbitRotation * planet.runtimeOrbitRotation;
            }

            planet.runtimeSystemRotation = planet.runtimeOrbitRotation *
                                           Quaternion.AngleAxis(planet.obliquity, Vector3.forward);

            
            /***********************************************************************************************/


            float habitableRadius = star.habitableRadius;

            if (gasGiant) {
                planet.type = EPlanetType.Gas;
                planet.radius = 80f;
                planet.scale = 10f;
                planet.habitableBias = 100f;
            }
            else {
                // for 1024 stars : 297
                //old num16
                float nbStarsBaseline = Mathf.Ceil(star.galaxy.starCount * 0.29f);
                if (nbStarsBaseline < 11.0) {
                    nbStarsBaseline = 11f;
                }


                //old num18
                float unHabitableStarsCount = nbStarsBaseline - star.galaxy.habitableCount;
                //old num19
                float starsLeftToGenerate = star.galaxy.starCount - star.index;
                float sunDistance = planet.sunDistance;
                float habitabilityRatioFromDistance = 1000f;
                float ratioPlanetInHabitableZone = 1000f;
                if (habitableRadius > 0.0 && sunDistance > 0.0) {
                    //old f2
                    // 
                    ratioPlanetInHabitableZone = sunDistance / habitableRadius;
                    //old num20
                    habitabilityRatioFromDistance = Mathf.Abs(Mathf.Log(ratioPlanetInHabitableZone));
                }

                //old num21
                float habitabilityBaseline = Mathf.Clamp(Mathf.Sqrt(habitableRadius), 1f, 2f) - 0.04f;
                //old num22
                float habitabilityRelativeRandomizer =
                    Mathf.Clamp(Mathf.Lerp(unHabitableStarsCount / starsLeftToGenerate, 0.35f, 0.5f), 0.08f, 0.8f);
                planet.habitableBias = habitabilityRatioFromDistance * habitabilityBaseline;
                planet.temperatureBias =
                    (float) (1.20000004768372 / (ratioPlanetInHabitableZone + 0.200000002980232) - 1.0);
                //old num23
                float earthLikePlanetBias =
                    Mathf.Pow(Mathf.Clamp01(planet.habitableBias / habitabilityRelativeRandomizer),
                        habitabilityRelativeRandomizer * 10f);
                
                // Birth planet stuff here
                if (randomNumber11 > earthLikePlanetBias && star.index > 0 ||
                    planet.orbitAround > 0 && planet.orbitIndex == 1 && star.index == 0) {
                    planet.type = EPlanetType.Ocean;
                    ++star.galaxy.habitableCount;
                }
                // if very close to the star
                else if (ratioPlanetInHabitableZone < 0.833333015441895) {
                    // old num24
                    float hotPlanetsBias = Mathf.Max(0.15f,
                        (float) (ratioPlanetInHabitableZone * 2.5 - 0.850000023841858));
                    planet.type = randomNumber12 >= (double) hotPlanetsBias
                        ? EPlanetType.Vocano
                        : EPlanetType.Desert;
                }
                // if  close to the star & not habitable
                else if (ratioPlanetInHabitableZone < 1.20000004768372) {
                    planet.type = EPlanetType.Desert;
                }
                // if further away 
                else {
                    float num24 = (float) (0.899999976158142 / ratioPlanetInHabitableZone - 0.100000001490116);
                    planet.type = randomNumber12 >= (double) num24 ? EPlanetType.Ice : EPlanetType.Desert;
                }

                //radius of the planet
                planet.radius = 200f;
            }

            if (planet.type != EPlanetType.Gas && planet.type != EPlanetType.None) {
                planet.precision = 200;
                planet.segment = 5;
            }
            else {
                planet.precision = 64;
                planet.segment = 2;
            }

            planet.luminosity = Mathf.Pow(planet.star.lightBalanceRadius / (planet.sunDistance + 0.01f), 0.6f);
            if (planet.luminosity > 1.0) {
                planet.luminosity = Mathf.Log(planet.luminosity) + 1f;
                planet.luminosity = Mathf.Log(planet.luminosity) + 1f;
                planet.luminosity = Mathf.Log(planet.luminosity) + 1f;
            }

            planet.luminosity = Mathf.Round(planet.luminosity * 100f) / 100f;
            Patch.Debug("Name : " + planet.name, LogLevel.Debug, Patch.DebugPlanetGen);
            PlanetGen.SetPlanetTheme(planet, star, gameDesc, 0, 0, rand1, rand2, rand3, rand4, theme_seed);
            Patch.Debug("SetPlanetTheme After ", LogLevel.Debug, Patch.DebugPlanetGen);
            star.galaxy.astroPoses[planet.id].uRadius = planet.realRadius;

            if (planet.orbitAround != 0) {
                Patch.Debug("it's a moon ! ", LogLevel.Debug, Patch.DebugPlanetGen);
                Patch.Debug("Patching the radius :", LogLevel.Debug, Patch.DebugPlanetGen);
                Patch.Debug("Patching the scale :", LogLevel.Debug, Patch.DebugPlanetGen);

                planet.radius = 100f;
                planet.scale = 1f;
                planet.precision = 100;
                planet.orbitRadius = 0.048125f;
            }


            __result = planet;
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