using PCGSharp;
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
            //GS2.Warn($"Creating star {star.Name} with id:{id} and index {index}");
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
            
            //0.10
            float num = (float)starData.position.magnitude;
            float num16 = Mathf.Pow(starData.color, 1.3f);
            float num17 = Mathf.Clamp((num - 2f) / 20f, 0f, 2.5f);
            if (num17 > 1f)
            {
	            num17 = Mathf.Log(num17) + 1f;
	            num17 = Mathf.Log(num17) + 1f;
            }
            num17 /= 1.4f;
            if (starData.type == EStarType.BlackHole)
            {
	            num16 = 5f;
            }
            else if (starData.type == EStarType.NeutronStar)
            {
	            num16 = 1.7f;
            }
            else if (starData.type == EStarType.WhiteDwarf)
            {
	            num16 = 1.2f;
            }
            else if (starData.type == EStarType.GiantStar)
            {
	            num16 = Mathf.Max(0.6f, num16);
            }
            else if (starData.spectr == ESpectrType.O)
            {
	            num16 += 0.05f;
            }
            num16 *= 0.9f;
            num16 += 0.07f;
            float num18 = Mathf.Clamp01(1f - Mathf.Pow(num16, 0.73f) * Mathf.Pow(num17, 0.27f) + (float)random.NextDouble() * 0.08f - 0.04f);
		if (num18 >= 0.7f)
		{
			starData.hivePatternLevel = 0;
		}
		else if (num18 >= 0.3f)
		{
			starData.hivePatternLevel = 1;
		}
		else
		{
			starData.hivePatternLevel = 2;
		}
		starData.safetyFactor = num18;
		int num19 = random.Next(0, 1000);
		int num20 = (starData.epicHive ? 2 : 1);
		starData.maxHiveCount = (int)(gameDesc.combatSettings.maxDensity * (float)num20 * 1000f + (float)num19 + 0.5f) / 1000;
		float initialColonize = gameDesc.combatSettings.initialColonize;
		if (initialColonize < 0.015f)
		{
			starData.initialHiveCount = 0;
		}
		else
		{
			float num21 = Mathf.Pow(Mathf.Clamp01(starData.safetyFactor - 0.2f), 0.86f);
			float num22 = Mathf.Clamp01(1f - num21 - (float)(starData.maxHiveCount - 1) * 0.05f) * (1.1f - (float)starData.maxHiveCount * 0.1f);
			if (initialColonize <= 1f)
			{
				num22 *= initialColonize;
			}
			else
			{
				num22 = Mathf.Lerp(num22, 1f + (initialColonize - 1f) * 0.2f, (initialColonize - 1f) * 0.5f);
			}
			if (starData.type == EStarType.GiantStar)
			{
				num22 *= 1.2f;
			}
			else if (starData.type == EStarType.WhiteDwarf)
			{
				num22 *= 1.4f;
			}
			else if (starData.type == EStarType.NeutronStar)
			{
				num22 *= 1.6f;
			}
			else if (starData.type == EStarType.BlackHole)
			{
				num22 *= 1.8f;
			}
			else if (starData.spectr == ESpectrType.O)
			{
				num22 *= 1.1f;
			}
			float num23 = num22 * (float)starData.maxHiveCount;
			if (num23 > (float)starData.maxHiveCount + 0.75f)
			{
				num23 = (float)starData.maxHiveCount + 0.75f;
			}
			float standardDeviation2 = 0.5f;
			if ((double)num23 <= 0.01)
			{
				standardDeviation2 = 0f;
			}
			else if (num23 < 1f)
			{
				standardDeviation2 = Mathf.Sqrt(num23) * 0.29f + 0.21f;
			}
			else if (num23 > 1f)
			{
				standardDeviation2 = 0.3f + 0.2f * num23;
			}
			int num24 = 64;
			do
			{
				double r = random.NextDouble();
				double r2 = random.NextDouble();
				starData.initialHiveCount = (int)((double)StarGen.RandNormal(num23, standardDeviation2, r, r2) + 0.5);
			}
			while (num24-- > 0 && (starData.initialHiveCount < 0 || starData.initialHiveCount > starData.maxHiveCount));
			if (starData.initialHiveCount < 0)
			{
				starData.initialHiveCount = 0;
			}
			else if (starData.initialHiveCount > starData.maxHiveCount)
			{
				starData.initialHiveCount = starData.maxHiveCount;
			}
		}
		if (starData.type == EStarType.BlackHole)
		{
			int num25 = (int)(gameDesc.combatSettings.maxDensity * 1000f + (float)num19 + 0.5f) / 1000;
			if (starData.initialHiveCount < num25)
			{
				starData.initialHiveCount = num25;
			}
			if (starData.initialHiveCount < 1)
			{
				starData.initialHiveCount = 1;
			}
		}
		//end 0.10
            return starData;
        }
    }
}