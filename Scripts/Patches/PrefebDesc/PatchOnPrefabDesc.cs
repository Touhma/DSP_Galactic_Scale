using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnPrefebDesc
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PrefabDesc), "ReadPrefab")]
        public static void ReadPrefab(GameObject _prefab, GameObject _colliderPrefab, ref PrefabDesc __instance)
        {
            string shadername = "VF Shaders/Forward/Unlit Additive Mining Drill MK2";
            Shader replacementShader = CustomShader.GetShader("VF Shaders/Forward/Unlit Additive Mining Drill MK2 Replace");
            if (replacementShader == null)
                GS2.Log("custom shader not found");

            List<Material> replaceShaderMats = new List<Material>();

            if (__instance.materials != null)
                foreach (var mat in __instance.materials)
                {
                    if (mat.shader.name == shadername)
                    {
                        GS2.Log($"Shader match: {mat.name}: {mat.shader.name}");
                        mat.shader = replacementShader;
                    }
                }

            if (__instance.lodMaterials != null)
                for (int i = 0; i < __instance.lodMaterials.Length; i++)
                {
                    if (__instance.lodMaterials[i] != null)
                        foreach (var mat in __instance.lodMaterials[i])
                        {
                            if (mat.shader.name == shadername)
                            {
                                foreach (var localKeywordName in mat.shaderKeywords)
                                {
                                    Debug.Log("Before Local shader keyword " + localKeywordName + " is currently enabled");
                                }
                                GS2.Log($"Shader match: {mat.name} (lod{i}): {mat.shader.name}");
                                mat.shader = replacementShader;
                                foreach (var localKeywordName in mat.shaderKeywords)
                                {
                                    Debug.Log("After Local shader keyword " + localKeywordName + " is currently enabled");
                                }
                            }
                        }
                }
        }
    }
}