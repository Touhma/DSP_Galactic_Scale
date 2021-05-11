using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.Bootstrap;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static GalaxyData CreateGalaxy(GameDesc desc, bool createPlanets = true)
        {
            gameDesc = desc;

            Log("CreateGalaxy:GenerateGalaxy()");
            GenerateGalaxy();
            Log("Galaxy Generated");
            gameDesc = desc;
            gameDesc.starCount = GSSettings.starCount;
            if (GSSettings.ThemeLibrary == null || GSSettings.ThemeLibrary == new GSThemeLibrary())
            {
                GS2.Log("ThemeLibrary in settings was null. Updating");
                GSSettings.ThemeLibrary = ThemeLibrary;
            }
            else GS2.ThemeLibrary = GSSettings.ThemeLibrary;
            int tempPoses = GenerateTempPoses(
                random.Next(),
                GSSettings.starCount,
                GSSettings.GalaxyParams.iterations,
                GSSettings.GalaxyParams.minDistance,
                GSSettings.GalaxyParams.minStepLength,
                GSSettings.GalaxyParams.maxStepLength,
                GSSettings.GalaxyParams.flatten
                );
            galaxy = new GalaxyData();
            galaxy.seed = GSSettings.Seed;
            galaxy.starCount = GSSettings.starCount;
            galaxy.stars = new StarData[GSSettings.starCount];
            //Log("Star Count = " + GSSettings.starCount);
            if (GSSettings.starCount <= 0) return galaxy;
            var gSize = galaxy.starCount > 64 ? galaxy.starCount * 4 * 100 : 25600;
            galaxy.astroPoses = new AstroPose[gSize];
            for (var i = 0; i < GSSettings.starCount; i++) galaxy.stars[i] = CreateStar(i);
            for (var i = 0; i < GSSettings.starCount; i++) CreateStarPlanets(ref galaxy.stars[i], gameDesc);
            //Log("Initialize Astroposes");
            InitializeAstroPoses();
            //galaxy.birthPlanetId = 1;
            if (createPlanets)
            {
                //Log("Creating Planets");
                SetupBirthPlanet();
                Log("Generating Veins "+galaxy.birthPlanetId);
                //SelectBirthPlanet();
                GenerateVeins();
            }
            
            UniverseGen.CreateGalaxyStarGraph(galaxy);
            return galaxy;

        }
        public static void SelectBirthPlanet ()
        {
            int id = -1;
            int i = -1;
            //Log("Start While Loop");
            while (id == -1 && i < galaxy.starCount) {
                i++; 
                List<PlanetData> habitablePlanets = GetHabitablePlanets(galaxy.stars[i]);
                //Log("i=" + i);
                if (habitablePlanets.Count > 0)
                {
                    int choice = random.Next(0, habitablePlanets.Count);
                    id = habitablePlanets[choice].id;
                }
                
            }
            //Log("EndWhile" + id + " " + galaxy.stars[i].id);
            galaxy.birthPlanetId = id;
            galaxy.birthStarId = galaxy.stars[i].id;
        }

        public static List<PlanetData> GetHabitablePlanets(StarData star)
        {
            List<PlanetData> list = new List<PlanetData>();
            foreach (PlanetData p in star.planets) if (LDB.themes.Select(p.theme).Distribute == EThemeDistribute.Birth) list.Add(p);
            return list;
        }
        public static void SetupBirthPlanet() {
            if (galaxy.starCount <= 0) return;
            if (GSSettings.birthPlanetId >= 0)
            {
                GS2.Log("Using BirthPlanet from Settings");
                galaxy.birthPlanetId = galaxy.stars[GSSettings.birthStarId].planets[GSSettings.birthPlanetId].id;
                galaxy.birthStarId = galaxy.stars[GSSettings.birthStarId].id;
            }
            else
            {
                for (int i = 0; i < GSSettings.starCount; i++)
                {
                    GSStar star = GSSettings.Stars[i];
                    List<GSPlanet> bodies = star.bodies;
                    for (int j = 0; j < star.bodyCount; j++)
                    {
                        GSPlanet planet = bodies[j];
                        if (GS2.ThemeLibrary[planet.Theme].PlanetType == EPlanetType.Ocean)
                        {
                            galaxy.birthPlanetId = planet.planetData.id;
                            galaxy.birthStarId = planet.planetData.star.id;
                            j = 9001;
                            i = 9001;
                        }
                    }
                }
            }
                //{
                //    StarData starData = galaxy.stars[0];
                //    for (int p = 0; p < starData.planetCount; p++)
                //    {
                //        //Log("Setting BirthPlanet");
                //        PlanetData planet = starData.planets[p];
                //        ThemeProto themeProto = LDB.themes.Select(planet.theme);
                //        if (themeProto != null && themeProto.Distribute == EThemeDistribute.Birth)
                //        {
                //            galaxy.birthPlanetId = planet.id;
                //            galaxy.birthStarId = starData.id;
                //            break;
                //        }
                //        else
                //        {
                //            Patch.Debug("FAILED TO SET BIRTH PLANET!", BepInEx.Logging.LogLevel.Warning, true);
                //            galaxy.birthPlanetId = galaxy.stars[0].planets[0].id;
                //            galaxy.birthStarId = galaxy.stars[0].id;
                //            //DumpObjectToJson(Path.Combine(DataDir, "error.json"), starData.planets);
                //        }
                //    }
                //}

        
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
            galaxy.UpdatePoses(0.0);
        }
    }
}