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
            GenerateGalaxy();
            gameDesc = desc;
            gameDesc.starCount = GSSettings.starCount;
            random = new System.Random(GSSettings.Seed);
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
            if (GSSettings.starCount <= 0) return galaxy;
            for (var i = 0; i < GSSettings.starCount; i++)
            {
                galaxy.stars[i] = CreateStar(i);
                //if (galaxy.stars[i].position == new VectorLF3() && i>1)
                //{
                //    galaxy.stars[i].position = tmp_poses[i];
                //    galaxy.stars[i].uPosition = tmp_poses[i] * 2400000.0;
                //}
            }
            for (var i = 0; i < GSSettings.starCount; i++) CreateStarPlanets(ref galaxy.stars[i], gameDesc);
            InitializeAstroPoses();
            galaxy.birthPlanetId = 1;
            if (createPlanets) PopulateStarsWithPlanets();
            UniverseGen.CreateGalaxyStarGraph(galaxy);
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
                    if (themeProto != null && themeProto.Distribute == EThemeDistribute.Birth)
                    {
                        galaxy.birthPlanetId = planet.id;
                        galaxy.birthStarId = starData.id;
                        break;
                    }
                    else
                    {
                        Patch.Debug("FAILED TO SET BIRTH PLANET!", BepInEx.Logging.LogLevel.Warning, true);
                        DumpObjectToJson(Path.Combine(DataDir, "error.json"), starData.planets);
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