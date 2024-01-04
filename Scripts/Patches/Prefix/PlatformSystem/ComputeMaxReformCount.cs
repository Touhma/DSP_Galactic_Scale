﻿using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlatformSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlatformSystem), nameof(PlatformSystem.ComputeMaxReformCount))]
        public static bool ComputeMaxReformCount(ref PlatformSystem __instance, int _segment)
        {
            var num1 = __instance.latitudeCount / 2;
            var num2 = 0;
            for (var index = 0; index < __instance.latitudeCount + 1; ++index)
            {
                __instance.reformOffsets[index] = num2;
                if (index != __instance.latitudeCount)
                {
                    var _latitudeIndex = (index >= num1 ? index - num1 : index) / 5;
                    num2 += PlatformSystem.DetermineLongitudeSegmentCount(_latitudeIndex, _segment) * 5;
                }
                else
                {
                    break;
                }
            }

            var s = "";
            var kc = GS3.keyedLUTs.Keys;
            foreach (var k in kc) s += k + " ";

            if (!GS3.keyedLUTs.ContainsKey(_segment)) GS3.SetLuts(_segment, __instance.planet.radius);
            if (GS3.keyedLUTs.ContainsKey(_segment))
            {
                var total = 0;
                foreach (var segCount in GS3.keyedLUTs[_segment]) total += segCount * 25 * 2;
                __instance.maxReformCount = total;
            }
            else
            {
                __instance.maxReformCount = num2;
            }

            return false;
        }
    }
}