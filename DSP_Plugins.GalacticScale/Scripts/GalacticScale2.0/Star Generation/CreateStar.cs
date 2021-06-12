﻿namespace GalacticScale {
    public static partial class GS2 {
        public static StarData CreateStar(int index) => CreateStar(galaxy, index + 1);

        public static StarData CreateStar(GalaxyData galaxy, int id) {
            StarData starData = new StarData();
            int index = id - 1;
            GSStar star = GSSettings.Stars[index];
            star.assignedIndex = index;
            if (star.Seed < 0) {
                star.Seed = random.Next();
            }

            gsStars.Add(id, star);
            starData.galaxy = galaxy;
            starData.index = index;
            starData.level = galaxy.starCount > 1 ? starData.index / (float)(galaxy.starCount - 1) : 0.0f;
            starData.id = id;
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
            starData.lightBalanceRadius = star.habitableRadius;
            starData.orbitScaler = star.orbitScaler;
            starData.dysonRadius = star.dysonRadius;
            starData.type = star.Type;
            starData.spectr = star.Spectr;
            return starData;
        }
    }
}