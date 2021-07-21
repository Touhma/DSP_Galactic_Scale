﻿using GSSerializer;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public class GSMaterialSettings
    {
        [SerializeField] public Dictionary<string, Color> Colors = new Dictionary<string, Color>();

        [SerializeField] public string CopyFrom;

        [SerializeField] public Dictionary<string, float> Params = new Dictionary<string, float>();

        [SerializeField] public string Path;

        [SerializeField] public Dictionary<string, string> Textures = new Dictionary<string, string>();

        [SerializeField] public Color Tint;
        public bool Equals(GSMaterialSettings other)
        {
            var serializer = new fsSerializer();
            serializer.TrySerialize(this, out var thisData).AssertSuccessWithoutWarnings();
            serializer.TrySerialize(this, out var otherData).AssertSuccessWithoutWarnings();
            // GS2.Warn($"MaterialSettings Equals: {thisData.Equals(otherData)}");
            return thisData.Equals(otherData);
        }
        public GSMaterialSettings Clone()
        {
            var clone = new GSMaterialSettings();
            clone.Path = Path;
            clone.Tint = Tint;
            clone.CopyFrom = CopyFrom;
            foreach (var kvp in Colors) clone.Colors.Add(kvp.Key, kvp.Value);

            foreach (var kvp in Params) clone.Params.Add(kvp.Key, kvp.Value);

            foreach (var kvp in Textures) clone.Textures.Add(kvp.Key, kvp.Value);

            return clone;
        }
    }
}