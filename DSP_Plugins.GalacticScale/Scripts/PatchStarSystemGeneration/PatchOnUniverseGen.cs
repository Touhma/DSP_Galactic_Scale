using HarmonyLib;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;
using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

namespace GalacticScale.Scripts.PatchStarSystemGeneration
{
    [HarmonyPatch(typeof(UniverseGen))]
    public class PatchOnUniverseGen
    {
        [HarmonyPrefix]
        [HarmonyPatch("CreateGalaxy")]
        public static bool CreateGalaxy(GameDesc gameDesc, ref GalaxyData __result, ref List<VectorLF3> ___tmp_poses)
        {
            if (DSPGame.IsMenuDemo) return true;
            InnoGen.CreateDummySettings(4);
            __result = InnoGen.CreateGalaxy(gameDesc);
            return false;

            //Patch.Debug("CreateGalaxy StarCount = " + settings.starCount);

            //// InnerCount for the System
            ////__result = ReworkUniverseGen.ReworkCreateGalaxy(gameDesc);
            //int galaxyAlgo = gameDesc.galaxyAlgo;
            //int galaxySeed = 1;//gameDesc.galaxySeed;
            //int starCount = settings.starCount;//gameDesc.starCount;
            ////if (galaxyAlgo < 20200101 || galaxyAlgo > 20591231)
            ////    throw new Exception("Wrong version of unigen algorithm!");
            //System.Random random = new System.Random(galaxySeed);

            //int tempPoses = 0;
            //try
            //{
            //    MethodInfo GenerateTempPoses = AccessTools.Method("UniverseGen:GenerateTempPoses", new Type[] { typeof(int), typeof(int), typeof(int), typeof(double), typeof(double), typeof(double), typeof(double) });
            //    tempPoses = (int)GenerateTempPoses.Invoke(null, new object[] { random.Next(), starCount, 4, 2.0, 2.3, 3.5, 0.18 });
            //}
            //catch (Exception e)
            //{
            //    Debug.Log("Except" + " " + e);
            //}
            //Patch.Debug("Generate Temp Posts");

            //Patch.Debug(starCount + " " + tempPoses);
            ///*
            //    int seed,
            //    int targetCount,
            //    int iterCount,
            //    double minDist,
            //    double minStepLen,
            //    double maxStepLen,
            //    double flatten)
            //*/
            //GalaxyData galaxy = new GalaxyData();
            //Patch.Debug(2);
            //galaxy.seed = galaxySeed;
            //galaxy.starCount = settings.starCount;
            //Patch.Debug(1);
            //galaxy.stars = new StarData[starCount];
            //Assert.Positive(starCount);
            //if (starCount <= 0)
            //{
            //    __result = galaxy;
            //    return false;
            //}
            //Patch.Debug(starCount + " " + tempPoses);
            //float num1 = (float)random.NextDouble();
            //float num2 = (float)random.NextDouble();
            //float num3 = (float)random.NextDouble();
            //float num4 = (float)random.NextDouble();
            //int num5 = Mathf.CeilToInt((float)(0.00999999977648258 * (double)starCount + (double)num1 * 0.300000011920929));
            //int num6 = Mathf.CeilToInt((float)(0.00999999977648258 * (double)starCount + (double)num2 * 0.300000011920929));
            //int num7 = Mathf.CeilToInt((float)(0.0160000007599592 * (double)starCount + (double)num3 * 0.400000005960464));
            //int num8 = Mathf.CeilToInt((float)(0.0130000002682209 * (double)starCount + (double)num4 * 1.39999997615814));
            //int num9 = starCount - num5;
            //int num10 = num9 - num6;
            //int num11 = num10 - num7;
            //int num12 = (num11 - 1) / num8;
            //int num13 = num12 / 2;
            //Patch.Debug(starCount + " " + tempPoses);
            ////for (int index = 0; index < starCount; ++index)
            ////{
            ////    int seed = random.Next();
            ////    if (index == 0)
            ////    {
            ////        galaxy.stars[index] = StarGen.CreateBirthStar(galaxy, seed);
            ////    }
            ////    else
            ////    {
            ////        ESpectrType needSpectr = ESpectrType.X;
            ////        if (index == 3)
            ////            needSpectr = ESpectrType.M;
            ////        else if (index == num11 - 1)
            ////            needSpectr = ESpectrType.O;
            ////        EStarType needtype = EStarType.MainSeqStar;
            ////        if (index % num12 == num13)
            ////            needtype = EStarType.GiantStar;
            ////        if (index >= num9)
            ////            needtype = EStarType.BlackHole;
            ////        else if (index >= num10)
            ////            needtype = EStarType.NeutronStar;
            ////        else if (index >= num11)
            ////            needtype = EStarType.WhiteDwarf;
            ////        galaxy.stars[index] = StarGen.CreateStar(galaxy, ___tmp_poses[index], index + 1, seed, needtype, needSpectr);
            ////    }
            ////}
            //Patch.Debug("3");
            //int seed = random.Next();
            //Patch.Debug("Greating Galaxy with Seed:" + seed);
            //galaxy.stars[0] = StarGen.CreateBirthStar(galaxy, seed);
            //for (var i = 1; i < settings.starCount; i++)
            //{
            //    Patch.Debug("-" + i + " " + settings.starCount);
            //    var s = settings.Stars[i - i];
            //    galaxy.stars[i] = StarGen.CreateStar(galaxy, ___tmp_poses[i], 2, seed, s.Type, s.Spectr);//EStarType.MainSeqStar, ESpectrType.O);
            //}
            //AstroPose[] astroPoses = galaxy.astroPoses;
            //Patch.Debug("Astroposes Length=" + astroPoses.Length);
            //StarData[] stars = galaxy.stars;
            //for (int index = 0; index < galaxy.astroPoses.Length; ++index)
            //{
            //    astroPoses[index].uRot.w = 1f;
            //    astroPoses[index].uRotNext.w = 1f;
            //}
            //for (int index = 0; index < starCount; ++index)
            //{
            //    Patch.Debug("Setting Positions? " + stars[0].id);
            //    StarGen.CreateStarPlanets(galaxy, stars[index], gameDesc);
            //    astroPoses[stars[index].id * 100].uPos = astroPoses[stars[index].id * 100].uPosNext = stars[index].uPosition;
            //    astroPoses[stars[index].id * 100].uRot = astroPoses[stars[index].id * 100].uRotNext = Quaternion.identity;
            //    astroPoses[stars[index].id * 100].uRadius = stars[index].physicsRadius;
            //}
            //galaxy.UpdatePoses(0.0);
            //galaxy.birthPlanetId = 0;
            //if (starCount > 0)
            //{
            //    StarData starData = stars[0];
            //    for (int index = 0; index < starData.planetCount; ++index)
            //    {
            //        PlanetData planet = starData.planets[index];
            //        ThemeProto themeProto = LDB.themes.Select(planet.theme);
            //        if (themeProto != null && themeProto.Distribute == EThemeDistribute.Birth)
            //        {
            //            galaxy.birthPlanetId = planet.id;
            //            galaxy.birthStarId = starData.id;
            //            break;
            //        }
            //    }
            //}
            //Assert.Positive(galaxy.birthPlanetId);
            //for (int index1 = 0; index1 < starCount; ++index1) //Generate All Veins
            //{
            //    StarData star = galaxy.stars[index1];
            //    for (int index2 = 0; index2 < star.planetCount; ++index2)
            //        PlanetModelingManager.Algorithm(star.planets[index2]).GenerateVeins(true);
            //}
            //UniverseGen.CreateGalaxyStarGraph(galaxy);
            //__result = galaxy;


            //return false;
        }
        
    }
}