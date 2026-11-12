using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlatformSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlatformSystem), "DetermineLongitudeSegmentCount")]
        public static bool DetermineLongitudeSegmentCount(PlatformSystem __instance, int _latitudeIndex, int _segment, ref int __result)
        {
            return PatchOnPlanetGrid.DetermineLongitudeSegmentCount(null, _latitudeIndex, _segment, ref __result);
        }
    }
}
