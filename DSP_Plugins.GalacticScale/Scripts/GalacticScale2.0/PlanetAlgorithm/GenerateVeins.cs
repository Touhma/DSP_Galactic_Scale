// PlanetAlgorithm
using GalacticScale;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace GalacticScale
{
    //opacity determines vein (node) amount its randomized around 100000*opacity
    //could this be problematic due to not being thread safe?
    public class GSPlanetVeins
    {
        public Vector3[] vectors = new Vector3[1024];
        public EVeinType[] types = new EVeinType[1024];
        public int count;

    }
    public static class GSPlanetAlgorithm
    {
        public static GS2.Random random = new GS2.Random();
        public static void GenerateVeins(GSPlanet gsPlanet, bool sketchOnly)
        {
            GS2.Log("GENERATEVEINS");
            random = new GS2.Random(gsPlanet.Seed);
            PlanetData planet = gsPlanet.planetData;
            ThemeProto themeProto = LDB.themes.Select(planet.theme);
            if (themeProto == null) return;
            bool birth = (GSSettings.birthPlanetId == planet.id);
            //System.Random random = new System.Random(planet.seed);

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
            AddVeinsToPlanet(planet, random, num2point1fdivbyplanetradius, _vein_counts, _vein_opacity, birth, resourceCoef, veinVectors, veinVectorTypes, ref veinVectorCount);
            GS2.Log("Assigned Veins. Done Generating Veins");
        }

        private static void AddVeinsToPlanet(
            PlanetData planet,
            System.Random random,
            float num2point1fdivbyplanetradius,
            float[] _vein_counts,
            float[] _vein_opacity,
            bool birth,
            float resourceCoef,
            Vector3[] veinVectors,
            EVeinType[] veinVectorTypes,
            ref int veinVectorCount)
        {
            InitializePlanetVeins(planet, veinVectorCount);
            List<Vector2> node_vectors = new List<Vector2>();
            bool infiniteResources = DSPGame.GameDesc.resourceMultiplier >= 99.5f;

            for (int i = 0; i < veinVectorCount; i++) // For each veingroup (patch of vein nodes)
            {
                node_vectors.Clear();
                Vector3 normalized = veinVectors[i].normalized;
                EVeinType veinType = veinVectorTypes[i];
                Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, normalized);
                Vector3 vector_right = quaternion * Vector3.right;
                Vector3 vector_forward = quaternion * Vector3.forward;
                //planet.veinGroups[i].type = veinType;
                //planet.veinGroups[i].pos = normalized;
                //planet.veinGroups[i].count = 0;
                //planet.veinGroups[i].amount = 0L;
                InitializeVeinGroup(i, veinType, normalized, planet);
                node_vectors.Add(Vector2.zero); //Add a node at the centre of the patch/group
                int max_count = Mathf.RoundToInt(_vein_counts[(int)veinType] * (float)random.Next(20, 525));
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
                int j = 0;
                while (j++ < 20) //do this 20 times
                {
                    int tmp_vecs_count = node_vectors.Count;
                    for (int m = 0; m < tmp_vecs_count; m++) //keep doing this while there are tmp_vecs to process. starting with one.
                    {
                        if (node_vectors.Count >= max_count)
                        {
                            break;
                        }
                        //GS2.Log("sqrMagnitude:" + tmp_vecs[m].sqrMagnitude);
                        if (node_vectors[m].sqrMagnitude > 36f)
                        {
                            continue; //if the tmp_vec has already been set go on to the next one?
                        }
                        double z = random.NextDouble() * Math.PI * 2.0; //random Z
                        Vector2 randomVector = new Vector2((float)Math.Cos(z), (float)Math.Sin(z)); //random x/y/z on a sphere of radius 1
                        randomVector += node_vectors[m] * 0.2f; //add 20% of the tmp_vec...first time its 0
                        randomVector.Normalize();//make the length 1
                        Vector2 vector4 = node_vectors[m] + randomVector; //vector4 is the tmp_vec thats got some randomness to it
                        bool flag5 = false;
                        for (int k = 0; k < node_vectors.Count; k++) //If there's already a vein (node) within 0.85 tiles, discard this one.
                        {
                            if ((node_vectors[k] - vector4).sqrMagnitude < 0.85f)
                            {
                                flag5 = true;
                                break;
                            }
                        }
                        if (!flag5)
                        {
                            node_vectors.Add(vector4);
                        }
                    }
                    if (node_vectors.Count >= max_count)
                    {
                        break;
                    }
                }
                //GS2.Log("tmp_vecs Count" + tmp_vecs.Count);
                int baseValue = Mathf.RoundToInt(opacity * 100000f * resourceCoef);
                if (baseValue < 20)
                {
                    baseValue = 20;
                }
                //int variance = ((basevalue >= 16000) ? 15000 : Mathf.FloorToInt((float)basevalue * 0.9375f));
                //int minValue = basevalue - variance;
                //int maxValue = basevalue + variance + 1;
                for (int k = 0; k < node_vectors.Count; k++) //Create a Vein for each of tmp_vecs
                {
                    Vector3 vector5 = (node_vectors[k].x * vector_right + node_vectors[k].y * vector_forward) * num2point1fdivbyplanetradius;
                    //vein.type = veinType;
                    //vein.groupIndex = (short)i;
                    //(short)random.Next(veinModelIndexs[veinTypeIndex], veinModelIndexs[veinTypeIndex] + veinModelCounts[veinTypeIndex]); //Choose a 3d Model for the Vein
                    //vein.amount = basevalue;// Mathf.RoundToInt((float)random.Next(minValue, maxValue) * 1.1f); //choose an amount
                    int veinAmount = baseValue;
                    if (planet.veinGroups[i].type != EVeinType.Oil)
                    {
                        veinAmount = Mathf.RoundToInt((float)veinAmount * DSPGame.GameDesc.resourceMultiplier);
                    }
                    if (veinAmount < 1)
                    {
                        veinAmount = 1;
                    }

                    if (infiniteResources && veinType != EVeinType.Oil)
                    {
                        veinAmount = 1000000000;
                    }

                    //vein.productId = veinProducts[veinTypeIndex];
                    Vector3 veinPosition = normalized + vector5; // vein.pos = normalized + vector5;
                    if (veinType == EVeinType.Oil) SnapToGrid(ref veinPosition, planet);//vein.pos = planet.aux.RawSnap(vein.pos); 

                    // vein.minerCount = 0;
                    //float height = GetSurfaceHeight(veinPosition, planet);// planetRawData.QueryHeight(vein.pos);
                    EraseVegetableAtPoint(veinPosition, planet);// planetRawData.EraseVegetableAtPoint(vein.pos);
                    veinPosition = PositionAtSurface(veinPosition, planet); // vein.pos.normalized * height; //position on surface
                    if (!IsUnderWater(veinPosition, planet)) //(planet.waterItemId == 0 || !(height < planet.radius)) //if its not underwater
                    {
                        AddVeinToPlanet(veinAmount, veinType, veinPosition, (short)i, planet);
                        //GS2.LogJson(vein);

                    }
                }
            }
            //GS2.LogJson(planet.veinAmounts);
            //GS2.LogJson(planet.veinGroups);
            node_vectors.Clear();
        }

        private static void InitializePlanetVeins(PlanetData planet, int veinVectorCount)
        {
            Array.Clear(planet.veinAmounts, 0, planet.veinAmounts.Length);
            planet.data.veinCursor = 1;
            planet.veinGroups = new PlanetData.VeinGroup[veinVectorCount];
        }

        public static void InitializeVeinGroup(int i, EVeinType veinType, Vector3 position,PlanetData planet )
        {
            planet.veinGroups[i].type = veinType;
            planet.veinGroups[i].pos = position;
            planet.veinGroups[i].count = 0;
            planet.veinGroups[i].amount = 0L;
        }
        public static void AddVeinToPlanet(int amount, EVeinType veinType, Vector3 position, short groupIndex, PlanetData planet)
        {
            //GS2.Log("Adding Vein");
            VeinData vein = new VeinData();
            vein.amount = amount;
            vein.pos = position;
            vein.type = veinType;
            vein.groupIndex = groupIndex;
            vein.minerCount = 0;
            vein.modelIndex = RandomVeinModelIndex(veinType);
            vein.productId = PlanetModelingManager.veinProducts[(int)veinType];
            planet.veinAmounts[(int)veinType] += vein.amount;
            planet.veinGroups[(int)veinType].count++;
            planet.veinGroups[(int)veinType].amount += vein.amount;
            planet.data.AddVeinData(vein); //add to the planets rawdata veinpool
        }
        public static Vector3 PositionAtSurface(Vector3 position, PlanetData planet)
        {
            return (position.normalized * GetSurfaceHeight(position, planet));
        }
        public static bool IsUnderWater(Vector3 position, PlanetData planet)
        {
            if (planet.waterItemId == 0) return false;
            if (position.magnitude < planet.radius) return true;
            return false;
        }
        public static void EraseVegetableAtPoint(Vector3 position, PlanetData planet)
        {
            planet.data.EraseVegetableAtPoint(position);
        }
        public static float GetSurfaceHeight(Vector3 position, PlanetData planet)
        {
            return planet.data.QueryHeight(position);
        }
        public static Vector3 SnapToGrid(ref Vector3 position, PlanetData planet)
        {
            return planet.aux.RawSnap(position);
        }
        public static short RandomVeinModelIndex(EVeinType veinType)
        {
            int index = (int)veinType;
            int[] veinModelIndexs = PlanetModelingManager.veinModelIndexs;
            int[] veinModelCounts = PlanetModelingManager.veinModelCounts;
            return (short)random.Next(veinModelIndexs[index], veinModelIndexs[index] + veinModelCounts[index]);

        }
        private static void CalculateVectors(PlanetData planet, System.Random random, PlanetRawData planetRawData, float num2Point1Fdivbyplanetradius, int[] _vein_spots, Vector3[] veinVectors, EVeinType[] veinVectorTypes, ref int veinVectorCount)
        {
            bool birth = planet.id == GSSettings.birthPlanetId;
            Vector3 spawnVector = InitSpawnVector(planet, random, birth);
            for (int k = 1; k < 15; k++) //for each of the vein types
            {
                //GS2.Log("For loop " + k + " " + veinVectors.Length + " " + veinVectorCount);
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
                            if ((veinVectors[m] - potentialVector).sqrMagnitude < num2Point1Fdivbyplanetradius * num2Point1Fdivbyplanetradius * either196or100forveinoroil)
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
                        //GS2.Log("Found a vector");
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
                Array.Copy(themeProto.VeinOpacity, 0, _vein_opacity, 1, Math.Min(themeProto.VeinOpacity.Length, _vein_opacity.Length - 1));
            }
            float p = InitSpecials(planet, _vein_spots, _vein_counts, _vein_opacity);
            InitRares(planet, themeProto, _vein_spots, _vein_counts, _vein_opacity, p);
        }
    }
}
