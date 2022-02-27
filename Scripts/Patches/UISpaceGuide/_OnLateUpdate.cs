using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUISpaceGuide
    {
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
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISpaceGuide), "_OnLateUpdate")]
        public static bool _OnLateUpdate(ref UISpaceGuide __instance)
        {
            var sailing = __instance.player.sailing;
            var _guidecnt = 0;
            var flag = __instance.gameData.guideComplete || __instance.gameData.guideMission.elapseTime > 36f;
            if (!VFInput.inFullscreenGUI && flag)
            {
                var pId0 = __instance.gameData.localStar == null ? 0 : __instance.gameData.localStar.id * 100;
                __instance.relPos = __instance.gameData.relativePos;
                __instance.relRot = __instance.gameData.relativeRot;
                var position = __instance.gameCamera.transform.position;
                var camUPos = __instance.relPos + Maths.QRotateLF(__instance.relRot, position);
                var directionOfView = Maths.QRotateLF(__instance.relRot, __instance.gameCamera.ScreenPointToRay(Input.mousePosition).direction);
                var flag2 = VFInput.onGUI || VFInput.onGUIOperate;
                var num = 0;
                for (var index = 1; index <= __instance.galaxy.starCount; index++) //For each star in the galaxy
                {
                    var starData = __instance.galaxy.StarById(index);
                    if (starData != null) //If there is a star
                    {
                        var _rpos = Vector3.zero;
                        var showStarLabel = false;
                        if (__instance.uiGame.dfSpaceGuideOn)
                        {
                            if (__instance.gameData.localStar == starData) showStarLabel = true;
                            if (!showStarLabel && (__instance.history.GetStarPin(index) == EPin.Show || (starData.uPosition - __instance.player.uPosition).sqrMagnitude < 92160000000000.0)) //If the current star isnt local, and it's pinned, or its closer than 9216....
                            {
                                var starContainsPinnedPlanet = false;
                                var planetId = index * 100;
                                while (planetId <= index * 100 + 99) //Edited here
                                {
                                    if (__instance.astroPoses[planetId].uRadius <= 1f)
                                    {
                                        planetId++;
                                        continue;
                                    }

                                    if (__instance.history.GetPlanetPin(planetId) == EPin.Show) //Edited here
                                    {
                                        starContainsPinnedPlanet = true;
                                        break;
                                    }

                                    planetId++;
                                }

                                showStarLabel = !starContainsPinnedPlanet;
                            }

                            if (showStarLabel && __instance.history.GetStarPin(index) == EPin.Hide) showStarLabel = false; //If the star is hidden
                        }

                        if (!showStarLabel && !flag2) //if we dont want to show the star, and we are not ?in a star gui?
                        {
                            var starRelativePosition = starData.uPosition - camUPos;
                            var num2 = directionOfView.x * starRelativePosition.x + directionOfView.y * starRelativePosition.y + directionOfView.z * starRelativePosition.z;
                            if (num2 > 0.0)
                            {
                                num2 /= starRelativePosition.magnitude;
                                if (num2 > 0.99994)
                                {
                                    showStarLabel = true;
                                    num = index;
                                }
                            }
                        }

                        if (__instance.mouseInStar == index) showStarLabel = true;
                        if (showStarLabel)
                            _rpos = Maths.QInvRotateLF(__instance.relRot, starData.uPosition - __instance.relPos);
                        if (showStarLabel) showStarLabel = __instance.CheckVisible(pId0, index * 100, starData.uPosition, camUPos);
                        if (showStarLabel)
                            __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Star, index, 0, _rpos, starData.viewRadius - 120f);
                    }
                }

                if (__instance.gameData.localStar != null)
                {
                    var planetID = pId0 + 1;
                    while (planetID <= pId0 + 99 && __instance.astroPoses[planetID].uRadius > 0f) //1f to 0f
                    {
                        //GS2.Warn((index2).ToString() + " " + __instance.astroPoses[index2].uRadius.ToString() + " " + GameMain.galaxy.PlanetById(index2).name);
                        if (__instance.astroPoses[planetID].uRadius > 1f) //added conditional
                        {
                            var _rpos2 = Vector3.zero;
                            var showPlanetLabel = __instance.uiGame.dfSpaceGuideOn;
                            if (__instance.uiGame.dfSpaceGuideOn && !showPlanetLabel && __instance.history.GetPlanetPin(planetID) == EPin.Show) showPlanetLabel = true;
                            if (showPlanetLabel && __instance.history.GetPlanetPin(planetID) == EPin.Hide) showPlanetLabel = false;
                            if (!showPlanetLabel && !flag2)
                            {
                                var vectorLf3_3 = __instance.astroPoses[planetID].uPos - camUPos;
                                var num3 = directionOfView.x * vectorLf3_3.x + directionOfView.y * vectorLf3_3.y + directionOfView.z * vectorLf3_3.z;
                                if (num3 > 0.0)
                                {
                                    num3 /= vectorLf3_3.magnitude;
                                    if (num3 > 0.9999) showPlanetLabel = true;
                                }
                            }

                            if (__instance.mouseInPlanet == planetID) showPlanetLabel = true;
                            if (showPlanetLabel)
                            {
                                _rpos2 = Maths.QInvRotateLF(__instance.relRot, __instance.astroPoses[planetID].uPos - __instance.relPos);
                                if (_rpos2.magnitude > __instance.gameData.localStar.systemRadius * 6f * 40000.0)
                                    showPlanetLabel = false;
                                if (!sailing && __instance.gameData.localPlanet != null && __instance.gameData.localPlanet.id == planetID) showPlanetLabel = false;
                            }

                            if (showPlanetLabel)
                                showPlanetLabel = __instance.CheckVisible(pId0, planetID, __instance.astroPoses[planetID].uPos, camUPos);
                            if (showPlanetLabel)
                                __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Planet, planetID, 0, _rpos2, __instance.astroPoses[planetID].uRadius);
                        }

                        planetID++;
                    }
                }

                if (__instance.uiGame.dfSpaceGuideOn)
                {
                    var num4 = 0;
                    foreach (var pinnedPlanet in __instance.history.pinnedPlanets)
                    {
                        var key = pinnedPlanet.Key;
                        if (key >= num4)
                        {
                            var planetData = __instance.galaxy.PlanetById(key);
                            if (planetData != null && planetData.star != __instance.gameData.localStar)
                            {
                                var star = planetData.star;
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
                    var num5 = 0;
                    var num6 = __instance.shipRenderer.shipCount;
                    if (num6 > 1024) num6 = 1024;
                    var shipsArr = __instance.shipRenderer.shipsArr;
                    var num7 = float.MaxValue;
                    for (var index3 = 0; index3 < num6; index3++)
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
                                if (__instance.shipDistArray[index3] < num7) num7 = __instance.shipDistArray[index3];
                            }
                        }
                        else
                        {
                            __instance.shipDistArray[index3] = 0f;
                        }

                    var num8 = 4;
                    num7 *= 2.25f;
                    for (var index4 = 0; index4 < num5; index4++)
                        if (__instance.shipDistArray[index4] >= 160000f && __instance.shipDistArray[index4] < num7)
                        {
                            var upos = __instance.relPos + Maths.QRotateLF(__instance.relRot, shipsArr[index4].pos);
                            if (__instance.CheckVisible(pId0, 0, upos, camUPos))
                            {
                                __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Ship, 0, (int)shipsArr[index4].itemId, shipsArr[index4].pos, 3f);
                                if (--num8 == 0) break;
                            }
                        }
                }
            }

            __instance.ClipEntryPool(_guidecnt);
            for (var index5 = 0; index5 < __instance.entryOpenedCount; index5++)
                __instance.entryPool[index5]._LateUpdate();
            return false;
        }
    }
}