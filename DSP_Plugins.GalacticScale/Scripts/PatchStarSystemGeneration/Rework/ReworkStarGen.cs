using BepInEx.Logging;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    public static class ReworkStarGen {
        public static void CreateStarPlanetsRework(ref GalaxyData galaxy, ref StarData star, ref GameDesc gameDesc) {
            star.name = SystemsNames.systems[star.index];
            
            Patch.Debug("System " + star.name + " - " + star.type + " - " + star.spectr, LogLevel.Debug,
                Patch.DebugStarGen);
            

            
            // Here we Decide How we create the planets
            // Random Generators
            UnityRandom.InitState(star.seed);

            Random mainSeed = new Random(star.seed);
            mainSeed.Next();
            mainSeed.Next();
            mainSeed.Next();
            // Random Generator 
            Random annexSeed = new Random(mainSeed.Next());

            //InnerParameters of the system

            // InnerCount for the System
            int nbOfPlanets = 0;
            int nbOfTelluricPlanets = 0;
            int nbOfGasGiantPlanets = 0;
            int nbOfMoons = 0;
            int nbOfMoonsTelluric = 0;
            int nbOfMoonsGasGiant = 0;

            PatchForStarSystemGeneration.StarSystemSetting currentSettings;

            if (star.type == EStarType.BlackHole || star.type == EStarType.GiantStar ||
                star.type == EStarType.NeutronStar || star.type == EStarType.WhiteDwarf) {
                Patch.Debug("Load config for star type :" + star.type, LogLevel.Debug, Patch.DebugStarGen);
                currentSettings = Patch.GeneratorSpecialsSystemConfig[star.type];
            }
            else {
                Patch.Debug("Load config for star spectr :" + star.spectr, LogLevel.Debug, Patch.DebugStarGen);
                currentSettings = Patch.GeneratorMainSystemConfig[star.spectr];
            }

            //Debugging configs
            Patch.Debug("*************************** :", LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("ChanceMoon :" + currentSettings.ChanceMoon, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("ChancePlanet :" + currentSettings.ChancePlanet, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("ChanceGasGiant :" + currentSettings.ChanceGasGiant, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("ChanceMoonTelluric :" + currentSettings.ChanceMoonTelluric, LogLevel.Debug,
                Patch.DebugStarGen);
            Patch.Debug("MaxMoonNb :" + currentSettings.MaxMoonNb, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("MaxPlanetNb :" + currentSettings.MaxPlanetNb, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("ChanceJumpOrbitMoons :" + currentSettings.ChanceJumpOrbitMoons, LogLevel.Debug,
                Patch.DebugStarGen);
            Patch.Debug("ChanceJumpOrbitPlanets :" + currentSettings.ChanceJumpOrbitPlanets, LogLevel.Debug,
                Patch.DebugStarGen);
            Patch.Debug("*************************** :", LogLevel.Debug, Patch.DebugStarGen);


            Patch.Debug("Definition of Nb of planets In the system :", LogLevel.Debug, Patch.DebugStarGen);
            // Define how much planets the system have
            for (var i = 0; i < currentSettings.MaxPlanetNb; i++) {
                if (annexSeed.NextDouble() <= currentSettings.ChancePlanet) {
                    nbOfPlanets++;
                }
            }

            // Define how much of the planets are GasGiant
            for (var j = 0; j < nbOfPlanets; j++) {
                if (annexSeed.NextDouble() <= currentSettings.ChanceGasGiant) {
                    nbOfGasGiantPlanets++;
                }
            }

            nbOfTelluricPlanets = nbOfPlanets - nbOfGasGiantPlanets;

            Patch.Debug("nbOfPlanets :" + nbOfPlanets, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("nbOfTelluricPlanets :" + nbOfTelluricPlanets, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("nbOfGasGiantPlanets :" + nbOfGasGiantPlanets, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("*************************** :", LogLevel.Debug, Patch.DebugStarGen);

            Patch.Debug("Definition of Nb of Moons In the system :", LogLevel.Debug, Patch.DebugStarGen);
            // Define how much moons the system have
            for (var i = 0; i < currentSettings.MaxMoonNb; i++) {
                if (annexSeed.NextDouble() <= currentSettings.ChanceMoon) {
                    nbOfMoons++;
                }
            }

            // Define how much moons the telluric  planets have 
            if (nbOfTelluricPlanets != 0) {
                for (var i = 0; i < nbOfMoons; i++) {
                    if (annexSeed.NextDouble() <= currentSettings.ChanceMoonTelluric) {
                        nbOfMoonsTelluric++;
                    }
                }
            }
            else {
                Patch.Debug("No Telluric in the system :" , LogLevel.Debug, Patch.DebugStarGen);
            }

            //Define how much moons GasGiants have
            if (nbOfGasGiantPlanets != 0) {
                nbOfMoonsGasGiant = nbOfMoons - nbOfMoonsTelluric;
            }
            else {
                Patch.Debug("No Gas Giant in the system :" , LogLevel.Debug, Patch.DebugStarGen);
                nbOfMoonsGasGiant = 0;
            }


            Patch.Debug("nbOfMoons :" + nbOfMoons, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("nbOfMoonsTelluric :" + nbOfMoonsTelluric, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("nbOfMoonsGasGiant :" + nbOfMoonsGasGiant, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("*************************** :", LogLevel.Debug, Patch.DebugStarGen);

            Patch.Debug("Define the belts for whatever they are for :", LogLevel.Debug, Patch.DebugStarGen);

            //Define where the 2 asteroids belts are ( maybe not implemented ) 
            int asterBelt1OrbitIndex = UnityRandom.Range(1, nbOfPlanets-1);
            int asterBelt2OrbitIndex = UnityRandom.Range(asterBelt1OrbitIndex + 1, nbOfPlanets);

            Patch.Debug("asterBelt1OrbitIndex :" + asterBelt1OrbitIndex, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("asterBelt2OrbitIndex :" + asterBelt2OrbitIndex, LogLevel.Debug, Patch.DebugStarGen);


            //Attach the information to the star
            star.asterBelt1OrbitIndex = asterBelt1OrbitIndex;
            star.asterBelt2OrbitIndex = asterBelt2OrbitIndex;
            star.asterBelt1Radius = Patch.OrbitRadiusPlanetArray[asterBelt1OrbitIndex];
            star.asterBelt2Radius = Patch.OrbitRadiusPlanetArray[asterBelt2OrbitIndex];
            Patch.Debug("*************************** :", LogLevel.Debug, Patch.DebugStarGen);

            Patch.Debug("Preparation of planet Creation :", LogLevel.Debug, Patch.DebugStarGen);
            //preparation of the planet creation :
            int infoSeed;
            int genSeed;

            star.planetCount = nbOfPlanets + nbOfMoons;
            star.planets = new PlanetData[star.planetCount];


            int planetsGenerated = 0;
            int planetsTelluricGenerated = 0;
            int planetsGasGenerated = 0;
            int moonsGenerated = 0;
            int moonsTelluricGenerated = 0;
            int moonsGasGenerated = 0;

            //nb of moons stored for each planets ( starting at 1 for convenience )
            int[] planetMoons = new int[nbOfPlanets + 1];
            //nb of orbit moons stored for each planets ( starting at 1 for convenience )
            int[] planetOrbitMoons = new int[nbOfPlanets + 1];

            //define starting orbit of the planet
            int currentOrbitIndex = currentSettings.JumpOrbitPlanetIndex;


            // Creation of the planets
            for (var i = 0; i < nbOfPlanets; i++) {
                //check for the belts : 
                if (asterBelt1OrbitIndex == currentOrbitIndex) {
                    Patch.Debug("Jump Belt 1 Orbit :", LogLevel.Debug, Patch.DebugStarGen);
                    currentOrbitIndex++;
                }

                if (asterBelt2OrbitIndex == currentOrbitIndex) {
                    Patch.Debug("Jump Belt 2 Orbit :", LogLevel.Debug, Patch.DebugStarGen);
                    currentOrbitIndex++;
                }

                //chance to jump an orbit

                int jumpOrbitMargin = Patch.OrbitRadiusArrayPlanetNb.Value - (nbOfPlanets - planetsGenerated);
                if (currentOrbitIndex < jumpOrbitMargin && jumpOrbitMargin < currentSettings.JumpOrbitPlanetIndex) {
                    if (annexSeed.NextDouble() < currentSettings.ChanceJumpOrbitPlanets) {
                        // can jump orbit up to JumpOrbitPlanetIndex
                        int oldOrbitIndex = currentOrbitIndex;
                        currentOrbitIndex = UnityRandom.Range(currentOrbitIndex,
                            currentOrbitIndex + currentSettings.JumpOrbitPlanetIndex);
                        Patch.Debug(
                            "Jump " + (currentOrbitIndex - oldOrbitIndex) + " Orbits, current orbit : " +
                            currentOrbitIndex, LogLevel.Debug, Patch.DebugStarGen);
                    }
                }

                infoSeed = annexSeed.Next();
                genSeed = annexSeed.Next();


                if (planetsTelluricGenerated < nbOfTelluricPlanets) {
                    Patch.Debug("CreatePlanet Telluric on orbit : " + currentOrbitIndex, LogLevel.Debug,
                        Patch.DebugStarGen);
                    // create the Telluric planet
                    star.planets[planetsGenerated] =
                        PlanetGen.CreatePlanet(galaxy, star, gameDesc, i, 0, currentOrbitIndex, planetsGenerated + 1,
                            false,
                            infoSeed, genSeed);
                    planetsTelluricGenerated++;
                }
                else if (planetsGasGenerated < nbOfGasGiantPlanets) {
                    Patch.Debug("CreatePlanet GasGiant on orbit : " + currentOrbitIndex, LogLevel.Debug,
                        Patch.DebugStarGen);
                    // create the GasGiant planet
                    star.planets[planetsGenerated] =
                        PlanetGen.CreatePlanet(galaxy, star, gameDesc, i, 0, currentOrbitIndex, planetsGenerated + 1,
                            true,
                            infoSeed, genSeed);
                    planetsGasGenerated++;
                }

                //commons
                planetsGenerated++;
                currentOrbitIndex++;
                

            }

            Patch.Debug("Planets Generated : " + planetsGenerated, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("Planets Telluric Generated : " + planetsTelluricGenerated, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("Planets Gas Giant Generated : " + planetsGasGenerated, LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("*************************** :", LogLevel.Debug, Patch.DebugStarGen);

            // starting index of the gas planets :
            int gasPlanetStartingIndex = planetsTelluricGenerated + 1;

            Patch.Debug("gasPlanetStartingIndex : " + gasPlanetStartingIndex, LogLevel.Debug, Patch.DebugStarGen);

            Patch.Debug("Moons Creation : ", LogLevel.Debug, Patch.DebugStarGen);
            Patch.Debug("Moons planetOrbitMoons lenght : " + planetOrbitMoons.Length, LogLevel.Debug,
                Patch.DebugStarGen);


            Patch.Debug("Test Moon Orbit Jumping", LogLevel.Debug, Patch.DebugStarGen);
            // Creation of the moons for each Planets
            for (var moonIndex = 0; moonIndex < nbOfMoons; moonIndex++) {
                //how much orbit the moon is jumping
                int jumpOrbitMoonMargin = 0;
                //id of the planet selected
                int orbitAroundSelected = 0;
                //if telluric Moon : 
                if (moonsTelluricGenerated < nbOfMoonsTelluric) {
                    //between the index 1 & the number generated. so between 1 & 4 if 4 are generated
                    orbitAroundSelected = UnityRandom.Range(1, planetsTelluricGenerated);
                    jumpOrbitMoonMargin = Patch.OrbitRadiusArrayMoonsNb.Value - (nbOfMoonsTelluric - moonsTelluricGenerated);
                    Patch.Debug(
                        "Telluric Moon Creation around: " + orbitAroundSelected + " jumpOrbitMoonMargin : " +
                        jumpOrbitMoonMargin, LogLevel.Debug, Patch.DebugStarGen);
                }
                else if (moonsGasGenerated < nbOfMoonsGasGiant) {
                    orbitAroundSelected = UnityRandom.Range(gasPlanetStartingIndex,
                        planetsTelluricGenerated + planetsGasGenerated);
                    jumpOrbitMoonMargin = Patch.OrbitRadiusArrayMoonsNb.Value - (nbOfMoonsGasGiant - moonsGasGenerated);
                    Patch.Debug(
                        "GasGiant Moon Creation around: " + orbitAroundSelected + " jumpOrbitMoonMargin : " +
                        jumpOrbitMoonMargin, LogLevel.Debug, Patch.DebugStarGen);
                }
                else {
                    //Trouble
                    orbitAroundSelected = 0;
                    jumpOrbitMoonMargin = 0;
                    Patch.Debug("Got Some trouble here .... /!\\/!\\/!\\/!\\/!\\/!\\/!\\", LogLevel.Debug,
                        Patch.DebugStarGen);
                }

                Patch.Debug("planetOrbitMoons" + planetOrbitMoons.Length, LogLevel.Debug, Patch.DebugStarGen);
                Patch.Debug("planetOrbitMoons[orbitAroundSelected]" + planetOrbitMoons[orbitAroundSelected], LogLevel.Debug, Patch.DebugStarGen);
                int orbitJumped = 0;
                //Same than before for the planets
                if (planetOrbitMoons[orbitAroundSelected] < jumpOrbitMoonMargin &&
                    jumpOrbitMoonMargin < currentSettings.JumpOrbitMoonIndex) {
                    Patch.Debug("Test Moon Orbit Jumping", LogLevel.Debug, Patch.DebugStarGen);
                    Patch.Debug("Orbit Selected for the moon : " + orbitAroundSelected, LogLevel.Debug,
                        Patch.DebugStarGen);
                    Patch.Debug("Nb of Orbit around the moon host : " + planetOrbitMoons[orbitAroundSelected],
                        LogLevel.Debug, Patch.DebugStarGen);
                    if (annexSeed.NextDouble() < currentSettings.ChanceJumpOrbitMoons) {
                        // can jump orbit up to JumpOrbitMoonIndex
                        orbitJumped += UnityRandom.Range(planetOrbitMoons[orbitAroundSelected],
                            planetOrbitMoons[orbitAroundSelected] + currentSettings.JumpOrbitMoonIndex);
                        Patch.Debug(
                            "Moon is Jumping orbits : " + orbitAroundSelected + " jumpOrbitMoonMargin : " +
                            jumpOrbitMoonMargin + " orbitJumped : " + orbitJumped, LogLevel.Debug, Patch.DebugStarGen);
                    }
                }

                Patch.Debug("Moon orbit Selected : " + (planetOrbitMoons[orbitAroundSelected] + orbitJumped),
                    LogLevel.Debug, Patch.DebugStarGen);

                infoSeed = annexSeed.Next();
                genSeed = annexSeed.Next();
                //create the moon

                star.planets[planetsGenerated+moonsGenerated]= PlanetGen.CreatePlanet(galaxy, star, gameDesc, moonIndex, orbitAroundSelected,
                    planetOrbitMoons[orbitAroundSelected] + orbitJumped, planetMoons[orbitAroundSelected], false,
                    infoSeed, genSeed);
                Patch.Debug("Moon is Created : ", LogLevel.Debug, Patch.DebugStarGen);
                Patch.Debug("Orbit index : " + (planetOrbitMoons[orbitAroundSelected] + orbitJumped), LogLevel.Debug,
                    Patch.DebugStarGen);
                Patch.Debug("Number : " + (planetMoons[orbitAroundSelected]), LogLevel.Debug, Patch.DebugStarGen);

                planetOrbitMoons[orbitAroundSelected]++;
                planetMoons[orbitAroundSelected]++;
                moonsGenerated++;
                if (moonsTelluricGenerated < nbOfMoonsTelluric) {
                    Patch.Debug("That was a Telluric Planet Moon  ", LogLevel.Debug, Patch.DebugStarGen);

                    moonsTelluricGenerated++;
                }
                else if (moonsGasGenerated < nbOfMoonsGasGiant) {
                    Patch.Debug("That was a Gas Giant Moon  ", LogLevel.Debug, Patch.DebugStarGen);
                    moonsGasGenerated++;
                }
                
         
            }
            
            //Singularities
            
            for (var i = 0; i < planetMoons.Length; i++) {
                if (planetMoons[i] > 1) {
                    star.planets[i].HasMultipleSatellites();
                }
            }
        }
    }
}