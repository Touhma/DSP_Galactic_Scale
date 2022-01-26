using NebulaAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static GalaxyData ProcessGalaxy(GameDesc desc, bool sketchOnly = false)
        {
            Log($"Start ProcessGalaxy:{sketchOnly} StarCount:{gameDesc.starCount} Seed:{gameDesc.galaxySeed} Called By{GetCaller()}. Galaxy StarCount : {galaxy?.stars?.Length}");
            var random = new Random(GSSettings.Seed);
            try
            {
                if (Config.ForceRare) GSSettings.GalaxyParams.forceSpecials = true;
                var highStopwatch = new HighStopwatch();
                highStopwatch.Begin();
                gameDesc = desc;
                Log($"Generating Galaxy of {GSSettings.StarCount}|{gameDesc.starCount} stars");
                Failed = false;
                PatchOnUIGalaxySelect.StartButton?.SetActive(true);
                if (!GSSettings.Instance.imported && sketchOnly)
                {
                    Log("Start");
                    GSSettings.Reset(gameDesc.galaxySeed);
                    Warn(LDB._themes.dataArray.Length.ToString());
                    if (LDB._themes.dataArray != null && LDB._themes.dataArray.Length > 128) Array.Resize(ref LDB._themes.dataArray, 128);
                    Warn(LDB._themes.dataArray.Length.ToString());
                    // GS2.LogJson(gameDesc);
                    // GS2.Warn(gameDesc.resourceMultiplier.ToString());
                    // GS2.Warn(GSSettings.Instance.galaxyParams.resourceMulti.ToString());
                    Log("Seed From gameDesc = " + GSSettings.Seed);
                    gsPlanets.Clear();
                    gsStars.Clear();

                    //Warn("Cleared");
                    Log("Loading Data from Generator : " + ActiveGenerator.Name);
                    ActiveGenerator.Generate(gameDesc.starCount);
                    GSSettings.Instance.galaxyParams.resourceMulti = gameDesc.resourceMultiplier;
                    GSSettings.Instance.generatorGUID = ActiveGenerator.GUID;
                    Log("Final Seed = " + GSSettings.Seed);
                    //Log("End");
                    // WarnJson(GSSettings.ThemeLibrary.Select(x=>x.Key).ToList());
                }
                else
                {
                    Log("Settings Loaded From Save File");
                    gameDesc.resourceMultiplier = GSSettings.Instance.galaxyParams.resourceMulti;
                    // Log($"RM1:{gameDesc.resourceMultiplier}");
                    // Log(gameDesc.resourceMultiplier.ToString());
                }

                Log($"Galaxy Loaded: {highStopwatch.duration:F5}");
                highStopwatch.Begin();
                Log($"Galaxy of GSSettings:{GSSettings.StarCount} stars Generated... or is it gameDesc :{gameDesc.starCount}");
                gameDesc.starCount = GSSettings.StarCount;
                //Log("Processing ThemeLibrary");
                // if (GSSettings.ThemeLibrary == null || GSSettings.ThemeLibrary == new ThemeLibrary())
                //     GSSettings.ThemeLibrary = ThemeLibrary.Vanilla();
                // else
                //     ThemeLibrary = GSSettings.ThemeLibrary;

                Log("Generating TempPoses");
                var tempPoses = StarPositions.GenerateTempPoses(random.Next(), GSSettings.StarCount, GSSettings.GalaxyParams.iterations, GSSettings.GalaxyParams.minDistance, GSSettings.GalaxyParams.minStepLength, GSSettings.GalaxyParams.maxStepLength, GSSettings.GalaxyParams.flatten);
                Log($"TempPoses Generated: {highStopwatch.duration:F5}");
                highStopwatch.Begin();

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
                CreateStarPlanetsAstroPoses(random);
                Log($"Astroposes Initialized: {highStopwatch.duration:F5}");
                highStopwatch.Begin();

                // Log("AstroPoses Initialized");
                //SetupBirthPlanet();
                galaxy.birthPlanetId = GSSettings.BirthPlanetId;
                galaxy.birthStarId = GSSettings.BirthStarId;
                //if (createPlanets) {
                var birthStar = galaxy.StarById(galaxy.birthStarId);
                for (var i = 0; i < galaxy.starCount && galaxy.starCount > 1; i++)
                {
                    var star = galaxy.stars[i];
                    var l = star.level;
                    star.level = Mathf.Abs(star.index - birthStar.index) / (float)(galaxy.starCount - 1) * 2f;
                    var num1 = (float)(star.position - birthStar.position).magnitude / 32f;
                    if (num1 > 1.0)
                        num1 = Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(num1) + 1f) + 1f) + 1f) + 1f) + 1f;
                    var rc = Mathf.Pow(7f, num1) * 0.6f;
                    //Warn($"Beta 68 Test:  Changed level of {star.name, 12} from {l, 12} to {star.level, 12} resource coef:{star.resourceCoef, 12} to {rc, 12} magnitude/32:{num1, 12}");
                    star.resourceCoef = rc;
                }

                Log($"Resource Coefficients Set: {highStopwatch.duration:F5}");
                highStopwatch.Begin();

                Log("Setting up Birth Planet");
                //SetupBirthPlanet();
                Log("Generating Veins");
                GenerateVeins(sketchOnly);
                Log($"Veins Generated: {highStopwatch.duration:F5}");
                highStopwatch.Begin();

                //if (GS2.CheatMode) return galaxy;
                //}
                Log("Creating Galaxy StarGraph");
                UniverseGen.CreateGalaxyStarGraph(galaxy);
                Log($"Stargraph Generated: {highStopwatch.duration:F5}");
                highStopwatch.Begin();

                //Log("End of galaxy generation");
                Log($"Galaxy Created. birthStarid:{galaxy.birthStarId}");
                Log($"birthPlanetId:{galaxy.birthPlanetId}");
                Warn($"birthPlanet:{galaxy.PlanetById(galaxy.birthPlanetId).name}");
                Log($"birthStarName: {galaxy.stars[galaxy.birthStarId - 1].name} Radius:{galaxy.PlanetById(galaxy.birthPlanetId).radius} Scale:{galaxy.PlanetById(galaxy.birthPlanetId).scale}");
                Log($"its planets length: {galaxy.stars[galaxy.birthStarId - 1].planets.Length}");
                Log($"First System Radius = {galaxy.stars[0].systemRadius}");
                // foreach (var star in GSSettings.Stars)
                // {
                //     foreach (var planet in star.Planets)
                //     {
                //         GS2.Warn($"Theme used:{planet.GsTheme.Name}");
                //     }
                // }
                Warn("RM:" + GSSettings.Instance.galaxyParams.resourceMulti);
                Warn($"GUID:{GSSettings.Instance.generatorGUID}");
                // WarnJson(GSSettings.ThemeLibrary.Select(x=>x.Key).ToList());
                return galaxy;
            }
            catch (Exception e)
            {
                GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/start-button").gameObject.SetActive(false);
                Log(e.ToString());
                DumpException(e);
                UIMessageBox.Show("Error", "There has been a problem creating the galaxy. \nPlease let the Galactic Scale team know in our discord server. An error log has been generated in the plugin/ErrorLog Directory", "Return", 0);
                UIRoot.instance.OnGameLoadFailed();
                return null;
            }
        }

        public static void GenerateVeins(bool SketchOnly)
        {
            for (var i = 1; i < galaxy.starCount; ++i)
            {
                var star = galaxy.stars[i];
                for (var j = 0; j < star.planetCount; ++j)
                    PlanetModelingManager.Algorithm(star.planets[j]).GenerateVeins(SketchOnly);
            }
        }
    }
}