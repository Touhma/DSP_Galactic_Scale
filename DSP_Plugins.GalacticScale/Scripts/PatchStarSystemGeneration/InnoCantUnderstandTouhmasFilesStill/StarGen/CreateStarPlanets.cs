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
            int bodyCount = settings.Stars[star.index].bodyCount;
            Patch.Debug("Star BodyCount = " + star.planetCount);
            //foreach (GSplanet planet in settings.Stars[star.index].Planets)
            //{
            //    Patch.Debug("Adding " + planet.MoonCount + " moons to body count");
            //    bodyCount += planet.MoonCount;
            //}
            
            Patch.Debug("--BodyCount = " + bodyCount + " for star index " + star.index);
            star.planets = new PlanetData[bodyCount+1];
            int id = star.index;
            List<GSplanet> planets = settings.Stars[id].Planets;
            for (var i = 0; i < settings.Stars[star.index].planetCount; i++)
            {
                Patch.Debug("Creating Planet " + i + " of " + settings.Stars[star.index].planetCount +  " for Star " + id);
                //star.planets[i] = PlanetGen.CreatePlanet(galaxy, star, gameDesc, i, 0, i+2, i+1, false, 123, 321);
                CreatePlanet(ref star, null);
                settings.Stars[star.index].counter++;
            }
            //for (var i = 0; i < settings.Stars[star.index].planetCount; i++)
            //{
            //    PlanetData planetData = star.planets[i];
            //    GSplanet gsPlanet = settings.Stars[id].Planets[i];
            //    if (gsPlanet.MoonCount > 0)
            //    {
            //        Patch.Debug("Creating moons for gsPlanet " + i + " of star " + star.index + ". Star.counter = " + settings.Stars[star.index].counter + " and star.planets.Length = " + star.planets.Length);

            //        CreateMoons(ref planetData, gsPlanet);
            //        settings.Stars[star.index].counter++;
            //        Patch.Debug("Moons Created, returning planetData");
            //    }
            //}
        }
    }
}