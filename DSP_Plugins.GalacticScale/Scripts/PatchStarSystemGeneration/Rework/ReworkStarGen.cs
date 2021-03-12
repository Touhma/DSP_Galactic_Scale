using System;
using System.Collections.Generic;
using BepInEx.Logging;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    public static class ReworkStarGen {
        public static void CreateStarPlanetsRework(GalaxyData galaxy, StarData star, GameDesc gameDesc, PlanetGeneratorSettings genSettings) {
            bool isDebugOn = star.IsStartingStar();

            star.name = SystemsNames.systems[star.index];

            Patch.Debug("System " + star.name + " - " + star.type + " - " + star.spectr, LogLevel.Debug,
                Patch.DebugStarGen && isDebugOn);

            // Random Generators Inits
            UnityRandom.InitState(star.seed);
            var mainSeed = new Random(star.seed);
            var annexSeed = new Random(mainSeed.Next());

            // InnerCount for the System
            var planetsToGenerate = new List<PlanetForGenerator>();

            // settings from the config
            PatchForStarSystemGeneration.StarSystemSetting currentSettings;

            if (star.type == EStarType.BlackHole || star.type == EStarType.GiantStar ||
                star.type == EStarType.NeutronStar || star.type == EStarType.WhiteDwarf)
                currentSettings = Patch.GeneratorSpecialsSystemConfig[star.type];
            else
                currentSettings = Patch.GeneratorMainSystemConfig[star.spectr];

            //Debugging configs
            Patch.Debug("*************************** : \n" +
                        "ChanceMoonGasGiant : " + currentSettings.ChanceGasGiantMoon + "\n" +
                        "ChanceMoonTelluric : " + currentSettings.ChanceMoonTelluric + "\n" +
                        "ChancePlanetTelluric : " + currentSettings.ChanceTelluricPlanet + "\n" +
                        "ChancePlanetGasGiant : " + currentSettings.ChanceGasGiant + "\n" +
                        "MaxMoonTelluricNb : " + currentSettings.MaxMoonTelluricNb + "\n" +
                        "MaxMoonGasGiantNb : " + currentSettings.MaxMoonGasGiantNb + "\n" +
                        "MaxTelluricNb : " + currentSettings.MaxTelluricNb + "\n" +
                        "MaxGasGiantNb : " + currentSettings.MaxGasGiantNb + "\n" +
                        "ChanceJumpOrbitMoons : " + currentSettings.ChanceJumpOrbitMoons + "\n" +
                        "ChanceJumpOrbitPlanets : " + currentSettings.ChanceJumpOrbitPlanets + "\n" +
                        "*************************** ", LogLevel.Debug, Patch.DebugStarGenDeep);

            Patch.Debug("Definition of Nb of planets In the system :", LogLevel.Debug, Patch.DebugStarGenDeep);

            DefineNumberOfBodies(currentSettings, annexSeed, genSettings);

            if (star.IsStartingStar()) {
                // check if minimum number of planet is met 
                genSettings.nbOfTelluricPlanets = genSettings.nbOfTelluricPlanets < Patch.StartingSystemMinPlanetTelluricNb.Value ? Patch.StartingSystemMinPlanetTelluricNb.Value : genSettings.nbOfTelluricPlanets;
                genSettings.nbOfMoonsTelluric = genSettings.nbOfMoonsTelluric < Patch.StartingSystemMinTelluricMoonNb.Value ? Patch.StartingSystemMinTelluricMoonNb.Value : genSettings.nbOfMoonsTelluric;
                genSettings.nbOfGasGiantPlanets = genSettings.nbOfGasGiantPlanets < Patch.StartingSystemMinGasGiantNb.Value ? Patch.StartingSystemMinGasGiantNb.Value : genSettings.nbOfGasGiantPlanets;
                genSettings.nbOfMoonsGasGiant = genSettings.nbOfMoonsGasGiant < Patch.StartingSystemMinGasGiantMoonNb.Value ? Patch.StartingSystemMinGasGiantMoonNb.Value : genSettings.nbOfMoonsGasGiant;

                genSettings.nbOfMoons = genSettings.nbOfMoonsTelluric + genSettings.nbOfMoonsGasGiant;
                genSettings.nbOfPlanets = genSettings.nbOfTelluricPlanets + genSettings.nbOfGasGiantPlanets;
                genSettings.nbOfStellarBodies = genSettings.nbOfPlanets + genSettings.nbOfMoons;
            }

            star.planets = new PlanetData[genSettings.nbOfStellarBodies];

            Patch.Debug("*************************** :", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
            Patch.Debug("\nSystem Presets : ", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);

            var preset =
                "nbOfPlanets : " + genSettings.nbOfPlanets + "\n" +
                "nbOfTelluricPlanets : " + genSettings.nbOfTelluricPlanets + "\n" +
                "nbOfGasGiantPlanets : " + genSettings.nbOfGasGiantPlanets + "\n" +
                "nbOfMoons : " + genSettings.nbOfMoons + "\n" +
                "nbOfMoonsTelluric : " + genSettings.nbOfMoonsTelluric + "\n" +
                "nbOfMoonsGasGiant : " + genSettings.nbOfMoonsGasGiant + "\n\n";
            Patch.Debug(preset, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);

            PreGenerateAllBodies(star, planetsToGenerate, annexSeed, genSettings, currentSettings);

            GenerateAllPlanets(galaxy, star, gameDesc, planetsToGenerate);


            if (star.IsStartingStar()) {
                Patch.Debug(star.name + " --recap-- : ", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                var nbOfHabitablePlanets = 0;
                foreach (var planet in star.planets) {
                    if (planet.orbitAround != 0)
                        Patch.Debug("moon type : " + planet.type, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    else
                        Patch.Debug("planet type : " + planet.type, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);

                    if (planet.type == EPlanetType.Ocean) nbOfHabitablePlanets++;
                }

                if (nbOfHabitablePlanets == 0) {
                    Patch.Debug("Nb of habitable == 0 --> Override one planet ", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    var @override = true;
                    while (@override) {
                        var indexStartingPlanet = UnityRandom.Range(0, star.planets.Length - 1);

                        if (star.planets[indexStartingPlanet].type != EPlanetType.Gas) {
                            star.planets[indexStartingPlanet].ShouldBeHabitable();
                            galaxy.birthPlanetId = star.planets[indexStartingPlanet].id;
                            @override = false;
                        }
                    }


                    Patch.Debug(" galaxy.birthPlanetId --> " + galaxy.birthPlanetId, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                }
            }

            // Apply themes 
            foreach (var planet in star.planets) {
                PlanetGen.SetPlanetTheme(planet, star, gameDesc, 0, 0, mainSeed.NextDouble(), mainSeed.NextDouble(), mainSeed.NextDouble(), mainSeed.NextDouble(), mainSeed.Next());
                if (star.IsStartingStar()) {
                    Patch.Debug("planet.name --> " + planet.name, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    Patch.Debug("planet.algoId --> " + planet.algoId, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    Patch.Debug("planet.type --> " + planet.type, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                }
              
        
            }

            star.planetCount = star.planets.Length;
        }

        public static void DefineNumberOfBodies(PatchForStarSystemGeneration.StarSystemSetting currentSettings, Random annexSeed, PlanetGeneratorSettings genSettings) {
            // Define how much planets the system have
            for (var i = 0; i < currentSettings.MaxTelluricNb; i++)
                if (annexSeed.NextDouble() <= currentSettings.ChanceTelluricPlanet) {
                    genSettings.nbOfTelluricPlanets++;
                    genSettings.nbOfPlanets++;
                    genSettings.nbOfStellarBodies++;
                }

            // Define how much of the planets are GasGiant
            for (var j = 0; j < currentSettings.MaxGasGiantNb; j++)
                if (annexSeed.NextDouble() <= currentSettings.ChanceGasGiant) {
                    genSettings.nbOfGasGiantPlanets++;
                    genSettings.nbOfPlanets++;
                    genSettings.nbOfStellarBodies++;
                }


            Patch.Debug("*************************** : \n" +
                        "nbOfPlanets : " + genSettings.nbOfPlanets + "\n" +
                        "nbOfTelluricPlanets : " + genSettings.nbOfTelluricPlanets + "\n" +
                        "nbOfGasGiantPlanets : " + genSettings.nbOfGasGiantPlanets + "\n" +
                        "*************************** ", LogLevel.Debug, Patch.DebugStarGenDeep);

            Patch.Debug("Definition of Nb of Moons In the system :", LogLevel.Debug, Patch.DebugStarGenDeep);
            // Define how much moons the system have
            // Define how much moons the telluric  planets have 
            if (genSettings.nbOfTelluricPlanets != 0) {
                for (var i = 0; i < currentSettings.MaxMoonTelluricNb; i++)
                    if (annexSeed.NextDouble() <= currentSettings.ChanceMoonTelluric) {
                        genSettings.nbOfMoonsTelluric++;
                        genSettings.nbOfMoons++;
                        genSettings.nbOfStellarBodies++;
                    }
            }
            else {
                Patch.Debug("No Telluric in the system :", LogLevel.Debug, Patch.DebugStarGenDeep);
            }

            // Define how much moons the gasGiants planets have 
            if (genSettings.nbOfGasGiantPlanets != 0) {
                for (var i = genSettings.nbOfMoons; i < currentSettings.MaxMoonGasGiantNb; i++)
                    if (annexSeed.NextDouble() <= currentSettings.ChanceGasGiantMoon) {
                        genSettings.nbOfMoonsGasGiant++;
                        genSettings.nbOfMoons++;
                        genSettings.nbOfStellarBodies++;
                    }
            }
            else {
                Patch.Debug("No Gas Giant in the system :", LogLevel.Debug, Patch.DebugStarGenDeep);
            }

            Patch.Debug("*************************** : \n" +
                        "nbOfMoons : " + genSettings.nbOfMoons + "\n" +
                        "nbOfMoonsTelluric : " + genSettings.nbOfMoonsTelluric + "\n" +
                        "nbOfMoonsGasGiant : " + genSettings.nbOfMoonsGasGiant + "\n" +
                        "*************************** ", LogLevel.Debug, Patch.DebugStarGenDeep);

            Patch.Debug("*************************** :", LogLevel.Debug, Patch.DebugStarGenDeep);
        }

        public static void PreGenerateAllBodies(StarData star, List<PlanetForGenerator> planetsToGenerate, Random annexSeed, PlanetGeneratorSettings genSettings, PatchForStarSystemGeneration.StarSystemSetting currentSettings) {
            bool isDebugOn = star.IsStartingStar();
            //
            //preparation of the planet creation :
            Patch.Debug("Define the belts for whatever they are for :", LogLevel.Debug, Patch.DebugStarGenDeep);
            //Define where the 2 asteroids belts are ( maybe not implemented ) 
            var asterBelt1OrbitIndex = UnityRandom.Range(1, genSettings.nbOfPlanets - 1);
            var asterBelt2OrbitIndex = UnityRandom.Range(asterBelt1OrbitIndex + 1, genSettings.nbOfPlanets);

            Patch.Debug("asterBelt1OrbitIndex :" + asterBelt1OrbitIndex, LogLevel.Debug, Patch.DebugStarGenDeep);
            Patch.Debug("asterBelt2OrbitIndex :" + asterBelt2OrbitIndex, LogLevel.Debug, Patch.DebugStarGenDeep);


            //Attach the information to the star
            star.asterBelt1OrbitIndex = asterBelt1OrbitIndex;
            star.asterBelt2OrbitIndex = asterBelt2OrbitIndex;
            star.asterBelt1Radius = Patch.OrbitRadiusPlanetArray[asterBelt1OrbitIndex];
            star.asterBelt2Radius = Patch.OrbitRadiusPlanetArray[asterBelt2OrbitIndex];


            int infoSeed;
            int genSeed;

            // planets pre-generation
            var nbOfBodiesPreGenerated = 0;
            var nbOfPlanetsPreGenerated = 0;
            var planetsPreGeneratedNumber = 1;
            var nbOfMoonsPreGenerated = 0;

            var currentOrbitPlanetIndex = 1;
            var previousOrbitPlanetIndex = 0;
            int currentOrbitMoonIndex;

            var beltGenerated = 0;

            int jumpOrbitMargin;

            for (var i = 0; i < genSettings.nbOfStellarBodies; i++) {

                infoSeed = annexSeed.Next();
                genSeed = annexSeed.Next();

                var planetInfoSeed = 0 + infoSeed;
                var planetGenSeed = 0 + genSeed;
                Patch.Debug("bodies generated !" + nbOfBodiesPreGenerated, LogLevel.Debug, Patch.DebugStarGenDeep);
                Patch.Debug("genSettings.nbOfPlanets + genSettings.nbOfMoons !" + (genSettings.nbOfPlanets + genSettings.nbOfMoons), LogLevel.Debug, Patch.DebugStarGenDeep);
                bool isGasGiant;
                var orbitAround = 0;

                if (asterBelt1OrbitIndex == currentOrbitPlanetIndex) {
                    Patch.Debug("Jump Belt 1 Orbit :", LogLevel.Debug, Patch.DebugStarGenDeep);
                    currentOrbitPlanetIndex++;
                    nbOfBodiesPreGenerated++;
                    beltGenerated++;
                }

                if (asterBelt2OrbitIndex == currentOrbitPlanetIndex) {
                    Patch.Debug("Jump Belt 2 Orbit :", LogLevel.Debug, Patch.DebugStarGenDeep);
                    currentOrbitPlanetIndex++;
                    nbOfBodiesPreGenerated++;
                    beltGenerated++;
                }

                Patch.Debug("nbOfPlanetsPreGenerated : " + nbOfPlanetsPreGenerated, LogLevel.Debug, Patch.DebugStarGenDeep);
                Patch.Debug("nbOfPlanets : " + genSettings.nbOfPlanets, LogLevel.Debug, Patch.DebugStarGenDeep);
                if (nbOfPlanetsPreGenerated < genSettings.nbOfPlanets) {
                    //planets
                    // jumporbit planet

                    jumpOrbitMargin = Patch.OrbitRadiusArrayPlanetNb.Value - (genSettings.nbOfPlanets - nbOfPlanetsPreGenerated);

                    if (currentOrbitPlanetIndex < jumpOrbitMargin && jumpOrbitMargin < currentSettings.JumpOrbitPlanetMax)
                        if (annexSeed.NextDouble() < currentSettings.ChanceJumpOrbitPlanets)// can jump orbit up to JumpOrbitPlanetIndex
                            currentOrbitPlanetIndex = UnityRandom.Range(currentOrbitPlanetIndex, currentOrbitPlanetIndex + currentSettings.JumpOrbitPlanetMax);

                    previousOrbitPlanetIndex = currentOrbitPlanetIndex;


                    orbitAround = 0;

                    if (nbOfBodiesPreGenerated < genSettings.nbOfTelluricPlanets + beltGenerated)//telluric
                        isGasGiant = false;
                    else//gasgiant
                        isGasGiant = true;


                    planetsToGenerate.Add(new PlanetForGenerator(nbOfBodiesPreGenerated - beltGenerated, orbitAround, currentOrbitPlanetIndex, planetsPreGeneratedNumber, isGasGiant, planetInfoSeed, planetGenSeed, null));
                    Patch.Debug("planetsToGenerate -->   \n" + planetsToGenerate[nbOfPlanetsPreGenerated].ToStringDebug(), LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    nbOfPlanetsPreGenerated++;
                    planetsPreGeneratedNumber++;
                    currentOrbitPlanetIndex++;
                    if (isGasGiant)
                        Patch.Debug("gas Giant generated !", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    else
                        Patch.Debug("planet generated !", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                }
                else if (nbOfBodiesPreGenerated < genSettings.nbOfPlanets + genSettings.nbOfMoons + beltGenerated) {
                    Patch.Debug("Moon in generation!", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);

                    isGasGiant = false;

                    if (genSettings.nbOfTelluricPlanets != 0 && nbOfBodiesPreGenerated < genSettings.nbOfPlanets + genSettings.nbOfMoonsTelluric + beltGenerated) {
                        // telluric moon
                        orbitAround = UnityRandom.Range(1, genSettings.nbOfTelluricPlanets);
                        Patch.Debug("telluric moon! orbit around : " + orbitAround, LogLevel.Debug, Patch.DebugStarGenDeep);
                    }
                    else {
                        if (genSettings.nbOfGasGiantPlanets != 0) {
                            //gasgiant moon 
                            orbitAround = UnityRandom.Range(genSettings.nbOfTelluricPlanets + 1, genSettings.nbOfTelluricPlanets + genSettings.nbOfGasGiantPlanets);
                            Patch.Debug("gas moon! orbit around : " + orbitAround, LogLevel.Debug, Patch.DebugStarGenDeep);
                        }
                    }

                    if (orbitAround <= 0) Patch.Debug("Issue in moon generation : " + orbitAround, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);

                    jumpOrbitMargin = Patch.OrbitRadiusArrayMoonsNb.Value - (genSettings.nbOfMoons - nbOfMoonsPreGenerated);

                    Patch.Debug("orbitAround - 1 : " + (orbitAround - 1), LogLevel.Debug, Patch.DebugStarGenDeep);
                    Patch.Debug("planetsToGenerate.Count :" + planetsToGenerate.Count, LogLevel.Debug, Patch.DebugStarGenDeep);
                    Patch.Debug("planetsToGenerate[orbitAround - 1] :" + planetsToGenerate[orbitAround - 1].orbitIndex, LogLevel.Debug, Patch.DebugStarGenDeep);


                    int currentPlanetMoonsNb;
                    var currentPlanet = planetsToGenerate[orbitAround - 1];
                    Patch.Debug("planetsToGenerate nb  :" + planetsToGenerate.Count, LogLevel.Debug, Patch.DebugStarGenDeep);
                    currentPlanetMoonsNb = currentPlanet.moons.Count;


                    Patch.Debug("currentPlanetMoonsNb :" + currentPlanetMoonsNb, LogLevel.Debug, Patch.DebugStarGenDeep);
                    if (currentPlanetMoonsNb != 0)
                        currentOrbitMoonIndex = currentPlanet.moons[currentPlanetMoonsNb - 1].orbitIndex + 1;
                    else
                        currentOrbitMoonIndex = 0;

                    Patch.Debug("currentOrbitMoonIndex : " + currentOrbitMoonIndex, LogLevel.Debug, Patch.DebugStarGenDeep);

                    if (currentOrbitMoonIndex < jumpOrbitMargin && jumpOrbitMargin < currentSettings.JumpOrbitMoonMax)
                        if (annexSeed.NextDouble() < currentSettings.ChanceJumpOrbitMoons) {
                            // can jump orbit up to JumpOrbitPlanetIndex
                            var oldOrbitIndex = currentOrbitMoonIndex;
                            currentOrbitMoonIndex += UnityRandom.Range(currentOrbitMoonIndex, currentOrbitMoonIndex + currentSettings.JumpOrbitMoonMax);
                        }

                    currentPlanet.AddMoonInOrbit(nbOfBodiesPreGenerated, currentOrbitMoonIndex, planetGenSeed, planetInfoSeed);


                    nbOfMoonsPreGenerated++;
                    Patch.Debug("moonToGenerate --> +" + genSettings.nbOfMoons + " --> nbOfMoonsPreGenerated : " + nbOfMoonsPreGenerated, LogLevel.Debug, Patch.DebugStarGenDeep);
                }

                nbOfBodiesPreGenerated++;
            }
        }

        public static void GenerateAllPlanets(GalaxyData galaxy, StarData star, GameDesc gameDesc, List<PlanetForGenerator> planetsToGenerate) {
            bool isDebugOn = star.IsStartingStar();
            Patch.Debug("Recap of what have to be generated : \n", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
            var finalIndex = 0;
            foreach (var planet in planetsToGenerate) {
                var debugLine = "A ";

                planet.planetIndex = finalIndex;
                foreach (var planetForGenerator in planet.moons) planetForGenerator.orbitAround = finalIndex + 1;

                if (planet.isGasGiant)
                    debugLine += " Gas Giant :" + planet.planetIndex + "with values : \n";
                else
                    debugLine += " Telluric Planet :" + planet.planetIndex + "with values : \n";

                //planet.ToString();

                //planet.GenerateThePlanet(ref galaxy,ref star,ref gameDesc);
                //  Debug.Log();
                PlanetGen.CreatePlanet(galaxy, star, gameDesc, planet.planetIndex, planet.orbitAround, planet.orbitIndex, planet.number, planet.isGasGiant, planet.infoSeed, planet.genSeed);
                star.planets[finalIndex].name = star.name + " - " + RomanNumbers.roman[planet.number];
                planet.name = star.planets[finalIndex].name;

                if (planet.moons.Count >= 2) star.planets[finalIndex].HasMultipleSatellites();

                Patch.Debug(star.planets[finalIndex].name, LogLevel.Debug, Patch.DebugStarNamingGen);
                finalIndex++;
                //debugLine += planet.ToString() + "\n\n";
                if (planet.moons.Count != 0) {
                    debugLine += "with " + planet.moons.Count + " Moons  : \n\n";
                    foreach (var moon in planet.moons) {
                        moon.planetIndex = finalIndex;
                        debugLine += " Moon : " + moon.planetIndex + "\n";
                        PlanetGen.CreatePlanet(galaxy, star, gameDesc, moon.planetIndex, moon.orbitAround, moon.orbitIndex, moon.number, moon.isGasGiant, moon.infoSeed, moon.genSeed);
                        star.planets[moon.planetIndex].name = planet.name + " - " + RomanNumbers.roman[moon.number];
                        Patch.Debug(star.planets[moon.planetIndex].name, LogLevel.Debug, Patch.DebugStarNamingGen);

                        finalIndex++;
                    }
                }



                Patch.Debug(debugLine, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
            }
        }
    }
}