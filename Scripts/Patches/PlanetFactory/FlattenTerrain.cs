using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnPlanetFactory
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), "InitVeinGroups", typeof(PlanetData))]
        public static bool InitVeinGroups(PlanetFactory __instance, PlanetData planet)
        {
            var veinGroupsLock = planet.veinGroupsLock;
            lock (veinGroupsLock)
            {
                if (planet.veinGroups == null) planet.veinGroups = new VeinGroup[1];
                var num = planet.veinGroups.Length;
                var num2 = num >= 1 ? num : 1;
                __instance.veinGroups = new VeinGroup[num2];
                Array.Copy(planet.veinGroups, __instance.veinGroups, num);
                __instance.veinGroups[0].SetNull();
            }

            return false;
        }

    }
}