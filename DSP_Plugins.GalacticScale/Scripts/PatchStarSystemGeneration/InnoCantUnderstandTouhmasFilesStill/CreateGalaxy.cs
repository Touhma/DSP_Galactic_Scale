
using System;
using System.Collections.Generic;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale
{
    public static class InnoGen
    {
        public static List<VectorLF3> tmp_poses;
        public static List<VectorLF3> tmp_drunk;
        public static int[] tmp_state;
        public static GalaxyData galaxy;
        public static System.Random random;
        public static GameDesc gameDesc;
        public static void CreateDummySettings(int starCount)
        {
            List<planet> p = new List<planet>
                {
                    new planet("Urf")
                };
            Patch.Debug("Setting BirthStar");
            settings.Stars.Add(new star(1,"BeetleJuice", ESpectrType.O, EStarType.MainSeqStar, p));
            Patch.Debug("Creating Dummy Stars");
            for (var i = 1; i < starCount; i++)
            {
                Patch.Debug("Adding new Star = " + i);
                settings.Stars.Add(new star(1,"Star" + i.ToString(), ESpectrType.X, EStarType.BlackHole, p));
            }
            Patch.Debug("Creating Dummy Params");
            galaxyParams g = new galaxyParams();
            g.iterations = 4;
            g.flatten = 0.18;
            g.minDistance = 1;
            g.minStepLength = 1; 
            g.maxStepLength = 1;
            settings.GalaxyParams = g;
        }

        public static GalaxyData CreateGalaxy(GameDesc desc)
        {
            gameDesc = desc;
            random = new System.Random(settings.Seed);
            Patch.Debug("CreateGalaxy StarCount = " + settings.starCount);
            int tempPoses = GenerateTempPoses(
                random.Next(), 
                settings.starCount, 
                settings.GalaxyParams.iterations, 
                settings.GalaxyParams.minDistance, 
                settings.GalaxyParams.minStepLength, 
                settings.GalaxyParams.maxStepLength, 
                settings.GalaxyParams.flatten
                );
            Patch.Debug("Created " + tempPoses + "temp poses");
            galaxy = new GalaxyData();
            galaxy.seed = settings.Seed;
            galaxy.starCount = settings.starCount;
            galaxy.stars = new StarData[settings.starCount];
            Patch.Debug("Galaxy Initialized");
            if (settings.starCount <= 0)
            {
                Patch.Debug("Starcount 0");
                return galaxy;
            }
            int seed = random.Next();
            Patch.Debug("Starting to create Stars");
            for (var i = 0; i < settings.starCount; i++)
            {
                Patch.Debug("-" + i + " " + settings.starCount);
                galaxy.stars[i] = CreateStar(i);
            }
            Patch.Debug("Finished creating Stars");
            InitializeAstroPoses();
            galaxy.UpdatePoses(0.0);
            galaxy.birthPlanetId = 0;
            PopulateStarsWithPlanets();
            UniverseGen.CreateGalaxyStarGraph(galaxy);
            return galaxy;

        }
        public static void PopulateStarsWithPlanets()
        {
            if (galaxy.starCount > 0)
            {
                StarData starData = galaxy.stars[0];
                for (int p = 0; p < starData.planetCount; p++)
                {
                    PlanetData planet = starData.planets[p];
                    ThemeProto themeProto = LDB.themes.Select(planet.theme);
                    if (themeProto != null && themeProto.Distribute == EThemeDistribute.Birth)
                    {
                        galaxy.birthPlanetId = planet.id;
                        galaxy.birthStarId = starData.id;
                        break;
                    }
                }
            }
            Assert.Positive(galaxy.birthPlanetId);
            for (int i = 1; i < galaxy.starCount; ++i)
            {
                StarData star = galaxy.stars[i];
                for (int j = 0; j < star.planetCount; ++j) PlanetModelingManager.Algorithm(star.planets[j]).GenerateVeins(true);
            }
        }
        public static void InitializeAstroPoses()
        {
            Patch.Debug("Initializing Astro Poses");
            AstroPose[] astroPoses = galaxy.astroPoses;
            for (int index = 0; index < galaxy.astroPoses.Length; ++index)
            {
                astroPoses[index].uRot.w = 1f;
                astroPoses[index].uRotNext.w = 1f;
            }
            Patch.Debug("Setting Astro Poses?");
            for (int index = 0; index < settings.starCount; ++index)
            {
                StarGen.CreateStarPlanets(galaxy, galaxy.stars[index], gameDesc);
                astroPoses[galaxy.stars[index].id * 100].uPos = astroPoses[galaxy.stars[index].id * 100].uPosNext = galaxy.stars[index].uPosition;
                astroPoses[galaxy.stars[index].id * 100].uRot = astroPoses[galaxy.stars[index].id * 100].uRotNext = Quaternion.identity;
                astroPoses[galaxy.stars[index].id * 100].uRadius = galaxy.stars[index].physicsRadius;
            }
            galaxy.UpdatePoses(0.0);
        }
        public static StarData CreateStar(int index)
        {
            Patch.Debug("Creating Star " + index);
            int seed = random.Next();
            if (index == 0) return CreateBirthStar(seed);
            return StarGen.CreateStar(galaxy, tmp_poses[index], index + 1, seed, settings.Stars[index].Type, settings.Stars[index].Spectr);
        }
        public static float RandNormal(
            float averageValue,
            float standardDeviation,
            double r1,
            double r2)
        {
            return averageValue + standardDeviation * (float)(Math.Sqrt(-2.0 * Math.Log(1.0 - r1)) * Math.Sin(2.0 * Math.PI * r2));
        }
        public static StarData CreateStar(
          GalaxyData galaxy,
          VectorLF3 pos,
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
            starData.seed = seed;
            System.Random random1 = new System.Random(seed);
            int seed1 = random1.Next();
            int Seed = random1.Next();
            starData.position = pos;
            float num1 = (float)pos.magnitude / 32f;
            if ((double)num1 > 1.0)
                num1 = Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(num1) + 1f) + 1f) + 1f) + 1f) + 1f;
            starData.resourceCoef = Mathf.Pow(7f, num1) * 0.6f;
            System.Random random2 = new System.Random(Seed);
            double r1 = random2.NextDouble();
            double r2 = random2.NextDouble();
            double num2 = random2.NextDouble();
            double rn = random2.NextDouble();
            double rt = random2.NextDouble();
            double num3 = (random2.NextDouble() - 0.5) * 0.2;
            double num4 = random2.NextDouble() * 0.2 + 0.9;
            double y = random2.NextDouble() * 0.4 - 0.2;
            double num5 = Math.Pow(2.0, y);
            float num6 = Mathf.Lerp(-0.98f, 0.88f, starData.level);
            float averageValue = (double)num6 >= 0.0 ? num6 + 0.65f : num6 - 0.65f;
            float standardDeviation = 0.33f;
            if (needtype == EStarType.GiantStar)
            {
                averageValue = y <= -0.08 ? 1.6f : -1.5f;
                standardDeviation = 0.3f;
            }
            float num7 = RandNormal(averageValue, standardDeviation, r1, r2);
            switch (needSpectr)
            {
                case ESpectrType.M:
                    num7 = -3f;
                    break;
                case ESpectrType.O:
                    num7 = 3f;
                    break;
            }
            float p1 = (float)((double)Mathf.Clamp((double)num7 <= 0.0 ? num7 * 1f : num7 * 2f, -2.4f, 4.65f) + num3 + 1.0);
            switch (needtype)
            {
                case EStarType.WhiteDwarf:
                    starData.mass = (float)(1.0 + r2 * 5.0);
                    break;
                case EStarType.NeutronStar:
                    starData.mass = (float)(7.0 + r1 * 11.0);
                    break;
                case EStarType.BlackHole:
                    starData.mass = (float)(18.0 + r1 * r2 * 30.0);
                    break;
                default:
                    starData.mass = Mathf.Pow(2f, p1);
                    break;
            }
            double d = 5.0;
            if ((double)starData.mass < 2.0)
                d = 2.0 + 0.4 * (1.0 - (double)starData.mass);
            starData.lifetime = (float)(10000.0 * Math.Pow(0.1, Math.Log10((double)starData.mass * 0.5) / Math.Log10(d) + 1.0) * num4);
            switch (needtype)
            {
                case EStarType.GiantStar:
                    starData.lifetime = (float)(10000.0 * Math.Pow(0.1, Math.Log10((double)starData.mass * 0.58) / Math.Log10(d) + 1.0) * num4);
                    starData.age = (float)(num2 * 0.0399999991059303 + 0.959999978542328);
                    break;
                case EStarType.WhiteDwarf:
                case EStarType.NeutronStar:
                case EStarType.BlackHole:
                    starData.age = (float)(num2 * 0.400000005960464 + 1.0);
                    if (needtype == EStarType.WhiteDwarf)
                    {
                        starData.lifetime += 10000f;
                        break;
                    }
                    if (needtype == EStarType.NeutronStar)
                    {
                        starData.lifetime += 1000f;
                        break;
                    }
                    break;
                default:
                    starData.age = (double)starData.mass >= 0.5 ? ((double)starData.mass >= 0.8 ? (float)(num2 * 0.699999988079071 + 0.200000002980232) : (float)(num2 * 0.400000005960464 + 0.100000001490116)) : (float)(num2 * 0.119999997317791 + 0.0199999995529652);
                    break;
            }
            float num8 = starData.lifetime * starData.age;
            if ((double)num8 > 5000.0)
                num8 = (float)(((double)Mathf.Log(num8 / 5000f) + 1.0) * 5000.0);
            if ((double)num8 > 8000.0)
                num8 = (Mathf.Log(Mathf.Log(Mathf.Log(num8 / 8000f) + 1f) + 1f) + 1f) * 8000f;
            starData.lifetime = num8 / starData.age;
            float f = (float)(1.0 - (double)Mathf.Pow(Mathf.Clamp01(starData.age), 20f) * 0.5) * starData.mass;
            starData.temperature = (float)(Math.Pow((double)f, 0.56 + 0.14 / (Math.Log10((double)f + 4.0) / Math.Log10(5.0))) * 4450.0 + 1300.0);
            double num9 = Math.Log10(((double)starData.temperature - 1300.0) / 4500.0) / Math.Log10(2.6) - 0.5;
            if (num9 < 0.0)
                num9 *= 4.0;
            if (num9 > 2.0)
                num9 = 2.0;
            else if (num9 < -4.0)
                num9 = -4.0;
            starData.spectr = (ESpectrType)Mathf.RoundToInt((float)num9 + 4f);
            starData.color = Mathf.Clamp01((float)((num9 + 3.5) * 0.200000002980232));
            starData.classFactor = (float)num9;
            starData.luminosity = Mathf.Pow(f, 0.7f);
            starData.radius = (float)(Math.Pow((double)starData.mass, 0.4) * num5);
            starData.acdiskRadius = 0.0f;
            float p2 = (float)num9 + 2f;
            starData.habitableRadius = Mathf.Pow(1.7f, p2) + 0.25f * Mathf.Min(1f, starData.orbitScaler);
            starData.lightBalanceRadius = Mathf.Pow(1.7f, p2);
            starData.orbitScaler = Mathf.Pow(1.35f, p2);
            if ((double)starData.orbitScaler < 1.0)
                starData.orbitScaler = Mathf.Lerp(starData.orbitScaler, 1f, 0.6f);
            StarGen.SetStarAge(starData, starData.age, rn, rt);
            starData.dysonRadius = starData.orbitScaler * 0.28f;
            if ((double)starData.dysonRadius * 40000.0 < (double)starData.physicsRadius * 1.5)
                starData.dysonRadius = (float)((double)starData.physicsRadius * 1.5 / 40000.0);
            starData.uPosition = starData.position * 2400000.0;
            starData.name = NameGen.RandomStarName(seed1, starData, galaxy);
            starData.overrideName = string.Empty;
            return starData;
        }

        public static StarData CreateBirthStar(int seed)
        {
            StarData starData = new StarData();
            starData.galaxy = galaxy;

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
        public static int GenerateTempPoses(
            int seed,
            int targetCount,
            int iterCount,
            double minDist,
            double minStepLen,
            double maxStepLen,
            double flatten)
        {
            if (tmp_poses == null)
            {
                tmp_poses = new List<VectorLF3>();
                tmp_drunk = new List<VectorLF3>();
            }
            else
            {
                tmp_poses.Clear();
                tmp_drunk.Clear();
            }
            if (iterCount < 1)
                iterCount = 1;
            else if (iterCount > 16)
                iterCount = 16;
            RandomPoses(seed, targetCount * iterCount, minDist, minStepLen, maxStepLen, flatten);
            for (int index = tmp_poses.Count - 1; index >= 0; --index)
            {
                if (index % iterCount != 0)
                    tmp_poses.RemoveAt(index);
                if (tmp_poses.Count <= targetCount)
                    break;
            }
            return tmp_poses.Count;
        }
        private static void RandomPoses(
          int seed,
          int maxCount,
          double minDist,
          double minStepLen,
          double maxStepLen,
          double flatten)
        {
            System.Random random = new System.Random(seed);
            double num1 = random.NextDouble();
            tmp_poses.Add(VectorLF3.zero);
            int num2 = 6;
            int num3 = 8;
            if (num2 < 1)
                num2 = 1;
            if (num3 < 1)
                num3 = 1;
            int num4 = (int)(num1 * (double)(num3 - num2) + (double)num2);
            for (int index = 0; index < num4; ++index)
            {
                int num5 = 0;
                while (num5++ < 256)
                {
                    double num6 = random.NextDouble() * 2.0 - 1.0;
                    double num7 = (random.NextDouble() * 2.0 - 1.0) * flatten;
                    double num8 = random.NextDouble() * 2.0 - 1.0;
                    double num9 = random.NextDouble();
                    double d = num6 * num6 + num7 * num7 + num8 * num8;
                    if (d <= 1.0 && d >= 1E-08)
                    {
                        double num10 = Math.Sqrt(d);
                        double num11 = (num9 * (maxStepLen - minStepLen) + minDist) / num10;
                        VectorLF3 pt = new VectorLF3(num6 * num11, num7 * num11, num8 * num11);
                        if (!CheckCollision(tmp_poses, pt, minDist))
                        {
                            tmp_drunk.Add(pt);
                            tmp_poses.Add(pt);
                            if (tmp_poses.Count >= maxCount)
                                return;
                            break;
                        }
                    }
                }
            }
            int num12 = 0;
            while (num12++ < 256)
            {
                for (int index = 0; index < tmp_drunk.Count; ++index)
                {
                    if (random.NextDouble() <= 0.7)
                    {
                        int num5 = 0;
                        while (num5++ < 256)
                        {
                            double num6 = random.NextDouble() * 2.0 - 1.0;
                            double num7 = (random.NextDouble() * 2.0 - 1.0) * flatten;
                            double num8 = random.NextDouble() * 2.0 - 1.0;
                            double num9 = random.NextDouble();
                            double d = num6 * num6 + num7 * num7 + num8 * num8;
                            if (d <= 1.0 && d >= 1E-08)
                            {
                                double num10 = Math.Sqrt(d);
                                double num11 = (num9 * (maxStepLen - minStepLen) + minDist) / num10;
                                VectorLF3 pt = new VectorLF3(tmp_drunk[index].x + num6 * num11, tmp_drunk[index].y + num7 * num11, tmp_drunk[index].z + num8 * num11);
                                if (!CheckCollision(tmp_poses, pt, minDist))
                                {
                                    tmp_drunk[index] = pt;
                                    tmp_poses.Add(pt);
                                    if (tmp_poses.Count >= maxCount)
                                        return;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        private static bool CheckCollision(List<VectorLF3> pts, VectorLF3 pt, double min_dist)
        {
            double num1 = min_dist * min_dist;
            foreach (VectorLF3 pt1 in pts)
            {
                double num2 = pt.x - pt1.x;
                double num3 = pt.y - pt1.y;
                double num4 = pt.z - pt1.z;
                if (num2 * num2 + num3 * num3 + num4 * num4 < num1)
                    return true;
            }
            return false;
        }

    }
}

