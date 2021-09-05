// PlanetAlgorithm

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    //opacity determines vein (node) amount its randomized around 100000*opacity
    //could this be problematic due to not being thread safe?
    public class GSPlanetVeins
    {
        public readonly EVeinType[] types = new EVeinType[1024];
        public readonly Vector3[] vectors = new Vector3[1024];
        public int count;

        public void Clear()
        {
            Array.Clear(vectors, 0, vectors.Length);
            Array.Clear(types, 0, types.Length);
            count = 0;
        }
    }

    public static partial class VeinAlgorithms
    {
        private static GS2.Random random;

        private static void GenBirthPoints(GSPlanet gsPlanet)
        {
            random = new GS2.Random(GSSettings.Seed);
            var planet = gsPlanet.planetData;
            //GS2.Log("GenBirthPoints");
            Pose pose;
            var n = 85.0 / planet.orbitalPeriod + planet.orbitPhase / 360.0;
            var n2 = (int)(n + 0.1);
            n -= n2;
            n *= Math.PI * 2.0;
            var n3 = 85.0 / planet.rotationPeriod + planet.rotationPhase / 360.0;
            var n4 = (int)(n3 + 0.1);
            n3 = (n3 - n4) * 360.0;
            var v = new Vector3((float)Math.Cos(n) * planet.orbitRadius, 0f, (float)Math.Sin(n) * planet.orbitRadius);
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
            var normalized = Vector3.Cross(vector, Vector3.up).normalized;
            var normalized2 = Vector3.Cross(normalized, vector).normalized;
            var num = 0;
            while (num++ < 256)
            {
                var num2 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.5f;
                var num3 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.5f;
                var vector2 = vector + num2 * normalized + num3 * normalized2;
                vector2.Normalize();
                planet.birthPoint = vector2 * (planet.realRadius + 0.2f + 1.58f);
                normalized = Vector3.Cross(vector2, Vector3.up).normalized;
                normalized2 = Vector3.Cross(normalized, vector2).normalized;
                var flag = false;
                for (var i = 0; i < 10; i++)
                {
                    var x = (float)(random.NextDouble() * 2.0 - 1.0);
                    var y = (float)(random.NextDouble() * 2.0 - 1.0);
                    var vector3 = new Vector2(x, y).normalized * 0.1f;
                    var vector4 = -vector3;
                    var num4 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.06f;
                    var num5 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.06f;
                    vector4.x += num4;
                    vector4.y += num5;
                    var normalized3 = (vector2 + vector3.x * normalized + vector3.y * normalized2).normalized;
                    var normalized4 = (vector2 + vector4.x * normalized + vector4.y * normalized2).normalized;
                    planet.birthResourcePoint0 = normalized3.normalized;
                    planet.birthResourcePoint1 = normalized4.normalized;
                    var num6 = planet.realRadius + 0.2f;


                    if (planet.data.QueryHeight(vector2) > num6 && planet.data.QueryHeight(normalized3) > num6 && planet.data.QueryHeight(normalized4) > num6)
                    {
                        var vpos = normalized3 + normalized * 0.03f;
                        var vpos2 = normalized3 - normalized * 0.03f;
                        var vpos3 = normalized3 + normalized2 * 0.03f;
                        var vpos4 = normalized3 - normalized2 * 0.03f;
                        var vpos5 = normalized4 + normalized * 0.03f;
                        var vpos6 = normalized4 - normalized * 0.03f;
                        var vpos7 = normalized4 + normalized2 * 0.03f;
                        var vpos8 = normalized4 - normalized2 * 0.03f;
                        if (planet.data.QueryHeight(vpos) > num6 && planet.data.QueryHeight(vpos2) > num6 && planet.data.QueryHeight(vpos3) > num6 && planet.data.QueryHeight(vpos4) > num6 && planet.data.QueryHeight(vpos5) > num6 && planet.data.QueryHeight(vpos6) > num6 && planet.data.QueryHeight(vpos7) > num6 && planet.data.QueryHeight(vpos8) > num6)
                        {
                            flag = true;
                            break;
                        }
                    }
                }

                if (flag) break;
            }
            //GS2.Log("Finished Birthpoints");
        }

        private static void GenerateNodeVectors(List<Vector2> nodeVectors, int maxCount)
        {
            var j = 0;
            while (j++ < 20) //do this 20 times
            {
                var tmpVecsCount = nodeVectors.Count;
                for (var m = 0; m < tmpVecsCount; m++) //keep doing this while there are tmp_vecs to process. starting with one.
                {
                    if (nodeVectors.Count >= maxCount) break;

                    if (nodeVectors[m].sqrMagnitude > 36f)
                        continue; //if the tmp_vec has already been set go on to the next one?

                    var z = random.NextDouble() * Math.PI * 2.0; //random Z
                    var randomVector = new Vector2((float)Math.Cos(z), (float)Math.Sin(z)); //random x/y/z on a sphere of radius 1
                    randomVector += nodeVectors[m] * 0.2f; //add 20% of the tmp_vec...first time its 0
                    randomVector.Normalize(); //make the length 1
                    var vector4 = nodeVectors[m] + randomVector; //vector4 is the tmp_vec thats got some randomness to it
                    var flag5 = false;
                    foreach (var t in nodeVectors)
                        if ((t - vector4).sqrMagnitude < 0.85) //0.85f)
                        {
                            flag5 = true;
                            break;
                        }

                    if (!flag5) nodeVectors.Add(vector4);
                }

                if (nodeVectors.Count >= maxCount) break;
            }
        }

        private static void InitializePlanetVeins(PlanetData planet, int veinVectorCount)
        {
            Array.Clear(planet.veinAmounts, 0, planet.veinAmounts.Length);
            planet.data.veinCursor = 1;
            planet.veinGroups = new PlanetData.VeinGroup[veinVectorCount];
        }

        private static void InitializeVeinGroup(int i, EVeinType veinType, Vector3 position, PlanetData planet)
        {
            planet.veinGroups[i].type = veinType;
            planet.veinGroups[i].pos = position;
            planet.veinGroups[i].count = 0;
            planet.veinGroups[i].amount = 0L;
        }

        private static void AddVeinToPlanet(int amount, EVeinType veinType, Vector3 position, short groupIndex, PlanetData planet)
        {
            //GS2.Log("Adding Vein GroupIndex = "+groupIndex);
            var vein = new VeinData
            {
                amount = amount,
                pos = position,
                type = veinType,
                groupIndex = groupIndex,
                minerCount = 0,
                modelIndex = RandomVeinModelIndex(veinType),
                productId = PlanetModelingManager.veinProducts[(int)veinType]
            };
            planet.veinAmounts[(int)veinType] += vein.amount;
            planet.veinSpotsSketch[(int)veinType]++;
            planet.veinGroups[groupIndex].count++;
            planet.veinGroups[groupIndex].amount += vein.amount;
            planet.data.AddVeinData(vein); //add to the planets rawdata veinpool
            // GS2.Warn("Added");
        }

        private static void EraseVegetableAtPoint(Vector3 position, PlanetData planet)
        {
            planet.data.EraseVegetableAtPoint(position);
        }

        private static void SnapToGrid(ref Vector3 position, PlanetData planet)
        {
            planet.aux.RawSnap(position);
        }

        private static short RandomVeinModelIndex(EVeinType veinType)
        {
            var index = (int)veinType;
            var veinModelIndexs = PlanetModelingManager.veinModelIndexs;
            var veinModelCounts = PlanetModelingManager.veinModelCounts;
            return (short)random.Next(veinModelIndexs[index], veinModelIndexs[index] + veinModelCounts[index]);
        }

        private static int MaxCount(List<GSVeinType> list)
        {
            var i = 0;
            foreach (var veinType in list)
                if (veinType.Count > i)
                    i = veinType.Count;
            return i;
        }

        private static void InitBirthVeinVectors(GSPlanet gsPlanet)
        {
            GS2.Warn("Initializing Birth Veins");
            var planet = gsPlanet.planetData;
            gsPlanet.veinData.types[0] = EVeinType.Iron;
            gsPlanet.veinData.vectors[0] = planet.birthResourcePoint0;
            gsPlanet.veinData.types[1] = EVeinType.Copper;
            gsPlanet.veinData.vectors[1] = planet.birthResourcePoint1;
            gsPlanet.veinData.count = 2;
        }

        private static Vector3 InitVeinGroupVector(PlanetData planet, bool birth)
        {
            //random = new GS2.Random(GSSettings.Seed);
            Vector3 groupVector;
            if (birth)
            {
                groupVector = planet.birthPoint;
                groupVector.Normalize();
                groupVector *= 0.75f;
            }
            else //randomize spawn vector
            {
                groupVector.x = (float)random.NextDouble() * 2f - 1f;
                groupVector.y = (float)random.NextDouble() - 0.5f;
                groupVector.z = (float)random.NextDouble() * 2f - 1f;
                groupVector.Normalize();
                groupVector *= (float)(random.NextDouble() * 0.4 + 0.2);
            }

            return groupVector;
        }

        private static void CutTheVeinGroup(ref List<GSVeinType> veinGroups)
        {
            if (veinGroups.Count < 1) return;
            var newList = new List<GSVeinType>();
            var r = random.Next(veinGroups.Count);
            for (var x = r; x < veinGroups.Count; x++) newList.Add(veinGroups[x]);
            for (var x = 0; x < r; x++) newList.Add(veinGroups[x]);
            veinGroups = newList;
        }

        private static bool[] DisableVeins(ref GSPlanet gsPlanet)
        {
            var disabled = new bool[16];
            for (var i = 8; i < 16; i++)
            {
                switch (gsPlanet.rareChance)
                {
                    case 0f:
                        disabled[i] = true;
                        break;
                    case -1f:
                        if (random.NextDouble() < 0.87) disabled[i] = true;
                        else disabled[i] = false;
                        break;
                    default:
                        if (random.NextPick(gsPlanet.rareChance)) disabled[i] = false;
                        else disabled[i] = true;
                        break;
                }

                if (GS2.Config.ForceRare) disabled[i] = false;
            }

            // GS2.Warn($"{gsPlanet.Name} chance of rare = {gsPlanet.rareChance} where -1f is default (13% chance of rare) and 0 is 0% chance, 1 is 100% chance");
            // GS2.WarnJson(disabled);
            return disabled;
        }
    }
}