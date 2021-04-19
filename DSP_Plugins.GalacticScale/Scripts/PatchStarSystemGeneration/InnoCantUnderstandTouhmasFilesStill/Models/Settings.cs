using FullSerializer;
using System;
using System.Collections.Generic;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale
{
    public class settings
    {
        private static settings instance = new settings();
        [SerializeField]
        private int seed = 1;
        [SerializeField]
        private  List<star> stars = new List<star>();
        [SerializeField]
        private galaxyParams galaxyParams = new galaxyParams();



        public static int Seed { get => instance.seed; set => instance.seed = value; }
        public static List<star> Stars { get => instance.stars; set => instance.stars = value; }
        public static int starCount { get => Stars.Count; }
        public static star BirthStar { get => Stars[0]; set => Stars[0] = value; }
        public static galaxyParams GalaxyParams { get => instance.galaxyParams; set => instance.galaxyParams = value; }
        static settings()
        {

        }
        private settings()
        {

        }
        public static settings Instance { get { return instance; } set { instance = value; } }
      
        public static void set(galaxyParams galaxyParams, int seed, star birthStar, List<star> stars)
        {
            GalaxyParams = galaxyParams;
            BirthStar = birthStar;
            Stars = stars;
            Seed = seed;
        }
        public static void reset()
        {
            GalaxyParams = new galaxyParams();
            Stars.Clear();
        }
    }
    public class galaxyParams
    {
        public int iterations = 4;
        public double minDistance = 2;
        public double minStepLength = 2.3;
        public double maxStepLength = 3.5;
        public double flatten = 0.18;
    }
}