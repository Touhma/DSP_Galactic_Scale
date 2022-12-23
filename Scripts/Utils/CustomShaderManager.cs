using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace GalacticScale
{
    public static class CustomShaderManager
    {
        private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(GS2)).Location);
        private static AssetBundle bundle;
        private static readonly List<Shader> bundleShaders = new();
        private static readonly List<CustomShaderDesc> customShaderDescs = new();
        private static readonly Dictionary<string, CustomShaderDesc> shortNameMap = new();
        private static readonly Dictionary<string, CustomShaderDesc> replacementForShaderMap = new();
        private static readonly Dictionary<CustomShaderDesc, List<Material>> shaderReplacedOnMaterialsMap = new();

        public static void InitWithBundle(string bundleFileName)
        {
            if (bundleShaders.Count > 0)
            {
                GS2.Error($"CustomShaderManager is already initialized with bundle: {bundle.name}");
                return;
            }
            var path = Path.Combine(AssemblyPath, bundleFileName);
            if (File.Exists(path))
            {
                bundle = AssetBundle.LoadFromFile(path);
                InitWithBundle(bundle);
            }
            else GS2.Error($"Bundle file not found at: {path}");
        }

        public static void InitWithBundle(AssetBundle assetBundle)
        {
            if (bundleShaders.Count > 0)
            {
                GS2.Error($"CustomShaderManager is already initialized with bundle: {bundle.name}");
                return;
            }
            bundle = assetBundle;
            if (!LoadShadersFromBundle())
            {
                GS2.Error("Failed to load custom shaders from bundle.");
                return;
            }

            InitShaderDescs();
        }

        private static bool LoadShadersFromBundle()
        {
            GS2.Log("Loading custom shaders from bundle.");
            if (bundle != null)
            {
                var shaders = bundle.LoadAllAssets<Shader>();
                foreach (var s in shaders)
                {
                    bundleShaders.Add(s);
                    GS2.Log($"Loaded custom shader: {s.name}");
                }
            }
            else
            {
                GS2.Error("Failed to load custom shaders from bundle".Translate());
                return false;
            }

            return true;
        }

        private static void InitShaderDescs()
        {

            AddCustomShaderDesc(
                "mk2effect",
                "VF Shaders/Forward/Unlit Additive Mining Drill MK2",
                "VF Shaders/Forward/Unlit Additive Mining Drill MK2 Replace",
                new Dictionary<string, EShaderPropType> 
                { 
                    {"_Radius", EShaderPropType.Float}
                }
            );
        }

        private static void AddCustomShaderDesc(string shortName, string shaderToReplace, string replacementShader,
            Dictionary<string, EShaderPropType> addedProps)
        {
            CustomShaderDesc shaderDesc = new(shortName, shaderToReplace, replacementShader, addedProps);
            customShaderDescs.Add(shaderDesc);
            replacementForShaderMap.Add(shaderDesc.replacementForShader, shaderDesc);
            shortNameMap.Add(shaderDesc.shortName, shaderDesc);

        }

        public static CustomShaderDesc LookupReplacementShaderFor(string originalShaderName)
        {
            return replacementForShaderMap.TryGetValue(originalShaderName, out CustomShaderDesc customShader) ? customShader : null;
        }

        public static Shader GetShader (string customShaderName)
        {
            foreach (var shader in bundleShaders)
            {
                if (shader.name.Equals(customShaderName)) return shader;
            }
            GS2.Warn($"Couldn't find custom shader with name: {customShaderName}");
            return null;
        }

        public static CustomShaderDesc GetCustomShaderDescByShortName(string shortName)
        {
            if (!shortNameMap.TryGetValue(shortName, out CustomShaderDesc csd))
            {
                GS2.Error($"CustomShaderDesc with ShortName: {shortName} not found");
                return null;
            }

            return csd;
        }

        public static void ApplyCustomShaderToMaterial(Material mat, CustomShaderDesc replacementShader)
        {
            mat.shader = replacementShader.shader;

            if(!shaderReplacedOnMaterialsMap.TryGetValue(replacementShader, out var matList))
            {
                matList = new List<Material>();
                shaderReplacedOnMaterialsMap.Add(replacementShader, matList);
            }

            matList.Add(mat);
        }

        public static void SetPropByShortName(string propName, float propVal, string shortName)
        {
            CustomShaderDesc csd = GetCustomShaderDescByShortName(shortName);
            if (csd == null)
            {
                GS2.Error($"No CustomShaderDesc found with shortName: {shortName}");
                return;
            }

            var propType = csd.TypeOfAddedProperty(propName);
            if (propType == null)
            {
                GS2.Error($"CustomShaderDesc has no AddedProperty named: {propName}");
                return;

            }

            if (propType != typeof(float))
            {
                GS2.Error($"Property {propName} is of type {csd.TypeOfAddedProperty(propName)} but value provided is float");
            }

            foreach (var mat in shaderReplacedOnMaterialsMap[csd])
            {
                mat.SetFloat(propName, propVal);
            }
        }
    }
}