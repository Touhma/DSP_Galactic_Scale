using System.Collections.Generic;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void CreateStarPlanets(ref  StarData star, GameDesc gameDesc)
        {
            Patch.Debug("Creating Planets for star " +star.index+"! "+ star.planetCount);
            settings.Stars[star.index].counter = 0;
            int bodyCount = star.planetCount;
            Patch.Debug("Star PlanetCount = " + star.planetCount);
            foreach (GSplanet planet in settings.Stars[star.index].Planets)
            {
                Patch.Debug("Adding " + planet.MoonCount + " moons to body count");
                bodyCount += planet.MoonCount;
            }
            
            Patch.Debug("--BodyCount = " + bodyCount + " for star index " + star.index);
            star.planets = new PlanetData[bodyCount];
            int id = star.index;
            List<GSplanet> planets = settings.Stars[id].Planets;
            for (var i = 0; i < star.planetCount; i++)
            {
                Patch.Debug("Creating Planet " + i + " of " + star.planetCount+  " for Star " + id);
                //star.planets[i] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, i, 0, i+2, i+1, false, 123, 321);
                CreatePlanet(ref star, i, 0);
            }
        }
    }
}