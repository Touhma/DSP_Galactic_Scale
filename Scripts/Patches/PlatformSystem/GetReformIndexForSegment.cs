using UnityEngine;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlatformSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlatformSystem), "GetReformIndexForSegment")]
        public static bool GetReformIndexForSegment(
            ref int __result, float _latitudeSeg, float _longitudeSeg, ref PlatformSystem __instance)
        {
            int num = (_latitudeSeg > 0f) ? Mathf.CeilToInt(_latitudeSeg * 5f) : Mathf.FloorToInt(_latitudeSeg * 5f);
            int num2 = (_longitudeSeg > 0f) ? Mathf.CeilToInt(_longitudeSeg * 5f) : Mathf.FloorToInt(_longitudeSeg * 5f);
            int num3 = __instance.latitudeCount / 2;
            int y = (num > 0) ? (num - 1) : (num3 - num - 1);
            int num4 = PlatformSystem.DetermineLongitudeSegmentCount(
                Mathf.FloorToInt(Mathf.Abs(_latitudeSeg)), __instance.segment);

            int period = num4 * 5;
            while (num2 > period / 2) num2 = num2 - period - 1;
            while (num2 < -period / 2) num2 = period + num2 + 1;

            int x = (num2 > 0) ? (num2 - 1) : (period / 2 - num2 - 1);
            __result = __instance.GetReformIndex(x, y);
            return false;
        }
    }
}