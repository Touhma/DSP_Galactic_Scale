using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetModelingManager))]
    public class PatchOnPlanetModelingManager : MonoBehaviour {
        [HarmonyPrefix]
        [HarmonyPatch("ModelingPlanetMain")]
        public static bool ModelingPlanetMain(PlanetData planet,
            ref Camera ___heightmapCamera,
            ref List<Mesh> ___tmpMeshList,
            ref PlanetData ___currentModelingPlanet,
            ref int ___currentModelingStage,
            ref int ___currentModelingSeamNormal,
            ref PlanetData ___currentFactingPlanet,
            ref int ___currentFactingStage,
            ref List<MeshRenderer> ___tmpMeshRendererList,
            ref List<MeshCollider> ___tmpMeshColliderList,
            ref Collider ___tmpOceanCollider,
            ref List<Vector3> ___tmpVerts,
            ref List<Vector3> ___tmpNorms,
            ref List<Vector4> ___tmpTgnts,
            ref VegeProto[] ___vegeProtos,
            ref List<Vector2> ___tmpUvs,
            ref List<Vector4> ___tmpUv2s,
            ref List<int> ___tmpTris,
            ref GameObject ___tmpPlanetGameObject,
            ref GameObject ___tmpPlanetBodyGameObject,
            ref GameObject ___tmpPlanetReformGameObject,
            ref MeshRenderer ___tmpPlanetReformRenderer) {
            Patch.Debug("ModelingPlanetMain", LogLevel.Debug,
                Patch.DebugGeneral);


            var themeProto = LDB.themes.Select(planet.theme);

            planet.data.AddFactoredRadius(planet);

            Patch.Debug("Planet " + planet.name, LogLevel.Debug,
                Patch.DebugPlanetModelingManager);
            Patch.Debug("Planet Modeling Main Start :", LogLevel.Debug,
                Patch.DebugPlanetModelingManager);
            Patch.Debug("Planet radius :" + planet.radius, LogLevel.Debug,
                Patch.DebugPlanetModelingManager);
            Patch.Debug("Planet RealRadius :" + planet.realRadius, LogLevel.Debug,
                Patch.DebugPlanetModelingManager);
            Patch.Debug("Planet precision :" + planet.precision, LogLevel.Debug,
                Patch.DebugPlanetModelingManager);
            Patch.Debug("Planet Scale :" + planet.scale, LogLevel.Debug,
                Patch.DebugPlanetModelingManager);
            Patch.Debug("Planet Algo :" + planet.algoId, LogLevel.Debug,
                Patch.DebugPlanetModelingManager);
            Patch.Debug("Planet RealRadius :" + planet.realRadius, LogLevel.Debug,
                Patch.DebugPlanetModelingManager);
            Patch.Debug("Planet Scale :" + planet.scale, LogLevel.Debug,
                Patch.DebugPlanetModelingManager);
            Patch.Debug("Planet scaleFactor :" + planet.GetScaleFactored(), LogLevel.Debug,
                Patch.DebugPlanetModelingManager);
            Patch.Debug("Planet Scale :" + planet.radius, LogLevel.Debug,
                Patch.DebugPlanetModelingManager);


            //data.heightData = scaledHeightData;
            if (___currentModelingStage == 1)
            {
                ___tmpPlanetGameObject = new GameObject(planet.displayName);
                ___tmpPlanetGameObject.layer = 31;
                PlanetSimulator sim = ___tmpPlanetGameObject.AddComponent<PlanetSimulator>();
                GameMain.universeSimulator.SetPlanetSimulator(sim, planet);
                ___tmpPlanetGameObject.transform.localPosition = Vector3.zero;
                ___tmpPlanetBodyGameObject = new GameObject("Planet Body");
                ___tmpPlanetBodyGameObject.transform.SetParent(___tmpPlanetGameObject.transform, worldPositionStays: false);
                ___tmpPlanetBodyGameObject.layer = 31;
                ___tmpPlanetReformGameObject = new GameObject("Terrain Reform");
                ___tmpPlanetReformGameObject.transform.SetParent(___tmpPlanetBodyGameObject.transform, worldPositionStays: false);
                ___tmpPlanetReformGameObject.layer = 14;
                MeshFilter meshFilter = ___tmpPlanetReformGameObject.AddComponent<MeshFilter>();
                ___tmpPlanetReformRenderer = ___tmpPlanetReformGameObject.AddComponent<MeshRenderer>();
                meshFilter.sharedMesh = Configs.builtin.planetReformMesh;
                Material[] sharedMaterials = new Material[2] { planet.reformMaterial0, planet.reformMaterial1 };
                ___tmpPlanetReformRenderer.sharedMaterials = sharedMaterials;
                ___tmpPlanetReformRenderer.receiveShadows = false;
                ___tmpPlanetReformRenderer.lightProbeUsage = LightProbeUsage.Off;
                ___tmpPlanetReformRenderer.shadowCastingMode = ShadowCastingMode.Off;
                float num = (planet.realRadius + 0.2f + planet.realRadius/8000f) * 2f;
                ___tmpPlanetReformRenderer.transform.localScale = new Vector3(num, num, num);
                ___tmpPlanetReformRenderer.transform.rotation = Quaternion.identity;
                if (planet.waterItemId != 0)
                {
                    GameObject oceanSphere = Configs.builtin.oceanSphere;
                    GameObject gameObject2 = UnityEngine.Object.Instantiate(oceanSphere, ___tmpPlanetBodyGameObject.transform);
                    gameObject2.name = "Ocean Sphere";
                    gameObject2.layer = 31;
                    gameObject2.transform.localPosition = Vector3.zero;
                    gameObject2.transform.localScale = Vector3.one * ((planet.realRadius + planet.waterHeight) * 2f);
                    Renderer component = gameObject2.GetComponent<Renderer>();
                    ___tmpOceanCollider = gameObject2.GetComponent<Collider>();
                    if (component != null)
                    {
                        component.enabled = planet.oceanMaterial != null;
                        component.shadowCastingMode = ShadowCastingMode.Off;
                        component.receiveShadows = false;
                        component.lightProbeUsage = LightProbeUsage.Off;
                        component.sharedMaterial = planet.oceanMaterial;
                    }
                }
                int precision = planet.precision;
                int num2 = precision / planet.segment;
                int num3 = num2 + 1;
                for (int i = 0; i < num2; i++)
                {
                    for (int j = 0; j < num2; j++)
                    {
                        ___tmpTris.Add(i + 1 + (j + 1) * num3);
                        ___tmpTris.Add(i + (j + 1) * num3);
                        ___tmpTris.Add(i + j * num3);
                        ___tmpTris.Add(i + j * num3);
                        ___tmpTris.Add(i + 1 + j * num3);
                        ___tmpTris.Add(i + 1 + (j + 1) * num3);
                    }
                }
                ___currentModelingStage = 2;
            }else if (___currentModelingStage == 2) {
                var planetPrecisionBySegment = planet.precision / planet.segment;
                var planetPrecision = planet.precision;
                var data = planet.data;
                var planetScale = planet.scale;
                // float planetRadiusScaled = (float) (planet.radius * (double) planetScale + 0.200000002980232);
                var planetRadiusScaled = (float) (planet.radius * (double) planetScale + 0.200000002980232);
                Patch.Debug("planetRadiusScaled " + planetRadiusScaled, LogLevel.Debug,
                    Patch.DebugPlanetModelingManager);
                // planetRadiusScaled *= scaleFactor;
                Patch.Debug("planetRadiusScaled Patched " + planetRadiusScaled, LogLevel.Debug,
                    Patch.DebugPlanetModelingManager);
                var stride = data.stride;
                var num6 = 0;
                var stateOfTheGame = !GameMain.isLoading ? 2 : 3;
                var num8 = 0;
                for (var index1 = 0; index1 < 4; ++index1) {
                    var num9 = index1 % 2 * (planetPrecision + 1);
                    var num10 = index1 / 2 * (planetPrecision + 1);
                    for (var index2 = 0; index2 < planetPrecision; index2 += planetPrecisionBySegment)
                    for (var index3 = 0; index3 < planetPrecision; index3 += planetPrecisionBySegment)
                        if (num8 == 0 && num6 < ___tmpMeshList.Count) {
                            ++num6;
                        }
                        else {
                            var mesh = new Mesh();
                            ___tmpMeshList.Add(mesh);
                            ___tmpVerts.Clear();
                            ___tmpNorms.Clear();
                            ___tmpTgnts.Clear();
                            ___tmpUvs.Clear();
                            ___tmpUv2s.Clear();
                            var gameObject = new GameObject("Surface");
                            gameObject.layer = 30;
                            gameObject.transform.SetParent(___tmpPlanetBodyGameObject.transform, false);
                            for (var index4 = index2;
                                index4 <= index2 + planetPrecisionBySegment &&
                                index4 <= planetPrecision;
                                ++index4)
                            for (var index5 = index3;
                                index5 <= index3 + planetPrecisionBySegment &&
                                index5 <= planetPrecision;
                                ++index5) {
                                var num11 = num9 + index5;
                                var num12 = num10 + index4;
                                var index6 = num11 + num12 * stride;
                                var num13 = index6;
                                if (index4 == 0) {
                                    var num14 = (index1 + 3) % 4;
                                    var num15 = num14 % 2 * (planetPrecision + 1);
                                    var num16 = num14 / 2 * (planetPrecision + 1);
                                    var num17 = planetPrecision;
                                    var num18 = planetPrecision - index5;
                                    num13 = num15 + num17 + (num16 + num18) * stride;
                                }
                                else if (index5 == 0) {
                                    var num14 = (index1 + 3) % 4;
                                    var num15 = num14 % 2 * (planetPrecision + 1);
                                    var num16 = num14 / 2 * (planetPrecision + 1);
                                    var num17 = planetPrecision - index4;
                                    var num18 = planetPrecision;
                                    num13 = num15 + num17 + (num16 + num18) * stride;
                                }

                                if (index4 == planetPrecision) {
                                    var num14 = (index1 + 1) % 4;
                                    var num15 = num14 % 2 * (planetPrecision + 1);
                                    var num16 = num14 / 2 * (planetPrecision + 1);
                                    var num17 = 0;
                                    var num18 = planetPrecision - index5;
                                    num13 = num15 + num17 + (num16 + num18) * stride;
                                }
                                else if (index5 == planetPrecision) {
                                    var num14 = (index1 + 1) % 4;
                                    var num15 = num14 % 2 * (planetPrecision + 1);
                                    var num16 = num14 / 2 * (planetPrecision + 1);
                                    var num17 = planetPrecision - index4;
                                    var num18 = 0;
                                    num13 = num15 + num17 + (num16 + num18) * stride;
                                }
                                Patch.Debug("data.heightData[index6]  :  " + data.heightData[index6],
                                    LogLevel.Debug,
                                    Patch.DebugPlanetModelingManagerDeep);
                                var heightDataScaled = data.heightData[index6] * 0.01f;
                                //If GasGiant
                                if (planet.type == EPlanetType.Gas) heightDataScaled *= planetScale;
                                Patch.Debug("heightDataScaled  :  " + heightDataScaled,
                                    LogLevel.Debug,
                                    Patch.DebugPlanetModelingManagerDeep);
                                var thirdOfModLevel = data.GetModLevel(index6) * 0.3333333f;

                                Patch.Debug("thirdOfModLevel  :  " + thirdOfModLevel,
                                    LogLevel.Debug,
                                    Patch.DebugPlanetModelingManagerDeep);

                                if (thirdOfModLevel > 0.0) {
                                    //data.GetModPlane(index6)) 20000 + * 0.01f --> 200 +
                                    Patch.Debug(
                                        "data.GetModPlane(index6) :  " + data.GetModPlaneInt(index6),
                                        LogLevel.Debug,
                                        Patch.DebugPlanetModelingManagerDeep);
                                    var modPlanePatched = data.GetModPlaneInt(index6);

                                    Patch.Debug("patch modPlane:  " + modPlanePatched,
                                        LogLevel.Debug,
                                        Patch.DebugPlanetModelingManagerDeep);
                                    planetRadiusScaled = modPlanePatched * 0.01f;

                                    Patch.Debug(
                                        "planetRadiusScaled is modified :  " + planetRadiusScaled,
                                        LogLevel.Debug,
                                        Patch.DebugPlanetModelingManagerDeep);
                                }

                                // final height modification ? 
                                var finalHeight = (float) (heightDataScaled * (1.0 - thirdOfModLevel) + planetRadiusScaled * (double) thirdOfModLevel);


                                Patch.Debug("finalHeight :  " + finalHeight, LogLevel.Debug,
                                    Patch.DebugPlanetModelingManagerDeep);

                                var vector3_1 = data.vertices[index6] * finalHeight;
                                ___tmpVerts.Add(vector3_1);
                                ___tmpNorms.Add(data.vertices[index6]);
                                var vector3_2 = Vector3.Cross(data.vertices[index6], Vector3.up).normalized;
                                if (vector3_2.sqrMagnitude == 0.0) vector3_2 = Vector3.right;

                                ___tmpTgnts.Add(new Vector4(vector3_2.x, vector3_2.y, vector3_2.z, 1f));
                                ___tmpUvs.Add(new Vector2((num11 + 0.5f) / stride, (num12 + 0.5f) / stride));
                                ___tmpUv2s.Add(new Vector4(data.biomoData[index6] * 0.01f, data.temprData[index6] * 0.01f, index6 + 0.3f, num13 + 0.3f));
                            }

                            mesh.indexFormat = IndexFormat.UInt16;
                            mesh.SetVertices(___tmpVerts);
                            mesh.SetNormals(___tmpNorms);
                            mesh.SetTangents(___tmpTgnts);
                            mesh.SetUVs(0, ___tmpUvs);
                            mesh.SetUVs(1, ___tmpUv2s);
                            mesh.SetTriangles(___tmpTris, 0, true, 0);
                            mesh.RecalculateNormals();
                            mesh.GetNormals(___tmpNorms);
                            for (var index4 = 0; index4 < ___tmpNorms.Count; ++index4) {
                                var z = (int) ___tmpUv2s[index4].z;
                                var w = (int) ___tmpUv2s[index4].w;
                                data.normals[z] = data.normals[z] + ___tmpNorms[index4];
                                data.normals[w] = data.normals[w] + ___tmpNorms[index4];
                            }

                            var meshFilter2 = gameObject.AddComponent<MeshFilter>();
                            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
                            var meshCollider = gameObject.AddComponent<MeshCollider>();
                            meshFilter2.sharedMesh = mesh;
                            meshRenderer.sharedMaterial = planet.terrainMaterial;
                            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                            meshRenderer.receiveShadows = false;
                            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
                            meshCollider.sharedMesh = mesh;
                            ___tmpMeshRendererList.Add(meshRenderer);
                            ___tmpMeshColliderList.Add(meshCollider);
                            ++num8;
                            if (num8 == stateOfTheGame) return false;
                        }
                }

                var num23 = !GameMain.isLoading ? 5 : 15;
                for (var index1 = 0; index1 < ___tmpMeshList.Count; ++index1) {
                    var num9 = index1 / num23;
                    if (num9 >= ___currentModelingSeamNormal) {
                        if (num9 > ___currentModelingSeamNormal) {
                            ++___currentModelingSeamNormal;
                            return false;
                        }

                        var tmpMesh = ___tmpMeshList[index1];
                        ___tmpNorms.Clear();
                        ___tmpUv2s.Clear();
                        var vertexCount = tmpMesh.vertexCount;
                        tmpMesh.GetUVs(1, ___tmpUv2s);
                        for (var index2 = 0; index2 < vertexCount; ++index2) {
                            var z = (int) ___tmpUv2s[index2].z;
                            ___tmpNorms.Add(data.normals[z].normalized);
                        }

                        tmpMesh.SetNormals(___tmpNorms);
                    }
                }
                ___currentModelingStage = 3;
            }



            return true;
        }


        public static void ModelingPlanetMainPost(PlanetData planet,
            ref Camera ___heightmapCamera,
            ref List<Mesh> ___tmpMeshList,
            ref PlanetData ___currentModelingPlanet,
            ref int ___currentModelingStage,
            ref int ___currentModelingSeamNormal,
            ref PlanetData ___currentFactingPlanet,
            ref int ___currentFactingStage,
            ref List<MeshRenderer> ___tmpMeshRendererList,
            ref List<MeshCollider> ___tmpMeshColliderList,
            ref Collider ___tmpOceanCollider,
            ref List<Vector3> ___tmpVerts,
            ref List<Vector3> ___tmpNorms,
            ref List<Vector4> ___tmpTgnts,
            ref VegeProto[] ___vegeProtos,
            ref List<Vector2> ___tmpUvs,
            ref List<Vector4> ___tmpUv2s,
            ref List<int> ___tmpTris,
            ref GameObject ___tmpPlanetGameObject,
            ref GameObject ___tmpPlanetBodyGameObject,
            ref GameObject ___tmpPlanetReformGameObject,
            ref MeshRenderer ___tmpPlanetReformRenderer) {
            if (planet.dirtyFlags != null)
                if (planet.dirtyFlags.Length != 0)
                    for (var i = 0; i < planet.dirtyFlags.Length; i++)
                        planet.dirtyFlags[i] = true;
            // Planet.wanted shows that we've modeled the planet and are keeping it. Modeling stage will be 0 in this postfix only AFTER modeling stage 4 is complete.
            if (GameMain.isRunning && planet.wanted && ___currentModelingStage == 0) planet.UpdateDirtyMeshes();
        }
    }
}