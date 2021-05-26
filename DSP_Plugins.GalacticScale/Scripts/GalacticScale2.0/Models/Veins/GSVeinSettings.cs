using UnityEngine;
using System.Collections.Generic;
using System;
using FullSerializer;

namespace GalacticScale
{
     public class GSVeinSettings
    {
        public List<GSVeinType> VeinTypes = new List<GSVeinType>();
        public string Algorithm = "Vanilla";
        public float VeinPadding = 1f;

        public bool Enabled { get => VeinTypes.Count > 0; }
        public GSVeinSettings ()
        {

        }
        public GSVeinSettings Clone()
        {
            GSVeinSettings clone = (GSVeinSettings)this.MemberwiseClone();
            clone.VeinTypes = new List<GSVeinType>();
            for (var i = 0; i < VeinTypes.Count; i++) clone.VeinTypes.Add(VeinTypes[i].Clone());
            return clone;
        }
    }
}