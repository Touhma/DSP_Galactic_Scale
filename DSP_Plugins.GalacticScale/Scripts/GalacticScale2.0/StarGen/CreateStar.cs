namespace GalacticScale
{
    public static partial class GS2
    {
        public static StarData CreateStar(int index)
        {
            int seed = random.Next();
           // if (index == 0) return CreateBirthStar(seed);
            StarData star = CreateStar(galaxy, tmp_poses[index], index + 1, seed, GSSettings.Stars[index].Type, GSSettings.Stars[index].Spectr);
            return star;
        }

        public static StarData CreateStar(
             GalaxyData galaxy,
             VectorLF3 tmp_pos,
             int id,
             int seed,
             EStarType needtype,
             ESpectrType needSpectr = ESpectrType.X)
        {
            StarData starData = new StarData()
            {
                galaxy = galaxy,
                index = id - 1
            };
            starData.level = galaxy.starCount <= 1 ? 0.0f : (float)starData.index / (float)(galaxy.starCount - 1);
            starData.id = id;
            starData.seed = GSSettings.Stars[id -1].Seed;
            GSSettings.Stars[id - 1].assignedIndex = id - 1;
            starData.position = GSSettings.Stars[id -1].position;
            starData.uPosition = starData.position * 2400000.0;
            starData.planetCount = GSSettings.Stars[id - 1].bodyCount;
            starData.resourceCoef = GSSettings.Stars[id -1].resourceCoef;
            starData.name = GSSettings.Stars[id -1].Name;
            starData.overrideName = string.Empty;
            starData.mass = GSSettings.Stars[id -1].mass;
            starData.age = GSSettings.Stars[id -1].age;
            starData.lifetime = GSSettings.Stars[id -1].lifetime;
            starData.temperature = GSSettings.Stars[id -1].temperature;
            starData.luminosity = GSSettings.Stars[id -1].luminosity;
            starData.color = GSSettings.Stars[id -1].color;
            starData.classFactor = GSSettings.Stars[id -1].classFactor;
            starData.radius = GSSettings.Stars[id -1].radius;
            starData.acdiskRadius = GSSettings.Stars[id -1].acDiscRadius;
            starData.habitableRadius = GSSettings.Stars[id -1].habitableRadius;
            starData.lightBalanceRadius = GSSettings.Stars[id -1].habitableRadius;
            starData.orbitScaler = GSSettings.Stars[id -1].orbitScaler;
            starData.dysonRadius = GSSettings.Stars[id -1].dysonRadius;
            starData.type = GSSettings.Stars[id -1].Type;
            starData.spectr = GSSettings.Stars[id -1].Spectr;

            return starData;
        }
    }
}