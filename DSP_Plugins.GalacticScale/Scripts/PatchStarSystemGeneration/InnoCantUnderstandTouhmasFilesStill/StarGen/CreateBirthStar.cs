using System;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;
namespace GalacticScale
{
    public static partial class GS2
    {
        public static StarData CreateBirthStar(int seed)
        {
            var gSize = galaxy.starCount > 64 ? galaxy.starCount * 4 * 100 : 25600;
            Patch.Debug("gSize = " + gSize);
            galaxy.astroPoses = new AstroPose[gSize];
            StarData starData = new StarData();
            starData.galaxy = galaxy;
            starData.planetCount = settings.BirthStar.planetCount;
            starData.index = 0;
            starData.level = 0.0f;
            //galaxy.starCount <= 1 ? 0.0f : (float)starData.index / (float)(galaxy.starCount - 1);
            starData.id = 1;
            starData.seed = settings.BirthStar.Seed;
            starData.resourceCoef = settings.BirthStar.resourceCoef;
            starData.name = settings.BirthStar.Name;
            starData.overrideName = string.Empty;
            starData.position = VectorLF3.zero;
            starData.mass = settings.BirthStar.mass;
            starData.age = settings.BirthStar.age;
            starData.lifetime = settings.BirthStar.lifetime;
            starData.temperature = settings.BirthStar.temperature;
            starData.luminosity = settings.BirthStar.luminosity;
            starData.color = settings.BirthStar.color;
            starData.classFactor = settings.BirthStar.classFactor;
            starData.radius = settings.BirthStar.radius;
            starData.acdiskRadius = settings.BirthStar.acDiscRadius;
            starData.habitableRadius = settings.BirthStar.habitableRadius;
            starData.lightBalanceRadius = settings.BirthStar.habitableRadius;
            starData.orbitScaler = settings.BirthStar.orbitScaler;
            starData.dysonRadius = settings.BirthStar.dysonRadius;
            starData.type = settings.BirthStar.Type;
            starData.spectr = settings.BirthStar.Spectr;

            return starData;
        }
    }
}