using System;
using System.Collections.Generic;
using BepInEx.Logging;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration
{
    public static class ReworkStarGen
    {
        public static int orb1 = -1;
        public static int orb2;
        public static int orb3;
        public static bool[] orbs;
        public static void CreateStarPlanetsRework(GalaxyData galaxy, StarData star, GameDesc gameDesc, PlanetGeneratorSettings genSettings)
        {
            bool isDebugOn = true;// star.IsStartingStar();

            star.name = SystemsNames.systems[star.index];
            Patch.Debug("System " + star.name + " - " + star.type + " - " + star.spectr, LogLevel.Warning,
                true);
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
                        "MinMoonTelluricNb : " + currentSettings.MinMoonTelluricNb + "\n" +
                        "MinMoonGasGiantNb : " + currentSettings.MinMoonGasGiantNb + "\n" +
                        "MinTelluricNb : " + currentSettings.MinTelluricNb + "\n" +
                        "MinGasGiantNb : " + currentSettings.MinGasGiantNb + "\n" +
                        "MaxMoonTelluricNb : " + currentSettings.MaxMoonTelluricNb + "\n" +
                        "MaxMoonGasGiantNb : " + currentSettings.MaxMoonGasGiantNb + "\n" +
                        "MaxTelluricNb : " + currentSettings.MaxTelluricNb + "\n" +
                        "MaxGasGiantNb : " + currentSettings.MaxGasGiantNb + "\n" +
                        "ChanceJumpOrbitMoons : " + currentSettings.ChanceJumpOrbitMoons + "\n" +
                        "ChanceJumpOrbitPlanets : " + currentSettings.ChanceJumpOrbitPlanets + "\n" +
                        "*************************** ", LogLevel.Debug, Patch.DebugStarGenDeep);

            Patch.Debug("Definition of Nb of planets In the system :", LogLevel.Debug, Patch.DebugStarGenDeep);

            DefineNumberOfBodies(currentSettings, annexSeed, genSettings);

            if (star.IsStartingStar())
            {
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


            if (star.IsStartingStar())
            {
                Patch.Debug(star.name + " --recap-- : ", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                var nbOfHabitablePlanets = 0;
                foreach (var planet in star.planets)
                {
                    if (planet.orbitAround != 0)
                        Patch.Debug("moon type : " + planet.type, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    else
                        Patch.Debug("planet type : " + planet.type, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);

                    if (planet.type == EPlanetType.Ocean) nbOfHabitablePlanets++;
                }

                if (nbOfHabitablePlanets == 0)
                {
                    Patch.Debug("Nb of habitable == 0 --> Override one planet ", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    var @override = true;
                    while (@override)
                    {
                        var indexStartingPlanet = UnityRandom.Range(0, star.planets.Length - 1);

                        if (star.planets[indexStartingPlanet].type != EPlanetType.Gas)
                        {
                            star.planets[indexStartingPlanet].ShouldBeHabitable();
                            galaxy.birthPlanetId = star.planets[indexStartingPlanet].id;
                            @override = false;
                        }
                    }
                }

                Patch.Debug(" galaxy.birthPlanetId --> " + galaxy.birthPlanetId, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);

                foreach (var planet in star.planets)
                {
                    if (planet.type == EPlanetType.Ocean) // For some reason the generator is only spitting out ONE habitable planet even if all the planets should be habitable
                    {
                        Patch.Debug("Radius is " + planet.radius, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                        if (planet.radius < 50) planet.radius = 50; // Don't want starting planet smaller than this or its unplayable. Maybe add a config option
                    }
                }

            }

            // Apply themes 
            foreach (var planet in star.planets)
            {
                PlanetGen.SetPlanetTheme(planet, star, gameDesc, 0, 0, mainSeed.NextDouble(), mainSeed.NextDouble(), mainSeed.NextDouble(), mainSeed.NextDouble(), mainSeed.Next());
                if (star.IsStartingStar())
                {
                    Patch.Debug("planet.name --> " + planet.name, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    Patch.Debug("planet.algoId --> " + planet.algoId, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    Patch.Debug("planet.type --> " + planet.type, LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                }


            }

            star.planetCount = star.planets.Length;
        }

        public static void DefineNumberOfBodies(PatchForStarSystemGeneration.StarSystemSetting currentSettings, Random annexSeed, PlanetGeneratorSettings genSettings)
        {
            // Define how much planets the system have
            for (var i = 0; i < currentSettings.MaxTelluricNb; i++)
            {
                if (annexSeed.NextDouble() <= currentSettings.ChanceTelluricPlanet)
                {
                    genSettings.nbOfTelluricPlanets++;
                    genSettings.nbOfPlanets++;
                    genSettings.nbOfStellarBodies++;
                }
                Patch.Debug("Initially generated " + genSettings.nbOfTelluricPlanets, LogLevel.Debug, Patch.DebugStarGen);

            }
            if (genSettings.nbOfTelluricPlanets < currentSettings.MinTelluricNb)
            {
                Patch.Debug("Minimum Telluric Planets is too few", LogLevel.Debug, Patch.DebugStarGen);
                Patch.Debug("Minimum Telluric Planets is " + currentSettings.MinTelluricNb + " while generated number was " + genSettings.nbOfTelluricPlanets, LogLevel.Debug, Patch.DebugStarGen);
                int difference = currentSettings.MinTelluricNb - genSettings.nbOfTelluricPlanets;
                genSettings.nbOfTelluricPlanets += difference;
                genSettings.nbOfPlanets += difference;
                genSettings.nbOfStellarBodies += difference;
            }
            // Define how much of the planets are GasGiant
            for (var j = 0; j < currentSettings.MaxGasGiantNb; j++)
            {
                if (annexSeed.NextDouble() <= currentSettings.ChanceGasGiant)
                {
                    genSettings.nbOfGasGiantPlanets++;
                    genSettings.nbOfPlanets++;
                    genSettings.nbOfStellarBodies++;
                }

            }
            if (genSettings.nbOfGasGiantPlanets < currentSettings.MinGasGiantNb)
            {
                int difference = currentSettings.MinGasGiantNb - genSettings.nbOfGasGiantPlanets;
                genSettings.nbOfGasGiantPlanets += difference;
                genSettings.nbOfPlanets += difference;
                genSettings.nbOfStellarBodies += difference;
            }

            Patch.Debug("*************************** : \n" +
                        "nbOfPlanets : " + genSettings.nbOfPlanets + "\n" +
                        "nbOfTelluricPlanets : " + genSettings.nbOfTelluricPlanets + "\n" +
                        "nbOfGasGiantPlanets : " + genSettings.nbOfGasGiantPlanets + "\n" +
                        "*************************** ", LogLevel.Debug, Patch.DebugStarGenDeep);

            Patch.Debug("Definition of Nb of Moons In the system :", LogLevel.Debug, Patch.DebugStarGenDeep);
            // Define how much moons the system have
            // Define how much moons the telluric  planets have 
            if (genSettings.nbOfTelluricPlanets != 0)
            {
                for (var i = 0; i < currentSettings.MaxMoonTelluricNb; i++)
                {
                    if (annexSeed.NextDouble() <= currentSettings.ChanceMoonTelluric)
                    {
                        genSettings.nbOfMoonsTelluric++;
                        genSettings.nbOfMoons++;
                        genSettings.nbOfStellarBodies++;
                    }

                }
                if (genSettings.nbOfMoonsTelluric < currentSettings.MinMoonTelluricNb)
                {
                    int difference = currentSettings.MinMoonTelluricNb - genSettings.nbOfMoonsTelluric;
                    genSettings.nbOfMoonsTelluric += difference;
                    genSettings.nbOfMoons += difference;
                    genSettings.nbOfStellarBodies += difference;
                }
            }
            else
            {
                Patch.Debug("No Telluric in the system :", LogLevel.Debug, Patch.DebugStarGenDeep);
            }

            // Define how much moons the gasGiants planets have 
            if (genSettings.nbOfGasGiantPlanets != 0)
            {
                for (var i = genSettings.nbOfMoons; i < currentSettings.MaxMoonGasGiantNb; i++)
                {
                    if (annexSeed.NextDouble() <= currentSettings.ChanceGasGiantMoon)
                    {
                        genSettings.nbOfMoonsGasGiant++;
                        genSettings.nbOfMoons++;
                        genSettings.nbOfStellarBodies++;
                    }

                }
                if (genSettings.nbOfMoonsGasGiant < currentSettings.MinMoonGasGiantNb)
                {
                    int difference = currentSettings.MinMoonGasGiantNb - genSettings.nbOfMoonsGasGiant;
                    genSettings.nbOfMoonsGasGiant += difference;
                    genSettings.nbOfMoons += difference;
                    genSettings.nbOfStellarBodies += difference;
                }
            }
            else
            {
                Patch.Debug("No Gas Giant in the system :", LogLevel.Debug, Patch.DebugStarGenDeep);
            }

            Patch.Debug("*************************** : \n" +
                        "nbOfMoons : " + genSettings.nbOfMoons + "\n" +
                        "nbOfMoonsTelluric : " + genSettings.nbOfMoonsTelluric + "\n" +
                        "nbOfMoonsGasGiant : " + genSettings.nbOfMoonsGasGiant + "\n" +
                        "*************************** ", LogLevel.Debug, Patch.DebugStarGenDeep);

            Patch.Debug("*************************** :", LogLevel.Debug, Patch.DebugStarGenDeep);
        }

        public static void PreGenerateAllBodies(StarData star, List<PlanetForGenerator> planetsToGenerate, Random annexSeed, PlanetGeneratorSettings genSettings, PatchForStarSystemGeneration.StarSystemSetting currentSettings)
        {
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
            var nbOfTelluricPlanetsPreGenerated = 0;
            var nbOfGasGiantPlanetsPreGenerated = 0;
            var planetsPreGeneratedNumber = 1;
            var nbOfMoonsPreGenerated = 0;

            var currentOrbitPlanetIndex = 1;
            var previousOrbitPlanetIndex = 0;
            int currentOrbitMoonIndex;

            var beltGenerated = 0;

            int jumpOrbitMargin;


            orb1 = -1;
            orb2 = -1;
            orb3 = -1;

            if (Patch.UseNewGasGiantOrbitPicker.Value) DistributePlanets(annexSeed, genSettings);
            for (var i = 0; i < genSettings.nbOfStellarBodies; i++)
            {

                infoSeed = annexSeed.Next();
                genSeed = annexSeed.Next();

                var planetInfoSeed = 0 + infoSeed;
                var planetGenSeed = 0 + genSeed;
                Patch.Debug("bodies generated !" + nbOfBodiesPreGenerated, LogLevel.Debug, Patch.DebugStarGenDeep);
                Patch.Debug("genSettings.nbOfPlanets + genSettings.nbOfMoons !" + (genSettings.nbOfPlanets + genSettings.nbOfMoons), LogLevel.Debug, Patch.DebugStarGenDeep);
                bool isGasGiant;
                var orbitAround = 0;

                if (asterBelt1OrbitIndex == currentOrbitPlanetIndex)
                {
                    Patch.Debug("Jump Belt 1 Orbit :", LogLevel.Debug, Patch.DebugStarGenDeep);
                    currentOrbitPlanetIndex++;
                    nbOfBodiesPreGenerated++;
                    beltGenerated++;
                }

                if (asterBelt2OrbitIndex == currentOrbitPlanetIndex)
                {
                    Patch.Debug("Jump Belt 2 Orbit :", LogLevel.Debug, Patch.DebugStarGenDeep);
                    currentOrbitPlanetIndex++;
                    nbOfBodiesPreGenerated++;
                    beltGenerated++;
                }

                Patch.Debug("nbOfPlanetsPreGenerated : " + nbOfPlanetsPreGenerated, LogLevel.Debug, Patch.DebugStarGenDeep);
                Patch.Debug("nbOfPlanets : " + genSettings.nbOfPlanets, LogLevel.Debug, Patch.DebugStarGenDeep);
                if (nbOfPlanetsPreGenerated < genSettings.nbOfPlanets)
                {
                    //planets
                    // jumporbit planet

                    jumpOrbitMargin = Patch.OrbitRadiusArrayPlanetNb.Value - (genSettings.nbOfPlanets - nbOfPlanetsPreGenerated);

                    if (currentOrbitPlanetIndex < jumpOrbitMargin && jumpOrbitMargin < currentSettings.JumpOrbitPlanetMax)
                        if (annexSeed.NextDouble() < currentSettings.ChanceJumpOrbitPlanets)// can jump orbit up to JumpOrbitPlanetIndex
                            currentOrbitPlanetIndex = UnityRandom.Range(currentOrbitPlanetIndex, currentOrbitPlanetIndex + currentSettings.JumpOrbitPlanetMax);

                    previousOrbitPlanetIndex = currentOrbitPlanetIndex;


                    orbitAround = 0;
                    if (Patch.UseNewGasGiantOrbitPicker.Value)
                    {
                        isGasGiant = orbs[i];
                    }
                    else
                    {
                        if (nbOfBodiesPreGenerated < genSettings.nbOfTelluricPlanets + beltGenerated)//telluric
                            isGasGiant = false;
                        else//gasgiant
                            isGasGiant = true;
                    }

                    planetsToGenerate.Add(new PlanetForGenerator(nbOfBodiesPreGenerated - beltGenerated, orbitAround, currentOrbitPlanetIndex, planetsPreGeneratedNumber, isGasGiant, planetInfoSeed, planetGenSeed, null));
                    Patch.Debug("planetsToGenerate -->   \n" + planetsToGenerate[nbOfPlanetsPreGenerated].ToStringDebug(), LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    nbOfPlanetsPreGenerated++;
                    planetsPreGeneratedNumber++;
                    currentOrbitPlanetIndex++;
                    if (isGasGiant)
                    {
                        nbOfGasGiantPlanetsPreGenerated++;
                        Patch.Debug("gas Giant generated !", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    }
                    else
                    {
                        nbOfTelluricPlanetsPreGenerated++;
                        Patch.Debug("planet generated !", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
                    }

                }
                else if (nbOfBodiesPreGenerated < genSettings.nbOfPlanets + genSettings.nbOfMoons + beltGenerated)
                {
                    Patch.Debug("Moon in generation!", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);

                    isGasGiant = false;

                    if (genSettings.nbOfTelluricPlanets != 0 && nbOfBodiesPreGenerated < genSettings.nbOfPlanets + genSettings.nbOfMoonsTelluric + beltGenerated)
                    {
                        // telluric moon
                        orbitAround = UnityRandom.Range(1, genSettings.nbOfTelluricPlanets);
                        Patch.Debug("telluric moon! orbit around : " + orbitAround, LogLevel.Debug, Patch.DebugStarGenDeep);
                    }
                    else
                    {
                        if (genSettings.nbOfGasGiantPlanets != 0)
                        {
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
                        if (annexSeed.NextDouble() < currentSettings.ChanceJumpOrbitMoons)
                        {
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

        public static void GenerateAllPlanets(GalaxyData galaxy, StarData star, GameDesc gameDesc, List<PlanetForGenerator> planetsToGenerate)
        {
            bool isDebugOn = star.IsStartingStar();
            Patch.Debug("Recap of what have to be generated : \n", LogLevel.Debug, Patch.DebugStarGen && isDebugOn);
            var finalIndex = 0;
            foreach (var planet in planetsToGenerate)
            {
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
                if (planet.moons.Count != 0)
                {
                    debugLine += "with " + planet.moons.Count + " Moons  : \n\n";
                    foreach (var moon in planet.moons)
                    {
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
        static int pickOrbit(float r, ref Random annexSeed, ref int orb1, ref int orb2, ref int orb3, ref float tBias, ref int total, ref float bias1, ref float bias2, ref float bias3, ref float bias1a, ref float bias2a, ref float bias3a)
        {
            Patch.Debug("PickOrbit", LogLevel.Message, true);
            int third = total / 3;
            if (third == 0)
            {
                Patch.Debug("Third would have been " + third, LogLevel.Message, true);
                third = 1;
            }
            Patch.Debug("Third = "+third, LogLevel.Message, true);
            tBias = (orb1 < (third) ? bias1 : 0) + (orb2 < (third) ? bias2 : 0) + (orb3 < (third) ? bias3 : 0);
            if (tBias == 0) return 1;
            Patch.Debug("tBias = "+tBias, LogLevel.Message, true);
            bias1a = (orb1 < (third) ? bias1 : 0) / tBias;
            Patch.Debug("bias1a = "+bias1a, LogLevel.Message, true);
            bias2a = (orb2 < (total - 2 * third) ? bias2 : 0) / tBias;
            Patch.Debug("bias1a = " + bias1a, LogLevel.Message, true);
            bias3a = (orb3 < (third) ? bias3 : 0) / tBias;
            Patch.Debug("bias1a = " + bias1a, LogLevel.Message, true);
            if (orb1 < (third) && r < bias1a) return 1;
            if (orb2 < (total - 2 * third) && r < (bias1a + bias2a)) return 2;
            if (orb3 < (third) && r < (bias1a + bias2a + bias3a)) return 3;
            Patch.Debug("starting while loop. orb1= "+orb1+" orb2="+orb2+"orb3="+orb3, LogLevel.Message, true);

            while (orb1 + orb2 + orb3 < total)
            {
                Patch.Debug(orb1 + " + " + orb2 + " + " + orb3 + " < " + total, LogLevel.Message, true);
                float r2 = (float)annexSeed.NextDouble();
                Patch.Debug("r2 = "+r2+" checking against 0.33, and 0.66", LogLevel.Message, true);
                Patch.Debug((r2 <= 0.33) + " && orb1 <= third = " + orb1 + " <= " + third + " = " + (orb1 < third), LogLevel.Message, true);
                if (r2 <= 0.33 && orb1 < (third)) return 1;
                Patch.Debug((r2 <= 0.66) + " && orb2 < third = " + orb2 + " <= " + third + " = " + (orb2 < third), LogLevel.Message, true);
                if (r2 <= 0.66 && orb2 < (third)) return 2;
                Patch.Debug((r2 <= 1) + " && orb3 < third = " + orb3 + " <= " + third + " = " + (orb3 < third), LogLevel.Message, true);
                if (orb3 <= third) return 3;
                Patch.Debug("that didnt work, trying again...", LogLevel.Message, true);
            }
            return 4; //this can never be called, why is it here?!
        }
        static void DistributePlanets(Random annexSeed, PlanetGeneratorSettings genSettings)
        {
            Patch.Debug("Distributing Planets-------------------", LogLevel.Message, true);
            int nRocky = genSettings.nbOfTelluricPlanets;
            int nGas = genSettings.nbOfGasGiantPlanets;
            var total = nRocky + nGas;
            int third = total / 3;
            if (third == 0) third = 1;
            float bias1 = Patch.DistributionBiasInner.Value; //1 = always put rocky planets here. 0 = never
            float bias2 = Patch.DistributionBiasMiddle.Value; 
            float bias3 = Patch.DistributionBiasOuter.Value; 

            float tBias = bias1 + bias2 + bias3;
            var bias1a = bias1 / tBias;
            var bias2a = bias2 / tBias;
            var bias3a = bias3 / tBias;

            var rRocky = nRocky;

            Patch.Debug("Resetting orb array", LogLevel.Message, true);
            orb1 = orb2 = orb3 = 0;
            orbs = new bool[genSettings.nbOfPlanets];
            Patch.Debug("Generating " + genSettings.nbOfPlanets + " planets using new Generator", LogLevel.Message, true);
            while (rRocky > 0)
            {
                Patch.Debug("remaining Rocky = " + rRocky, LogLevel.Message, true);
                float rr = (float)annexSeed.NextDouble();
                var orbitArray = pickOrbit(rr, ref annexSeed, ref orb1, ref orb2, ref orb3, ref tBias, ref total, ref bias1, ref bias2, ref bias3, ref bias1a, ref bias2a, ref bias3a);
                Patch.Debug("OrbitArray is " + orbitArray, LogLevel.Message, true);
                if (orbitArray == 1) orb1++;
                if (orbitArray == 2) orb2++;
                if (orbitArray == 3) orb3++;
                if (orbitArray == 4) Patch.Debug("PickOrbit Failed", LogLevel.Warning, true);
                rRocky--;
            }

            Patch.Debug("Finished that part", LogLevel.Message, true);
            Patch.Debug(orb1 + " " + orb2 + " " + orb3, LogLevel.Message, true);
            int rTelluric = orb1;
            int rGas = third - orb1;
            int i = 0;
            while (rTelluric > 0 && rGas > 0)
            {
                Patch.Debug("They are both greater than zero. " + rTelluric + " " + rGas, LogLevel.Message, true);
                float rand = (float)annexSeed.NextDouble();
                if (rand < (rTelluric / (rTelluric + rGas)))
                {
                    orbs[i] = false;
                    rTelluric--;
                }
                else
                {
                    orbs[i] = true;
                    rGas--;
                }
            }
            Patch.Debug("finished both rTelluric= "+rTelluric + " " + orbs.Length, LogLevel.Message, true);
            while (rTelluric > 0)
            {
                orbs[i] = false;
                rTelluric--;
            }
            Patch.Debug("finished rTelluric", LogLevel.Message, true);
            while (rGas > 0)
            {
                orbs[i] = true;
                rGas--;
            }
            Patch.Debug("finished orb1", LogLevel.Message, true);
            rTelluric = orb2;
            rGas = (total - 2 * third) - orb2;

            while (rTelluric > 0 && rGas > 0)
            {
                float rand = (float)annexSeed.NextDouble();
                if (rand < (rTelluric / (rTelluric + rGas)))
                {
                    orbs[i] = false;
                    rTelluric--;
                }
                else
                {
                    orbs[i] = true;
                    rGas--;
                }
            }
            while (rTelluric > 0)
            {
                orbs[i] = false;
                rTelluric--;
            }
            while (rGas > 0)
            {
                orbs[i] = true;
                rGas--;
            }
            Patch.Debug("finished orb2", LogLevel.Message, true);
            rTelluric = orb3;
            rGas = third - orb3;

            while (rTelluric > 0 && rGas > 0)
            {
                float rand = (float)annexSeed.NextDouble();
                if (rand < (rTelluric / (rTelluric + rGas)))
                {
                    orbs[i] = false;
                    rTelluric--;
                }
                else
                {
                    orbs[i] = true;
                    rGas--;
                }
            }
            while (rTelluric > 0)
            {
                orbs[i] = false;
                rTelluric--;
            }
            while (rGas > 0)
            {
                orbs[i] = true;
                rGas--;
            }
            string output = "--Orbit 1--\n";
            for (var i2 = 0; i2 < orbs.Length; i2++)
            {
                if (i2 == third) output += "--Orbit 2--\n";
                if (i2 == 2 * third) output += "--Orbit 3--\n";
                output += ((orbs[i2]) ? "R" : "G");
            }
            Patch.Debug(output, LogLevel.Message, true);
            Patch.Debug("-*-*-*-*finished distributing planets. returning", LogLevel.Message, true);
        }
    }
}