using UnityEngine;
using System.Collections.Generic;

namespace GalacticScale
{
    public class GSMaterialSettings
    {
        public string Path;
        public string CopyFrom;
        public Dictionary<string, Color> Colors = new Dictionary<string, Color>();
        public Dictionary<string, float> Params = new Dictionary<string, float>();
        public Color Tint;
        public GSMaterialSettings Clone()
        {
            GSMaterialSettings clone = new GSMaterialSettings();
            clone.Path = Path;
            clone.Tint = Tint;
            clone.CopyFrom = CopyFrom;
            foreach (var kvp in Colors) clone.Colors.Add(kvp.Key, kvp.Value);
            foreach (var kvp in Params) clone.Params.Add(kvp.Key, kvp.Value);
            return clone;
        }
    }
}