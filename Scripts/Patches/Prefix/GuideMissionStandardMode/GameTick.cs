using System;
using HarmonyLib;
using NGPT;
using UnityEngine;

namespace GalacticScale.Patches
{
    public partial class PatchOnGuideMissionStandardMode
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GuideMissionStandardMode), nameof(GuideMissionStandardMode.GameTick))]
        public static bool GameTick(ref GuideMissionStandardMode __instance)
        {
            if (GS3.IsMenuDemo) return true;
            __instance.spaceCapsule.landanim.enabled = true;
            __instance.spaceCapsule.landanim.weight = 1f;
            __instance.spaceCapsule.landanim.speed = 0f;
            __instance.gameData.hidePlayerModel = true;
            __instance.gameData.disableController = __instance.stage != GuideMissionStandardMode.Stage.Drive;
            var num = (float)(__instance.player.uPosition - __instance.localPlanet.uPosition).magnitude - __instance.localPlanet.realRadius;
            if (__instance.stage == GuideMissionStandardMode.Stage.Curve)
            {
                __instance.sailPoser.disableFov = true;
                var pRadius = Mathf.Clamp(GameMain.localStar.physicsRadius, 1f, 1200f);
                var num2 = Mathf.Clamp(((float)(__instance.player.uPosition - GameMain.localStar.uPosition).magnitude / pRadius - 2f) * 0.3f, 0f, 49.9f) + 0.1f;

                __instance.curveSpeed = __instance.spaceCapsule.speedOverCurveTime.Evaluate(__instance.curveDistance / __instance.curveLength) * num2;
                __instance.curveDistance += (float)(__instance.curveSpeed * 0.016666666666666666);
                var locationByLength = __instance.bezierPath.GetLocationByLength(__instance.curveDistance * 1f);
                var vec = __instance.bezierPath.GetPoint(locationByLength) / 1f;
                var normalized = __instance.bezierPath.GetDerivative(locationByLength).normalized;
                var quaternion = Quaternion.LookRotation(normalized, Vector3.up);
                var lhs = quaternion * Quaternion.Inverse(__instance.lastURot);
                __instance.sailPoser.targetURot = lhs * __instance.sailPoser.targetURot;
                __instance.sailPoser.targetURotWanted = lhs * __instance.sailPoser.targetURotWanted;
                __instance.player.uPosition = vec;
                __instance.player.uRotation = quaternion;
                // GS3.Warn($"Num2:{num2} CurveSpeed:{__instance.curveSpeed}");
                __instance.player.uVelocity = normalized * __instance.curveSpeed;
                __instance.lastURot = __instance.player.uRotation;
                var flag = __instance.curveTime > 15f && __instance.curveTime < 18f || __instance.curveSpeed > 3000f || __instance.curveLength - __instance.curveDistance < 1000f;
                if (__instance.curveLength - __instance.curveDistance < 1000f)
                    __instance.sailPoser.disableRot = true;
                else
                    __instance.sailPoser.disableRot = false;

                __instance.sailPoser.fov = 55f + Mathf.Log(__instance.curveSpeed / 2000f + 1f) * 15f;
                if (flag)
                {
                    var t = Mathf.Clamp01(1f - VFInput.mouseMoveAxis.magnitude / 10f) * 0.015f;
                    __instance.sailPoser.targetURot = Quaternion.Slerp(__instance.sailPoser.targetURot, quaternion, t);
                    __instance.sailPoser.targetURotWanted = Quaternion.Slerp(__instance.sailPoser.targetURotWanted, quaternion, t);
                }

                __instance.curveTime += 0.016666668f;
                if (__instance.curveDistance + __instance.curveSpeed * 0.016666666666666666 > __instance.curveLength)
                {
                    __instance.sailPoser.disableRot = false;
                    __instance.curveTime = 0f;
                    __instance.stage = GuideMissionStandardMode.Stage.Drive;
                }

                __instance.spaceCapsule.animTime = 0f;
                if (__instance.curveTime > 7.5f && !__instance.tip1Played)
                {
                    UIRoot.instance.uiGame.RequestAdvisorTip(1);
                    __instance.tip1Played = true;
                }

                if (__instance.curveTime > 36f && !__instance.tip2Played)
                {
                    UIRoot.instance.uiGame.RequestAdvisorTip(2);
                    __instance.tip2Played = true;
                }

                __instance.forceLocalPlanet = num < 9000f;
            }
            else if (__instance.stage == GuideMissionStandardMode.Stage.Drive)
            {
                __instance.sailPoser.disableFov = false;
                __instance.gameData.disableController = false;
                if (__instance.player.uVelocity.magnitude > num) __instance.player.uVelocity = __instance.player.uVelocity.normalized * num;

                if (num > 10000) __instance.player.uVelocity = __instance.player.uVelocity.normalized * num;
                if (num < 400f)
                {
                    __instance.stage = GuideMissionStandardMode.Stage.ArriveA;
                    __instance.arriveHorzSpeedWanted = 70.0;
                    __instance.arriveUVel = __instance.player.uVelocity;
                    __instance.arriveUPos = __instance.player.uPosition;
                    __instance.arriveLandTime = 0f;
                }

                if (num < 900f)
                {
                    var num3 = __instance.player.uVelocity.magnitude;
                    num3 = num3 * 0.98 + 2.0;
                    __instance.player.uVelocity = __instance.player.uVelocity.normalized * num3;
                }

                if (num < 600f && !__instance.tip3Played)
                {
                    UIRoot.instance.uiGame.RequestAdvisorTip(3);
                    __instance.tip3Played = true;
                }

                __instance.forceLocalPlanet = num < 5000f;
                __instance.lastURot = __instance.player.uRotation;
                __instance.spaceCapsule.animTime = 0f;
            }
            else if (__instance.stage == GuideMissionStandardMode.Stage.ArriveA)
            {
                __instance.gameData.disableController = true;
                var vectorLF = __instance.arriveUPos - __instance.localPlanet.uPosition;
                VectorLF3 vectorLF2;
                VectorLF3 vectorLF3;
                Maths.HorzVertVector(__instance.arriveUVel, vectorLF.normalized, out vectorLF2, out vectorLF3);
                var vec2 = vectorLF2.normalized * __instance.arriveHorzSpeedWanted - vectorLF.normalized * 20.0;
                var b = Vector3.Angle(vec2, __instance.arriveUVel);
                var t2 = __instance.arriveControl * 0.7f / Mathf.Max(5f, b);
                __instance.arriveUVel = Vector3.Slerp(__instance.arriveUVel, vec2, t2);
                __instance.arriveUPos += __instance.arriveUVel * 0.016666666666666666;
                __instance.player.uPosition = __instance.arriveUPos;
                __instance.player.uVelocity = __instance.arriveUVel;
                __instance.player.uRotation = Quaternion.Slerp(__instance.player.uRotation, Quaternion.LookRotation(__instance.arriveUVel, vectorLF.normalized), 0.005f);
                __instance.lastURot = __instance.player.uRotation;
                __instance.arriveHorzSpeedWanted -= 0.05;
                __instance.arriveControl += 0.008333334f;
                if (__instance.arriveControl > 1f) __instance.arriveControl = 1f;

                GameCamera.instance.sailOverride = true;
                __instance.sailPoser.targetUPos = __instance.player.uPosition + Maths.QRotateLF(__instance.player.uRotation, new VectorLF3(0f, 0f, -__instance.arriveLandTime * 10f));
                __instance.arriveLandTime += 0.016666668f;
                __instance.forceLocalPlanet = true;
                if (__instance.arriveHorzSpeedWanted <= 40.0)
                {
                    __instance.arriveHorzSpeedWanted = 40.0;
                    __instance.arriveLandTime = 0f;
                    __instance.stage = GuideMissionStandardMode.Stage.ArriveB;
                }

                __instance.spaceCapsule.animTime = 0f;
            }
            else if (__instance.stage == GuideMissionStandardMode.Stage.ArriveB)
            {
                GameCamera.instance.sailOverride = true;
                __instance.sailPoser.disableRot = true;
                __instance.sailPoser.disableDist = true;
                __instance.gameData.disableController = true;
                var vectorLF4 = __instance.localPlanet.star.uPosition - __instance.localPlanet.uPosition;
                var normalized2 = Maths.HorzVector(__instance.localPlanet.uPosition + vectorLF4.normalized * __instance.localPlanet.realRadius - __instance.targetUPos, (__instance.targetUPos - __instance.localPlanet.uPosition).normalized).normalized;
                Vector3 vector = (__instance.targetUPos + normalized2 * 300.0 - __instance.localPlanet.uPosition).normalized * __instance.localPlanet.realRadius;
                Vector3 vector2 = (__instance.targetUPos - __instance.localPlanet.uPosition).normalized * __instance.localPlanet.realRadius;
                __instance.arriveLandTime += 0.0011666667f;
                if (__instance.arriveLandTime > 1f) __instance.arriveLandTime = 1f;

                var num4 = Mathf.Clamp01(__instance.spaceCapsule.lerpOverArriveTime.Evaluate(__instance.arriveLandTime));
                var num5 = __instance.spaceCapsule.heightOverArriveLerp.Evaluate(num4) + 1.58f;
                var vec3 = Vector3.Slerp(vector, vector2, num4).normalized * (__instance.localPlanet.realRadius + 0.2f + num5);
                var normalized3 = vec3.normalized;
                __instance.arriveUPos = (VectorLF3)vec3 + __instance.localPlanet.uPosition;
                var quaternion2 = Quaternion.LookRotation(Maths.HorzVector(vector2 - vector, normalized3).normalized, normalized3);
                __instance.arriveUVel = quaternion2 * (Vector3.forward * 30f);
                quaternion2 = Quaternion.Slerp(__instance.targetURot, quaternion2, Mathf.Clamp01((num5 - 5f) / 15f));
                __instance.player.uPosition = __instance.arriveUPos;
                __instance.player.uRotation = quaternion2;
                __instance.player.uVelocity = __instance.arriveUVel;
                __instance.lastURot = __instance.player.uRotation;
                if (__instance.arriveLandTime < 0.5)
                {
                    __instance.arriveUPosMark = __instance.arriveUPos;
                    __instance.arriveURotMark = quaternion2;
                    __instance.sailPoser.targetUPos = __instance.player.uPosition - Maths.QRotateLF(__instance.player.uRotation, new VectorLF3(0f, 0f, 70f - __instance.arriveLandTime * 140f)) + (__instance.localPlanet.uPosition - __instance.player.uPosition).normalized * 5.0;
                    __instance.sailPoser.targetURot = __instance.sailPoser.targetURotWanted = __instance.player.uRotation * Quaternion.Euler(20f - __instance.arriveLandTime * 40f, __instance.arriveLandTime * 120f, 25f - __instance.arriveLandTime * 50f);
                    __instance.sailPoser.distCoef = __instance.sailPoser.distCoefWanted = 0.3f;
                    GameCamera.instance.rtsPoser.distCoefWanted = 1f;
                    GameCamera.instance.rtsPoser.distCoef = 1f;
                    GameCamera.instance.rtsPoser.pitchCoefWanted = 0.4f;
                    GameCamera.instance.rtsPoser.pitchCoef = 0.4f;
                    GameCamera.instance.rtsPoser.disableDist = true;
                }
                else
                {
                    __instance.player.controller.memCameraTargetRot = __instance.targetRot;
                    __instance.player.cameraTarget.position = __instance.targetPos;
                    __instance.player.cameraTarget.rotation = __instance.targetRot;
                    GameCamera.instance.rtsPoser.distCoefWanted = 0.9f - (__instance.arriveLandTime - 0.5f) * 0.5f;
                    GameCamera.instance.rtsPoser.distCoef = 0.9f - (__instance.arriveLandTime - 0.5f) * 0.5f;
                    GameCamera.instance.rtsPoser.disableDist = true;
                    __instance.player.controller.cameraTargetLockExternal = true;
                    __instance.player.controller.memCameraTargetPos = __instance.player.cameraTarget.position;
                    __instance.player.controller.memCameraTargetRot = __instance.player.cameraTarget.rotation;
                    GameCamera.instance.overrideGameMode = 0;
                }

                __instance.spaceCapsule.animTime = Mathf.Max(0f, (__instance.arriveLandTime - 0.6f) / 0.1f);
                __instance.forceLocalPlanet = true;
                if (__instance.arriveLandTime == 1f)
                {
                    __instance.player.uVelocity = VectorLF3.zero;
                    __instance.player.controller.velocityOnLanding = Vector3.zero;
                    __instance.arriveLandTime = 0f;
                    __instance.stage = GuideMissionStandardMode.Stage.Land;
                }
            }
            else if (__instance.stage == GuideMissionStandardMode.Stage.Land)
            {
                __instance.gameData.disableController = true;
                GameCamera.instance.sailOverride = true;
                __instance.sailPoser.disableRot = true;
                __instance.sailPoser.disableDist = true;
                __instance.arriveLandTime += 0.016666668f;
                __instance.player.uPosition = (__instance.targetUPos - __instance.localPlanet.uPosition).normalized * (__instance.localPlanet.realRadius + 0.2f + 1.58f) + __instance.localPlanet.uPosition;
                __instance.player.position = __instance.targetPos.normalized * (__instance.localPlanet.realRadius + 0.2f + 1.58f);
                __instance.player.uRotation = __instance.targetURot;
                __instance.player.uVelocity = VectorLF3.zero;
                __instance.player.controller.velocityOnLanding = Vector3.zero;
                __instance.player.transform.rotation = __instance.targetRot;
                __instance.player.controller.memCameraTargetRot = __instance.targetRot;
                __instance.player.cameraTarget.rotation = __instance.targetRot;
                __instance.spaceCapsule.animTime = 4f + __instance.arriveLandTime;
                GS3.Warn("Unhiding player model");
                __instance.gameData.hidePlayerModel = false;
                if (__instance.arriveLandTime > 1f)
                {
                    __instance.player.movementState = EMovementState.Walk;
                    __instance.player.animator.Refresh();
                }

                __instance.forceLocalPlanet = true;
                if (__instance.arriveLandTime >= 5f)
                {
                    __instance.arriveLandTime = 5f;
                    __instance.gameData.EndStandardModeGuide();
                    GS3.Warn("StandardModeGuide Ended");
                    if (GameMain.gameScenario != null) GameMain.gameScenario.NotifyOnGameStart();
                }
            }

            __instance.elapseTime += 0.016666668f;
            // GS3.Log(__instance.stage.ToString() + " " + __instance.player.uVelocity.magnitude);
            return false;
        }


    }
}