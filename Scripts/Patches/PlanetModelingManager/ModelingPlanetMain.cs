using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), "ModelingPlanetMain")]
        public static bool ModelingPlanetMain(PlanetData planet)
        {
            planet.data.AddFactoredRadius(planet);
            return true;
        }
        //[HarmonyPostfix, HarmonyPatch(typeof(PlanetModelingManager), "ModelingPlanetMain")]
        //public static void Postfix(PlanetData planet,ref GameObject ___tmpPlanetGameObject)
        //{
        //    if (PlanetModelingManager.currentModelingStage == 4)
        //    {
        //        var objectGroup = new GameObject("Blackhole");
        //        objectGroup.transform.parent = ___tmpPlanetGameObject.transform;
        //        objectGroup.transform.localPosition = Vector3.zero;
        //        objectGroup.transform.localRotation = Quaternion.identity;
        //        objectGroup.transform.localScale = Vector3.one;
        //        var blackHole = UnityEngine.Object.Instantiate(Configs.builtin.blackHolePrefab, objectGroup.transform);
        //        blackHole.radius = planet.realRadius/2.5f;
        //    }
        //}
        //[HarmonyPostfix, HarmonyPatch(typeof(PlanetModelingManager), "ModelingPlanetMain")]
        //public static void ModelingPlanetMain2(PlanetData planet)
        //{
        //    //GS2.Log($"{planet.name} end of modeling planet main");
        //    return;
        //}
        //public static int vertCount = 0;
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(PlanetModelingManager), "ModelingPlanetMain")]
        //public static bool ModelingPlanetMain(PlanetData planet)
        //{
        //    planet.data.AddFactoredRadius(planet);

        //    ThemeProto themeProto = LDB.themes.Select(planet.theme);
        //    switch (PlanetModelingManager.currentModelingStage)
        //    {
        //        case 0:
        //            if (PlanetModelingManager.tmpMeshList == null)
        //            {
        //                PlanetModelingManager.tmpMeshList = new List<Mesh>(200);
        //                PlanetModelingManager.tmpMeshRendererList = new List<MeshRenderer>(200);
        //                PlanetModelingManager.tmpMeshColliderList = new List<MeshCollider>(200);
        //                PlanetModelingManager.tmpOceanCollider = (Collider)null;
        //                PlanetModelingManager.tmpVerts = new List<Vector3>(3400);
        //                PlanetModelingManager.tmpNorms = new List<Vector3>(3400);
        //                PlanetModelingManager.tmpTgnts = new List<Vector4>(3400);
        //                PlanetModelingManager.tmpUvs = new List<Vector2>(3400);
        //                PlanetModelingManager.tmpUv2s = new List<Vector4>(3400);
        //                PlanetModelingManager.tmpTris = new List<int>(20000);
        //            }
        //            if ((UnityEngine.Object)planet.heightmap == (UnityEngine.Object)null)
        //                planet.heightmap = new RenderTexture(new RenderTextureDescriptor(512, 512, RenderTextureFormat.RGHalf, 0)
        //                {
        //                    dimension = TextureDimension.Cube,
        //                    useMipMap = false,
        //                    autoGenerateMips = false
        //                });
        //            if ((UnityEngine.Object)PlanetModelingManager.heightmapCamera == (UnityEngine.Object)null)
        //            {
        //                GameObject gameObject = new GameObject("Heightmap Camera");
        //                PlanetModelingManager.heightmapCamera = gameObject.AddComponent<Camera>();
        //                PlanetModelingManager.heightmapCamera.cullingMask = 1073741824;
        //                PlanetModelingManager.heightmapCamera.enabled = false;
        //                PlanetModelingManager.heightmapCamera.farClipPlane = 900f;
        //                PlanetModelingManager.heightmapCamera.nearClipPlane = 10f;
        //                PlanetModelingManager.heightmapCamera.renderingPath = RenderingPath.Forward;
        //                PlanetModelingManager.heightmapCamera.allowDynamicResolution = false;
        //                PlanetModelingManager.heightmapCamera.allowMSAA = false;
        //                PlanetModelingManager.heightmapCamera.allowHDR = true;
        //                PlanetModelingManager.heightmapCamera.depthTextureMode = DepthTextureMode.None;
        //                PlanetModelingManager.heightmapCamera.clearFlags = CameraClearFlags.Color;
        //                PlanetModelingManager.heightmapCamera.backgroundColor = Color.black;
        //                PlanetModelingManager.heightmapCamera.depth = 0.0f;
        //                PlanetModelingManager.heightmapCamera.SetReplacementShader(Configs.builtin.heightmapShader, "ReplaceTag");
        //                gameObject.SetActive(false);
        //            }
        //            if ((UnityEngine.Object)planet.terrainMaterial == (UnityEngine.Object)null)
        //            {
        //                if (themeProto != null && (UnityEngine.Object)themeProto.terrainMat != (UnityEngine.Object)null)
        //                {
        //                    planet.terrainMaterial = UnityEngine.Object.Instantiate<Material>(themeProto.terrainMat);
        //                    planet.terrainMaterial.name = planet.displayName + " Terrain";
        //                    planet.terrainMaterial.SetFloat("_Radius", planet.realRadius);
        //                }
        //                else
        //                    planet.terrainMaterial = UnityEngine.Object.Instantiate<Material>(Configs.builtin.planetSurfaceMatProto);
        //            }
        //            if ((UnityEngine.Object)planet.oceanMaterial == (UnityEngine.Object)null)
        //            {
        //                if (themeProto != null && (UnityEngine.Object)themeProto.oceanMat != (UnityEngine.Object)null)
        //                {
        //                    planet.oceanMaterial = UnityEngine.Object.Instantiate<Material>(themeProto.oceanMat);
        //                    planet.oceanMaterial.name = planet.displayName + " Ocean";
        //                    planet.oceanMaterial.SetFloat("_Radius", planet.realRadius);
        //                }
        //                else
        //                    planet.oceanMaterial = (Material)null;
        //            }
        //            if ((UnityEngine.Object)planet.atmosMaterial == (UnityEngine.Object)null)
        //            {
        //                if (themeProto != null && (UnityEngine.Object)themeProto.atmosMat != (UnityEngine.Object)null)
        //                {
        //                    planet.atmosMaterial = UnityEngine.Object.Instantiate<Material>(themeProto.atmosMat);
        //                    planet.atmosMaterial.name = planet.displayName + " Atmos";
        //                }
        //                else
        //                    planet.atmosMaterial = (Material)null;
        //            }
        //            if ((UnityEngine.Object)planet.reformMaterial0 == (UnityEngine.Object)null)
        //                planet.reformMaterial0 = UnityEngine.Object.Instantiate<Material>(Configs.builtin.planetReformMatProto0);
        //            if ((UnityEngine.Object)planet.reformMaterial1 == (UnityEngine.Object)null)
        //                planet.reformMaterial1 = UnityEngine.Object.Instantiate<Material>(Configs.builtin.planetReformMatProto1);
        //            if ((UnityEngine.Object)planet.ambientDesc == (UnityEngine.Object)null)
        //                planet.ambientDesc = themeProto == null || !((UnityEngine.Object)themeProto.ambientDesc != (UnityEngine.Object)null) ? (AmbientDesc)null : themeProto.ambientDesc;
        //            if ((UnityEngine.Object)planet.ambientSfx == (UnityEngine.Object)null && themeProto != null && (UnityEngine.Object)themeProto.ambientSfx != (UnityEngine.Object)null)
        //            {
        //                planet.ambientSfx = themeProto.ambientSfx;
        //                planet.ambientSfxVolume = themeProto.SFXVolume;
        //            }
        //            if ((UnityEngine.Object)planet.minimapMaterial == (UnityEngine.Object)null)
        //            {
        //                planet.minimapMaterial = themeProto == null || !((UnityEngine.Object)themeProto.minimapMat != (UnityEngine.Object)null) ? UnityEngine.Object.Instantiate<Material>(Configs.builtin.planetMinimapDefault) : UnityEngine.Object.Instantiate<Material>(themeProto.minimapMat);
        //                planet.minimapMaterial.name = planet.displayName + " Minimap";
        //                planet.minimapMaterial.SetTexture("_HeightMap", (Texture)planet.heightmap);
        //            }
        //            PlanetModelingManager.tmpMeshList.Clear();
        //            PlanetModelingManager.tmpMeshRendererList.Clear();
        //            PlanetModelingManager.tmpMeshColliderList.Clear();
        //            PlanetModelingManager.tmpOceanCollider = (Collider)null;
        //            PlanetModelingManager.tmpTris.Clear();
        //            PlanetModelingManager.currentModelingStage = 1;
        //            break;
        //        case 1:
        //            PlanetModelingManager.tmpPlanetGameObject = new GameObject(planet.displayName);
        //            PlanetModelingManager.tmpPlanetGameObject.layer = 31;
        //            GameMain.universeSimulator.SetPlanetSimulator(PlanetModelingManager.tmpPlanetGameObject.AddComponent<PlanetSimulator>(), planet);
        //            PlanetModelingManager.tmpPlanetGameObject.transform.localPosition = Vector3.zero;
        //            PlanetModelingManager.tmpPlanetBodyGameObject = new GameObject("Planet Body");
        //            PlanetModelingManager.tmpPlanetBodyGameObject.transform.SetParent(PlanetModelingManager.tmpPlanetGameObject.transform, false);
        //            PlanetModelingManager.tmpPlanetBodyGameObject.layer = 31;
        //            PlanetModelingManager.tmpPlanetReformGameObject = new GameObject("Terrain Reform");
        //            PlanetModelingManager.tmpPlanetReformGameObject.transform.SetParent(PlanetModelingManager.tmpPlanetBodyGameObject.transform, false);
        //            PlanetModelingManager.tmpPlanetReformGameObject.layer = 14;
        //            MeshFilter meshFilter1 = PlanetModelingManager.tmpPlanetReformGameObject.AddComponent<MeshFilter>();
        //            PlanetModelingManager.tmpPlanetReformRenderer = PlanetModelingManager.tmpPlanetReformGameObject.AddComponent<MeshRenderer>();
        //            Mesh planetReformMesh = Configs.builtin.planetReformMesh;
        //            meshFilter1.sharedMesh = planetReformMesh;
        //            Material[] materialArray = new Material[2]
        //            {
        //  planet.reformMaterial0,
        //  planet.reformMaterial1
        //            };
        //            PlanetModelingManager.tmpPlanetReformRenderer.sharedMaterials = materialArray;
        //            PlanetModelingManager.tmpPlanetReformRenderer.receiveShadows = false;
        //            PlanetModelingManager.tmpPlanetReformRenderer.lightProbeUsage = LightProbeUsage.Off;
        //            PlanetModelingManager.tmpPlanetReformRenderer.shadowCastingMode = ShadowCastingMode.Off;
        //            float num1 = (float)(((double)planet.realRadius + 0.200000002980232 + (planet.realRadius / 8000f)) * 2.0);
        //            PlanetModelingManager.tmpPlanetReformRenderer.transform.localScale = new Vector3(num1, num1, num1);
        //            PlanetModelingManager.tmpPlanetReformRenderer.transform.rotation = Quaternion.identity;
        //            if (planet.waterItemId != 0)
        //            {
        //                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Configs.builtin.oceanSphere, PlanetModelingManager.tmpPlanetBodyGameObject.transform);
        //                gameObject.name = "Ocean Sphere";
        //                gameObject.layer = 31;
        //                gameObject.transform.localPosition = Vector3.zero;
        //                gameObject.transform.localScale = Vector3.one * (float)(((double)planet.realRadius + (double)planet.waterHeight) * 2.0);
        //                Renderer component = gameObject.GetComponent<Renderer>();
        //                PlanetModelingManager.tmpOceanCollider = gameObject.GetComponent<Collider>();
        //                if ((UnityEngine.Object)component != (UnityEngine.Object)null)
        //                {
        //                    component.enabled = (UnityEngine.Object)planet.oceanMaterial != (UnityEngine.Object)null;
        //                    component.shadowCastingMode = ShadowCastingMode.Off;
        //                    component.receiveShadows = false;
        //                    component.lightProbeUsage = LightProbeUsage.Off;
        //                    component.sharedMaterial = planet.oceanMaterial;
        //                }
        //            }
        //            int num2 = planet.precision / planet.segment;
        //            int num3 = num2 + 1;
        //            for (int index1 = 0; index1 < num2; ++index1)
        //            {
        //                for (int index2 = 0; index2 < num2; ++index2)
        //                {
        //                    PlanetModelingManager.tmpTris.Add(index1 + 1 + (index2 + 1) * num3);
        //                    PlanetModelingManager.tmpTris.Add(index1 + (index2 + 1) * num3);
        //                    PlanetModelingManager.tmpTris.Add(index1 + index2 * num3);
        //                    PlanetModelingManager.tmpTris.Add(index1 + index2 * num3);
        //                    PlanetModelingManager.tmpTris.Add(index1 + 1 + index2 * num3);
        //                    PlanetModelingManager.tmpTris.Add(index1 + 1 + (index2 + 1) * num3);
        //                }
        //            }
        //            PlanetModelingManager.currentModelingStage = 2;
        //            break;
        //        case 2:
        //            int precision = planet.precision;
        //            int num4 = precision / planet.segment;
        //            PlanetRawData data = planet.data;
        //            float scale = planet.scale;
        //            float num5 = (float)((double)planet.radius * (double)scale + 0.200000002980232);
        //            int stride = data.stride;
        //            int num6 = 0;
        //            int num7 = GameMain.isLoading ? 3 : 2;
        //            int num8 = 0;
        //            for (int index1 = 0; index1 < 4; ++index1)
        //            {
        //                int num9 = index1 % 2 * (precision + 1);
        //                int num10 = index1 / 2 * (precision + 1);
        //                for (int index2 = 0; index2 < precision; index2 += num4)
        //                {
        //                    for (int index3 = 0; index3 < precision; index3 += num4)
        //                    {
        //                        if (num8 == 0 && num6 < PlanetModelingManager.tmpMeshList.Count)
        //                        {
        //                            ++num6;
        //                        }
        //                        else
        //                        {
        //                            Mesh mesh = new Mesh();
        //                            PlanetModelingManager.tmpMeshList.Add(mesh);
        //                            PlanetModelingManager.tmpVerts.Clear();
        //                            PlanetModelingManager.tmpNorms.Clear();
        //                            PlanetModelingManager.tmpTgnts.Clear();
        //                            PlanetModelingManager.tmpUvs.Clear();
        //                            PlanetModelingManager.tmpUv2s.Clear();
        //                            GameObject gameObject = new GameObject("Surface");
        //                            gameObject.layer = 30;
        //                            gameObject.transform.SetParent(PlanetModelingManager.tmpPlanetBodyGameObject.transform, false);
        //                            for (int index4 = index2; index4 <= index2 + num4 && index4 <= precision; ++index4)
        //                            {
        //                                for (int index5 = index3; index5 <= index3 + num4 && index5 <= precision; ++index5)
        //                                {
        //                                    int num11 = num9 + index5;
        //                                    int num12 = num10 + index4;
        //                                    int index6 = num11 + num12 * stride;
        //                                    int num13 = index6;
        //                                    if (index4 == 0)
        //                                    {
        //                                        int num14 = (index1 + 3) % 4;
        //                                        int num15 = num14 % 2 * (precision + 1);
        //                                        int num16 = num14 / 2 * (precision + 1);
        //                                        int num17 = precision;
        //                                        int num18 = precision - index5;
        //                                        num13 = num15 + num17 + (num16 + num18) * stride;
        //                                    }
        //                                    else if (index5 == 0)
        //                                    {
        //                                        int num14 = (index1 + 3) % 4;
        //                                        int num15 = num14 % 2 * (precision + 1);
        //                                        int num16 = num14 / 2 * (precision + 1);
        //                                        int num17 = precision - index4;
        //                                        int num18 = precision;
        //                                        num13 = num15 + num17 + (num16 + num18) * stride;
        //                                    }
        //                                    if (index4 == precision)
        //                                    {
        //                                        int num14 = (index1 + 1) % 4;
        //                                        int num15 = num14 % 2 * (precision + 1);
        //                                        int num16 = num14 / 2 * (precision + 1);
        //                                        int num17 = 0;
        //                                        int num18 = precision - index5;
        //                                        num13 = num15 + num17 + (num16 + num18) * stride;
        //                                    }
        //                                    else if (index5 == precision)
        //                                    {
        //                                        int num14 = (index1 + 1) % 4;
        //                                        int num15 = num14 % 2 * (precision + 1);
        //                                        int num16 = num14 / 2 * (precision + 1);
        //                                        int num17 = precision - index4;
        //                                        int num18 = 0;
        //                                        num13 = num15 + num17 + (num16 + num18) * stride;
        //                                    }
        //                                    double num19 = (double)data.heightData[index6] * 0.00999999977648258 * (double)scale;
        //                                    float num20 = (float)data.GetModLevel(index6) * 0.3333333f;
        //                                    float num21 = num5;
        //                                    if ((double)num20 > 0.0)
        //                                        num21 = (float)data.GetModPlaneInt(index6) * 0.01f * scale;
        //                                    double num22 = 1.0 - (double)num20;
        //                                    float num23 = (float)(num19 * num22 + (double)num21 * (double)num20);
        //                                    Vector3 vector3_1 = data.vertices[index6] * num23;
        //                                    PlanetModelingManager.tmpVerts.Add(vector3_1);
        //                                    PlanetModelingManager.tmpNorms.Add(data.vertices[index6]);
        //                                    Vector3 vector3_2 = Vector3.Cross(data.vertices[index6], Vector3.up).normalized;
        //                                    if ((double)vector3_2.sqrMagnitude == 0.0)
        //                                        vector3_2 = Vector3.right;
        //                                    PlanetModelingManager.tmpTgnts.Add(new Vector4(vector3_2.x, vector3_2.y, vector3_2.z, 1f));
        //                                    PlanetModelingManager.tmpUvs.Add(new Vector2(((float)num11 + 0.5f) / (float)stride, ((float)num12 + 0.5f) / (float)stride));
        //                                    PlanetModelingManager.tmpUv2s.Add(new Vector4((float)data.biomoData[index6] * 0.01f, (float)data.temprData[index6] * 0.01f, (float)index6 + 0.3f, (float)num13 + 0.3f));
        //                                }
        //                            }
        //                            mesh.indexFormat = IndexFormat.UInt16;
        //                            if (vertCount == 0) GS2.Warn($"Setting Verts {planet.name}");
        //                            vertCount++;
        //                            mesh.SetVertices(PlanetModelingManager.tmpVerts);
        //                            mesh.SetNormals(PlanetModelingManager.tmpNorms);
        //                            mesh.SetTangents(PlanetModelingManager.tmpTgnts);
        //                            mesh.SetUVs(0, PlanetModelingManager.tmpUvs);
        //                            mesh.SetUVs(1, PlanetModelingManager.tmpUv2s);
        //                            mesh.SetTriangles(PlanetModelingManager.tmpTris, 0, true, 0);
        //                            mesh.RecalculateNormals();
        //                            mesh.GetNormals(PlanetModelingManager.tmpNorms);
        //                            if (vertCount == 1) GS2.Warn("Set");
        //                            if (vertCount > 100) vertCount = 0;
        //                            for (int index4 = 0; index4 < PlanetModelingManager.tmpNorms.Count; ++index4)
        //                            {
        //                                int z = (int)PlanetModelingManager.tmpUv2s[index4].z;
        //                                int w = (int)PlanetModelingManager.tmpUv2s[index4].w;
        //                                data.normals[z] = data.normals[z] + PlanetModelingManager.tmpNorms[index4];
        //                                data.normals[w] = data.normals[w] + PlanetModelingManager.tmpNorms[index4];
        //                            }
        //                            MeshFilter meshFilter2 = gameObject.AddComponent<MeshFilter>();
        //                            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //                            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        //                            meshFilter2.sharedMesh = mesh;
        //                            meshRenderer.sharedMaterial = planet.terrainMaterial;
        //                            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        //                            meshRenderer.receiveShadows = false;
        //                            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
        //                            meshCollider.sharedMesh = mesh;
        //                            PlanetModelingManager.tmpMeshRendererList.Add(meshRenderer);
        //                            PlanetModelingManager.tmpMeshColliderList.Add(meshCollider);
        //                            ++num8;
        //                            if (num8 == num7)
        //                                return false;
        //                        }
        //                    }
        //                }
        //            }
        //            int num24 = GameMain.isLoading ? 15 : 5;
        //            for (int index1 = 0; index1 < PlanetModelingManager.tmpMeshList.Count; ++index1)
        //            {
        //                int num9 = index1 / num24;
        //                if (num9 >= PlanetModelingManager.currentModelingSeamNormal)
        //                {
        //                    if (num9 > PlanetModelingManager.currentModelingSeamNormal)
        //                    {
        //                        ++PlanetModelingManager.currentModelingSeamNormal;
        //                        return false;
        //                    }
        //                    Mesh tmpMesh = PlanetModelingManager.tmpMeshList[index1];
        //                    PlanetModelingManager.tmpNorms.Clear();
        //                    PlanetModelingManager.tmpUv2s.Clear();
        //                    int vertexCount = tmpMesh.vertexCount;
        //                    tmpMesh.GetUVs(1, PlanetModelingManager.tmpUv2s);
        //                    for (int index2 = 0; index2 < vertexCount; ++index2)
        //                    {
        //                        int z = (int)PlanetModelingManager.tmpUv2s[index2].z;
        //                        PlanetModelingManager.tmpNorms.Add(data.normals[z].normalized);
        //                    }
        //                    tmpMesh.SetNormals(PlanetModelingManager.tmpNorms);
        //                }
        //            }
        //            PlanetModelingManager.currentModelingStage = 3;
        //            break;
        //        case 3:
        //            PlanetModelingManager.tmpPlanetBodyGameObject.SetActive(true);
        //            PlanetModelingManager.tmpPlanetReformGameObject.SetActive(true);
        //            PlanetModelingManager.heightmapCamera.transform.localPosition = PlanetModelingManager.tmpPlanetGameObject.transform.localPosition;
        //            PlanetModelingManager.heightmapCamera.RenderToCubemap(planet.heightmap, 63);
        //            PlanetModelingManager.currentModelingStage = 4;
        //            break;
        //        case 4:
        //            if (planet.wanted)
        //            {
        //                planet.gameObject = PlanetModelingManager.tmpPlanetGameObject;
        //                planet.bodyObject = PlanetModelingManager.tmpPlanetBodyGameObject;
        //                PlanetSimulator component = PlanetModelingManager.tmpPlanetGameObject.GetComponent<PlanetSimulator>();
        //                component.surfaceRenderer = new Renderer[PlanetModelingManager.tmpMeshRendererList.Count];
        //                component.surfaceCollider = new Collider[PlanetModelingManager.tmpMeshColliderList.Count];
        //                for (int index = 0; index < PlanetModelingManager.tmpMeshList.Count; ++index)
        //                {
        //                    planet.meshes[index] = PlanetModelingManager.tmpMeshList[index];
        //                    planet.meshRenderers[index] = PlanetModelingManager.tmpMeshRendererList[index];
        //                    planet.meshColliders[index] = PlanetModelingManager.tmpMeshColliderList[index];
        //                }
        //                for (int index = 0; index < PlanetModelingManager.tmpMeshRendererList.Count; ++index)
        //                {
        //                    PlanetModelingManager.tmpMeshRendererList[index].gameObject.layer = 31;
        //                    PlanetModelingManager.tmpMeshRendererList[index].sharedMaterial = planet.terrainMaterial;
        //                    PlanetModelingManager.tmpMeshRendererList[index].receiveShadows = false;
        //                    PlanetModelingManager.tmpMeshRendererList[index].shadowCastingMode = ShadowCastingMode.Off;
        //                    component.surfaceRenderer[index] = (Renderer)PlanetModelingManager.tmpMeshRendererList[index];
        //                    component.surfaceCollider[index] = (Collider)PlanetModelingManager.tmpMeshColliderList[index];
        //                }
        //                component.oceanCollider = PlanetModelingManager.tmpOceanCollider;
        //                component.sphereCollider = PlanetModelingManager.tmpPlanetBodyGameObject.AddComponent<SphereCollider>();
        //                if ((UnityEngine.Object)component.sphereCollider != (UnityEngine.Object)null)
        //                    component.sphereCollider.enabled = false;
        //                component.sphereCollider.radius = planet.realRadius;
        //                component.reformRenderer = (Renderer)PlanetModelingManager.tmpPlanetReformRenderer;
        //                component.reformMat0 = planet.reformMaterial0;
        //                component.reformMat1 = planet.reformMaterial1;
        //                Material sharedMaterial = component.surfaceRenderer[0].sharedMaterial;
        //                if (planet.type != EPlanetType.Gas)
        //                {
        //                    component.reformMat0.SetColor("_AmbientColor0", sharedMaterial.GetColor("_AmbientColor0"));
        //                    component.reformMat0.SetColor("_AmbientColor1", sharedMaterial.GetColor("_AmbientColor1"));
        //                    component.reformMat0.SetColor("_AmbientColor2", sharedMaterial.GetColor("_AmbientColor2"));
        //                    component.reformMat0.SetColor("_LightColorScreen", sharedMaterial.GetColor("_LightColorScreen"));
        //                    component.reformMat0.SetFloat("_Multiplier", sharedMaterial.GetFloat("_Multiplier"));
        //                    component.reformMat0.SetFloat("_AmbientInc", sharedMaterial.GetFloat("_AmbientInc"));
        //                    component.reformMat1.SetColor("_AmbientColor0", sharedMaterial.GetColor("_AmbientColor0"));
        //                    component.reformMat1.SetColor("_AmbientColor1", sharedMaterial.GetColor("_AmbientColor1"));
        //                    component.reformMat1.SetColor("_AmbientColor2", sharedMaterial.GetColor("_AmbientColor2"));
        //                    component.reformMat1.SetColor("_LightColorScreen", sharedMaterial.GetColor("_LightColorScreen"));
        //                    component.reformMat1.SetFloat("_Multiplier", sharedMaterial.GetFloat("_Multiplier"));
        //                    component.reformMat1.SetFloat("_AmbientInc", sharedMaterial.GetFloat("_AmbientInc"));
        //                }
        //                PlanetModelingManager.tmpPlanetGameObject.transform.localPosition = Vector3.zero;
        //                PlanetModelingManager.heightmapCamera.transform.localPosition = Vector3.zero;
        //                PlanetModelingManager.tmpPlanetBodyGameObject.SetActive(true);
        //                PlanetModelingManager.tmpPlanetReformGameObject.SetActive(true);
        //                PlanetModelingManager.tmpPlanetGameObject = (GameObject)null;
        //                PlanetModelingManager.tmpPlanetBodyGameObject = (GameObject)null;
        //                PlanetModelingManager.tmpPlanetReformGameObject = (GameObject)null;
        //                PlanetModelingManager.tmpPlanetReformRenderer = (MeshRenderer)null;
        //                PlanetModelingManager.tmpMeshList.Clear();
        //                PlanetModelingManager.tmpMeshRendererList.Clear();
        //                PlanetModelingManager.tmpMeshColliderList.Clear();
        //                PlanetModelingManager.tmpOceanCollider = (Collider)null;
        //                PlanetModelingManager.tmpTris.Clear();
        //                PlanetModelingManager.tmpVerts.Clear();
        //                PlanetModelingManager.tmpNorms.Clear();
        //                PlanetModelingManager.tmpTgnts.Clear();
        //                PlanetModelingManager.tmpUvs.Clear();
        //                PlanetModelingManager.tmpUv2s.Clear();
        //                PlanetModelingManager.currentModelingPlanet = (PlanetData)null;
        //                PlanetModelingManager.currentModelingStage = 0;
        //                PlanetModelingManager.currentModelingSeamNormal = 0;
        //                planet.NotifyLoaded();
        //                if (!planet.star.loaded)
        //                    break;
        //                planet.star.NotifyLoaded();
        //                break;
        //            }
        //            for (int index = 0; index < PlanetModelingManager.tmpMeshList.Count; ++index)
        //                UnityEngine.Object.Destroy((UnityEngine.Object)PlanetModelingManager.tmpMeshList[index]);
        //            UnityEngine.Object.Destroy((UnityEngine.Object)PlanetModelingManager.tmpPlanetGameObject);
        //            PlanetModelingManager.tmpPlanetGameObject = (GameObject)null;
        //            PlanetModelingManager.tmpPlanetBodyGameObject = (GameObject)null;
        //            PlanetModelingManager.tmpPlanetReformGameObject = (GameObject)null;
        //            PlanetModelingManager.tmpPlanetReformRenderer = (MeshRenderer)null;
        //            PlanetModelingManager.tmpMeshList.Clear();
        //            PlanetModelingManager.tmpTris.Clear();
        //            PlanetModelingManager.tmpVerts.Clear();
        //            PlanetModelingManager.tmpNorms.Clear();
        //            PlanetModelingManager.tmpTgnts.Clear();
        //            PlanetModelingManager.tmpUvs.Clear();
        //            PlanetModelingManager.tmpUv2s.Clear();
        //            PlanetModelingManager.currentModelingPlanet = (PlanetData)null;
        //            PlanetModelingManager.currentModelingStage = 0;
        //            PlanetModelingManager.currentModelingSeamNormal = 0;
        //            break;
        //    }
        //    return false;

        //}
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetModelingManager), "ModelingPlanetMain")]
        public static IEnumerable<CodeInstruction> ModelingPlanetMainTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionList = new List<CodeInstruction>(instructions);

            //Patch.Debug("ModelingPlanetMain Transpiler.", LogLevel.Debug, Patch.DebugPlanetModelingManagerDeep);
            for (var instructionCounter = 0; instructionCounter < instructionList.Count; instructionCounter++)
                if (instructionList[instructionCounter].Calls(typeof(PlanetData).GetProperty("realRadius").GetGetMethod()))
                {
                    //Patch.Debug("Found realRadius Property getter call.", LogLevel.Debug, Patch.DebugPlanetModelingManagerDeep);
                    if (instructionCounter + 4 < instructionList.Count && instructionList[instructionCounter + 1].opcode == OpCodes.Ldc_R4 && instructionList[instructionCounter + 1].OperandIs(0.2f) && instructionList[instructionCounter + 2].opcode == OpCodes.Add && instructionList[instructionCounter + 3].opcode == OpCodes.Ldc_R4 && instructionList[instructionCounter + 3].OperandIs(0.025f))
                    {
                        //Patch.Debug("Found THE CORRECT realRadius Property getter call.", LogLevel.Debug, Patch.DebugPlanetModelingManagerDeep);
                        //+1 = ldc.r4 0.2
                        //+2 = add
                        //+3 = ldc.r4 0.025 <-- replace
                        instructionList.RemoveAt(instructionCounter + 3);
                        var toInsert = new List<CodeInstruction>
                        {
                            new(OpCodes.Ldarg_0),
                            new(instructionList[instructionCounter]),
                            new(OpCodes.Ldc_R4, 8000f),
                            new(OpCodes.Div)
                        };
                        instructionList.InsertRange(instructionCounter + 3, toInsert);
                    }
                }
                else if (instructionList[instructionCounter].Calls(typeof(PlanetRawData).GetMethod("GetModPlane")))
                {
                    //GS2.Log("Found GetModPlane callvirt. Replacing with GetModPlaneInt call.");
                    instructionList[instructionCounter] = new CodeInstruction(OpCodes.Call, typeof(PlanetRawDataExtension).GetMethod("GetModPlaneInt"));
                }

            return instructionList.AsEnumerable();
        }
    }
}