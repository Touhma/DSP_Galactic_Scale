// PlanetAlgorithm
using GalacticScale;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace GalacticScale
{
    public static partial class VeinAlgorithms
    {
        public static void GenerateVeinsVanilla(GSPlanet gsPlanet, bool sketchOnly)
        {
            ThemeProto themeProto = LDB.themes.Select(gsPlanet.planetData.theme);
            if (themeProto == null) return;
            bool birth = GSSettings.BirthPlanet == gsPlanet;
            float planetRadiusFactor = 2.1f / gsPlanet.planetData.radius;
            InitializeFromThemeProto(gsPlanet, themeProto, out int[] _vein_spots, out float[] _vein_counts, out float[] _vein_opacity);
            if (birth && !sketchOnly) GenBirthPoints(gsPlanet);
            gsPlanet.veinData.Clear();
            if (sketchOnly) return;
            if (birth) InitBirthVeinVectors(gsPlanet);
            CalculateVectorsVanilla(gsPlanet, planetRadiusFactor, _vein_spots);
            AddVeinsToPlanetVanilla(gsPlanet, planetRadiusFactor, _vein_counts, _vein_opacity, birth);
        }

        public static void AddVeinsToPlanetVanilla(
            GSPlanet gsPlanet,
            float num2point1fdivbyplanetradius,
            float[] _vein_counts,
            float[] _vein_opacity,
            bool birth)
        {
            float resourceCoef = gsPlanet.planetData.star.resourceCoef;
            if (birth) resourceCoef *= 2f / 3f;
            InitializePlanetVeins(gsPlanet.planetData, gsPlanet.veinData.count);
            List<Vector2> node_vectors = new List<Vector2>();
            bool infiniteResources = DSPGame.GameDesc.resourceMultiplier >= 99.5f;

            for (int i = 0; i < gsPlanet.veinData.count; i++) // For each veingroup (patch of vein nodes)
            {
                node_vectors.Clear();
                Vector3 normalized = gsPlanet.veinData.vectors[i].normalized;
                EVeinType veinType = gsPlanet.veinData.types[i];
                Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, normalized);
                Vector3 vector_right = quaternion * Vector3.right;
                Vector3 vector_forward = quaternion * Vector3.forward;
                InitializeVeinGroup(i, veinType, normalized, gsPlanet.planetData);
                node_vectors.Add(Vector2.zero); //Add a node at the centre of the patch/group
                int max_count = Mathf.RoundToInt(_vein_counts[(int)veinType] * (float)random.Next(20, 25)); //change this to affect veingroup size.
                if (veinType == EVeinType.Oil)
                {
                    max_count = 1;
                }
                float opacity = _vein_opacity[(int)veinType];
                if (birth && i < 2)
                {
                    max_count = 6;
                    opacity = 0.2f;
                }
                GenerateNodeVectors(node_vectors, max_count);

                int veinAmount = Mathf.RoundToInt(opacity * 100000f * resourceCoef);
                if (veinAmount < 20) veinAmount = 20;

                for (int k = 0; k < node_vectors.Count; k++)
                {
                    //GS2.Log(node_vectors[k] + " is the node_vector[k]");
                    Vector3 vector5 = (node_vectors[k].x * vector_right + node_vectors[k].y * vector_forward) * num2point1fdivbyplanetradius;
                    //GS2.Log("and its vector5 is " + vector5);
                    if (gsPlanet.planetData.veinGroups[i].type != EVeinType.Oil) veinAmount = Mathf.RoundToInt(veinAmount * DSPGame.GameDesc.resourceMultiplier);
                    if (veinAmount < 1) veinAmount = 1;
                    if (infiniteResources && veinType != EVeinType.Oil) veinAmount = 1000000000;

                    Vector3 veinPosition = normalized + vector5;
                    //GS2.Log("veinPosition = " + veinPosition);
                    if (veinType == EVeinType.Oil) SnapToGrid(ref veinPosition, gsPlanet.planetData);

                    EraseVegetableAtPoint(veinPosition, gsPlanet.planetData);
                    veinPosition = PositionAtSurface(veinPosition, gsPlanet.planetData);
                    if (!IsUnderWater(veinPosition, gsPlanet.planetData)) AddVeinToPlanet(veinAmount, veinType, veinPosition, (short)i, gsPlanet.planetData);
                }
            }
            node_vectors.Clear();
        }

        public static void CalculateVectorsVanilla(GSPlanet gsPlanet, float planetRadiusFactor, int[] _vein_spots)
        {
            bool birth = (gsPlanet.planetData.id == GSSettings.birthPlanetId);
            Vector3 spawnVector = InitVeinGroupVector(gsPlanet.planetData, birth); //Random Vector, unless its birth planet.
            for (int k = 1; k < 15; k++) //for each of the vein types
            {
                //GS2.Log("For loop " + k + " " + veinVectors.Length + " " + veinVectorCount);
                if (gsPlanet.veinData.count >= gsPlanet.veinData.vectors.Length) break;//If Greater than 1024 quit

                EVeinType eVeinType = (EVeinType)k;
                int spotsCount = _vein_spots[k];
                if (spotsCount > 1)
                {
                    spotsCount += random.Next(-1, 2); //randomly -1, 0, 1
                }
                for (int i = 0; i < spotsCount; i++)
                {
                    int j = 0;
                    Vector3 potentialVector = Vector3.zero;
                    bool succeeded = false;
                    int c = 1;
                    while (j++ < 50) //do this 200 times Default 50
                    {
                        c++;
                        potentialVector = RandomDirection();
                        if (eVeinType != EVeinType.Oil)
                        {
                            potentialVector += spawnVector; //if its not an oil vein, add the random spawn vector to this tiny vector..moving the location away from spawn?
                        }
                        potentialVector.Normalize(); //make the length of the vector 1
                        float height = gsPlanet.planetData.data.QueryHeight(potentialVector);
                        if (height < gsPlanet.planetData.radius || (eVeinType == EVeinType.Oil && height < gsPlanet.planetData.radius + 0.5f)) //if height is less than the planets radius, or its an oil vein and its less than slightly more than the planets radius...
                        {
                            continue; //find another potential vector, this one was underground?
                        }
                        bool failed = false;
                        float veinGroupPadding = ((eVeinType != EVeinType.Oil) ? 196f : 100f);
                        for (int m = 0; m < gsPlanet.veinData.count; m++) //check each veinvector we have already calculated
                        {
                            if ((gsPlanet.veinData.vectors[m] - potentialVector).sqrMagnitude < Mathf.Pow(planetRadiusFactor, 2) * veinGroupPadding)
                            { //if the (vein vector less the potential vector (above ground)) length is less than (2.1/radius)^2 * 196
                              //... in other words for a 200 planet 0.0196 or 0.01 vein/oil . 
                              // I believe this is checking to see if there will be a collision between an already placed vein and this one
                                failed = true; //guess thats a loser?
                                break;
                            }
                        }
                        if (failed)
                        {
                            continue;
                        }
                        succeeded = true;//we have a winner
                        break;
                    }
                    if (succeeded)
                    {
                        //GS2.Log("Found a vector");
                        gsPlanet.veinData.vectors[gsPlanet.veinData.count] = potentialVector;
                        gsPlanet.veinData.types[gsPlanet.veinData.count] = eVeinType;
                        gsPlanet.veinData.count++;
                        if (gsPlanet.veinData.count == gsPlanet.veinData.vectors.Length)
                        {
                            break;
                        }
                    }
                    else
                    {
                        GS2.Warn(eVeinType + " vein unable to be placed on planet " + gsPlanet.planetData.name);
                    }
                }
            }
        }
        public static float InitSpecials(GSPlanet gsPlanet, int[] _vein_spots, float[] _vein_counts, float[] _vein_opacity)
        {
            System.Random random = GS2.random;
            float p = 1f;
            ESpectrType _star_spectr = gsPlanet.planetData.star.spectr;
            switch (gsPlanet.planetData.star.type)
            {
                case EStarType.MainSeqStar:
                    switch (_star_spectr)
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
                        _vein_spots[9]++;
                        _vein_spots[9]++;
                        for (int j = 1; j < 12; j++)
                        {
                            if (random.NextDouble() >= 0.44999998807907104)
                            {
                                break;
                            }
                            _vein_spots[9]++;
                        }
                        _vein_counts[9] = 0.7f;
                        _vein_opacity[9] = 1f;
                        _vein_spots[10]++;
                        _vein_spots[10]++;
                        for (int k = 1; k < 12; k++)
                        {
                            if (random.NextDouble() >= 0.44999998807907104)
                            {
                                break;
                            }
                            _vein_spots[10]++;
                        }
                        _vein_counts[10] = 0.7f;
                        _vein_opacity[10] = 1f;
                        _vein_spots[12]++;
                        for (int l = 1; l < 12; l++)
                        {
                            if (random.NextDouble() >= 0.5)
                            {
                                break;
                            }
                            _vein_spots[12]++;
                        }
                        _vein_counts[12] = 0.7f;
                        _vein_opacity[12] = 0.3f;
                        break;
                    }
                case EStarType.NeutronStar:
                    {
                        p = 4.5f;
                        _vein_spots[14]++;
                        for (int m = 1; m < 12; m++)
                        {
                            if (random.NextDouble() >= 0.64999997615814209)
                            {
                                break;
                            }
                            _vein_spots[14]++;
                        }
                        _vein_counts[14] = 0.7f;
                        _vein_opacity[14] = 0.3f;
                        break;
                    }
                case EStarType.BlackHole:
                    {
                        p = 5f;
                        _vein_spots[14]++;
                        for (int i = 1; i < 12; i++)
                        {
                            if (random.NextDouble() >= 0.64999997615814209)
                            {
                                break;
                            }
                            _vein_spots[14]++;
                        }
                        _vein_counts[14] = 0.7f;
                        _vein_opacity[14] = 0.3f;
                        break;
                    }
            }

            return p;
        }
        public static void InitRares(GSPlanet gsPlanet, ThemeProto themeProto, int[] _vein_spots, float[] _vein_counts, float[] _vein_opacity, float p)
        {
            System.Random random = GS2.random;
            for (int n = 0; n < themeProto.RareVeins.Length; n++)
            {
                int _rareVeinId = themeProto.RareVeins[n];
                float _chance_spawn_rare_vein = ((gsPlanet.planetData.star.index != 0) ? themeProto.RareSettings[n * 4 + 1] : themeProto.RareSettings[n * 4]);
                float _chanceforextrararespot = themeProto.RareSettings[n * 4 + 2];
                float _veincountandopacity = themeProto.RareSettings[n * 4 + 3];

                _chance_spawn_rare_vein = 1f - Mathf.Pow(1f - _chance_spawn_rare_vein, p);
                _veincountandopacity = 1f - Mathf.Pow(1f - _veincountandopacity, p);

                if (!(random.NextDouble() < (double)_chance_spawn_rare_vein))
                {
                    continue;
                }
                _vein_spots[_rareVeinId]++;
                _vein_counts[_rareVeinId] = _veincountandopacity;
                _vein_opacity[_rareVeinId] = _veincountandopacity;
                for (int i = 1; i < 12; i++)
                {
                    if (random.NextDouble() >= (double)_chanceforextrararespot)
                    {
                        break;
                    }
                    _vein_spots[_rareVeinId]++;
                }
            }
        }
        public static void InitializeFromThemeProto(GSPlanet gsPlanet, ThemeProto themeProto, out int[] _vein_spots, out float[] _vein_counts, out float[] _vein_opacity)
        {
            int len = PlanetModelingManager.veinProtos.Length;
            _vein_counts = new float[len];
            _vein_opacity = new float[len];
            _vein_spots = new int[len];
            if (themeProto.VeinSpot != null)
            {
                Array.Copy(themeProto.VeinSpot, 0, _vein_spots, 1, Math.Min(themeProto.VeinSpot.Length, _vein_spots.Length - 1)); //How many Groups
            }
            if (themeProto.VeinCount != null)
            {
                Array.Copy(themeProto.VeinCount, 0, _vein_counts, 1, Math.Min(themeProto.VeinCount.Length, _vein_counts.Length - 1)); //How many veins per group
            }
            if (themeProto.VeinOpacity != null)
            {
                Array.Copy(themeProto.VeinOpacity, 0, _vein_opacity, 1, Math.Min(themeProto.VeinOpacity.Length, _vein_opacity.Length - 1)); //How Rich the veins are
            }
            gsPlanet.planetData.veinSpotsSketch = _vein_spots;
            float p = InitSpecials(gsPlanet, _vein_spots, _vein_counts, _vein_opacity);
            InitRares(gsPlanet, themeProto, _vein_spots, _vein_counts, _vein_opacity, p);
        }

    }
}