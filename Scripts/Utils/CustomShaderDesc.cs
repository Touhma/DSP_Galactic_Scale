using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public class CustomShaderDesc
    {
        public readonly string shortName;
        public readonly Shader shader;
        public readonly string shaderName;
        public readonly string replacementForShader;
        public readonly Dictionary<string, EShaderPropType> addedProperties;

        public CustomShaderDesc (string shortName, string shaderToReplace, string replacementShader, Dictionary<string, EShaderPropType> addedProps)
        {
            shader = CustomShaderManager.GetShader(replacementShader);
            if (shader == null) GS2.Error($"Could not find shader for name: {replacementShader}");
            shaderName = replacementShader;
            replacementForShader = shaderToReplace;
            addedProperties = addedProps;
            this.shortName = shortName;
        }

        public Type TypeOfAddedProperty(string propName)
        {
            if (!addedProperties.TryGetValue(propName, out var ePropType))
            {
                GS2.Warn($"{shaderName} has no added property named {propName}.");
                return null;
            }

            return ePropType switch
            {
                EShaderPropType.Buffer => typeof(ComputeBuffer),
                EShaderPropType.Color => typeof(Color),
                EShaderPropType.Float => typeof(float),
                EShaderPropType.Int => typeof(int),
                EShaderPropType.Matrix => typeof(Matrix4x4),
                EShaderPropType.Vector => typeof(Vector4),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public enum EShaderPropType
    {
        Buffer,
        Color,
        Float,
        Int,
        Matrix,
        Vector
    }
}