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
            ThemeProto themeProto = LDB.themes.Select(planet.theme);
            if (themeProto == null) return;

            System.Random random = new System.Random(planet.seed);

            int birthSeed = random.Next();
            PlanetRawData planetRawData = planet.data;
            float num2point1fdivbyplanetradius = 2.1f / planet.radius;
            VeinProto[] veinProtos = PlanetModelingManager.veinProtos;
            int[] veinModelIndexs = PlanetModelingManager.veinModelIndexs;
            int[] veinModelCounts = PlanetModelingManager.veinModelCounts;
            int[] veinProducts = PlanetModelingManager.veinProducts;
            int[] _vein_spots = new int[veinProtos.Length];
            float[] _vein_counts = new float[veinProtos.Length];
            float[] _vein_opacity = new float[veinProtos.Length];

            InitializeFromThemeProto(planet, themeProto, _vein_spots, _vein_counts, _vein_opacity);


            bool birth = (GSSettings.birthPlanetId == planet.id);
            if (birth) planet.GenBirthPoints(planetRawData, birthSeed);
       
            float resourceCoef = planet.star.resourceCoef;

            if (birth) resourceCoef *= 2f / 3f;

            Vector3[] veinVectors = gsPlanet.veinData.vectors;
            EVeinType[] veinVectorTypes = gsPlanet.veinData.types;
            ref int veinVectorCount = ref gsPlanet.veinData.count;

            Array.Clear(veinVectors, 0, veinVectors.Length);
            Array.Clear(veinVectorTypes, 0, veinVectorTypes.Length);
            veinVectorCount = 0;
            
            planet.veinSpotsSketch = _vein_spots;
            if (sketchOnly) return;
            GS2.Log("Still Going");
            if (birth) InitBirthVeinVectors(planet, veinVectors, veinVectorTypes, veinVectorCount);
            GS2.Log("Initted birthveinvectors, about to calculateveinvectors");
            CalculateVectors(planet, random, planetRawData, num2point1fdivbyplanetradius, _vein_spots,  veinVectors, veinVectorTypes, ref veinVectorCount);
            GS2.Log("Calculated VeinVectors, about to assignveinvectors");
            AssignVeins(planet, random, planetRawData, num2point1fdivbyplanetradius, veinModelIndexs, veinModelCounts, veinProducts, _vein_counts, _vein_opacity, birth, resourceCoef, veinVectors, veinVectorTypes, ref veinVectorCount);
            GS2.Log("Assigned Veins. Done Generating Veins");
        }

        private static void AssignVeins(PlanetData planet, System.Random random, PlanetRawData planetRawData, float num2point1fdivbyplanetradius, int[] veinModelIndexs, int[] veinModelCounts, int[] veinProducts, float[] _vein_counts, float[] _vein_opacity, bool birth, float resourceCoef,Vector3[] veinVectors, EVeinType[] veinVectorTypes, ref int veinVectorCount)
        {
            GS2.Log("AssignVeins " + planet.name);
            Array.Clear(planet.veinAmounts, 0, planet.veinAmounts.Length);
            planetRawData.veinCursor = 1;
            planet.veinGroups = new PlanetData.VeinGroup[veinVectorCount];
            List<Vector2> tmp_vecs = new List<Vector2>();
            bool infiniteResources = DSPGame.GameDesc.resourceMultiplier >= 99.5f;
            VeinData vein = default(VeinData);
            GS2.Log("  Vein Vector Count : " + veinVectorCount);
            for (int i = 0; i < veinVectorCount; i++)
            {
                GS2.Log("  -i- " + i);
                tmp_vecs.Clear();
                Vector3 normalized = veinVectors[i].normalized;
                EVeinType eVeinType2 = veinVectorTypes[i];
                int veinTypeIndex = (int)eVeinType2;
                Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, normalized);
                Vector3 vector_right = quaternion * Vector3.right;
                Vector3 vector_forward = quaternion * Vector3.forward;
                planet.veinGroups[i].type = eVeinType2;
                planet.veinGroups[i].pos = normalized;
                planet.veinGroups[i].count = 0;
                planet.veinGroups[i].amount = 0L;
                tmp_vecs.Add(Vector2.zero);
                int max_count = Mathf.RoundToInt(_vein_counts[veinTypeIndex] * (float)random.Next(20, 25));
                if (eVeinType2 == EVeinType.Oil)
                {
                    max_count = 1;
                }
                float opacity = _vein_opacity[veinTypeIndex];
                if (birth && i < 2)
                {
                    max_count = 6;
                    opacity = 0.2f;
                }
                int j = 0;
                while (j++ < 20) //do this 20 times
                {
                    int tmp_vecs_count = tmp_vecs.Count;
                    for (int m = 0; m < tmp_vecs_count; m++) //keep doing this while there are tmp_vecs to process. starting with one.
                    {
                        if (tmp_vecs.Count >= max_count)
                        {
                            break;
                        }
                        GS2.Log("sqrMagnitude:" + tmp_vecs[m].sqrMagnitude);
                        if (tmp_vecs[m].sqrMagnitude > 36f)
                        {
                            continue; //if the tmp_vec has already been set go on to the next one?
                        }
                        double z = random.NextDouble() * Math.PI * 2.0; //random Z
                        Vector2 randomVector = new Vector2((float)Math.Cos(z), (float)Math.Sin(z)); //random x/y/z on a sphere of radius 1
                        randomVector += tmp_vecs[m] * 0.2f; //add 20% of the tmp_vec...first time its 0
                        randomVector.Normalize();//make the length 1
                        Vector2 vector4 = tmp_vecs[m] + randomVector; //vector4 is the tmp_vec thats got some randomness to it
                        GS2.Log("*****" + m + " " + tmp_vecs[m] + " " + randomVector + " " + vector4);
                        bool flag5 = false;
                        for (int k = 0; k < tmp_vecs.Count; k++) //If there's already a vein within 0.85 tiles, discard this one.
                        {
                            if ((tmp_vecs[k] - vector4).sqrMagnitude < 0.85f)
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
                    if (tmp_vecs.Count >= max_count)
                    {
                        break;
                    }
                }
                GS2.Log("tmp_vecs Count" + tmp_vecs.Count);
                int num26 = Mathf.RoundToInt(opacity * 100000f * resourceCoef);
                if (num26 < 20)
                {
                    num26 = 20;
                }
                int num27 = ((num26 >= 16000) ? 15000 : Mathf.FloorToInt((float)num26 * 0.9375f));
                int minValue = num26 - num27;
                int maxValue = num26 + num27 + 1;
                for (int k = 0; k < tmp_vecs.Count; k++) //Create a Vein for each of tmp_vecs
                {
                    Vector3 vector5 = (tmp_vecs[k].x * vector_right + tmp_vecs[k].y * vector_forward) * num2point1fdivbyplanetradius;
                    vein.type = eVeinType2;
                    vein.groupIndex = (short)i;
                    vein.modelIndex = (short)random.Next(veinModelIndexs[veinTypeIndex], veinModelIndexs[veinTypeIndex] + veinModelCounts[veinTypeIndex]); //Choose a 3d Model for the Vein
                    vein.amount = Mathf.RoundToInt((float)random.Next(minValue, maxValue) * 1.1f); //choose an amount
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

                    vein.productId = veinProducts[veinTypeIndex];
                    vein.pos = normalized + vector5;
                    if (vein.type == EVeinType.Oil) vein.pos = planet.aux.RawSnap(vein.pos); //snap to grid
                    
                    vein.minerCount = 0;
                    float height = planetRawData.QueryHeight(vein.pos);
                    planetRawData.EraseVegetableAtPoint(vein.pos);
                    vein.pos = vein.pos.normalized * height;
                    GS2.Log("Going to add vein. Height is "+height);
                    if (planet.waterItemId == 0 || !(height < planet.radius)) //if its not underwater
                    {
                        GS2.Log("Adding Vein");
                        planet.veinAmounts[(uint)eVeinType2] += vein.amount;
                        planet.veinGroups[i].count++;
                        planet.veinGroups[i].amount += vein.amount;
                        planetRawData.AddVeinData(vein); //add to the planets rawdata veinpool
                        GS2.LogJson(vein);
                    }
                }
            }
            GS2.LogJson(planet.veinAmounts);
            GS2.LogJson(planet.veinGroups);
            tmp_vecs.Clear();
        }

        private static void CalculateVectors(PlanetData planet, System.Random random, PlanetRawData planetRawData, float num2point1fdivbyplanetradius, int[] _vein_spots, Vector3[] veinVectors, EVeinType[] veinVectorTypes, ref int veinVectorCount)
        {
            bool birth = planet.id == GSSettings.birthPlanetId;
            Vector3 spawnVector = InitSpawnVector(planet, random, birth);
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
                    spotsCount += random.Next(-1, 2); //randomly -1, 0, 1
                }
                for (int i = 0; i < spotsCount; i++)
                {
                    int j = 0;
                    Vector3 potentialVector = Vector3.zero;
                    bool flag3 = false;
                    while (j++ < 200) //do this 200 times
                    {
                        potentialVector.x = (float)random.NextDouble() * 2f - 1f; //Tiny Vector3 made up of Random numbers between -0.5 and 0.5
                        potentialVector.y = (float)random.NextDouble() * 2f - 1f;
                        potentialVector.z = (float)random.NextDouble() * 2f - 1f;
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
        }

        private static void InitBirthVeinVectors(PlanetData planet, Vector3[] veinVectors, EVeinType[] veinVectorTypes, int veinVectorCount)
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

        private static Vector3 InitSpawnVector(PlanetData planet, System.Random random2, bool birth)
        {
            Vector3 spawnVector;
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

            return spawnVector;
        }

        private static float InitSpecials(PlanetData planet, int[] _vein_spots, float[] _vein_counts, float[] _vein_opacity)
        {
            System.Random random = GS2.random;
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

            return p;
        }

        private static void InitRares(PlanetData planet, ThemeProto themeProto, int[] _vein_spots, float[] _vein_counts, float[] _vein_opacity, float p)
        {
            System.Random random = GS2.random;
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
        }

        private static void InitializeFromThemeProto(PlanetData planet, ThemeProto themeProto, int[] _vein_spots, float[] _vein_counts, float[] _vein_opacity)
        {
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
            float p = InitSpecials(planet, _vein_spots, _vein_counts, _vein_opacity);
            InitRares(planet, themeProto, _vein_spots, _vein_counts, _vein_opacity, p);
        }
    }
}
//}