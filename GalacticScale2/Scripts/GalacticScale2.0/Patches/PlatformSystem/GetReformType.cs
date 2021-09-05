using System;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlatformSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlatformSystem), "GetReformType")] //Increase size of array
        public static bool GetReformType(ref int __result, int index, ref PlatformSystem __instance)
        {
            var len = __instance.reformData.Length;
            if (index > len - 1)
            {
                __instance.ComputeMaxReformCount((int)(__instance.planet.radius / 4f + 0.01f) * 4);
                var tempArray = new byte[__instance.maxReformCount];
                Array.Copy(__instance.reformData, tempArray, len);
                __instance.reformData = tempArray;
            }
            else
            {
                __result = __instance.reformData[index] >> 5;
            }

            return false;
        }
    }
}