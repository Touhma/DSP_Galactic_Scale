using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnPlayerMove_Fly
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMove_Fly), "GameTick")]
        public static bool GameTick(ref PlayerMove_Fly __instance)
        {
            var num = 0.0166666675f;
            if (__instance.player == null) //
                __instance.player = GameMain.mainPlayer;

            if (__instance.controller == null) __instance.controller = GameMain.mainPlayer.controller;

            if (__instance.player.movementState != EMovementState.Fly) return false;
            if (__instance.navigation == null) __instance.navigation = GameMain.mainPlayer.navigation;

            if (__instance.mecha == null) __instance.mecha = GameMain.mainPlayer.mecha;

            var forward = __instance.controller.mainCamera.transform.forward;
            var normalized = __instance.player.position.normalized;
            var normalized2 = Vector3.Cross(normalized, forward).normalized;
            forward = Vector3.Cross(normalized2, normalized);
            var vector = forward * __instance.controller.input0.y + normalized2 * __instance.controller.input0.x;
            if (__instance.controller.cmd.type == ECommand.Build && !VFInput._godModeMechaMove && !PlayerController.operationWhenBuild || __instance.navigation.navigating) vector = Vector3.zero;
            if (GameMain.localPlanet == null) //
            {
                GS2.Error("Error: Local planet does not exist.");
                return false;
            }

            var localPlanet = GameMain.localPlanet;
            var num2 = localPlanet != null && localPlanet.type != EPlanetType.Gas;
            var softLandingRecover = __instance.controller.softLandingRecover;
            softLandingRecover *= softLandingRecover;
            var num3 = 0.022f * softLandingRecover;
            var num4 = __instance.player.mecha.walkSpeed * 2.5f;
            var num5 = __instance.controller.input1.y;
            if (__instance.controller.cmd.type == ECommand.Build && !VFInput._godModeMechaMove && !PlayerController.operationWhenBuild || __instance.navigation.navigating) num5 = 0f;
            if (__instance.navigation.navigating)
            {
                var lift = false;
                var drop = false;
                __instance.navigation.DetermineHighOperation(num4, ref lift, ref drop);
                num5 = 0f;
                if (lift) num5 += 1f;
                if (drop && __instance.targetAltitude > 15.01f + num * 20f) num5 += -1f;
            }

            __instance.targetAltitude += num5 * num * 20f;
            if (__instance.controller.cmd.type == ECommand.Build && !PlayerController.operationWhenBuild && __instance.targetAltitude > 40f) __instance.targetAltitude = 40f;
            if (num5 == 0f && __instance.targetAltitude > 15f)
            {
                __instance.targetAltitude -= num * 20f * 0.3f;
                if (__instance.targetAltitude < 15f) __instance.targetAltitude = 15f;
            }
            else if (__instance.targetAltitude >= 50f)
            {
                if (__instance.currentAltitude > 49f && __instance.controller.horzSpeed > 12.5f && __instance.mecha.thrusterLevel >= 2)
                {
                    if (__instance.controller.cmd.type == ECommand.Build) __instance.controller.cmd.SetNoneCommand();
                    __instance.controller.movementStateInFrame = EMovementState.Sail;
                    __instance.controller.actionSail.ResetSailState();
                    GameCamera.instance.SyncForSailMode();
                    GameMain.gameScenario.NotifyOnSailModeEnter();
                }

                __instance.targetAltitude = 50f;
            }

            if (num2)
            {
                if (__instance.targetAltitude < 14.5f)
                {
                    if (num5 > 0f)
                        __instance.targetAltitude = 15f;
                    else
                        __instance.targetAltitude = 1f;
                    if (__instance.currentAltitude < 3f)
                    {
                        __instance.controller.movementStateInFrame = EMovementState.Walk;
                        __instance.controller.softLandingTime = 1.2f;
                    }
                }
            }
            else if (__instance.targetAltitude < 20f)
            {
                __instance.targetAltitude = 20f;
            }

            var realRadius = __instance.player.planetData.realRadius;
            var num6 = Mathf.Max(__instance.player.position.magnitude, realRadius * 0.9f);
            __instance.currentAltitude = num6 - realRadius;
            var num7 = __instance.targetAltitude - __instance.currentAltitude;
            __instance.verticalThrusterForce = 0f;
            var b = Mathf.Clamp(num7 * 0.5f, -10f, 10f) * 100f + (float)__instance.controller.universalGravity.magnitude;
            b = Mathf.Max(0f, b);
            __instance.verticalThrusterForce += b;
            __instance.UseThrustEnergy(ref __instance.verticalThrusterForce, __instance.controller.vertSpeed, 0.016666666666666666);
            var num8 = Mathf.Sin(Time.time * 2f) * 0.1f + 1f;
            if (Mathf.Abs(__instance.verticalThrusterForce) > 0.001f)
                __instance.controller.AddLocalForce(normalized * (__instance.verticalThrusterForce * num8));
            var currentOrder = __instance.player.currentOrder;
            if (currentOrder != null && !currentOrder.targetReached)
            {
                var rhs = currentOrder.target.normalized * localPlanet.realRadius - __instance.player.position.normalized * localPlanet.realRadius;
                _ = rhs.magnitude;
                __instance.rtsVelocity = Vector3.Slerp(b: Vector3.Cross(Vector3.Cross(normalized, rhs).normalized, normalized).normalized * num4, a: __instance.rtsVelocity, t: num3);
            }
            else
            {
                __instance.rtsVelocity = Vector3.MoveTowards(__instance.rtsVelocity, Vector3.zero, num * 6f * num4);
            }

            if (__instance.navigation.navigating)
                __instance.navigation.DetermineHighVelocity(num4, num3, ref __instance.moveVelocity, num);
            else
                __instance.moveVelocity = Vector3.Slerp(__instance.moveVelocity, vector * num4, num3);
            var vel = __instance.moveVelocity + __instance.rtsVelocity;
            if (softLandingRecover > 0.9) vel = Vector3.ClampMagnitude(vel, num4);
            __instance.UseFlyEnergy(ref vel, __instance.mecha.walkPower * num * __instance.controller.softLandingRecover);
            var vector2 = Vector3.Dot(vel, normalized) * normalized;
            vel -= vector2;
            var num9 = __instance.controller.vertSpeed;
            var num10 = 0.6f;
            var num11 = 1f;
            num10 = Mathf.Lerp(0.95f, 0.8f, Mathf.Abs(num7) * 0.3f);
            num11 = num10;
            num10 = Mathf.Lerp(1f, num10, Mathf.Clamp01(__instance.verticalThrusterForce));
            num11 = Mathf.Lerp(1f, num11, Mathf.Clamp01(__instance.verticalThrusterForce) * Mathf.Clamp01((float)(__instance.mecha.coreEnergy - 5000.0) * 0.0001f));
            if (num9 > 0f)
                num9 *= num10;
            else if (num9 < 0f) num9 *= num11;
            __instance.controller.velocity = num9 * normalized + vel;
            if (vector.sqrMagnitude > 0.25f)
                __instance.controller.turning = Vector3.SignedAngle(vel, vector, normalized);
            else
                __instance.controller.turning = 0f;
            if (num2 && __instance.mecha.coreEnergy < 10000.0)
            {
                __instance.controller.movementStateInFrame = EMovementState.Walk;
                __instance.controller.softLandingTime = 1.2f;
            }

            __instance.controller.actionWalk.rtsVelocity = __instance.rtsVelocity;
            __instance.controller.actionWalk.moveVelocity = __instance.moveVelocity * 0.5f;
            __instance.controller.actionDrift.rtsVelocity = __instance.rtsVelocity;
            __instance.controller.actionDrift.moveVelocity = __instance.moveVelocity * 0.5f;
            return false;
        }
    }
}