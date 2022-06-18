using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GalacticScale
{
	public partial class PatchOnUIPlanetDetail
	{
		private static int actualLevel = 5;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(UIPlanetDetail), "OnPlanetDataSet")]
		public static void OnPlanetDataSet(ref UIPlanetDetail __instance)
		{
			// GS2.Warn("Dataset");
			// GS2.Warn(__instance.obliquityValueText.transform.parent.transform.parent.childCount.ToString());
			// Add the planets radius to the Planet Detail UI
			// if (GS2.GetGSPlanet( __instance.planet).GsTheme.DisplayName == "Comet") __instance.
			if (__instance.obliquityValueText.transform.parent.transform.parent.childCount == 7)
			{
				GameObject radiusLabel;
				var obliquityLabel = __instance.obliquityValueText.transform.parent.gameObject;
				radiusLabel = Object.Instantiate(obliquityLabel, obliquityLabel.transform.parent.transform);
				var parentRect = obliquityLabel.transform.parent.transform.GetComponent<RectTransform>();
				parentRect.sizeDelta = new Vector2(0, 40);
				radiusLabel.transform.localPosition += Vector3.down * 20;
				var radiusLabelText = radiusLabel.GetComponent<Text>();
				radiusLabelText.GetComponent<Localizer>().enabled = false;
				var radiusIcon = radiusLabel.transform.GetChild(1).GetComponent<Image>();
				var uiButton = radiusLabel.transform.GetChild(1).GetComponent<UIButton>();
				uiButton.tips.tipText = "How large the planet is. Standard is 200".Translate();
				uiButton.tips.tipTitle = "Planet Radius".Translate();

				//GS2.LogJson(uiButton.button);
				if (uiButton.button == null) uiButton.button = uiButton.gameObject.AddComponent<Button>();

				uiButton.button.transform.SetParent(uiButton.transform);

				radiusIcon.sprite = Utils.GetSpriteAsset("ruler");
				var radiusValueText = radiusLabel.transform.GetChild(0).GetComponent<Text>();
				radiusLabelText.text = "Planetary Radius".Translate();
				radiusValueText.text = __instance.planet.realRadius.ToString();
			}

			if (__instance.obliquityValueText.transform.parent.transform.parent.childCount == 8)
			{
				var p = __instance.obliquityValueText.transform.parent.parent;
				var radiusLabel = p.GetChild(p.childCount - 1).gameObject;
				var radiusValueText = radiusLabel.transform.GetChild(0).GetComponent<Text>();
				if (__instance.planet != null) radiusValueText.text = __instance.planet.realRadius.ToString();
			}

			GameMain.history.universeObserveLevel = actualLevel;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(UIPlanetDetail), "OnPlanetDataSet")]
		public static bool OnPlanetDataSetPrefix(ref UIPlanetDetail __instance)
		{
			actualLevel = GameMain.history.universeObserveLevel;
			GameMain.history.universeObserveLevel = 4;
			return true;
		}

		// UIPlanetDetail
// Token: 0x06002584 RID: 9604 RVA: 0x001A5278 File Offset: 0x001A3478
		[HarmonyPrefix]
		[HarmonyPatch(typeof(UIPlanetDetail), "OnPlanetDataSet")]
		public static bool OnPlanetDataSet7Prefix(ref UIPlanetDetail __instance) //, ref UIResAmountEntry __instance.tipEntry,
			// ref Text __instance.nameText,
			// ref InputField __instance.nameInput, ref Text __instance.typeText, ref Text __instance.orbitRadiusValueText, ref Text __instance.orbitRadiusValueTextEx, ref Text __instance.orbitPeriodValueText, ref Text __instance.rotationPeriodValueText, ref Text __instance.inclinationValueText, ref Text __instance.obliquityValueText, ref Sprite __instance.unknownResIcon, ref Sprite __instance.sprite6, ref Sprite __instance.sprite8, ref Sprite __instance.sprite9)
		{
			{
				for (int i = 0; i < __instance.entries.Count; i++)
				{
					UIResAmountEntry uiresAmountEntry = __instance.entries[i];
					uiresAmountEntry.SetEmpty();
					__instance.pool.Add(uiresAmountEntry);
				}

				__instance.entries.Clear();
				__instance.tipEntry = null;
				if (__instance.veinAmounts == null)
				{
					__instance.veinAmounts = new long[64];
				}

				if (__instance.veinCounts == null)
				{
					__instance.veinCounts = new int[64];
				}

				Array.Clear(__instance.veinAmounts, 0, __instance.veinAmounts.Length);
				Array.Clear(__instance.veinCounts, 0, __instance.veinCounts.Length);
				__instance.calculated = false;
				if (__instance.planet != null)
				{
					if (!__instance.planet.calculated)
					{
						__instance.planet.RunCalculateThread();
					}

					__instance.calculated = __instance.planet.calculated;
					int num = (__instance.planet == GameMain.localPlanet) ? 1 : ((__instance.planet.star == GameMain.localStar) ? 2 : (((GameMain.mainPlayer.uPosition - __instance.planet.uPosition).magnitude < 14400000.0) ? 3 : 4));
					bool flag = GameMain.history.universeObserveLevel >= num;
					if (__instance.planet.factory != null && GameMain.history.universeObserveLevel >= 1)
					{
						flag = true;
					}

					if (__instance.calculated && flag)
					{
						__instance.planet.CalcVeinAmounts(ref __instance.veinAmounts);
						__instance.planet.CalcVeinCounts(ref __instance.veinCounts);
					}

					if (!__instance.nameInput.isFocused)
					{
						__instance.nameInput.text = __instance.planet.displayName;
					}

					string arg = "<color=\"#FD965EC0\">" + __instance.planet.singularityString + "</color>";
					__instance.typeText.text = string.Format("{0} {1}", __instance.planet.typeString, arg);
					__instance.orbitRadiusValueText.text = __instance.planet.orbitRadius.ToString("0.00#") + " AU";
					__instance.orbitRadiusValueTextEx.text = ((__instance.planet.orbitAround == 0) ? "环绕恒星".Translate() : ("环绕空格".Translate() + __instance.planet.orbitAround + "号星".Translate())); //removed roman number reference that didn't go up to 100.
					__instance.orbitPeriodValueText.text = __instance.planet.orbitalPeriod.ToString("#,##0") + "空格秒".Translate();
					__instance.rotationPeriodValueText.text = __instance.planet.rotationPeriod.ToString("#,##0") + "空格秒".Translate();
					float num2 = __instance.planet.orbitInclination;
					float num3 = 180f - __instance.planet.orbitLongitude;
					if (num2 < 0f)
					{
						num2 = -num2;
						num3 = 180f + num3;
					}

					float num4 = Mathf.Abs(num2);
					int num5 = (int)num4;
					int num6 = (int)((num4 - (float)num5) * 60f);
					float num7 = Mathf.Abs(__instance.planet.obliquity);
					int num8 = (int)num7;
					int num9 = (int)((num7 - (float)num8) * 60f);
					if (__instance.planet.obliquity < 0f)
					{
						num8 = -num8;
					}

					float num10 = Mathf.Repeat(num3, 360f);
					int num11 = (int)num10;
					int num12 = (int)((num10 - (float)num11) * 60f);
					__instance.inclinationValueText.text = string.Format("{0}° {1}′", num5, num6);
					__instance.obliquityValueText.text = string.Format("{0}° {1}′", num8, num9);
					__instance.longiAscValueText.text = string.Format("{0}° {1}′", num11, num12);
					int num13 = 0;
					if (__instance.planet.type != EPlanetType.Gas)
					{
						if (flag)
						{
							for (int j = 0; j < 6; j++)
							{
								int num14 = j + 1;
								VeinProto veinProto = LDB.veins.Select(num14);
								ItemProto itemProto = LDB.items.Select(veinProto.MiningItem);
								if (veinProto != null && itemProto != null)
								{
									UIResAmountEntry entry = __instance.GetEntry();
									__instance.entries.Add(entry);
									entry.SetInfo(num13, itemProto.name, veinProto.iconSprite, veinProto.description, false, false, "                ");
									entry.refId = num14;
									num13++;
								}
							}
						}

						int waterItemId = __instance.planet.waterItemId;
						Sprite icon = null;
						string valueString = "无".Translate();
						if (waterItemId < 0)
						{
							if (waterItemId == -1)
							{
								valueString = "熔岩".Translate();
							}
							else if (waterItemId == -2)
							{
								valueString = "冰".Translate();
							}
							else
							{
								valueString = "未知".Translate();
							}
						}

						ItemProto itemProto2 = LDB.items.Select(waterItemId);
						if (itemProto2 != null)
						{
							icon = itemProto2.iconSprite;
							valueString = itemProto2.name;
						}

						UIResAmountEntry entry2 = __instance.GetEntry();
						__instance.entries.Add(entry2);
						entry2.SetInfo(num13, "海洋类型".Translate(), icon, (itemProto2 != null) ? itemProto2.description : "", false, itemProto2 != null && waterItemId != 1000, "");
						entry2.valueString = valueString;
						num13++;
						UIResAmountEntry entry3 = __instance.GetEntry();
						__instance.entries.Add(entry3);
						entry3.SetInfo(num13, "适建区域".Translate(), __instance.sprite6, "", false, false, "      %");
						if (__instance.planet.landPercentDirty)
						{
							PlanetAlgorithm.CalcLandPercent(__instance.planet);
							__instance.planet.landPercentDirty = false;
						}

						StringBuilderUtility.WritePositiveFloat(entry3.sb, 0, 5, __instance.planet.landPercent * 100f, 1, ' ');
						entry3.DisplayStringBuilder();
						num13++;
						UIResAmountEntry entry4 = __instance.GetEntry();
						__instance.entries.Add(entry4);
						entry4.SetInfo(num13, "风能利用率".Translate(), __instance.sprite8, "", false, __instance.planet.windStrength > 1.499f, "    %");
						StringBuilderUtility.WriteUInt(entry4.sb, 0, 3, (uint)(__instance.planet.windStrength * 100f + 0.4999f), 1, ' ');
						entry4.DisplayStringBuilder();
						num13++;
						UIResAmountEntry entry5 = __instance.GetEntry();
						__instance.entries.Add(entry5);
						entry5.SetInfo(num13, "光能利用率".Translate(), __instance.sprite9, "", false, __instance.planet.luminosity > 1.499f, "    %");
						StringBuilderUtility.WriteUInt(entry5.sb, 0, 3, (uint)(__instance.planet.luminosity * 100f + 0.4999f), 1, ' ');
						entry5.DisplayStringBuilder();
						num13++;
						if (flag)
						{
							for (int k = 7; k < 15; k++)
							{
								int num15 = k;
								VeinProto veinProto2 = LDB.veins.Select(num15);
								ItemProto itemProto3 = LDB.items.Select(veinProto2.MiningItem);
								if (veinProto2 != null && itemProto3 != null && __instance.veinCounts[num15] > 0)
								{
									UIResAmountEntry entry6 = __instance.GetEntry();
									__instance.entries.Add(entry6);
									entry6.SetInfo(num13, itemProto3.name, veinProto2.iconSprite, veinProto2.description, true, false, (k == 7) ? "         /s" : "                ");
									entry6.refId = num15;
									num13++;
								}
							}
						}

						if (!flag)
						{
							UIResAmountEntry entry7 = __instance.GetEntry();
							__instance.entries.Add(entry7);
							entry7.SetInfo(num13, "", null, "", true, true, "");
							__instance.tipEntry = entry7;
							num13++;
						}
					}
					else
					{
						if (flag)
						{
							for (int l = 0; l < __instance.planet.gasItems.Length; l++)
							{
								ItemProto itemProto4 = LDB.items.Select(__instance.planet.gasItems[l]);
								if (itemProto4 != null)
								{
									UIResAmountEntry entry8 = __instance.GetEntry();
									__instance.entries.Add(entry8);
									if (flag)
									{
										entry8.SetInfo(num13, "可采集".Translate() + itemProto4.name, itemProto4.iconSprite, "环绕行星手动采集".Translate(), false, false, "        /s");
										StringBuilderUtility.WritePositiveFloat(entry8.sb, 0, 7, __instance.planet.gasSpeeds[l], 4, ' ');
										entry8.DisplayStringBuilder();
									}
									else
									{
										entry8.SetInfo(num13, "未知".Translate(), __instance.unknownResIcon, "环绕行星手动采集".Translate(), false, false, "        /s");
										entry8.valueString = "未知".Translate();
									}

									entry8.SetObserved(flag);
									num13++;
								}
							}
						}

						if (!flag)
						{
							UIResAmountEntry entry9 = __instance.GetEntry();
							__instance.entries.Add(entry9);
							entry9.SetInfo(num13, "", null, "", true, true, "");
							__instance.tipEntry = entry9;
							num13++;
						}
					}

					__instance.SetResCount(num13);
					__instance.RefreshDynamicProperties();
				}
			}
			return false;
		}
	}
}

// [HarmonyPrefix]
        // [HarmonyPatch(typeof(UIPlanetDetail), "OnPlanetDataSet")]
        // public static bool OnPlanetDataSetPrefix(ref UIPlanetDetail __instance) //, ref UIResAmountEntry __instance.tipEntry,
        //     // ref Text __instance.nameText,
        //     // ref InputField __instance.nameInput, ref Text __instance.typeText, ref Text __instance.orbitRadiusValueText, ref Text __instance.orbitRadiusValueTextEx, ref Text __instance.orbitPeriodValueText, ref Text __instance.rotationPeriodValueText, ref Text __instance.inclinationValueText, ref Text __instance.obliquityValueText, ref Sprite __instance.unknownResIcon, ref Sprite __instance.sprite6, ref Sprite __instance.sprite8, ref Sprite __instance.sprite9)
        // {
        //     //var getEntry = Traverse.Create(__instance).Method("GetEntry");
        //
        //     //getEntry.GetValue<UIResAmountEntry>();
        //     for (var index = 0; index < __instance.entries.Count; ++index)
        //     {
        //         var entry = __instance.entries[index];
        //         entry.SetEmpty();
        //         __instance.pool.Add(entry);
        //     }
        //
        //     __instance.entries.Clear();
        //     __instance.tipEntry = null;
        //     if (__instance.planet == null) return false;
        //
        //     var _observed = true; // GameMain.history.universeObserveLevel >= (__instance.planet != GameMain.localPlanet ? 2 : 1);
        //     __instance.nameInput.text = __instance.planet.displayName;
        //     var empty = string.Empty;
        //     __instance.typeText.text = string.Format("{0} {1}", __instance.planet.typeString, "<color=\"#FD965EC0\">" + __instance.planet.singularityString + "</color>");
        //     __instance.orbitRadiusValueText.text = __instance.planet.orbitRadius.ToString("0.00#") + " AU";
        //     __instance.orbitRadiusValueTextEx.text = __instance.planet.name;
        //     __instance.orbitPeriodValueText.text = __instance.planet.orbitalPeriod.ToString("#,##0") + "空格秒".Translate();
        //     __instance.rotationPeriodValueText.text = __instance.planet.rotationPeriod.ToString("#,##0") + "空格秒".Translate();
        //     var f = __instance.planet.orbitInclination;
        //     var t = 180f - __instance.planet.orbitLongitude;
        //     if (f < 0.0)
        //     {
        //         f = -f;
        //         t = 180f + t;
        //     }
        //
        //     var num1 = Mathf.Abs(__instance.planet.orbitInclination);
        //     var num2 = (int)num1;
        //     var num3 = (int)((num1 - (double)num2) * 60.0);
        //     if (__instance.planet.orbitInclination < 0.0) num2 = -num2;
        //
        //     var num4 = Mathf.Abs(__instance.planet.obliquity);
        //     var num5 = (int)num4;
        //     var num6 = (int)((num4 - (double)num5) * 60.0);
        //     int numa;
        //     var numb = (int)(((numa = (int)Mathf.Repeat(t, 360f)) - (double)numa) * 60.0);
        //     if (__instance.planet.obliquity < 0.0) num5 = -num5;
        //
        //     __instance.inclinationValueText.text = string.Format("{0}° {1}′", num2, num3);
        //     __instance.obliquityValueText.text = string.Format("{0}° {1}′", num5, num6);
        //     __instance.longiAscValueText.text = string.Format("{0}° {1}′", numa, numb);
        //     var num7 = 0;
        //     if (__instance.planet.type != EPlanetType.Gas)
        //     {
        //         // Logger.LogMessage("TEST");
        //         for (var index = 0; index < 6; ++index)
        //         {
        //             var id = index + 1;
        //             var veinProto = LDB.veins.Select(id);
        //             var itemProto = LDB.items.Select(veinProto.MiningItem);
        //             if (veinProto != null && itemProto != null)
        //             {
        //                 var entry = __instance.GetEntry();
        //                 ;
        //                 __instance.entries.Add(entry);
        //                 entry.SetInfo(num7, itemProto.name, veinProto.iconSprite, veinProto.description, false, false, "                ");
        //                 entry.refId = id;
        //                 ++num7;
        //             }
        //         }
        //
        //         var waterItemId = __instance.planet.waterItemId;
        //         Sprite icon = null;
        //         var str = "无".Translate();
        //         if (waterItemId < 0)
        //         {
        //             if (waterItemId == -1)
        //             {
        //                 str = "熔岩".Translate();
        //             }
        //             else if (waterItemId == -2)
        //             {
        //                 str = "冰".Translate();
        //             }
        //             else
        //             {
        //                 str = "未知".Translate();
        //             }
        //         }
        //
        //         var itemProto1 = LDB.items.Select(waterItemId);
        //         if (itemProto1 != null)
        //         {
        //             icon = itemProto1.iconSprite;
        //             str = itemProto1.name;
        //         }
        //
        //         var entry1 = __instance.GetEntry();
        //         ;
        //         __instance.entries.Add(entry1);
        //         entry1.SetInfo(num7, "海洋类型".Translate(), icon, itemProto1 == null ? string.Empty : itemProto1.description, false, itemProto1 != null && waterItemId != 1000, string.Empty);
        //         entry1.valueString = str;
        //         var index1 = num7 + 1;
        //         var entry2 = __instance.GetEntry();
        //         ;
        //         __instance.entries.Add(entry2);
        //         entry2.SetInfo(index1, "适建区域".Translate(), __instance.sprite6, string.Empty, false, false, "      %");
        //         if (__instance.planet.landPercentDirty)
        //         {
        //             PlanetAlgorithm.CalcLandPercent(__instance.planet);
        //             __instance.planet.landPercentDirty = false;
        //         }
        //
        //         StringBuilderUtility.WritePositiveFloat(entry2.sb, 0, 5, __instance.planet.landPercent * 100f, 1);
        //         entry2.DisplayStringBuilder();
        //         var index2 = index1 + 1;
        //         var entry3 = __instance.GetEntry();
        //         ;
        //         __instance.entries.Add(entry3);
        //         entry3.SetInfo(index2, "风能利用率".Translate(), __instance.sprite8, string.Empty, false, __instance.planet.windStrength > 1.49899995326996, "      %");
        //         StringBuilderUtility.WriteUInt(entry3.sb, 0, 5, (uint)(__instance.planet.windStrength * 100.0 + 0.499900013208389));
        //         entry3.DisplayStringBuilder();
        //         var index3 = index2 + 1;
        //         var entry4 = __instance.GetEntry();
        //         ;
        //         __instance.entries.Add(entry4);
        //         entry4.SetInfo(index3, "光能利用率".Translate(), __instance.sprite9, string.Empty, false, __instance.planet.luminosity > 1.49899995326996, "      %");
        //         StringBuilderUtility.WriteUInt(entry4.sb, 0, 5, (uint)(__instance.planet.luminosity * 100.0 + 0.499900013208389));
        //         entry4.DisplayStringBuilder();
        //         num7 = index3 + 1;
        //         for (var index4 = 7; index4 < 15; ++index4)
        //         {
        //             var id = index4;
        //             var veinProto = LDB.veins.Select(id);
        //             var itemProto2 = LDB.items.Select(veinProto.MiningItem);
        //             if (veinProto != null && itemProto2 != null && __instance.veinCounts[id] > 0L)
        //             {
        //                 var entry5 = __instance.GetEntry();
        //                 ;
        //                 __instance.entries.Add(entry5);
        //                 entry5.SetInfo(num7, itemProto2.name, veinProto.iconSprite, veinProto.description, true, false, index4 != 7 ? "                " : "         /s");
        //                 entry5.refId = id;
        //                 ++num7;
        //             }
        //         }
        //
        //         if (!_observed)
        //         {
        //             var entry5 = __instance.GetEntry();
        //             __instance.entries.Add(entry5);
        //             entry5.SetInfo(num7, string.Empty, null, string.Empty, false, false, string.Empty);
        //             __instance.tipEntry = entry5;
        //             ++num7;
        //         }
        //     }
        //     else
        //     {
        //         for (var index = 0; index < __instance.planet.gasItems.Length; ++index)
        //         {
        //             var itemProto = LDB.items.Select(__instance.planet.gasItems[index]);
        //             if (itemProto != null)
        //             {
        //                 var entry = __instance.GetEntry();
        //                 ;
        //                 __instance.entries.Add(entry);
        //                 if (_observed)
        //                 {
        //                     entry.SetInfo(num7, "可采集".Translate() + itemProto.name, itemProto.iconSprite, "环绕行星手动采集".Translate(), false, false, "        /s");
        //                     StringBuilderUtility.WritePositiveFloat(entry.sb, 0, 7, __instance.planet.gasSpeeds[index]);
        //                     entry.DisplayStringBuilder();
        //                 }
        //                 else
        //                 {
        //                     entry.SetInfo(num7, "未知".Translate(), __instance.unknownResIcon, "环绕行星手动采集".Translate(), false, false, "        /s");
        //                     entry.valueString = "未知".Translate();
        //                 }
        //
        //                 entry.SetObserved(_observed);
        //                 ++num7;
        //             }
        //         }
        //     }
        //
        //     __instance.SetResCount(num7);
        //     __instance.RefreshDynamicProperties();
        //     return false;
        // }
//     }
// }