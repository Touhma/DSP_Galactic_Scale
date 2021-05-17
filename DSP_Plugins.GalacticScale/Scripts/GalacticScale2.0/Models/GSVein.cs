using UnityEngine;
using System.Collections.Generic;
using System;

namespace GalacticScale
{
    public class GSVein
    {
        public float richness;
        public int count;
        [NonSerialized]
        public Vector3 position;
        //public float density = -1f;
        public GSVein(float richness, Vector3 position, int count)
        {
            this.richness = richness;
            this.position = position;
            this.count = count;
            //this.density = density;
    }

        public GSVein (GSPlanet gsPlanet, int seed = -1)
        {
            if (seed < 0) seed = GSSettings.Seed;
            GS2.Random random = new GS2.Random(seed);
            this.richness =  (float)random.NextDouble() * gsPlanet.planetData.star.resourceCoef;//(int)(random.Next(50000, 150000)
            this.count = (int)random.Next(1, 30);
            this.position = Vector3.zero;
        }
        public GSVein ()
        {
            this.richness = 0;
            this.count = 0;
            this.position = Vector3.zero;
        }
        public GSVein Clone()
        {
            return (GSVein)MemberwiseClone();
        }
    }
    public class GSVeinType
    {
        public List<GSVein> veins = new List<GSVein>();
        public EVeinType type = EVeinType.None;
        public bool rare = false;
        public int Count { get => veins.Count; }
        [NonSerialized]
        public PlanetData planet;
        public GSVeinType Clone()
        {
            GSVeinType clone = (GSVeinType)this.MemberwiseClone();
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


    public class GSVeinDescriptor
    {
        public EVeinType type;
        public int count;
        public float richness;
        public float density;
        public Vector3 position;
        public bool rare;
    }
}