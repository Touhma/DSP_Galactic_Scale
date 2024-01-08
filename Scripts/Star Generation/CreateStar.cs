﻿using PCGSharp;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS3
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
            //GS3.Warn($"Creating star {star.Name} with id:{id} and index {index}");
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
            starData.type = star.Type;
            starData.spectr = star.Spectr;
            // Log($"BirthPlanet:{GSSettings.BirthPlanet.Name}");
            //0.10
            if (star == GSSettings.BirthStar)
            {
	            galaxy.birthStarId = starData.id;
	            // GS3.Log($"Applying Birth Hive Settings to {star.Name}");
	            // ConfigureBirthStarHiveSettings(random, starData);
            }
            // else ConfigureStarHiveSettings(random, starData);
            starData.safetyFactor = star.safetyFactor;
            starData.maxHiveCount = star.maxHiveCount;
            starData.hiveAstroOrbits = star.hiveAstroOrbits;
            GS3.LogJson(star.hiveAstroOrbits);
            starData.initialHiveCount = star.initialHiveCount;
 
            
            //end 0.10
            return starData;
        }

        
    }
}