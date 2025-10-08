using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Utils
    {
        private static readonly Dictionary<string, Shader> ReplaceShaderMap = new();
        
        public static void ReplaceShaderIfAvailable(Material mat)
        {
            var oriShaderName = mat.shader.name;
            if (ReplaceShaderMap.TryGetValue(oriShaderName, out var replacementShader))
            {
                mat.shader = replacementShader;
            }
        }

        internal static void AddSwapShaderMapping(string oriShaderName, Shader replacementShader)
        {
            if (replacementShader == null)
                GS2.Error("replacementShader is null");
        
            ReplaceShaderMap.Add(oriShaderName, replacementShader);
        }
    }
}