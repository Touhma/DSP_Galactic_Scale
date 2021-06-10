using UnityEngine;
using HarmonyLib;
using System;

namespace GalacticScale
{
    public class PatchOnPlayerMove_Sail
    {
        [HarmonyPatch(typeof(PlayerMove_Sail), "GameTick"), HarmonyPrefix]
        public static bool PatchGasPlanetSailing(ref PlayerMove_Sail __instance)
        {
            __instance.maxSailSpeed = __instance.player.mecha.maxSailSpeed;
            __instance.maxWarpSpeed = __instance.player.mecha.maxWarpSpeed;
            double dt = 1.0 / 60.0;
            if (__instance.player.sailing)
            {
                ++__instance.sailCounter;
                if (__instance.navigation.navigating)
                    __instance.navigation.DetermineSailVelocity();
                __instance.uForce = VectorLF3.zero;
                PlanetData localPlanet = GameMain.localPlanet;
                StarData localStar = GameMain.localStar;
                if (__instance.player.warping)
                    Assert.Null((object)localPlanet);
                bool guideRunning = __instance.controller.gameData.guideRunning;
                bool disableController = __instance.controller.gameData.disableController;
                bool flag1 = VFInput._warpKey.onDown && !disableController;
                float num1 = guideRunning ? 0.4f : 1f;
                __instance.energy_scale = guideRunning ? 0.0 : 1.0;
                if (__instance.player.mecha.thrusterLevel >= 3)
                {
                    if (flag1)
                    {
                        if (__instance.player.warping)
                        {
                            __instance.player.warpCommand = false;
                            VFAudio.Create("warp-end", __instance.player.transform, Vector3.zero, true);
                        }
                        else if (localPlanet == null)
                        {
                            if (__instance.mecha.coreEnergy > __instance.mecha.warpStartPowerPerSpeed * (double)__instance.maxWarpSpeed)
                            {
                                if (__instance.player.mecha.UseWarper())
                                {
                                    __instance.player.warpCommand = true;
                                    VFAudio.Create("warp-begin", __instance.player.transform, Vector3.zero, true);
                                    GameMain.gameScenario.NotifyOnWarpModeEnter();
                                }
                                else
                                    UIRealtimeTip.PopupAhead("空间翘曲器不足".Translate());
                            }
                            else
                                UIRealtimeTip.PopupAhead("曲速能量不足".Translate());
                        }
                    }
                    if (__instance.player.warping && __instance.player.warpCommand && localStar != null)
                    {
                        for (int index = 0; index < localStar.planetCount; ++index)
                        {
                            PlanetData planet = localStar.planets[index];
                            VectorLF3 vectorLf3 = planet.uPosition - __instance.player.uPosition;
                            VectorLF3 normalized1 = vectorLf3.normalized;
                            VectorLF3 normalized2 = __instance.player.uVelocity.normalized;
                            double magnitude = vectorLf3.magnitude;
                            VectorLF3 b = normalized2;
                            double num2 = VectorLF3.Dot(normalized1, b);
                            double num3 = __instance.currentWarpSpeed * 0.1;
                            double num4 = (double)planet.realRadius + 500.0 + num3;
                            if (num4 > 4000.0 + (double)planet.realRadius)
                                num4 = 4000.0 + (double)planet.realRadius;
                            double num5 = (double)planet.realRadius + num4 * 0.200000002980232;
                            if (magnitude < num4 && num2 > Math.Sqrt(Math.Max(0.0, magnitude * magnitude - num5 * num5)) / magnitude)
                            {
                                __instance.player.warpCommand = false;
                                VFAudio.Create("warp-end", __instance.player.transform, Vector3.zero, true);
                                __instance.player.uVelocity = (VectorLF3)(__instance.player.uRotation.Forward() * 50f);
                                break;
                            }
                        }
                    }
                    if (__instance.player.warping && !disableController)
                    {
                        if (VFInput._sailSpeedUp)
                            __instance.warpSpeedControl += 0.008;
                        if ((double)__instance.controller.input0.y < 0.0)
                            __instance.warpSpeedControl -= 0.008;
                        if (__instance.warpSpeedControl > 1.0)
                            __instance.warpSpeedControl = 1.0;
                        if (__instance.warpSpeedControl < 0.2)
                            __instance.warpSpeedControl = 0.2;
                    }
                    else if (!__instance.player.warping)
                        __instance.warpSpeedControl = 1.0;
                }
                else
                {
                    __instance.player.warpCommand = false;
                    if (localPlanet == null & flag1)
                        UIRealtimeTip.PopupAhead("驱动引擎等级不足".Translate());
                }
                if (__instance.player.warping && !__instance.UseWarpEnergy(dt))
                    __instance.player.warpCommand = false;
                Quaternion q = Quaternion.identity;
                VectorLF3 uPosition = __instance.player.uPosition;
                VectorLF3 zero = VectorLF3.zero;
                VectorLF3 vectorLf3_1 = VectorLF3.zero;
                VectorLF3 vectorLf3_2 = VectorLF3.zero;
                double altitude = 1000.0;
                double num7 = 0.0;
                double num8 = 0.0;
                double num9 = 0.0;
                if (localPlanet != null)
                {
                    uPosition = localPlanet.uPosition;
                    q = localPlanet.runtimeRotation;
                    VectorLF3 v = __instance.player.uPosition - uPosition;
                    altitude = v.magnitude - (double)localPlanet.realRadius;
                    if (altitude < 0) __instance.player.uPosition = localPlanet.uPosition + VectorLF3.unit_z * 2 * localPlanet.realRadius;
                    VectorLF3 vectorLf3_3 = Maths.QInvRotateLF(q, v);
                    vectorLf3_1 = localPlanet.GetUniversalVelocityAtLocalPoint(GameMain.gameTime, (Vector3)vectorLf3_3);
                    if (guideRunning)
                        vectorLf3_1 = VectorLF3.zero;
                    num7 = Maths.Clamp01((600.0 - altitude) / 450.0);
                    num8 = Maths.Clamp01((600.0 - altitude) / 300.0);
                    vectorLf3_2 = vectorLf3_1 * num7;
                }
                __instance.visual_uvel = __instance.player.uVelocity - vectorLf3_2;
                double magnitude1 = __instance.visual_uvel.magnitude;
                __instance.input_aff_0 -= dt;
                __instance.input_aff_1 -= dt * 0.35;
                __instance.input_aff_2 -= dt * 0.7;
                if (__instance.input_aff_0 < 0.0)
                    __instance.input_aff_0 = 0.0;
                if (__instance.input_aff_1 < 0.0)
                    __instance.input_aff_1 = 0.0;
                if (__instance.input_aff_2 < 0.0)
                    __instance.input_aff_2 = 0.0;
                __instance.input_cd += dt;
                if (__instance.input_cd > 1.0)
                    __instance.input_cd = 1.0;
                Vector3 vector3_1 = new Vector3(__instance.controller.input0.x, Mathf.Max(0.0f, __instance.controller.input0.y), __instance.controller.input1.y) * (float)Maths.Clamp01(__instance.input_cd);
                bool flag2 = VFInput._sailSpeedUp && !disableController;
                if (__instance.navigation.navigating)
                {
                    vector3_1 = Vector3.zero;
                    __instance.controller.input0.x = 0.0f;
                    if ((double)__instance.controller.input0.y > 0.0)
                        __instance.controller.input0.y = 0.0f;
                    __instance.controller.input0.z = 0.0f;
                    __instance.controller.input0.w = 0.0f;
                    __instance.controller.input1.y = 0.0f;
                    __instance.controller.input1.z = 0.0f;
                    __instance.controller.input1.w = 0.0f;
                }
                bool flag3 = false;
                if ((double)vector3_1.sqrMagnitude > 0.25)
                {
                    int num2 = (double)vector3_1.y > 0.5 ? 1 : 0;
                    if (num2 == 0 && (double)vector3_1.x * (double)vector3_1.x + (double)vector3_1.z * (double)vector3_1.z > 0.25)
                        vector3_1.y += 1.2f;
                    __instance.input_aff_1 = 1.0;
                    __instance.input_aff_0 += dt * 2.0;
                    VectorLF3 vectorLf3_3 = Maths.QRotateLF(__instance.sailPoser.targetURot, (VectorLF3)Vector3.forward);
                    VectorLF3 vectorLf3_4 = Maths.QRotateLF(__instance.sailPoser.targetURot, (VectorLF3)Vector3.up);
                    VectorLF3 vectorLf3_5 = Maths.QRotateLF(__instance.sailPoser.targetURot, (VectorLF3)Vector3.right);
                    double y = (double)vector3_1.y;
                    VectorLF3 normalized = (vectorLf3_3 * y + vectorLf3_4 * (double)vector3_1.z + vectorLf3_5 * (double)vector3_1.x).normalized;
                    double num3 = magnitude1;
                    if (num2 != 0)
                    {
                        flag3 = true;
                        num3 = magnitude1 > 100.0 ? magnitude1 : 100.0;
                        if (flag2 && !__instance.player.warping)
                        {
                            double dvel = num3 * 0.02;
                            if (dvel < 7.0)
                                dvel = 7.0;
                            else if (dvel > __instance.max_acc)
                                dvel = __instance.max_acc;
                            if (num3 + dvel > (double)__instance.maxSailSpeed)
                                dvel = (double)__instance.maxSailSpeed - num3;
                            double num4 = dvel * __instance.UseSailEnergy(dvel);
                            num3 += num4;
                        }
                    }
                    VectorLF3 vectorLf3_6 = normalized * num3 + vectorLf3_2;
                    float b = Vector3.Angle((Vector3)vectorLf3_6, (Vector3)__instance.player.uVelocity);
                    float t = 1.6f * num1 / Mathf.Max(10f, b);
                    VectorLF3 dvel1 = (VectorLF3)Vector3.Slerp((Vector3)__instance.player.uVelocity, (Vector3)vectorLf3_6, t) - __instance.player.uVelocity;
                    __instance.UseSailEnergy(ref dvel1, 0.360000014305115);
                    if (!disableController)
                        __instance.player.uVelocity += dvel1;
                }
                if (!flag3 & flag2 && !__instance.player.warping)
                {
                    double num2 = magnitude1;
                    double dvel = num2 * 0.02;
                    if (dvel < 7.0)
                        dvel = 7.0;
                    else if (dvel > __instance.max_acc)
                        dvel = __instance.max_acc;
                    if (num2 + dvel > (double)__instance.maxSailSpeed)
                        dvel = (double)__instance.maxSailSpeed - num2;
                    double num3 = dvel * __instance.UseSailEnergy(dvel);
                    double num4 = num2 + num3;
                    __instance.input_aff_1 = 0.5;
                    VectorLF3 vectorLf3_3 = __instance.visual_uvel.normalized * num4 + vectorLf3_2;
                    float b = Vector3.Angle((Vector3)vectorLf3_3, (Vector3)__instance.player.uVelocity);
                    float t = 1.6f * num1 / Mathf.Max(10f, b);
                    __instance.player.uVelocity = (VectorLF3)Vector3.Slerp((Vector3)__instance.player.uVelocity, (Vector3)vectorLf3_3, t);
                }
                if ((double)__instance.controller.input0.y < 0.0 && !__instance.player.warping)
                {
                    VectorLF3 dvel = __instance.visual_uvel * 0.008;
                    __instance.UseSailEnergy(ref dvel, 1.5);
                    __instance.visual_uvel -= dvel;
                    __instance.input_aff_1 = 0.85;
                    __instance.player.uVelocity = __instance.visual_uvel + vectorLf3_2;
                    __instance.input_aff_2 = 0.7;
                }
                if ((double)__instance.controller.input1.x != 0.0)
                {
                    __instance.sailPoser.targetURotWanted = Quaternion.AngleAxis(-__instance.controller.input1.x, (Vector3)(VectorLF3)(__instance.player.uRotation * Vector3.forward)) * __instance.sailPoser.targetURotWanted;
                    __instance.input_aff_0 += dt * 2.0;
                }
                if (__instance.input_aff_0 > 1.0)
                    __instance.input_aff_0 = 1.0;
                VectorLF3 universalGravity = __instance.controller.universalGravity;
                double magnitude2 = universalGravity.magnitude;
                if (!guideRunning)
                    __instance.uForce = universalGravity;
                if (localPlanet != null)
                {
                    VectorLF3 vectorLf3_3 = __instance.player.uPosition - uPosition;
                    double magnitude3 = vectorLf3_3.magnitude;
                    VectorLF3 vectorLf3_4 = vectorLf3_3 / magnitude3;
                    VectorLF3 vectorLf3_5 = __instance.player.uVelocity - vectorLf3_1;
                    double num2 = num7 * num7 * 0.08 * (1.0 - __instance.input_aff_1) * (1.0 - __instance.input_aff_1);
                    double num3 = vectorLf3_5.x * vectorLf3_4.x + vectorLf3_5.y * vectorLf3_4.y + vectorLf3_5.z * vectorLf3_4.z;
                    VectorLF3 vectorLf3_6 = vectorLf3_4 * num3;
                    VectorLF3 vectorLf3_7 = vectorLf3_5 - vectorLf3_6;
                    double magnitude4 = vectorLf3_7.magnitude;
                    double num4 = magnitude4 * magnitude4 / magnitude3 * dt;
                    double num5 = Math.Sqrt(magnitude2 * magnitude3 * 0.7);
                    double num10 = magnitude4 * (1.0 - num2) + num5 * num2;
                    double num11 = altitude > 60.0 ? -(altitude - 60.0) * 0.01 : (60.0 - altitude) * 0.5;
                    double num12 = num3 * (1.0 - num2) + num11 * num2 - num4 * num8 * (1.0 - __instance.input_aff_1);
                    VectorLF3 dvel = vectorLf3_4 * num12 + vectorLf3_7.normalized * num10 + vectorLf3_1 - __instance.player.uVelocity;
                    __instance.UseSailEnergy(ref dvel, 1.5);
                    if (!disableController && !guideRunning)
                        __instance.player.uVelocity += dvel;
                    double num13 = num7 * (1.0 - __instance.input_aff_2);
                    double num14 = magnitude2 * num13;
                    if (num14 > 1.0 / 1000.0)
                    {
                        VectorLF3 force = -universalGravity * (num14 / magnitude2);
                        __instance.UseSailEnergy(ref force, dt, 1.0);
                        if (!guideRunning)
                            __instance.uForce += force;
                    }
                }
                else if (localStar != null && !__instance.player.warping)
                {
                    VectorLF3 vectorLf3_3 = __instance.player.uPosition - localStar.uPosition;
                    double magnitude3 = vectorLf3_3.magnitude;
                    VectorLF3 vectorLf3_4 = vectorLf3_3 / magnitude3;
                    double num2 = magnitude3 - (double)localStar.viewRadius;
                    if (num2 < 1000.0)
                    {
                        num9 = Maths.Clamp01((1000.0 - num2) / 500.0);
                        VectorLF3 uVelocity = __instance.player.uVelocity;
                        double num3 = num9 * num9 * 0.08 * (1.0 - __instance.input_aff_1) * (1.0 - __instance.input_aff_1);
                        double num4 = uVelocity.x * vectorLf3_4.x + uVelocity.y * vectorLf3_4.y + uVelocity.z * vectorLf3_4.z;
                        VectorLF3 vectorLf3_5 = vectorLf3_4 * num4;
                        VectorLF3 vectorLf3_6 = uVelocity - vectorLf3_5;
                        double magnitude4 = vectorLf3_6.magnitude;
                        double num5 = magnitude4 * magnitude4 / magnitude3 * dt;
                        double num10 = Math.Sqrt(magnitude2 * magnitude3 * 2.0);
                        double num11 = magnitude4 * (1.0 - num3) + num10 * num3;
                        double num12 = num2 > 300.0 ? -(num2 - 300.0) * 0.01 : (300.0 - num2) * 0.1;
                        double num13 = num4 * (1.0 - num3) + num12 * num3 - num5 * num9 * (1.0 - __instance.input_aff_1);
                        VectorLF3 dvel = vectorLf3_4 * num13 + vectorLf3_6.normalized * num11 - __instance.player.uVelocity;
                        __instance.UseSailEnergy(ref dvel, 1.5);
                        if (!disableController && !guideRunning)
                            __instance.player.uVelocity += dvel;
                        double num14 = num9 * (1.0 - __instance.input_aff_2);
                        double num15 = magnitude2 * num14;
                        if (num15 > 1.0 / 1000.0)
                        {
                            VectorLF3 force = -universalGravity * (num15 / magnitude2);
                            __instance.UseSailEnergy(ref force, dt, 1.0);
                            if (!guideRunning)
                                __instance.uForce += force;
                        }
                    }
                }
                if (!disableController)
                    __instance.player.uVelocity += __instance.uForce * dt;
                if (localPlanet != null && num8 > 0.0 && !__instance.player.warping)
                {
                    VectorLF3 vectorLf3_3 = (__instance.galaxy.astroPoses[localPlanet.id].uPosNext - __instance.galaxy.astroPoses[localPlanet.id].uPos) / dt;
                    VectorLF3 vectorLf3_4 = __instance.player.uPosition - uPosition;
                    VectorLF3 vectorLf3_5 = vectorLf3_4 + (__instance.player.uVelocity - vectorLf3_3) * dt;
                    VectorLF3 normalized = vectorLf3_4.normalized;
                    vectorLf3_5 = vectorLf3_5.normalized;
                    Quaternion quaternion = Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation((Vector3)normalized, (Vector3)vectorLf3_5), (float)num8);
                    if (!disableController)
                    {
                        __instance.sailPoser.targetURot = quaternion * __instance.sailPoser.targetURot;
                        __instance.sailPoser.targetURotWanted = quaternion * __instance.sailPoser.targetURotWanted;
                    }
                }
                else if (localStar != null && !__instance.player.warping && num9 > 0.0)
                {
                    VectorLF3 vectorLf3_3 = __instance.player.uPosition - localStar.uPosition;
                    VectorLF3 vectorLf3_4 = vectorLf3_3 + __instance.player.uVelocity * dt;
                    vectorLf3_3 = vectorLf3_3.normalized;
                    vectorLf3_4 = vectorLf3_4.normalized;
                    Quaternion quaternion = Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation((Vector3)vectorLf3_3, (Vector3)vectorLf3_4), (float)num9);
                    if (!disableController)
                    {
                        __instance.sailPoser.targetURot = quaternion * __instance.sailPoser.targetURot;
                        __instance.sailPoser.targetURotWanted = quaternion * __instance.sailPoser.targetURotWanted;
                    }
                }
                __instance.visual_uvel = __instance.player.uVelocity - vectorLf3_2;
                double magnitude5 = __instance.visual_uvel.magnitude;
                if (localPlanet != null && !__instance.player.warping && !guideRunning)
                {
                    float num2 = Mathf.Abs(__instance.controller.vertSpeed);
                    if (localPlanet.type != EPlanetType.Gas)
                    {
                        if (altitude < 46.0 && magnitude5 < 75.0 && (double)num2 < 7.0 || altitude < 25.0 && magnitude5 < 85.0 && (double)num2 < 18.0 || altitude < 7.0)
                        {
                            __instance.controller.movementStateInFrame = EMovementState.Fly;
                            __instance.controller.actionFly.targetAltitude = (float)altitude;
                            GameCamera.instance.SyncForSailMode();
                            if (!guideRunning)
                                __instance.controller.velocityOnLanding = Maths.QInvRotate(q, (Vector3)__instance.visual_uvel);
                            else
                                __instance.controller.velocityOnLanding = Vector3.zero;
                            Debug.DrawRay(__instance.player.position, __instance.controller.velocityOnLanding * 0.3f, Color.yellow, 20f);
                        }
                    }
                    else
                    {
                        __instance.SoftLimit(localPlanet.uPosition, (double)localPlanet.realRadius, (double)localPlanet.realRadius + 48.0, 1f, ref __instance.player.uPosition, ref __instance.player.uVelocity);
                        if (altitude < 48.0)
                        {
                            __instance.controller.movementStateInFrame = EMovementState.Fly;
                            __instance.controller.actionFly.targetAltitude = Mathf.Max(20f, (float)altitude);
                            GameCamera.instance.SyncForSailMode();
                            if (!guideRunning)
                                __instance.controller.velocityOnLanding = Maths.QInvRotate(q, (Vector3)__instance.visual_uvel);
                            else
                                __instance.controller.velocityOnLanding = Vector3.zero;
                            Debug.DrawRay(__instance.player.position, __instance.controller.velocityOnLanding * 0.3f, Color.yellow, 20f);
                        }
                    }
                }
                else if (localStar != null)
                {
                    __instance.SoftLimit(localStar.uPosition, (double)localStar.viewRadius + 10.0, (double)localStar.viewRadius + 80.0, 1f, ref __instance.player.uPosition, ref __instance.player.uVelocity);
                    if (guideRunning)
                    {
                        for (int index = 0; index < localStar.planetCount; ++index)
                        {
                            PlanetData planet = localStar.planets[index];
                            if (planet.id != GameMain.galaxy.birthPlanetId)
                                __instance.SoftLimit(planet.uPosition, (double)planet.realRadius + 40.0, (double)planet.realRadius + 400.0, 0.025f, ref __instance.player.uPosition, ref __instance.player.uVelocity);
                        }
                    }
                }
                if (!disableController)
                {
                    __instance.player.uPosition += __instance.player.uVelocity * dt;
                    if (__instance.player.warping)
                        __instance.player.uPosition += __instance.currentWarpVelocity * dt;
                }
                __instance.controller.gameData.DetermineRelative();
                if (__instance.player.uPosition.x < -480000000.0)
                    __instance.player.uPosition.x = -480000000.0;
                else if (__instance.player.uPosition.x > 480000000.0)
                    __instance.player.uPosition.x = 480000000.0;
                if (__instance.player.uPosition.y < -240000000.0)
                    __instance.player.uPosition.y = -240000000.0;
                else if (__instance.player.uPosition.y > 240000000.0)
                    __instance.player.uPosition.y = 240000000.0;
                if (__instance.player.uPosition.z < -480000000.0)
                    __instance.player.uPosition.z = -480000000.0;
                else if (__instance.player.uPosition.z > 480000000.0)
                    __instance.player.uPosition.z = 480000000.0;
                Vector3 vector3_2 = Vector3.zero;
                if (localPlanet != null)
                    vector3_2 = Vector3.ClampMagnitude(Maths.QInvRotate(q, (Vector3)__instance.visual_uvel) * 0.8f, 110f);
                __instance.controller.actionWalk.rtsVelocity = Vector3.zero;
                __instance.controller.actionWalk.moveVelocity = vector3_2;
                __instance.controller.actionDrift.rtsVelocity = Vector3.zero;
                __instance.controller.actionDrift.moveVelocity = vector3_2;
                __instance.controller.actionFly.rtsVelocity = Vector3.zero;
                __instance.controller.actionFly.moveVelocity = vector3_2;
            }
            else
                __instance.sailCounter = 0;
            return false;
        }

        [HarmonyPatch(typeof(PlayerMove_Fly), "OnNavigationArrive"), HarmonyPrefix]
        public static bool OnNavigationArrive(ref PlayerMove_Fly __instance)
        {
            if (__instance.player.movementState != EMovementState.Fly)
                return false;
            PlanetData planetData = __instance.player.planetData;
            if (planetData == null || (planetData.type == EPlanetType.Gas && planetData.scale >= 1))
                return false;
            __instance.controller.movementStateInFrame = EMovementState.Walk;
            __instance.controller.softLandingTime = 1.2f;
            return false;
        }
        //[HarmonyPatch(typeof(PlayerMove_Fly), "GameTick"), HarmonyPrefix]
        //public static bool GameTick(ref PlayerMove_Fly __instance)
        //{
        //    float dt = 0.01666667f;
        //    if (__instance.player.movementState != EMovementState.Fly)
        //        return false;
        //    Vector3 forward = __instance.controller.mainCamera.transform.forward;
        //    Vector3 normalized1 = __instance.player.position.normalized;
        //    Vector3 normalized2 = Vector3.Cross(normalized1, forward).normalized;
        //    Vector3 to = Vector3.Cross(normalized2, normalized1) * __instance.controller.input0.y + normalized2 * __instance.controller.input0.x;
        //    if (__instance.controller.cmd.type == ECommand.Build && !VFInput._godModeMechMove && !PlayerController.operationWhenBuild || __instance.navigation.navigating)
        //        to = Vector3.zero;
        //    PlanetData localPlanet = GameMain.localPlanet;
        //    bool flag = localPlanet != null && localPlanet.type != EPlanetType.Gas;
        //    float softLandingRecover = __instance.controller.softLandingRecover;
        //    float num1 = softLandingRecover * softLandingRecover;
        //    float num2 = 0.022f * num1;
        //    float num3 = __instance.player.mecha.walkSpeed * 2.5f;
        //    float num4 = __instance.controller.input1.y;
        //    if (__instance.controller.cmd.type == ECommand.Build && !VFInput._godModeMechMove && !PlayerController.operationWhenBuild || __instance.navigation.navigating)
        //        num4 = 0.0f;
        //    if (__instance.navigation.navigating)
        //    {
        //        bool lift = false;
        //        bool drop = false;
        //        __instance.navigation.DetermineHighOperation(num3, ref lift, ref drop);
        //        num4 = 0.0f;
        //        if (lift)
        //            ++num4;
        //        if (drop && (double)__instance.targetAltitude > 15.0100002288818 + (double)dt * 20.0)
        //            num4 += -1f;
        //    }
        //    __instance.targetAltitude += (float)((double)num4 * (double)dt * 20.0);
        //    //GS2.Log("TA"+__instance.targetAltitude);
        //    __instance.targetAltitude = 3;
        //    if (__instance.controller.cmd.type == ECommand.Build && !PlayerController.operationWhenBuild && (double)__instance.targetAltitude > 40.0)
        //        __instance.targetAltitude = 40f;
        //    if ((double)num4 == 0.0 && (double)__instance.targetAltitude > 15.0)
        //    {
        //        __instance.targetAltitude -= (float)((double)dt * 20.0 * 0.300000011920929);
        //        if ((double)__instance.targetAltitude < 15.0)
        //            __instance.targetAltitude = 15f;
        //    }
        //    else if ((double)__instance.targetAltitude >= 50.0 || (localPlanet.scale < 1 && __instance.targetAltitude >= 5))
        //    {
        //        if (((double)__instance.currentAltitude > 49.0 && (double)__instance.controller.horzSpeed > 12.5 && __instance.mecha.thrusterLevel >= 2)
        //            || (localPlanet.scale < 1 && (double)__instance.currentAltitude > 4.90 && (double)__instance.controller.horzSpeed > 12.5 && __instance.mecha.thrusterLevel >= 2))
        //        {
        //            if (__instance.controller.cmd.type == ECommand.Build)
        //                __instance.controller.cmd.type = ECommand.None;
        //            __instance.controller.movementStateInFrame = EMovementState.Sail;
        //            __instance.controller.actionSail.ResetSailState();
        //            GameCamera.instance.SyncForSailMode();
        //            GameMain.gameScenario.NotifyOnSailModeEnter();
        //        }
        //        __instance.targetAltitude = 50f;
        //        if (localPlanet.scale < 1) __instance.targetAltitude = 5f;
        //    }
        //    if (flag)
        //    {
        //        if ((double)__instance.targetAltitude < 14.5)
        //        {
        //            __instance.targetAltitude = (double)num4 <= 0.0 ? 1f : 15f;
        //            if ((double)__instance.currentAltitude < 3.0)
        //            {
        //                __instance.controller.movementStateInFrame = EMovementState.Walk;
        //                __instance.controller.softLandingTime = 1.2f;
        //            }
        //        }
        //    }
        //    else if ((double)__instance.targetAltitude < 20.0 && localPlanet.scale >= 1)
        //        __instance.targetAltitude = 20f;
        //    float realRadius = __instance.player.planetData.realRadius;
        //    __instance.currentAltitude = Mathf.Max(__instance.player.position.magnitude, realRadius * 0.9f) - realRadius;
        //    float f = __instance.targetAltitude - __instance.currentAltitude;
        //    __instance.verticalThrusterForce = 0.0f;
        //    __instance.verticalThrusterForce += Mathf.Max(0.0f, Mathf.Clamp(f * 0.5f, -10f, 10f) * 100f + (float)__instance.controller.universalGravity.magnitude);
        //    __instance.UseThrustEnergy(ref __instance.verticalThrusterForce, __instance.controller.vertSpeed, 1.0 / 60.0);
        //    float num5 = (float)((double)Mathf.Sin(Time.time * 2f) * 0.100000001490116 + 1.0);
        //    //float scaleFactor = 1f;
        //    //if (localPlanet.scale < 1) scaleFactor = 0.1f;
        //    if ((double)Mathf.Abs(__instance.verticalThrusterForce) > 1.0 / 1000.0)
        //        __instance.controller.AddLocalForce(normalized1 * (__instance.verticalThrusterForce * num5));
        //    OrderNode currentOrder = __instance.player.currentOrder;
        //    if (currentOrder != null && !currentOrder.targetReached)
        //    {
        //        Vector3 rhs = currentOrder.target.normalized * localPlanet.realRadius - __instance.player.position.normalized * localPlanet.realRadius;
        //        float magnitude = rhs.magnitude;
        //        __instance.rtsVelocity = Vector3.Slerp(__instance.rtsVelocity, Vector3.Cross(Vector3.Cross(normalized1, rhs).normalized, normalized1).normalized * num3, num2);
        //    }
        //    else
        //    { __instance.rtsVelocity = Vector3.MoveTowards(__instance.rtsVelocity, Vector3.zero, dt * 6f * num3); }
        //    if (__instance.navigation.navigating)
        //        __instance.navigation.DetermineHighVelocity(num3, num2, ref __instance.moveVelocity, dt);
        //    else
        //        __instance.moveVelocity = Vector3.Slerp(__instance.moveVelocity, to * num3, num2);
        //    Vector3 vel = __instance.moveVelocity + __instance.rtsVelocity;
        //    //GS2.Log(num1.ToString());
        //    if ((double)num1 > 0.9)
        //        vel = Vector3.ClampMagnitude(vel, num3);
        //    __instance.UseFlyEnergy(ref vel, __instance.mecha.walkPower * (double)dt * (double)__instance.controller.softLandingRecover);
        //    Vector3 vector3 = Vector3.Dot(vel, normalized1) * normalized1;
        //    vel -= vector3;
        //    float vertSpeed = __instance.controller.vertSpeed;
        //    float b1 = Mathf.Lerp(0.95f, 0.8f, Mathf.Abs(f) * 0.3f);
        //    float b2 = b1;
        //    float num6 = Mathf.Lerp(1f, b1, Mathf.Clamp01(__instance.verticalThrusterForce));
        //    float num7 = Mathf.Lerp(1f, b2, Mathf.Clamp01(__instance.verticalThrusterForce) * Mathf.Clamp01((float)(__instance.mecha.coreEnergy - 5000.0) * 0.0001f));
        //    if ((double)vertSpeed > 0.0)
        //        vertSpeed *= num6;
        //    else if ((double)vertSpeed < 0.0)
        //        vertSpeed *= num7;
        //    __instance.controller.velocity = vertSpeed * normalized1 + vel;
        //    if ((double)to.sqrMagnitude > 0.25)
        //        __instance.controller.turning = Vector3.SignedAngle(vel, to, normalized1);
        //    else
        //        __instance.controller.turning = 0.0f;
        //    if (flag && __instance.mecha.coreEnergy < 10000.0)
        //    {
        //        __instance.controller.movementStateInFrame = EMovementState.Walk;
        //        __instance.controller.softLandingTime = 1.2f;
        //    }
        //    __instance.controller.actionWalk.rtsVelocity = __instance.rtsVelocity;
        //    __instance.controller.actionWalk.moveVelocity = __instance.moveVelocity * 0.5f;
        //    __instance.controller.actionDrift.rtsVelocity = __instance.rtsVelocity;
        //    __instance.controller.actionDrift.moveVelocity = __instance.moveVelocity * 0.5f;
        //    return false;
        //}

    }
}