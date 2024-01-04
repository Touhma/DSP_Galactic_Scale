using System;
using HarmonyLib;

namespace GalacticScale.Patches
{
    public static partial class PatchOnPlanetFactory
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.RemoveEmptyVeinGroups))]
        public static bool RemoveEmptyVeinGroups(PlanetFactory __instance)
        {
            if (__instance.veinGroups[0].count <= 0) return true;

			GS3.Warn("Reassign veinGroup at position 0: " + __instance.veinGroups[0].ToString());
			for (int i = 1; i < __instance.veinGroups.Length; i++)
                if (__instance.veinGroups[i].count <= 0)  __instance.veinGroups[i].SetNull();

			// Modify ArrangeVeinGroups which maps veinGroup at index-0 to other positive location
			int oldLength = __instance.veinGroups.Length;
			if (__instance.tmp_vein_group_idx_mapping == null || __instance.tmp_vein_group_idx_mapping.Length < oldLength)
				__instance.tmp_vein_group_idx_mapping = new int[oldLength * 2];
			int[] array = __instance.tmp_vein_group_idx_mapping;
			Array.Clear(array, 0, array.Length);
			int newLength = 1;
			for (int i = 1; i < oldLength; i++)
			{
				if (__instance.veinGroups[i].isNull) 
					array[i] = 0;
				else
				{
					if (i > newLength) __instance.veinGroups[newLength] = __instance.veinGroups[i];
					array[i] = newLength;
					newLength++;
				}
			}

			Array sourceArray = __instance.veinGroups;
			__instance.veinGroups = new VeinGroup[++newLength]; // make extra space for 0
			Array.Copy(sourceArray, __instance.veinGroups, Math.Min(newLength, oldLength));
			array[0] = newLength - 1; // remap 0 to the last element
			__instance.veinGroups[newLength - 1] = __instance.veinGroups[0];
			__instance.veinGroups[0].SetNull();
			for (int j = 1; j < __instance.veinCursor; j++)
			{
				if (__instance.veinPool[j].id == j)
				{
					int groupIndex = array[__instance.veinPool[j].groupIndex];
					__instance.veinPool[j].groupIndex = (short)groupIndex;
					if (__instance.veinGroups[groupIndex].type == EVeinType.None)
					    __instance.veinGroups[groupIndex].type = __instance.veinPool[j].type;
				}
			}
			return false;
        }
    }
}