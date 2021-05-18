using UnityEngine;
using System.Collections.Generic;
using System;
using FullSerializer;

namespace GalacticScale
{
     public class GSVeinSettings
    {
        public List<GSVeinType> VeinTypes = new List<GSVeinType>();
        public string VeinAlgorithm = "GS2";
        public float VeinPadding = 1f;
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