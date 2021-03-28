using System;
using UnityEngine;
using Random = System.Random;

namespace GalacticScale.Scripts {
    public static class MathUtils {
        public static float RangePlusMinusOne(Random mainSeed) {
            // will return a number between -1 and 1 
            return Mathf.Sin((float) (mainSeed.NextDouble() * (2 * Mathf.PI)));
        }
        
        public static float RandNormal(
            float averageValue,
            float standardDeviation,
            double r1,
            double r2)
        {
            return averageValue + standardDeviation * (float) (Math.Sqrt(-2.0 * Math.Log(1.0 - r1)) * Math.Sin(2.0 * Math.PI * r2));
        }
    }
}