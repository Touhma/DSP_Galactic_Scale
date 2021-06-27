using System;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static GalaxyData ProcessGalaxy(GameDesc desc, bool sketchOnly = false)
        {
            Log($"Start ProcessGalaxy:{sketchOnly} StarCount:{gameDesc.starCount} Seed:{gameDesc.galaxySeed} Called By{GetCaller()}");
            Random random = new Random(GSSettings.Seed);
            try
            {
                gameDesc = desc;
                Log($"Generating Galaxy of {GSSettings.StarCount}|{gameDesc.starCount} stars");
                Failed = false;
                PatchOnUIGalaxySelect.StartButton?.SetActive(true);
                if (!GSSettings.Instance.imported && sketchOnly)
                {
                    //Warn("Start");
                    GSSettings.Reset(gameDesc.galaxySeed);

                    Log("Seed From gameDesc = " + GSSettings.Seed);
                    gsPlanets.Clear();
                    gsStars.Clear();
                    //Warn("Cleared");
                    Warn("Loading Data from Generator : " + generator.Name);
                    generator.Generate(gameDesc.starCount);
                    Warn("Final Seed = " + GSSettings.Seed);
                    //Log("End");
                }
                else
                {
                    Log("Settings Loaded From Save File");
                }

                Log($"Galaxy of GSSettings:{GSSettings.StarCount} stars Generated... or is it gameDesc :{gameDesc.starCount}");
                gameDesc.starCount = GSSettings.StarCount;
                //Log("Processing ThemeLibrary");
                if (GSSettings.ThemeLibrary == null || GSSettings.ThemeLibrary == new ThemeLibrary())
                {
                    GSSettings.ThemeLibrary = ThemeLibrary;
                }
                else
                {
                    ThemeLibrary = GSSettings.ThemeLibrary;
                }

                Log("Generating TempPoses");
                int tempPoses = StarPositions.GenerateTempPoses(
                    random.Next(),
                    GSSettings.StarCount,
                    GSSettings.GalaxyParams.iterations,
                    GSSettings.GalaxyParams.minDistance,
                    GSSettings.GalaxyParams.minStepLength,
                    GSSettings.GalaxyParams.maxStepLength,
                    GSSettings.GalaxyParams.flatten
                    );
                Log("Creating new GalaxyData");
                galaxy = new GalaxyData();
                galaxy.seed = GSSettings.Seed;
                galaxy.starCount = GSSettings.StarCount;
                galaxy.stars = new StarData[GSSettings.StarCount];
                if (GSSettings.StarCount <= 0)
                {
                    Log("StarCount <= 0, returning galaxy");
                    return galaxy;
                }
                Log("Initializing AstroPoses");
                InitializeAstroPoses(random);
                Log("AstroPoses Initialized");
                //SetupBirthPlanet();
                galaxy.birthPlanetId = GSSettings.BirthPlanetId;
                galaxy.birthStarId = GSSettings.BirthStarId;
                //if (createPlanets) {
                Log("Setting up Birth Planet");
                //SetupBirthPlanet();
                Log("Generating Veins");
                GenerateVeins(sketchOnly);
                //if (GS2.CheatMode) return galaxy;
                //}
                Log("Creating Galaxy StarGraph");
                UniverseGen.CreateGalaxyStarGraph(galaxy);
                //Log("End of galaxy generation");
                Log($"Galaxy Created. birthStarid:{galaxy.birthStarId}");
                Log($"birthPlanetId:{galaxy.birthPlanetId}");
                Log($"birthStarName: {galaxy.stars[galaxy.birthStarId - 1].name} Radius:{galaxy.PlanetById(galaxy.birthPlanetId).radius} Scale:{galaxy.PlanetById(galaxy.birthPlanetId).scale}");
                Log($"its planets length: {galaxy.stars[galaxy.birthStarId - 1].planets.Length}");
                Log($"First System Radius = {galaxy.stars[0].systemRadius}");
                return galaxy;
            }
            catch (Exception e)
            {
                GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/start-button").gameObject.SetActive(false);
                GS2.Log(e.ToString());
                GS2.DumpException(e);
                UIMessageBox.Show("Error", "There has been a problem creating the galaxy. \nPlease let the Galactic Scale team know in our discord server. An error log has been generated in the plugin/ErrorLog Directory", "Return", 0);
                UIRoot.instance.OnGameLoadFailed();
                return null;
            }
        }
       
        public static void GenerateVeins(bool SketchOnly)
        {
            for (int i = 1; i < galaxy.starCount; ++i)
            {
                StarData star = galaxy.stars[i];
                for (int j = 0; j < star.planetCount; ++j)
                {
                    PlanetModelingManager.Algorithm(star.planets[j]).GenerateVeins(SketchOnly);
                }
            }
        }
        public static void InitializeAstroPoses(GS2.Random random)
        {
            var gSize = galaxy.starCount * 4000;
            galaxy.astroPoses = new AstroPose[gSize];
            //Log("Creating Stars");
            for (var i = 0; i < GSSettings.StarCount; i++)
            {
                galaxy.stars[i] = CreateStar(i, random);
            }
            //for (var i = 0; i < galaxy.stars.Length; i++) GS2.Warn($"Star {galaxy.stars[i].index} id:{galaxy.stars[i].id} name:{galaxy.stars[i].name} GSSettings:{GSSettings.Stars[i].Name}");
            //Log("Creating Planets");
            for (var i = 0; i < GSSettings.StarCount; i++)
            {
                CreateStarPlanets(ref galaxy.stars[i], gameDesc, random);
            }

            //Log("Planets have been created");
            AstroPose[] astroPoses = galaxy.astroPoses;
            for (int index = 0; index < galaxy.astroPoses.Length; ++index)
            {
                astroPoses[index].uRot.w = 1f;
                astroPoses[index].uRotNext.w = 1f;
            }
            for (int index = 0; index < GSSettings.StarCount; ++index)
            {
                astroPoses[galaxy.stars[index].id * 100].uPos = astroPoses[galaxy.stars[index].id * 100].uPosNext = galaxy.stars[index].uPosition;
                astroPoses[galaxy.stars[index].id * 100].uRot = astroPoses[galaxy.stars[index].id * 100].uRotNext = Quaternion.identity;
                astroPoses[galaxy.stars[index].id * 100].uRadius = galaxy.stars[index].physicsRadius;
            }
            //Log("Updating Poses");
            galaxy.UpdatePoses(0.0);
            //Log("End");
        }
    }
}