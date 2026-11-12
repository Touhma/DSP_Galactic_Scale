using HarmonyLib;
using System;

namespace GalacticScale
{
    public class PatchOnGalaxyData
    {
        private static bool[] _active;
        private static GalaxyData _galaxy;
        private static int _call;
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
        public static bool UpdatePoses(double time, GalaxyData __instance)
        {
            var count = __instance.starCount;
            var stars = __instance.stars;
            var data = GameMain.data;
            var config = GS2.Config;
            var interval = Math.Max(1, config.FarPoseInterval);
            if (!config.ThrottleFarPoses || interval <= 1 || data == null)
            {
                for (var i = 0; i < count; i++) UpdateStarPoses(stars[i], time);
                return false;
            }

            _call++;
            var first = !ReferenceEquals(_galaxy, __instance) || _active == null || _active.Length != count;
            if (first || _call % interval == 1)
            {
                if (_active == null || _active.Length != count) _active = new bool[count];
                else Array.Clear(_active, 0, count);
                for (var f = 0; f < data.factoryCount; f++)
                {
                    var star = data.factories[f]?.planet?.star;
                    if (star != null && star.index >= 0 && star.index < count) _active[star.index] = true;
                }
                if (data.dysonSpheres != null)
                    for (var i = 0; i < count && i < data.dysonSpheres.Length; i++)
                        if (data.dysonSpheres[i] != null) _active[i] = true;
                _galaxy = __instance;
            }
            var localIndex = GameMain.localStar != null ? GameMain.localStar.index : -1;
            for (var i = 0; i < count; i++)
                if (first || i == localIndex || _active[i] || (i + _call) % interval == 0)
                    UpdateStarPoses(stars[i], time);
            return false;
        }

        private static void UpdateStarPoses(StarData star, double time)
        {
            for (var j = 0; j < star.planetCount; j++) star.planets[j].UpdateRuntimePose(time);
        }

        // {
        //     var threads = MultithreadSystem.usedThreadCntSetting;
        //     // 0 = "Default" in Gameplay settings. Use all available cores.
        //     if (threads == 0) threads = SystemInfo.processorCount;
        //
        //     if (threads < 2 || __instance.starCount <= 64) return true;
        //     Parallel.For(0, __instance.starCount, new ParallelOptions {MaxDegreeOfParallelism = threads},
        //         (i) =>
        //         {
        //             for (var j = 0; j < __instance.stars[i].planetCount; j++)
        //             {
        //                 __instance.stars[i].planets[j].UpdateRuntimePose(time);
        //             }
        //         }
        //
        //     );
        //     return false;
        // }
    }
}
