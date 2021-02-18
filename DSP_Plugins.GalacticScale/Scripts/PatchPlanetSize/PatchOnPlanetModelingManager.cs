using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using GalacticScale.Scripts.PatchStarSystemGeneration;
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


            ThemeProto themeProto = LDB.themes.Select(planet.theme);
            
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

            int planetPrecisionBySegment = planet.precision / planet.segment;
            switch (___currentModelingStage) {
                case 0: {
                    Patch.Debug("___currentModelingStage :" + 0, LogLevel.Debug,
                        Patch.DebugPlanetModelingManager);

                    if (___tmpMeshList == null) {
                        ___tmpMeshList = new List<Mesh>(100);
                        ___tmpMeshRendererList = new List<MeshRenderer>(100);
                        ___tmpMeshColliderList = new List<MeshCollider>(100);
                        ___tmpOceanCollider = null;
                        ___tmpVerts = new List<Vector3>(1700);
                        ___tmpNorms = new List<Vector3>(1700);
                        ___tmpTgnts = new List<Vector4>(1700);
                        ___tmpUvs = new List<Vector2>(1700);
                        ___tmpUv2s = new List<Vector4>(1700);
                        ___tmpTris = new List<int>(10000);
                    }

                    if (planet.heightmap == null)
                        planet.heightmap = new RenderTexture(
                            new RenderTextureDescriptor(512, 512, RenderTextureFormat.RGHalf, 0) {
                                dimension = TextureDimension.Cube,
                                useMipMap = false,
                                autoGenerateMips = false
                            });
                    if (___heightmapCamera == null) {
                        GameObject gameObject = new GameObject("Heightmap Camera");
                        ___heightmapCamera = gameObject.AddComponent<Camera>();
                        ___heightmapCamera.cullingMask = 1073741824;
                        ___heightmapCamera.enabled = false;
                        ___heightmapCamera.farClipPlane = 900f;
                        ___heightmapCamera.nearClipPlane = 10f;
                        ___heightmapCamera.renderingPath = RenderingPath.Forward;
                        ___heightmapCamera.allowDynamicResolution = false;
                        ___heightmapCamera.allowMSAA = false;
                        ___heightmapCamera.allowHDR = true;
                        ___heightmapCamera.depthTextureMode = DepthTextureMode.None;
                        ___heightmapCamera.clearFlags = CameraClearFlags.Color;
                        ___heightmapCamera.backgroundColor = Color.black;
                        ___heightmapCamera.depth = 0.0f;
                        ___heightmapCamera.SetReplacementShader(Configs.builtin.heightmapShader, "ReplaceTag");
                        gameObject.SetActive(false);
                    }

                    if (planet.terrainMaterial == null) {
                        if (themeProto != null &&
                            themeProto.terrainMat != null) {
                            planet.terrainMaterial =
                                Instantiate(themeProto.terrainMat);
                            planet.terrainMaterial.name = planet.displayName + " Terrain";
                            planet.terrainMaterial.SetFloat("_Radius", planet.realRadius);
                        }
                        else
                            planet.terrainMaterial =
                                Instantiate(Configs.builtin.planetSurfaceMatProto);
                    }

                    if (planet.oceanMaterial == null) {
                        if (themeProto != null &&
                            themeProto.oceanMat != null) {
                            planet.oceanMaterial =
                                Instantiate(themeProto.oceanMat);
                            planet.oceanMaterial.name = planet.displayName + " Ocean";
                            planet.oceanMaterial.SetFloat("_Radius", planet.realRadius);
                        }
                        else
                            planet.oceanMaterial = null;
                    }

                    if (planet.atmosMaterial == null) {
                        if (themeProto != null &&
                            themeProto.atmosMat != null) {
                            planet.atmosMaterial =
                                Instantiate(themeProto.atmosMat);
                            planet.atmosMaterial.name = planet.displayName + " Atmos";
                        }
                        else
                            planet.atmosMaterial = null;
                    }

                    if (planet.reformMaterial == null)
                        planet.reformMaterial =
                            Instantiate(Configs.builtin.planetReformMatProto);
                    if (planet.ambientDesc == null)
                        planet.ambientDesc =
                            themeProto == null || !(themeProto.ambientDesc !=
                                                    null)
                                ? null
                                : themeProto.ambientDesc;
                    if (planet.ambientSfx == null &&
                        themeProto != null &&
                        themeProto.ambientSfx != null) {
                        planet.ambientSfx = themeProto.ambientSfx;
                        planet.ambientSfxVolume = themeProto.SFXVolume;
                    }

                    if (planet.minimapMaterial == null) {
                        planet.minimapMaterial =
                            themeProto == null || !(themeProto.minimapMat !=
                                                    null)
                                ? Instantiate(Configs.builtin.planetMinimapDefault)
                                : Instantiate(themeProto.minimapMat);
                        planet.minimapMaterial.name = planet.displayName + " Minimap";
                        planet.minimapMaterial.SetTexture("_HeightMap", planet.heightmap);
                    }

                    ___tmpMeshList.Clear();
                    ___tmpMeshRendererList.Clear();
                    ___tmpMeshColliderList.Clear();
                    ___tmpOceanCollider = null;
                    ___tmpTris.Clear();
                    ___currentModelingStage = 1;
                    break;
                }
                case 1: {
                    Patch.Debug("___currentModelingStage :" + 1, LogLevel.Debug,
                        Patch.DebugPlanetModelingManager);

                    ___tmpPlanetGameObject = new GameObject(planet.displayName);
                    ___tmpPlanetGameObject.layer = 31;
                    GameMain.universeSimulator.SetPlanetSimulator(
                        ___tmpPlanetGameObject.AddComponent<PlanetSimulator>(), planet);
                    
                    ___tmpPlanetGameObject.transform.localPosition = Vector3.zero;
                    ___tmpPlanetBodyGameObject = new GameObject("Planet Body");
                    ___tmpPlanetBodyGameObject.transform.SetParent(___tmpPlanetGameObject.transform, false);
                    ___tmpPlanetBodyGameObject.layer = 31;
                    ___tmpPlanetReformGameObject = new GameObject("Terrain Reform");
                    ___tmpPlanetReformGameObject.transform.SetParent(___tmpPlanetBodyGameObject.transform,
                        false);
                    ___tmpPlanetReformGameObject.layer = 14;
                    MeshFilter meshFilter1 = ___tmpPlanetReformGameObject.AddComponent<MeshFilter>();
                    ___tmpPlanetReformRenderer = ___tmpPlanetReformGameObject.AddComponent<MeshRenderer>();
                    meshFilter1.sharedMesh = Configs.builtin.planetReformMesh;
                    ___tmpPlanetReformRenderer.sharedMaterial = planet.reformMaterial;
                    ___tmpPlanetReformRenderer.receiveShadows = false;
                    ___tmpPlanetReformRenderer.lightProbeUsage = LightProbeUsage.Off;
                    ___tmpPlanetReformRenderer.shadowCastingMode = ShadowCastingMode.Off;
                    // radius x2 = diameter
                    float planetReformDiameter =
                        (float) ((planet.realRadius + 0.200000002980232 + 0.025000000372529) * 2.0);


                    Patch.Debug("planetReformDiameter :" + planetReformDiameter, LogLevel.Debug,
                        Patch.DebugPlanetModelingManager);

                    planetReformDiameter /= 2;

                    Patch.Debug("Try patching reform Diameter / 2  :" + planetReformDiameter,
                        LogLevel.Debug,
                        Patch.DebugPlanetModelingManager);

                    ___tmpPlanetReformRenderer.transform.localScale = new Vector3(planetReformDiameter,
                        planetReformDiameter, planetReformDiameter);
                    ___tmpPlanetReformRenderer.transform.rotation = Quaternion.identity;
                    if (planet.waterItemId != 0) {
                        GameObject gameObject =
                            Instantiate(Configs.builtin.oceanSphere,
                                ___tmpPlanetBodyGameObject.transform);
                        gameObject.name = "Ocean Sphere";
                        gameObject.layer = 31;
                        gameObject.transform.localPosition = Vector3.zero;
                        gameObject.transform.localScale = Vector3.one *
                                                          (float) ((planet.realRadius +
                                                                    (double) planet.waterHeight) * 2.0);

                        Patch.Debug("Ocean Sphere Scale : realRadius : " + planet.realRadius +
                                    " water height " + planet.waterHeight + " x 2 ", LogLevel.Debug,
                            Patch.DebugPlanetModelingManager);


                        Patch.Debug("Ocean Sphere Scale :" + gameObject.transform.localScale,
                            LogLevel.Debug,
                            Patch.DebugPlanetModelingManager);

                        Renderer component = gameObject.GetComponent<Renderer>();
                        ___tmpOceanCollider = gameObject.GetComponent<Collider>();
                        if (component != null) {
                            component.enabled = planet.oceanMaterial !=
                                                null;
                            component.shadowCastingMode = ShadowCastingMode.Off;
                            component.receiveShadows = false;
                            component.lightProbeUsage = LightProbeUsage.Off;
                            component.sharedMaterial = planet.oceanMaterial;
                        }
                    }

                    Patch.Debug(
                        "Planet Precision  : " + planet.precision + " Planet Segments " + planet.segment,
                        LogLevel.Debug,
                        Patch.DebugPlanetModelingManager);

                    int planetPrecisionBySegmentPlusOne = planetPrecisionBySegment + 1;


                    Patch.Debug("Planet tris Generation", LogLevel.Debug,
                        Patch.DebugPlanetModelingManager);
                    for (int index1 = 0; index1 < planetPrecisionBySegment; ++index1) {
                        for (int index2 = 0; index2 < planetPrecisionBySegment; ++index2) {
                            ___tmpTris.Add(index1 + 1 + (index2 + 1) * planetPrecisionBySegmentPlusOne);
                            ___tmpTris.Add(index1 + (index2 + 1) * planetPrecisionBySegmentPlusOne);
                            ___tmpTris.Add(index1 + index2 * planetPrecisionBySegmentPlusOne);
                            ___tmpTris.Add(index1 + index2 * planetPrecisionBySegmentPlusOne);
                            ___tmpTris.Add(index1 + 1 + index2 * planetPrecisionBySegmentPlusOne);
                            ___tmpTris.Add(index1 + 1 + (index2 + 1) * planetPrecisionBySegmentPlusOne);
                        }
                    }

                    Patch.Debug("___currentModelingStage end of 1", LogLevel.Debug,
                        Patch.DebugPlanetModelingManager);
                    ___currentModelingStage = 2;
                    break;
                }
                case 2: {
                    int planetPrecision = planet.precision;
                    PlanetRawData data = planet.data;
                    float planetScale = planet.scale;
                    // float planetRadiusScaled = (float) (planet.radius * (double) planetScale + 0.200000002980232);
                    float planetRadiusScaled =
                        (float) (planet.radius + 0.200000002980232);
                    Patch.Debug("planetRadiusScaled " + planetRadiusScaled, LogLevel.Debug,
                        Patch.DebugPlanetModelingManager);
                    // planetRadiusScaled *= scaleFactor;
                    Patch.Debug("planetRadiusScaled Patched " + planetRadiusScaled, LogLevel.Debug,
                        Patch.DebugPlanetModelingManager);
                    int stride = data.stride;
                    int num6 = 0;
                    int stateOfTheGame = !GameMain.isLoading ? 2 : 3;
                    int num8 = 0;
                    for (int index1 = 0; index1 < 4; ++index1) {
                        int num9 = index1 % 2 * (planetPrecision + 1);
                        int num10 = index1 / 2 * (planetPrecision + 1);
                        for (int index2 = 0; index2 < planetPrecision; index2 += planetPrecisionBySegment) {
                            for (int index3 = 0; index3 < planetPrecision; index3 += planetPrecisionBySegment) {
                                if (num8 == 0 && num6 < ___tmpMeshList.Count) {
                                    ++num6;
                                }
                                else {
                                    Mesh mesh = new Mesh();
                                    ___tmpMeshList.Add(mesh);
                                    ___tmpVerts.Clear();
                                    ___tmpNorms.Clear();
                                    ___tmpTgnts.Clear();
                                    ___tmpUvs.Clear();
                                    ___tmpUv2s.Clear();
                                    GameObject gameObject = new GameObject("Surface");
                                    gameObject.layer = 30;
                                    gameObject.transform.SetParent(___tmpPlanetBodyGameObject.transform, false);
                                    for (int index4 = index2;
                                        index4 <= index2 + planetPrecisionBySegment &&
                                        index4 <= planetPrecision;
                                        ++index4) {
                                        for (int index5 = index3;
                                            index5 <= index3 + planetPrecisionBySegment &&
                                            index5 <= planetPrecision;
                                            ++index5) {
                                            int num11 = num9 + index5;
                                            int num12 = num10 + index4;
                                            int index6 = num11 + num12 * stride;
                                            int num13 = index6;
                                            if (index4 == 0) {
                                                int num14 = (index1 + 3) % 4;
                                                int num15 = num14 % 2 * (planetPrecision + 1);
                                                int num16 = num14 / 2 * (planetPrecision + 1);
                                                int num17 = planetPrecision;
                                                int num18 = planetPrecision - index5;
                                                num13 = num15 + num17 + (num16 + num18) * stride;
                                            }
                                            else if (index5 == 0) {
                                                int num14 = (index1 + 3) % 4;
                                                int num15 = num14 % 2 * (planetPrecision + 1);
                                                int num16 = num14 / 2 * (planetPrecision + 1);
                                                int num17 = planetPrecision - index4;
                                                int num18 = planetPrecision;
                                                num13 = num15 + num17 + (num16 + num18) * stride;
                                            }

                                            if (index4 == planetPrecision) {
                                                int num14 = (index1 + 1) % 4;
                                                int num15 = num14 % 2 * (planetPrecision + 1);
                                                int num16 = num14 / 2 * (planetPrecision + 1);
                                                int num17 = 0;
                                                int num18 = planetPrecision - index5;
                                                num13 = num15 + num17 + (num16 + num18) * stride;
                                            }
                                            else if (index5 == planetPrecision) {
                                                int num14 = (index1 + 1) % 4;
                                                int num15 = num14 % 2 * (planetPrecision + 1);
                                                int num16 = num14 / 2 * (planetPrecision + 1);
                                                int num17 = planetPrecision - index4;
                                                int num18 = 0;
                                                num13 = num15 + num17 + (num16 + num18) * stride;
                                            }

                                            float heightDataScaled = data.heightData[index6] * 0.01f;
                                            if (planet.type == EPlanetType.Gas) {
                                                heightDataScaled *= planetScale;
                                            }
                                            Patch.Debug("heightDataScaled  :  " + heightDataScaled,
                                                LogLevel.Debug,
                                                Patch.DebugPlanetModelingManagerDeep);
                                            float thirdOfModLevel =
                                                data.GetModLevel(index6) * 0.3333333f;

                                            Patch.Debug("thirdOfModLevel  :  " + thirdOfModLevel,
                                                LogLevel.Debug,
                                                Patch.DebugPlanetModelingManagerDeep);

                                            if (thirdOfModLevel > 0.0) {
                                                //data.GetModPlane(index6)) 20000 + * 0.01f --> 200 +
                                                Patch.Debug(
                                                    "data.GetModPlane(index6) :  " + data.GetModPlane(index6),
                                                    LogLevel.Debug,
                                                    Patch.DebugPlanetModelingManagerDeep);
                                                float modPlanePatched = data.GetModPlane(index6) *
                                                                        planet.GetScaleFactored();

                                                Patch.Debug("patch modPlane:  " + modPlanePatched,
                                                    LogLevel.Debug,
                                                    Patch.DebugPlanetModelingManagerDeep);
                                                planetRadiusScaled = modPlanePatched * 0.01f *
                                                                     planetScale;

                                                Patch.Debug(
                                                    "planetRadiusScaled is modified :  " + planetRadiusScaled,
                                                    LogLevel.Debug,
                                                    Patch.DebugPlanetModelingManagerDeep);
                                            }

                                            // final height modification ? 
                                            float finalHeight =
                                                (float) (heightDataScaled *
                                                         (1.0 - thirdOfModLevel) +
                                                         planetRadiusScaled *
                                                         (double) thirdOfModLevel);

                                            Patch.Debug("finalHeight :  " + finalHeight, LogLevel.Debug,
                                                Patch.DebugPlanetModelingManagerDeep);

                                            Vector3 vector3_1 = data.vertices[index6] * finalHeight;
                                            ___tmpVerts.Add(vector3_1);
                                            ___tmpNorms.Add(data.vertices[index6]);
                                            Vector3 vector3_2 = Vector3.Cross(data.vertices[index6], Vector3.up)
                                                .normalized;
                                            if (vector3_2.sqrMagnitude == 0.0) {
                                                vector3_2 = Vector3.right;
                                            }

                                            ___tmpTgnts.Add(new Vector4(vector3_2.x, vector3_2.y, vector3_2.z,
                                                1f));
                                            ___tmpUvs.Add(new Vector2((num11 + 0.5f) / stride,
                                                (num12 + 0.5f) / stride));
                                            ___tmpUv2s.Add(new Vector4(data.biomoData[index6] * 0.01f,
                                                data.temprData[index6] * 0.01f, index6 + 0.3f,
                                                num13 + 0.3f));
                                        }
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
                                    for (int index4 = 0; index4 < ___tmpNorms.Count; ++index4) {
                                        int z = (int) ___tmpUv2s[index4].z;
                                        int w = (int) ___tmpUv2s[index4].w;
                                        data.normals[z] = data.normals[z] + ___tmpNorms[index4];
                                        data.normals[w] = data.normals[w] + ___tmpNorms[index4];
                                    }

                                    MeshFilter meshFilter2 = gameObject.AddComponent<MeshFilter>();
                                    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                                    MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
                                    meshFilter2.sharedMesh = mesh;
                                    meshRenderer.sharedMaterial = planet.terrainMaterial;
                                    meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                                    meshRenderer.receiveShadows = false;
                                    meshRenderer.lightProbeUsage = LightProbeUsage.Off;
                                    meshCollider.sharedMesh = mesh;
                                    ___tmpMeshRendererList.Add(meshRenderer);
                                    ___tmpMeshColliderList.Add(meshCollider);
                                    ++num8;
                                    if (num8 == stateOfTheGame) {
                                        return false;
                                    }
                                }
                            }
                        }
                    }

                    int num23 = !GameMain.isLoading ? 5 : 15;
                    for (int index1 = 0; index1 < ___tmpMeshList.Count; ++index1) {
                        int num9 = index1 / num23;
                        if (num9 >= ___currentModelingSeamNormal) {
                            if (num9 > ___currentModelingSeamNormal) {
                                ++___currentModelingSeamNormal;
                                return false;
                            }

                            Mesh tmpMesh = ___tmpMeshList[index1];
                            ___tmpNorms.Clear();
                            ___tmpUv2s.Clear();
                            int vertexCount = tmpMesh.vertexCount;
                            tmpMesh.GetUVs(1, ___tmpUv2s);
                            for (int index2 = 0; index2 < vertexCount; ++index2) {
                                int z = (int) ___tmpUv2s[index2].z;
                                ___tmpNorms.Add(data.normals[z].normalized);
                            }

                            tmpMesh.SetNormals(___tmpNorms);
                        }
                    }

                    ___currentModelingStage = 3;
                    break;
                }
                case 3: {
                    ___tmpPlanetBodyGameObject.SetActive(true);
                    ___tmpPlanetReformGameObject.SetActive(true);
                    ___heightmapCamera.transform.localPosition = ___tmpPlanetGameObject.transform.localPosition;
                    ___heightmapCamera.RenderToCubemap(planet.heightmap, 63);
                    ___currentModelingStage = 4;
                    break;
                }
                case 4: {
                    Patch.Debug("___currentModelingStage start of 4", LogLevel.Debug,
                        Patch.DebugPlanetModelingManager);

                    if (planet.wanted) {
                        Patch.Debug("planet.wanted is True", LogLevel.Debug,
                            Patch.DebugPlanetModelingManager);

                        planet.gameObject = ___tmpPlanetGameObject;
                        planet.bodyObject = ___tmpPlanetBodyGameObject;
                        PlanetSimulator component = ___tmpPlanetGameObject.GetComponent<PlanetSimulator>();
                        component.surfaceRenderer = new Renderer[___tmpMeshRendererList.Count];
                        component.surfaceCollider = new Collider[___tmpMeshColliderList.Count];
                        for (int index = 0; index < ___tmpMeshList.Count; ++index) {
                            planet.meshes[index] = ___tmpMeshList[index];
                            planet.meshRenderers[index] = ___tmpMeshRendererList[index];
                            planet.meshColliders[index] = ___tmpMeshColliderList[index];
                        }

                        for (int index = 0; index < ___tmpMeshRendererList.Count; ++index) {
                            ___tmpMeshRendererList[index].gameObject.layer = 31;
                            ___tmpMeshRendererList[index].sharedMaterial = planet.terrainMaterial;
                            ___tmpMeshRendererList[index].receiveShadows = false;
                            ___tmpMeshRendererList[index].shadowCastingMode = ShadowCastingMode.Off;
                            component.surfaceRenderer[index] = ___tmpMeshRendererList[index];
                            component.surfaceCollider[index] = ___tmpMeshColliderList[index];
                        }

                        component.oceanCollider = ___tmpOceanCollider;
                        component.sphereCollider = ___tmpPlanetBodyGameObject.AddComponent<SphereCollider>();
                        if (component.sphereCollider != null)
                            component.sphereCollider.enabled = false;
                        component.sphereCollider.radius = planet.realRadius;
                        component.reformRenderer = ___tmpPlanetReformRenderer;
                        component.reformMat = planet.reformMaterial;
                        Material sharedMaterial = component.surfaceRenderer[0].sharedMaterial;
                        if (planet.type != EPlanetType.Gas) {
                            component.reformMat.SetColor("_AmbientColor0",
                                sharedMaterial.GetColor("_AmbientColor0"));
                            component.reformMat.SetColor("_AmbientColor1",
                                sharedMaterial.GetColor("_AmbientColor1"));
                            component.reformMat.SetColor("_AmbientColor2",
                                sharedMaterial.GetColor("_AmbientColor2"));
                            component.reformMat.SetColor("_LightColorScreen",
                                sharedMaterial.GetColor("_LightColorScreen"));
                            component.reformMat.SetFloat("_Multiplier", sharedMaterial.GetFloat("_Multiplier"));
                            component.reformMat.SetFloat("_AmbientInc", sharedMaterial.GetFloat("_AmbientInc"));
                        }

                        ___tmpPlanetGameObject.transform.localPosition = Vector3.zero;
                        ___heightmapCamera.transform.localPosition = Vector3.zero;
                        ___tmpPlanetBodyGameObject.SetActive(true);
                        ___tmpPlanetReformGameObject.SetActive(true);
                        ___tmpPlanetGameObject = null;
                        ___tmpPlanetBodyGameObject = null;
                        ___tmpPlanetReformGameObject = null;
                        ___tmpPlanetReformRenderer = null;
                        ___tmpMeshList.Clear();
                        ___tmpMeshRendererList.Clear();
                        ___tmpMeshColliderList.Clear();
                        ___tmpOceanCollider = null;
                        ___tmpTris.Clear();
                        ___tmpVerts.Clear();
                        ___tmpNorms.Clear();
                        ___tmpTgnts.Clear();
                        ___tmpUvs.Clear();
                        ___tmpUv2s.Clear();
                        ___currentModelingPlanet = null;
                        ___currentModelingStage = 0;
                        ___currentModelingSeamNormal = 0;
                        planet.NotifyLoaded();
                        if (!planet.star.loaded) {
                            break;
                        }

                        planet.star.NotifyLoaded();

                        Patch.Debug("___currentModelingStage end of 4", LogLevel.Debug,
                            Patch.DebugPlanetModelingManager);

                        break;
                    }

                    for (int index = 0; index < ___tmpMeshList.Count; ++index)
                        Destroy(___tmpMeshList[index]);
                    Destroy(___tmpPlanetGameObject);
                    ___tmpPlanetGameObject = null;
                    ___tmpPlanetBodyGameObject = null;
                    ___tmpPlanetReformGameObject = null;
                    ___tmpPlanetReformRenderer = null;
                    ___tmpMeshList.Clear();
                    ___tmpTris.Clear();
                    ___tmpVerts.Clear();
                    ___tmpNorms.Clear();
                    ___tmpTgnts.Clear();
                    ___tmpUvs.Clear();
                    ___tmpUv2s.Clear();
                    ___currentModelingPlanet = null;
                    ___currentModelingStage = 0;
                    ___currentModelingSeamNormal = 0;
                    break;
                }
            }

            return false;
        }
    }
}