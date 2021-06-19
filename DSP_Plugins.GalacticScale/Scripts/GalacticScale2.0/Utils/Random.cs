using PCGSharp;
using System;

namespace GalacticScale {
    public static partial class GS2 {
        public class Random : Pcg {
            public int Seed { 
                get => seed; 
                set { 
                    this.seed = value; 
                    this.reseed(value); 
                } 
            }
            private int seed = 1;
            public int count = 0;
            public string Id { get => $"[{seed}=>{count}]"; }
            public bool NextPick(double chance) => NextDouble() < chance;
            public override uint NextUInt() {
                count++;
                //if (count > 10) 
                    //Warn($"Generating=>{Id}=> {GetCaller(1)}=>{GetCaller()}");
                return base.NextUInt();
            }
            public float Normal(float averageValue, float standardDeviation) => averageValue + standardDeviation * (float)(Math.Sqrt(-2.0 * Math.Log(1.0 - NextDouble())) * Math.Sin(2.0 * Math.PI * NextDouble()));

            //public float RangePlusMinusOne() => UnityEngine.Mathf.Sin((float)(NextDouble() * (2 * UnityEngine.Mathf.PI)));
            public new float NextFloat(float min, float max) {
                //Log($"{Id} NextFloat {min} {max} {GetCaller()}");
                if (min == max) return min;
                if (min > max) {
                    Warn($"{GetCaller()}-NextFloat: Min > Max. {min} {max}");
                    return max;
                }
                float result = base.NextFloat(min, max);
                //GS2.Warn(result.ToString());
                return result;
            }
            public new int Next(int min, int max) {
                //Log($"{Id} Next {min} {max} {GetCaller()}");
                if (min == max) return min;
                if (min > max) {
                    Error($"Next: Min > Max. {min} {max}");
                    return max;
                }
                return base.Next(min, max);
            }
            public VectorLF3 PointOnSphere(double radius) {
                double z = (NextDouble() * 2 * radius) - radius;
                double phi = NextDouble() * Math.PI * 2;
                double x = Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(z, 2)) * Math.Cos(phi);
                double y = Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(z, 2)) * Math.Sin(phi);
                return new VectorLF3(x, y, z);
            }
            public Random(int seed) : base(seed) {
                //Warn($"Creating new Random based on seed:{seed}");
                //Warn($"Generating=>{Id}=> {GetCaller(1)}=>{GetCaller()}");
                this.Seed = seed;
            }
        }
    }
}