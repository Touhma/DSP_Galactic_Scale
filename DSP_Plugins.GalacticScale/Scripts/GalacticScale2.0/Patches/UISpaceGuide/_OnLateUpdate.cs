using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale {
    public partial class PatchOnUISpaceGuide {

        ////Strategy: Replace ldc.i4.s 10 instructions with a dynamic addition equal to the current system's planet count
        //// Get the local system:
        ///* 0x000E0746 02           */// IL_034A: ldarg.0
        ///* 0x000E0747 7B0E190004   */// IL_034B: ldfld class GameData UISpaceGuide::gameData
        ///* 0x000E074C 6F4C090006   */// IL_0350: callvirt instance class StarData GameData::get_localStar()
        //// Get the planet count
        ///* 0x000E0751 6F970A0006   */// IL_0355: ldfld instance int StarData::planetCount
        ////
        ////
        //[HarmonyTranspiler, HarmonyPatch(typeof(UISpaceGuide), "_OnLateUpdate")]
        //public static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) => ReplaceLd10(instructions);
        [HarmonyPrefix, HarmonyPatch(typeof(UISpaceGuide), "_OnLateUpdate")]
        public static bool _OnLateUpdate(ref UISpaceGuide __instance)
        {
            bool sailing = __instance.player.sailing;
            int _guidecnt = 0;
            bool flag = __instance.gameData.guideComplete || __instance.gameData.guideMission.elapseTime > 36f;
            if (!VFInput.inFullscreenGUI && flag)
            {
                int pId0 = (__instance.gameData.localStar == null) ? 0 : (__instance.gameData.localStar.id * 100);
                __instance.relPos = __instance.gameData.relativePos;
                __instance.relRot = __instance.gameData.relativeRot;
                Vector3 position = __instance.gameCamera.transform.position;
                VectorLF3 camUPos = __instance.relPos + Maths.QRotateLF(__instance.relRot, position);
                VectorLF3 vectorLf3_ = Maths.QRotateLF(__instance.relRot, __instance.gameCamera.ScreenPointToRay(Input.mousePosition).direction);
                bool flag2 = VFInput.onGUI || VFInput.onGUIOperate;
                int num = 0;
                for (int index = 1; index <= __instance.galaxy.starCount; index++)
                {
                    StarData starData = __instance.galaxy.StarById(index);
                    if (starData != null)
                    {
                        Vector3 _rpos = Vector3.zero;
                        bool flag3 = false;
                        if (__instance.uiGame.dfSpaceGuideOn)
                        {
                            if (__instance.gameData.localStar == starData)
                            {
                                flag3 = true;
                            }
                            if (!flag3 && (__instance.history.GetStarPin(index) == EPin.Show || (starData.uPosition - __instance.player.uPosition).sqrMagnitude < 92160000000000.0))
                            {
                                bool flag4 = false;
                                int planetId = index * 100;
                                while (planetId <= index * 100 + 99 && __instance.astroPoses[planetId].uRadius >= 0f)//Edited here
                                {
                                    if (__instance.history.GetPlanetPin(planetId) == EPin.Show && __instance.astroPoses[planetId].uRadius > 1f)//Edited here
                                    {
                                        flag4 = true;
                                        break;
                                    }
                                    planetId++;
                                }
                                flag3 = !flag4;
                            }
                            if (flag3 && __instance.history.GetStarPin(index) == EPin.Hide)
                            {
                                flag3 = false;
                            }
                        }
                        if (!flag3 && !flag2)
                        {
                            VectorLF3 vectorLf3_2 = starData.uPosition - camUPos;
                            double num2 = vectorLf3_.x * vectorLf3_2.x + vectorLf3_.y * vectorLf3_2.y + vectorLf3_.z * vectorLf3_2.z;
                            if (num2 > 0.0)
                            {
                                num2 /= vectorLf3_2.magnitude;
                                if (num2 > 0.99994)
                                {
                                    flag3 = true;
                                    num = index;
                                }
                            }
                        }
                        if (__instance.mouseInStar == index)
                        {
                            flag3 = true;
                        }
                        if (flag3)
                        {
                            _rpos = Maths.QInvRotateLF(__instance.relRot, starData.uPosition - __instance.relPos);
                        }
                        if (flag3)
                        {
                            flag3 = __instance.CheckVisible(pId0, index * 100, starData.uPosition, camUPos);
                        }
                        if (flag3)
                        {
                            __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Star, index, 0, _rpos, starData.viewRadius - 120f);
                        }
                    }
                }
                if (__instance.gameData.localStar != null)
                {
                    int index2 = pId0 + 1;
                    while (index2 <= (pId0 + 99) && __instance.astroPoses[index2].uRadius > 0f) //1f to 0f
                    {
                        //GS2.Warn((index2).ToString() + " " + __instance.astroPoses[index2].uRadius.ToString() + " " + GameMain.galaxy.PlanetById(index2).name);
                        if (__instance.astroPoses[index2].uRadius > 1f) //added conditional
                        {
                            Vector3 _rpos2 = Vector3.zero;
                            bool flag5 = __instance.uiGame.dfSpaceGuideOn;
                            if (__instance.uiGame.dfSpaceGuideOn && !flag5 && __instance.history.GetPlanetPin(index2) == EPin.Show)
                            {
                                flag5 = true;
                            }
                            if (flag5 && __instance.history.GetPlanetPin(index2) == EPin.Hide)
                            {
                                flag5 = false;
                            }
                            if (!flag5 && !flag2)
                            {
                                VectorLF3 vectorLf3_3 = __instance.astroPoses[index2].uPos - camUPos;
                                double num3 = vectorLf3_.x * vectorLf3_3.x + vectorLf3_.y * vectorLf3_3.y + vectorLf3_.z * vectorLf3_3.z;
                                if (num3 > 0.0)
                                {
                                    num3 /= vectorLf3_3.magnitude;
                                    if (num3 > 0.9999)
                                    {
                                        flag5 = true;
                                    }
                                }
                            }
                            if (__instance.mouseInPlanet == index2)
                            {
                                flag5 = true;
                            }
                            if (flag5)
                            {
                                _rpos2 = Maths.QInvRotateLF(__instance.relRot, __instance.astroPoses[index2].uPos - __instance.relPos);
                                if ((double)_rpos2.magnitude > (double)(__instance.gameData.localStar.systemRadius * 6f) * 40000.0)
                                {
                                    flag5 = false;
                                }
                                if (!sailing && __instance.gameData.localPlanet != null && __instance.gameData.localPlanet.id == index2)
                                {
                                    flag5 = false;
                                }
                            }
                            if (flag5)
                            {
                                flag5 = __instance.CheckVisible(pId0, index2, __instance.astroPoses[index2].uPos, camUPos);
                            }
                            if (flag5)
                            {
                                __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Planet, index2, 0, _rpos2, __instance.astroPoses[index2].uRadius);
                            }
                        }
                        index2++;
                    }
                }
                if (__instance.uiGame.dfSpaceGuideOn)
                {
                    int num4 = 0;
                    foreach (KeyValuePair<int, int> pinnedPlanet in __instance.history.pinnedPlanets)
                    {
                        int key = pinnedPlanet.Key;
                        if (key >= num4)
                        {
                            PlanetData planetData = __instance.galaxy.PlanetById(key);
                            if (planetData != null && planetData.star != __instance.gameData.localStar)
                            {
                                StarData star = planetData.star;
                                if (__instance.gameData.localStar != star && __instance.mouseInStar != star.id && num != star.id)
                                {
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
                if (__instance.uiGame.dfSpaceGuideOn && __instance.shipRenderer != null)
                {
                    int num5 = 0;
                    int num6 = __instance.shipRenderer.shipCount;
                    if (num6 > 1024)
                    {
                        num6 = 1024;
                    }
                    ShipRenderingData[] shipsArr = __instance.shipRenderer.shipsArr;
                    float num7 = float.MaxValue;
                    for (int index3 = 0; index3 < num6; index3++)
                    {
                        if (shipsArr[index3].anim.z > 0.95f)
                        {
                            __instance.shipDistArray[index3] = shipsArr[index3].pos.sqrMagnitude;
                            if (__instance.shipDistArray[index3] < 160000f || __instance.shipDistArray[index3] > 4E+10f)
                            {
                                __instance.shipDistArray[index3] = 0f;
                            }
                            else
                            {
                                num5 = index3 + 1;
                                if (__instance.shipDistArray[index3] < num7)
                                {
                                    num7 = __instance.shipDistArray[index3];
                                }
                            }
                        }
                        else
                        {
                            __instance.shipDistArray[index3] = 0f;
                        }
                    }
                    int num8 = 4;
                    num7 *= 2.25f;
                    for (int index4 = 0; index4 < num5; index4++)
                    {
                        if (__instance.shipDistArray[index4] >= 160000f && __instance.shipDistArray[index4] < num7)
                        {
                            VectorLF3 upos = __instance.relPos + Maths.QRotateLF(__instance.relRot, shipsArr[index4].pos);
                            if (__instance.CheckVisible(pId0, 0, upos, camUPos))
                            {
                                __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Ship, 0, (int)shipsArr[index4].itemId, shipsArr[index4].pos, 3f);
                                if (--num8 == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            __instance.ClipEntryPool(_guidecnt, false);
            for (int index5 = 0; index5 < __instance.entryOpenedCount; index5++)
            {
                __instance.entryPool[index5]._LateUpdate();
            }
            return false;
        }
    }
}