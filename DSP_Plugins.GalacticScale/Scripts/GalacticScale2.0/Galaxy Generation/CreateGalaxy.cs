using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static GalaxyData CreateGalaxy(GameDesc desc, bool createPlanets = true)
        {
            Log("Start");
            gameDesc = desc;

            Log("Generating Galaxy");
            Failed = false;
            if (!GSSettings.Instance.imported)
            {
                GSSettings.Reset(gameDesc.galaxySeed);
                Log("Seed From gameDesc = " + GSSettings.Seed);
                gsPlanets.Clear();
                Log("Loading Data from Generator : " + generator.Name);
                generator.Generate(gameDesc.starCount);
                Log("Final Seed = " + GSSettings.Seed);
                Log("End");
            }
            else Log("Settings Loaded From Save File");

            Log("Galaxy Generated");
            gameDesc.starCount = GSSettings.starCount;
            Log("Processing ThemeLibrary");
            if (GSSettings.ThemeLibrary == null || GSSettings.ThemeLibrary == new ThemeLibrary()) GSSettings.ThemeLibrary = ThemeLibrary;
            else ThemeLibrary = GSSettings.ThemeLibrary;
            Log("Generating TempPoses");
            int tempPoses = GenerateTempPoses(
                random.Next(),
                GSSettings.starCount,
                GSSettings.GalaxyParams.iterations,
                GSSettings.GalaxyParams.minDistance,
                GSSettings.GalaxyParams.minStepLength,
                GSSettings.GalaxyParams.maxStepLength,
                GSSettings.GalaxyParams.flatten
                );
            Log("Creating new GalaxyData");
            galaxy = new GalaxyData();
            galaxy.seed = GSSettings.Seed;
            galaxy.starCount = GSSettings.starCount;
            galaxy.stars = new StarData[GSSettings.starCount];
            if (GSSettings.starCount <= 0) {
                Log("StarCount <= 0, returning galaxy");
                return galaxy;
            }
            Log("Initializing AstroPoses");
            InitializeAstroPoses();
            Log("AstroPoses Initialized");
            if (createPlanets)
            {
                Log("Setting up Birth Planet");
                SetupBirthPlanet();
                Log("Generating Veins");
                GenerateVeins();
            }
            Log("Creating Galaxy StarGraph");
            UniverseGen.CreateGalaxyStarGraph(galaxy);
            Log("End of galaxy generation");
            return galaxy;
        }
        public static void SetupBirthPlanet() {
            if (galaxy.starCount <= 0) return;
            if (GSSettings.birthPlanetId >= 0)
            {
                galaxy.birthPlanetId = galaxy.stars[GSSettings.birthStarId].planets[GSSettings.birthPlanetId].id;
                galaxy.birthStarId = galaxy.stars[GSSettings.birthStarId].id;
            }
            else
            {
                for (int i = 0; i < GSSettings.starCount; i++)
                {
                    GSStar star = GSSettings.Stars[i];
                    List<GSPlanet> bodies = star.Bodies;
                    for (int j = 0; j < star.bodyCount; j++)
                    {
                        GSPlanet planet = bodies[j];
                        if (ThemeLibrary[planet.Theme].PlanetType == EPlanetType.Ocean)
                        {
                            galaxy.birthPlanetId = planet.planetData.id;
                            galaxy.birthStarId = planet.planetData.star.id;
                            GSSettings.birthPlanetId = j;
                            GSSettings.birthStarId = i;
                            i = j = 9001;
                        }
                    }
                }
            }  
            Assert.Positive(galaxy.birthPlanetId);
        }
        public static void GenerateVeins()
        {
            for (int i = 1; i < galaxy.starCount; ++i)
            {
                StarData star = galaxy.stars[i];
                for (int j = 0; j < star.planetCount; ++j) PlanetModelingManager.Algorithm(star.planets[j]).GenerateVeins(true);
            }
        }
        public static void InitializeAstroPoses()
        {
            var gSize = galaxy.starCount > 64 ? galaxy.starCount * 4 * 100 : 25600;
            galaxy.astroPoses = new AstroPose[gSize];
            Log("Creating Stars");
            for (var i = 0; i < GSSettings.starCount; i++) galaxy.stars[i] = CreateStar(i);
            Log("Creating Planets");
            for (var i = 0; i < GSSettings.starCount; i++) CreateStarPlanets(ref galaxy.stars[i], gameDesc);
            Log("Planets have been created");
            AstroPose[] astroPoses = galaxy.astroPoses;
            for (int index = 0; index < galaxy.astroPoses.Length; ++index)
            {
                astroPoses[index].uRot.w = 1f;
                astroPoses[index].uRotNext.w = 1f;
            }
            for (int index = 0; index < GSSettings.starCount; ++index)
            {
                astroPoses[galaxy.stars[index].id * 100].uPos = astroPoses[galaxy.stars[index].id * 100].uPosNext = galaxy.stars[index].uPosition;
                astroPoses[galaxy.stars[index].id * 100].uRot = astroPoses[galaxy.stars[index].id * 100].uRotNext = Quaternion.identity;
                astroPoses[galaxy.stars[index].id * 100].uRadius = galaxy.stars[index].physicsRadius;
            }
            Log("Updating Poses");
            galaxy.UpdatePoses(0.0);
            Log("End");
        }
    }
}