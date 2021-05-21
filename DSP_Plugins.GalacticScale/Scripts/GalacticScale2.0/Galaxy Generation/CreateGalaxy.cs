using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static GalaxyData CreateGalaxy(GameDesc desc, bool createPlanets = true)
        {
            gameDesc = desc;
            GenerateGalaxy();
            gameDesc.starCount = GSSettings.starCount;
            if (GSSettings.ThemeLibrary == null || GSSettings.ThemeLibrary == new ThemeLibrary()) GSSettings.ThemeLibrary = ThemeLibrary;
            else ThemeLibrary = GSSettings.ThemeLibrary;
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
            InitializeAstroPoses();
            if (createPlanets)
            {
                SetupBirthPlanet();
                GenerateVeins();
            }
            UniverseGen.CreateGalaxyStarGraph(galaxy);
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
                    List<GSPlanet> bodies = star.bodies;
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
            for (var i = 0; i < GSSettings.starCount; i++) galaxy.stars[i] = CreateStar(i);
            for (var i = 0; i < GSSettings.starCount; i++) CreateStarPlanets(ref galaxy.stars[i], gameDesc);
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