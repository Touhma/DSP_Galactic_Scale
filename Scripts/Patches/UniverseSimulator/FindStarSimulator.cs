using HarmonyLib;

namespace GalacticScale
{
    //Adding Null Checks for System Display in Galaxy Select
    public partial class PatchOnUniverseSimulator
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UniverseSimulator), "FindStarSimulator")]
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