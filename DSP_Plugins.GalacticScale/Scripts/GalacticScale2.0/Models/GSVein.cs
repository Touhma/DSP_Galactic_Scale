using UnityEngine;
using System.Collections.Generic;
using System;

namespace GalacticScale
{
    public class GSVein
    {
        public int amount;
        public Vector3 position;
        public GSVein(int amount, Vector3 position)
        {
            this.amount = amount;
            this.position = position;
        }

        public GSVein (PlanetData planet, int seed = -1)
        {
            if (seed < 0) seed = GSSettings.Seed;
            System.Random random = new System.Random(seed);
            this.amount = (int)(random.Next(50000, 150000) * planet.star.resourceCoef);
            this.position = Vector3.zero;
        }
        public GSVein ()
        {
            this.amount = 0;
            this.position = Vector3.zero;
        }
    }
    public class GSVeinGroup
    {
        public List<GSVein> veins = new List<GSVein>();
        public EVeinType type = EVeinType.None;
        public int Count { get => veins.Count; }
        public int density = -1;
        [NonSerialized]
        public PlanetData planet;
    }
    public class GSVeinSettings
    {
        public List<GSVeinGroup> veinGroups = new List<GSVeinGroup>();
        public string algorithm = "GS2";
        public GSVeinSettings ()
        {

        }
    }
}