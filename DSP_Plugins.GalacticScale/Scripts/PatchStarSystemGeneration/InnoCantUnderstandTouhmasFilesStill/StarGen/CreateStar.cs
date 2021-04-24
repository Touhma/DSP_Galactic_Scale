using System;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static StarData CreateStar(int index)
        {
            Patch.Debug("Creating Star " + index);
            int seed = random.Next();
            if (index == 0) return CreateBirthStar(seed);
            StarData star = CreateStar(galaxy, tmp_poses[index], index + 1, seed, settings.Stars[index].Type, settings.Stars[index].Spectr);
            Patch.Debug("CreateStar " + tmp_poses[index]);
            //galaxy.stars[index] = star;
            //StarGen.CreateStarPlanets(galaxy, star, gameDesc);
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
            starData.level = galaxy.starCount <= 1 ? 0.0f : (float)starData.index / (float)(galaxy.starCount - 1); //maybe this should be a function
            starData.id = id;
            starData.seed = settings.Stars[id -1].Seed;
            if (settings.Stars[id - 1].pos == new VectorLF3()) settings.Stars[id - 1].pos = tmp_pos;
            starData.position = settings.Stars[id -1].pos;
            starData.uPosition = starData.position * 2400000.0;
            //starData.planetCount = settings.Stars[id - 1].bodyCount;
            starData.planetCount = 4;
            Patch.Debug("Set planetcount for " + (id - 1) + " to " + starData.planetCount);
            starData.resourceCoef = settings.Stars[id -1].resourceCoef;
            starData.name = settings.Stars[id -1].Name;
            starData.overrideName = string.Empty;
            starData.position = VectorLF3.zero;
            starData.mass = settings.Stars[id -1].mass;
            starData.age = settings.Stars[id -1].age;
            starData.lifetime = settings.Stars[id -1].lifetime;
            starData.temperature = settings.Stars[id -1].temperature;
            starData.luminosity = settings.Stars[id -1].luminosity;
            starData.color = settings.Stars[id -1].color;
            starData.classFactor = settings.Stars[id -1].classFactor;
            starData.radius = settings.Stars[id -1].radius;
            starData.acdiskRadius = settings.Stars[id -1].acDiscRadius;
            starData.habitableRadius = settings.Stars[id -1].habitableRadius;
            starData.lightBalanceRadius = settings.Stars[id -1].habitableRadius;
            starData.orbitScaler = settings.Stars[id -1].orbitScaler;
            starData.dysonRadius = settings.Stars[id -1].dysonRadius;
            starData.type = settings.Stars[id -1].Type;
            starData.spectr = settings.Stars[id -1].Spectr;

            return starData;
        }
        //{
        //    StarData starData = new StarData()
        //    {
        //        galaxy = galaxy,
        //        index = id - 1
        //    };
        //    starData.level = galaxy.starCount <= 1 ? 0.0f : (float)starData.index / (float)(galaxy.starCount - 1);
        //    starData.id = id;
        //    starData.seed = seed;
        //    System.Random random1 = new System.Random(seed);
        //    int seed1 = random1.Next();
        //    int Seed = random1.Next();
        //    starData.position = pos;
        //    float num1 = (float)pos.magnitude / 32f;
        //    if ((double)num1 > 1.0)
        //        num1 = Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(num1) + 1f) + 1f) + 1f) + 1f) + 1f;
        //    starData.resourceCoef = Mathf.Pow(7f, num1) * 0.6f;
        //    System.Random random2 = new System.Random(Seed);
        //    double r1 = random2.NextDouble();
        //    double r2 = random2.NextDouble();
        //    double num2 = random2.NextDouble();
        //    double rn = random2.NextDouble();
        //    double rt = random2.NextDouble();
        //    double num3 = (random2.NextDouble() - 0.5) * 0.2;
        //    double num4 = random2.NextDouble() * 0.2 + 0.9;
        //    double y = random2.NextDouble() * 0.4 - 0.2;
        //    double num5 = Math.Pow(2.0, y);
        //    float num6 = Mathf.Lerp(-0.98f, 0.88f, starData.level);
        //    float averageValue = (double)num6 >= 0.0 ? num6 + 0.65f : num6 - 0.65f;
        //    float standardDeviation = 0.33f;
        //    if (needtype == EStarType.GiantStar)
        //    {
        //        averageValue = y <= -0.08 ? 1.6f : -1.5f;
        //        standardDeviation = 0.3f;
        //    }
        //    float num7 = RandNormal(averageValue, standardDeviation, r1, r2);
        //    switch (needSpectr)
        //    {
        //        case ESpectrType.M:
        //            num7 = -3f;
        //            break;
        //        case ESpectrType.O:
        //            num7 = 3f;
        //            break;
        //    }
        //    float p1 = (float)((double)Mathf.Clamp((double)num7 <= 0.0 ? num7 * 1f : num7 * 2f, -2.4f, 4.65f) + num3 + 1.0);
        //    switch (needtype)
        //    {
        //        case EStarType.WhiteDwarf:
        //            starData.mass = (float)(1.0 + r2 * 5.0);
        //            break;
        //        case EStarType.NeutronStar:
        //            starData.mass = (float)(7.0 + r1 * 11.0);
        //            break;
        //        case EStarType.BlackHole:
        //            starData.mass = (float)(18.0 + r1 * r2 * 30.0);
        //            break;
        //        default:
        //            starData.mass = Mathf.Pow(2f, p1);
        //            break;
        //    }
        //    double d = 5.0;
        //    if ((double)starData.mass < 2.0)
        //        d = 2.0 + 0.4 * (1.0 - (double)starData.mass);
        //    starData.lifetime = (float)(10000.0 * Math.Pow(0.1, Math.Log10((double)starData.mass * 0.5) / Math.Log10(d) + 1.0) * num4);
        //    switch (needtype)
        //    {
        //        case EStarType.GiantStar:
        //            starData.lifetime = (float)(10000.0 * Math.Pow(0.1, Math.Log10((double)starData.mass * 0.58) / Math.Log10(d) + 1.0) * num4);
        //            starData.age = (float)(num2 * 0.0399999991059303 + 0.959999978542328);
        //            break;
        //        case EStarType.WhiteDwarf:
        //        case EStarType.NeutronStar:
        //        case EStarType.BlackHole:
        //            starData.age = (float)(num2 * 0.400000005960464 + 1.0);
        //            if (needtype == EStarType.WhiteDwarf)
        //            {
        //                starData.lifetime += 10000f;
        //                break;
        //            }
        //            if (needtype == EStarType.NeutronStar)
        //            {
        //                starData.lifetime += 1000f;
        //                break;
        //            }
        //            break;
        //        default:
        //            starData.age = (double)starData.mass >= 0.5 ? ((double)starData.mass >= 0.8 ? (float)(num2 * 0.699999988079071 + 0.200000002980232) : (float)(num2 * 0.400000005960464 + 0.100000001490116)) : (float)(num2 * 0.119999997317791 + 0.0199999995529652);
        //            break;
        //    }
        //    float num8 = starData.lifetime * starData.age;
        //    if ((double)num8 > 5000.0)
        //        num8 = (float)(((double)Mathf.Log(num8 / 5000f) + 1.0) * 5000.0);
        //    if ((double)num8 > 8000.0)
        //        num8 = (Mathf.Log(Mathf.Log(Mathf.Log(num8 / 8000f) + 1f) + 1f) + 1f) * 8000f;
        //    starData.lifetime = num8 / starData.age;
        //    float f = (float)(1.0 - (double)Mathf.Pow(Mathf.Clamp01(starData.age), 20f) * 0.5) * starData.mass;
        //    starData.temperature = (float)(Math.Pow((double)f, 0.56 + 0.14 / (Math.Log10((double)f + 4.0) / Math.Log10(5.0))) * 4450.0 + 1300.0);
        //    double num9 = Math.Log10(((double)starData.temperature - 1300.0) / 4500.0) / Math.Log10(2.6) - 0.5;
        //    if (num9 < 0.0)
        //        num9 *= 4.0;
        //    if (num9 > 2.0)
        //        num9 = 2.0;
        //    else if (num9 < -4.0)
        //        num9 = -4.0;
        //    starData.spectr = (ESpectrType)Mathf.RoundToInt((float)num9 + 4f);
        //    starData.color = Mathf.Clamp01((float)((num9 + 3.5) * 0.200000002980232));
        //    starData.classFactor = (float)num9;
        //    starData.luminosity = Mathf.Pow(f, 0.7f);
        //    starData.radius = (float)(Math.Pow((double)starData.mass, 0.4) * num5);
        //    starData.acdiskRadius = 0.0f;
        //    float p2 = (float)num9 + 2f;
        //    starData.habitableRadius = Mathf.Pow(1.7f, p2) + 0.25f * Mathf.Min(1f, starData.orbitScaler);
        //    starData.lightBalanceRadius = Mathf.Pow(1.7f, p2);
        //    starData.orbitScaler = Mathf.Pow(1.35f, p2);
        //    if ((double)starData.orbitScaler < 1.0)
        //        starData.orbitScaler = Mathf.Lerp(starData.orbitScaler, 1f, 0.6f);
        //    StarGen.SetStarAge(starData, starData.age, rn, rt);
        //    starData.dysonRadius = starData.orbitScaler * 0.28f;
        //    if ((double)starData.dysonRadius * 40000.0 < (double)starData.physicsRadius * 1.5)
        //        starData.dysonRadius = (float)((double)starData.physicsRadius * 1.5 / 40000.0);
        //    starData.uPosition = starData.position * 2400000.0;
        //    starData.name = NameGen.RandomStarName(seed1, starData, galaxy);
        //    starData.overrideName = string.Empty;
        //    return starData;
        //}
    }
}