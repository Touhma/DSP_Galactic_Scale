using System;
using System.Collections.Generic;
using PCGSharp;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS3
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
                //GS3.Warn(result.ToString());
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

                if (maxExclusive <= 0)
                {
                    Error($"Max {maxExclusive} <= 0 {GetCaller()}");
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
            public float ClampedNormal(float min, float max, int bias)
            {
                var range = max - min;
                var average = bias / 100f * range + min;
                var sdHigh = (max - average) / 3;
                var sdLow = (average - min) / 3;
                var sd = Math.Max(sdLow, sdHigh);
                var rResult = Normal(average, sd);
                var result = Mathf.Clamp(rResult, min, max);
                //Warn($"ClampedNormal min:{min} max:{max} bias:{bias} range:{range} average:{average} sdHigh:{sdHigh} sdLow:{sdLow} sd:{sd} fResult:{fResult} result:{result}");
                return result;
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
                if (items.Count == 0)
                {
                    Error($"Item Length 0 {GetCaller()}");
                }
                return items[Next(items.Count)];
            }

            public (int, T) ItemWithIndex<T>(List<T> items)
            {
                if (items.Count == 0)
                {
                    Error($"Item Length 0 {GetCaller()}");
                }
                var n = Next(items.Count);
                return (n, items[n]);
            }

            public T ItemAndRemove<T>(List<T> items)
            {
                if (items.Count == 0)
                {
                    Error($"Item Length 0 {GetCaller()}");
                }
                var n = Next(items.Count);
                var result = items[n];
                items.RemoveAt(n);
                return result;
            }
            public T Item<T>(T[] items)
            {
                if (items.Length == 0)
                {
                    Error($"Item Length 0 {GetCaller()}");
                }
                return items[Next(items.Length)];
            }

            public (int, T) ItemWithIndex<T>(T[] items)
            {
                if (items.Length == 0)
                {
                    Error($"Item Length 0 {GetCaller()}");
                }
                var n = Next(items.Length);
                return (n, items[n]);
            }

            public KeyValuePair<W, X> Item<W, X>(Dictionary<W, X> items)
            {
                if (items.Count == 0)
                {
                    Error($"Item Length 0 {GetCaller()}");
                }
                W[] keys = { };
                items.Keys.CopyTo(keys, 0);
                var key = keys[Next(keys.Length)];
                return new KeyValuePair<W, X>(key, items[key]);
            }
        }
    }
}