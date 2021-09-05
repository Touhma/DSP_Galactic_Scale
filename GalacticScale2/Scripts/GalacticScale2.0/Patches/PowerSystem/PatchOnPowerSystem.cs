using HarmonyLib;
using PowerNetworkStructures;

namespace GalacticScale
{
    public class PatchOnPowerSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PowerSystem), "line_arragement_for_add_node")]
        public static bool line_arrangement_for_add_node(Node node, ref int[] ___tmp_state)
        {
            if (___tmp_state == null) ___tmp_state = new int[2048];
            return true;
        }
    }
}