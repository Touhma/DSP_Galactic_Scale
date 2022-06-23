using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnPlanetData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetData), "AddHeightMapModLevel")]
        public static bool AddHeightMapModLevel(int index, int level, PlanetData __instance)
        {
            if (__instance.data.AddModLevel(index, level))
            {
                var num = __instance.precision / __instance.segment;
                var num2 = index % __instance.data.stride;
                var num3 = index / __instance.data.stride;
                var num4 = (num2 < __instance.data.substride ? 0 : 1) + (num3 < __instance.data.substride ? 0 : 2);
                var num5 = num2 % __instance.data.substride;
                var num6 = num3 % __instance.data.substride;
                var num7 = (num5 - 1) / num;
                var num8 = (num6 - 1) / num;
                var num9 = num5 / num;
                var num10 = num6 / num;
                if (num9 >= __instance.segment) num9 = __instance.segment - 1;
                if (num10 >= __instance.segment) num10 = __instance.segment - 1;
                var num11 = num4 * __instance.segment * __instance.segment;
                var num12 = num7 + num8 * __instance.segment + num11;
                var num13 = num9 + num8 * __instance.segment + num11;
                var num14 = num7 + num10 * __instance.segment + num11;
                var num15 = num9 + num10 * __instance.segment + num11;
                num12 = Mathf.Clamp(num12, 0, 99);
                num13 = Mathf.Clamp(num13, 0, 99);
                num14 = Mathf.Clamp(num14, 0, 99);
                num15 = Mathf.Clamp(num15, 0, 99);
                __instance.dirtyFlags[num12] = true;
                __instance.dirtyFlags[num13] = true;
                __instance.dirtyFlags[num14] = true;
                __instance.dirtyFlags[num15] = true;
            }

            return false;
        }
    }
}