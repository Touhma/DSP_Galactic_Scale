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
            if (__instance.materials != null)
                foreach (var mat in __instance.materials)
                {
                    var replacementShader = CustomShaderManager.LookupReplacementShaderFor(mat.shader.name);
                    if (replacementShader != null)
                    {
                        GS2.Log($"Replacing shader {mat.shader.name} on material {mat.name} with {replacementShader.shaderName}");
                        CustomShaderManager.ApplyCustomShaderToMaterial(mat, replacementShader);
                    }
                }

            if (__instance.lodMaterials != null)
                for (int i = 0; i < __instance.lodMaterials.Length; i++)
                {
                    if (__instance.lodMaterials[i] != null)
                        foreach (var mat in __instance.lodMaterials[i])
                        {
                            var replacementShader = CustomShaderManager.LookupReplacementShaderFor(mat.shader.name); 
                            if (replacementShader != null)
                            {
                                GS2.Log($"Replacing shader {mat.shader.name} on material {mat.name} (lod{i}) with {replacementShader.shaderName}");
                                CustomShaderManager.ApplyCustomShaderToMaterial(mat, replacementShader);
                            }
                        }
                }
        }
    }
}