using System.Collections.Generic;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void CreateStarPlanets(GalaxyData galaxy, StarData star, GameDesc gameDesc)
        {
            Patch.Debug("Creating Planets for star " +star.index+"! "+ star.planetCount);
            star.planets = new PlanetData[star.planetCount];
            int id = star.index;
            List<planet> planets = settings.Stars[id].Planets;
            for (var i = 0; i < star.planetCount; i++)
            {
                Patch.Debug("Creating Planet " + i + " for Star " + id);
                star.planets[i] = CreatePlanet(star, i);
            }
        }
    }
}