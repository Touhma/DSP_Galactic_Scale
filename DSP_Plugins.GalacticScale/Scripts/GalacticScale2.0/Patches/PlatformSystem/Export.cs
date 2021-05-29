using HarmonyLib;
using System;
using System.IO;

namespace GalacticScale
{
    public partial class PatchOnPlatformSystem
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "Export")]
        public static bool Export(BinaryWriter w, ref PlatformSystem __instance)
        {
            if (__instance.reformData != null)
            {
                int len = __instance.reformData.Length;
                __instance.ComputeMaxReformCount((int)(__instance.planet.radius / 4f + 0.01f) * 4);
                if (__instance.maxReformCount > len - 1)
                {
                    byte[] tempArray = new byte[__instance.maxReformCount];
                    Array.Copy(__instance.reformData, tempArray, len);
                    __instance.reformData = tempArray;
                }
            }
            return true;
        }
    }
}