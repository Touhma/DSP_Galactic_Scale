using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static GalaxyData ProcessGalaxy(GameDesc desc, bool createPlanets = true)
        {
            Log($"Start CreatePlanets:{createPlanets}");

            try
            {
                random = new Random(GSSettings.Seed);
                gameDesc = desc;
                Log("Generating Galaxy");
                Failed = false;
                PatchOnUIGalaxySelect.StartButton?.SetActive(true);
                if (!GSSettings.Instance.imported)
                {
                    Warn("Start");
                    GSSettings.Reset(gameDesc.galaxySeed);
                    Log("Seed From gameDesc = " + GSSettings.Seed);
                    gsPlanets.Clear();
                    gsStars.Clear();
                    Warn("Cleared");
                    Warn("Loading Data from Generator : " + generator.Name);
                    generator.Generate(gameDesc.starCount);
                    Warn("Final Seed = " + GSSettings.Seed);
                    Log("End");
                }
                else Log("Settings Loaded From Save File");

                Log("Galaxy Generated");
                gameDesc.starCount = GSSettings.StarCount;
                Log("Processing ThemeLibrary");
                if (GSSettings.ThemeLibrary == null || GSSettings.ThemeLibrary == new ThemeLibrary()) GSSettings.ThemeLibrary = ThemeLibrary;
                else ThemeLibrary = GSSettings.ThemeLibrary;
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
                InitializeAstroPoses();
                Log("AstroPoses Initialized");
                //SetupBirthPlanet();
                galaxy.birthPlanetId = GSSettings.BirthPlanetId;
                galaxy.birthStarId = GSSettings.BirthStarId;
                if (createPlanets)
                {
                    Log("Setting up Birth Planet");
                    //SetupBirthPlanet();
                    Log("Generating Veins");
                    GenerateVeins();
                    //if (GS2.CheatMode) return galaxy;
                }
                Log("Creating Galaxy StarGraph");
                UniverseGen.CreateGalaxyStarGraph(galaxy);
                Log("End of galaxy generation");
                Warn($"Galaxy Created. birthStarid:{galaxy.birthStarId}");
                Warn($"birthPlanetId:{galaxy.birthPlanetId}");
                Warn($"birthStarName: {galaxy.stars[galaxy.birthStarId - 1].name}");
                Warn($"its planets length: {galaxy.stars[galaxy.birthStarId -1].planets.Length}");
                Warn($"First System Radius = {galaxy.stars[0].systemRadius}");
                return galaxy;
            } catch (Exception e)
            {
                GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/start-button").gameObject.SetActive(false);
                GS2.Log("EXCEPT");
                GS2.Log(e.ToString());
                GS2.DumpException(e);
                UIMessageBox.Show("Error", "There has been a problem creating the galaxy. \nPlease let the Galactic Scale team know in our discord server. An error log has been generated in the plugin/ErrorLog Directory", "Return", 0);
                UIRoot.instance.OnGameLoadFailed();
                return null;
            }
        }
        //public static void SetupBirthPlanet() {
        //    Log("Start");
        //    if (galaxy.starCount <= 0) return;
        //    if (GSSettings.Instance.BirthPlanetName != null && GSSettings.Instance.BirthPlanetName != "")
        //    {
        //        Warn("BirthPlanetName Found");
        //        GSPlanet BirthPlanet = GetGSPlanet(GSSettings.Instance.BirthPlanetName);
                
        //        if (BirthPlanet != null)
        //        {
        //            Log(BirthPlanet.ToString());
        //            Log("Found BirthPlanet, Adding ID's");
                    
        //            GSSettings.BirthPlanetId = galaxy.birthPlanetId = BirthPlanet.planetData.id;
        //            galaxy.birthStarId = BirthPlanet.planetData.star.id;
        //            Warn("Birth Star Name = " + galaxy.stars[GSSettings.BirthStarId]);
        //            return;
        //        }
        //        Warn("BirthPlanet Name Not Found In Planet List!");
        //    }
        //    if (GSSettings.BirthPlanetId >= 0)
        //    {
        //        Warn("Set BirthPlanet by it's ID being > 0: "+GSSettings.birthPlanetId + " of " + galaxy.stars.Length);
        //        Warn($"Set BirthStar ID :{GSSettings.BirthStarId -1 } of { galaxy.stars.Length}");
        //        //galaxy.birthPlanetId = galaxy.stars[GSSettings.birthStarId].planets[GSSettings.birthPlanetId].id;
                
        //        //galaxy.birthStarId = galaxy.stars[GSSettings.birthStarId].id;
        //        GSPlanet BirthPlanet = GetGSPlanet(GSSettings.birthPlanetId);
        //        galaxy.birthPlanetId = BirthPlanet.planetData.id;
        //        galaxy.birthStarId = BirthPlanet.planetData.star.id;
        //    }
        //    else
        //    {
        //        Warn("Trying to find a birth planet via iteration");
        //        for (int i = 0; i < GSSettings.StarCount; i++)
        //        {
        //            GSStar star = GSSettings.Stars[i];
        //            List<GSPlanet> bodies = star.Bodies;
        //            for (int j = 0; j < star.bodyCount; j++)
        //            {
        //                GSPlanet planet = bodies[j];
        //                if (ThemeLibrary[planet.Theme].PlanetType == EPlanetType.Ocean)
        //                {
        //                    GS2.Log("Found a birth planet: "+planet.Name);
        //                    GSSettings.birthPlanetId =galaxy.birthPlanetId = planet.planetData.id;
        //                    galaxy.birthStarId = planet.planetData.star.id;
        //                    i = j = 9001;
        //                }
        //            }
        //        }
        //    }  
        //    Assert.Positive(galaxy.birthPlanetId);
        //}
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
            var gSize = galaxy.starCount * 4000;
            galaxy.astroPoses = new AstroPose[gSize];
            Log("Creating Stars");
            for (var i = 0; i < GSSettings.StarCount; i++) galaxy.stars[i] = CreateStar(i);
            Log("Creating Planets");
            for (var i = 0; i < GSSettings.StarCount; i++) CreateStarPlanets(ref galaxy.stars[i], gameDesc);
            Log("Planets have been created");
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
            Log("Updating Poses");
            galaxy.UpdatePoses(0.0);
            Log("End");
        }
    }
}