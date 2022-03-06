using System;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void CreateStarPlanets(ref StarData star, GameDesc gameDesc, Random random)
        {
            //Log($"Start|{star.name}");
            var gsStar = GSSettings.Stars[star.index];
            gsStar.counter = 0;
            while (gsStar.bodyCount > 99)
            {
                Log($"Truncating planets for star {star.name} as it has {gsStar.bodyCount}");
                gsStar.Planets.RemoveAt(gsStar.Planets.Count - 1);
                Warn($"New BodyCount = {gsStar.bodyCount}, existing planetCount was {star.planetCount}");
                star.planetCount = gsStar.bodyCount;
            }

            star.planets = new PlanetData[Math.Min(100, gsStar.bodyCount)];
            //Log("Creating Planet");
            for (var i = 0; i < gsStar.PlanetCount; i++) CreatePlanet(ref star, gsStar.Planets[i], random);
            star.planetCount = star.planets.Length;

            //Log($"End|{star.name}");
        }
    }
}