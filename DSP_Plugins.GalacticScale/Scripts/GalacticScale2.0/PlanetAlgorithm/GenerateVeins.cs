// PlanetAlgorithm
using GalacticScale;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace GalacticScale
{
    public class GSPlanetVeins
    {
        public Vector3[] vectors = new Vector3[1024];
        public EVeinType[] types = new EVeinType[1024];
        public int count;

    }
    public static class GSPlanetAlgorithm
    {
        public static void GenerateVeins(GSPlanet gsPlanet, bool sketchOnly)
        {

            GS2.Log("GENERATEVEINS");
            PlanetData planet = gsPlanet.planetData;
            GS2.Log(planet.name);
            //lock (planet)
            //{
            ThemeProto themeProto = LDB.themes.Select(planet.theme);
            if (themeProto == null)
            {
                return;
            }
            System.Random random = new System.Random(planet.seed);
            int birthSeed = random.Next();
            int rnum1 = random.Next();
            System.Random random2 = new System.Random(rnum1);

            PlanetRawData planetRawData = planet.data;

            GS2.Log("1");

            float num2point1fdivbyplanetradius = 2.1f / planet.radius;
            VeinProto[] veinProtos = PlanetModelingManager.veinProtos;
            int[] veinModelIndexs = PlanetModelingManager.veinModelIndexs;
            int[] veinModelCounts = PlanetModelingManager.veinModelCounts;
            int[] veinProducts = PlanetModelingManager.veinProducts;
            int[] _vein_spots = new int[veinProtos.Length];
            float[] _vein_counts = new float[veinProtos.Length];
            float[] _vein_opacity = new float[veinProtos.Length];

            if (themeProto.VeinSpot != null)
            {
                Array.Copy(themeProto.VeinSpot, 0, _vein_spots, 1, Math.Min(themeProto.VeinSpot.Length, _vein_spots.Length - 1));
            }
            if (themeProto.VeinCount != null)
            {
                Array.Copy(themeProto.VeinCount, 0, _vein_counts, 1, Math.Min(themeProto.VeinCount.Length, _vein_counts.Length - 1));
            }
            if (themeProto.VeinOpacity != null)
            {
                GS2.Log("veinOpacity not null");
                Array.Copy(themeProto.VeinOpacity, 0, _vein_opacity, 1, Math.Min(themeProto.VeinOpacity.Length, _vein_opacity.Length - 1));
            }
            GS2.Log("2");
            GS2.LogJson(_vein_opacity);
            float p = 1f;
            ESpectrType _star_spectr = planet.star.spectr;
            switch (planet.star.type)
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

            for (int n = 0; n < themeProto.RareVeins.Length; n++)
            {
                int _rareVeinId = themeProto.RareVeins[n];
                float _chance_spawn_rare_vein = ((planet.star.index != 0) ? themeProto.RareSettings[n * 4 + 1] : themeProto.RareSettings[n * 4]);
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

            bool birth = (GSSettings.birthPlanetId == planet.id);
            GS2.Log("3");
            if (birth)
            {
                GS2.Log("Generating BirthPoints");
                planet.GenBirthPoints(planetRawData, birthSeed);
            }
            GS2.Log("30");
            float resourceCoef = planet.star.resourceCoef;
            bool infiniteResources = DSPGame.GameDesc.resourceMultiplier >= 99.5f;
            GS2.Log("3aa");

            if (birth)
            {
                resourceCoef *= 2f / 3f;
            }
            GS2.Log("3a1");
            float num10 = 1f;
            num10 *= 1.1f;
            Vector3[] veinVectors = gsPlanet.veinData.vectors;
            GS2.Log("3a1a");
            EVeinType[] veinVectorTypes = gsPlanet.veinData.types;
            GS2.Log("3a1b");
            ref int veinVectorCount = ref gsPlanet.veinData.count;
            GS2.Log("3a");
            Array.Clear(veinVectors, 0, veinVectors.Length);
            GS2.Log("3b");
            Array.Clear(veinVectorTypes, 0, veinVectorTypes.Length);
            GS2.Log("3c");
            veinVectorCount = 0;
            GS2.Log("31");
            Vector3 spawnVector = default(Vector3);
            if (birth)
            {
                spawnVector = planet.birthPoint;
                spawnVector.Normalize();
                spawnVector *= 0.75f;
            }
            else //randomize spawn vector
            {
                spawnVector.x = (float)random2.NextDouble() * 2f - 1f;
                spawnVector.y = (float)random2.NextDouble() - 0.5f;
                spawnVector.z = (float)random2.NextDouble() * 2f - 1f;
                spawnVector.Normalize();
                spawnVector *= (float)(random2.NextDouble() * 0.4 + 0.2);
            }
            planet.veinSpotsSketch = _vein_spots;

            if (sketchOnly)
            {
                return;
            }
            GS2.Log("34 Spawnvector is - " + spawnVector);

            if (birth) //set up the first two veinvectors.
            {
                GS2.Log("Setting up birth vectors");
                veinVectorTypes[0] = EVeinType.Iron;
                ref Vector3 reference = ref veinVectors[0];
                reference = planet.birthResourcePoint0;
                veinVectorTypes[1] = EVeinType.Copper;
                ref Vector3 reference2 = ref veinVectors[1];
                reference2 = planet.birthResourcePoint1;
                veinVectorCount = 2;
            }
            GS2.Log("35 - " + veinVectorCount);
            //Calculate Vectors
            GS2.Log("CALCULATING VECTORS");
            for (int k = 1; k < 15; k++) //for each of the vein types
            {
                GS2.Log("For loop " + k + " " + veinVectors.Length + " " + veinVectorCount);
                if (veinVectorCount >= veinVectors.Length)
                {
                    break;
                }
                EVeinType eVeinType = (EVeinType)k;
                int spotsCount = _vein_spots[k];
                if (spotsCount > 1)
                {
                    spotsCount += random2.Next(-1, 2); //randomly -1, 0, 1
                }
                for (int i = 0; i < spotsCount; i++)
                {
                    int j = 0;
                    Vector3 potentialVector = Vector3.zero;
                    bool flag3 = false;
                    while (j++ < 200) //do this 200 times
                    {
                        potentialVector.x = (float)random2.NextDouble() * 2f - 1f; //Tiny Vector3 made up of Random numbers between -0.5 and 0.5
                        potentialVector.y = (float)random2.NextDouble() * 2f - 1f;
                        potentialVector.z = (float)random2.NextDouble() * 2f - 1f;
                        if (eVeinType != EVeinType.Oil)
                        {
                            potentialVector += spawnVector; //if its not an oil vein, add the random spawn vector to this tiny vector
                        }
                        potentialVector.Normalize(); //make the length of the vector 1
                        float height = planetRawData.QueryHeight(potentialVector);
                        if (height < planet.radius || (eVeinType == EVeinType.Oil && height < planet.radius + 0.5f)) //if height is less than the planets radius, or its an oil vein and its less than slightly more than the planets radius...
                        {
                            continue; //find another potential vector, this one was underground?
                        }
                        bool flag4 = false;
                        float either196or100forveinoroil = ((eVeinType != EVeinType.Oil) ? 196f : 100f);
                        for (int m = 0; m < veinVectorCount; m++) //check each veinvector we have already calculated
                        {
                            if ((veinVectors[m] - potentialVector).sqrMagnitude < num2point1fdivbyplanetradius * num2point1fdivbyplanetradius * either196or100forveinoroil)
                            { //if the (vein vector less the potential vector (above ground)) length is less than (2.1/radius)^2 * 196
                              //... in other words for a 200 planet 0.0196 or 0.01 vein/oil . 
                              // I believe this is checking to see if there will be a collision between an already placed vein and this one
                                flag4 = true; //guess thats a loser?
                                break;
                            }
                        }
                        if (flag4)
                        {
                            continue;
                        }
                        flag3 = true;//we have a winner
                        break;
                    }
                    if (flag3)
                    {
                        GS2.Log("Found a vector");
                        veinVectors[veinVectorCount] = potentialVector;
                        veinVectorTypes[veinVectorCount] = eVeinType;
                        veinVectorCount++;
                        if (veinVectorCount == veinVectors.Length)
                        {
                            break;
                        }
                    }
                }
            }
            GS2.Log("FINISHED CALCULATING VECTORS");

            Array.Clear(planet.veinAmounts, 0, planet.veinAmounts.Length);
            planetRawData.veinCursor = 1;
            planet.veinGroups = new PlanetData.VeinGroup[veinVectorCount];
            List<Vector2> tmp_vecs = new List<Vector2>();
            tmp_vecs.Clear();
            VeinData vein = default(VeinData);
            GS2.Log("5");




            for (int i = 0; i < veinVectorCount; i++)
            {
                tmp_vecs.Clear();
                Vector3 normalized = veinVectors[i].normalized;
                EVeinType eVeinType2 = veinVectorTypes[i];
                int num19 = (int)eVeinType2;
                Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, normalized);
                Vector3 vector = quaternion * Vector3.right;
                Vector3 vector2 = quaternion * Vector3.forward;
                planet.veinGroups[i].type = eVeinType2;
                planet.veinGroups[i].pos = normalized;
                planet.veinGroups[i].count = 0;
                planet.veinGroups[i].amount = 0L;
                tmp_vecs.Add(Vector2.zero);
                int num20 = Mathf.RoundToInt(_vein_counts[num19] * (float)random2.Next(20, 25));
                if (eVeinType2 == EVeinType.Oil)
                {
                    num20 = 1;
                }
                float opacity = _vein_opacity[num19];
                if (birth && i < 2)
                {
                    num20 = 6;
                    opacity = 0.2f;
                }
                int j = 0;
                while (j++ < 20)
                {
                    int count = tmp_vecs.Count;
                    for (int num23 = 0; num23 < count; num23++)
                    {
                        if (tmp_vecs.Count >= num20)
                        {
                            break;
                        }
                        if (tmp_vecs[num23].sqrMagnitude > 36f)
                        {
                            continue;
                        }
                        double num24 = random2.NextDouble() * Math.PI * 2.0;
                        Vector2 vector3 = new Vector2((float)Math.Cos(num24), (float)Math.Sin(num24));
                        vector3 += tmp_vecs[num23] * 0.2f;
                        vector3.Normalize();
                        Vector2 vector4 = tmp_vecs[num23] + vector3;
                        bool flag5 = false;
                        for (int num25 = 0; num25 < tmp_vecs.Count; num25++)
                        {
                            if ((tmp_vecs[num25] - vector4).sqrMagnitude < 0.85f)
                            {
                                flag5 = true;
                                break;
                            }
                        }
                        if (!flag5)
                        {
                            tmp_vecs.Add(vector4);
                        }
                    }
                    if (tmp_vecs.Count >= num20)
                    {
                        break;
                    }
                }
                int num26 = Mathf.RoundToInt(opacity * 100000f * resourceCoef);
                if (num26 < 20)
                {
                    num26 = 20;
                }
                int num27 = ((num26 >= 16000) ? 15000 : Mathf.FloorToInt((float)num26 * 0.9375f));
                int minValue = num26 - num27;
                int maxValue = num26 + num27 + 1;
                for (int k = 0; k < tmp_vecs.Count; k++)
                {
                    Vector3 vector5 = (tmp_vecs[k].x * vector + tmp_vecs[k].y * vector2) * num2point1fdivbyplanetradius;
                    vein.type = eVeinType2;
                    vein.groupIndex = (short)i;
                    vein.modelIndex = (short)random2.Next(veinModelIndexs[num19], veinModelIndexs[num19] + veinModelCounts[num19]);
                    vein.amount = Mathf.RoundToInt((float)random2.Next(minValue, maxValue) * num10);
                    if (planet.veinGroups[i].type != EVeinType.Oil)
                    {
                        vein.amount = Mathf.RoundToInt((float)vein.amount * DSPGame.GameDesc.resourceMultiplier);
                    }
                    if (vein.amount < 1)
                    {
                        vein.amount = 1;
                    }

                    if (infiniteResources && vein.type != EVeinType.Oil)
                    {
                        vein.amount = 1000000000;
                    }

                    vein.productId = veinProducts[num19];
                    vein.pos = normalized + vector5;
                    if (vein.type == EVeinType.Oil)
                    {
                        vein.pos = planet.aux.RawSnap(vein.pos);
                    }
                    vein.minerCount = 0;
                    float height = planetRawData.QueryHeight(vein.pos);
                    planetRawData.EraseVegetableAtPoint(vein.pos);
                    vein.pos = vein.pos.normalized * height;
                    if (planet.waterItemId == 0 || !(height < planet.radius)) //if its not underwater
                    {
                        planet.veinAmounts[(uint)eVeinType2] += vein.amount;
                        planet.veinGroups[i].count++;
                        planet.veinGroups[i].amount += vein.amount;
                        planetRawData.AddVeinData(vein); //add to the planets rawdata veinpool
                    }
                }
            }
            tmp_vecs.Clear();
        }
    }
}
//}