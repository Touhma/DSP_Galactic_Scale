using System;
using System.IO;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        // Length of LDB._themes before GS2 ever appended a theme proto: vanilla themes plus
        // anything other mods registered at load. Captured on the first ProcessGalaxy call and
        // used as the reset point for the theme table between generations, replacing the old
        // hardcoded 128 (which silently truncated real themes once vanilla+mods crossed it, and
        // left stale dataIndices entries pointing past the shortened array).
        private static int builtinThemeCount = -1;

        private static void ResetThemeProtoSet()
        {
            var themes = LDB._themes;
            if (themes?.dataArray == null || builtinThemeCount < 0 || themes.dataArray.Length <= builtinThemeCount) return;
            var staleIds = new System.Collections.Generic.List<int>();
            foreach (var kv in themes.dataIndices)
                if (kv.Value >= builtinThemeCount)
                    staleIds.Add(kv.Key);
            foreach (var id in staleIds) themes.dataIndices.Remove(id);
            Array.Resize(ref themes.dataArray, builtinThemeCount);
        }

        public static GalaxyData ProcessGalaxy(GameDesc desc, bool sketchOnly = false)
        {
            Log($"Start ProcessGalaxy:{sketchOnly} StarCount:{gameDesc.starCount} Seed:{gameDesc.galaxySeed} Called By{GetCaller()}. Galaxy StarCount : {galaxy?.stars?.Length}");
            if (builtinThemeCount < 0 && LDB._themes?.dataArray != null)
            {
                builtinThemeCount = LDB._themes.dataArray.Length;
                Log($"Captured builtin theme table size: {builtinThemeCount}");
            }
            var random = new Random(GSSettings.Seed);
            try
            {
                if (Config.ForceRare) GSSettings.GalaxyParams.forceSpecials = true;
                var highStopwatch = new HighStopwatch();
                highStopwatch.Begin();
                gameDesc = desc;
                Log($"Generating Galaxy of {GSSettings.StarCount}|{gameDesc.starCount} stars");
                // Warn($"GSSettings.BirthPlanet.Name:{GSSettings.BirthPlanet?.Name} ID:{GSSettings.BirthPlanetId}");
                Failed = false;
                PatchOnUIGalaxySelect.StartButton?.SetActive(true);
                if (!GSSettings.Instance.imported && sketchOnly)
                {
                    // Log("Start");
                    GSSettings.Reset(gameDesc.galaxySeed);
                    // Warn(LDB._themes.dataArray.Length.ToString());
                    ResetThemeProtoSet();
                    // Warn(LDB._themes.dataArray.Length.ToString());
                    // GS2.LogJson(gameDesc);
                    // GS2.Warn(gameDesc.resourceMultiplier.ToString());
                    // GS2.Warn(GSSettings.Instance.galaxyParams.resourceMulti.ToString());
                    Log("Seed From gameDesc = " + GSSettings.Seed);
                    gsPlanets.Clear();
                    gsStars.Clear();

                    // Warn("Cleared");
                    Warn("Loading Data from Generator : " + ActiveGenerator.Name);
                    ActiveGenerator.Generate(gameDesc.starCount);
                    GSSettings.Instance.galaxyParams.resourceMulti = gameDesc.resourceMultiplier;
                    GSSettings.Instance.generatorGUID = ActiveGenerator.GUID;
                    // Log("Final Seed = " + GSSettings.Seed);
                    // Log("End");
                    // WarnJson(GSSettings.ThemeLibrary.Select(x=>x.Key).ToList());
                }
                else
                {
                    // Warn("**************************");
                    Log($"Settings Loaded From Save File {GSSettings.BirthPlanet.Name} {GSSettings.Instance.stars.Count} {GSSettings.StarCount}");
                    gameDesc.resourceMultiplier = GSSettings.Instance.galaxyParams.resourceMulti;
                    // Log($"RM1:{gameDesc.resourceMultiplier}");
                    
                }
                LogJson(gameDesc.combatSettings);
                Log($"Galaxy Loaded: {highStopwatch.duration:F5}");
                highStopwatch.Begin();
                // Log($"Galaxy of GSSettings:{GSSettings.StarCount} stars Generated... or is it gameDesc :{gameDesc.starCount}");
                gameDesc.starCount = GSSettings.StarCount;
                var tempPoses = StarPositions.GenerateTempPoses(random.Next(), GSSettings.StarCount, GSSettings.GalaxyParams.iterations, GSSettings.GalaxyParams.minDistance, GSSettings.GalaxyParams.minStepLength, GSSettings.GalaxyParams.maxStepLength, GSSettings.GalaxyParams.flatten);
                // Log($"TempPoses Generated: {highStopwatch.duration:F5}");
                highStopwatch.Begin();

                // Log("Creating new GalaxyData");
                galaxy = new GalaxyData();
                galaxy.seed = GSSettings.Seed;
                galaxy.starCount = GSSettings.StarCount;
                galaxy.stars = new StarData[GSSettings.StarCount];
                if (GSSettings.StarCount <= 0)
                {
                    Log("StarCount <= 0, returning galaxy");
                    return galaxy;
                }

                // Log("Initializing AstroPoses");
                CreateStarPlanetsAstroPoses(random);
                var bs = galaxy.stars[galaxy.birthStarId - 1];
                // Log($"{bs.name} - {bs.initialHiveCount}/{bs.maxHiveCount}");
                Log($"Astroposes Initialized: {highStopwatch.duration:F5}");
                highStopwatch.Begin();

                // Log("AstroPoses Initialized");
                //SetupBirthPlanet();
                Warn($"Setting up birthPlanet {GSSettings.BirthPlanetId}");
                galaxy.birthPlanetId = GSSettings.BirthPlanetId;
                galaxy.birthStarId = GSSettings.BirthStarId;
                
                // Log($"{bs.name} - {bs.initialHiveCount}/{bs.maxHiveCount}");
                //if (createPlanets) {
                var birthStar = galaxy.StarById(galaxy.birthStarId);
                AssignStarLevels(GSSettings.BirthStar);
                for (var i = 0; i < galaxy.starCount && galaxy.starCount > 1; i++)
                {
                    var star = galaxy.stars[i];
                    // star.level = Mathf.Abs(star.index - birthStar.index) / (float)(galaxy.starCount - 1) * 2f;
                    var num1 = (float)(star.position - birthStar.position).magnitude / 32f;
                    if (num1 > 1.0)
                        num1 = Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(num1) + 1f) + 1f) + 1f) + 1f) + 1f;
                    var rc = Mathf.Pow(7f, num1) * 0.6f;
                    star.resourceCoef = rc;
                }

                Log($"Resource Coefficients Set: {highStopwatch.duration:F5}");
                highStopwatch.Begin();
                UniverseGen.CreateGalaxyStarGraph(galaxy);
                // Log($"{bs.name} - {bs.initialHiveCount}/{bs.maxHiveCount}");
                Log($"Stargraph Generated: {highStopwatch.duration:F5}");
                highStopwatch.Begin();

                //Log("End of galaxy generation");
                Log($"Galaxy Created. birthStarid:{galaxy.birthStarId}");
                Log($"birthPlanetId:{galaxy.birthPlanetId}");
                Log($"birthPlanet:{galaxy.PlanetById(galaxy.birthPlanetId).name}");
                Log($"birthStarName: {galaxy.stars[galaxy.birthStarId - 1].name} Radius:{galaxy.PlanetById(galaxy.birthPlanetId).radius} Scale:{galaxy.PlanetById(galaxy.birthPlanetId).scale}");
                if (Config.Dev) DumpObjectToJson(Path.Combine(DataDir, "ldbthemesPost.json"), LDB._themes.dataArray);
                Log("Galaxy Generated");
                Log($"{bs.name} - {bs.initialHiveCount}/{bs.maxHiveCount}");
                if (GSSettings.Instance.Preferences == null) GSSettings.Instance.Preferences = GS2.Preferences;
                return galaxy;
            }
            catch (Exception e)
            {
                GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/start-button").gameObject.SetActive(false);
                Log(e.ToString());
                Log(GetCaller());
                Log(GetCaller(1));
                // DumpException(e);
                UIMessageBox.Show("Error", "There has been a problem creating the galaxy. \nPlease let the Galactic Scale team know in our discord server. An error log has been generated in the plugin/ErrorLog Directory", "Return", 0);
                UIRoot.instance.OnGameLoadFailed();
                return null;
            }
        }
        public static void AssignStarLevels(GSStar birthStar)
        {
            birthStar.level = 0;
            var maxDistance = 0f;
            foreach (GSStar s in GSSettings.Stars)
            {
                float m = (float)s.position.magnitude - (float)birthStar.position.magnitude;
                if (m > maxDistance) maxDistance = m;
            }
            foreach (GSStar s in GSSettings.Stars)
            {
                var m = (float)s.position.magnitude- (float)birthStar.position.magnitude;;
                s.level = m / maxDistance;
            }
        }
        public static void GenerateVeins(bool SketchOnly)
        {
            for (var i = 1; i < galaxy.starCount; ++i)
            {
                var star = galaxy.stars[i];
                for (var j = 0; j < star.planetCount; ++j)
                    PlanetModelingManager.Algorithm(star.planets[j]).GenerateVeins();
            }
        }
    }
}