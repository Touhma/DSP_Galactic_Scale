using UnityEngine;
using Random = System.Random;

namespace GalacticScale.Scripts.PatchStarSystemGeneration.Utils {
    public static class MathUtils {
        public static float RangePlusMinusOne(Random mainSeed) {
            // will return a number between -1 and 1 
            return Mathf.Sin((float) (mainSeed.NextDouble() * (2 * Mathf.PI)));
        }
    }
}