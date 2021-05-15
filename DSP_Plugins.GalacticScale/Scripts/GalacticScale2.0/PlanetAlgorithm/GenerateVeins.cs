// PlanetAlgorithm
using GalacticScale;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Debug = System.Diagnostics.Debug;

namespace GalacticScale
{
    //opacity determines vein (node) amount its randomized around 100000*opacity
    //could this be problematic due to not being thread safe?
    public class GSPlanetVeins
    {
        public Vector3[] vectors = new Vector3[1024];
        public EVeinType[] types = new EVeinType[1024];
        public int count;
        public void Clear()
        {
            Array.Clear(vectors, 0, vectors.Length);
            Array.Clear(types, 0, types.Length);
            count = 0;
        }
    }
    public static partial class GSPlanetAlgorithm
    {
        public delegate void VeinAlgo(GSPlanet gsPlanet, bool sketchOnly);
        public static Dictionary<string, VeinAlgo> VeinAlgorithms = new Dictionary<string, VeinAlgo>() { ["Vanilla"] = GenerateVeinsVanilla, ["GS2"] = GenerateVeinsGS2 };
        public static GS2.Random random = new GS2.Random();
        public static void GenerateVeins(GSPlanet gsPlanet, bool sketchOnly)
        {
            GSTheme theme = GS2.ThemeLibrary[gsPlanet.Theme];
            VeinAlgorithms[theme.VeinSettings.VeinAlgorithm](gsPlanet, sketchOnly);
        }
        private static void GenerateVeinsVanilla(GSPlanet gsPlanet, bool sketchOnly)
        {
            //GS2.Log("GENERATEVEINS");
            random = new GS2.Random(gsPlanet.Seed);
            PlanetData planet = gsPlanet.planetData;
            ThemeProto themeProto = LDB.themes.Select(planet.theme);
            if (themeProto == null) return;
            bool birth = GSSettings.BirthPlanet == gsPlanet;
            float num2point1fdivbyplanetradius = 2.1f / planet.radius;

            InitializeFromThemeProto(planet, themeProto, out int[] _vein_spots, out float[] _vein_counts, out float[] _vein_opacity);

            if (birth && !sketchOnly) GenBirthPoints(planet);
            Vector3[] veinVectors = gsPlanet.veinData.vectors;
            EVeinType[] veinVectorTypes = gsPlanet.veinData.types;
            ref int veinVectorCount = ref gsPlanet.veinData.count;
            gsPlanet.veinData.Clear();

            if (sketchOnly) return;
            //GS2.Log("Still Going");
            if (birth) InitBirthVeinVectors(planet, veinVectors, veinVectorTypes, ref veinVectorCount);
            //GS2.Log("Initted birthveinvectors, about to calculateveinvectors");
            CalculateVectorsVanilla(planet, random, num2point1fdivbyplanetradius, _vein_spots, veinVectors, veinVectorTypes, ref veinVectorCount);
            //GS2.Log("Calculated VeinVectors, about to assignveinvectors");
            AddVeinsToPlanet(planet, random, num2point1fdivbyplanetradius, _vein_counts, _vein_opacity, birth, veinVectors, veinVectorTypes, ref veinVectorCount);
            //GS2.Log("Assigned Veins. Done Generating Veins");
        }
        private static void GenerateVeinsGS2(GSPlanet gsPlanet, bool sketchOnly)
        {
            random = new GS2.Random(gsPlanet.Seed);
            bool birth = GSSettings.BirthPlanet == gsPlanet;

            InitializeFromVeinSettings(gsPlanet);

            if (birth && !sketchOnly) GenBirthPoints(gsPlanet.planetData);
            Vector3[] veinVectors = gsPlanet.veinData.vectors;
            EVeinType[] veinVectorTypes = gsPlanet.veinData.types;
            ref int veinVectorCount = ref gsPlanet.veinData.count;
            gsPlanet.veinData.Clear();

            if (sketchOnly) return;
            if (birth) InitBirthVeinVectors(gsPlanet.planetData, veinVectors, veinVectorTypes, ref veinVectorCount);

            List<GSVeinData> veinData = CalculateVectorsGS2(gsPlanet, veinVectors, veinVectorTypes, ref veinVectorCount);
            AddVeinsToPlanetGS2(gsPlanet, veinData);
        }
        public static void InitializeFromVeinSettings(GSPlanet gsPlanet)
        {
            List<GSVeinType> ores = GS2.ThemeLibrary[gsPlanet.Theme].VeinSettings.VeinTypes;
            int[] veinSpots = new int[PlanetModelingManager.veinProtos.Length];
            foreach (GSVeinType veinGroup in ores)
            {
                veinSpots[(int)veinGroup.type]++;
            }
            gsPlanet.planetData.veinSpotsSketch = veinSpots;
        }
        private static void GenBirthPoints(PlanetData planet)
        {
            GS2.Log("GenBirthPoints");
            System.Random random = new System.Random(planet.seed);
            Pose pose;
            double n = 85.0 / planet.orbitalPeriod + (double)planet.orbitPhase / 360.0;
            int n2 = (int)(n + 0.1);
            n -= (double)n2;
            n *= Math.PI * 2.0;
            double n3 = 85.0 / planet.rotationPeriod + (double)planet.rotationPhase / 360.0;
            int n4 = (int)(n3 + 0.1);
            n3 = (n3 - (double)n4) * 360.0;
            Vector3 v = new Vector3((float)Math.Cos(n) * planet.orbitRadius, 0f, (float)Math.Sin(n) * planet.orbitRadius);
            v = Maths.QRotate(planet.runtimeOrbitRotation, v);
            if (planet.orbitAroundPlanet != null)
            {
                pose = planet.orbitAroundPlanet.PredictPose(85.0);
                v.x += pose.position.x;
                v.y += pose.position.y;
                v.z += pose.position.z;
            }
            pose = new Pose(v, planet.runtimeSystemRotation * Quaternion.AngleAxis((float)n3, Vector3.down));
            Vector3 vector = Maths.QInvRotateLF(pose.rotation, planet.star.uPosition - (VectorLF3)pose.position * 40000.0);
            vector.Normalize();
            Vector3 normalized = Vector3.Cross(vector, Vector3.up).normalized;
            Vector3 normalized2 = Vector3.Cross(normalized, vector).normalized;
            int num = 0;
            while (num++ < 256)
            {
                GS2.Log("num" + num);
                float num2 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.5f;
                float num3 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.5f;
                Vector3 vector2 = vector + num2 * normalized + num3 * normalized2;
                vector2.Normalize();
                planet.birthPoint = vector2 * (planet.realRadius + 0.2f + 1.58f);
                normalized = Vector3.Cross(vector2, Vector3.up).normalized;
                normalized2 = Vector3.Cross(normalized, vector2).normalized;
                bool flag = false;
                for (int i = 0; i < 10; i++)
                {
                    float x = (float)(random.NextDouble() * 2.0 - 1.0);
                    float y = (float)(random.NextDouble() * 2.0 - 1.0);
                    Vector2 vector3 = new Vector2(x, y).normalized * 0.1f;
                    Vector2 vector4 = -vector3;
                    float num4 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.06f;
                    float num5 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.06f;
                    vector4.x += num4;
                    vector4.y += num5;
                    Vector3 normalized3 = (vector2 + vector3.x * normalized + vector3.y * normalized2).normalized;
                    Vector3 normalized4 = (vector2 + vector4.x * normalized + vector4.y * normalized2).normalized;
                    planet.birthResourcePoint0 = normalized3.normalized;
                    planet.birthResourcePoint1 = normalized4.normalized;
                    float num6 = planet.realRadius + 0.2f;


                    if (planet.data.QueryHeight(vector2) > num6 && planet.data.QueryHeight(normalized3) > num6 && planet.data.QueryHeight(normalized4) > num6)
                    {
                        Vector3 vpos = normalized3 + normalized * 0.03f;
                        Vector3 vpos2 = normalized3 - normalized * 0.03f;
                        Vector3 vpos3 = normalized3 + normalized2 * 0.03f;
                        Vector3 vpos4 = normalized3 - normalized2 * 0.03f;
                        Vector3 vpos5 = normalized4 + normalized * 0.03f;
                        Vector3 vpos6 = normalized4 - normalized * 0.03f;
                        Vector3 vpos7 = normalized4 + normalized2 * 0.03f;
                        Vector3 vpos8 = normalized4 - normalized2 * 0.03f;
                        if (planet.data.QueryHeight(vpos) > num6 && planet.data.QueryHeight(vpos2) > num6 && planet.data.QueryHeight(vpos3) > num6 && planet.data.QueryHeight(vpos4) > num6 && planet.data.QueryHeight(vpos5) > num6 && planet.data.QueryHeight(vpos6) > num6 && planet.data.QueryHeight(vpos7) > num6 && planet.data.QueryHeight(vpos8) > num6)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    break;
                }
            }
            GS2.Log("Finished Birthpoints");
        }
        private static void AddVeinsToPlanet(
            PlanetData planet,
            System.Random random,
            float num2point1fdivbyplanetradius,
            float[] _vein_counts,
            float[] _vein_opacity,
            bool birth,
            Vector3[] veinVectors,
            EVeinType[] veinVectorTypes,
            ref int veinVectorCount)
        {
            float resourceCoef = planet.star.resourceCoef;
            if (birth) resourceCoef *= 2f / 3f;
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
                InitializeVeinGroup(i, veinType, normalized, planet);
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
                    if (planet.veinGroups[i].type != EVeinType.Oil) veinAmount = Mathf.RoundToInt(veinAmount * DSPGame.GameDesc.resourceMultiplier);
                    if (veinAmount < 1) veinAmount = 1;
                    if (infiniteResources && veinType != EVeinType.Oil) veinAmount = 1000000000;

                    Vector3 veinPosition = normalized + vector5;
                    //GS2.Log("veinPosition = " + veinPosition);
                    if (veinType == EVeinType.Oil) SnapToGrid(ref veinPosition, planet);

                    EraseVegetableAtPoint(veinPosition, planet);
                    veinPosition = PositionAtSurface(veinPosition, planet);
                    if (!IsUnderWater(veinPosition, planet)) AddVeinToPlanet(veinAmount, veinType, veinPosition, (short)i, planet);
                }
            }
            node_vectors.Clear();
        }
        private static void AddVeinsToPlanetGS2( GSPlanet gsPlanet, List<GSVeinData> veinData )
        {
            PlanetData planet = gsPlanet.planetData;
            float resourceCoef = planet.star.resourceCoef;
            bool birth = GSSettings.BirthPlanet == gsPlanet;
            if (birth) resourceCoef *= 2f / 3f;
            double tileSize = 2.1 / gsPlanet.planetData.radius;
            InitializePlanetVeins(planet, veinData.Count);
            List<Vector2> node_vectors = new List<Vector2>();
            bool infiniteResources = DSPGame.GameDesc.resourceMultiplier >= 99.5f;

            for (int i = 0; i < veinData.Count; i++) // For each veingroup (patch of vein nodes)
            {
                node_vectors.Clear();
                if (veinData[i].position == Vector3.zero) continue;
                Vector3 normalized = veinData[i].position.normalized;
                EVeinType veinType = veinData[i].type;
                Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, normalized);
                Vector3 vector_right = quaternion * Vector3.right;
                Vector3 vector_forward = quaternion * Vector3.forward;
                InitializeVeinGroup(i, veinType, normalized, planet);
                node_vectors.Add(Vector2.zero);
                if (veinType == EVeinType.Oil)
                {
                    veinData[i].count = 1;
                }
                GenerateNodeVectors(node_vectors, veinData[i].count);

                int veinAmount = Mathf.RoundToInt(veinData[i].richness * 100000f * resourceCoef);
                if (veinAmount < 20) veinAmount = 20;

                for (int k = 0; k < node_vectors.Count; k++)
                {
                    //GS2.Log(node_vectors[k] + " is the node_vector[k]");
                    Vector3 vector5 = (node_vectors[k].x * vector_right + node_vectors[k].y * vector_forward) * (float)tileSize;
                    //GS2.Log("and its vector5 is " + vector5);
                    if (planet.veinGroups[i].type != EVeinType.Oil) veinAmount = Mathf.RoundToInt(veinAmount * DSPGame.GameDesc.resourceMultiplier);
                    if (veinAmount < 1) veinAmount = 1;
                    if (infiniteResources && veinType != EVeinType.Oil) veinAmount = 1000000000;

                    Vector3 veinPosition = normalized + vector5;
                    //GS2.Log("veinPosition = " + veinPosition);
                    if (veinType == EVeinType.Oil) SnapToGrid(ref veinPosition, planet);

                    EraseVegetableAtPoint(veinPosition, planet);
                    veinPosition = PositionAtSurface(veinPosition, planet);
                    if (!IsUnderWater(veinPosition, planet)) AddVeinToPlanet(veinAmount, veinType, veinPosition, (short)i, planet);
                }
            }
            node_vectors.Clear();
        }

        private static void GenerateNodeVectors(List<Vector2> node_vectors, int max_count)
        {
            int j = 0;
            while (j++ < 20) //do this 20 times
            {
                int tmp_vecs_count = node_vectors.Count;
                for (int m = 0; m < tmp_vecs_count; m++) //keep doing this while there are tmp_vecs to process. starting with one.
                {
                    if (node_vectors.Count >= max_count) break;
                    if (node_vectors[m].sqrMagnitude > 36f) continue; //if the tmp_vec has already been set go on to the next one?

                    double z = random.NextDouble() * Math.PI * 2.0; //random Z
                    Vector2 randomVector = new Vector2((float)Math.Cos(z), (float)Math.Sin(z)); //random x/y/z on a sphere of radius 1
                    randomVector += node_vectors[m] * 0.2f; //add 20% of the tmp_vec...first time its 0
                    randomVector.Normalize();//make the length 1
                    Vector2 vector4 = node_vectors[m] + randomVector; //vector4 is the tmp_vec thats got some randomness to it
                    bool flag5 = false;
                    for (int k = 0; k < node_vectors.Count; k++) //If there's already a vein (node) within 0.85 tiles, discard this one.
                    {
                        if ((node_vectors[k] - vector4).sqrMagnitude < 0.85)//0.85f)
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
        }

        private static void InitializePlanetVeins(PlanetData planet, int veinVectorCount)
        {
            Array.Clear(planet.veinAmounts, 0, planet.veinAmounts.Length);
            planet.data.veinCursor = 1;
            planet.veinGroups = new PlanetData.VeinGroup[veinVectorCount];
        }

        public static void InitializeVeinGroup(int i, EVeinType veinType, Vector3 position, PlanetData planet)
        {
            planet.veinGroups[i].type = veinType;
            planet.veinGroups[i].pos = position;
            planet.veinGroups[i].count = 0;
            planet.veinGroups[i].amount = 0L;
        }
        public static void AddVeinToPlanet(int amount, EVeinType veinType, Vector3 position, short groupIndex, PlanetData planet)
        {
            //GS2.Log("Adding Vein GroupIndex = "+groupIndex);
            VeinData vein = new VeinData();
            vein.amount = amount;
            vein.pos = position;
            vein.type = veinType;
            vein.groupIndex = groupIndex;
            vein.minerCount = 0;
            vein.modelIndex = RandomVeinModelIndex(veinType);
            vein.productId = PlanetModelingManager.veinProducts[(int)veinType];
            planet.veinAmounts[(int)veinType] += vein.amount;
            planet.veinGroups[groupIndex].count++;
            planet.veinGroups[groupIndex].amount += vein.amount;
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
        public static Vector3 RandomDirection()
        {
            Vector3 randomVector = Vector3.zero;
            randomVector.x = (float)random.NextDouble() * 2f - 1f; //Tiny Vector3 made up of Random numbers between -0.5 and 0.5
            randomVector.y = (float)random.NextDouble() * 2f - 1f;
            randomVector.z = (float)random.NextDouble() * 2f - 1f;
            return randomVector;
        }
        private static void CalculateVectorsVanilla(PlanetData planet, System.Random random, float num2Point1Fdivbyplanetradius, int[] _vein_spots, Vector3[] veinVectors, EVeinType[] veinVectorTypes, ref int veinVectorCount)
        {
            bool birth = planet.id == GSSettings.birthPlanetId;
            Vector3 spawnVector = InitSpawnVector(planet, birth); //Random Vector, unless its birth planet.
            for (int k = 1; k < 15; k++) //for each of the vein types
            {
                //GS2.Log("For loop " + k + " " + veinVectors.Length + " " + veinVectorCount);
                if (veinVectorCount >= veinVectors.Length) break;//If Greater than 1024 quit

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
                        float height = planet.data.QueryHeight(potentialVector);
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
                    } else
                    {
                        GS2.Warn(eVeinType + " vein unable to be placed on planet " + planet.name);
                    }
                }
            }
        }
        public static int MaxCount(List<GSVeinType> list)
        {
            int i = 0;
            foreach (GSVeinType veinType in list)
            {
                if (veinType.Count > i) i = veinType.Count;
            }
            return i;
        }
        public static List<GSVeinData> DistributeVeinTypes(List<GSVeinType> veinGroups)
        {
            int maxVeinGroupSize = MaxCount(veinGroups);
            List<GSVeinData> distributed = new List<GSVeinData>();
            //GS2.Log("veinGroups.Count = " + veinGroups.Count);
            for (var i = 0; i < maxVeinGroupSize; i++)
            {
                //GS2.Log("Testing index " + i);
                for (var j = 0; j < veinGroups.Count; j++)
                {
                    //GS2.Log("Testing type " + veinGroups[j].type + " which has " + veinGroups[j].veins.Count + " at index " + i);
                    if (veinGroups[j].veins.Count <= i) continue;
                    //GS2.Log("Adding " + veinGroups[j].type + " vein");
                    if (i == 5 && veinGroups[j].type == EVeinType.Iron) GS2.LogJson(veinGroups[j]);
                    distributed.Add(new GSVeinData()
                    {
                        count = veinGroups[j].veins[i].count,
                        type = veinGroups[j].type,
                        position = veinGroups[j].veins[i].position,
                        density = veinGroups[j].veins[i].density,
                        richness = veinGroups[j].veins[i].richness
                    });
                    //GS2.Log("Added new GSVeinData()");
                }
            }
            //GS2.Log("Done Distribution");
            return distributed;
        }
        private static List<GSVeinData> CalculateVectorsGS2(GSPlanet gsPlanet, Vector3[] veinVectors, EVeinType[] veinVectorTypes, ref int veinVectorCount)
        {
            Debug.Assert(gsPlanet.planetData != null, "gsPlanet.planetData != null");
            GSTheme gsTheme = GS2.ThemeLibrary[gsPlanet.Theme];
            PlanetData planet = gsPlanet.planetData;
            double tileSize = Math.Pow(2.1 / gsPlanet.planetData.radius, 2);
            bool birth = planet.id == GSSettings.birthPlanetId;
            Vector3 spawnVector = InitSpawnVector(planet, birth); //Random Vector, unless its birth planet.
            //GS2.Log("SpawnVector Initialized");
            List<GSVeinData> veinGroups = DistributeVeinTypes(GS2.ThemeLibrary[gsPlanet.Theme].VeinSettings.VeinTypes);
            //GS2.Log("Distributed veinGroups");
            //GS2.LogJson(veinGroups);
            for (var i = 0; i < veinGroups.Count; i++)
            {
                GSVeinData v = veinGroups[i];
                if (v.position != Vector3.zero) continue;
                bool oreVein = v.type != EVeinType.Oil;
                int repeats = 0;
                Vector3 potentialVector = Vector3.zero;
                bool succeeded = false;
                while (repeats++ < 500) //do this 200 times
                {
                    potentialVector = RandomDirection();
                    if (oreVein) potentialVector += spawnVector; //if its not an oil vein, add the random spawn vector to this tiny vector..moving the location away from spawn?
                    potentialVector.Normalize(); //make the length of the vector 1
                    float height = planet.data.QueryHeight(potentialVector);
                    if (height < planet.radius || (v.type == EVeinType.Oil && height < planet.radius + 0.5f)) continue;// Check for spawn point in a hollow

                    bool failed = false;
                    float padding = oreVein ? gsTheme.VeinSettings.VeinPadding * 196f : 100f;
                    for (int m = 0; m < veinVectorCount; m++) // for each previous veingroup
                    {
                        if ((veinVectors[m] - potentialVector).sqrMagnitude < tileSize * padding) // Check for collisiong between veingroups
                        {
                            failed = true;
                            break;
                        }
                    }
                    if (failed) continue;
                    succeeded = true;
                    break;
                }
                if (succeeded)
                {
                    veinVectors[veinVectorCount] = potentialVector;
                    veinGroups[i].position = potentialVector;
                    veinVectorTypes[veinVectorCount] = v.type;
                    veinVectorCount++;
                    if (veinVectorCount == veinVectors.Length)
                    {
                        break;
                    }
                }
                else
                {
                    GS2.Log("Couldn't find a vector");
                }
            }
            return veinGroups;
        }
        private static void InitBirthVeinVectors(PlanetData planet, Vector3[] veinVectors, EVeinType[] veinVectorTypes, ref int veinVectorCount)
        {
            veinVectorTypes[0] = EVeinType.Iron;
            ref Vector3 reference = ref veinVectors[0];
            reference = planet.birthResourcePoint0;
            veinVectorTypes[1] = EVeinType.Copper;
            ref Vector3 reference2 = ref veinVectors[1];
            reference2 = planet.birthResourcePoint1;
            veinVectorCount = 2;
        }

        private static Vector3 InitSpawnVector(PlanetData planet, bool birth)
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
                spawnVector.x = (float)random.NextDouble() * 2f - 1f;
                spawnVector.y = (float)random.NextDouble() - 0.5f;
                spawnVector.z = (float)random.NextDouble() * 2f - 1f;
                spawnVector.Normalize();
                spawnVector *= (float)(random.NextDouble() * 0.4 + 0.2);
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

        private static void InitializeFromThemeProto(PlanetData planet, ThemeProto themeProto, out int[] _vein_spots, out float[] _vein_counts, out float[] _vein_opacity)
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
            planet.veinSpotsSketch = _vein_spots;
            float p = InitSpecials(planet, _vein_spots, _vein_counts, _vein_opacity);
            InitRares(planet, themeProto, _vein_spots, _vein_counts, _vein_opacity, p);
        }
    }
}
