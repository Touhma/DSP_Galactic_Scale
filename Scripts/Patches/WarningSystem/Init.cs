using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnWarningSystem
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WarningSystem), "Init")]
        public static void Init(ref WarningSystem __instance)
        {
            var planetCount = GSSettings.PlanetCount;
            if (__instance.gameData.factories.Length > planetCount) planetCount = __instance.gameData.factories.Length;
            __instance.tmpEntityPools = new EntityData[planetCount][];
            __instance.tmpPrebuildPools = new PrebuildData[planetCount][];
            __instance.tmpSignPools = new SignData[planetCount][];
            var l = GameMain.galaxy.starCount * 400;
            // __instance.astroArr = new AstroPoseR[l]; // removed in 0.10
            // __instance.astroBuffer = new ComputeBuffer(l, 32, ComputeBufferType.Default); // removed in 0.10
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WarningSystem), "SetWarningCapacity")]
        public static bool SetWarningCapacity(ref WarningSystem __instance, ref int newCapacity)
        {
            
            if (newCapacity == 0) {
            {
                GS2.Warn($"Setting Warning Capacity from 0 to 32");
                newCapacity = 32;
            }}
            return true;
        }

        // WarningData[] array = __instance.warningPool;
        //     __instance.warningPool = new WarningData[newCapacity];
        //     __instance.warningRecycle = new int[newCapacity];
        //     if (array != null)
        //     {
        //         Array.Copy(array, __instance.warningPool, (newCapacity > __instance.warningCapacity) ? __instance.warningCapacity : newCapacity);
        //     }
        //
        //     __instance.warningCapacity = newCapacity;
        //     if (__instance.instBuffer != null)
        //     {
        //         __instance.instBuffer.Release();
        //     }
        //
        //     __instance.instBuffer = new ComputeBuffer(newCapacity, 40, ComputeBufferType.Default);
        //     return false;
        // }
    }
}