// PlanetAlgorithm
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale {
    //opacity determines vein (node) amount its randomized around 100000*opacity
    //could this be problematic due to not being thread safe?
    public class GSPlanetVeins {
        public Vector3[] vectors = new Vector3[1024];
        public EVeinType[] types = new EVeinType[1024];
        public int count;
        public void Clear() {
            Array.Clear(vectors, 0, vectors.Length);
            Array.Clear(types, 0, types.Length);
            count = 0;
        }
    }
    public static partial class VeinAlgorithms {
        public static GS2.Random random;// = new GS2.Random(GSSettings.Seed);

        private static void GenBirthPoints(GSPlanet gsPlanet) {
            random = new GS2.Random(GSSettings.Seed);
            PlanetData planet = gsPlanet.planetData;
            //GS2.Log("GenBirthPoints");
            //System.Random random = new System.Random(planet.seed);
            Pose pose;
            double n = 85.0 / planet.orbitalPeriod + planet.orbitPhase / 360.0;
            int n2 = (int)(n + 0.1);
            n -= n2;
            n *= Math.PI * 2.0;
            double n3 = 85.0 / planet.rotationPeriod + planet.rotationPhase / 360.0;
            int n4 = (int)(n3 + 0.1);
            n3 = (n3 - n4) * 360.0;
            Vector3 v = new Vector3((float)Math.Cos(n) * planet.orbitRadius, 0f, (float)Math.Sin(n) * planet.orbitRadius);
            v = Maths.QRotate(planet.runtimeOrbitRotation, v);
            if (planet.orbitAroundPlanet != null) {
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
            while (num++ < 256) {
                //GS2.Log("num" + num);
                float num2 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.5f;
                float num3 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.5f;
                Vector3 vector2 = vector + num2 * normalized + num3 * normalized2;
                vector2.Normalize();
                planet.birthPoint = vector2 * (planet.realRadius + 0.2f + 1.58f);
                normalized = Vector3.Cross(vector2, Vector3.up).normalized;
                normalized2 = Vector3.Cross(normalized, vector2).normalized;
                bool flag = false;
                for (int i = 0; i < 10; i++) {
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


                    if (planet.data.QueryHeight(vector2) > num6 && planet.data.QueryHeight(normalized3) > num6 && planet.data.QueryHeight(normalized4) > num6) {
                        Vector3 vpos = normalized3 + normalized * 0.03f;
                        Vector3 vpos2 = normalized3 - normalized * 0.03f;
                        Vector3 vpos3 = normalized3 + normalized2 * 0.03f;
                        Vector3 vpos4 = normalized3 - normalized2 * 0.03f;
                        Vector3 vpos5 = normalized4 + normalized * 0.03f;
                        Vector3 vpos6 = normalized4 - normalized * 0.03f;
                        Vector3 vpos7 = normalized4 + normalized2 * 0.03f;
                        Vector3 vpos8 = normalized4 - normalized2 * 0.03f;
                        if (planet.data.QueryHeight(vpos) > num6 && planet.data.QueryHeight(vpos2) > num6 && planet.data.QueryHeight(vpos3) > num6 && planet.data.QueryHeight(vpos4) > num6 && planet.data.QueryHeight(vpos5) > num6 && planet.data.QueryHeight(vpos6) > num6 && planet.data.QueryHeight(vpos7) > num6 && planet.data.QueryHeight(vpos8) > num6) {
                            flag = true;
                            break;
                        }
                    }
                }
                if (flag) {
                    break;
                }
            }
            //GS2.Log("Finished Birthpoints");
        }

        private static void GenerateNodeVectors(List<Vector2> node_vectors, int max_count) {
            //random = new GS2.Random(GSSettings.Seed);
            int j = 0;
            while (j++ < 20) //do this 20 times
            {
                int tmp_vecs_count = node_vectors.Count;
                for (int m = 0; m < tmp_vecs_count; m++) //keep doing this while there are tmp_vecs to process. starting with one.
                {
                    if (node_vectors.Count >= max_count) {
                        break;
                    }

                    if (node_vectors[m].sqrMagnitude > 36f) {
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
                        if ((node_vectors[k] - vector4).sqrMagnitude < 0.85)//0.85f)
                        {
                            flag5 = true;
                            break;
                        }
                    }
                    if (!flag5) {
                        node_vectors.Add(vector4);
                    }
                }
                if (node_vectors.Count >= max_count) {
                    break;
                }
            }
        }

        private static void InitializePlanetVeins(PlanetData planet, int veinVectorCount) {
            Array.Clear(planet.veinAmounts, 0, planet.veinAmounts.Length);
            planet.data.veinCursor = 1;
            planet.veinGroups = new PlanetData.VeinGroup[veinVectorCount];
        }

        public static void InitializeVeinGroup(int i, EVeinType veinType, Vector3 position, PlanetData planet) {
            planet.veinGroups[i].type = veinType;
            planet.veinGroups[i].pos = position;
            planet.veinGroups[i].count = 0;
            planet.veinGroups[i].amount = 0L;
        }
        public static void AddVeinToPlanet(int amount, EVeinType veinType, Vector3 position, short groupIndex, PlanetData planet) {
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
        public static Vector3 PositionAtSurface(Vector3 position, PlanetData planet) => (position.normalized * GetSurfaceHeight(position, planet));
        public static bool IsUnderWater(Vector3 position, PlanetData planet) {
            if (planet.waterItemId == 0) {
                return false;
            }

            if (position.magnitude < planet.radius) {
                return true;
            }

            return false;
        }
        public static void EraseVegetableAtPoint(Vector3 position, PlanetData planet) => planet.data.EraseVegetableAtPoint(position);
        public static float GetSurfaceHeight(Vector3 position, PlanetData planet) => planet.data.QueryHeight(position);
        public static Vector3 SnapToGrid(ref Vector3 position, PlanetData planet) => planet.aux.RawSnap(position);
        public static short RandomVeinModelIndex(EVeinType veinType) {
            int index = (int)veinType;
            int[] veinModelIndexs = PlanetModelingManager.veinModelIndexs;
            int[] veinModelCounts = PlanetModelingManager.veinModelCounts;
            return (short)random.Next(veinModelIndexs[index], veinModelIndexs[index] + veinModelCounts[index]);

        }
        public static Vector3 RandomDirection() {
            //random = new GS2.Random(GSSettings.Seed);
            Vector3 randomVector = Vector3.zero;
            randomVector.x = (float)random.NextDouble() * 2f - 1f; //Tiny Vector3 made up of Random numbers between -0.5 and 0.5
            randomVector.y = (float)random.NextDouble() * 2f - 1f;
            randomVector.z = (float)random.NextDouble() * 2f - 1f;
            return randomVector;
        }

        public static int MaxCount(List<GSVeinType> list) {
            int i = 0;
            foreach (GSVeinType veinType in list) {
                if (veinType.Count > i) {
                    i = veinType.Count;
                }
            }
            return i;
        }
        public static bool SurfaceVectorCollision(Vector3 vector, Vector3[] vectors, int veinVectorCount, float padding) {
            for (int m = 0; m < veinVectorCount; m++) {
                if ((vectors[m] - vector).sqrMagnitude < padding) {
                    return true;
                }
            }

            return false;
        }
        private static void InitBirthVeinVectors(GSPlanet gsPlanet) {
            PlanetData planet = gsPlanet.planetData;
            gsPlanet.veinData.types[0] = EVeinType.Iron;
            gsPlanet.veinData.vectors[0] = planet.birthResourcePoint0;
            gsPlanet.veinData.types[1] = EVeinType.Copper;
            gsPlanet.veinData.vectors[1] = planet.birthResourcePoint1;
            gsPlanet.veinData.count = 2;
        }

        private static Vector3 InitVeinGroupVector(PlanetData planet, bool birth) {
            //random = new GS2.Random(GSSettings.Seed);
            Vector3 groupVector;
            if (birth) {
                groupVector = planet.birthPoint;
                groupVector.Normalize();
                groupVector *= 0.75f;
            } else //randomize spawn vector
              {
                groupVector.x = (float)random.NextDouble() * 2f - 1f;
                groupVector.y = (float)random.NextDouble() - 0.5f;
                groupVector.z = (float)random.NextDouble() * 2f - 1f;
                groupVector.Normalize();
                groupVector *= (float)(random.NextDouble() * 0.4 + 0.2);
            }

            return groupVector;
        }

    }
}
