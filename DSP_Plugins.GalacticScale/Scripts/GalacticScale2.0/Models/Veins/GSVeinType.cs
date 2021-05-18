using UnityEngine;
using System.Collections.Generic;
using System;
using FullSerializer;

namespace GalacticScale
{
    [fsObject(Converter = typeof(GSFSVeinTypeConverter))]
    public class GSVeinType
    {
        public List<GSVein> veins = new List<GSVein>();
        public EVeinType type = EVeinType.None;
        public bool rare = false;
        public int generate;
        public int Count { get => veins.Count; }
        [NonSerialized]
        public PlanetData planet;
        public GSVeinType Clone()
        {
            GSVeinType clone = (GSVeinType)MemberwiseClone();
            clone.veins = new List<GSVein>();
            for (var i = 0; i < veins.Count; i++) clone.veins.Add(veins[i].Clone());
            return clone;
        }
        public GSVeinType (EVeinType type)
        {
            this.type = type;
        }
        public GSVeinType() { }
    }

}