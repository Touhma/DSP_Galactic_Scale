namespace GalacticScale
{
    public static partial class GS2
    {
        public static StarData CreateBirthStar(int seed)
        {
            var gSize = galaxy.starCount > 64 ? galaxy.starCount * 4 * 100 : 25600;
            galaxy.astroPoses = new AstroPose[gSize];
            StarData starData = new StarData();
            starData.galaxy = galaxy;
            starData.planetCount = GSSettings.BirthStar.bodyCount;
            starData.index = 0;
            starData.level = 0.0f; 
            starData.id = 1;
            starData.seed = GSSettings.BirthStar.Seed;
            starData.resourceCoef = GSSettings.BirthStar.resourceCoef;
            starData.name = GSSettings.BirthStar.Name;
            starData.overrideName = string.Empty;
            starData.position = VectorLF3.zero;
            starData.mass = GSSettings.BirthStar.mass;
            starData.age = GSSettings.BirthStar.age;
            starData.lifetime = GSSettings.BirthStar.lifetime;
            starData.temperature = GSSettings.BirthStar.temperature;
            starData.luminosity = GSSettings.BirthStar.luminosity;
            starData.color = GSSettings.BirthStar.color;
            starData.classFactor = GSSettings.BirthStar.classFactor;
            starData.radius = GSSettings.BirthStar.radius;
            starData.acdiskRadius = GSSettings.BirthStar.acDiscRadius;
            starData.habitableRadius = GSSettings.BirthStar.habitableRadius;
            starData.lightBalanceRadius = GSSettings.BirthStar.habitableRadius;
            starData.orbitScaler = GSSettings.BirthStar.orbitScaler;
            starData.dysonRadius = GSSettings.BirthStar.dysonRadius;
            starData.type = GSSettings.BirthStar.Type;
            starData.spectr = GSSettings.BirthStar.Spectr;
            starData.asterBelt1OrbitIndex = 1;
            starData.asterBelt1Radius = 1.1f;

            return starData;
        }
    }
}