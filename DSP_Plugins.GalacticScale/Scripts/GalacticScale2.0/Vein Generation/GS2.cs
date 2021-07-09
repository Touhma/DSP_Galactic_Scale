using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class VeinAlgorithms
    {
        public static void GenerateVeinsGS2(GSPlanet gsPlanet, bool sketchOnly)
        {
            random = new GS2.Random(gsPlanet.Seed);
            InitializeFromVeinSettings(gsPlanet);
            if (GSSettings.BirthPlanet == gsPlanet && !sketchOnly)
            {
                GS2.Log("Generating birth points");
                gsPlanet.planetData.GenBirthPoints(gsPlanet.planetData.data, random.Next());// GenBirthPoints(gsPlanet);
            }

            AddSpecialVeins(gsPlanet);
            gsPlanet.veinData.Clear();
            if (sketchOnly)
            {
                return;
            }

            if (GSSettings.BirthPlanet == gsPlanet)
            {
                InitBirthVeinVectors(gsPlanet);
            }

            AddVeinsToPlanetGS2(gsPlanet, CalculateVectorsGS2(gsPlanet));
        }
        public static void InitializeFromVeinSettings(GSPlanet gsPlanet)
        {
            gsPlanet.veinSettings = GS2.ThemeLibrary[gsPlanet.Theme].VeinSettings.Clone();
            List<GSVeinType> ores = gsPlanet.veinSettings.VeinTypes;
            int[] veinSpots = new int[PlanetModelingManager.veinProtos.Length];
            foreach (GSVeinType veinGroup in ores)
            {
                veinSpots[(int)veinGroup.type]++;
            }
            gsPlanet.planetData.veinSpotsSketch = veinSpots;
        }

        private static void AddVeinsToPlanetGS2(GSPlanet gsPlanet, List<GSVeinDescriptor> veinData)
        {
            PlanetData planet = gsPlanet.planetData;
            float resourceCoef = planet.star.resourceCoef;
            bool birth = GSSettings.BirthPlanet == gsPlanet;
            if (birth)
            {
                resourceCoef *= 2f / 3f;
            }

            double planetRadiusFactor = 2.1 / gsPlanet.planetData.radius;
            InitializePlanetVeins(planet, veinData.Count);
            List<Vector2> node_vectors = new List<Vector2>();
            bool infiniteResources = DSPGame.GameDesc.resourceMultiplier >= 99.5f;
            //GS2.Warn($"Adding Veins to Planet {gsPlanet.Name} infinite:{infiniteResources} resourceMulti:{DSPGame.GameDesc.resourceMultiplier}");

            for (int i = 0; i < veinData.Count; i++) // For each veingroup (patch of vein nodes)
            {

                node_vectors.Clear();
                if (veinData[i].position == Vector3.zero)
                {
                    continue;
                }

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
                if (gsPlanet.randomizeVeinAmounts)
                {
                    veinAmount = (int)(veinAmount * (random.NextDouble() + 0.5));
                }

                if (veinAmount < 20)
                {
                    veinAmount = 20;
                }
                if (planet.veinGroups[i].type != EVeinType.Oil) veinAmount = Mathf.RoundToInt(veinAmount * DSPGame.GameDesc.resourceMultiplier);

                for (int k = 0; k < node_vectors.Count; k++)
                {
                    //GS2.Log(node_vectors[k] + " is the node_vector[k]");
                    Vector3 vector5 = (node_vectors[k].x * vector_right + node_vectors[k].y * vector_forward) * (float)planetRadiusFactor;
                    //GS2.Log("and its vector5 is " + vector5);


                    if (veinAmount < 1)
                    {
                        veinAmount = 1;
                    }

                    if (infiniteResources && veinType != EVeinType.Oil)
                    {
                        veinAmount = 1000000000;
                    }

                    Vector3 veinPosition = normalized + vector5;
                    //GS2.Log("veinPosition = " + veinPosition);
                    if (veinType == EVeinType.Oil)
                    {
                        SnapToGrid(ref veinPosition, planet);
                    }

                    EraseVegetableAtPoint(veinPosition, planet);
                    veinPosition = PositionAtSurface(veinPosition, planet);
                    if (!IsUnderWater(veinPosition, planet))
                    {
                        AddVeinToPlanet(veinAmount, veinType, veinPosition, (short)i, planet);
                    }
                }
            }
            node_vectors.Clear();
        }


        public static List<GSVeinDescriptor> DistributeVeinTypes(List<GSVeinType> veinGroups)
        {

            bool[] disabled = new bool[16];
            for (var i = 8; i < 16; i++)
            {
                if (random.NextDouble() < 0.87)
                {
                    disabled[i] = true;
                }
                else
                {
                    disabled[i] = false;
                }

                if (GS2.Force1RareChance)
                {
                    disabled[i] = false;
                }
            }
            int maxVeinGroupSize = MaxCount(veinGroups);
            List<GSVeinDescriptor> distributed = new List<GSVeinDescriptor>();
            for (var i = 0; i < maxVeinGroupSize; i++)
            {
                for (var j = 0; j < veinGroups.Count; j++)
                {
                    if (veinGroups[j].veins.Count <= i)
                    {
                        continue;
                    }

                    if (veinGroups[j].rare && disabled[(int)veinGroups[j].type])
                    {
                        continue;
                    }
                    if (veinGroups[j].veins[i].count < 0)
                    {
                        veinGroups[j].veins[i].count = random.Next(5, 25);
                    }

                    if (veinGroups[j].veins[i].richness < 0)
                    {
                        veinGroups[j].veins[i].richness = random.NextFloat();
                    }

                    distributed.Add(new GSVeinDescriptor()
                    {

                        count = veinGroups[j].veins[i].count,
                        type = veinGroups[j].type,
                        position = Vector3.zero,// veinGroups[j].veins[i].position,
                        rare = veinGroups[j].rare,
                        richness = veinGroups[j].veins[i].richness
                    });
                }
            }
            return distributed;
        }
        public static List<GSVeinDescriptor> CalculateVectorsGS2(GSPlanet gsPlanet)
        {
            //GS2.Log("Calculating Vein Vectors for " + gsPlanet.Name);
            double randomFactor = 1.0;
            if (gsPlanet.randomizeVeinCounts)
            {
                randomFactor = 0.5 + (random.NextDouble() / 2);
            }

            PlanetData planet = gsPlanet.planetData;
            double planetRadiusFactor = Math.Pow(2.1 / gsPlanet.planetData.radius, 2);
            bool birth = planet.id == GSSettings.BirthPlanetId;
            Vector3 groupVector = InitVeinGroupVector(planet, birth); //Random Vector, unless its birth planet.
            List<GSVeinDescriptor> veinGroups = DistributeVeinTypes(gsPlanet.veinSettings.VeinTypes);
            Dictionary<EVeinType, int> veinTotals = new Dictionary<EVeinType, int>();
            for (var i = 0; i < veinGroups.Count; i++)
            {
                if (gsPlanet.randomizeVeinCounts && random.NextDouble() > randomFactor && veinTotals.ContainsKey(veinGroups[i].type))
                {
                    //GS2.Log("Randomly Skipping Vein " + veinGroups[i].type + " on planet " + gsPlanet.Name + " due to 'randomizeVeinCounts:true'");
                    continue;
                }
                if (!GS2.Force1RareChance && veinGroups[i].rare && (gsPlanet.planetData.star.level + 0.1) < random.NextDouble() * random.NextDouble())
                {
                    //GS2.Log("Randomly Skipping Rare Vein " + veinGroups[i].type + " on planet " + gsPlanet.Name + " due to star level");
                    continue;
                }
                GSVeinDescriptor v = veinGroups[i];
                if (v.position != Vector3.zero)
                {
                    continue;
                }

                bool oreVein = v.type != EVeinType.Oil;
                int repeats = 0;
                Vector3 potentialVector = Vector3.zero;
                bool succeeded = false;
                while (repeats++ < 99)
                {
                    potentialVector = RandomDirection();
                    if (oreVein)
                    {
                        potentialVector += groupVector; //randomize placement in the patch
                    }

                    potentialVector.Normalize(); //make the length of the vector 1
                    float height = planet.data.QueryHeight(potentialVector);
                    if (height < planet.radius || (!oreVein && height < planet.radius + 0.5f))
                    {
                        continue;// Check for spawn point in a hollow
                    }

                    float padding = (float)planetRadiusFactor * (oreVein ? gsPlanet.veinSettings.VeinPadding * 196f : 100f);
                    if (SurfaceVectorCollisionGS2(potentialVector, veinGroups, i, padding))
                    {
                        continue;
                    }

                    succeeded = true;
                    break;
                }
                if (succeeded)
                {

                    if (!veinTotals.ContainsKey(v.type))
                    {
                        veinTotals.Add(v.type, 1);
                    }
                    else
                    {
                        veinTotals[v.type]++;
                    }

                    veinGroups[i].position = potentialVector;
                    //GS2.Log("Succeeded finding a vector =" + veinGroups[i].type + " on planet:" + gsPlanet.Name);
                }
                //else GS2.Log("Failed to find a vector for " + veinGroups[i].type + " on planet:" + gsPlanet.Name + " after 99 attemps");
            }
            //GS2.Log(gsPlanet.Name + " VeinTotals:");
            //GS2.LogJson(veinTotals);
            if (!birth) return veinGroups;
            List<GSVeinDescriptor> gsVeinDescriptorList = new List<GSVeinDescriptor>();
            gsVeinDescriptorList.Add(new GSVeinDescriptor()
            {
                count = 5,
                position = gsPlanet.planetData.birthResourcePoint0,
                rare = false,
                type = EVeinType.Iron,
                richness = 0.5f
            });
            gsVeinDescriptorList.Add(new GSVeinDescriptor()
            {
                count = 5,
                position = gsPlanet.planetData.birthResourcePoint1,
                rare = false,
                type = EVeinType.Copper,
                richness = 0.5f
            });
            gsVeinDescriptorList.AddRange((IEnumerable<GSVeinDescriptor>)veinGroups);
            return gsVeinDescriptorList;
        }
        public static bool SurfaceVectorCollisionGS2(Vector3 vector, List<GSVeinDescriptor> vectors, int processedVectorCount, float padding)
        {
            for (int m = 0; m < processedVectorCount; m++)
            {
                if ((vectors[m].position - vector).sqrMagnitude < padding)
                {
                    return true;
                }
            }

            return false;
        }
        public static void AddSpecialVeins(GSPlanet gsPlanet)
        {
            double chanceOfSpecial = 0;
            if (GSSettings.GalaxyParams.ignoreSpecials)
            {
                chanceOfSpecial = random.NextDouble() / 2 + gsPlanet.planetData.star.level / 2;
            }

            if (gsPlanet.planetData.star.type == EStarType.BlackHole || gsPlanet.planetData.star.type == EStarType.NeutronStar || random.NextDouble() < chanceOfSpecial)
            {
                AddVeinType(gsPlanet, EVeinType.Mag, Mathf.RoundToInt((gsPlanet.planetData.star.level * 10) + (10 * (float)random.NextDouble())));
            }

            if ((gsPlanet.planetData.star.type == EStarType.WhiteDwarf && (random.NextDouble() >= 0.5)) || random.NextDouble() < chanceOfSpecial)
            {
                AddVeinType(gsPlanet, EVeinType.Grat, Mathf.RoundToInt(gsPlanet.planetData.star.level * 20 * (float)random.NextDouble()));
            }

            if ((gsPlanet.planetData.star.type == EStarType.WhiteDwarf && (random.NextDouble() >= 0.5)) || random.NextDouble() < chanceOfSpecial)
            {
                AddVeinType(gsPlanet, EVeinType.Fireice, Mathf.RoundToInt(gsPlanet.planetData.star.level * 20 * (float)random.NextDouble()));
            }

            if ((gsPlanet.planetData.star.type == EStarType.WhiteDwarf && (random.NextDouble() >= 0.5)) || random.NextDouble() < chanceOfSpecial)
            {
                AddVeinType(gsPlanet, EVeinType.Diamond, Mathf.RoundToInt(gsPlanet.planetData.star.level * 10 * (float)random.NextDouble()));
            }
        }
        public static void AddVeinType(GSPlanet gsPlanet, EVeinType type, int count)
        {
            //GS2.Log("Adding to " + gsPlanet.Name + " a vein of " + type + ": x" + count);
            GSVeinType veinType = new GSVeinType(type);
            for (var i = 0; i < count; i++)
            {
                GSVein vein = new GSVein(gsPlanet, random.Next());
                //GS2.LogJson(vein);
                veinType.veins.Add(vein);
            }
            gsPlanet.veinSettings.VeinTypes.Add(veinType);
        }
    }
}
