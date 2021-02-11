using System;
using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Random = System.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [HarmonyPatch(typeof(StarGen))]
    public class PatchOnStarGen {
        [HarmonyPrefix]
        [HarmonyPatch("CreateStarPlanets")]
        public static bool CreateStarPlanets(GalaxyData galaxy, StarData star, GameDesc gameDesc) {
            double[] pgasRef = new double[Patch.StartingSystemPlanetNb];
            string[] pnameRef = new string[Patch.StartingSystemPlanetNb];

            // Random Generator 
            Random random1 = new Random(star.seed);
            random1.Next();
            random1.Next();
            random1.Next();

            // Random Generator 
            Random random2 = new Random(random1.Next());

            //old num1
            double randomNumber1 = random2.NextDouble();

            //old num2
            double randomNumber2 = random2.NextDouble();

            //old num3
            //orbit index rnumber
            double randomNumber3 = random2.NextDouble();

            //old num4
            double randomNumber4 = random2.NextDouble();

            //old num5
            double randomNumber5 = random2.NextDouble();

            //old num6
            double randomNumber6 = random2.NextDouble() * 0.2 + 0.9;

            //old num7
            double randomNumber7 = random2.NextDouble() * 0.2 + 0.9;

            // Generation specific to some star type
            if (star.type == EStarType.BlackHole) {
                star.planetCount = 1;
                star.planets = new PlanetData[star.planetCount];
                int info_seed = random2.Next();
                int gen_seed = random2.Next();
                // create a planet index 0 in the system, not a moon, on the 3rd orbit available, with the number 1 corresponding on the 1th on the orbit ? , not a gas giant
                star.planets[0] =
                    PlanetGen.CreatePlanet(galaxy, star, gameDesc, 0, 0, 3, 1, false, info_seed, gen_seed);
            }

            else if (star.type == EStarType.NeutronStar) {
                star.planetCount = 1;
                star.planets = new PlanetData[star.planetCount];
                int info_seed = random2.Next();
                int gen_seed = random2.Next();
                star.planets[0] =
                    PlanetGen.CreatePlanet(galaxy, star, gameDesc, 0, 0, 3, 1, false, info_seed, gen_seed);
            }

            else if (star.type == EStarType.WhiteDwarf) {
                if (randomNumber1 < 0.699999988079071) {
                    star.planetCount = 1;
                    star.planets = new PlanetData[star.planetCount];
                    int info_seed = random2.Next();
                    int gen_seed = random2.Next();
                    star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 0, 0, 3, 1, false, info_seed,
                        gen_seed);
                }
                else {
                    star.planetCount = 2;
                    star.planets = new PlanetData[star.planetCount];
                    if (randomNumber2 < 0.300000011920929) {
                        int info_seed1 = random2.Next();
                        int gen_seed1 = random2.Next();
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 0, 0, 3, 1, false,
                            info_seed1, gen_seed1);
                        int info_seed2 = random2.Next();
                        int gen_seed2 = random2.Next();
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 1, 0, 4, 2, false,
                            info_seed2, gen_seed2);
                    }

                    else {
                        int info_seed1 = random2.Next();
                        int gen_seed1 = random2.Next();
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 0, 0, 4, 1, true,
                            info_seed1, gen_seed1);
                        int info_seed2 = random2.Next();
                        int gen_seed2 = random2.Next();
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 1, 1, 1, 1, false,
                            info_seed2, gen_seed2);
                    }
                }
            }

            else if (star.type == EStarType.GiantStar) {
                if (randomNumber1 < 0.300000011920929) {
                    star.planetCount = 1;
                    star.planets = new PlanetData[star.planetCount];
                    int info_seed = random2.Next();
                    int gen_seed = random2.Next();
                    star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 0, 0,
                        randomNumber3 <= 0.5 ? 2 : 3, 1,
                        false, info_seed, gen_seed);
                }
                else if (randomNumber1 < 0.800000011920929) {
                    star.planetCount = 2;
                    star.planets = new PlanetData[star.planetCount];
                    if (randomNumber2 < 0.25) {
                        int info_seed1 = random2.Next();
                        int gen_seed1 = random2.Next();
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 0, 0,
                            randomNumber3 <= 0.5 ? 2 : 3,
                            1, false, info_seed1, gen_seed1);
                        int info_seed2 = random2.Next();
                        int gen_seed2 = random2.Next();
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 1, 0,
                            randomNumber3 <= 0.5 ? 3 : 4,
                            2, false, info_seed2, gen_seed2);
                    }
                    else {
                        int info_seed1 = random2.Next();
                        int gen_seed1 = random2.Next();
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 0, 0, 3, 1, true,
                            info_seed1, gen_seed1);
                        int info_seed2 = random2.Next();
                        int gen_seed2 = random2.Next();
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 1, 1, 1, 1, false,
                            info_seed2, gen_seed2);
                    }
                }
                else {
                    star.planetCount = 3;
                    star.planets = new PlanetData[star.planetCount];
                    if (randomNumber2 < 0.150000005960464) {
                        int info_seed1 = random2.Next();
                        int gen_seed1 = random2.Next();
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 0, 0,
                            randomNumber3 <= 0.5 ? 2 : 3,
                            1, false, info_seed1, gen_seed1);
                        int info_seed2 = random2.Next();
                        int gen_seed2 = random2.Next();
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 1, 0,
                            randomNumber3 <= 0.5 ? 3 : 4,
                            2, false, info_seed2, gen_seed2);
                        int info_seed3 = random2.Next();
                        int gen_seed3 = random2.Next();
                        star.planets[2] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 2, 0,
                            randomNumber3 <= 0.5 ? 4 : 5,
                            3, false, info_seed3, gen_seed3);
                    }
                    else if (randomNumber2 < 0.75) {
                        int info_seed1 = random2.Next();
                        int gen_seed1 = random2.Next();
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 0, 0,
                            randomNumber3 <= 0.5 ? 2 : 3,
                            1, false, info_seed1, gen_seed1);
                        int info_seed2 = random2.Next();
                        int gen_seed2 = random2.Next();
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 1, 0, 4, 2, true,
                            info_seed2, gen_seed2);
                        int info_seed3 = random2.Next();
                        int gen_seed3 = random2.Next();
                        star.planets[2] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 2, 2, 1, 1, false,
                            info_seed3, gen_seed3);
                    }
                    else {
                        int info_seed1 = random2.Next();
                        int gen_seed1 = random2.Next();
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 0, 0,
                            randomNumber3 <= 0.5 ? 3 : 4,
                            1, true, info_seed1, gen_seed1);
                        int info_seed2 = random2.Next();
                        int gen_seed2 = random2.Next();
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 1, 1, 1, 1, false,
                            info_seed2, gen_seed2);
                        int info_seed3 = random2.Next();
                        int gen_seed3 = random2.Next();
                        star.planets[2] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, 2, 1, 2, 2, false,
                            info_seed3, gen_seed3);
                    }
                }
            }
            else {
                Array.Clear(pgasRef, 0, pgasRef.Length);
                if (star.index == 0) {
                    star.planetCount = Patch.StartingSystemPlanetNb;
                    List<double> pgaslist = new List<double>();
                    List<string> pnamelist = new List<string>();
                    pgaslist.Add(0); // Mercure
                    pgaslist.Add(0); // Venus
                    pgaslist.Add(0); // Terre
                    pgaslist.Add(0.6); // Luna
                    pgaslist.Add(0); // Mars
                    pgaslist.Add(1.0); // Jupiter
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(1.0); //saturn
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(1.0); //Uranus
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(0.6);
                    pgaslist.Add(1.0); // Neptune
                    pgaslist.Add(0.6);
                    pgaslist.Add(0); // Pluton
                    pgaslist.Add(0); // Makemake
                    pgaslist.Add(0); // Haumea
                    pgaslist.Add(0); // Eris
                    pgaslist.Add(0); // Kuiper
                    pgasRef = pgaslist.ToArray();
                    pnamelist.Add("Mercury"); // 
                    pnamelist.Add("Venus"); // Venus
                    pnamelist.Add("Earth"); // Terre
                    pnamelist.Add("Luna"); // Terre
                    pnamelist.Add("Mars"); // Mars
                    pnamelist.Add("Jupiter"); // Jupiter
                    pnamelist.Add("Io");
                    pnamelist.Add("Europa");
                    pnamelist.Add("Ganymede");
                    pnamelist.Add("Callisto");
                    pnamelist.Add("Saturn"); //saturn
                    pnamelist.Add("Titan");
                    pnamelist.Add("Lapetus");
                    pnamelist.Add("Rhea");
                    pnamelist.Add("Dione");
                    pnamelist.Add("Tethys");
                    pnamelist.Add("Enceladus");
                    pnamelist.Add("Mimas");
                    pnamelist.Add("Uranus"); //Uranus
                    pnamelist.Add("Miranda");
                    pnamelist.Add("Ariel");
                    pnamelist.Add("Umbriel");
                    pnamelist.Add("Titania");
                    pnamelist.Add("Oberon");
                    pnamelist.Add("Neptune"); // Neptune
                    pnamelist.Add("Triton");
                    pnamelist.Add("Pluto"); // Pluton
                    pnamelist.Add("Haumea");
                    pnamelist.Add("Makemake");

                    pnamelist.Add("Eris");
                    pnamelist.Add("Kuiper");
                    pnameRef = pnamelist.ToArray();

                    star.name = "Sol";

                    if (star.index == 0) {
                        Patch.Debug("*************************", LogLevel.Debug, Patch.DebugStarGen);
                    }
                }
                else if (star.spectr == ESpectrType.M) {
                    star.planetCount = randomNumber1 >= 0.1
                        ? (randomNumber1 >= 0.3 ? (randomNumber1 >= 0.8 ? 4 : 3) : 2)
                        : 1;
                    if (star.planetCount <= 3) {
                        pgasRef[0] = 0.2;
                        pgasRef[1] = 0.2;
                    }
                    else {
                        pgasRef[0] = 0.0;
                        pgasRef[1] = 0.2;
                        pgasRef[2] = 0.3;
                    }
                }
                else if (star.spectr == ESpectrType.K) {
                    star.planetCount =
                        randomNumber1 >= 0.1
                            ? (randomNumber1 >= 0.2
                                ? (randomNumber1 >= 0.7 ? (randomNumber1 >= 0.95 ? 5 : 4) : 3)
                                : 2)
                            : 1;
                    if (star.planetCount <= 3) {
                        pgasRef[0] = 0.18;
                        pgasRef[1] = 0.18;
                    }
                    else {
                        pgasRef[0] = 0.0;
                        pgasRef[1] = 0.18;
                        pgasRef[2] = 0.28;
                        pgasRef[3] = 0.28;
                    }
                }
                else if (star.spectr == ESpectrType.G) {
                    star.planetCount = randomNumber1 >= 0.4 ? (randomNumber1 >= 0.9 ? 5 : 4) : 3;
                    if (star.planetCount <= 3) {
                        pgasRef[0] = 0.18;
                        pgasRef[1] = 0.18;
                    }
                    else {
                        pgasRef[0] = 0.0;
                        pgasRef[1] = 0.2;
                        pgasRef[2] = 0.3;
                        pgasRef[3] = 0.3;
                    }
                }
                else if (star.spectr == ESpectrType.F) {
                    star.planetCount = randomNumber1 >= 0.35 ? (randomNumber1 >= 0.8 ? 5 : 4) : 3;
                    if (star.planetCount <= 3) {
                        pgasRef[0] = 0.2;
                        pgasRef[1] = 0.2;
                    }
                    else {
                        pgasRef[0] = 0.0;
                        pgasRef[1] = 0.22;
                        pgasRef[2] = 0.31;
                        pgasRef[3] = 0.31;
                    }
                }
                else if (star.spectr == ESpectrType.A) {
                    star.planetCount = randomNumber1 >= 0.3 ? (randomNumber1 >= 0.75 ? 5 : 4) : 3;
                    if (star.planetCount <= 3) {
                        pgasRef[0] = 0.2;
                        pgasRef[1] = 0.2;
                    }
                    else {
                        pgasRef[0] = 0.1;
                        pgasRef[1] = 0.28;
                        pgasRef[2] = 0.3;
                        pgasRef[3] = 0.35;
                    }
                }
                else if (star.spectr == ESpectrType.B) {
                    star.planetCount = randomNumber1 >= 0.3 ? (randomNumber1 >= 0.75 ? 6 : 5) : 4;
                    if (star.planetCount <= 3) {
                        pgasRef[0] = 0.2;
                        pgasRef[1] = 0.2;
                    }
                    else {
                        pgasRef[0] = 0.1;
                        pgasRef[1] = 0.22;
                        pgasRef[2] = 0.28;
                        pgasRef[3] = 0.35;
                        pgasRef[4] = 0.35;
                    }
                }
                else if (star.spectr == ESpectrType.O) {
                    star.planetCount = randomNumber1 >= 0.5 ? 6 : 5;
                    pgasRef[0] = 0.1;
                    pgasRef[1] = 0.2;
                    pgasRef[2] = 0.25;
                    pgasRef[3] = 0.3;
                    pgasRef[4] = 0.32;
                    pgasRef[5] = 0.35;
                }
                else {
                    star.planetCount = 12;
                }
/* *************************************************************************** */

                star.planets = new PlanetData[star.planetCount];
                //old num8
                int currentIndexOfPlanet = 0;
                //old num9
                int currentIndexOfMoons = 0;
                int orbitAround = 0;
                int currentIndexOfBodies = 0;

                for (int index = 0; index < star.planetCount; ++index) {
                    if (star.index == 0) {
                        //  UnityEngine.Debug.Log("#" + index + "#");
                    }

                    int info_seed = random2.Next();
                    int gen_seed = random2.Next();
                    //old num11
                    double randomNumber8 = random2.NextDouble();
                    //old num12
                    double randomNumber9 = random2.NextDouble();
                    bool gasGiant = false;
                    if (star.index == 0) {
                        Patch.Debug("#pgasRef -->  " + pgasRef[index] + "#", LogLevel.Debug, Patch.DebugStarGen);
                    }

                    if (pgasRef[index] == 1.0) {
                        gasGiant = true;
                    }

                    if (orbitAround == 0) {
                        // the last planet of a system can't be a gas giant
                        // if randomNumber8 less than pgasRef[index] --> then it's a gas giant 


                        for (; star.index != 0; ++currentIndexOfBodies) {
                            // old num13
                            int planetCountLeftToGenerate = star.planetCount - index;
                            // was 9 in the condition under
                            int hardLimit = 9;
                            int num14 = hardLimit - currentIndexOfBodies;
                            if (num14 > planetCountLeftToGenerate) {
                                float ratioPlanetGenerated = planetCountLeftToGenerate / (float) num14;

                                // if not a giant ? 
                                float num15 = currentIndexOfBodies <= 3
                                    ? Mathf.Lerp(ratioPlanetGenerated, 1f, 0.15f) + 0.01f
                                    : Mathf.Lerp(ratioPlanetGenerated, 1f, 0.45f) + 0.01f;
                                if (random2.NextDouble() < num15) {
                                    goto label_63;
                                }
                            }
                            else
                                goto label_63;
                        }
                    }


                    label_63:
                    if (pgasRef[index] == 1.0) {
                        gasGiant = true;
                    }

                    if (pgasRef[index] != 0.6) {
                        if (star.index == 0) {
                            Patch.Debug("#currentIndexOfPlanet ++ #", LogLevel.Debug, Patch.DebugStarGen);
                        }

                        orbitAround = 0;
                        currentIndexOfMoons = 0;
                        currentIndexOfPlanet++;
                    }
                    else if (pgasRef[index] == 0.6) {
                        if (star.index == 0) {
                            Patch.Debug("#currentIndexOfMoon ++ #", LogLevel.Debug, Patch.DebugStarGen);
                        }

                        orbitAround = currentIndexOfPlanet;
                        currentIndexOfMoons++;
                    }

                    ++currentIndexOfBodies;


                    if (star.index == 0) {
                        Patch.Debug("#star.planets count : " + star.planets.Length + "#", LogLevel.Debug,
                            Patch.DebugStarGen);
                        Patch.Debug("#currentIndexOfBodies of : " + currentIndexOfBodies + "#", LogLevel.Debug,
                            Patch.DebugStarGen);
                        Patch.Debug("#currentIndexOfPlanet : " + currentIndexOfPlanet + "#", LogLevel.Debug,
                            Patch.DebugStarGen);
                        Patch.Debug("#currentIndexOfMoons : " + currentIndexOfMoons + "#", LogLevel.Debug,
                            Patch.DebugStarGen);
                        Patch.Debug("#orbitAround : " + orbitAround + "#", LogLevel.Debug, Patch.DebugStarGen);
                        if (orbitAround != 0 && pgasRef[index] == 0.6) {
                            Patch.Debug(
                                "#Applied Orbit Index For Moon : " + currentIndexOfMoons + "Around : " + orbitAround +
                                "#", LogLevel.Debug, Patch.DebugStarGen);
                        }
                        else {
                            Patch.Debug("#Applied Orbit Index For Planet : " + currentIndexOfPlanet + "#",
                                LogLevel.Debug, Patch.DebugStarGen);
                        }

                        Patch.Debug("#End of : " + index + "#", LogLevel.Debug, Patch.DebugStarGen);
                    }

                    star.planets[index] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, index, orbitAround,
                        orbitAround != 0 ? currentIndexOfMoons : currentIndexOfPlanet,
                        orbitAround != 0 ? currentIndexOfMoons : currentIndexOfPlanet, gasGiant, info_seed,
                        gen_seed);
                    star.planets[index].name = pnameRef[index];
                }
            }

// old num16
            int orbitIndexOfGasGiant = 0;
// old num17
            int orbitIndexOfAlonePlanet = 0;
//old index 1 
            int AsteroidBeltOrbitIndex = 0;

// search for a gas giant
            foreach (var planetData in star.planets) {
                if (planetData.type == EPlanetType.Gas) {
                    orbitIndexOfGasGiant = planetData.orbitIndex;
                    break;
                }
            }

// search for a single planet
            foreach (var planetData in star.planets) {
                if (planetData.orbitAround == 0) {
                    orbitIndexOfAlonePlanet = planetData.orbitIndex;
                }
            }

// if there is a gas giant
            if (orbitIndexOfGasGiant > 0) {
                //old num8
                int orbitIndexBeforeGasGiant = orbitIndexOfGasGiant - 1;
                bool flag = true;
                foreach (var planetData in star.planets) {
                    if (planetData.orbitAround == 0 && planetData.orbitIndex == orbitIndexOfGasGiant - 1) {
                        flag = false;
                        break;
                    }
                }

                if (flag && randomNumber4 < 0.2 + orbitIndexBeforeGasGiant * 0.2) {
                    AsteroidBeltOrbitIndex = orbitIndexBeforeGasGiant;
                }
            }

//Asteroids belt Generation ?
// old index3
            int asteroidBelt2OrbitIndex = randomNumber5 >= 0.2
                ? (randomNumber5 >= 0.4
                    ? (randomNumber5 >= 0.8 ? 0 : orbitIndexOfAlonePlanet + 1)
                    : orbitIndexOfAlonePlanet + 2)
                : orbitIndexOfAlonePlanet + 3;
            if (asteroidBelt2OrbitIndex != 0 && asteroidBelt2OrbitIndex < 5)
                asteroidBelt2OrbitIndex = 5;
            star.asterBelt1OrbitIndex = AsteroidBeltOrbitIndex;
            star.asterBelt2OrbitIndex = asteroidBelt2OrbitIndex;
            if (AsteroidBeltOrbitIndex > 0) {
                star.asterBelt1Radius = Patch.OrbitRadiusArray[AsteroidBeltOrbitIndex] * (float) randomNumber6 *
                                        star.orbitScaler;
            }

            if (asteroidBelt2OrbitIndex <= 0) {
                return false;
            }

            star.asterBelt2Radius = Patch.OrbitRadiusArray[asteroidBelt2OrbitIndex] * (float) randomNumber7 *
                                    star.orbitScaler;
            if (star.index == 0) {
                Patch.Debug("#End of Function : #", LogLevel.Debug, Patch.DebugStarGen);
            }


            return false;
        }
    }
}