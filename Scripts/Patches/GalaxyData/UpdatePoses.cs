using HarmonyLib;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnGalaxyData
    {
        /// <summary>
        /// Only beneficial for >64 stars because of the overhead involved in creating threads.
        /// 
        /// Reads/Writes in UpdateRuntimePose to values outside of the PlanetData instance:
        /// orbitAroundPlanet.runtimePosition (read) - Parallel per star, so if this worked before, it should work now.
        /// orbitAroundPlanet.runtimePositionNext (read) - Parallel per star, so if this worked before, it should work now.
        /// star.uPosition (read) - Parallel per star, so if this worked before, it should work now.
        /// galaxy.astrosData[id].uPos (write) - Multiple threads writing to astrosData simultaneously, but each planet is only writing to its own portion of the array.
        /// galaxy.astrosData[id].uRot (write) - Multiple threads writing to astrosData simultaneously, but each planet is only writing to its own portion of the array.
        /// galaxy.astrosData[id].uPosNext (write) - Multiple threads writing to astrosData simultaneously, but each planet is only writing to its own portion of the array.
        /// galaxy.astrosData[id].uRotNext (write) - Multiple threads writing to astrosData simultaneously, but each planet is only writing to its own portion of the array.
        /// No locking needed in UpdateRuntimePose.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GalaxyData), "UpdatePoses")]
        public static bool UpdatePoses(Double time, GalaxyData __instance)
        {
            var threads = MultithreadSystem.usedThreadCntSetting;
            // 0 = "Default" in Gameplay settings. Use all available cores.
            if (threads == 0) threads = SystemInfo.processorCount;

            if (threads < 2 || __instance.starCount <= 64) return true;
            Parallel.For(0, __instance.starCount, new ParallelOptions {MaxDegreeOfParallelism = threads},
                (i) =>
                {
                    for (var j = 0; j < __instance.stars[i].planetCount; j++)
                    {
                        __instance.stars[i].planets[j].UpdateRuntimePose(time);
                    }
                }

            );
            return false;
        }
    }
}