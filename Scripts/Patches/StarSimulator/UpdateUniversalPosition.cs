using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnStarSimulator
    {
        private const double LightYear = 2400000.0;
        private const double MaxPlayerMoveSqr = 5000.0 * 5000.0;
        private static int _frame = -1;
        private static bool _updateAll = true;
        private static bool _enabled = true;
        private static int _interval = 3;
        private static double _nearSqr;
        private static Vector3 _lastCameraPos;
        private static VectorLF3 _lastPlayerUPos;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StarSimulator), nameof(StarSimulator.UpdateUniversalPosition))]
        public static bool UpdateUniversalPosition_Throttle(StarSimulator __instance, VectorLF3 playerUPos, Vector3 cameraPos)
        {
            if (Time.frameCount != _frame)
            {
                _frame = Time.frameCount;
                var config = GS2.Config;
                _enabled = config.ThrottleFarStars;
                _interval = Mathf.Max(1, config.FarStarInterval);
                var near = config.NearStarDistance * LightYear;
                _nearSqr = near * near;
                var cameraMove = cameraPos - _lastCameraPos;
                var playerMove = playerUPos - _lastPlayerUPos;
                _updateAll = cameraMove.sqrMagnitude > config.StarFlightThreshold || playerMove.sqrMagnitude > MaxPlayerMoveSqr;
                _lastCameraPos = cameraPos;
                _lastPlayerUPos = playerUPos;
            }

            if (!_enabled || _interval <= 1 || _updateAll) return true;
            var star = __instance.starData;
            if (star == null || star == GameMain.localStar) return true;
            var toStar = star.uPosition - playerUPos;
            if (toStar.sqrMagnitude < _nearSqr) return true;
            return (star.index + _frame) % _interval == 0;
        }
    }
}
