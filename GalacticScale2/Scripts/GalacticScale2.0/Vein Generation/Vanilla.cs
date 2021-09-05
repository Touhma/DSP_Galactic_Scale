using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class VeinAlgorithms
    {
        public static void GenerateVeinsVanilla(GSPlanet gsPlanet, bool sketchOnly)
        {
            random = new GS2.Random(gsPlanet.Seed);
            var themeProto = LDB.themes.Select(gsPlanet.planetData.theme);
            if (themeProto == null) return;

            var birth = GSSettings.BirthPlanet == gsPlanet;
            var planetRadiusFactor = 2.1f / gsPlanet.planetData.radius;
            InitializeFromThemeProto(gsPlanet, themeProto, out var veinSpots, out var veinCounts, out var veinOpacity);
            if (birth && !sketchOnly)
                gsPlanet.planetData.GenBirthPoints(gsPlanet.planetData.data, random.Next()); //GenBirthPoints(gsPlanet);

            gsPlanet.veinData.Clear();
            if (sketchOnly) return;
            if (birth) InitBirthVeinVectors(gsPlanet);

            CalculateVectorsVanilla(gsPlanet, planetRadiusFactor, veinSpots);
            AddVeinsToPlanetVanilla(gsPlanet, planetRadiusFactor, veinCounts, veinOpacity, birth);
        }

        private static void AddVeinsToPlanetVanilla(GSPlanet gsPlanet, float num2Point1Fdivbyplanetradius, float[] veinCounts, float[] veinOpacity, bool birth)
        {
            //random = new GS2.Random(GSSettings.Seed);
            var resourceCoef = gsPlanet.planetData.star.resourceCoef;
            if (birth) resourceCoef *= 2f / 3f;

            InitializePlanetVeins(gsPlanet.planetData, gsPlanet.veinData.count);
            var nodeVectors = new List<Vector2>();
            var infiniteResources = DSPGame.GameDesc.resourceMultiplier >= 99.5f;

            for (var i = 0; i < gsPlanet.veinData.count; i++) // For each veingroup (patch of vein nodes)
            {
                nodeVectors.Clear();
                var normalized = gsPlanet.veinData.vectors[i].normalized;
                var veinType = gsPlanet.veinData.types[i];
                var quaternion = Quaternion.FromToRotation(Vector3.up, normalized);
                var vectorRight = quaternion * Vector3.right;
                var vectorForward = quaternion * Vector3.forward;
                InitializeVeinGroup(i, veinType, normalized, gsPlanet.planetData);
                nodeVectors.Add(Vector2.zero); //Add a node at the centre of the patch/group
                var maxCount = Mathf.RoundToInt(veinCounts[(int)veinType] * random.Next(20, 25)); //change this to affect veingroup size.
                if (veinType == EVeinType.Oil) maxCount = 1;
                var opacity = veinOpacity[(int)veinType];
                if (birth && i < 2)
                {
                    maxCount = 6;
                    opacity = 0.2f;
                }

                GenerateNodeVectors(nodeVectors, maxCount);

                var veinAmount = Mathf.RoundToInt(opacity * 100000f * resourceCoef);
                if (veinAmount < 20) veinAmount = 20;

                for (var k = 0; k < nodeVectors.Count; k++)
                {
                    //GS2.Log(node_vectors[k] + " is the node_vector[k]");
                    var vector5 = (nodeVectors[k].x * vectorRight + nodeVectors[k].y * vectorForward) * num2Point1Fdivbyplanetradius;
                    //GS2.Log("and its vector5 is " + vector5);
                    if (gsPlanet.planetData.veinGroups[i].type != EVeinType.Oil)
                        veinAmount = Mathf.RoundToInt(veinAmount * DSPGame.GameDesc.resourceMultiplier);

                    if (veinAmount < 1) veinAmount = 1;

                    if (infiniteResources && veinType != EVeinType.Oil) veinAmount = 1000000000;

                    var veinPosition = normalized + vector5;
                    //GS2.Log("veinPosition = " + veinPosition);
                    if (veinType == EVeinType.Oil) SnapToGrid(ref veinPosition, gsPlanet.planetData);

                    EraseVegetableAtPoint(veinPosition, gsPlanet.planetData);
                    veinPosition = Utils.PositionAtSurface(veinPosition, gsPlanet);
                    if (!Utils.IsUnderWater(veinPosition, gsPlanet))
                        AddVeinToPlanet(veinAmount, veinType, veinPosition, (short)i, gsPlanet.planetData);
                }
            }

            nodeVectors.Clear();
        }

        private static void CalculateVectorsVanilla(GSPlanet gsPlanet, float planetRadiusFactor, int[] veinSpots)
        {
            //random = new GS2.Random(GSSettings.Seed);
            var birth = gsPlanet.planetData.id == GSSettings.BirthPlanetId;
            var spawnVector = InitVeinGroupVector(gsPlanet.planetData, birth); //Random Vector, unless its birth planet.
            for (var k = 1; k < 15; k++) //for each of the vein types
            {
                //GS2.Log("For loop " + k + " " + veinVectors.Length + " " + veinVectorCount);
                if (gsPlanet.veinData.count >= gsPlanet.veinData.vectors.Length) break; //If Greater than 1024 quit

                var eVeinType = (EVeinType)k;
                var spotsCount = veinSpots[k];
                if (spotsCount > 1) spotsCount += random.Next(-1, 2); //randomly -1, 0, 1
                for (var i = 0; i < spotsCount; i++)
                {
                    var j = 0;
                    var potentialVector = Vector3.zero;
                    var succeeded = false;
                    while (j++ < 50) //do this 200 times Default 50
                    {
                        potentialVector = Utils.RandomDirection(random);
                        if (eVeinType != EVeinType.Oil)
                            potentialVector += spawnVector; //if its not an oil vein, add the random spawn vector to this tiny vector..moving the location away from spawn?
                        potentialVector.Normalize(); //make the length of the vector 1
                        var height = gsPlanet.planetData.data.QueryHeight(potentialVector);
                        if (height < gsPlanet.planetData.radius || eVeinType == EVeinType.Oil && height < gsPlanet.planetData.radius + 0.5f) //if height is less than the planets radius, or its an oil vein and its less than slightly more than the planets radius...
                            continue; //find another potential vector, this one was underground?
                        var failed = false;
                        var veinGroupPadding = eVeinType != EVeinType.Oil ? 196f : 100f;
                        for (var m = 0; m < gsPlanet.veinData.count; m++) //check each veinvector we have already calculated
                            if ((gsPlanet.veinData.vectors[m] - potentialVector).sqrMagnitude < Mathf.Pow(planetRadiusFactor, 2) * veinGroupPadding)
                            {
                                //if the (vein vector less the potential vector (above ground)) length is less than (2.1/radius)^2 * 196
                                //... in other words for a 200 planet 0.0196 or 0.01 vein/oil . 
                                // I believe this is checking to see if there will be a collision between an already placed vein and this one
                                failed = true; //guess thats a loser?
                                break;
                            }

                        if (failed) continue;
                        succeeded = true; //we have a winner
                        break;
                    }

                    if (succeeded)
                    {
                        //GS2.Log("Found a vector");
                        gsPlanet.veinData.vectors[gsPlanet.veinData.count] = potentialVector;
                        gsPlanet.veinData.types[gsPlanet.veinData.count] = eVeinType;
                        gsPlanet.veinData.count++;
                        if (gsPlanet.veinData.count == gsPlanet.veinData.vectors.Length) break;
                    }
                    else
                    {
                        GS2.Warn(eVeinType + " vein unable to be placed on planet " + gsPlanet.planetData.name);
                    }
                }
            }
        }

        private static float InitSpecials(GSPlanet gsPlanet, int[] veinSpots, float[] veinCounts, float[] veinOpacity)
        {
            //random = new GS2.Random(GSSettings.Seed);
            var p = 1f;
            var starSpectr = gsPlanet.planetData.star.spectr;
            switch (gsPlanet.planetData.star.type)
            {
                case EStarType.MainSeqStar:
                    switch (starSpectr)
                    {
                        case ESpectrType.M:
                            p = 2.5f;
                            break;
                        case ESpectrType.K:
                            p = 1f;
                            break;
                        case ESpectrType.G:
                            p = 0.7f;
                            break;
                        case ESpectrType.F:
                            p = 0.6f;
                            break;
                        case ESpectrType.A:
                            p = 1f;
                            break;
                        case ESpectrType.B:
                            p = 0.4f;
                            break;
                        case ESpectrType.O:
                            p = 1.6f;
                            break;
                    }

                    break;
                case EStarType.GiantStar:
                    p = 2.5f;
                    break;
                case EStarType.WhiteDwarf:
                {
                    p = 3.5f;
                    veinSpots[9]++;
                    veinSpots[9]++;
                    for (var j = 1; j < 12; j++)
                    {
                        if (random.NextDouble() >= 0.44999998807907104) break;
                        veinSpots[9]++;
                    }

                    veinCounts[9] = 0.7f;
                    veinOpacity[9] = 1f;
                    veinSpots[10]++;
                    veinSpots[10]++;
                    for (var k = 1; k < 12; k++)
                    {
                        if (random.NextDouble() >= 0.44999998807907104) break;
                        veinSpots[10]++;
                    }

                    veinCounts[10] = 0.7f;
                    veinOpacity[10] = 1f;
                    veinSpots[12]++;
                    for (var l = 1; l < 12; l++)
                    {
                        if (random.NextDouble() >= 0.5) break;
                        veinSpots[12]++;
                    }

                    veinCounts[12] = 0.7f;
                    veinOpacity[12] = 0.3f;
                    break;
                }
                case EStarType.NeutronStar:
                {
                    p = 4.5f;
                    veinSpots[14]++;
                    for (var m = 1; m < 12; m++)
                    {
                        if (random.NextDouble() >= 0.64999997615814209) break;
                        veinSpots[14]++;
                    }

                    veinCounts[14] = 0.7f;
                    veinOpacity[14] = 0.3f;
                    break;
                }
                case EStarType.BlackHole:
                {
                    p = 5f;
                    veinSpots[14]++;
                    for (var i = 1; i < 12; i++)
                    {
                        if (random.NextDouble() >= 0.64999997615814209) break;
                        veinSpots[14]++;
                    }

                    veinCounts[14] = 0.7f;
                    veinOpacity[14] = 0.3f;
                    break;
                }
            }

            return p;
        }

        private static void InitRares(GSPlanet gsPlanet, ThemeProto themeProto, int[] veinSpots, float[] veinCounts, float[] veinOpacity, float p)
        {
            //random = new GS2.Random(GSSettings.Seed);
            for (var n = 0; n < themeProto.RareVeins.Length; n++)
            {
                var rareVeinId = themeProto.RareVeins[n];
                var chanceSpawnRareVein = gsPlanet.planetData.star.index != 0 ? themeProto.RareSettings[n * 4 + 1] : themeProto.RareSettings[n * 4];
                var chanceforextrararespot = themeProto.RareSettings[n * 4 + 2];
                var veincountandopacity = themeProto.RareSettings[n * 4 + 3];

                chanceSpawnRareVein = 1f - Mathf.Pow(1f - chanceSpawnRareVein, p);
                veincountandopacity = 1f - Mathf.Pow(1f - veincountandopacity, p);

                if (!(random.NextDouble() < chanceSpawnRareVein)) continue;
                veinSpots[rareVeinId]++;
                veinCounts[rareVeinId] = veincountandopacity;
                veinOpacity[rareVeinId] = veincountandopacity;
                for (var i = 1; i < 12; i++)
                {
                    if (random.NextDouble() >= chanceforextrararespot) break;
                    veinSpots[rareVeinId]++;
                }
            }
        }

        private static void InitializeFromThemeProto(GSPlanet gsPlanet, ThemeProto themeProto, out int[] veinSpots, out float[] veinCounts, out float[] veinOpacity)
        {
            var len = PlanetModelingManager.veinProtos.Length;
            veinCounts = new float[len];
            veinOpacity = new float[len];
            veinSpots = new int[len];
            if (themeProto.VeinSpot != null)
                Array.Copy(themeProto.VeinSpot, 0, veinSpots, 1, Math.Min(themeProto.VeinSpot.Length, veinSpots.Length - 1)); //How many Groups
            if (themeProto.VeinCount != null)
                Array.Copy(themeProto.VeinCount, 0, veinCounts, 1, Math.Min(themeProto.VeinCount.Length, veinCounts.Length - 1)); //How many veins per group
            if (themeProto.VeinOpacity != null)
                Array.Copy(themeProto.VeinOpacity, 0, veinOpacity, 1, Math.Min(themeProto.VeinOpacity.Length, veinOpacity.Length - 1)); //How Rich the veins are
            gsPlanet.planetData.veinSpotsSketch = veinSpots;
            var p = InitSpecials(gsPlanet, veinSpots, veinCounts, veinOpacity);
            InitRares(gsPlanet, themeProto, veinSpots, veinCounts, veinOpacity, p);
        }
    }
}