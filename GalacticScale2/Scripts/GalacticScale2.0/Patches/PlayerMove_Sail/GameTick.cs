namespace GalacticScale
{
    // ReSharper disable once InconsistentNaming
    public partial class PatchOnPlayerMove_Sail
    {
        // [HarmonyPatch(typeof(PlayerMove_Sail), "GameTick")]
        // [HarmonyPrefix]
        // public static bool PatchGasPlanetSailing(ref PlayerMove_Sail __instance, long timei)
        // {
        //     __instance.maxSailSpeed = __instance.player.mecha.maxSailSpeed;
        //     __instance.maxWarpSpeed = __instance.player.mecha.maxWarpSpeed;
        //     var dt = 1.0 / 60.0;
        //     if (__instance.player.sailing)
        //     {
        //         ++__instance.sailCounter;
        //         if (__instance.navigation.navigating)
        //             __instance.navigation.DetermineSailVelocity();
        //         __instance.uForce = VectorLF3.zero;
        //         var localPlanet = GameMain.localPlanet;
        //         var localStar = GameMain.localStar;
        //         if (__instance.player.warping)
        //             Assert.Null(localPlanet);
        //         var guideRunning = __instance.controller.gameData.guideRunning;
        //         var disableController = __instance.controller.gameData.disableController;
        //         var flag1 = VFInput._warpKey.onDown && !disableController;
        //         var num1 = guideRunning ? 0.4f : 1f;
        //         __instance.energy_scale = guideRunning ? 0.0 : 1.0;
        //         if (__instance.player.mecha.thrusterLevel >= 3)
        //         {
        //             if (flag1)
        //             {
        //                 if (__instance.player.warping)
        //                 {
        //                     __instance.player.warpCommand = false;
        //                     VFAudio.Create("warp-end", __instance.player.transform, Vector3.zero, true);
        //                 }
        //                 else if (localPlanet == null)
        //                 {
        //                     if (__instance.mecha.coreEnergy >
        //                         __instance.mecha.warpStartPowerPerSpeed * __instance.maxWarpSpeed)
        //                     {
        //                         if (__instance.player.mecha.UseWarper())
        //                         {
        //                             __instance.player.warpCommand = true;
        //                             VFAudio.Create("warp-begin", __instance.player.transform, Vector3.zero, true);
        //                             GameMain.gameScenario.NotifyOnWarpModeEnter();
        //                         }
        //                         else
        //                         {
        //                             UIRealtimeTip.PopupAhead("空间翘曲器不足".Translate());
        //                         }
        //                     }
        //                     else
        //                     {
        //                         UIRealtimeTip.PopupAhead("曲速能量不足".Translate());
        //                     }
        //                 }
        //             }
        //
        //             if (__instance.player.warping && __instance.player.warpCommand && localStar != null)
        //                 for (var index = 0; index < localStar.planetCount; ++index)
        //                 {
        //                     var planet = localStar.planets[index];
        //                     var vectorLf3 = planet.uPosition - __instance.player.uPosition;
        //                     var normalized1 = vectorLf3.normalized;
        //                     var normalized2 = __instance.player.uVelocity.normalized;
        //                     var magnitude = vectorLf3.magnitude;
        //                     var b = normalized2;
        //                     var num2 = VectorLF3.Dot(normalized1, b);
        //                     var num3 = __instance.currentWarpSpeed * 0.1;
        //                     var num4 = planet.realRadius + 500.0 + num3;
        //                     if (num4 > 4000.0 + planet.realRadius)
        //                         num4 = 4000.0 + planet.realRadius;
        //                     var num5 = planet.realRadius + num4 * 0.200000002980232;
        //                     if (magnitude < num4 && num2 >
        //                         Math.Sqrt(Math.Max(0.0, magnitude * magnitude - num5 * num5)) / magnitude)
        //                     {
        //                         __instance.player.warpCommand = false;
        //                         VFAudio.Create("warp-end", __instance.player.transform, Vector3.zero, true);
        //                         __instance.player.uVelocity = __instance.player.uRotation.Forward() * 50f;
        //                         break;
        //                     }
        //                 }
        //
        //             if (__instance.player.warping && !disableController)
        //             {
        //                 if (VFInput._sailSpeedUp)
        //                     __instance.warpSpeedControl += 0.008;
        //                 if (__instance.controller.input0.y < 0.0)
        //                     __instance.warpSpeedControl -= 0.008;
        //                 if (__instance.warpSpeedControl > 1.0)
        //                     __instance.warpSpeedControl = 1.0;
        //                 if (__instance.warpSpeedControl < 0.2)
        //                     __instance.warpSpeedControl = 0.2;
        //             }
        //             else if (!__instance.player.warping)
        //             {
        //                 __instance.warpSpeedControl = 1.0;
        //             }
        //         }
        //         else
        //         {
        //             __instance.player.warpCommand = false;
        //             if ((localPlanet == null) & flag1)
        //                 UIRealtimeTip.PopupAhead("驱动引擎等级不足".Translate());
        //         }
        //
        //         if (__instance.player.warping && !__instance.UseWarpEnergy(dt))
        //             __instance.player.warpCommand = false;
        //         var q = Quaternion.identity;
        //         var uPosition = __instance.player.uPosition;
        //         var zero = VectorLF3.zero;
        //         var vectorLf3_1 = VectorLF3.zero;
        //         var vectorLf3_2 = VectorLF3.zero;
        //         var altitude = 1000.0;
        //         var num7 = 0.0;
        //         var num8 = 0.0;
        //         var num9 = 0.0;
        //         if (localPlanet != null)
        //         {
        //             uPosition = localPlanet.uPosition;
        //             q = localPlanet.runtimeRotation;
        //             var v = __instance.player.uPosition - uPosition;
        //             altitude = v.magnitude - localPlanet.realRadius;
        //             if (altitude < 0) //****
        //                 __instance.player.uPosition = //****
        //                     localPlanet.uPosition + VectorLF3.unit_z * 2 * localPlanet.realRadius; //**** Added to 
        //             var vectorLf3_3 = Maths.QInvRotateLF(q, v);
        //             vectorLf3_1 = localPlanet.GetUniversalVelocityAtLocalPoint(GameMain.gameTime, vectorLf3_3);
        //             if (guideRunning)
        //                 vectorLf3_1 = VectorLF3.zero;
        //             num7 = Maths.Clamp01((600.0 - altitude) / 450.0);
        //             num8 = Maths.Clamp01((600.0 - altitude) / 300.0);
        //             vectorLf3_2 = vectorLf3_1 * num7;
        //         }
        //
        //         __instance.visual_uvel = __instance.player.uVelocity - vectorLf3_2;
        //         var magnitude1 = __instance.visual_uvel.magnitude;
        //         __instance.input_aff_0 -= dt;
        //         __instance.input_aff_1 -= dt * 0.35;
        //         __instance.input_aff_2 -= dt * 0.7;
        //         if (__instance.input_aff_0 < 0.0)
        //             __instance.input_aff_0 = 0.0;
        //         if (__instance.input_aff_1 < 0.0)
        //             __instance.input_aff_1 = 0.0;
        //         if (__instance.input_aff_2 < 0.0)
        //             __instance.input_aff_2 = 0.0;
        //         __instance.input_cd += dt;
        //         if (__instance.input_cd > 1.0)
        //             __instance.input_cd = 1.0;
        //         var vector3_1 =
        //             new Vector3(__instance.controller.input0.x, Mathf.Max(0.0f, __instance.controller.input0.y),
        //                 __instance.controller.input1.y) * (float) Maths.Clamp01(__instance.input_cd);
        //         var flag2 = VFInput._sailSpeedUp && !disableController;
        //         if (__instance.navigation.navigating)
        //         {
        //             vector3_1 = Vector3.zero;
        //             __instance.controller.input0.x = 0.0f;
        //             if (__instance.controller.input0.y > 0.0)
        //                 __instance.controller.input0.y = 0.0f;
        //             __instance.controller.input0.z = 0.0f;
        //             __instance.controller.input0.w = 0.0f;
        //             __instance.controller.input1.y = 0.0f;
        //             __instance.controller.input1.z = 0.0f;
        //             __instance.controller.input1.w = 0.0f;
        //         }
        //
        //         var flag3 = false;
        //         if (vector3_1.sqrMagnitude > 0.25)
        //         {
        //             var num2 = vector3_1.y > 0.5 ? 1 : 0;
        //             if (num2 == 0 && vector3_1.x * (double) vector3_1.x + vector3_1.z * (double) vector3_1.z > 0.25)
        //                 vector3_1.y += 1.2f;
        //             __instance.input_aff_1 = 1.0;
        //             __instance.input_aff_0 += dt * 2.0;
        //             var vectorLf3_3 = Maths.QRotateLF(__instance.sailPoser.targetURot, Vector3.forward);
        //             var vectorLf3_4 = Maths.QRotateLF(__instance.sailPoser.targetURot, Vector3.up);
        //             var vectorLf3_5 = Maths.QRotateLF(__instance.sailPoser.targetURot, Vector3.right);
        //             double y = vector3_1.y;
        //             var normalized = (vectorLf3_3 * y + vectorLf3_4 * vector3_1.z + vectorLf3_5 * vector3_1.x)
        //                 .normalized;
        //             var num3 = magnitude1;
        //             if (num2 != 0)
        //             {
        //                 flag3 = true;
        //                 num3 = magnitude1 > 100.0 ? magnitude1 : 100.0;
        //                 if (flag2 && !__instance.player.warping)
        //                 {
        //                     var dvel = num3 * 0.02;
        //                     if (dvel < 7.0)
        //                         dvel = 7.0;
        //                     else if (dvel > __instance.max_acc)
        //                         dvel = __instance.max_acc;
        //                     if (num3 + dvel > __instance.maxSailSpeed)
        //                         dvel = __instance.maxSailSpeed - num3;
        //                     var num4 = dvel * __instance.UseSailEnergy(dvel);
        //                     num3 += num4;
        //                 }
        //             }
        //
        //             var vectorLf3_6 = normalized * num3 + vectorLf3_2;
        //             var b = Vector3.Angle(vectorLf3_6, __instance.player.uVelocity);
        //             var t = 1.6f * num1 / Mathf.Max(10f, b);
        //             var dvel1 = (VectorLF3) Vector3.Slerp(__instance.player.uVelocity, vectorLf3_6, t) -
        //                         __instance.player.uVelocity;
        //             __instance.UseSailEnergy(ref dvel1, 0.360000014305115);
        //             if (!disableController)
        //                 __instance.player.uVelocity += dvel1;
        //         }
        //
        //         if (!flag3 & flag2 && !__instance.player.warping)
        //         {
        //             var num2 = magnitude1;
        //             var dvel = num2 * 0.02;
        //             if (dvel < 7.0)
        //                 dvel = 7.0;
        //             else if (dvel > __instance.max_acc)
        //                 dvel = __instance.max_acc;
        //             if (num2 + dvel > __instance.maxSailSpeed)
        //                 dvel = __instance.maxSailSpeed - num2;
        //             var num3 = dvel * __instance.UseSailEnergy(dvel);
        //             var num4 = num2 + num3;
        //             __instance.input_aff_1 = 0.5;
        //             var vectorLf3_3 = __instance.visual_uvel.normalized * num4 + vectorLf3_2;
        //             var b = Vector3.Angle(vectorLf3_3, __instance.player.uVelocity);
        //             var t = 1.6f * num1 / Mathf.Max(10f, b);
        //             __instance.player.uVelocity = Vector3.Slerp(__instance.player.uVelocity, vectorLf3_3, t);
        //         }
        //
        //         if (__instance.controller.input0.y < 0.0 && !__instance.player.warping)
        //         {
        //             var dvel = __instance.visual_uvel * 0.008;
        //             __instance.UseSailEnergy(ref dvel, 1.5);
        //             __instance.visual_uvel -= dvel;
        //             __instance.input_aff_1 = 0.85;
        //             __instance.player.uVelocity = __instance.visual_uvel + vectorLf3_2;
        //             __instance.input_aff_2 = 0.7;
        //         }
        //
        //         if (__instance.controller.input1.x != 0.0)
        //         {
        //             __instance.sailPoser.targetURotWanted = Quaternion.AngleAxis(-__instance.controller.input1.x,
        //                                                         (VectorLF3) (__instance.player.uRotation *
        //                                                                      Vector3.forward)) *
        //                                                     __instance.sailPoser.targetURotWanted;
        //             __instance.input_aff_0 += dt * 2.0;
        //         }
        //
        //         if (__instance.input_aff_0 > 1.0)
        //             __instance.input_aff_0 = 1.0;
        //         var universalGravity = __instance.controller.universalGravity;
        //         var magnitude2 = universalGravity.magnitude;
        //         if (!guideRunning)
        //             __instance.uForce = universalGravity;
        //         if (localPlanet != null)
        //         {
        //             var vectorLf3_3 = __instance.player.uPosition - uPosition;
        //             var magnitude3 = vectorLf3_3.magnitude;
        //             var vectorLf3_4 = vectorLf3_3 / magnitude3;
        //             var vectorLf3_5 = __instance.player.uVelocity - vectorLf3_1;
        //             var num2 = num7 * num7 * 0.08 * (1.0 - __instance.input_aff_1) * (1.0 - __instance.input_aff_1);
        //             var num3 = vectorLf3_5.x * vectorLf3_4.x + vectorLf3_5.y * vectorLf3_4.y +
        //                        vectorLf3_5.z * vectorLf3_4.z;
        //             var vectorLf3_6 = vectorLf3_4 * num3;
        //             var vectorLf3_7 = vectorLf3_5 - vectorLf3_6;
        //             var magnitude4 = vectorLf3_7.magnitude;
        //             var num4 = magnitude4 * magnitude4 / magnitude3 * dt;
        //             var num5 = Math.Sqrt(magnitude2 * magnitude3 * 0.7);
        //             var num10 = magnitude4 * (1.0 - num2) + num5 * num2;
        //             var num11 = altitude > 60.0 ? -(altitude - 60.0) * 0.01 : (60.0 - altitude) * 0.5;
        //             var num12 = num3 * (1.0 - num2) + num11 * num2 - num4 * num8 * (1.0 - __instance.input_aff_1);
        //             var dvel = vectorLf3_4 * num12 + vectorLf3_7.normalized * num10 + vectorLf3_1 -
        //                        __instance.player.uVelocity;
        //             __instance.UseSailEnergy(ref dvel, 1.5);
        //             if (!disableController && !guideRunning)
        //                 __instance.player.uVelocity += dvel;
        //             var num13 = num7 * (1.0 - __instance.input_aff_2);
        //             var num14 = magnitude2 * num13;
        //             if (num14 > 1.0 / 1000.0)
        //             {
        //                 var force = -universalGravity * (num14 / magnitude2);
        //                 __instance.UseSailEnergy(ref force, dt, 1.0);
        //                 if (!guideRunning)
        //                     __instance.uForce += force;
        //             }
        //         }
        //         else if (localStar != null && !__instance.player.warping)
        //         {
        //             var vectorLf3_3 = __instance.player.uPosition - localStar.uPosition;
        //             var magnitude3 = vectorLf3_3.magnitude;
        //             var vectorLf3_4 = vectorLf3_3 / magnitude3;
        //             var num2 = magnitude3 - localStar.viewRadius;
        //             if (num2 < 1000.0)
        //             {
        //                 num9 = Maths.Clamp01((1000.0 - num2) / 500.0);
        //                 var uVelocity = __instance.player.uVelocity;
        //                 var num3 = num9 * num9 * 0.08 * (1.0 - __instance.input_aff_1) * (1.0 - __instance.input_aff_1);
        //                 var num4 = uVelocity.x * vectorLf3_4.x + uVelocity.y * vectorLf3_4.y +
        //                            uVelocity.z * vectorLf3_4.z;
        //                 var vectorLf3_5 = vectorLf3_4 * num4;
        //                 var vectorLf3_6 = uVelocity - vectorLf3_5;
        //                 var magnitude4 = vectorLf3_6.magnitude;
        //                 var num5 = magnitude4 * magnitude4 / magnitude3 * dt;
        //                 var num10 = Math.Sqrt(magnitude2 * magnitude3 * 2.0);
        //                 var num11 = magnitude4 * (1.0 - num3) + num10 * num3;
        //                 var num12 = num2 > 300.0 ? -(num2 - 300.0) * 0.01 : (300.0 - num2) * 0.1;
        //                 var num13 = num4 * (1.0 - num3) + num12 * num3 - num5 * num9 * (1.0 - __instance.input_aff_1);
        //                 var dvel = vectorLf3_4 * num13 + vectorLf3_6.normalized * num11 - __instance.player.uVelocity;
        //                 __instance.UseSailEnergy(ref dvel, 1.5);
        //                 if (!disableController && !guideRunning)
        //                     __instance.player.uVelocity += dvel;
        //                 var num14 = num9 * (1.0 - __instance.input_aff_2);
        //                 var num15 = magnitude2 * num14;
        //                 if (num15 > 1.0 / 1000.0)
        //                 {
        //                     var force = -universalGravity * (num15 / magnitude2);
        //                     __instance.UseSailEnergy(ref force, dt, 1.0);
        //                     if (!guideRunning)
        //                         __instance.uForce += force;
        //                 }
        //             }
        //         }
        //
        //         if (!disableController)
        //             __instance.player.uVelocity += __instance.uForce * dt;
        //         if (localPlanet != null && num8 > 0.0 && !__instance.player.warping)
        //         {
        //             var vectorLf3_3 = (__instance.galaxy.astroPoses[localPlanet.id].uPosNext -
        //                                __instance.galaxy.astroPoses[localPlanet.id].uPos) / dt;
        //             var vectorLf3_4 = __instance.player.uPosition - uPosition;
        //             var vectorLf3_5 = vectorLf3_4 + (__instance.player.uVelocity - vectorLf3_3) * dt;
        //             var normalized = vectorLf3_4.normalized;
        //             vectorLf3_5 = vectorLf3_5.normalized;
        //             var quaternion = Quaternion.Slerp(Quaternion.identity,
        //                 Quaternion.FromToRotation(normalized, vectorLf3_5), (float) num8);
        //             if (!disableController)
        //             {
        //                 __instance.sailPoser.targetURot = quaternion * __instance.sailPoser.targetURot;
        //                 __instance.sailPoser.targetURotWanted = quaternion * __instance.sailPoser.targetURotWanted;
        //             }
        //         }
        //         else if (localStar != null && !__instance.player.warping && num9 > 0.0)
        //         {
        //             var vectorLf3_3 = __instance.player.uPosition - localStar.uPosition;
        //             var vectorLf3_4 = vectorLf3_3 + __instance.player.uVelocity * dt;
        //             vectorLf3_3 = vectorLf3_3.normalized;
        //             vectorLf3_4 = vectorLf3_4.normalized;
        //             var quaternion = Quaternion.Slerp(Quaternion.identity,
        //                 Quaternion.FromToRotation(vectorLf3_3, vectorLf3_4), (float) num9);
        //             if (!disableController)
        //             {
        //                 __instance.sailPoser.targetURot = quaternion * __instance.sailPoser.targetURot;
        //                 __instance.sailPoser.targetURotWanted = quaternion * __instance.sailPoser.targetURotWanted;
        //             }
        //         }
        //
        //         __instance.visual_uvel = __instance.player.uVelocity - vectorLf3_2;
        //         var magnitude5 = __instance.visual_uvel.magnitude;
        //         if (localPlanet != null && !__instance.player.warping && !guideRunning)
        //         {
        //             var num2 = Mathf.Abs(__instance.controller.vertSpeed);
        //             if (localPlanet.type != EPlanetType.Gas)
        //             {
        //                 if (altitude < 46.0 && magnitude5 < 75.0 && num2 < 7.0 ||
        //                     altitude < 25.0 && magnitude5 < 85.0 && num2 < 18.0 || altitude < 7.0)
        //                 {
        //                     __instance.controller.movementStateInFrame = EMovementState.Fly;
        //                     __instance.controller.actionFly.targetAltitude = (float) altitude;
        //                     GameCamera.instance.SyncForSailMode();
        //                     if (!guideRunning)
        //                         __instance.controller.velocityOnLanding = Maths.QInvRotate(q, __instance.visual_uvel);
        //                     else
        //                         __instance.controller.velocityOnLanding = Vector3.zero;
        //                     Debug.DrawRay(__instance.player.position, __instance.controller.velocityOnLanding * 0.3f,
        //                         Color.yellow, 20f);
        //                 }
        //             }
        //             else
        //             {
        //                 __instance.SoftLimit(localPlanet.uPosition, localPlanet.realRadius,
        //                     localPlanet.realRadius + 48.0, 1f, ref __instance.player.uPosition,
        //                     ref __instance.player.uVelocity);
        //                 if (altitude < 48.0)
        //                 {
        //                     __instance.controller.movementStateInFrame = EMovementState.Fly;
        //                     __instance.controller.actionFly.targetAltitude = Mathf.Max(20f, (float) altitude);
        //                     GameCamera.instance.SyncForSailMode();
        //                     if (!guideRunning)
        //                         __instance.controller.velocityOnLanding = Maths.QInvRotate(q, __instance.visual_uvel);
        //                     else
        //                         __instance.controller.velocityOnLanding = Vector3.zero;
        //                     Debug.DrawRay(__instance.player.position, __instance.controller.velocityOnLanding * 0.3f,
        //                         Color.yellow, 20f);
        //                 }
        //             }
        //         }
        //         else if (localStar != null)
        //         {
        //             __instance.SoftLimit(localStar.uPosition, localStar.viewRadius + 10.0, localStar.viewRadius + 80.0,
        //                 1f, ref __instance.player.uPosition, ref __instance.player.uVelocity);
        //             if (guideRunning)
        //                 for (var index = 0; index < localStar.planetCount; ++index)
        //                 {
        //                     var planet = localStar.planets[index];
        //                     if (planet.id != GameMain.galaxy.birthPlanetId)
        //                         __instance.SoftLimit(planet.uPosition, planet.realRadius + 40.0,
        //                             planet.realRadius + 400.0, 0.025f, ref __instance.player.uPosition,
        //                             ref __instance.player.uVelocity);
        //                 }
        //         }
        //
        //         if (!disableController)
        //         {
        //             __instance.player.uPosition += __instance.player.uVelocity * dt;
        //             if (__instance.player.warping)
        //                 __instance.player.uPosition += __instance.currentWarpVelocity * dt;
        //         }
        //
        //         __instance.controller.gameData.DetermineRelative();
        //         if (__instance.player.uPosition.x < -480000000.0)
        //             __instance.player.uPosition.x = -480000000.0;
        //         else if (__instance.player.uPosition.x > 480000000.0)
        //             __instance.player.uPosition.x = 480000000.0;
        //         if (__instance.player.uPosition.y < -240000000.0)
        //             __instance.player.uPosition.y = -240000000.0;
        //         else if (__instance.player.uPosition.y > 240000000.0)
        //             __instance.player.uPosition.y = 240000000.0;
        //         if (__instance.player.uPosition.z < -480000000.0)
        //             __instance.player.uPosition.z = -480000000.0;
        //         else if (__instance.player.uPosition.z > 480000000.0)
        //             __instance.player.uPosition.z = 480000000.0;
        //         var vector3_2 = Vector3.zero;
        //         if (localPlanet != null)
        //             vector3_2 = Vector3.ClampMagnitude(Maths.QInvRotate(q, __instance.visual_uvel) * 0.8f, 110f);
        //         __instance.controller.actionWalk.rtsVelocity = Vector3.zero;
        //         __instance.controller.actionWalk.moveVelocity = vector3_2;
        //         __instance.controller.actionDrift.rtsVelocity = Vector3.zero;
        //         __instance.controller.actionDrift.moveVelocity = vector3_2;
        //         __instance.controller.actionFly.rtsVelocity = Vector3.zero;
        //         __instance.controller.actionFly.moveVelocity = vector3_2;
        //     }
        //     else
        //     {
        //         __instance.sailCounter = 0;
        //     }
        //
        //     return false;
        // }
    }
}