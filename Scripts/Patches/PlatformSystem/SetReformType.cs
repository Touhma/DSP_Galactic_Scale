using System;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlatformSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlatformSystem), "SetReformType")] // Increase size of array
        public static bool SetReformType(int index, int type, ref PlatformSystem __instance)
        {
            var len = __instance.reformData.Length;
            if (index > len - 1)
            {
                __instance.ComputeMaxReformCount((int)(__instance.planet.radius / 4f + 0.01f) * 4);
                var tempArray = new byte[__instance.maxReformCount];
                Array.Copy(__instance.reformData, tempArray, len);
                __instance.reformData = tempArray;
                __instance.reformData[index] = (byte)(((type & 7) << 5) | (__instance.reformData[index] & 31));
            }
            else
            {
                __instance.reformData[index] = (byte)(((type & 7) << 5) | (__instance.reformData[index] & 31));
            }

            return false;
        }
    }
}