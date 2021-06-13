using System;

namespace GalacticScale {
    public static partial class GS2 {
        public class Random : System.Random {
            public float NextFloat() => (float)NextDouble();
            public float NextFloat(float max) => (float)Range(0f, max);
            public bool NextBool(double chance) => (NextDouble() < chance);
            public bool NextBool() => (NextDouble() < 0.5);

            public int Range(int min, int max) => (UnityEngine.Mathf.RoundToInt((float)Range(min, (float)max)));

            public float Range(float min, float max) => (float)Math.Round(min + (NextDouble() * (max - min)), 8);

            public float Normal(float averageValue, float standardDeviation) => averageValue + standardDeviation * (float)(Math.Sqrt(-2.0 * Math.Log(1.0 - NextDouble())) * Math.Sin(2.0 * Math.PI * NextDouble()));

            public float RangePlusMinusOne() => UnityEngine.Mathf.Sin((float)(NextDouble() * (2 * UnityEngine.Mathf.PI)));

            public VectorLF3 PointOnSphere(double radius) {
                double z = (NextDouble() * 2 * radius) - radius;
                double phi = NextDouble() * Math.PI * 2;
                double x = Math.Sqrt((Math.Pow(radius, 2) - Math.Pow(z, 2))) * Math.Cos(phi);
                double y = Math.Sqrt((Math.Pow(radius, 2) - Math.Pow(z, 2))) * Math.Sin(phi);
                return new VectorLF3(x, y, z);
            }
            public Random(int seed) : base(seed) { }
            private Random() { }
        }
    } 
}