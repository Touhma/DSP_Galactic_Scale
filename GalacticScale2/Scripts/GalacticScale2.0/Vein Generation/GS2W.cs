using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GalacticScale
{
    public static partial class VeinAlgorithms
    {
        public static void GenerateVeinsGS2W(GSPlanet gsPlanet, bool sketchOnly)
        {
            random = new GS2.Random(gsPlanet.Seed);
            InitializeFromVeinSettings(gsPlanet);
            if (GSSettings.BirthPlanet == gsPlanet && !sketchOnly)
                GenBirthPoints(gsPlanet);
            AddSpecialVeins(gsPlanet);
            gsPlanet.veinData.Clear();
            if (sketchOnly) return;

            if (GSSettings.BirthPlanet == gsPlanet) InitBirthVeinVectors(gsPlanet);
            AddVeinsToPlanetGS2(gsPlanet, CalculateVectorsGS2W(gsPlanet));
        }

        private static List<GSVeinDescriptor> DistributeVeinTypesGS2W(List<GSVeinType> veinGroups)
        {
            var disabled = new bool[16];
            for (var i = 8; i < 16; i++)
            {
                if (random.NextDouble() < 0.87)
                    disabled[i] = true;
                else
                    disabled[i] = false;

                if (GS2.Config.ForceRare) disabled[i] = false;
            }

            var maxVeinGroupSize = MaxCount(veinGroups);
            var distributed = new List<GSVeinDescriptor>();
            var newList = new List<GSVeinType>();
            var r = random.Next(veinGroups.Count);
            for (var x = r; x < veinGroups.Count; x++)
            {
                newList.Add(veinGroups[x]);
            }

            for (var x = 0; x < r; x++)
            {
                newList.Add(veinGroups[x]);
            }

            veinGroups = newList;
            for (var i = 0; i < maxVeinGroupSize; i++)
            for (var j = 0; j < veinGroups.Count; j++)
            {
                if (veinGroups[j].veins.Count <= i) continue;

                if (veinGroups[j].rare && disabled[(int) veinGroups[j].type]) continue;

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

            return distributed;
        }

        private static List<GSVeinDescriptor> CalculateVectorsGS2W(GSPlanet gsPlanet, bool sketchOnly = false)
        {
            //GS2.Log("Calculating Vein Vectors for " + gsPlanet.Name);
            var randomFactor = 1.0;
            if (gsPlanet.randomizeVeinCounts) randomFactor = 0.5 + random.NextDouble() / 2;

            var planet = gsPlanet.planetData;
            var planetRadiusFactor = Math.Pow(2.1 / gsPlanet.planetData.radius, 2);
            var birth = planet.id == GSSettings.BirthPlanetId;
            Vector3 groupVector = new Vector3();
            if (!sketchOnly) groupVector = InitVeinGroupVector(planet, birth); //Random Vector, unless its birth planet.
            var veinGroups = DistributeVeinTypesGS2R(gsPlanet.veinSettings.VeinTypes);
            var veinTotals = new Dictionary<EVeinType, int>();
            for (var i = 0; i < veinGroups.Count; i++)
            {
                if (gsPlanet.randomizeVeinCounts && random.NextDouble() > randomFactor &&
                    veinTotals.ContainsKey(veinGroups[i].type))
                    //GS2.Log("Randomly Skipping Vein " + veinGroups[i].type + " on planet " + gsPlanet.Name + " due to 'randomizeVeinCounts:true'");
                    continue;

                if (!GS2.Config.ForceRare && veinGroups[i].rare && gsPlanet.planetData.star.level + 0.1 <
                        random.NextDouble() * random.NextDouble())
                    //GS2.Log("Randomly Skipping Rare Vein " + veinGroups[i].type + " on planet " + gsPlanet.Name + " due to star level");
                    continue;
                if (sketchOnly)
                {
                    // GS2.Log("*");
                    gsPlanet.planetData.veinSpotsSketch[(int)veinGroups[i].type]++;
                    // GS2.Log("*");
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
                    //if (height < planet.radius || !oreVein && height < planet.radius + 0.5f)
                    //    continue; // Check for spawn point in a hollow

                    var padding = (float) planetRadiusFactor *
                                  (oreVein ? gsPlanet.veinSettings.VeinPadding * 196f : 100f);
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
                    //GS2.Log("Succeeded finding a vector =" + veinGroups[i].type + " on planet:" + gsPlanet.Name);
                }
                //else GS2.Log("Failed to find a vector for " + veinGroups[i].type + " on planet:" + gsPlanet.Name + " after 99 attemps");
            }

            //GS2.Log(gsPlanet.Name + " VeinTotals:");
            //GS2.LogJson(veinTotals);
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


    }
}