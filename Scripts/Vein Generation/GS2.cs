using System;
using System.Collections.Generic;
using System.Linq;
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
                GenBirthPoints(gsPlanet);
            AddSpecialVeins(gsPlanet);
            gsPlanet.veinData.Clear();
            if (sketchOnly)
            {
                gsPlanet.planetData.veinSpotsSketch = new int[PlanetModelingManager.veinProtos.Length];
                CalculateVectorsGS2(gsPlanet, true);
                return;
            }

            if (GSSettings.BirthPlanet == gsPlanet) InitBirthVeinVectors(gsPlanet);
            AddVeinsToPlanetGS2(gsPlanet, CalculateVectorsGS2(gsPlanet));
        }

        private static void InitializeFromVeinSettings(GSPlanet gsPlanet)
        {
            // GS2.Warn($"Initializing Veins for Theme { gsPlanet.Theme}");
            // GS2.WarnJson(gsPlanet?.veinSettings);
            if (gsPlanet?.veinSettings == null)
            {
                // GS2.Log("Cloning veinsettings for planet");
                if (!GSSettings.ThemeLibrary.ContainsKey(gsPlanet.Theme))
                {
                    GS2.Warn($"{gsPlanet.Theme} not found in themelibrary. ThemeLibrary Contents:");
                    GS2.WarnJson(GSSettings.ThemeLibrary.Keys.ToList());
                }

                if (gsPlanet.veinSettings == null || gsPlanet.veinSettings == new GSVeinSettings()) gsPlanet.veinSettings = GSSettings.ThemeLibrary.Find(gsPlanet.Theme).VeinSettings.Clone();
            }

            List<GSVeinType> ores = gsPlanet.veinSettings.VeinTypes;
            var veinSpots = new int[PlanetModelingManager.veinProtos.Length];
            foreach (var veinGroup in ores) veinSpots[(int)veinGroup.type]++;

            gsPlanet.planetData.veinSpotsSketch = veinSpots;
        }

        private static void AddVeinsToPlanetGS2(GSPlanet gsPlanet, List<GSVeinDescriptor> veinData)
        {
            var planet = gsPlanet.planetData;
            var resourceCoef = planet.star.resourceCoef;
            var planetRadiusFactor = 2.1 / gsPlanet.planetData.radius;
            InitializePlanetVeins(planet, veinData.Count);
            var nodeVectors = new List<Vector2>();
            var infiniteResources = DSPGame.GameDesc.resourceMultiplier >= 99.5f;
            // GS2.Warn($"Adding Veins to Planet {gsPlanet.Name} infinite:{infiniteResources} resourceMulti:{DSPGame.GameDesc.resourceMultiplier}");

            for (var i = 0; i < veinData.Count; i++) // For each veingroup (patch of vein nodes)
            {
                // GS2.Warn("Adding VeinGroup");
                nodeVectors.Clear();
                if (veinData[i].position == Vector3.zero) continue;

                var normalized = veinData[i].position.normalized;
                var veinType = veinData[i].type;
                var quaternion = Quaternion.FromToRotation(Vector3.up, normalized);
                var vectorRight = quaternion * Vector3.right;
                var vectorForward = quaternion * Vector3.forward;
                InitializeVeinGroup(i, veinType, normalized, planet);
                nodeVectors.Add(Vector2.zero);
                if (veinType == EVeinType.Oil) veinData[i].count = 1;

                GenerateNodeVectors(nodeVectors, veinData[i].count);
                // GS2.Warn("Vectors generated");
                var veinAmount = Mathf.RoundToInt(veinData[i].richness * 100000f * resourceCoef);
                if (gsPlanet.randomizeVeinAmounts) veinAmount = (int)(veinAmount * (random.NextDouble() + 0.5));

                if (veinType == EVeinType.Oil) veinAmount *= 2;

                if (veinAmount < 20) veinAmount = 20;

                if (planet.veinGroups[i].type != EVeinType.Oil)
                    veinAmount = Mathf.RoundToInt(veinAmount * DSPGame.GameDesc.resourceMultiplier);
                // GS2.Warn($"NodeVectorCount:{nodeVectors.Count}");
                for (var k = 0; k < nodeVectors.Count; k++)
                {
                    // if (gsPlanet.Theme == "MoltenWorld") GS2.Warn(veinType.ToString());
                    //GS2.Log(node_vectors[k] + " is the node_vector[k]");
                    var vector5 = (nodeVectors[k].x * vectorRight + nodeVectors[k].y * vectorForward) * (float)planetRadiusFactor;
                    //GS2.Log("and its vector5 is " + vector5);


                    if (veinAmount < 1) veinAmount = 1;

                    if (infiniteResources && veinType != EVeinType.Oil) veinAmount = 1000000000;

                    var veinPosition = normalized + vector5;
                    //GS2.Log("veinPosition = " + veinPosition);
                    if (veinType == EVeinType.Oil) SnapToGrid(ref veinPosition, planet);

                    EraseVegetableAtPoint(veinPosition, planet);
                    veinPosition = Utils.PositionAtSurface(veinPosition, gsPlanet);
                    //if (!Utils.IsUnderWater(veinPosition, gsPlanet))
                    // GS2.Warn("Adding Vein To Planet");
                    AddVeinToPlanet(veinAmount, veinType, veinPosition, (short)i, planet);
                }
            }

            nodeVectors.Clear();
        }


        private static List<GSVeinDescriptor> DistributeVeinTypes(GSPlanet gsPlanet)
        {
            List<GSVeinType> veinGroups = gsPlanet.veinSettings.VeinTypes;
            CutTheVeinGroup(ref veinGroups);
            var disabled = DisableVeins(ref gsPlanet);
            var maxVeinGroupSize = MaxCount(veinGroups);
            var distributed = new List<GSVeinDescriptor>();

            for (var i = 0; i < maxVeinGroupSize; i++)
            for (var j = 0; j < veinGroups.Count; j++)
            {
                if (veinGroups[j].veins.Count <= i) continue;

                if (veinGroups[j].rare && disabled[(int)veinGroups[j].type]) continue;

                if (veinGroups[j].veins[i].count < 0) veinGroups[j].veins[i].count = random.Next(5, 25);

                if (veinGroups[j].veins[i].richness < 0) veinGroups[j].veins[i].richness = random.NextFloat();

                distributed.Add(new GSVeinDescriptor
                {
                    count = veinGroups[j].veins[i].count,
                    type = veinGroups[j].type,
                    position = Vector3.zero, // veinGroups[j].veins[i].position,
                    rare = veinGroups[j].rare,
                    richness = veinGroups[j].veins[i].richness
                });
            }

            // GS2.Warn($"Distributing veins for planet {gsPlanet.Name} {gsPlanet.Theme}");
            // GS2.WarnJson(distributed);
            return distributed;
        }

        private static List<GSVeinDescriptor> CalculateVectorsGS2(GSPlanet gsPlanet, bool sketchOnly = false, bool PreventUnderwater = true)
        {
            var randomFactor = 1.0;
            if (gsPlanet.randomizeVeinCounts) randomFactor = 0.5 + random.NextDouble() / 2;

            var planet = gsPlanet.planetData;
            var planetRadiusFactor = Math.Pow(2.1 / gsPlanet.Radius, 2);
            var birth = planet.id == GSSettings.BirthPlanetId;
            var groupVector = new Vector3();
            if (!sketchOnly) groupVector = InitVeinGroupVector(planet, birth); //Random Vector, unless its birth planet.
            var veinGroups = DistributeVeinTypes(gsPlanet);
            var veinTotals = new Dictionary<EVeinType, int>();
            for (var i = 0; i < veinGroups.Count; i++)
            {
                if (gsPlanet.randomizeVeinCounts && random.NextDouble() > randomFactor && veinTotals.ContainsKey(veinGroups[i].type))
                    //GS2.Log("Randomly Skipping Vein " + veinGroups[i].type + " on planet " + gsPlanet.Name + " due to 'randomizeVeinCounts:true'");
                    continue;

                if (!GS2.Config.ForceRare && veinGroups[i].rare && gsPlanet.planetData.star.level + 0.1 < random.NextDouble() * random.NextDouble())
                    //GS2.Log("Randomly Skipping Rare Vein " + veinGroups[i].type + " on planet " + gsPlanet.Name + " due to star level");
                    continue;
                if (sketchOnly)
                {
                    gsPlanet.planetData.veinSpotsSketch[(int)veinGroups[i].type]++;
                    continue;
                }

                var v = veinGroups[i];
                if (v.position != Vector3.zero) continue;

                var oreVein = v.type != EVeinType.Oil;
                var repeats = 0;
                var potentialVector = Vector3.zero;
                var succeeded = false;
                while (repeats++ < 99)
                {
                    potentialVector = Utils.RandomDirection(random);
                    if (oreVein) potentialVector += groupVector; //randomize placement in the patch

                    potentialVector.Normalize(); //make the length of the vector 1
                    var height = planet.data.QueryHeight(potentialVector);
                    if (PreventUnderwater && (height < planet.radius || !oreVein && height < planet.radius + 0.5f))
                        // GS2.Log("Point is underwater!");
                        continue; // Check for spawn point in a hollow

                    var padding = (float)planetRadiusFactor * (oreVein ? gsPlanet.veinSettings.VeinPadding * 196f : 100f);
                    if (SurfaceVectorCollisionGS2(potentialVector, veinGroups, i, padding)) continue;

                    succeeded = true;
                    break;
                }

                if (succeeded)
                {
                    if (!veinTotals.ContainsKey(v.type))
                        veinTotals.Add(v.type, 1);
                    else
                        veinTotals[v.type]++;

                    veinGroups[i].position = potentialVector;
                    // GS2.Log("Succeeded finding a vector =" + veinGroups[i].type + " on planet:" + gsPlanet.Name);
                }
                // else GS2.Log("Failed to find a vector for " + veinGroups[i].type + " on planet:" + gsPlanet.Name + " after 99 attemps");
            }

            if (sketchOnly) return null;
            if (!birth) return veinGroups;
            var gsVeinDescriptorList = new List<GSVeinDescriptor>();
            var ironCount = 6;
            var ironRichness = 0.5f;
            var copperCount = 6;
            var copperRichness = 0.5f;
            if (GSSettings.BirthIron != null)
            {
                ironCount = GSSettings.BirthIron.count;
                ironRichness = GSSettings.BirthIron.richness;
            }

            if (GSSettings.BirthCopper != null)
            {
                copperCount = GSSettings.BirthCopper.count;
                copperRichness = GSSettings.BirthCopper.richness;
            }

            gsVeinDescriptorList.Add(new GSVeinDescriptor
            {
                count = ironCount,
                position = gsPlanet.planetData.birthResourcePoint0,
                rare = false,
                type = EVeinType.Iron,
                richness = ironRichness
            });
            gsVeinDescriptorList.Add(new GSVeinDescriptor
            {
                count = copperCount,
                position = gsPlanet.planetData.birthResourcePoint1,
                rare = false,
                type = EVeinType.Copper,
                richness = copperRichness
            });
            gsVeinDescriptorList.AddRange(veinGroups);
            return gsVeinDescriptorList;
        }

        private static bool SurfaceVectorCollisionGS2(Vector3 vector, List<GSVeinDescriptor> vectors, int processedVectorCount, float padding)
        {
            for (var m = 0; m < processedVectorCount; m++)
                if ((vectors[m].position - vector).sqrMagnitude < padding)
                    return true;

            return false;
        }

        private static void AddSpecialVeins(GSPlanet gsPlanet)
        {
            if (gsPlanet.rareChance == 0) return;
            var isBlackHole = gsPlanet.planetData.star.type == EStarType.BlackHole || gsPlanet.planetData.star.type == EStarType.NeutronStar;
            var isWhiteDwarf = gsPlanet.planetData.star.type == EStarType.WhiteDwarf;
            var star = GS2.GetGSStar(gsPlanet.planetData.star);
            if (star.BinaryCompanion != null)
            {
                var BinaryCompanion = GS2.GetGSStar(star.BinaryCompanion);
                if (BinaryCompanion != null)
                {
                    if (BinaryCompanion.Type == EStarType.BlackHole) isBlackHole = true;
                    if (BinaryCompanion.Type == EStarType.NeutronStar) isBlackHole = true;
                    if (BinaryCompanion.Type == EStarType.WhiteDwarf) isWhiteDwarf = true;
                }
            }

            // double chanceOfSpecial = 0;
            // if (GSSettings.GalaxyParams.ignoreSpecials) chanceOfSpecial = 1;
            var forceSpecials = GSSettings.GalaxyParams.forceSpecials || GS2.Config.ForceRare;
            //chanceOfSpecial = random.NextDouble() / 2 + gsPlanet.planetData.star.level / 2;

            if (isBlackHole || forceSpecials)
                AddVeinType(gsPlanet, EVeinType.Mag, Mathf.RoundToInt(gsPlanet.planetData.star.level * 10 + 10 * (float)random.NextDouble()));

            if (isWhiteDwarf && random.NextDouble() >= 0.5 || forceSpecials)
                AddVeinType(gsPlanet, EVeinType.Grat, Mathf.RoundToInt(gsPlanet.planetData.star.level * 20 * (float)random.NextDouble()));

            if (isWhiteDwarf && random.NextDouble() >= 0.5 || forceSpecials)
                AddVeinType(gsPlanet, EVeinType.Fireice, Mathf.RoundToInt(gsPlanet.planetData.star.level * 20 * (float)random.NextDouble()));

            if (isWhiteDwarf && random.NextDouble() >= 0.5 || forceSpecials)
                AddVeinType(gsPlanet, EVeinType.Diamond, Mathf.RoundToInt(gsPlanet.planetData.star.level * 10 * (float)random.NextDouble()));
        }

        private static void AddVeinType(GSPlanet gsPlanet, EVeinType type, int count)
        {
            //GS2.Log("Adding to " + gsPlanet.Name + " a vein of " + type + ": x" + count);
            var veinType = new GSVeinType(type);
            for (var i = 0; i < count; i++)
            {
                var vein = new GSVein(gsPlanet, random.Next());
                //GS2.LogJson(vein);
                veinType.veins.Add(vein);
            }

            gsPlanet.veinSettings.VeinTypes.Add(veinType);
        }
    }
}