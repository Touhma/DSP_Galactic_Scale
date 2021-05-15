using UnityEngine;
using System.Collections.Generic;
using System;

namespace GalacticScale
{
    public class GSVein
    {
        public float richness;
        public int count;
        public Vector3 position;
        public float density = -1f;
        public GSVein(float richness, Vector3 position, int count, float density)
        {
            this.richness = richness;
            this.position = position;
            this.count = count;
            this.density = density;
    }

        public GSVein (PlanetData planet, int seed = -1)
        {
            if (seed < 0) seed = GSSettings.Seed;
            System.Random random = new System.Random(seed);
            this.richness = (int)(random.Next(50000, 150000) * planet.star.resourceCoef);
            this.count = (int)random.Next(1, 30);
            this.position = Vector3.zero;
        }
        public GSVein ()
        {
            this.richness = 0;
            this.count = 0;
            this.position = Vector3.zero;
        }
    }
    public class GSVeinType
    {
        public List<GSVein> veins = new List<GSVein>();
        public EVeinType type = EVeinType.None;
        public int Count { get => veins.Count; }
        [NonSerialized]
        public PlanetData planet;
    }
    public class GSVeinSettings
    {
        public List<GSVeinType> VeinTypes = new List<GSVeinType>();
        public string VeinAlgorithm = "GS2";
        public float VeinPadding = 1f;
        public GSVeinSettings ()
        {

        }
    }
    public class GSVeinData
    {
        public EVeinType type;
        public int count;
        public float richness;
        public float density;
        public Vector3 position;
    }
}