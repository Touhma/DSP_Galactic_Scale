using HarmonyLib;
using System;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        private const int PostFastTravelRecoveryTicks = 180;
        private static int postFastTravelRecoveryTicks;

        public static bool IsNaNRecoveryActive { get; private set; }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "OnActivePlanetLoaded")]
        public static void OnActivePlanetLoaded(PlanetData planet)
        {
            //GS2.Warn($"{planet.name}");
            if (!GS2.Vanilla)
            {
                TryRecoverInvalidPlayerPosition(planet);

                var segments = (int)(planet.radius / 4f + 0.1f) * 4;
                if (!PatchOnUIBuildingGrid.LUT512.ContainsKey(segments)) GS2.SetLuts(segments, planet.radius);
                PatchOnUIBuildingGrid.refreshGridRadius = Mathf.RoundToInt(planet.radius);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerController), "GameTick")]
        public static void PlayerControllerGameTick()
        {
            var player = GameMain.mainPlayer;
            if (player == null || player.controller?.actionSail == null)
            {
                postFastTravelRecoveryTicks = 0;
                IsNaNRecoveryActive = false;
                return;
            }

            var fastTravelling = player.controller.actionSail.fastTravelling;
            if (fastTravelling)
                postFastTravelRecoveryTicks = PostFastTravelRecoveryTicks;
            else if (postFastTravelRecoveryTicks > 0)
                postFastTravelRecoveryTicks--;

            IsNaNRecoveryActive = fastTravelling || postFastTravelRecoveryTicks > 0;
            if (!IsNaNRecoveryActive || GameMain.localPlanet == null) return;

            if (player.movementState != EMovementState.Fly)
                TryRecoverInvalidPlayerPosition(GameMain.localPlanet);
        }

        public static bool TryRecoverInvalidPlayerPosition(PlanetData planet)
        {
            var player = GameMain.mainPlayer;
            if (planet == null || player == null || !IsFinite(planet.uPosition)) return false;

            var planetRadius = planet.realRadius;
            if (float.IsNaN(planetRadius) || float.IsInfinity(planetRadius) || planetRadius <= 0f) return false;

            var localPosition = player.position;
            var localDistanceSquared = localPosition.sqrMagnitude;
            var invalidLocalPosition = !IsFinite(localPosition) || float.IsNaN(localDistanceSquared) || float.IsInfinity(localDistanceSquared);
            var transform = player.transform;
            var invalidTransformPosition = transform == null || !IsFinite(transform.localPosition);
            var invalidWorldTransformPosition = transform == null || !IsFinite(transform.position);
            var invalidTransformRotation = transform == null || !IsUsableRotation(transform.localRotation);
            var invalidControllerVelocity = player.controller != null && !IsFinite(player.controller.velocity);
            var offset = player.uPosition - planet.uPosition;
            var distanceSquared = SqrMagnitude(offset);
            var invalidPosition = !IsFinite(player.uPosition) || double.IsNaN(distanceSquared) || double.IsInfinity(distanceSquared) ||
                                  invalidLocalPosition || invalidTransformPosition || invalidWorldTransformPosition ||
                                  invalidTransformRotation || !IsUsableRotation(player.uRotation) || invalidControllerVelocity;
            var recoveryThreshold = Mathf.Max(5f, planetRadius * 0.001f);
            var safeRadius = Mathf.Max(0f, planetRadius - recoveryThreshold);
            var safeRadiusSquared = (double)safeRadius * safeRadius;
            var insidePlanet = !invalidPosition &&
                               (distanceSquared < safeRadiusSquared || localDistanceSquared < safeRadiusSquared);

            if (invalidPosition || insidePlanet)
            {
                var localDistance = Mathf.Sqrt(Mathf.Max(0f, localDistanceSquared));
                var distance = Math.Sqrt(Math.Max(0.0, distanceSquared));
                Vector3 localOutward = Vector3.zero;
                if (!invalidLocalPosition && localDistanceSquared > 0.000001f)
                {
                    localOutward = localPosition.normalized;
                }
                else if (!double.IsNaN(distanceSquared) && !double.IsInfinity(distanceSquared) && distanceSquared > 0.000001)
                {
                    localOutward = (Vector3)Maths.QInvRotateLF(planet.runtimeRotation, offset.normalized);
                }
                else if (planet.star != null && IsFinite(planet.star.uPosition))
                {
                    var starRelativeWorldDirection = (planet.uPosition - planet.star.uPosition).normalized;
                    localOutward = (Vector3)Maths.QInvRotateLF(planet.runtimeRotation, starRelativeWorldDirection);
                }

                if (!IsFinite(localOutward) || localOutward.sqrMagnitude < 0.5f) localOutward = Vector3.forward;
                localOutward.Normalize();
                var worldOutward = (VectorLF3)(planet.runtimeRotation * localOutward).normalized;

                var surfaceDistance = planetRadius + 10.0;
                var surfaceOffset = worldOutward * surfaceDistance;
                player.position = localOutward * (float)surfaceDistance;
                player.uPosition = planet.uPosition + surfaceOffset;
                player.uVelocity = VectorLF3.zero;
                player.uRotation = Quaternion.identity;
                player.planetId = planet.id;
                if (player.controller != null)
                {
                    player.controller.velocity = Vector3.zero;
                    if (player.controller.actionFly != null)
                    {
                        player.controller.actionFly.moveVelocity = Vector3.zero;
                        player.controller.actionFly.rtsVelocity = Vector3.zero;
                    }
                    player.controller.ResetTransRotation();
                }
                var recoveryReason = invalidPosition ? "position or transform was invalid" : "inside the planet";
                GS2.Warn($"Recovered invalid player arrival position at {planet.name}; player was {recoveryReason}. " +
                    $"Local position before recovery: {localPosition} (magnitude {localDistance:0.##}); world distance from planet center: {distance:0.##}. " +
                    $"Placed at radius {surfaceDistance:0.##}; transform local position is {player.transform.localPosition}, rotation is {player.transform.localRotation}.");
                return true;
            }
            else if (!IsFinite(player.uVelocity))
            {
                player.uVelocity = VectorLF3.zero;
                GS2.Warn($"Reset non-finite player velocity after arriving at {planet.name}.");
                return true;
            }

            return false;
        }

        private static bool IsFinite(VectorLF3 vector)
        {
            return IsFinite(vector.x) && IsFinite(vector.y) && IsFinite(vector.z);
        }

        private static double SqrMagnitude(VectorLF3 vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }

        private static bool IsFinite(Vector3 vector)
        {
            return IsFinite(vector.x) && IsFinite(vector.y) && IsFinite(vector.z);
        }

        private static bool IsFinite(Quaternion rotation)
        {
            return IsFinite(rotation.x) && IsFinite(rotation.y) && IsFinite(rotation.z) && IsFinite(rotation.w);
        }

        private static bool IsUsableRotation(Quaternion rotation)
        {
            return IsFinite(rotation) && rotation.x * rotation.x + rotation.y * rotation.y +
                   rotation.z * rotation.z + rotation.w * rotation.w > 0.25f;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
