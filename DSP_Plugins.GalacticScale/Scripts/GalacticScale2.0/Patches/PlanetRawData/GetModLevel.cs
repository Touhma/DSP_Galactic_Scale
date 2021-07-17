using System;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetRawData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetRawData), "GetModLevel")]
        public static bool GetModLevel(int index, ref PlanetRawData __instance, ref int __result)
        {
            try // try-catch block probably unnecessary, left in for debugging use in future
            {
                __result = (__instance.modData[index >> 1] >> ((index & 1) << 2)) & 3;
                return false;
            }
            catch (Exception e)
            {
                GS2.Error("modData Index " + index + " doesn't exist: " + e);
                return false;
            }
        }
    }
}