using HarmonyLib;

namespace GalacticScale.Patches
{
    //Adding Null Checks for System Display in Galaxy Select
    public class PatchOnUniverseSimulator
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UniverseSimulator), nameof(UniverseSimulator.FindStarSimulator))]
        public static bool FindStarSimulator(UniverseSimulator __instance, StarData star, ref StarSimulator __result)
        {
            if (__instance.starSimulators != null && star != null)
            {
                if (star.index > __instance.starSimulators.Length - 1)
                {
                    __result = null;
                    return false;
                }

                __result = __instance.starSimulators[star.index];
                return false;
            }

            __result = null;
            return false;
        }
    }
}