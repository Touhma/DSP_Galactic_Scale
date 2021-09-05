using System;
using System.IO;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlatformSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlatformSystem), "Export")] //Increase the size of the array to prevent the game only saving part of it
        public static bool Export(BinaryWriter w, ref PlatformSystem __instance)
        {
            if (__instance.reformData != null)
            {
                var len = __instance.reformData.Length;
                __instance.ComputeMaxReformCount((int)(__instance.planet.radius / 4f + 0.01f) * 4);
                if (__instance.maxReformCount > len - 1)
                {
                    var tempArray = new byte[__instance.maxReformCount];
                    Array.Copy(__instance.reformData, tempArray, len);
                    __instance.reformData = tempArray;
                }
            }

            return true;
        }
    }
}