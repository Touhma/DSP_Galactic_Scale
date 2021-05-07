using HarmonyLib;
using PowerNetworkStructures;

namespace GalacticScale.Scripts.PatchPlanetSize
{
    [HarmonyPatch(typeof(PowerSystem))]
    static class PatchOnPowerSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch("line_arrangement_for_add_node")]
        public static bool line_arrangement_for_add_node(Node node, ref int[] ___tmp_state)
        {
            if (___tmp_state == null)
            {
                ___tmp_state = new int[2048];
            }
            return true;
        }
    }
}