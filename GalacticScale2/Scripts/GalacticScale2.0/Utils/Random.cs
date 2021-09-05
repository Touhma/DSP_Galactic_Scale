using System;
using System.Collections.Generic;
using PCGSharp;

namespace GalacticScale
{
    public static partial class GS2
    {
        public class Random : Pcg
        {
            private int count;
            private int seed = 1;

            public Random(int seed) : base(seed)
            {
                //Warn($"Creating new Random based on seed:{seed}");
                //Warn($"Generating=>{Id}=> {GetCaller(1)}=>{GetCaller()}");
                Seed = seed;
            }

            public int Seed
            {
                get => seed;
                set
                {
                    seed = value;
                    reseed(value);
                }
            }

            public string Id => $"[{seed}=>{count}]";

            public bool NextPick(double chance)
            {
                return NextDouble() < chance;
            }

            public override uint NextUInt()
            {
                count++;
                //if (count > 10) 
                //Warn($"Generating=>{Id}=> {GetCaller(1)}=>{GetCaller()}");
                return base.NextUInt();
            }

            public float Normal(float averageValue, float standardDeviation)
            {
                return averageValue + standardDeviation * (float)(Math.Sqrt(-2.0 * Math.Log(1.0 - NextDouble())) * Math.Sin(2.0 * Math.PI * NextDouble()));
            }

            //public float RangePlusMinusOne() => UnityEngine.Mathf.Sin((float)(NextDouble() * (2 * UnityEngine.Mathf.PI)));
            public new float NextFloat(float min, float max)
            {
                //Log($"{Id} NextFloat {min} {max} {GetCaller()}");
                if (Math.Abs(min - max) < float.MinValue) return min;
                if (min > max)
                {
                    Warn($"{GetCaller()}-NextFloat: Min > Max. {min} {max}");
                    return max;
                }

                var result = base.NextFloat(min, max);
                //GS2.Warn(result.ToString());
                return result;
            }

            public new int Next(int minInclusive, int maxExclusive)
            {
                //Log($"{Id} Next {min} {max} {GetCaller()}");
                if (minInclusive == maxExclusive) return minInclusive;
                if (minInclusive > maxExclusive)
                {
                    Error($"Next: Min > Max. {minInclusive} {maxExclusive}");
                    return maxExclusive - 1;
                }

                if (maxExclusive < 0)
                {
                    Error($"Max {maxExclusive} < 0");
                    return maxExclusive;
                }

                return base.Next(minInclusive, maxExclusive);
            }

            public int NextInclusive(int minInclusive, int maxInclusive)
            {
                //Log($"{Id} Next {min} {max} {GetCaller()}");
                if (minInclusive == maxInclusive) return minInclusive;
                if (minInclusive > maxInclusive)
                {
                    Error($"Next: Min > Max. {minInclusive} {maxInclusive}");
                    return maxInclusive;
                }

                return base.Next(minInclusive, maxInclusive + 1);
            }

            public VectorLF3 PointOnSphere(double radius)
            {
                var z = NextDouble() * 2 * radius - radius;
                var phi = NextDouble() * Math.PI * 2;
                var x = Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(z, 2)) * Math.Cos(phi);
                var y = Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(z, 2)) * Math.Sin(phi);
                return new VectorLF3(x, y, z);
            }

            public T Item<T>(List<T> items)
            {
                return items[Next(items.Count)];
            }

            public (int, T) ItemWithIndex<T>(List<T> items)
            {
                var n = Next(items.Count);
                return (n, items[n]);
            }

            public T Item<T>(T[] items)
            {
                return items[Next(items.Length)];
            }

            public (int, T) ItemWithIndex<T>(T[] items)
            {
                var n = Next(items.Length);
                return (n, items[n]);
            }

            public KeyValuePair<W, X> Item<W, X>(Dictionary<W, X> items)
            {
                W[] keys = { };
                items.Keys.CopyTo(keys, 0);
                var key = keys[Next(keys.Length)];
                return new KeyValuePair<W, X>(key, items[key]);
            }
        }
    }
}