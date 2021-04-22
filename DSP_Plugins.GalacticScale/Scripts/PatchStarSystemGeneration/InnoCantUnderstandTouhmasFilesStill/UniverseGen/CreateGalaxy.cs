using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static GalaxyData CreateGalaxy(GameDesc desc)
        {
            gameDesc = desc;
            random = new System.Random(settings.Seed);
            Patch.Debug("CreateGalaxy StarCount = " + settings.starCount);
            int tempPoses = GenerateTempPoses(
                random.Next(),
                settings.starCount,
                settings.GalaxyParams.iterations,
                settings.GalaxyParams.minDistance,
                settings.GalaxyParams.minStepLength,
                settings.GalaxyParams.maxStepLength,
                settings.GalaxyParams.flatten
                );
            Patch.Debug("Created " + tempPoses + "temp poses");
            galaxy = new GalaxyData();
            galaxy.seed = settings.Seed;
            galaxy.starCount = settings.starCount;
            galaxy.stars = new StarData[settings.starCount];
            Patch.Debug("Galaxy Initialized");
            if (settings.starCount <= 0)
            {
                Patch.Debug("Starcount 0");
                return galaxy;
            }
            int seed = random.Next();
            Patch.Debug("Starting to create Stars");
            for (var i = 0; i < settings.starCount; i++)
            {
                Patch.Debug("-" + i + " " + settings.starCount);
                galaxy.stars[i] = CreateStar(i);
            }
            Patch.Debug("Finished creating Stars");
            for (var i = 0; i < settings.starCount; i++)
                //StarGen.CreateStarPlanets(galaxy, galaxy.stars[i], gameDesc);
                CreateStarPlanets(ref galaxy.stars[i], gameDesc);
            InitializeAstroPoses();
            //galaxy.UpdatePoses(0.0);
            galaxy.birthPlanetId = 1;
            PopulateStarsWithPlanets();
            UniverseGen.CreateGalaxyStarGraph(galaxy);
            Patch.Debug("Returning Galaxy");
            return galaxy;

        }
        public static void PopulateStarsWithPlanets()
        {
            if (galaxy.starCount > 0)
            {
                StarData starData = galaxy.stars[0];
                for (int p = 0; p < starData.planetCount; p++)
                {
                    PlanetData planet = starData.planets[p];
                    ThemeProto themeProto = LDB.themes.Select(planet.theme);
                    Patch.Debug("Checking ThemeProto: " + (themeProto != null) + " " + themeProto.Distribute);
                    if (themeProto != null && themeProto.Distribute == EThemeDistribute.Birth)
                    {
                        Patch.Debug("Setting birth planet! " + planet.id + " / " + starData.id);
                        galaxy.birthPlanetId = planet.id;
                        galaxy.birthStarId = starData.id;
                        break;
                    }
                    else
                    {
                        Patch.Debug("FAILED TO SET BIRTH PLANET!@#");
                        DumpObjectToJson(Path.Combine(DataDir, "shit.json"), starData.planets);
                    }
                }
            }
            Assert.Positive(galaxy.birthPlanetId);
            for (int i = 1; i < galaxy.starCount; ++i)
            {
                StarData star = galaxy.stars[i];
                for (int j = 0; j < star.planetCount; ++j) PlanetModelingManager.Algorithm(star.planets[j]).GenerateVeins(true);
            }
        }
        public static void InitializeAstroPoses()
        {
            Patch.Debug("Initializing Astro Poses");
            AstroPose[] astroPoses = galaxy.astroPoses;
            for (int index = 0; index < galaxy.astroPoses.Length; ++index)
            {
                astroPoses[index].uRot.w = 1f;
                astroPoses[index].uRotNext.w = 1f;
            }
            Patch.Debug("Setting Astro Poses?");
            for (int index = 0; index < settings.starCount; ++index)
            {
                //StarGen.CreateStarPlanets(galaxy, galaxy.stars[index], gameDesc);
                astroPoses[galaxy.stars[index].id * 100].uPos = astroPoses[galaxy.stars[index].id * 100].uPosNext = galaxy.stars[index].uPosition;
                astroPoses[galaxy.stars[index].id * 100].uRot = astroPoses[galaxy.stars[index].id * 100].uRotNext = Quaternion.identity;
                astroPoses[galaxy.stars[index].id * 100].uRadius = galaxy.stars[index].physicsRadius;
            }
            galaxy.UpdatePoses(0.0);
        }
    }
}