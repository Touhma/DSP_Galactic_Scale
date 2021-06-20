using System;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void CreateStarPlanets(ref StarData star, GameDesc gameDesc, GS2.Random random)
        {
            Log($"Start|{star.name}");
            GSStar gsStar = GSSettings.Stars[star.index];
            gsStar.counter = 0;
            while (gsStar.bodyCount > 99)
            {
                Warn($"Truncating planets for star {star.name} as it has {gsStar.bodyCount}");
                gsStar.Planets.RemoveAt(gsStar.Planets.Count - 1);
                Warn($"New BodyCount = {gsStar.bodyCount}, existing planetCount was {star.planetCount}");
                star.planetCount = gsStar.bodyCount;
            }
            star.planets = new PlanetData[Math.Min(100, gsStar.bodyCount)];
            Log("Creating Planet");
            for (var i = 0; i < gsStar.planetCount; i++)
            {
                CreatePlanet(ref star, gsStar.Planets[i], random, null);
            }

            Log($"End|{star.name}");
        }
    }
}