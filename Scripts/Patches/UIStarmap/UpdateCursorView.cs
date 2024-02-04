using System;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GalacticScale
{
    public partial class PatchOnUIStarmap
    {
//         [HarmonyPrefix]
//         [HarmonyPatch(typeof(UIStarmap), "UpdateCursorView")]
//         public static bool UpdateCursorView(UIStarmap __instance)
//         {
//             Player mainPlayer = GameMain.mainPlayer; // 0.10
//             var active = false;
//             var uistarmapPlanet = __instance.mouseHoverPlanet;
//             var uistarmapStar = __instance.mouseHoverStar;
//             UIStarmapDFHive uIStarmapDFHive = __instance.mouseHoverHive;
//             int tinderEnemyId = __instance.mouseHoverTinderEnemyId;
//             if (__instance.focusPlanet != null)
//             {
//                 uistarmapPlanet = __instance.focusPlanet;
//                 uistarmapStar = null;
//                 uIStarmapDFHive = null;
//                 tinderEnemyId = 0;
//                 __instance.cursorFunctionGroup.SetActive(true);
//                 bool active2 = GameMain.sandboxToolsEnabled && GameMain.mainPlayer.planetId != __instance.focusPlanet.planet.id && !GameMain.mainPlayer.warping;
//                 __instance.fastTravelButton.gameObject.SetActive(active2);
//                 var planetPin = GameMain.history.GetPlanetPin(__instance.focusPlanet.planet.id);
//                 RectTransform rectTransform = __instance.cursorFunctionIcon1;
//                 int num2;
//                 switch (planetPin)
//                 {
//                     default:
//                         num2 = 0;
//                         break;
//                     case EPin.Hide:
//                         num2 = 90;
//                         break;
//                     case EPin.Show:
//                         num2 = -90;
//                         break;
//                 }
//                 
//                 __instance.cursorFunctionIcon1.localEulerAngles = new Vector3(0f, 0f, num2);
//                 // __instance.cursorFunctionText1.text = (planetPin == EPin.Show ? "天体显示标签" : planetPin == EPin.Hide ? "天体隐藏标签" : "天体自动标签").Translate();
//                 object s;
//                 switch (planetPin)
//                 {
//                     default:
//                         s = "天体自动标签";
//                         break;
//                     case EPin.Hide:
//                         s = "天体隐藏标签";
//                         break;
//                     case EPin.Show:
//                         s = "天体显示标签";
//                         break;
//                 }
//                 __instance.cursorFunctionText1.text = ((string)s).Translate();
//                 __instance.fastTravelButton.button.interactable = !mainPlayer.fastTravelling && mainPlayer.isAlive;
//             }
//             else if (__instance.focusStar != null)
//             {
//                 uistarmapPlanet = null;
//                 uIStarmapDFHive = null;
//                 uistarmapStar = __instance.focusStar;
//                 __instance.cursorFunctionGroup.SetActive(true);
//                 __instance.fastTravelButton.gameObject.SetActive(false);
//                 var starPin = GameMain.history.GetStarPin(__instance.focusStar.star.id);
//                 RectTransform rectTransform2 = __instance.cursorFunctionIcon1;
//                 int num3;
//                 switch (starPin)
//                 {
//                     default:
//                         num3 = 0;
//                         break;
//                     case EPin.Hide:
//                         num3 = 90;
//                         break;
//                     case EPin.Show:
//                         num3 = -90;
//                         break;
//                 }
//                 __instance.cursorFunctionIcon1.localEulerAngles = new Vector3(0f, 0f, num3);
//                 object s2;
//                 switch (starPin)
//                 {
//                     default:
//                         s2 = "天体自动标签";
//                         break;
//                     case EPin.Hide:
//                         s2 = "天体隐藏标签";
//                         break;
//                     case EPin.Show:
//                         s2 = "天体显示标签";
//                         break;
//                 }
//                 __instance.cursorFunctionText1.text = ((string)s2).Translate();
//                 // __instance.cursorFunctionText1.text = (starPin == EPin.Show ? "天体显示标签" : starPin == EPin.Hide ? "天体隐藏标签" : "天体自动标签").Translate();
//             }
//             else if (__instance.focusHive != null)
//             {
//                 if (!__instance.focusHive.active)
//                 {
//                     PlayerNavigation navigation = GameMain.mainPlayer.navigation;
//                     if (navigation != null && navigation.indicatorAstroId == __instance.focusHive.hive.hiveAstroId)
//                     {
//                         navigation.indicatorAstroId = 0;
//                     }
//                     GameMain.history.SetHivePin(__instance.focusHive.hive.hiveAstroId - 1000000, EPin.Auto);
//                     __instance.focusHive = null;
//                 }
//                 uIStarmapDFHive = __instance.focusHive;
//                 uistarmapPlanet = null;
//                 uistarmapStar = null;
//                 tinderEnemyId = 0;
//                 __instance.cursorFunctionGroup.SetActive((__instance.focusHive != null) ? true : false);
//                 if (__instance.focusHive != null)
//                 {
//                     EPin hivePin = GameMain.history.GetHivePin(__instance.focusHive.hive.hiveAstroId - 1000000);
//                     RectTransform rectTransform3 = __instance.cursorFunctionIcon1;
//                     int num4;
//                     switch (hivePin)
//                     {
//                         default:
//                             num4 = 0;
//                             break;
//                         case EPin.Hide:
//                             num4 = 90;
//                             break;
//                         case EPin.Show:
//                             num4 = -90;
//                             break;
//                     }
//                     rectTransform3.localEulerAngles = new Vector3(0f, 0f, num4);
//                     var text3 = __instance.cursorFunctionText1;
//                     object s3;
//                     switch (hivePin)
//                     {
//                         default:
//                             s3 = "天体自动标签";
//                             break;
//                         case EPin.Hide:
//                             s3 = "天体隐藏标签";
//                             break;
//                         case EPin.Show:
//                             s3 = "天体显示标签";
//                             break;
//                     }
//                     text3.text = ((string)s3).Translate();
//                 }
//                 __instance.fastTravelButton.gameObject.SetActive(value: false);
//             }
//             else if (__instance.focusTinderEnemyId != 0)
//             {
//                 if (__instance.focusTinderEnemyId < 0 || __instance.focusTinderEnemyId >= __instance.spaceSector.enemyPool.Length)
//                 {
//                     __instance.focusTinderEnemyId = 0;
//                 }
//                 else if (__instance.spaceSector.enemyPool[__instance.focusTinderEnemyId].id == 0 || __instance.spaceSector.enemyPool[__instance.focusTinderEnemyId].dfTinderId == 0)
//                 {
//                     __instance.focusTinderEnemyId = 0;
//                 }
//                 uIStarmapDFHive = null;
//                 uistarmapPlanet = null;
//                 uistarmapStar = null;
//                 tinderEnemyId = __instance.focusTinderEnemyId;
//                 __instance.cursorFunctionGroup.SetActive(value: false);
//                 __instance.fastTravelButton.gameObject.SetActive(value: false);
//             }
//             else
//             {
//                 __instance.cursorFunctionGroup.SetActive(false);
//                 __instance.fastTravelButton.gameObject.SetActive(false);
//             }
//
//             if (uistarmapPlanet != null) uistarmapStar = null;
//             if (uistarmapPlanet != null && uistarmapPlanet.projected)
//             {
//                 active = true;
//                 __instance.cursorViewTrans.anchoredPosition = uistarmapPlanet.projectedCoord;
//                 if (__instance.cursorViewDisplayObject != uistarmapPlanet || __instance.forceUpdateCursorView)
//                 {
//                     var str = "";
//                     var planet = uistarmapPlanet.planet;
//                     var arg = "";
//                     if (planet.singularity > EPlanetSingularity.None || planet.orbitAround > 0) arg = "<color=\"#FD965EC0\">" + planet.singularityString + "</color>";
//                     var text = string.Format("行星类型".Translate() + "\r\n", planet.typeString, arg);
//                     if (!GS2.Vanilla && GS2.GetGSPlanet(planet).GsTheme.DisplayName == "Comet") text = "Comet\r\n";
//                     if (uistarmapPlanet == __instance.focusPlanet) text = "<color=\"#FFFFFFB0\">" + __instance.focusPlanet.planet.displayName + "</color>\r\n" + text;
//                     var num = (planet.uPosition - mainPlayer.uPosition).magnitude - planet.realRadius - 50.0;
//                     string str2;
//                     if (num < 50.0)
//                         str2 = string.Format(planet.type != EPlanetType.Gas ? "已登陆".Translate() : "已靠近".Translate(), Array.Empty<object>());
//                     else if (num < 5000.0)
//                         str2 = string.Format("距离米".Translate(), num);
//                     else if (num < 2400000.0)
//                         str2 = string.Format("距离日距".Translate(), num / 40000.0);
//                     else
//                         str2 = string.Format("距离光年".Translate(), num / 2400000.0);
//                     var num2 = 0.0001;
//                     if (mainPlayer.mecha.thrusterLevel >= 2) num2 = mainPlayer.mecha.maxSailSpeed;
//                     if (mainPlayer.mecha.thrusterLevel >= 3 && uistarmapPlanet.planet.star == GameMain.localStar) num2 += mainPlayer.mecha.maxWarpSpeed * 0.03;
//                     if (mainPlayer.mecha.thrusterLevel >= 3 && uistarmapPlanet.planet.star != GameMain.localStar) num2 += mainPlayer.mecha.maxWarpSpeed * 0.98;
//                     var num3 = (float)(num / num2);
//                     if (mainPlayer.planetId != 0)
//                     {
//                         var num4 = mainPlayer.position.magnitude - mainPlayer.planetData.realRadius;
//                         num3 += Mathf.Clamp01((800f - num4) / 800f) * 10f;
//                     }
//
//                     if (mainPlayer.mecha.thrusterLevel >= 3 && uistarmapPlanet.planet.star != GameMain.localStar) num3 += 24f;
//                     var num5 = (int)num3 / 3600;
//                     var num6 = (int)num3 / 60 % 60;
//                     var num7 = (int)num3 % 60;
//                     if (num > 900.0)
//                     {
//                         if (num3 < 60f)
//                             str = string.Format("最快秒".Translate(), num7);
//                         else if (num3 < 600f)
//                             str = string.Format("最快分秒".Translate(), num6, num7);
//                         else if (num3 < 3600f)
//                             str = string.Format("最快分钟".Translate(), num6);
//                         else if (num3 < 720000f)
//                             str = string.Format("最快小时".Translate(), num5, num6);
//                         else if (num < 2400000.0)
//                             str = string.Format("需要驱动引擎".Translate(), 2);
//                         else
//                             str = string.Format("需要驱动引擎".Translate(), 4);
//                     }
//
//                     __instance.cursorViewText.text = text + str2 + str;
//                     __instance.cursorViewTrans.sizeDelta = new Vector2(__instance.cursorViewText.preferredWidth * 0.5f + 44f, __instance.cursorViewText.preferredHeight * 0.5f + 14f);
//                     __instance.cursorRightDeco.sizeDelta = new Vector2(__instance.cursorViewTrans.sizeDelta.y - 12f, 5f);
//                     __instance.cursorViewDisplayObject = uistarmapPlanet;
//                 }
//             }
//             else if (uistarmapStar != null && uistarmapStar.projected)
//             {
//                 active = true;
//                 __instance.cursorViewTrans.anchoredPosition = uistarmapStar.projectedCoord;
//                 if (__instance.cursorViewDisplayObject != uistarmapStar || __instance.forceUpdateCursorView)
//                 {
//                     var str3 = "";
//                     var star = uistarmapStar.star;
//                     var text2 = star.typeString + "\r\n";
//                     if (uistarmapStar == __instance.focusStar) text2 = "<color=\"#FFFFFFB0\">" + __instance.focusStar.star.displayName + "</color>\r\n" + text2;
//                     var mainPlayer2 = GameMain.mainPlayer;
//                     var num8 = (star.uPosition - mainPlayer2.uPosition).magnitude - star.physicsRadius - 100.0;
//                     string str4;
//                     if (num8 < 50.0)
//                         str4 = string.Format("已靠近".Translate(), Array.Empty<object>());
//                     else if (num8 < 5000.0)
//                         str4 = string.Format("距离米".Translate(), num8);
//                     else if (num8 < 2400000.0)
//                         str4 = string.Format("距离日距".Translate(), num8 / 40000.0);
//                     else
//                         str4 = string.Format("距离光年".Translate(), num8 / 2400000.0);
//                     var num9 = 0.0001;
//                     if (mainPlayer2.mecha.thrusterLevel >= 2) num9 = mainPlayer2.mecha.maxSailSpeed;
//                     if (mainPlayer2.mecha.thrusterLevel >= 3 && uistarmapStar.star == GameMain.localStar) num9 += mainPlayer2.mecha.maxWarpSpeed * 0.03;
//                     if (mainPlayer2.mecha.thrusterLevel >= 3 && uistarmapStar.star != GameMain.localStar) num9 += mainPlayer2.mecha.maxWarpSpeed * 0.98;
//                     var num10 = (float)(num8 / num9);
//                     if (mainPlayer2.planetId != 0)
//                     {
//                         var num11 = mainPlayer2.position.magnitude - mainPlayer2.planetData.realRadius;
//                         num10 += Mathf.Clamp01((800f - num11) / 800f) * 9f;
//                     }
//
//                     if (mainPlayer2.mecha.thrusterLevel >= 3 && uistarmapStar.star != GameMain.localStar) num10 += 20f;
//                     var num12 = (int)num10 / 3600;
//                     var num13 = (int)num10 / 60 % 60;
//                     var num14 = (int)num10 % 60;
//                     if (num8 > 5000.0)
//                     {
//                         if (num10 < 60f)
//                             str3 = string.Format("最快秒".Translate(), num14);
//                         else if (num10 < 600f)
//                             str3 = string.Format("最快分秒".Translate(), num13, num14);
//                         else if (num10 < 3600f)
//                             str3 = string.Format("最快分钟".Translate(), num13);
//                         else if (num10 < 720000f)
//                             str3 = string.Format("需要驱动引擎4".Translate(), num12, num13);
//                         else if (num8 < 2400000.0)
//                             str3 = string.Format("需要驱动引擎".Translate(), 2);
//                         else
//                             str3 = string.Format("需要驱动引擎".Translate(), 4);
//                     }
//
//                     __instance.cursorViewText.text = text2 + str4 + str3;
//                     __instance.cursorViewTrans.sizeDelta = new Vector2(__instance.cursorViewText.preferredWidth * 0.5f + 44f, __instance.cursorViewText.preferredHeight * 0.5f + 14f);
//                     __instance.cursorRightDeco.sizeDelta = new Vector2(__instance.cursorViewTrans.sizeDelta.y - 12f, 5f);
//                     __instance.cursorViewDisplayObject = uistarmapStar;
//                 }
//             }
// else if (uIStarmapDFHive != null && uIStarmapDFHive.projected)
// 		{
// 			active = true;
//             __instance.cursorViewTrans.anchoredPosition = uIStarmapDFHive.projectedCoord;
// 			if (__instance.cursorViewDisplayObject != uIStarmapDFHive || __instance.forceUpdateCursorView)
// 			{
// 				string text10 = "";
// 				string text11 = "";
// 				string text12 = "";
// 				EnemyDFHiveSystem hive = uIStarmapDFHive.hive;
// 				text10 = uIStarmapDFHive.nameText.text + "\r\n";
// 				Player mainPlayer4 = GameMain.mainPlayer;
// 				double num19 = (hive.sector.astros[hive.hiveAstroId - 1000000].uPos - mainPlayer4.uPosition).magnitude - (double)hive.sector.astros[hive.hiveAstroId - 1000000].uRadius;
// 				text11 = ((num19 < 50.0) ? string.Format("已靠近".Translate()) : ((num19 < 5000.0) ? string.Format("距离米".Translate(), num19) : ((!(num19 < 2400000.0)) ? string.Format("距离光年".Translate(), num19 / 2400000.0) : string.Format("距离日距".Translate(), num19 / 40000.0))));
// 				double num20 = 0.0001;
// 				if (mainPlayer4.mecha.thrusterLevel >= 2)
// 				{
// 					num20 = mainPlayer4.mecha.maxSailSpeed;
// 				}
// 				if (mainPlayer4.mecha.thrusterLevel >= 3 && uIStarmapDFHive.hive.starData == GameMain.localStar)
// 				{
// 					num20 += (double)mainPlayer4.mecha.maxWarpSpeed * 0.03;
// 				}
// 				if (mainPlayer4.mecha.thrusterLevel >= 3 && uIStarmapDFHive.hive.starData != GameMain.localStar)
// 				{
// 					num20 += (double)mainPlayer4.mecha.maxWarpSpeed * 0.98;
// 				}
// 				float num21 = (float)(num19 / num20);
// 				if (mainPlayer4.planetId != 0)
// 				{
// 					float num22 = mainPlayer4.position.magnitude - mainPlayer4.planetData.realRadius;
// 					num21 += Mathf.Clamp01((800f - num22) / 800f) * 9f;
// 				}
// 				if (mainPlayer4.mecha.thrusterLevel >= 3 && uIStarmapDFHive.hive.starData != GameMain.localStar)
// 				{
// 					num21 += 20f;
// 				}
// 				int num23 = (int)num21 / 3600;
// 				int num24 = (int)num21 / 60 % 60;
// 				int num25 = (int)num21 % 60;
// 				if (num19 > 5000.0)
// 				{
// 					text12 = ((num21 < 60f) ? string.Format("最快秒".Translate(), num25) : ((num21 < 600f) ? string.Format("最快分秒".Translate(), num24, num25) : ((num21 < 3600f) ? string.Format("最快分钟".Translate(), num24) : ((num21 < 720000f) ? string.Format("需要驱动引擎4".Translate(), num23, num24) : ((!(num19 < 2400000.0)) ? string.Format("需要驱动引擎".Translate(), 4) : string.Format("需要驱动引擎".Translate(), 2))))));
// 				}
//                 __instance.cursorViewText.text = text10 + text11 + text12;
//                 __instance.cursorViewTrans.sizeDelta = new Vector2(__instance.cursorViewText.preferredWidth * 0.5f + 44f, __instance.cursorViewText.preferredHeight * 0.5f + 14f);
//                 __instance.cursorRightDeco.sizeDelta = new Vector2(__instance.cursorViewTrans.sizeDelta.y - 12f, 5f);
//                 __instance.cursorViewDisplayObject = uIStarmapDFHive;
// 			}
// 		}
// 		else if (tinderEnemyId != 0)
// 		{
// 			ref EnemyData reference = ref __instance.spaceSector.enemyPool[tinderEnemyId];
//             __instance.spaceSector.TransformFromAstro_ref(reference.astroId, out var upos, ref reference.pos);
// 			Vector3 worldPoint = (upos - __instance.viewTargetUPos) * 0.00025;
// 			if (!UIStarmap.isChangingToMilkyWay)
// 			{
// 				Vector2 rectPoint;
// 				bool num26 = __instance.WorldPointIntoScreen(worldPoint, out rectPoint);
// 				rectPoint = new Vector2(Mathf.Round(rectPoint.x), Mathf.Round(rectPoint.y));
// 				if (num26)
// 				{
//                     active = true;
//                     __instance.cursorViewTrans.anchoredPosition = rectPoint;
// 					if (__instance.cursorViewDisplayObject != (object)tinderEnemyId || __instance.forceUpdateCursorView)
// 					{
// 						string text13 = "";
// 						string text14 = "";
// 						string text15 = "";
// 						EnemyDFHiveSystem enemyDFHiveSystem = __instance.spaceSector.dfHivesByAstro[reference.originAstroId - 1000000];
// 						text13 = "火种".Translate() + "\r\n" + "火种来自".Translate() + " <color=\"#FFFFFFB0\">" + enemyDFHiveSystem.starData.displayName + "</color>\r\n";
// 						int targetHiveAstroId = enemyDFHiveSystem.tinders.buffer[reference.dfTinderId].targetHiveAstroId;
// 						if (targetHiveAstroId > 0)
// 						{
// 							text13 = text13 + "火种前往".Translate() + " <color=\"#FFFFFFB0\">" + __instance.spaceSector.dfHivesByAstro[targetHiveAstroId - 1000000].starData.displayName + "</color>\r\n";
// 						}
//                         double distance = 1.0;
//                         if (targetHiveAstroId > 0)
//                         {
//                             var targetAstro = __instance.spaceSector.dfHivesByAstro[targetHiveAstroId - 1000000];
//                             var targetStar = new StarData();
//                             if (targetAstro != null) targetStar = targetAstro.starData;
//                             var pos = new VectorLF3();
//                             pos = reference.pos;
//                             
//                             if (targetStar != null) distance = (targetStar.uPosition - pos).magnitude;
//                         }
//
//                         distance *= 1000;
//                         distance = Mathf.RoundToInt((float)distance);
//                         distance /= 1000;
//                         var tinder = enemyDFHiveSystem.tinders.buffer[reference.dfTinderId];
//                         var uSpeed = 0f;
//                         uSpeed = tinder.uSpeed;
// 						Player mainPlayer5 = GameMain.mainPlayer;
// 						double magnitude = (upos - mainPlayer5.uPosition).magnitude;
// 						text14 = ((magnitude < 50.0) ? string.Format("已靠近".Translate()) : ((magnitude < 5000.0) ? string.Format("距离米".Translate(), magnitude) : ((!(magnitude < 2400000.0)) ? string.Format("距离光年".Translate(), magnitude / 2400000.0) : string.Format("距离日距".Translate(), magnitude / 40000.0))));
// 						double num27 = 0.0001;
// 						if (mainPlayer5.mecha.thrusterLevel >= 2)
// 						{
// 							num27 = mainPlayer5.mecha.maxSailSpeed;
// 						}
// 						if (mainPlayer5.mecha.thrusterLevel >= 3)
// 						{
// 							num27 += (double)mainPlayer5.mecha.maxWarpSpeed * 0.98;
// 						}
// 						float num28 = (float)(magnitude / num27);
// 						if (mainPlayer5.planetId != 0)
// 						{
// 							float num29 = mainPlayer5.position.magnitude - mainPlayer5.planetData.realRadius;
// 							num28 += Mathf.Clamp01((800f - num29) / 800f) * 9f;
// 						}
// 						int num30 = (int)num28 / 3600;
// 						int num31 = (int)num28 / 60 % 60;
// 						int num32 = (int)num28 % 60;
// 						if (magnitude > 5000.0)
// 						{
// 							text15 = ((num28 < 60f) ? string.Format("最快秒".Translate(), num32) : ((num28 < 600f) ? string.Format("最快分秒".Translate(), num31, num32) : ((num28 < 3600f) ? string.Format("最快分钟".Translate(), num31) : ((num28 < 720000f) ? string.Format("需要驱动引擎4".Translate(), num30, num31) : ((!(magnitude < 2400000.0)) ? string.Format("需要驱动引擎".Translate(), 4) : string.Format("需要驱动引擎".Translate(), 2))))));
// 						}
// 						__instance.cursorViewText.text = text13 + text14 + text15 + "\r\n\r\n<color=\"#FFFFFFB0\">" + "双击方位指示".Translate() + "</color>"+ "\r\n<color=\"#00FFFFB0\">Speed:" + (uSpeed/40000) + "AU/s</color>"+ "\r\n<color=\"#FF0000B0\">"+"Distance Left".Translate()+": " + distance/40000 + "AU</color>";
//                         __instance.cursorViewTrans.sizeDelta = new Vector2(__instance.cursorViewText.preferredWidth * 0.5f + 44f, __instance.cursorViewText.preferredHeight * 0.5f + 14f);
//                         __instance.cursorRightDeco.sizeDelta = new Vector2(__instance.cursorViewTrans.sizeDelta.y - 12f, 5f);
//                         __instance.cursorViewDisplayObject = tinderEnemyId;
// 					}
// 				}
// 			}
// 		}
//             __instance.forceUpdateCursorView = false;
//             __instance.cursorViewTrans.gameObject.SetActive(active);
//             return false;
//         }
    }
}