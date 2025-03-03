﻿using PCGSharp;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static StarData CreateStar(int index, Random random)
        {
            return CreateStar(galaxy, index + 1, random);
        }

        public static StarData CreateStar(GalaxyData galaxy, int id, Random random)
        {
            var starData = new StarData();
            var index = id - 1;
            var star = GSSettings.Stars[index];
            star.assignedIndex = index;
            if (star.Seed < 0) star.Seed = random.Next();

            if (!gsStars.ContainsKey(id)) gsStars.Add(id, star);
            else gsStars[id] = star;
            starData.galaxy = galaxy;
            starData.index = index;
            starData.level = galaxy.starCount > 1 ? starData.index / (float)(galaxy.starCount - 1) : 0.0f;
            starData.id = id;
            // GS2.Warn($"Creating star {star.Name} with id:{id} and index {index} dyson radius {star.dysonRadius} lum:{star.luminosity}");
            starData.seed = star.Seed;
            starData.position = star.position;
            starData.uPosition = starData.position * 2400000.0;
            starData.planetCount = star.bodyCount;
            starData.resourceCoef = star.resourceCoef;
            starData.name = star.Name;
            starData.overrideName = string.Empty;
            starData.mass = star.mass;
            starData.age = star.age;
            starData.lifetime = star.lifetime;
            starData.temperature = star.temperature;
            starData.luminosity = star.luminosity;
            starData.color = star.color;
            starData.classFactor = star.classFactor;
            starData.radius = star.radius;
            starData.acdiskRadius = star.acDiscRadius;
            starData.habitableRadius = star.habitableRadius;
            starData.lightBalanceRadius = star.lightBalanceRadius;
            starData.orbitScaler = star.orbitScaler;
            starData.dysonRadius = star.dysonRadius;
                // GS2.Warn($"Set Radius for {star.Name} to {star.dysonRadius}.");
            starData.type = star.Type;
            starData.spectr = star.Spectr;
            // Log($"BirthPlanet:{GSSettings.BirthPlanet.Name}");
            //0.10
            if (star == GSSettings.BirthStar)
            {
	            galaxy.birthStarId = starData.id;
	            // GS2.Log($"Applying Birth Hive Settings to {star.Name}");
	            ConfigureBirthStarHiveSettings(random, starData);
            }
            else ConfigureStarHiveSettings(random, starData);
            //end 0.10
            return starData;
        }

        
    }
}