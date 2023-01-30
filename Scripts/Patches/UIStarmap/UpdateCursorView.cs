using System;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GalacticScale
{
    public partial class PatchOnUIStarmap
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarmap), "UpdateCursorView")]
        public static bool UpdateCursorView(UIStarmap __instance)
        {
            var active = false;
            var uistarmapPlanet = __instance.mouseHoverPlanet;
            var uistarmapStar = __instance.mouseHoverStar;
            if (__instance.focusPlanet != null)
            {
                uistarmapPlanet = __instance.focusPlanet;
                uistarmapStar = null;
                __instance.cursorFunctionGroup.SetActive(true);
                bool active2 = GameMain.sandboxToolsEnabled && GameMain.mainPlayer.planetId != __instance.focusPlanet.planet.id && !GameMain.mainPlayer.warping;
                __instance.fastTravelButton.gameObject.SetActive(active2);
                var planetPin = GameMain.history.GetPlanetPin(__instance.focusPlanet.planet.id);
                __instance.cursorFunctionIcon1.localEulerAngles = new Vector3(0f, 0f, planetPin == EPin.Show ? -90 : planetPin == EPin.Hide ? 90 : 0);
                __instance.cursorFunctionText1.text = (planetPin == EPin.Show ? "天体显示标签" : planetPin == EPin.Hide ? "天体隐藏标签" : "天体自动标签").Translate();
                __instance.fastTravelButton.button.interactable = !__instance.fastTravelling;
            }
            else if (__instance.focusStar != null)
            {
                uistarmapPlanet = null;
                uistarmapStar = __instance.focusStar;
                __instance.cursorFunctionGroup.SetActive(true);
                __instance.fastTravelButton.gameObject.SetActive(false);
                var starPin = GameMain.history.GetStarPin(__instance.focusStar.star.id);
                __instance.cursorFunctionIcon1.localEulerAngles = new Vector3(0f, 0f, starPin == EPin.Show ? -90 : starPin == EPin.Hide ? 90 : 0);
                __instance.cursorFunctionText1.text = (starPin == EPin.Show ? "天体显示标签" : starPin == EPin.Hide ? "天体隐藏标签" : "天体自动标签").Translate();
            }
            else
            {
                __instance.cursorFunctionGroup.SetActive(false);
                __instance.fastTravelButton.gameObject.SetActive(false);
            }

            if (uistarmapPlanet != null) uistarmapStar = null;
            if (uistarmapPlanet != null && uistarmapPlanet.projected)
            {
                active = true;
                __instance.cursorViewTrans.anchoredPosition = uistarmapPlanet.projectedCoord;
                if ((Object)__instance.cursorViewDisplayObject != uistarmapPlanet || __instance.forceUpdateCursorView)
                {
                    var str = "";
                    var planet = uistarmapPlanet.planet;
                    var arg = "";
                    if (planet.singularity > EPlanetSingularity.None || planet.orbitAround > 0) arg = "<color=\"#FD965EC0\">" + planet.singularityString + "</color>";
                    var text = string.Format("行星类型".Translate() + "\r\n", planet.typeString, arg);
                    if (!GS2.Vanilla && GS2.GetGSPlanet(planet).GsTheme.DisplayName == "Comet") text = "Comet\r\n";
                    if (uistarmapPlanet == __instance.focusPlanet) text = "<color=\"#FFFFFFB0\">" + __instance.focusPlanet.planet.displayName + "</color>\r\n" + text;
                    var mainPlayer = GameMain.mainPlayer;
                    var num = (planet.uPosition - mainPlayer.uPosition).magnitude - planet.realRadius - 50.0;
                    string str2;
                    if (num < 50.0)
                        str2 = string.Format(planet.type != EPlanetType.Gas ? "已登陆".Translate() : "已靠近".Translate(), Array.Empty<object>());
                    else if (num < 5000.0)
                        str2 = string.Format("距离米".Translate(), num);
                    else if (num < 2400000.0)
                        str2 = string.Format("距离日距".Translate(), num / 40000.0);
                    else
                        str2 = string.Format("距离光年".Translate(), num / 2400000.0);
                    var num2 = 0.0001;
                    if (mainPlayer.mecha.thrusterLevel >= 2) num2 = mainPlayer.mecha.maxSailSpeed;
                    if (mainPlayer.mecha.thrusterLevel >= 3 && uistarmapPlanet.planet.star == GameMain.localStar) num2 += mainPlayer.mecha.maxWarpSpeed * 0.03;
                    if (mainPlayer.mecha.thrusterLevel >= 3 && uistarmapPlanet.planet.star != GameMain.localStar) num2 += mainPlayer.mecha.maxWarpSpeed * 0.98;
                    var num3 = (float)(num / num2);
                    if (mainPlayer.planetId != 0)
                    {
                        var num4 = mainPlayer.position.magnitude - mainPlayer.planetData.realRadius;
                        num3 += Mathf.Clamp01((800f - num4) / 800f) * 10f;
                    }

                    if (mainPlayer.mecha.thrusterLevel >= 3 && uistarmapPlanet.planet.star != GameMain.localStar) num3 += 24f;
                    var num5 = (int)num3 / 3600;
                    var num6 = (int)num3 / 60 % 60;
                    var num7 = (int)num3 % 60;
                    if (num > 900.0)
                    {
                        if (num3 < 60f)
                            str = string.Format("最快秒".Translate(), num7);
                        else if (num3 < 600f)
                            str = string.Format("最快分秒".Translate(), num6, num7);
                        else if (num3 < 3600f)
                            str = string.Format("最快分钟".Translate(), num6);
                        else if (num3 < 720000f)
                            str = string.Format("最快小时".Translate(), num5, num6);
                        else if (num < 2400000.0)
                            str = string.Format("需要驱动引擎".Translate(), 2);
                        else
                            str = string.Format("需要驱动引擎".Translate(), 4);
                    }

                    __instance.cursorViewText.text = text + str2 + str;
                    __instance.cursorViewTrans.sizeDelta = new Vector2(__instance.cursorViewText.preferredWidth * 0.5f + 44f, __instance.cursorViewText.preferredHeight * 0.5f + 14f);
                    __instance.cursorRightDeco.sizeDelta = new Vector2(__instance.cursorViewTrans.sizeDelta.y - 12f, 5f);
                    __instance.cursorViewDisplayObject = uistarmapPlanet;
                }
            }
            else if (uistarmapStar != null && uistarmapStar.projected)
            {
                active = true;
                __instance.cursorViewTrans.anchoredPosition = uistarmapStar.projectedCoord;
                if ((Object)__instance.cursorViewDisplayObject != uistarmapStar || __instance.forceUpdateCursorView)
                {
                    var str3 = "";
                    var star = uistarmapStar.star;
                    var text2 = star.typeString + "\r\n";
                    if (uistarmapStar == __instance.focusStar) text2 = "<color=\"#FFFFFFB0\">" + __instance.focusStar.star.displayName + "</color>\r\n" + text2;
                    var mainPlayer2 = GameMain.mainPlayer;
                    var num8 = (star.uPosition - mainPlayer2.uPosition).magnitude - star.physicsRadius - 100.0;
                    string str4;
                    if (num8 < 50.0)
                        str4 = string.Format("已靠近".Translate(), Array.Empty<object>());
                    else if (num8 < 5000.0)
                        str4 = string.Format("距离米".Translate(), num8);
                    else if (num8 < 2400000.0)
                        str4 = string.Format("距离日距".Translate(), num8 / 40000.0);
                    else
                        str4 = string.Format("距离光年".Translate(), num8 / 2400000.0);
                    var num9 = 0.0001;
                    if (mainPlayer2.mecha.thrusterLevel >= 2) num9 = mainPlayer2.mecha.maxSailSpeed;
                    if (mainPlayer2.mecha.thrusterLevel >= 3 && uistarmapStar.star == GameMain.localStar) num9 += mainPlayer2.mecha.maxWarpSpeed * 0.03;
                    if (mainPlayer2.mecha.thrusterLevel >= 3 && uistarmapStar.star != GameMain.localStar) num9 += mainPlayer2.mecha.maxWarpSpeed * 0.98;
                    var num10 = (float)(num8 / num9);
                    if (mainPlayer2.planetId != 0)
                    {
                        var num11 = mainPlayer2.position.magnitude - mainPlayer2.planetData.realRadius;
                        num10 += Mathf.Clamp01((800f - num11) / 800f) * 9f;
                    }

                    if (mainPlayer2.mecha.thrusterLevel >= 3 && uistarmapStar.star != GameMain.localStar) num10 += 20f;
                    var num12 = (int)num10 / 3600;
                    var num13 = (int)num10 / 60 % 60;
                    var num14 = (int)num10 % 60;
                    if (num8 > 5000.0)
                    {
                        if (num10 < 60f)
                            str3 = string.Format("最快秒".Translate(), num14);
                        else if (num10 < 600f)
                            str3 = string.Format("最快分秒".Translate(), num13, num14);
                        else if (num10 < 3600f)
                            str3 = string.Format("最快分钟".Translate(), num13);
                        else if (num10 < 720000f)
                            str3 = string.Format("需要驱动引擎4".Translate(), num12, num13);
                        else if (num8 < 2400000.0)
                            str3 = string.Format("需要驱动引擎".Translate(), 2);
                        else
                            str3 = string.Format("需要驱动引擎".Translate(), 4);
                    }

                    __instance.cursorViewText.text = text2 + str4 + str3;
                    __instance.cursorViewTrans.sizeDelta = new Vector2(__instance.cursorViewText.preferredWidth * 0.5f + 44f, __instance.cursorViewText.preferredHeight * 0.5f + 14f);
                    __instance.cursorRightDeco.sizeDelta = new Vector2(__instance.cursorViewTrans.sizeDelta.y - 12f, 5f);
                    __instance.cursorViewDisplayObject = uistarmapStar;
                }
            }

            __instance.forceUpdateCursorView = false;
            __instance.cursorViewTrans.gameObject.SetActive(active);
            return false;
        }
    }
}