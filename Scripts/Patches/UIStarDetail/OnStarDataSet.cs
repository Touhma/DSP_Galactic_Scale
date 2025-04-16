using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class PatchOnUIStarDetail
    {
        private static int actualLevel = -1;
        
        // Store the last checked frame to avoid checking every single frame
        private static int lastCheckedFrame = -1;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarDetail), "OnStarDataSet")]
        private static void OnStarDataSetPost(StarData ____star, InputField ___nameInput, Text ___typeText, RectTransform ___paramGroup, Text ___massValueText, Text ___spectrValueText, Text ___radiusValueText, Text ___luminoValueText, Text ___temperatureValueText, Text ___ageValueText, Sprite ___unknownResIcon, GameObject ___trslBg, GameObject ___imgBg, UIResAmountEntry ___tipEntry, UIResAmountEntry ___entryPrafab, ref UIStarDetail __instance)
        {
            if (!SystemDisplay.inGalaxySelect && actualLevel >= 0) 
            {
                GameMain.history.universeObserveLevel = actualLevel;
                GS2.Log($"Restoring universeObserveLevel to {actualLevel}");
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarDetail), "OnStarDataSet")]
        private static bool OnStarDataSet(StarData ____star, InputField ___nameInput, Text ___typeText, RectTransform ___paramGroup, Text ___massValueText, Text ___spectrValueText, Text ___radiusValueText, Text ___luminoValueText, Text ___temperatureValueText, Text ___ageValueText, Sprite ___unknownResIcon, GameObject ___trslBg, GameObject ___imgBg, UIResAmountEntry ___tipEntry, UIResAmountEntry ___entryPrafab, ref UIStarDetail __instance)
        {
            CheckAndInitializeLevel();
            
            // Always check for research level increases
            if (GameMain.history != null && GameMain.history.universeObserveLevel > actualLevel)
            {
                actualLevel = GameMain.history.universeObserveLevel;
                GS2.Log($"Updated stored universeObserveLevel to {actualLevel} (research or save loaded)");
            }
            
            // When in galaxy select, set maximum visibility while preserving actual level
            if (SystemDisplay.inSystemDisplay)
            {
                // Store current level if it's not already stored and it's valid
                if (actualLevel < 0 && GameMain.history != null)
                {
                    actualLevel = GameMain.history.universeObserveLevel;
                    GS2.Log($"Initialized universeObserveLevel to {actualLevel}");
                }
                
                // Set to max level for galaxy view
                if (GameMain.history != null)
                {
                    GameMain.history.universeObserveLevel = 4;
                    GS2.Log($"Set universeObserveLevel to 4 for Galaxy Selection {SystemDisplay.inSystemDisplay}");
                }
                
                return true;
            }

            return true;
        }
        
        // Helper method to check and initialize the level from GameMain.history
        private static void CheckAndInitializeLevel()
        {
            // Skip if we checked recently (optimization)
            if (Time.frameCount - lastCheckedFrame < 60) return;
            
            lastCheckedFrame = Time.frameCount;
            
            if (GameMain.history != null && actualLevel < 0)
            {
                actualLevel = GameMain.history.universeObserveLevel;
                GS2.Log($"Initialized universeObserveLevel to {actualLevel} on frame {Time.frameCount}");
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarDetail), "OnStarDataSet")]
        public static bool OnStarDataSet2(StarData ____star, InputField ___nameInput, Text ___typeText, RectTransform ___paramGroup, Text ___massValueText, Text ___spectrValueText, Text ___radiusValueText, Text ___luminoValueText, Text ___temperatureValueText, Text ___ageValueText, Sprite ___unknownResIcon, GameObject ___trslBg, GameObject ___imgBg, UIResAmountEntry ___tipEntry, UIResAmountEntry ___entryPrafab, ref UIStarDetail __instance)
        {
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
	__instance.calculated = false;
	if (__instance.star != null)
	{
		if (!__instance.star.scanned)
		{
			__instance.star.RunScanningThread();
		}
		__instance.calculated = __instance.star.scanned;
		double magnitude = (__instance.star.uPosition - GameMain.mainPlayer.uPosition).magnitude;
		int num = (__instance.star == GameMain.localStar) ? 2 : ((magnitude < 14400000.0) ? 3 : 4);
		bool flag2 = GameMain.history.universeObserveLevel >= num;
		if (__instance.calculated && flag2)
		{
			__instance.star.CalcVeinAmounts(ref __instance.veinAmounts, __instance.tmp_ids, __instance.uiGame.veinAmountDisplayFilter);
			__instance.star.CalcVeinCounts(ref __instance.veinCounts, __instance.tmp_ids, __instance.uiGame.veinAmountDisplayFilter);
		}
		if (!__instance.nameInput.isFocused)
		{
			__instance.nameInput.text = __instance.star.displayName;
		}
		__instance.typeText.text = __instance.star.typeString;
		__instance.massValueText.text = __instance.star.mass.ToString("0.000") + " M    ";
		__instance.spectrValueText.text = __instance.star.spectr.ToString();
		__instance.radiusValueText.text = __instance.star.radius.ToString("0.00") + " R    ";
		double num2 = (double)__instance.star.dysonLumino;
		__instance.luminoValueText.text = num2.ToString("0.000") + " L    ";
		__instance.temperatureValueText.text = __instance.star.temperature.ToString("#,##0") + " K";
		if (Localization.isKMG)
		{
			__instance.ageValueText.text = (__instance.star.age * __instance.star.lifetime).ToString("#,##0 ") + "百万亿年".Translate();
		}
		else
		{
			__instance.ageValueText.text = (__instance.star.age * __instance.star.lifetime * 0.01f).ToString("#,##0.00 ") + "百万亿年".Translate();
		}
		int index = 0;
		if (flag2)
		{
			for (int j = 1; j < 15; j++)
			{
				int num4 = j;
				VeinProto veinProto = LDB.veins.Select(num4);
				ItemProto itemProto = LDB.items.Select(veinProto.MiningItem);
				if (flag2 || j < 7)
				{
					bool flag3 = __instance.veinAmounts[j] > 0L;
					if (veinProto != null && itemProto != null && flag3)
					{
						UIResAmountEntry entry = __instance.GetEntry();
						__instance.entries.Add(entry);
						entry.SetInfo(index, itemProto.name, veinProto.iconSprite, veinProto.description, j >= 7, false, (j == 7) ? "         /s" : "                ");
						entry.refId = num4;
						index++;
						if (j == 7 && __instance.uiGame.veinAmountDisplayFilter == 1)
						{
							flag = true;
						}
					}
				}
			}
		}
		if (flag2)
		{
			for (int k = 0; k < __instance.star.planetCount; k++)
			{
				int waterItemId = __instance.star.planets[k].waterItemId;
				string text = "无".Translate();
				if (waterItemId > 0)
				{
					ItemProto itemProto2 = LDB.items.Select(waterItemId);
					if (itemProto2 != null)
					{
						Sprite iconSprite = itemProto2.iconSprite;
						text = itemProto2.name;
						if (__instance.uiGame.veinAmountDisplayFilter == 0)
						{
							UIResAmountEntry entry2 = __instance.GetEntry();
							__instance.entries.Add(entry2);
							entry2.SetInfo(index, text, iconSprite, itemProto2.description, itemProto2 != null && waterItemId != 1000, false, "");
							entry2.valueString = "海洋".Translate();
							index++;
						}
						else if (__instance.uiGame.veinAmountDisplayFilter == 1)
						{
							float num5 = 0f;
							float miningSpeedScale = GameMain.history.miningSpeedScale;
							PlanetFactory factory = __instance.star.planets[k].factory;
							if (factory != null)
							{
								MinerComponent[] minerPool = factory.factorySystem.minerPool;
								int minerCursor = factory.factorySystem.minerCursor;
								PowerConsumerComponent[] consumerPool = factory.powerSystem.consumerPool;
								for (int l = 0; l < minerCursor; l++)
								{
									ref MinerComponent ptr = ref minerPool[l];
									if (ptr.id == l && ptr.type == EMinerType.Water && consumerPool[ptr.pcId].networkId > 0)
									{
										float num6 = (float)((double)ptr.period / 600000.0);
										float num7 = 60f / num6;
										float num8 = (float)(0.0001 * (double)ptr.speed);
										num5 += num8 * num7;
									}
								}
							}
							if (num5 > 0f)
							{
								UIResAmountEntry entry3 = __instance.GetEntry();
								__instance.entries.Add(entry3);
								entry3.SetInfo(index, text ?? "", iconSprite, itemProto2.description, waterItemId != 1000, false, "");
								entry3.valueString = (num5 * GameMain.history.miningSpeedScale).ToString("0") + " /min";
								index++;
								flag = true;
							}
						}
					}
				}
			}
		}
		if (flag2)
		{
			for (int m = 0; m < __instance.star.planetCount; m++)
			{
				PlanetData planetData = __instance.star.planets[m];
				if (planetData.type == EPlanetType.Gas && planetData.gasItems != null)
				{
					for (int n = 0; n < planetData.gasItems.Length; n++)
					{
						ItemProto itemProto3 = LDB.items.Select(planetData.gasItems[n]);
						if (itemProto3 != null)
						{
							if (flag2)
							{
								if (__instance.uiGame.veinAmountDisplayFilter == 0)
								{
									UIResAmountEntry entry4 = __instance.GetEntry();
									__instance.entries.Add(entry4);
									entry4.SetInfo(index, itemProto3.name, itemProto3.iconSprite, itemProto3.description, false, false, "        /s");
									StringBuilderUtility.WritePositiveFloat(entry4.sb, 0, 7, planetData.gasSpeeds[n], 2, ' ');
									entry4.DisplayStringBuilder();
									entry4.SetObserved(flag2);
									index++;
								}
								else if (__instance.uiGame.veinAmountDisplayFilter == 1)
								{
									double num9 = 0.0;
									float miningSpeedScale2 = GameMain.history.miningSpeedScale;
									PlanetFactory factory2 = planetData.factory;
									if (factory2 != null)
									{
										StationComponent[] stationPool = factory2.transport.stationPool;
										int stationCursor = factory2.transport.stationCursor;
										for (int num10 = 1; num10 < stationCursor; num10++)
										{
											StationComponent stationComponent = stationPool[num10];
											if (stationComponent != null && stationComponent.id == num10 && stationComponent.isCollector)
											{
												for (int num11 = 0; num11 < stationComponent.storage.Length; num11++)
												{
													if (stationComponent.storage[num11].itemId == planetData.gasItems[n] && stationComponent.storage[num11].remoteLogic == ELogisticStorage.Supply)
													{
														PrefabDesc prefabDesc = LDB.items.Select(ItemProto.stationCollectorId).prefabDesc;
														double num12 = (double)prefabDesc.workEnergyPerTick * 60.0 / (double)prefabDesc.stationCollectSpeed;
														double num13 = (double)miningSpeedScale2;
														double num14 = num12;
														double gasTotalHeat = planetData.gasTotalHeat;
														double num15 = (double)((gasTotalHeat - num14 <= 0.0) ? 1f : ((float)((num13 * gasTotalHeat - num14) / (gasTotalHeat - num14))));
														double num16 = (double)stationComponent.collectionPerTick[num11] * num15;
														double num17 = 3600.0 * num16;
														if (num17 > 0.0)
														{
															num9 += num17;
															flag = true;
														}
													}
												}
											}
										}
									}
									if (num9 > 0.0)
									{
										UIResAmountEntry entry5 = __instance.GetEntry();
										__instance.entries.Add(entry5);
										entry5.SetInfo(index, itemProto3.name, itemProto3.iconSprite, itemProto3.description, false, false, "");
										if (num9 < 10000.0)
										{
											entry5.valueString = string.Format("{0:0.0} /min", num9);
										}
										else if (num9 < 1000000.0)
										{
											entry5.valueString = string.Format("{0:0.00}k /min", num9 / 1000.0);
										}
										else
										{
											entry5.valueString = string.Format("{0:0.000}M /min", num9 / 1000000.0);
										}
										entry5.SetObserved(flag2);
										index++;
									}
								}
								else if (__instance.uiGame.veinAmountDisplayFilter == 2)
								{
									bool flag4 = false;
									PlanetFactory factory3 = planetData.factory;
									if (factory3 != null)
									{
										StationComponent[] stationPool2 = factory3.transport.stationPool;
										int stationCursor2 = factory3.transport.stationCursor;
										for (int num18 = 1; num18 < stationCursor2; num18++)
										{
											StationComponent stationComponent2 = stationPool2[num18];
											if (stationComponent2 != null && stationComponent2.id == num18 && stationComponent2.isCollector)
											{
												for (int num19 = 0; num19 < stationComponent2.storage.Length; num19++)
												{
													if (stationComponent2.storage[num19].itemId == planetData.gasItems[n] && stationComponent2.storage[num19].remoteLogic == ELogisticStorage.Supply)
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
									if (!flag4)
									{
										UIResAmountEntry entry6 = __instance.GetEntry();
										__instance.entries.Add(entry6);
										entry6.SetInfo(index, "可采集".Translate() + itemProto3.name, itemProto3.iconSprite, "环绕行星手动采集".Translate(), false, false, "        /s");
										StringBuilderUtility.WritePositiveFloat(entry6.sb, 0, 7, planetData.gasSpeeds[n], 4, ' ');
										entry6.DisplayStringBuilder();
										entry6.SetObserved(flag2);
										index++;
									}
								}
							}
							else if (__instance.uiGame.veinAmountDisplayFilter != 1)
							{
								UIResAmountEntry entry7 = __instance.GetEntry();
								__instance.entries.Add(entry7);
								entry7.SetInfo(index, "未知".Translate(), __instance.unknownResIcon, "", false, false, "        /s");
								entry7.valueString = "未知".Translate();
								entry7.SetObserved(flag2);
								index++;
							}
							else
							{
								bool flag5 = false;
								PlanetFactory factory4 = planetData.factory;
								if (factory4 != null)
								{
									StationComponent[] stationPool3 = factory4.transport.stationPool;
									int stationCursor3 = factory4.transport.stationCursor;
									for (int num20 = 1; num20 < stationCursor3; num20++)
									{
										StationComponent stationComponent3 = stationPool3[num20];
										if (stationComponent3 != null && stationComponent3.id == num20 && stationComponent3.isCollector)
										{
											for (int num21 = 0; num21 < stationComponent3.storage.Length; num21++)
											{
												if (stationComponent3.storage[num21].itemId == planetData.gasItems[n] && stationComponent3.storage[num21].remoteLogic == ELogisticStorage.Supply)
												{
													flag5 = true;
													break;
												}
											}
										}
										if (flag5)
										{
											break;
										}
									}
								}
								if (flag5)
								{
									UIResAmountEntry entry8 = __instance.GetEntry();
									__instance.entries.Add(entry8);
									entry8.SetInfo(index, "未知".Translate(), __instance.unknownResIcon, "", false, false, "        /s");
									entry8.valueString = "未知".Translate();
									entry8.SetObserved(flag2);
									index++;
								}
							}
						}
					}
				}
			}
		}

		if (____star.initialHiveCount > 0)
		{
			UIResAmountEntry entry = __instance.GetEntry();
			__instance.entries.Add(entry);
			entry.SetInfo(index, "DarkFog Infested".Translate(), __instance.unknownResIcon, "", false, false, "        /s");
			entry.valueString = "".Translate();
			entry.SetObserved(flag2);
			index++;
		}
		if (!flag2)
		{
			UIResAmountEntry entry9 = __instance.GetEntry();
			__instance.entries.Add(entry9);
			entry9.SetInfo(index, "", null, "", true, true, "");
			__instance.tipEntry = entry9;
			index++;
		}
		if (flag)
		{
			UIResAmountEntry entry10 = __instance.GetEntry();
			__instance.entries.Add(entry10);
			entry10.SetInfo(index, "实际采集速度".Translate(), null, "", false, false, "");
			__instance.speedTipEntry = entry10;
			__instance.speedTipEntry.valueString = "";
			index++;
		}
		__instance.SetResCount(index);
		__instance.RefreshDynamicProperties();
	}

	return false;
        }
    }
}