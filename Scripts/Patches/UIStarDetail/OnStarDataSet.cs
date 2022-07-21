using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class PatchOnUIStarDetail
    {
        private static int actualLevel = 5;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarDetail), "OnStarDataSet")]
        private static void OnStarDataSetPost(StarData ____star, InputField ___nameInput, Text ___typeText, RectTransform ___paramGroup, Text ___massValueText, Text ___spectrValueText, Text ___radiusValueText, Text ___luminoValueText, Text ___temperatureValueText, Text ___ageValueText, Sprite ___unknownResIcon, GameObject ___trslBg, GameObject ___imgBg, UIResAmountEntry ___tipEntry, UIResAmountEntry ___entryPrafab, ref UIStarDetail __instance)
        {
            if (!SystemDisplay.inGalaxySelect) return;
            GameMain.history.universeObserveLevel = actualLevel;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarDetail), "OnStarDataSet")]
        private static bool OnStarDataSet(StarData ____star, InputField ___nameInput, Text ___typeText, RectTransform ___paramGroup, Text ___massValueText, Text ___spectrValueText, Text ___radiusValueText, Text ___luminoValueText, Text ___temperatureValueText, Text ___ageValueText, Sprite ___unknownResIcon, GameObject ___trslBg, GameObject ___imgBg, UIResAmountEntry ___tipEntry, UIResAmountEntry ___entryPrafab, ref UIStarDetail __instance)
        {
            if (!SystemDisplay.inGalaxySelect) return true;
            actualLevel = GameMain.history.universeObserveLevel;
            GameMain.history.universeObserveLevel = 4;
            return true;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarDetail), "OnStarDataSet")]
        public static bool OnStarDataSet2(StarData ____star, InputField ___nameInput, Text ___typeText, RectTransform ___paramGroup, Text ___massValueText, Text ___spectrValueText, Text ___radiusValueText, Text ___luminoValueText, Text ___temperatureValueText, Text ___ageValueText, Sprite ___unknownResIcon, GameObject ___trslBg, GameObject ___imgBg, UIResAmountEntry ___tipEntry, UIResAmountEntry ___entryPrafab, ref UIStarDetail __instance)
        {
            for (var i = 0; i < __instance.entries.Count; i++)
            {
                var uiresAmountEntry = __instance.entries[i];
                uiresAmountEntry.SetEmpty();
                __instance.pool.Add(uiresAmountEntry);
            }

            __instance.entries.Clear();
            __instance.tipEntry = null;
            if (__instance.veinAmounts == null) __instance.veinAmounts = new long[64];

            if (__instance.veinCounts == null) __instance.veinCounts = new int[64];

            Array.Clear(__instance.veinAmounts, 0, __instance.veinAmounts.Length);
            Array.Clear(__instance.veinCounts, 0, __instance.veinCounts.Length);
            __instance.calculated = false;
            if (__instance.star != null)
            {
                if (!__instance.star.calculated) __instance.star.RunCalculateThread();

                __instance.calculated = __instance.star.calculated;
                var magnitude = (__instance.star.uPosition - GameMain.mainPlayer.uPosition).magnitude;
                var num = __instance.star == GameMain.localStar ? 2 : magnitude < 14400000.0 ? 3 : 4;
                var flag = GameMain.history.universeObserveLevel >= num;
                if (__instance.calculated && flag)
                {
                    __instance.star.CalcVeinAmounts(ref __instance.veinAmounts);
                    __instance.star.CalcVeinCounts(ref __instance.veinCounts);
                }

                if (!__instance.nameInput.isFocused) __instance.nameInput.text = __instance.star.displayName;

                __instance.typeText.text = __instance.star.typeString;
                __instance.massValueText.text = __instance.star.mass.ToString("0.000") + " M    ";
                __instance.spectrValueText.text = __instance.star.spectr.ToString();
                __instance.radiusValueText.text = __instance.star.radius.ToString("0.00") + " R    ";
                double num2 = __instance.star.dysonLumino;
                __instance.luminoValueText.text = num2.ToString("0.000") + " L    ";
                __instance.temperatureValueText.text = __instance.star.temperature.ToString("#,##0") + " K";
                if (Localization.isKMG)
                    __instance.ageValueText.text = (__instance.star.age * __instance.star.lifetime).ToString("#,##0 ") + "百万亿年".Translate();
                else
                    __instance.ageValueText.text = (__instance.star.age * __instance.star.lifetime * 0.01f).ToString("#,##0.00 ") + "百万亿年".Translate();

                var num3 = 0;
                if (flag)
                    for (var j = 1; j < 15; j++)
                    {
                        var num4 = j;
                        var veinProto = LDB.veins.Select(num4);
                        var itemProto = LDB.items.Select(veinProto.MiningItem);
                        var flag2 = __instance.veinAmounts[j] > 0L; //Has this vein
                        if (itemProto != null && flag2)
                        {
                            var entry = __instance.GetEntry();
                            __instance.entries.Add(entry);
                            entry.SetInfo(num3, itemProto.name, veinProto.iconSprite, veinProto.description, j >= 7, false, j == 7 ? "         /s" : "                ");
                            entry.refId = num4;
                            num3++;
                        }
                    }

                var entrycounts = new Dictionary<ItemProto, int>();
                var entries = new Dictionary<ItemProto, UIResAmountEntry>();
                if (flag)
                    for (var index = 0; index < __instance.star.planetCount; ++index)
                    {
                        var waterItemId = __instance.star.planets[index].waterItemId;
                        "无".Translate();
                        if (waterItemId > 0)
                        {
                            var itemProto = LDB.items.Select(waterItemId);
                            if (itemProto != null)
                            {
                                if (entrycounts.ContainsKey(itemProto))
                                {
                                    entrycounts[itemProto]++;
                                    var entry = entries[itemProto];
                                    entry._label = entry.labelText.text = $"{itemProto.name}{" x" + entrycounts[itemProto]}";
                                    // GS2.WarnJson(entry);
                                }
                                else
                                {
                                    entrycounts.Add(itemProto, 1);
                                    var iconSprite = itemProto.iconSprite;
                                    var name = itemProto.name;
                                    var entry = __instance.GetEntry();
                                    __instance.entries.Add(entry);
                                    // GS2.Log($"Num:{num}");
                                    entry.SetInfo(num3, name, iconSprite, itemProto.description, waterItemId != 1000, false, string.Empty);
                                    entry.valueString = "海洋".Translate();
                                    entries.Add(itemProto, entry);

                                    ++num3;
                                }
                            }
                        }
                    }

                //*
                var resources = new Dictionary<int, float>();

                for (var index1 = 0; index1 < __instance.star.planetCount; ++index1)
                {
                    var planet = __instance.star.planets[index1];
                    if (planet.type == EPlanetType.Gas && planet.gasItems != null)
                        for (var i = 0; i < planet.gasItems.Length; i++)
                            if (resources.ContainsKey(planet.gasItems[i]))
                                resources[planet.gasItems[i]] += planet.gasSpeeds[i];
                            else
                                resources[planet.gasItems[i]] = planet.gasSpeeds[i];
                }


                foreach (var keyValuePair in resources)
                {
                    var itemProto = LDB.items.Select(keyValuePair.Key);

                    var entry = __instance.GetEntry();
                    __instance.entries.Add(entry);
                    if (flag)
                    {
                        entry.SetInfo(num3, itemProto.name, itemProto.iconSprite, itemProto.description, false, false, "        /s");
                        if (__instance.star.loaded)
                        {
                            StringBuilderUtility.WritePositiveFloat(entry.sb, 0, 7, keyValuePair.Value);
                            entry.DisplayStringBuilder();
                        }
                        else
                        {
                            entry.valueString = "探测到信号".Translate();
                        }
                    }
                    else
                    {
                        entry.SetInfo(num3, "未知".Translate(), ___unknownResIcon, string.Empty, false, false, "        /s");
                        entry.valueString = "未知".Translate();
                    }

                    entry.SetObserved(flag);
                    ++num3;
                }


                // if (flag)
                // {
                // 	// Create Water Items
                // 	for (int k = 0; k < __instance.star.planetCount; k++)
                // 	{
                // 		int waterItemId = __instance.star.planets[k].waterItemId;
                // 		string label = "无".Translate();
                // 		if (waterItemId > 0)
                // 		{
                // 			ItemProto itemProto2 = LDB.items.Select(waterItemId);
                // 			if (itemProto2 != null)
                // 			{
                // 				Sprite iconSprite = itemProto2.iconSprite;
                // 				label = itemProto2.name;
                // 				UIResAmountEntry entry2 = __instance.GetEntry();
                // 				__instance.entries.Add(entry2);
                // 				entry2.SetInfo(num3, label, iconSprite, itemProto2.description, itemProto2 != null && waterItemId != 1000, false, "");
                // 				entry2.valueString = "海洋".Translate();
                // 				num3++;
                // 			}
                // 		}
                // 	}
                // 	//End Create Water Items
                // }
                // if (flag)
                // {
                // 	for (int l = 0; l < __instance.star.planetCount; l++)
                // 	{
                // 		PlanetData planetData = __instance.star.planets[l];
                // 		if (planetData.type == EPlanetType.Gas && planetData.gasItems != null)
                // 		{
                // 			for (int m = 0; m < planetData.gasItems.Length; m++)
                // 			{
                // 				ItemProto itemProto3 = LDB.items.Select(planetData.gasItems[m]);
                // 				if (itemProto3 != null)
                // 				{
                // 					UIResAmountEntry entry3 = __instance.GetEntry();
                // 					__instance.entries.Add(entry3);
                // 					if (flag)
                // 					{
                // 						entry3.SetInfo(num3, itemProto3.name, itemProto3.iconSprite, itemProto3.description, false, false, "        /s");
                // 						StringBuilderUtility.WritePositiveFloat(entry3.sb, 0, 7, planetData.gasSpeeds[m], 2, ' ');
                // 						entry3.DisplayStringBuilder();
                // 					}
                // 					else
                // 					{
                // 						entry3.SetInfo(num3, "未知".Translate(), __instance.unknownResIcon, "", false, false, "        /s");
                // 						entry3.valueString = "未知".Translate();
                // 					}
                // 					entry3.SetObserved(flag);
                // 					num3++;
                // 				}
                // 			}
                // 		}
                // 	}
                // }
                if (!flag)
                {
                    var entry4 = __instance.GetEntry();
                    __instance.entries.Add(entry4);
                    entry4.SetInfo(num3, "", null, "", true, true, "");
                    __instance.tipEntry = entry4;
                    num3++;
                }

                __instance.SetResCount(num3);
                __instance.RefreshDynamicProperties();
            }

            return false;

            // var getEntry = Traverse.Create(__instance).Method("GetEntry");
            //
            // for (var index = 0; index < __instance.entries.Count; ++index)
            // {
            //     var entry = __instance.entries[index];
            //     entry.SetEmpty();
            //     __instance.pool.Add(entry);
            // }
            //
            // __instance.entries.Clear();
            // if (__instance.star == null) return false;
            // var magnitude = (__instance.star.uPosition - GameMain.mainPlayer.uPosition).magnitude;
            // var _observed = true; // GameMain.history.universeObserveLevel >= (__instance.star != GameMain.localStar ? magnitude >= 14400000.0 ? 4 : 3 : 2);
            // ___nameInput.text = __instance.star.displayName;
            // ___typeText.text = __instance.star.typeString;
            // ___massValueText.text = __instance.star.mass.ToString("0.000") + " M    ";
            // ___spectrValueText.text = __instance.star.spectr.ToString();
            // ___radiusValueText.text = __instance.star.radius.ToString("0.00") + " R    ";
            // ___luminoValueText.text = ((double)__instance.star.dysonLumino).ToString("0.000") + " L    ";
            // ___temperatureValueText.text = __instance.star.temperature.ToString("#,##0") + " K";
            // if (Localization.isKMG)
            //     ___ageValueText.text = (__instance.star.age * __instance.star.lifetime).ToString("#,##0 ") + "百万亿年".Translate();
            // else
            //     ___ageValueText.text = ((float)(__instance.star.age * (double)__instance.star.lifetime * 0.00999999977648258)).ToString("#,##0.00 ") + "百万亿年".Translate();
            //
            // var num = 0;
            // for (var type = 1; type < 15; ++type)
            // {
            //     var id = type;
            //     var veinProto = LDB.veins.Select(id);
            //     var itemProto = LDB.items.Select(veinProto.MiningItem);
            //     if (_observed || type < 7)
            //     {
            //         bool flag = __instance.veinAmounts[type] > 0L;
            //         if (veinProto != null && itemProto != null && flag)
            //         {
            //             var entry = getEntry.GetValue<UIResAmountEntry>();
            //             __instance.entries.Add(entry);
            //             entry.SetInfo(num, itemProto.name, veinProto.iconSprite, veinProto.description, type >= 7, false, type != 7 ? "                " : "         /s");
            //             entry.refId = id;
            //             ++num;
            //         }
            //     }
            // }
            //
            // var entrycounts = new Dictionary<ItemProto, int>();
            // var entries = new Dictionary<ItemProto, UIResAmountEntry>();
            // if (_observed)
            //     for (var index = 0; index < __instance.star.planetCount; ++index)
            //     {
            //         var waterItemId = __instance.star.planets[index].waterItemId;
            //         "无".Translate();
            //         if (waterItemId > 0)
            //         {
            //             var itemProto = LDB.items.Select(waterItemId);
            //             if (itemProto != null)
            //             {
            //                 if (entrycounts.ContainsKey(itemProto))
            //                 {
            //                     entrycounts[itemProto]++;
            //                     var entry = entries[itemProto];
            //                     entry._label = entry.labelText.text = $"{itemProto.name}{" x" + entrycounts[itemProto]}";
            //                     // GS2.WarnJson(entry);
            //                 }
            //                 else
            //                 {
            //                     entrycounts.Add(itemProto, 1);
            //                     var iconSprite = itemProto.iconSprite;
            //                     var name = itemProto.name;
            //                     var entry = getEntry.GetValue<UIResAmountEntry>();
            //                     __instance.entries.Add(entry);
            //                     // GS2.Log($"Num:{num}");
            //                     entry.SetInfo(num, name, iconSprite, itemProto.description, itemProto != null && waterItemId != 1000, false, string.Empty);
            //                     entry.valueString = "海洋".Translate();
            //                     entries.Add(itemProto, entry);
            //
            //                     ++num;
            //                 }
            //             }
            //         }
            //     }
            //
            // //*
            // var resources = new Dictionary<int, float>();
            //
            // for (var index1 = 0; index1 < __instance.star.planetCount; ++index1)
            // {
            //     var planet = __instance.star.planets[index1];
            //     if (planet.type == EPlanetType.Gas && planet.gasItems != null)
            //         for (var i = 0; i < planet.gasItems.Length; i++)
            //             if (resources.ContainsKey(planet.gasItems[i]))
            //                 resources[planet.gasItems[i]] += planet.gasSpeeds[i];
            //             else
            //                 resources[planet.gasItems[i]] = planet.gasSpeeds[i];
            // }
            //
            //
            // foreach (var keyValuePair in resources)
            // {
            //     var itemProto = LDB.items.Select(keyValuePair.Key);
            //
            //     var entry = getEntry.GetValue<UIResAmountEntry>();
            //     __instance.entries.Add(entry);
            //     if (_observed)
            //     {
            //         entry.SetInfo(num, itemProto.name, itemProto.iconSprite, itemProto.description, false, false, "        /s");
            //         if (__instance.star.loaded)
            //         {
            //             StringBuilderUtility.WritePositiveFloat(entry.sb, 0, 7, keyValuePair.Value);
            //             entry.DisplayStringBuilder();
            //         }
            //         else
            //         {
            //             entry.valueString = "探测到信号".Translate();
            //         }
            //     }
            //     else
            //     {
            //         entry.SetInfo(num, "未知".Translate(), ___unknownResIcon, string.Empty, false, false, "        /s");
            //         entry.valueString = "未知".Translate();
            //     }
            //
            //     entry.SetObserved(_observed);
            //     ++num;
            // }
            //
            // if (!_observed)
            // {
            //     var entry = getEntry.GetValue<UIResAmountEntry>();
            //     __instance.entries.Add(entry);
            //     entry.SetInfo(num, string.Empty, null, string.Empty, false, false, string.Empty);
            //     ___tipEntry = entry;
            //     var num1 = ____star != GameMain.localStar ? magnitude >= 14400000.0 ? 4 : 3 : 2;
            //     var flag = GameMain.history.universeObserveLevel >= num1;
            //     ___tipEntry.valueString = flag ? string.Empty : "宇宙探索等级".Translate() + num1;
            //     ++num;
            // }
            //
            // __instance.SetResCount(num);
            // __instance.RefreshDynamicProperties();
            // // foreach (var p in ____star.planets)
            // // {
            // //     GS2.Log($"{p.name}");
            // //     GS2.WarnJson(p.veinSpotsSketch);
            // // }
            //
            // return false;
        }
    }
}