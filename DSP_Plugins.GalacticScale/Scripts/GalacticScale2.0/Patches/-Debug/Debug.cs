using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale {
    public class PatchOnWhatever {

        [HarmonyPrefix, HarmonyPatch(typeof(UISpaceGuide), "_OnLateUpdate")]
        public static bool _OnLateUpdate(ref UISpaceGuide __instance) {
            bool sailing = __instance.player.sailing;
            int _guidecnt = 0;
            bool flag = __instance.gameData.guideComplete || __instance.gameData.guideMission.elapseTime > 36f;
            if (!VFInput.inFullscreenGUI && flag) {
                int pId0 = (__instance.gameData.localStar == null) ? 0 : (__instance.gameData.localStar.id * 100);
                __instance.relPos = __instance.gameData.relativePos;
                __instance.relRot = __instance.gameData.relativeRot;
                Vector3 position = __instance.gameCamera.transform.position;
                VectorLF3 camUPos = __instance.relPos + Maths.QRotateLF(__instance.relRot, position);
                VectorLF3 vectorLf3_ = Maths.QRotateLF(__instance.relRot, __instance.gameCamera.ScreenPointToRay(Input.mousePosition).direction);
                bool flag2 = VFInput.onGUI || VFInput.onGUIOperate;
                int num = 0;
                for (int index = 1; index <= __instance.galaxy.starCount; index++) {
                    StarData starData = __instance.galaxy.StarById(index);
                    if (starData != null) {
                        Vector3 _rpos = Vector3.zero;
                        bool flag3 = false;
                        if (__instance.uiGame.dfSpaceGuideOn) {
                            if (__instance.gameData.localStar == starData) {
                                flag3 = true;
                            }
                            if (!flag3 && (__instance.history.GetStarPin(index) == EPin.Show || (starData.uPosition - __instance.player.uPosition).sqrMagnitude < 92160000000000.0)) {
                                bool flag4 = false;
                                int planetId = index * 100;
                                while (planetId <= index * 100 + 10 && __instance.astroPoses[planetId].uRadius >= 1f) {
                                    if (__instance.history.GetPlanetPin(planetId) == EPin.Show) {
                                        flag4 = true;
                                        break;
                                    }
                                    planetId++;
                                }
                                flag3 = !flag4;
                            }
                            if (flag3 && __instance.history.GetStarPin(index) == EPin.Hide) {
                                flag3 = false;
                            }
                        }
                        if (!flag3 && !flag2) {
                            VectorLF3 vectorLf3_2 = starData.uPosition - camUPos;
                            double num2 = vectorLf3_.x * vectorLf3_2.x + vectorLf3_.y * vectorLf3_2.y + vectorLf3_.z * vectorLf3_2.z;
                            if (num2 > 0.0) {
                                num2 /= vectorLf3_2.magnitude;
                                if (num2 > 0.99994) {
                                    flag3 = true;
                                    num = index;
                                }
                            }
                        }
                        if (__instance.mouseInStar == index) {
                            flag3 = true;
                        }
                        if (flag3) {
                            _rpos = Maths.QInvRotateLF(__instance.relRot, starData.uPosition - __instance.relPos);
                        }
                        if (flag3) {
                            flag3 = __instance.CheckVisible(pId0, index * 100, starData.uPosition, camUPos);
                        }
                        if (flag3) {
                            __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Star, index, 0, _rpos, starData.viewRadius - 120f);
                        }
                    }
                }
                if (__instance.gameData.localStar != null) {
                    int index2 = pId0 + 1;
                    while (index2 <= pId0 + 10 && __instance.astroPoses[index2].uRadius >= 1f) {
                        Vector3 _rpos2 = Vector3.zero;
                        bool flag5 = __instance.uiGame.dfSpaceGuideOn;
                        if (__instance.uiGame.dfSpaceGuideOn && !flag5 && __instance.history.GetPlanetPin(index2) == EPin.Show) {
                            flag5 = true;
                        }
                        if (flag5 && __instance.history.GetPlanetPin(index2) == EPin.Hide) {
                            flag5 = false;
                        }
                        if (!flag5 && !flag2) {
                            VectorLF3 vectorLf3_3 = __instance.astroPoses[index2].uPos - camUPos;
                            double num3 = vectorLf3_.x * vectorLf3_3.x + vectorLf3_.y * vectorLf3_3.y + vectorLf3_.z * vectorLf3_3.z;
                            if (num3 > 0.0) {
                                num3 /= vectorLf3_3.magnitude;
                                if (num3 > 0.9999) {
                                    flag5 = true;
                                }
                            }
                        }
                        if (__instance.mouseInPlanet == index2) {
                            flag5 = true;
                        }
                        if (flag5) {
                            _rpos2 = Maths.QInvRotateLF(__instance.relRot, __instance.astroPoses[index2].uPos - __instance.relPos);
                            if ((double)_rpos2.magnitude > (double)(__instance.gameData.localStar.systemRadius * 6f) * 40000.0) {
                                flag5 = false;
                            }
                            if (!sailing && __instance.gameData.localPlanet != null && __instance.gameData.localPlanet.id == index2) {
                                flag5 = false;
                            }
                        }
                        if (flag5) {
                            flag5 = __instance.CheckVisible(pId0, index2, __instance.astroPoses[index2].uPos, camUPos);
                        }
                        if (flag5) {
                            __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Planet, index2, 0, _rpos2, __instance.astroPoses[index2].uRadius);
                        }
                        index2++;
                    }
                }
                if (__instance.uiGame.dfSpaceGuideOn) {
                    int num4 = 0;
                    foreach (KeyValuePair<int, int> pinnedPlanet in __instance.history.pinnedPlanets) {
                        int key = pinnedPlanet.Key;
                        if (key >= num4) {
                            PlanetData planetData = __instance.galaxy.PlanetById(key);
                            if (planetData != null && planetData.star != __instance.gameData.localStar) {
                                StarData star = planetData.star;
                                if (__instance.gameData.localStar != star && __instance.mouseInStar != star.id && num != star.id) {
                                    Vector3 _rpos3 = Maths.QInvRotateLF(__instance.relRot, __instance.astroPoses[key].uPos - __instance.relPos);
                                    __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Planet, key, 0, _rpos3, 0f);
                                    num4 = key + 100;
                                    num4 /= 100;
                                    num4 *= 100;
                                }
                            }
                        }
                    }
                }
                Array.Clear(__instance.shipDistArray, 0, 1024);
                if (__instance.uiGame.dfSpaceGuideOn && __instance.shipRenderer != null) {
                    int num5 = 0;
                    int num6 = __instance.shipRenderer.shipCount;
                    if (num6 > 1024) {
                        num6 = 1024;
                    }
                    ShipRenderingData[] shipsArr = __instance.shipRenderer.shipsArr;
                    float num7 = float.MaxValue;
                    for (int index3 = 0; index3 < num6; index3++) {
                        if (shipsArr[index3].anim.z > 0.95f) {
                            __instance.shipDistArray[index3] = shipsArr[index3].pos.sqrMagnitude;
                            if (__instance.shipDistArray[index3] < 160000f || __instance.shipDistArray[index3] > 4E+10f) {
                                __instance.shipDistArray[index3] = 0f;
                            } else {
                                num5 = index3 + 1;
                                if (__instance.shipDistArray[index3] < num7) {
                                    num7 = __instance.shipDistArray[index3];
                                }
                            }
                        } else {
                            __instance.shipDistArray[index3] = 0f;
                        }
                    }
                    int num8 = 4;
                    num7 *= 2.25f;
                    for (int index4 = 0; index4 < num5; index4++) {
                        if (__instance.shipDistArray[index4] >= 160000f && __instance.shipDistArray[index4] < num7) {
                            VectorLF3 upos = __instance.relPos + Maths.QRotateLF(__instance.relRot, shipsArr[index4].pos);
                            if (__instance.CheckVisible(pId0, 0, upos, camUPos)) {
                                __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Ship, 0, (int)shipsArr[index4].itemId, shipsArr[index4].pos, 3f);
                                if (--num8 == 0) {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            __instance.ClipEntryPool(_guidecnt, false);
            for (int index5 = 0; index5 < __instance.entryOpenedCount; index5++) {
                __instance.entryPool[index5]._LateUpdate();
            }
            return false;
        }

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