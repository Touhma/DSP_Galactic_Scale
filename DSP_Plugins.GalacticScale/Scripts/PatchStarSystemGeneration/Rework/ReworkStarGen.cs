using System.Collections.Generic;
using BepInEx.Logging;
using Steamworks;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    public static class ReworkStarGen {
        public static void CreateStarPlanetsRework(GalaxyData galaxy,  StarData star,  GameDesc gameDesc , PlanetGeneratorSettings genSettings) {
            star.name = SystemsNames.systems[star.index];

            Patch.Debug("System " + star.name + " - " + star.type + " - " + star.spectr, LogLevel.Debug,
                Patch.DebugStarGen);

            // Random Generators Inits
            UnityRandom.InitState(star.seed);
            Random mainSeed = new Random(star.seed);
            Random annexSeed = new Random(mainSeed.Next());

            // InnerCount for the System
            List<PlanetForGenerator> planetsToGenerate = new List<PlanetForGenerator>();

            // settings from the config
            PatchForStarSystemGeneration.StarSystemSetting currentSettings;

            if (star.type == EStarType.BlackHole || star.type == EStarType.GiantStar ||
                star.type == EStarType.NeutronStar || star.type == EStarType.WhiteDwarf) {
                currentSettings = Patch.GeneratorSpecialsSystemConfig[star.type];
            }
            else {
                currentSettings = Patch.GeneratorMainSystemConfig[star.spectr];
            }

            //Debugging configs
            Patch.Debug("*************************** : \n" +
                        "ChanceMoonGasGiant : " + currentSettings.ChanceGasGiantMoon + "\n" +
                        "ChanceMoonTelluric : " + currentSettings.ChanceMoonTelluric + "\n" +
                        "ChancePlanetTelluric : " + currentSettings.ChanceTelluricPlanet + "\n" +
                        "ChancePlanetGasGiant : " + currentSettings.ChanceGasGiant + "\n" +
                        "MaxMoonNb : " + currentSettings.MaxMoonNb + "\n" +
                        "MaxPlanetNb : " + currentSettings.MaxPlanetNb + "\n" +
                        "ChanceJumpOrbitMoons : " + currentSettings.ChanceJumpOrbitMoons + "\n" +
                        "ChanceJumpOrbitPlanets : " + currentSettings.ChanceJumpOrbitPlanets + "\n" +
                        "*************************** ", LogLevel.Debug, Patch.DebugStarGenDeep);

            Patch.Debug("Definition of Nb of planets In the system :", LogLevel.Debug, Patch.DebugStarGenDeep);


            // Define how much planets the system have
            for (var i = 0; i < currentSettings.MaxPlanetNb; i++) {
                if (annexSeed.NextDouble() <= currentSettings.ChanceTelluricPlanet) {
                    genSettings.nbOfTelluricPlanets++;
                    genSettings.nbOfPlanets++;
                    genSettings.nbOfStellarBodies++;
                }
            }

            // Define how much of the planets are GasGiant
            for (var j = genSettings.nbOfPlanets; j < currentSettings.MaxPlanetNb; j++) {
                if (annexSeed.NextDouble() <= currentSettings.ChanceGasGiant) {
                    genSettings.nbOfGasGiantPlanets++;
                    genSettings.nbOfPlanets++;
                    genSettings.nbOfStellarBodies++;
                }
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
                for (var i = 0; i < currentSettings.MaxMoonNb; i++) {
                    if (annexSeed.NextDouble() <= currentSettings.ChanceMoonTelluric) {
                        genSettings.nbOfMoonsTelluric++;
                        genSettings.nbOfMoons++;
                        genSettings.nbOfStellarBodies++;
                    }
                }
            }
            else {
                Patch.Debug("No Telluric in the system :", LogLevel.Debug, Patch.DebugStarGenDeep);
            }

            // Define how much moons the gasGiants planets have 
            if (genSettings.nbOfGasGiantPlanets != 0) {
                for (var i = genSettings.nbOfMoons; i < currentSettings.MaxMoonNb; i++) {
                    if (annexSeed.NextDouble() <= currentSettings.ChanceGasGiantMoon) {
                        genSettings.nbOfMoonsGasGiant++;
                        genSettings.nbOfMoons++;
                        genSettings.nbOfStellarBodies++;
                    }
                }
            }
            else {
                Patch.Debug("No Gas Giant in the system :", LogLevel.Debug, Patch.DebugStarGenDeep);
            }

            star.planets = new PlanetData[genSettings.nbOfStellarBodies];

            Patch.Debug("*************************** : \n" +
                        "nbOfMoons : " + genSettings.nbOfMoons + "\n" +
                        "nbOfMoonsTelluric : " + genSettings.nbOfMoonsTelluric + "\n" +
                        "nbOfMoonsGasGiant : " + genSettings.nbOfMoonsGasGiant + "\n" +
                        "*************************** ", LogLevel.Debug, Patch.DebugStarGenDeep);

            Patch.Debug("*************************** :", LogLevel.Debug, Patch.DebugStarGenDeep);

            Patch.Debug("Define the belts for whatever they are for :", LogLevel.Debug, Patch.DebugStarGenDeep);

            //Define where the 2 asteroids belts are ( maybe not implemented ) 
            int asterBelt1OrbitIndex = UnityRandom.Range(1, genSettings.nbOfPlanets - 1);
            int asterBelt2OrbitIndex = UnityRandom.Range(asterBelt1OrbitIndex + 1, genSettings.nbOfPlanets);

            Patch.Debug("asterBelt1OrbitIndex :" + asterBelt1OrbitIndex, LogLevel.Debug, Patch.DebugStarGenDeep);
            Patch.Debug("asterBelt2OrbitIndex :" + asterBelt2OrbitIndex, LogLevel.Debug, Patch.DebugStarGenDeep);


            //Attach the information to the star
            star.asterBelt1OrbitIndex = asterBelt1OrbitIndex;
            star.asterBelt2OrbitIndex = asterBelt2OrbitIndex;
            star.asterBelt1Radius = Patch.OrbitRadiusPlanetArray[asterBelt1OrbitIndex];
            star.asterBelt2Radius = Patch.OrbitRadiusPlanetArray[asterBelt2OrbitIndex];

            Patch.Debug("*************************** :", LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("\nSystem Presets : ", LogLevel.Debug, Patch.DebugStarGen);
            string preset =
                "nbOfPlanets : " + genSettings.nbOfPlanets + "\n" +
                "nbOfTelluricPlanets : " + genSettings.nbOfTelluricPlanets + "\n" +
                "nbOfGasGiantPlanets : " + genSettings.nbOfGasGiantPlanets + "\n" +
                "nbOfMoons : " + genSettings.nbOfMoons + "\n" +
                "nbOfMoonsTelluric : " + genSettings.nbOfMoonsTelluric + "\n" +
                "nbOfMoonsGasGiant : " + genSettings.nbOfMoonsGasGiant + "\n\n";
            Patch.Debug(preset, LogLevel.Debug, Patch.DebugStarGen);

            //
            //preparation of the planet creation :
            int infoSeed;
            int genSeed = 0;

            // planets pre-generation
            int nbOfBodiesPreGenerated = 0;
            int nbOfPlanetsPreGenerated = 0;
            int planetsPreGeneratedNumber = 1;
            int nbOfMoonsPreGenerated = 0;


            infoSeed = annexSeed.Next();
            genSeed = annexSeed.Next();

            int currentOrbitPlanetIndex = 1;
            int previousOrbitPlanetIndex = 0;
            int currentOrbitMoonIndex = 1;
            int previousOrbitMoonIndex = 0;

            int beltGenerated = 0;

            int jumpOrbitMargin;

            for (var i = 0; i < genSettings.nbOfStellarBodies; i++) {
                
                Patch.Debug("bodies generated !"  + nbOfBodiesPreGenerated, LogLevel.Debug, Patch.DebugStarGenDeep);
                Patch.Debug("genSettings.nbOfPlanets + genSettings.nbOfMoons !"  + (genSettings.nbOfPlanets + genSettings.nbOfMoons), LogLevel.Debug, Patch.DebugStarGenDeep);
                bool isGasGiant = false;
                int orbitAround = 0;

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

                    if (currentOrbitPlanetIndex < jumpOrbitMargin && jumpOrbitMargin < currentSettings.JumpOrbitPlanetMax) {
                        if (annexSeed.NextDouble() < currentSettings.ChanceJumpOrbitPlanets) {
                            // can jump orbit up to JumpOrbitPlanetIndex
                            currentOrbitPlanetIndex = UnityRandom.Range(currentOrbitPlanetIndex, currentOrbitPlanetIndex + currentSettings.JumpOrbitPlanetMax);
                        }
                    }

                    previousOrbitPlanetIndex = currentOrbitPlanetIndex;


                    orbitAround = 0;

                    if (nbOfBodiesPreGenerated < genSettings.nbOfTelluricPlanets + beltGenerated) {
                        //telluric
                        isGasGiant = false;
                    }
                    else {
                        //gasgiant
                        isGasGiant = true;
                    }

                    planetsToGenerate.Add(new PlanetForGenerator(nbOfBodiesPreGenerated - beltGenerated, orbitAround, currentOrbitPlanetIndex, planetsPreGeneratedNumber, isGasGiant, genSeed,infoSeed, null));
                    Patch.Debug("planetsToGenerate -->   \n" + planetsToGenerate[nbOfPlanetsPreGenerated].ToString(), LogLevel.Debug, Patch.DebugStarGen);
                    nbOfPlanetsPreGenerated++;
                    planetsPreGeneratedNumber++;
                    currentOrbitPlanetIndex++;
                    if (isGasGiant) {
                        Patch.Debug("gas Giant generated !", LogLevel.Debug, Patch.DebugStarGen);
                    }
                    else {
                        Patch.Debug("planet generated !", LogLevel.Debug, Patch.DebugStarGen);
                    }
                  
                }
                else if (nbOfBodiesPreGenerated < genSettings.nbOfPlanets + genSettings.nbOfMoons + beltGenerated ) {
                    Patch.Debug("Moon in generation!", LogLevel.Debug, Patch.DebugStarGen);

                    isGasGiant = false;
                    
                    if (genSettings.nbOfTelluricPlanets != 0 && nbOfBodiesPreGenerated <= genSettings.nbOfPlanets + genSettings.nbOfMoonsTelluric + beltGenerated ) {
                        // telluric moon
                        orbitAround = UnityRandom.Range(1, genSettings.nbOfTelluricPlanets);
                        Patch.Debug("telluric moon! orbit around : " +  orbitAround, LogLevel.Debug, Patch.DebugStarGenDeep);
                        
                    }
                    else {
                        if (genSettings.nbOfGasGiantPlanets != 0) {
                            //gasgiant moon 
                            orbitAround = UnityRandom.Range(genSettings.nbOfTelluricPlanets + 1, genSettings.nbOfTelluricPlanets + genSettings.nbOfGasGiantPlanets);
                            Patch.Debug("gas moon! orbit around : " +  orbitAround, LogLevel.Debug, Patch.DebugStarGenDeep);
                        }
                    }

                    if (orbitAround <= 0) {
                        Patch.Debug("Issue in moon generation : " +  orbitAround, LogLevel.Debug, Patch.DebugStarGen);
                    }
                    //jumporbit moon 
                    int jumpOrbitMoonMargin = 0;

                    jumpOrbitMargin = Patch.OrbitRadiusArrayMoonsNb.Value - (genSettings.nbOfMoons - nbOfMoonsPreGenerated);

                    Patch.Debug("orbitAround - 1 : " + (orbitAround - 1), LogLevel.Debug, Patch.DebugStarGenDeep);
                    Patch.Debug("planetsToGenerate.Count :" + planetsToGenerate.Count, LogLevel.Debug, Patch.DebugStarGenDeep);
                    Patch.Debug("planetsToGenerate[orbitAround - 1] :" + planetsToGenerate[orbitAround - 1], LogLevel.Debug, Patch.DebugStarGenDeep);


                    int currentPlanetMoonsNb ;
                   
                        currentPlanetMoonsNb = planetsToGenerate[orbitAround - 1].moons.Count;
                  
                    
                    Patch.Debug("currentPlanetMoonsNb] :" + currentPlanetMoonsNb, LogLevel.Debug, Patch.DebugStarGenDeep);
                    if (currentPlanetMoonsNb != 0) {
                        currentOrbitMoonIndex = planetsToGenerate[orbitAround - 1].moons[currentPlanetMoonsNb - 1].orbitIndex;
                    }
                    else {
                        currentOrbitMoonIndex = 0;
                    }

                    Patch.Debug("currentOrbitMoonIndex : " + currentOrbitMoonIndex, LogLevel.Debug, Patch.DebugStarGenDeep);

                    if (currentOrbitMoonIndex < jumpOrbitMargin && jumpOrbitMargin < currentSettings.JumpOrbitMoonMax) {
                        if (annexSeed.NextDouble() < currentSettings.ChanceJumpOrbitMoons) {
                            // can jump orbit up to JumpOrbitPlanetIndex
                            int oldOrbitIndex = currentOrbitMoonIndex;
                            currentOrbitMoonIndex = UnityRandom.Range(currentOrbitMoonIndex, currentOrbitMoonIndex + currentSettings.JumpOrbitMoonMax);
                        }
                    }
                    
                    planetsToGenerate[orbitAround - 1].AddMoonInOrbit(nbOfBodiesPreGenerated, currentOrbitMoonIndex, genSeed, infoSeed);
                    
                
                    nbOfMoonsPreGenerated++;
                    Patch.Debug("moonToGenerate --> +" + genSettings.nbOfMoons +  " --> nbOfMoonsPreGenerated : " + nbOfMoonsPreGenerated, LogLevel.Debug, Patch.DebugStarGenDeep);
                }

                nbOfBodiesPreGenerated++;
            }

            Patch.Debug("Recap of what have to be generated : \n", LogLevel.Debug, Patch.DebugStarGen);

            int finalIndex = 0;
            foreach (var planet in planetsToGenerate) {
                string debugLine = "A ";
                
                planet.planetIndex = finalIndex;
                if (planet.isGasGiant) {
                    debugLine += " Gas Giant :" + planet.planetIndex + "with values : \n";
                }
                else {
                    debugLine += " Telluric Planet :" + planet.planetIndex + "with values : \n";
                }
                
                //planet.ToString();
                
                //planet.GenerateThePlanet(ref galaxy,ref star,ref gameDesc);
                PlanetGen.CreatePlanet( galaxy, star,  gameDesc, planet.planetIndex, planet.orbitAround, planet.orbitIndex, planet.number, planet.isGasGiant, planet.infoSeed, planet.genSeed);
                star.planets[finalIndex].name =  star.name + " - " + RomanNumbers.roman[planet.number];
                planet.name = star.planets[finalIndex].name;
                Debug.Log(star.planets[finalIndex].name);
                finalIndex++;
                //debugLine += planet.ToString() + "\n\n";
                if (planet.moons.Count != 0) {
                    debugLine += "with " + planet.moons.Count + " Moons  : \n\n";
                    foreach (var moon in planet.moons) {
                        
                        moon.planetIndex = finalIndex;
                        debugLine += " Moon : " + moon.planetIndex + "\n";
                        PlanetGen.CreatePlanet( galaxy, star,  gameDesc, moon.planetIndex, moon.orbitAround, moon.orbitIndex, moon.number, moon.isGasGiant, moon.infoSeed, moon.genSeed);
                        star.planets[moon.planetIndex].name = planet.name + " - " + RomanNumbers.roman[moon.number];
                        Debug.Log(star.planets[moon.planetIndex].name);
                        finalIndex++;
                    }
                }
                Patch.Debug(debugLine, LogLevel.Debug, Patch.DebugStarGen);
            }

        }
    }
}