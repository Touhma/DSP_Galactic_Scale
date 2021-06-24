using HarmonyLib;
using UnityEngine;

namespace GalacticScale
    
{
    public class PatchOnWhatever
    {
       
        //[HarmonyPrefix, HarmonyPatch(typeof(PlayerController), "GameTick")]
        //public static bool GameTick(ref PlayerController __instance, long time)
        //{
        //    if (NebulaCompatibility.IsMasterClient) return true;
        //    GS2.Warn("Start GameTick");
        //    __instance.UpdateEnvironment();
        //    GS2.Warn("Updated Environment");
        //    __instance.SetCommandStateHeader();
        //    GS2.Warn("SetCommandStateHeader");
        //    __instance.UpdateCommandState();
        //    GS2.Warn("UpdatedCommandState");

        //    __instance.GetInput();
        //    GS2.Warn("GotInput");

        //    __instance.HandleBaseInput();
        //    GS2.Warn("HandledBaseInput");

        //    __instance.ClearForce();
        //    GS2.Warn("Cleared Force");

        //    __instance.ApplyGravity();
        //    GS2.Warn("AppliedGravity");

        //    __instance.rigidbodySleep = false;
        //    __instance.movementStateInFrame = __instance.player.movementState;
        //    __instance.velocityOnLanding = Vector3.zero;
        //    GS2.Warn("Running PlayerAction GameTicks");

        //    foreach (PlayerAction playerAction in __instance.actions)
        //    {
        //        playerAction.GameTick(time);
        //    }
        //    GS2.Warn("Applying Local Force");

        //    __instance.ApplyLocalForce();
        //    GS2.Warn("Updating Rotation");

        //    __instance.UpdateRotation();
        //    GS2.Warn("RigidbodySafer");

        //    __instance.RigidbodySafer();
        //    GS2.Warn("Updating PhysicsDirect");

        //    __instance.UpdatePhysicsDirect();
        //    GS2.Warn("Updating Tracker");

        //    __instance.UpdateTracker();
        //    GS2.Warn("Setting MovementState");

        //    __instance.player.movementState = __instance.movementStateInFrame;
        //    if (__instance.velocityOnLanding.sqrMagnitude > 0f)
        //    {
        //        __instance.velocity = __instance.velocityOnLanding;
        //    }
        //    GS2.Warn("Done");

        //    if (DSPGame.IsMenuDemo) return true;

        //return false;
        //}

        //    [HarmonyPostfix, HarmonyPatch(typeof(PlayerController), "RigidbodySafer")]
        //    public static void Postfix()
        //    {
        //        GS2.Warn("RigidbodySafer Postfix");
        //    }




        //		//[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "ComputeMaxReformCount")]
        //		//public static bool ComputeMaxReformCount() {
        //		//    GS2.Warn("."); return true;
        //		//}
        //		//[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "DetermineLongitudeSegmentCount")]
        //		//public static bool DetermineLongitudeSegmentCount() {
        //		//    GS2.Warn("."); return true;
        //		//}
        //		//[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "FreeReformData")]
        //		//public static bool FreeReformData() {
        //		//    GS2.Warn("."); return true;
        //		//}
        //		//[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "GetReformIndex")]
        //		//public static bool GetReformIndex(ref int __result, ref PlatformSystem __instance, int x, int y) {
        //		//    __result = __instance.reformOffsets[y] + x;
        //		//    GS2.Warn($"{__result}: x:{x}, y:{y}");
        //		//    return false;
        //		//}

        //		//[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "GetReformIndexForPosition")]
        //		//public static bool GetReformIndexForPosition(ref int __result, ref PlatformSystem __instance, Vector3 pos) {
        //		//    GS2.Warn($"{pos}");
        //		//    pos.Normalize();
        //		//    float num = Mathf.Asin(pos.y);
        //		//    float num2 = Mathf.Atan2(pos.x, -pos.z);
        //		//    float num3 = num / 6.2831855f * (float)__instance.segment;
        //		//    float num4 = (float)PlatformSystem.DetermineLongitudeSegmentCount(Mathf.FloorToInt(Mathf.Abs(num3)), __instance.segment);
        //		//    float num5 = num2 / 6.2831855f * num4;
        //		//    float num6 = Mathf.Round(num3 * 10f);
        //		//    float num7 = Mathf.Round(num5 * 10f);
        //		//    float num8 = Mathf.Abs(num6);
        //		//    float num9 = Mathf.Abs(num7);
        //		//    if (num8 % 2f != 1f) {
        //		//        num3 = Mathf.Abs(num3);
        //		//        num8 = (float)Mathf.FloorToInt(num3 * 10f);
        //		//        if (num8 % 2f != 1f) {
        //		//            num8 += 1f;
        //		//        }
        //		//    }
        //		//    num8 = ((num6 >= 0f) ? num8 : (-num8));
        //		//    if (num9 % 2f != 1f) {
        //		//        num5 = Mathf.Abs(num5);
        //		//        num9 = (float)Mathf.FloorToInt(num5 * 10f);
        //		//        if (num9 % 2f != 1f) {
        //		//            num9 += 1f;
        //		//        }
        //		//    }
        //		//    num9 = ((num7 >= 0f) ? num9 : (-num9));
        //		//    num8 /= 10f;
        //		//    num9 /= 10f;
        //		//    float num10 = (float)(__instance.latitudeCount / 10);
        //		//    if (num8 >= num10 || num8 <= -num10) {
        //		//        __result = -1; GS2.Warn(__result.ToString()); return false;
        //		//    }
        //		//    __result = __instance.GetReformIndexForSegment(num8, num9); GS2.Warn(__result.ToString()); return false;
        //		//}
        //		//[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "GetReformIndexForSegment")]
        //		//public static bool GetReformIndexForSegment(ref int __result, ref PlatformSystem __instance, float _latitudeSeg, float _longitudeSeg) {
        //		//    int LatitudeSegment = (_latitudeSeg > 0f) ? Mathf.CeilToInt(_latitudeSeg * 5f) : Mathf.FloorToInt(_latitudeSeg * 5f);
        //		//    int LongitudeSegment = (_longitudeSeg > 0f) ? Mathf.CeilToInt(_longitudeSeg * 5f) : Mathf.FloorToInt(_longitudeSeg * 5f);
        //		//    int HalfLatitudeCount = __instance.latitudeCount / 2;
        //		//    int y = (LatitudeSegment > 0) ? (LatitudeSegment - 1) : (HalfLatitudeCount - LatitudeSegment - 1);
        //		//    int LongSegmentCount = PlatformSystem.DetermineLongitudeSegmentCount(Mathf.FloorToInt(Mathf.Abs(_latitudeSeg)), __instance.segment);
        //		//    //GS2.Warn($"LongSegmentCount:{LongSegmentCount}, LongitudeSegment:{LongitudeSegment}, segment:{__instance.segment} {GS2.GetCaller(1)}");
        //		//    if (LongitudeSegment > LongSegmentCount * 5 / 2) {
        //		//        LongitudeSegment = LongitudeSegment - LongSegmentCount * 5 - 1;
        //		//    }
        //		//    if (LongitudeSegment < -LongSegmentCount * 5 / 2) {
        //		//        LongitudeSegment = LongSegmentCount * 5 + LongitudeSegment + 1;
        //		//    }
        //		//    int x = (LongitudeSegment > 0) ? (LongitudeSegment - 1) : (LongSegmentCount * 5 / 2 - LongitudeSegment - 1);
        //		//    __result = __instance.GetReformIndex(x, y);
        //		//    //GS2.Warn($"LongSegmentCount:{LongSegmentCount}, LongitudeSegment:{LongitudeSegment}, segment:{__instance.segment} _longitudeSeg:{_longitudeSeg} x:{x}");
        //		//    //GS2.Warn($"x:{x}, y:{y}, _latitudeSeg:{_latitudeSeg}, _longitudeSeg:{_longitudeSeg}. Result of DLSC:{LongSegmentCount}");
        //		//    return false;
        //		//}
        //		////[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "GetReformType")]
        //		////public static bool GetReformType() {
        //		////    GS2.Warn("."); return true;
        //		////}
        //		////[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "InitReformData")]
        //		////public static bool InitReformData() {
        //		////    GS2.Warn("."); return true;
        //		////}
        //		////[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "IsTerrainMapping")]
        //		////public static bool IsTerrainMapping() {
        //		////    GS2.Warn("."); return true;
        //		////}
        //		////[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "IsTerrainReformed")]
        //		////public static bool IsTerrainReformed() {
        //		////    GS2.Warn("."); return true;
        //		////}
        //		//static bool output = false;
        //		//[HarmonyPrefix, HarmonyPatch(typeof(PlanetGrid), "ReformSnapTo")]

        //		//public static bool ReformSnapTo(ref int __result, ref PlanetGrid __instance, Vector3 pos, int reformSize, int reformType, int reformColor, Vector3[] reformPoints, int[] reformIndices, PlatformSystem platform, out Vector3 reformCenter) {
        //		//    pos.Normalize();
        //		//    float AsinY = Mathf.Asin(pos.y);
        //		//    float AtanXZ = Mathf.Atan2(pos.x, -pos.z);
        //		//    float latitude = AsinY / 6.2831855f * (float)__instance.segment;
        //		//    int latitudeIndex = Mathf.FloorToInt(Mathf.Abs(latitude));
        //		//    int LSC = PlanetGrid.DetermineLongitudeSegmentCount(latitudeIndex, __instance.segment);
        //		//    float fLSC = (float)LSC;
        //		//    float longitude = AtanXZ / 6.2831855f * fLSC;
        //		//    if (VFInput.control && !output) {
        //		//        GS2.Warn($"Latitude:{latitude}:{latitudeIndex}, Longitude:{longitude}, LSC:{LSC}, Segment:{__instance.segment}, AsinY:{AsinY}, AtanXZ:{AtanXZ} Pos:{pos}");
        //		//        output = true;
        //		//    }
        //		//    if (!VFInput.control) output = false;
        //		//        float LatitudeX10 = Mathf.Round(latitude * 10f);
        //		//    float LongitudeX10 = Mathf.Round(longitude * 10f);
        //		//    float num9 = Mathf.Abs(LatitudeX10);
        //		//    float absLongitudeX10 = Mathf.Abs(LongitudeX10);
        //		//    int reformSizeMod2 = reformSize % 2;
        //		//    if (num9 % 2f != (float)reformSizeMod2) {
        //		//        latitude = Mathf.Abs(latitude);
        //		//        num9 = (float)Mathf.FloorToInt(latitude * 10f);
        //		//        if (num9 % 2f != (float)reformSizeMod2) {
        //		//            num9 += 1f;
        //		//        }
        //		//    }
        //		//    num9 = (LatitudeX10 < 0f) ? (-num9) : num9;
        //		//    if (absLongitudeX10 % 2f != (float)reformSizeMod2) {
        //		//        longitude = Mathf.Abs(longitude);
        //		//        absLongitudeX10 = (float)Mathf.FloorToInt(longitude * 10f);
        //		//        if (absLongitudeX10 % 2f != (float)reformSizeMod2) {
        //		//            absLongitudeX10 += 1f;
        //		//        }
        //		//    }
        //		//    absLongitudeX10 = ((LongitudeX10 < 0f) ? (-absLongitudeX10) : absLongitudeX10);
        //		//    AsinY = num9 / 10f / (float)__instance.segment * 6.2831855f;
        //		//    AtanXZ = absLongitudeX10 / 10f / fLSC * 6.2831855f;
        //		//    float y = Mathf.Sin(AsinY);
        //		//    float num12 = Mathf.Cos(AsinY);
        //		//    float num13 = Mathf.Sin(AtanXZ);
        //		//    float num14 = Mathf.Cos(AtanXZ);
        //		//    reformCenter = new Vector3(num12 * num13, y, num12 * -num14);
        //		//    int num15 = 1 - reformSize;
        //		//    int num16 = 1 - reformSize;
        //		//    int num17 = 0;
        //		//    int num18 = 0;
        //		//    float num19 = (float)(platform.latitudeCount / 10);
        //		//    for (int i = 0; i < reformSize * reformSize; i++) {
        //		//        num18++;
        //		//        latitude = (num9 + (float)num15) / 10f;
        //		//        longitude = (absLongitudeX10 + (float)num16) / 10f;
        //		//        num16 += 2;
        //		//        if (num18 % reformSize == 0) {
        //		//            num16 = 1 - reformSize;
        //		//            num15 += 2;
        //		//        }
        //		//        if (latitude >= num19 || latitude <= -num19) {
        //		//            reformIndices[i] = -1;
        //		//        } else {
        //		//            latitudeIndex = Mathf.FloorToInt(Mathf.Abs(latitude));
        //		//            if (LSC != PlanetGrid.DetermineLongitudeSegmentCount(latitudeIndex, __instance.segment)) {
        //		//                reformIndices[i] = -1;
        //		//            } else {
        //		//                int reformIndexForSegment = platform.GetReformIndexForSegment(latitude, longitude);
        //		//                reformIndices[i] = reformIndexForSegment;
        //		//                int reformType2 = platform.GetReformType(reformIndexForSegment);
        //		//                int reformColor2 = platform.GetReformColor(reformIndexForSegment);
        //		//                if (!platform.IsTerrainReformed(reformType2) && (reformType2 != reformType || reformColor2 != reformColor)) {
        //		//                    AsinY = latitude / (float)__instance.segment * 6.2831855f;
        //		//                    AtanXZ = longitude / fLSC * 6.2831855f;
        //		//                    y = Mathf.Sin(AsinY);
        //		//                    num12 = Mathf.Cos(AsinY);
        //		//                    num13 = Mathf.Sin(AtanXZ);
        //		//                    num14 = Mathf.Cos(AtanXZ);
        //		//                    reformPoints[num17] = new Vector3(num12 * num13, y, num12 * -num14);
        //		//                    num17++;
        //		//                }
        //		//            }
        //		//        }
        //		//    }
        //		//    __result = num17;
        //		//    return false;
        //		//}




    }
}