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
            catch 
            {
                // GS2.DevLog("modData Index " + index + " doesn't exist: " + e);
                __result = 0;
                return false;
            }
        }
    }
}

/*ChatGPT Generated Explanation:

To calculate the index, the index >> 1 operation shifts the bits of the index to the right by one position, effectively dividing the index by 2 and discarding the remainder.
The index & 1 operation performs a bitwise AND operation between index and the binary number 1, which has a value of 00000001 in binary. 
This operation effectively extracts the least significant bit of the index.

The result of the bitwise AND operation is shifted to the left by two positions with the << 2 operation, effectively multiplying it by 4. 
This value is then used as an offset to retrieve the desired value from the modData array.

The value is retrieved by performing a bitwise AND operation between the value in modData at the calculated index and the binary number 00000011. 
This operation effectively extracts the two least significant bits of the value, which represent the mod level. */