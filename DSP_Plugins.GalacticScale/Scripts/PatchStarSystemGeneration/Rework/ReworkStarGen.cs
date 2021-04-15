using System;
using System.Collections.Generic;
using BepInEx.Logging;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration
{
    public static class ReworkStarGen
    {

        
        public static void CreateStarPlanetsRework(GalaxyData galaxy, StarData star, GameDesc gameDesc, PlanetGeneratorSettings genSettings)
        {
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
            bool[] assignedOrbits;
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

            if (Patch.UseNewGasGiantOrbitPicker.Value) assignedOrbits = DistributePlanets(annexSeed, genSettings);
            else assignedOrbits = new bool[0];
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
                        isGasGiant = assignedOrbits[i];
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
        static int SelectOrbitRange(ref Random seed, ref int orbitRange1nbTelluric, ref int orbitRange2nbTelluric, ref int orbitRange3nbTelluric, ref float totalProbability, ref int totalPlanetsToGenerate, ref float orbit1Probability, ref float orbit2Probability, ref float orbit3Probability)
        {
            Patch.Debug("Selecting Orbit Range", LogLevel.Debug, Patch.DebugPlanetDistribution);
            float r = (float)seed.NextDouble();
            float bias1 = Patch.DistributionBiasInner.Value; //1 = always put rocky planets here. 0 = never
            float bias2 = Patch.DistributionBiasMiddle.Value;
            float bias3 = Patch.DistributionBiasOuter.Value;
            int orbit1TotalSlots = totalPlanetsToGenerate / 3;
            int orbit2TotalSlots = totalPlanetsToGenerate / 3 + totalPlanetsToGenerate % 3;
            int orbit3TotalSlots = totalPlanetsToGenerate / 3;

            totalProbability = (orbitRange1nbTelluric < (orbit1TotalSlots) ? bias1 : 0) + (orbitRange2nbTelluric < (orbit2TotalSlots) ? bias2 : 0) + (orbitRange3nbTelluric < (orbit3TotalSlots) ? bias3 : 0);
            if (totalProbability == 0) return 1;
            orbit1Probability = (orbitRange1nbTelluric < (orbit1TotalSlots) ? bias1 : 0) / totalProbability;
            orbit2Probability = (orbitRange2nbTelluric < (orbit2TotalSlots) ? bias2 : 0) / totalProbability;
            orbit3Probability = (orbitRange3nbTelluric < (orbit3TotalSlots) ? bias3 : 0) / totalProbability;

            Patch.Debug("SelectOrbitRange:Available Orbit Slots: " + String.Format("|{0,10}|{1,10}|{2,10}|", (orbit1TotalSlots - orbitRange1nbTelluric), (orbit2TotalSlots - orbitRange2nbTelluric), (orbit3TotalSlots - orbitRange3nbTelluric)), LogLevel.Debug, Patch.DebugPlanetDistribution);
            Patch.Debug("SelectOrbitRange:Orbit Probability    : " + String.Format("|{0,10}|{1,10}|{2,10}|", orbit1Probability, orbit2Probability, orbit3Probability), LogLevel.Debug, Patch.DebugPlanetDistribution);
            if (orbitRange1nbTelluric < (orbit1TotalSlots) && r < orbit1Probability) return 1;
            if (orbitRange2nbTelluric < (orbit2TotalSlots) && r < (orbit1Probability + orbit2Probability)) return 2;
            if (orbitRange3nbTelluric < (orbit3TotalSlots) && r < (orbit1Probability + orbit2Probability + orbit3Probability)) return 3;

            while (orbitRange1nbTelluric + orbitRange2nbTelluric + orbitRange3nbTelluric < totalPlanetsToGenerate)
            {
                float r2 = (float)seed.NextDouble();
                if (r2 <= 0.33 && orbitRange1nbTelluric < (orbit1TotalSlots)) return 1;
                if (r2 <= 0.66 && orbitRange2nbTelluric < (orbit2TotalSlots)) return 2;
                if (orbitRange3nbTelluric <= orbit3TotalSlots) return 3;
            }
            return 4;
        }
        static bool[] DistributePlanets(Random seed, PlanetGeneratorSettings genSettings)
        {
            int totalTelluricPlanets = genSettings.nbOfTelluricPlanets;
            int totalGasGiants = genSettings.nbOfGasGiantPlanets;
            var totalSlots = totalTelluricPlanets + totalGasGiants;
            int orbitRange1Slots = totalSlots/3;
            int orbitRange2Slots = totalSlots/3 + totalSlots%3;
            int orbitRange3Slots = totalSlots/3;
            
            float bias1 = Patch.DistributionBiasInner.Value; //1 = always put rocky planets here. 0 = never
            float bias2 = Patch.DistributionBiasMiddle.Value; 
            float bias3 = Patch.DistributionBiasOuter.Value; 
            float totalBias = bias1 + bias2 + bias3;
            
            var orbitRange1Probability = bias1 / totalBias;
            var orbitRange2Probability = bias2 / totalBias;
            var orbitRange3Probability = bias3 / totalBias;

            int remainingTelluricPlanets = totalTelluricPlanets;
            int orbitRange1nbTelluric = 0, orbitRange2nbTelluric = 0, orbitRange3nbTelluric = 0;

            bool[] assignedOrbits = new bool[genSettings.nbOfPlanets]; //Reset the assignedOrbits array
            Patch.Debug("Distributing " + genSettings.nbOfPlanets + " Planets using new Generator: "+ genSettings.nbOfTelluricPlanets + " Telluric, "+genSettings.nbOfGasGiantPlanets + " Gas Giants", LogLevel.Debug, Patch.DebugPlanetDistribution);
            while (remainingTelluricPlanets > 0)
            {
                var orbitRange = SelectOrbitRange(ref seed, ref orbitRange1nbTelluric, ref orbitRange2nbTelluric, ref orbitRange3nbTelluric, ref totalBias, ref totalSlots, ref orbitRange1Probability, ref orbitRange2Probability, ref orbitRange3Probability);
                Patch.Debug("OrbitRange " + orbitRange + " Selected", LogLevel.Debug, Patch.DebugPlanetDistribution);
                if (orbitRange == 1) orbitRange1nbTelluric++;
                if (orbitRange == 2) orbitRange2nbTelluric++;
                if (orbitRange == 3) orbitRange3nbTelluric++;
                if (orbitRange == 4) Patch.Debug("PickOrbit Failed", LogLevel.Warning, true);
                remainingTelluricPlanets--;
            }
            int i = 0;
            AssignOrbits(ref assignedOrbits, orbitRange1nbTelluric, orbitRange1Slots - orbitRange1nbTelluric, ref seed, ref i);
            AssignOrbits(ref assignedOrbits, orbitRange2nbTelluric, orbitRange2Slots - orbitRange2nbTelluric, ref seed, ref i);
            AssignOrbits(ref assignedOrbits, orbitRange3nbTelluric, orbitRange3Slots - orbitRange3nbTelluric, ref seed, ref i);
            Patch.Debug("Final Orbit Ranges:", LogLevel.Debug, Patch.DebugPlanetDistribution);
            if (Patch.DebugPlanetDistribution)
            {
                string output = (orbitRange1Slots > 0) ? "\n--Orbit Range 1--\n|" : "";
                for (var i2 = 0; i2 < assignedOrbits.Length; i2++)
                {
                    if (i2 == orbitRange1Slots) output += "\n--Orbit Range 2--\n|";
                    if (i2 == orbitRange1Slots + orbitRange2Slots) output += "\n--Orbit Range 3--\n|";
                    output += ((assignedOrbits[i2]) ? " Gas |" : " Telluric |");
                }
                Patch.Debug(output, LogLevel.Debug, Patch.DebugPlanetDistribution);
            }
            
            Patch.Debug("Distribution of Planets Complete.\n\n", LogLevel.Debug, Patch.DebugPlanetDistribution);
            return assignedOrbits;
        }
        static void AssignOrbits(ref bool[] assignedOrbits, int remainingTelluricPlanets, int remainingGasGiants, ref Random seed, ref int position)
        {
            while (remainingTelluricPlanets > 0 && remainingGasGiants > 0)
            {
                float r = (float)seed.NextDouble();
                if (r < (remainingTelluricPlanets / (remainingTelluricPlanets + remainingGasGiants)))
                {
                    assignedOrbits[position] = false;
                    remainingTelluricPlanets--;
                    Patch.Debug("Assigned Telluric at position " + position, LogLevel.Debug, Patch.DebugPlanetDistribution);
                }
                else
                {
                    assignedOrbits[position] = true;
                    remainingGasGiants--;
                    Patch.Debug("Assigned Gas Giant at position " + position, LogLevel.Debug, Patch.DebugPlanetDistribution);
                }
                position++;
            }
            while (remainingTelluricPlanets > 0)
            {
                assignedOrbits[position] = false;
                remainingTelluricPlanets--;
                Patch.Debug("Assigned Telluric at position " + position, LogLevel.Debug, Patch.DebugPlanetDistribution);
                position++;
            }
            while (remainingGasGiants > 0)
            {
                assignedOrbits[position] = true;
                remainingGasGiants--;
                Patch.Debug("Assigned Gas Giant at position " + position, LogLevel.Debug, Patch.DebugPlanetDistribution);
                position++;
            }
        }
    }
}