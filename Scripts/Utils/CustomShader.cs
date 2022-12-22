using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace GalacticScale
{

    public class CustomShader
    {
        private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(GS2)).Location);
        private static AssetBundle bundle;
        public static List<string> shaderNames = new();
        public static Dictionary<string, Shader> bundleShaders = new();

        public static AssetBundle Bundle
        {
            get
            {
                if (bundle == null)
                {
                    GS2.Log("Loading customshader bundle.");
                    var path = Path.Combine(AssemblyPath, "customshader");
                    if (File.Exists(path)) bundle = AssetBundle.LoadFromFile(path);
                    var shaders = bundle.LoadAllAssets<Shader>();
                    for (int i = 0; i < shaders.Length; i++)
                    {
                        bundleShaders.Add(shaders[i].name, shaders[i]);
                        GS2.Warn($"Loaded Shader: {shaders[i].name}");
                    }
                }

                if (bundle == null)
                {
                    GS2.Error("Failed to load customshader bundle!".Translate());
                    return null;
                }

                return bundle;
            }
        }

        public static Shader GetShader(string name)
        {
            if (bundleShaders.Count == 0) _ = Bundle;
            return bundleShaders[name];
        }

    }
}