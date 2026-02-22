/*
 * Change Log:
 * - 2026-01-25: Preserve radius row + temporary observe level via postfix/prefix.
 * - 2026-01-25: Comment out legacy full-prefix OnPlanetDataSet replacement.
 * - 2026-02-22: Add transpiler for safe orbit roman labels beyond vanilla limits.
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
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
			EnsureNameGenRomanCapacity(__instance?.planet?.orbitAround ?? 0);
			actualLevel = GameMain.history.universeObserveLevel;
			GameMain.history.universeObserveLevel = 4;
			return true;
		}

		private static void EnsureNameGenRomanCapacity(int orbitAround)
		{
			if (orbitAround <= 0)
			{
				return;
			}

			var roman = NameGen.roman ?? Array.Empty<string>();
			if (orbitAround < roman.Length)
			{
				return;
			}

			var desiredLength = orbitAround + 1;
			if (RomanNumbers.roman != null && RomanNumbers.roman.Length > desiredLength)
			{
				desiredLength = RomanNumbers.roman.Length;
			}

			var expanded = new string[desiredLength];
			Array.Copy(roman, expanded, roman.Length);
			for (var i = roman.Length; i < expanded.Length; i++)
			{
				if (RomanNumbers.roman != null && i < RomanNumbers.roman.Length)
				{
					expanded[i] = RomanNumbers.roman[i];
				}
				else
				{
					expanded[i] = i.ToString();
				}
			}

			NameGen.roman = expanded;
		}

		private static string SafeOrbitRoman(int orbitAround)
		{
			if (orbitAround <= 0)
			{
				return string.Empty;
			}

			if (RomanNumbers.roman != null && orbitAround < RomanNumbers.roman.Length)
			{
				return RomanNumbers.roman[orbitAround];
			}

			return orbitAround.ToString();
		}

		[HarmonyTranspiler]
		[HarmonyPatch(typeof(UIPlanetDetail), "OnPlanetDataSet")]
		public static IEnumerable<CodeInstruction> OnPlanetDataSetTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			var code = new List<CodeInstruction>(instructions);
			var safeRomanMethod = AccessTools.Method(typeof(PatchOnUIPlanetDetail), nameof(SafeOrbitRoman));

			for (var i = 0; i < code.Count - 2; i++)
			{
				if (code[i].opcode == OpCodes.Ldsfld
					&& code[i].operand is FieldInfo field
					&& field.DeclaringType == typeof(NameGen)
					&& field.Name == nameof(NameGen.roman)
					&& code[i + 2].opcode == OpCodes.Ldelem_Ref)
				{
					yield return code[i + 1];
					yield return new CodeInstruction(OpCodes.Call, safeRomanMethod);
					i += 2;
					continue;
				}

				yield return code[i];
			}

			for (var i = Math.Max(0, code.Count - 2); i < code.Count; i++)
			{
				yield return code[i];
			}
		}

		/*
		// UIPlanetDetail
// Token: 0x06002584 RID: 9604 RVA: 0x001A5278 File Offset: 0x001A3478
		[HarmonyPrefix]
		[HarmonyPatch(typeof(UIPlanetDetail), "OnPlanetDataSet")]
		public static bool
			OnPlanetDataSet7Prefix(ref UIPlanetDetail __instance) //, ref UIResAmountEntry __instance.tipEntry,
		{
            __instance.uiRoutePanel.astroId = ((__instance._planet != null) ? __instance._planet.astroId : 0);
            if (__instance.uiRoutePanel.active)
            {
                __instance.uiRoutePanel.RefreshEntries();
                __instance.uiRoutePanel.Refresh();
            }
            
			for (int i = 0; i < __instance.entries.Count; i++)
			{
				UIResAmountEntry uiresAmountEntry = __instance.entries[i];
				uiresAmountEntry.SetEmpty();
				__instance.pool.Add(uiresAmountEntry);
			}

			__instance.entries.Clear();
			__instance.tipEntry = null;
			__instance.speedTipEntry = null;
			bool flag = false;
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
			__instance._scanned = false;
			if (__instance.planet != null)
			{
				if (!__instance.planet.scanned)
				{
					__instance.planet.RunScanThread();
				}

				__instance._scanned = __instance.planet.scanned;
				int num = (__instance.planet == GameMain.localPlanet)
					? 1
					: ((__instance.planet.star == GameMain.localStar)
						? 2
						: (((GameMain.mainPlayer.uPosition - __instance.planet.uPosition).magnitude < 14400000.0)
							? 3
							: 4));
				bool flag2 = GameMain.history.universeObserveLevel >= num;
				if (__instance.planet.factory != null && GameMain.history.universeObserveLevel >= 1)
				{
					flag2 = true;
				}

				if (__instance._scanned && flag2)
				{
					__instance.planet.SummarizeVeinAmountsByFilter(ref __instance.veinAmounts, __instance.tmp_ids,
						__instance.uiGame.veinAmountDisplayFilter);
					__instance.planet.SummarizeVeinCountsByFilter(ref __instance.veinCounts, __instance.tmp_ids,
						__instance.uiGame.veinAmountDisplayFilter);
				}

				if (!__instance.nameInput.isFocused)
				{
					__instance.nameInput.text = __instance.planet.displayName;
				}

				string arg = "<color=\"#FD965EC0\">" + __instance.planet.singularityString + "</color>";
				__instance.typeText.text = string.Format("{0} {1}", __instance.planet.typeString, arg);
				
				
				
				string[] array = __instance.planet.briefString.Split(new char[]
				{
					';'
				});
				ThemeProto themeProto = LDB.themes.Select(__instance.planet.theme);
				int _num2 = themeProto.terrainMat.Length;
				int _num3 = __instance.planet.style % themeProto.terrainMat.Length;
				string s;
				if (_num2 < array.Length)
				{
					s = array[UnityEngine.Random.Range(0, array.Length)];
				}
				else if (_num2 > array.Length)
				{
					s = array[_num3 % array.Length];
				}
				else
				{
					s = array[_num3];
				}
				__instance.planetBrief.text = s.Translate();
				// __instance.planetBackContentRect.sizeDelta = new Vector2(__instance.planetBackContentRect.sizeDelta.x, __instance.planetBrief.preferredHeight);
				__instance.briefContentRect.sizeDelta = new Vector2(Mathf.Round(__instance.briefContentRect.sizeDelta.x), Mathf.Round(__instance.planetBrief.preferredHeight));
				__instance.briefContentRect.anchoredPosition = new Vector2(Mathf.Round(__instance.briefContentRect.anchoredPosition.x), 0f);
				
				
				
				__instance.orbitRadiusValueText.text = __instance.planet.orbitRadius.ToString("0.00#") + " AU";
				__instance.orbitRadiusValueTextEx.text = ((__instance.planet.orbitAround == 0)
					? "环绕恒星".Translate()
					: ("环绕空格".Translate() + RomanNumbers.roman[__instance.planet.orbitAround] + "号星".Translate()));
				__instance.orbitPeriodValueText.text =
					__instance.planet.orbitalPeriod.ToString("#,##0") + "空格秒".Translate();
				__instance.rotationPeriodValueText.text =
					__instance.planet.rotationPeriod.ToString("#,##0") + "空格秒".Translate();
				float num2 = __instance.planet.orbitInclination;
				float num3 = 180f - __instance.planet.orbitLongitude;
				if (num2 < 0f)
				{
					num2 = -num2;
					num3 = 180f + num3;
				}

				float num4 = Mathf.Abs(num2);
				int num5 = (int) num4;
				int num6 = (int) ((num4 - (float) num5) * 60f);
				float num7 = Mathf.Abs(__instance.planet.obliquity);
				int num8 = (int) num7;
				int num9 = (int) ((num7 - (float) num8) * 60f);
				if (__instance.planet.obliquity < 0f)
				{
					num8 = -num8;
				}

				float num10 = Mathf.Repeat(num3, 360f);
				int num11 = (int) num10;
				int num12 = (int) ((num10 - (float) num11) * 60f);
				__instance.inclinationValueText.text = string.Format("{0}° {1}′", num5, num6);
				__instance.obliquityValueText.text = string.Format("{0}° {1}′", num8, num9);
				__instance.longiAscValueText.text = string.Format("{0}° {1}′", num11, num12);
				int num13 = 0;
				if (__instance.planet.type != EPlanetType.Gas)
				{
					if (flag2)
					{
						for (int j = 0; j < 6; j++)
						{
							int num14 = j + 1;
							VeinProto veinProto = LDB.veins.Select(num14);
							ItemProto itemProto = LDB.items.Select(veinProto.MiningItem);
							if (veinProto != null && itemProto != null &&
							    (__instance.uiGame.veinAmountDisplayFilter != 1 || __instance.veinAmounts[num14] > 0L))
							{
								UIResAmountEntry entry = __instance.GetEntry();
								__instance.entries.Add(entry);
								entry.SetInfo(num13, itemProto.name, veinProto.iconSprite, veinProto.description, false,
									false, "                ");
								entry.refId = num14;
								num13++;
							}
						}
					}

					int waterItemId = __instance.planet.waterItemId;
					Sprite icon = null;
					string text = "无".Translate();
					if (waterItemId < 0)
					{
						if (waterItemId == -1)
						{
							text = "熔岩".Translate();
						}
						else if (waterItemId == -2)
						{
							text = "冰".Translate();
						}
						else
						{
							text = "未知".Translate();
						}
					}

					ItemProto itemProto2 = LDB.items.Select(waterItemId);
					if (itemProto2 != null)
					{
						icon = itemProto2.iconSprite;
						text = itemProto2.name;
					}

					if (__instance.uiGame.veinAmountDisplayFilter == 0)
					{
						UIResAmountEntry entry2 = __instance.GetEntry();
						__instance.entries.Add(entry2);
						entry2.SetInfo(num13, "海洋类型".Translate(), icon,
							(itemProto2 != null) ? itemProto2.description : "", false,
							itemProto2 != null && waterItemId != 1000, "");
						entry2.valueString = text;
						num13++;
					}
					else if (__instance.uiGame.veinAmountDisplayFilter == 1 && flag2)
					{
						float num15 = 0f;
						float miningSpeedScale = GameMain.history.miningSpeedScale;
						PlanetFactory factory = __instance.planet.factory;
						if (factory != null)
						{
							MinerComponent[] minerPool = factory.factorySystem.minerPool;
							int minerCursor = factory.factorySystem.minerCursor;
							PowerConsumerComponent[] consumerPool = factory.powerSystem.consumerPool;
							for (int k = 0; k < minerCursor; k++)
							{
								ref MinerComponent ptr = ref minerPool[k];
								if (ptr.id == k && ptr.type == EMinerType.Water && consumerPool[ptr.pcId].networkId > 0)
								{
									float num16 = (float) ((double) ptr.period / 600000.0);
									float num17 = 60f / num16;
									float num18 = (float) (0.0001 * (double) ptr.speed);
									num15 += num18 * num17;
								}
							}
						}

						if (num15 > 0f)
						{
							UIResAmountEntry entry3 = __instance.GetEntry();
							__instance.entries.Add(entry3);
							entry3.SetInfo(num13, text ?? "", icon, (itemProto2 != null) ? itemProto2.description : "",
								false, itemProto2 != null && waterItemId != 1000, "");
							entry3.valueString = (num15 * GameMain.history.miningSpeedScale).ToString("0") + " /min";
							num13++;
							flag = true;
						}
					}

					if (__instance.uiGame.veinAmountDisplayFilter == 0)
					{
						UIResAmountEntry entry4 = __instance.GetEntry();
						__instance.entries.Add(entry4);
						entry4.SetInfo(num13, "适建区域".Translate(), __instance.sprite6, "", false, false, "      %");
						if (__instance.planet.landPercentDirtyFlag)
						{
							PlanetAlgorithm.CalcLandPercent(__instance.planet);
							__instance.planet.landPercentDirtyFlag = false;
						}

						StringBuilderUtility.WritePositiveFloat(entry4.sb, 0, 5, __instance.planet.landPercent * 100f,
							1, ' ');
						entry4.DisplayStringBuilder();
						num13++;
					}

					if (__instance.uiGame.veinAmountDisplayFilter == 0)
					{
						UIResAmountEntry entry5 = __instance.GetEntry();
						__instance.entries.Add(entry5);
						entry5.SetInfo(num13, "风能利用率".Translate(), __instance.sprite8, "", false,
							__instance.planet.windStrength > 1.499f, "    %");
						StringBuilderUtility.WriteUInt(entry5.sb, 0, 3,
							(uint) (__instance.planet.windStrength * 100f + 0.4999f), 1, ' ');
						entry5.DisplayStringBuilder();
						num13++;
					}

					if (__instance.uiGame.veinAmountDisplayFilter == 0)
					{
						UIResAmountEntry entry6 = __instance.GetEntry();
						__instance.entries.Add(entry6);
						entry6.SetInfo(num13, "光能利用率".Translate(), __instance.sprite9, "", false,
							__instance.planet.luminosity > 1.499f, "    %");
						StringBuilderUtility.WriteUInt(entry6.sb, 0, 3,
							(uint) (__instance.planet.luminosity * 100f + 0.4999f), 1, ' ');
						entry6.DisplayStringBuilder();
						num13++;
					}

					if (flag2)
					{
						for (int l = 7; l < 15; l++)
						{
							int num19 = l;
							VeinProto veinProto2 = LDB.veins.Select(num19);
							ItemProto itemProto3 = LDB.items.Select(veinProto2.MiningItem);
							if (veinProto2 != null && itemProto3 != null && __instance.veinCounts[num19] > 0)
							{
								UIResAmountEntry entry7 = __instance.GetEntry();
								__instance.entries.Add(entry7);
								entry7.SetInfo(num13, itemProto3.name, veinProto2.iconSprite, veinProto2.description,
									true, false, (l == 7) ? "         /s" : "                ");
								entry7.refId = num19;
								num13++;
								if (l == 7 && __instance.uiGame.veinAmountDisplayFilter == 1)
								{
									flag = true;
								}
							}
						}
					}

					if (!flag2)
					{
						UIResAmountEntry entry8 = __instance.GetEntry();
						__instance.entries.Add(entry8);
						entry8.SetInfo(num13, "", null, "", true, true, "");
						__instance.tipEntry = entry8;
						num13++;
					}

					if (flag)
					{
						UIResAmountEntry entry9 = __instance.GetEntry();
						__instance.entries.Add(entry9);
						entry9.SetInfo(num13, "实际采集速度".Translate(), null, "", false, false, "");
						__instance.speedTipEntry = entry9;
						__instance.speedTipEntry.valueString = "";
						num13++;
					}
					var f = Utils.GetPlanetFactoryFromPlanetData(__instance.planet);
					var baseCount = f?.enemySystem?.bases?.count ?? 0;
					var relayCount = Utils.GetRelaysTargettingPlanet(__instance.planet) - baseCount;
					if (relayCount > 0)
					{
						UIResAmountEntry entry9 = __instance.GetEntry();
						__instance.entries.Add(entry9);
						entry9.SetInfo(num13, "Darkfog Bases".Translate(), null, "", false, false, "");
						__instance.speedTipEntry = entry9;
						__instance.speedTipEntry.valueString = relayCount.ToString();
						num13++;
					}
				}
				else
				{
					if (flag2)
					{
						for (int m = 0; m < __instance.planet.gasItems.Length; m++)
						{
							ItemProto itemProto4 = LDB.items.Select(__instance.planet.gasItems[m]);
							if (itemProto4 != null)
							{
								if (__instance.uiGame.veinAmountDisplayFilter == 0)
								{
									UIResAmountEntry entry10 = __instance.GetEntry();
									__instance.entries.Add(entry10);
									entry10.SetInfo(num13, "可采集".Translate() + itemProto4.name, itemProto4.iconSprite,
										"环绕行星手动采集".Translate(), false, false, "        /s");
									StringBuilderUtility.WritePositiveFloat(entry10.sb, 0, 7,
										__instance.planet.gasSpeeds[m], 4, ' ');
									entry10.DisplayStringBuilder();
									entry10.SetObserved(flag2);
									num13++;
								}
								else if (__instance.uiGame.veinAmountDisplayFilter == 1)
								{
									double num20 = 0.0;
									float miningSpeedScale2 = GameMain.history.miningSpeedScale;
									PlanetFactory factory2 = __instance.planet.factory;
									if (factory2 != null)
									{
										StationComponent[] stationPool = factory2.transport.stationPool;
										int stationCursor = factory2.transport.stationCursor;
										for (int n = 1; n < stationCursor; n++)
										{
											StationComponent stationComponent = stationPool[n];
											if (stationComponent != null && stationComponent.id == n &&
											    stationComponent.isCollector)
											{
												for (int num21 = 0; num21 < stationComponent.storage.Length; num21++)
												{
													if (stationComponent.storage[num21].itemId ==
													    __instance.planet.gasItems[m] &&
													    stationComponent.storage[num21].remoteLogic ==
													    ELogisticStorage.Supply)
													{
														PrefabDesc prefabDesc = LDB.items
															.Select(ItemProto.stationCollectorId).prefabDesc;
														double num22 = (double) prefabDesc.workEnergyPerTick * 60.0 /
														               (double) prefabDesc.stationCollectSpeed;
														double num23 = (double) miningSpeedScale2;
														double num24 = num22;
														double gasTotalHeat = __instance.planet.gasTotalHeat;
														double num25 = (double) ((gasTotalHeat - num24 <= 0.0)
															? 1f
															: ((float) ((num23 * gasTotalHeat - num24) /
															            (gasTotalHeat - num24))));
														double num26 =
															(double) stationComponent.collectionPerTick[num21] * num25;
														double num27 = 3600.0 * num26;
														if (num27 > 0.0)
														{
															num20 += num27;
															flag = true;
														}
													}
												}
											}
										}
									}

									if (num20 > 0.0)
									{
										UIResAmountEntry entry11 = __instance.GetEntry();
										__instance.entries.Add(entry11);
										entry11.SetInfo(num13, itemProto4.name, itemProto4.iconSprite,
											itemProto4.description, false, false, "");
										if (num20 < 10000.0)
										{
											entry11.valueString = string.Format("{0:0.0}/min", num20);
										}
										else if (num20 < 1000000.0)
										{
											entry11.valueString = string.Format("{0:0.00}k/min", num20 / 1000.0);
										}
										else
										{
											entry11.valueString = string.Format("{0:0.000}M/min", num20 / 1000000.0);
										}

										entry11.SetObserved(flag2);
										num13++;
									}
								}
								else if (__instance.uiGame.veinAmountDisplayFilter == 2)
								{
									bool flag3 = false;
									PlanetFactory factory3 = __instance.planet.factory;
									if (factory3 != null)
									{
										StationComponent[] stationPool2 = factory3.transport.stationPool;
										int stationCursor2 = factory3.transport.stationCursor;
										for (int num28 = 1; num28 < stationCursor2; num28++)
										{
											StationComponent stationComponent2 = stationPool2[num28];
											if (stationComponent2 != null && stationComponent2.id == num28 &&
											    stationComponent2.isCollector)
											{
												for (int num29 = 0; num29 < stationComponent2.storage.Length; num29++)
												{
													if (stationComponent2.storage[num29].itemId ==
													    __instance.planet.gasItems[m] &&
													    stationComponent2.storage[num29].remoteLogic ==
													    ELogisticStorage.Supply)
													{
														flag3 = true;
														break;
													}
												}
											}

											if (flag3)
											{
												break;
											}
										}
									}

									if (!flag3)
									{
										UIResAmountEntry entry12 = __instance.GetEntry();
										__instance.entries.Add(entry12);
										entry12.SetInfo(num13, "可采集".Translate() + itemProto4.name,
											itemProto4.iconSprite, "环绕行星手动采集".Translate(), false, false, "        /s");
										StringBuilderUtility.WritePositiveFloat(entry12.sb, 0, 7,
											__instance.planet.gasSpeeds[m], 4, ' ');
										entry12.DisplayStringBuilder();
										entry12.SetObserved(flag2);
										num13++;
									}
								}
							}
						}
					}
					else
					{
						for (int num30 = 0; num30 < __instance.planet.gasItems.Length; num30++)
						{
							if (LDB.items.Select(__instance.planet.gasItems[num30]) != null)
							{
								if (__instance.uiGame.veinAmountDisplayFilter != 1)
								{
									UIResAmountEntry entry13 = __instance.GetEntry();
									__instance.entries.Add(entry13);
									entry13.SetInfo(num13, "未知".Translate(), __instance.unknownResIcon, "", false,
										false, "        /s");
									entry13.valueString = "未知".Translate();
									entry13.SetObserved(flag2);
									num13++;
								}
								else
								{
									bool flag4 = false;
									PlanetFactory factory4 = __instance.planet.factory;
									if (factory4 != null)
									{
										StationComponent[] stationPool3 = factory4.transport.stationPool;
										int stationCursor3 = factory4.transport.stationCursor;
										for (int num31 = 1; num31 < stationCursor3; num31++)
										{
											StationComponent stationComponent3 = stationPool3[num31];
											if (stationComponent3 != null && stationComponent3.id == num31 &&
											    stationComponent3.isCollector)
											{
												for (int num32 = 0; num32 < stationComponent3.storage.Length; num32++)
												{
													if (stationComponent3.storage[num32].itemId ==
													    __instance.planet.gasItems[num30] &&
													    stationComponent3.storage[num32].remoteLogic ==
													    ELogisticStorage.Supply)
													{
														flag4 = true;
														break;
													}
												}
											}

											if (flag4)
											{
												break;
											}
										}
									}

									if (flag4)
									{
										UIResAmountEntry entry14 = __instance.GetEntry();
										__instance.entries.Add(entry14);
										entry14.SetInfo(num13, "未知".Translate(), __instance.unknownResIcon, "", false,
											false, "        /s");
										entry14.valueString = "未知".Translate();
										entry14.SetObserved(flag2);
										num13++;
									}
								}
							}
						}
					}

					if (!flag2)
					{
						UIResAmountEntry entry15 = __instance.GetEntry();
						__instance.entries.Add(entry15);
						entry15.SetInfo(num13, "", null, "", true, true, "");
						__instance.tipEntry = entry15;
						num13++;
					}

					if (flag)
					{
						UIResAmountEntry entry16 = __instance.GetEntry();
						__instance.entries.Add(entry16);
						entry16.SetInfo(num13, "实际采集速度".Translate(), null, "", false, false, "");
						__instance.speedTipEntry = entry16;
						__instance.speedTipEntry.valueString = "";
						num13++;
					}
					var f = Utils.GetPlanetFactoryFromPlanetData(__instance.planet);
					var baseCount = f?.enemySystem?.bases?.count ?? 0;
					var relayCount = Utils.GetRelaysTargettingPlanet(__instance.planet) - baseCount;
					if (relayCount > 0)
					{
						UIResAmountEntry entry9 = __instance.GetEntry();
						__instance.entries.Add(entry9);
						entry9.SetInfo(num13, "Darkfog Bases".Translate(), null, "", false, false, "");
						__instance.speedTipEntry = entry9;
						__instance.speedTipEntry.valueString = relayCount.ToString();
						num13++;
					}
				}

				__instance.SetResCount(num13);
				__instance.RefreshDynamicProperties();
                __instance.RefreshTabPanel();
			}

			return false;
		}
		*/
	}
}

