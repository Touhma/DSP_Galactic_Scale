﻿//// PlanetAlgorithm
//using System;
//using UnityEngine;

//public virtual void GenerateVeins(bool sketchOnly)
//{
//    lock (planet)
//    {
//        ThemeProto themeProto = LDB.themes.Select(planet.theme);
//        if (themeProto == null)
//        {
//            return;
//        }
//        System.Random random = new System.Random(planet.seed);
//        random.Next();
//        random.Next();
//        random.Next();
//        random.Next();
//        int birthSeed = random.Next();
//        int rnum1 = random.Next();
//        System.Random random2 = new System.Random(rnum1);
//        PlanetRawData planet_data = planet.data;
//        float num2 = 2.1f / planet.radius;
//        VeinProto[] veinProtos = PlanetModelingManager.veinProtos;
//        int[] veinModelIndexs = PlanetModelingManager.veinModelIndexs;
//        int[] veinModelCounts = PlanetModelingManager.veinModelCounts;
//        int[] veinProducts = PlanetModelingManager.veinProducts;
//        int[] _vein_spots = new int[veinProtos.Length];
//        float[] _vein_counts = new float[veinProtos.Length];
//        float[] _vein_opacity = new float[veinProtos.Length];
//        if (themeProto.VeinSpot != null)
//        {
//            Array.Copy(themeProto.VeinSpot, 0, _vein_spots, 1, Math.Min(themeProto.VeinSpot.Length, _vein_spots.Length - 1));
//        }
//        if (themeProto.VeinCount != null)
//        {
//            Array.Copy(themeProto.VeinCount, 0, _vein_counts, 1, Math.Min(themeProto.VeinCount.Length, _vein_counts.Length - 1));
//        }
//        if (themeProto.VeinOpacity != null)
//        {
//            Array.Copy(themeProto.VeinOpacity, 0, _vein_opacity, 1, Math.Min(themeProto.VeinOpacity.Length, _vein_opacity.Length - 1));
//        }
//        float p = 1f;
//        ESpectrType _star_spectr = planet.star.spectr;
//        switch (planet.star.type)
//        {
//            case EStarType.MainSeqStar:
//                switch (_star_spectr)
//                {
//                    case ESpectrType.M:
//                        p = 2.5f;
//                        break;
//                    case ESpectrType.K:
//                        p = 1f;
//                        break;
//                    case ESpectrType.G:
//                        p = 0.7f;
//                        break;
//                    case ESpectrType.F:
//                        p = 0.6f;
//                        break;
//                    case ESpectrType.A:
//                        p = 1f;
//                        break;
//                    case ESpectrType.B:
//                        p = 0.4f;
//                        break;
//                    case ESpectrType.O:
//                        p = 1.6f;
//                        break;
//                }
//                break;
//            case EStarType.GiantStar:
//                p = 2.5f;
//                break;
//            case EStarType.WhiteDwarf:
//                {
//                    p = 3.5f;
//                    _vein_spots[9]++;
//                    _vein_spots[9]++;
//                    for (int j = 1; j < 12; j++)
//                    {
//                        if (random.NextDouble() >= 0.44999998807907104)
//                        {
//                            break;
//                        }
//                        _vein_spots[9]++;
//                    }
//                    _vein_counts[9] = 0.7f;
//                    _vein_opacity[9] = 1f;
//                    _vein_spots[10]++;
//                    _vein_spots[10]++;
//                    for (int k = 1; k < 12; k++)
//                    {
//                        if (random.NextDouble() >= 0.44999998807907104)
//                        {
//                            break;
//                        }
//                        _vein_spots[10]++;
//                    }
//                    _vein_counts[10] = 0.7f;
//                    _vein_opacity[10] = 1f;
//                    _vein_spots[12]++;
//                    for (int l = 1; l < 12; l++)
//                    {
//                        if (random.NextDouble() >= 0.5)
//                        {
//                            break;
//                        }
//                        _vein_spots[12]++;
//                    }
//                    _vein_counts[12] = 0.7f;
//                    _vein_opacity[12] = 0.3f;
//                    break;
//                }
//            case EStarType.NeutronStar:
//                {
//                    p = 4.5f;
//                    _vein_spots[14]++;
//                    for (int m = 1; m < 12; m++)
//                    {
//                        if (random.NextDouble() >= 0.64999997615814209)
//                        {
//                            break;
//                        }
//                        _vein_spots[14]++;
//                    }
//                    _vein_counts[14] = 0.7f;
//                    _vein_opacity[14] = 0.3f;
//                    break;
//                }
//            case EStarType.BlackHole:
//                {
//                    p = 5f;
//                    _vein_spots[14]++;
//                    for (int i = 1; i < 12; i++)
//                    {
//                        if (random.NextDouble() >= 0.64999997615814209)
//                        {
//                            break;
//                        }
//                        _vein_spots[14]++;
//                    }
//                    _vein_counts[14] = 0.7f;
//                    _vein_opacity[14] = 0.3f;
//                    break;
//                }
//        }
//        for (int n = 0; n < themeProto.RareVeins.Length; n++)
//        {
//            int _rareVeinId = themeProto.RareVeins[n];
//            float _chance_spawn_rare_vein = ((planet.star.index != 0) ? themeProto.RareSettings[n * 4 + 1] : themeProto.RareSettings[n * 4]);
//            float _chanceforextrararespot = themeProto.RareSettings[n * 4 + 2];
//            float _veincountandopacity = themeProto.RareSettings[n * 4 + 3];
//            float num7 = _veincountandopacity;
//            _chance_spawn_rare_vein = 1f - Mathf.Pow(1f - _chance_spawn_rare_vein, p);
//            _veincountandopacity = 1f - Mathf.Pow(1f - _veincountandopacity, p);
//            num7 = 1f - Mathf.Pow(1f - num7, p);
//            if (!(random.NextDouble() < (double)_chance_spawn_rare_vein))
//            {
//                continue;
//            }
//            _vein_spots[_rareVeinId]++;
//            _vein_counts[_rareVeinId] = _veincountandopacity;
//            _vein_opacity[_rareVeinId] = _veincountandopacity;
//            for (int i = 1; i < 12; i++)
//            {
//                if (random.NextDouble() >= (double)_chanceforextrararespot)
//                {
//                    break;
//                }
//                _vein_spots[_rareVeinId]++;
//            }
//        }
//        bool flag = planet.galaxy.birthPlanetId == planet.id;
//        if (flag && !sketchOnly)
//        {
//            planet.GenBirthPoints(planet_data, birthSeed);
//        }
//        float num9 = planet.star.resourceCoef;
//        bool flag2 = DSPGame.GameDesc.resourceMultiplier >= 99.5f;
//        if (flag)
//        {
//            num9 *= 2f / 3f;
//        }
//        float num10 = 1f;
//        num10 *= 1.1f;
//        Array.Clear(veinVectors, 0, veinVectors.Length);
//        Array.Clear(veinVectorTypes, 0, veinVectorTypes.Length);
//        veinVectorCount = 0;
//        Vector3 birthPoint = default(Vector3);
//        if (flag)
//        {
//            birthPoint = planet.birthPoint;
//            birthPoint.Normalize();
//            birthPoint *= 0.75f;
//        }
//        else
//        {
//            birthPoint.x = (float)random2.NextDouble() * 2f - 1f;
//            birthPoint.y = (float)random2.NextDouble() - 0.5f;
//            birthPoint.z = (float)random2.NextDouble() * 2f - 1f;
//            birthPoint.Normalize();
//            birthPoint *= (float)(random2.NextDouble() * 0.4 + 0.2);
//        }
//        planet.veinSpotsSketch = _vein_spots;
//        if (sketchOnly)
//        {
//            return;
//        }
//        if (flag)
//        {
//            veinVectorTypes[0] = EVeinType.Iron;
//            ref Vector3 reference = ref veinVectors[0];
//            reference = planet.birthResourcePoint0;
//            veinVectorTypes[1] = EVeinType.Copper;
//            ref Vector3 reference2 = ref veinVectors[1];
//            reference2 = planet.birthResourcePoint1;
//            veinVectorCount = 2;
//        }
//        for (int num11 = 1; num11 < 15; num11++)
//        {
//            if (veinVectorCount >= veinVectors.Length)
//            {
//                break;
//            }
//            EVeinType eVeinType = (EVeinType)num11;
//            int num12 = _vein_spots[num11];
//            if (num12 > 1)
//            {
//                num12 += random2.Next(-1, 2);
//            }
//            for (int num13 = 0; num13 < num12; num13++)
//            {
//                int num14 = 0;
//                Vector3 zero = Vector3.zero;
//                bool flag3 = false;
//                while (num14++ < 200)
//                {
//                    zero.x = (float)random2.NextDouble() * 2f - 1f;
//                    zero.y = (float)random2.NextDouble() * 2f - 1f;
//                    zero.z = (float)random2.NextDouble() * 2f - 1f;
//                    if (eVeinType != EVeinType.Oil)
//                    {
//                        zero += birthPoint;
//                    }
//                    zero.Normalize();
//                    float num15 = planet_data.QueryHeight(zero);
//                    if (num15 < planet.radius || (eVeinType == EVeinType.Oil && num15 < planet.radius + 0.5f))
//                    {
//                        continue;
//                    }
//                    bool flag4 = false;
//                    float num16 = ((eVeinType != EVeinType.Oil) ? 196f : 100f);
//                    for (int num17 = 0; num17 < veinVectorCount; num17++)
//                    {
//                        if ((veinVectors[num17] - zero).sqrMagnitude < num2 * num2 * num16)
//                        {
//                            flag4 = true;
//                            break;
//                        }
//                    }
//                    if (flag4)
//                    {
//                        continue;
//                    }
//                    flag3 = true;
//                    break;
//                }
//                if (flag3)
//                {
//                    veinVectors[veinVectorCount] = zero;
//                    veinVectorTypes[veinVectorCount] = eVeinType;
//                    veinVectorCount++;
//                    if (veinVectorCount == veinVectors.Length)
//                    {
//                        break;
//                    }
//                }
//            }
//        }
//        Array.Clear(planet.veinAmounts, 0, planet.veinAmounts.Length);
//        planet_data.veinCursor = 1;
//        planet.veinGroups = new PlanetData.VeinGroup[veinVectorCount];
//        tmp_vecs.Clear();
//        VeinData vein = default(VeinData);
//        for (int num18 = 0; num18 < veinVectorCount; num18++)
//        {
//            tmp_vecs.Clear();
//            Vector3 normalized = veinVectors[num18].normalized;
//            EVeinType eVeinType2 = veinVectorTypes[num18];
//            int num19 = (int)eVeinType2;
//            Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, normalized);
//            Vector3 vector = quaternion * Vector3.right;
//            Vector3 vector2 = quaternion * Vector3.forward;
//            planet.veinGroups[num18].type = eVeinType2;
//            planet.veinGroups[num18].pos = normalized;
//            planet.veinGroups[num18].count = 0;
//            planet.veinGroups[num18].amount = 0L;
//            tmp_vecs.Add(Vector2.zero);
//            int num20 = Mathf.RoundToInt(_vein_counts[num19] * (float)random2.Next(20, 25));
//            if (eVeinType2 == EVeinType.Oil)
//            {
//                num20 = 1;
//            }
//            float num21 = _vein_opacity[num19];
//            if (flag && num18 < 2)
//            {
//                num20 = 6;
//                num21 = 0.2f;
//            }
//            int num22 = 0;
//            while (num22++ < 20)
//            {
//                int count = tmp_vecs.Count;
//                for (int num23 = 0; num23 < count; num23++)
//                {
//                    if (tmp_vecs.Count >= num20)
//                    {
//                        break;
//                    }
//                    if (tmp_vecs[num23].sqrMagnitude > 36f)
//                    {
//                        continue;
//                    }
//                    double num24 = random2.NextDouble() * Math.PI * 2.0;
//                    Vector2 vector3 = new Vector2((float)Math.Cos(num24), (float)Math.Sin(num24));
//                    vector3 += tmp_vecs[num23] * 0.2f;
//                    vector3.Normalize();
//                    Vector2 vector4 = tmp_vecs[num23] + vector3;
//                    bool flag5 = false;
//                    for (int num25 = 0; num25 < tmp_vecs.Count; num25++)
//                    {
//                        if ((tmp_vecs[num25] - vector4).sqrMagnitude < 0.85f)
//                        {
//                            flag5 = true;
//                            break;
//                        }
//                    }
//                    if (!flag5)
//                    {
//                        tmp_vecs.Add(vector4);
//                    }
//                }
//                if (tmp_vecs.Count >= num20)
//                {
//                    break;
//                }
//            }
//            int num26 = Mathf.RoundToInt(num21 * 100000f * num9);
//            if (num26 < 20)
//            {
//                num26 = 20;
//            }
//            int num27 = ((num26 >= 16000) ? 15000 : Mathf.FloorToInt((float)num26 * 0.9375f));
//            int minValue = num26 - num27;
//            int maxValue = num26 + num27 + 1;
//            for (int num28 = 0; num28 < tmp_vecs.Count; num28++)
//            {
//                Vector3 vector5 = (tmp_vecs[num28].x * vector + tmp_vecs[num28].y * vector2) * num2;
//                vein.type = eVeinType2;
//                vein.groupIndex = (short)num18;
//                vein.modelIndex = (short)random2.Next(veinModelIndexs[num19], veinModelIndexs[num19] + veinModelCounts[num19]);
//                vein.amount = Mathf.RoundToInt((float)random2.Next(minValue, maxValue) * num10);
//                if (planet.veinGroups[num18].type != EVeinType.Oil)
//                {
//                    vein.amount = Mathf.RoundToInt((float)vein.amount * DSPGame.GameDesc.resourceMultiplier);
//                }
//                if (vein.amount < 1)
//                {
//                    vein.amount = 1;
//                }
//                if (flag2 && vein.type != EVeinType.Oil)
//                {
//                    vein.amount = 1000000000;
//                }
//                vein.productId = veinProducts[num19];
//                vein.pos = normalized + vector5;
//                if (vein.type == EVeinType.Oil)
//                {
//                    vein.pos = planet.aux.RawSnap(vein.pos);
//                }
//                vein.minerCount = 0;
//                float num29 = planet_data.QueryHeight(vein.pos);
//                planet_data.EraseVegetableAtPoint(vein.pos);
//                vein.pos = vein.pos.normalized * num29;
//                if (planet.waterItemId == 0 || !(num29 < planet.radius))
//                {
//                    planet.veinAmounts[(uint)eVeinType2] += vein.amount;
//                    planet.veinGroups[num18].count++;
//                    planet.veinGroups[num18].amount += vein.amount;
//                    planet_data.AddVeinData(vein);
//                }
//            }
//        }
//        tmp_vecs.Clear();
//    }
//}