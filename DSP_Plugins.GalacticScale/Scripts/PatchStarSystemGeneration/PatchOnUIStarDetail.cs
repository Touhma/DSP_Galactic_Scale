using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [HarmonyPatch(typeof(UIStarDetail))]
    public class PatchOnUIStarDetail {
        
        [HarmonyPrefix]
        [HarmonyPatch("OnStarDataSet")]
        private static bool OnStarDataSet(
            Text ___nameText,
            Text ___typeText,
            RectTransform ___paramGroup,
            Text ___massValueText,
            Text ___spectrValueText,
            Text ___radiusValueText,
            Text ___luminoValueText,
            Text ___temperatureValueText,
            Text ___ageValueText,
            Sprite ___unknownResIcon,
            GameObject ___trslBg,
            GameObject ___imgBg,
            UIResAmountEntry ___tipEntry,
            ref UIStarDetail __instance
        ) {
            var getEntry = Traverse.Create(__instance).Method("GetEntry").GetValue<UIResAmountEntry>();
            
            for (int index = 0; index < __instance.entries.Count; ++index) {
                UIResAmountEntry entry = __instance.entries[index];
                entry.SetEmpty();
                __instance.pool.Add(entry);
            }
            __instance.entries.Clear();
            ___tipEntry = (UIResAmountEntry) null;
            if (__instance.star == null) {
                return false;
            }
            double magnitude = (__instance.star.uPosition - GameMain.mainPlayer.uPosition).magnitude;
            bool _observed = GameMain.history.universeObserveLevel >= (__instance.star != GameMain.localStar ? (magnitude >= 14400000.0 ? 4 : 3) : 2);
            ___nameText.text = __instance.star.displayName;
            ___typeText.text =(__instance.star.typeString);
            ___massValueText.text =(__instance.star.mass.ToString("0.000") + " M    ");
            ___spectrValueText.text =(__instance.star.spectr.ToString());
            ___radiusValueText.text =(__instance.star.radius.ToString("0.00") + " R    ");
            ___luminoValueText.text =(((double) __instance.star.dysonLumino).ToString("0.000") + " L    ");
            ___temperatureValueText.text =(__instance.star.temperature.ToString("#,##0") + " K");
            if (Localization.isKMG)
                ___ageValueText.text =((__instance.star.age * __instance.star.lifetime).ToString("#,##0 ") + "百万亿年".Translate());
            else
                ___ageValueText.text =(((float) ((double) __instance.star.age * (double) __instance.star.lifetime * 0.00999999977648258)).ToString("#,##0.00 ") + "百万亿年".Translate());
            int num = 0;
            for (int type = 1; type < 15; ++type) {
                int id = type;
                VeinProto veinProto = LDB.veins.Select(id);
                ItemProto itemProto = LDB.items.Select(veinProto.MiningItem);
                if (_observed || type < 7) {
                    bool flag = (!__instance.star.loaded ? __instance.star.GetResourceSpots(type) > 0 : __instance.star.GetResourceAmount(type) > 0L) || type < 7;
                    if (veinProto != null && itemProto != null && flag) {
                        UIResAmountEntry entry = getEntry;
                        __instance.entries.Add(entry);
                        entry.SetInfo(num, itemProto.name, veinProto.iconSprite, veinProto.description, type >= 7, false, type != 7 ? "                " : "         /s");
                        entry.refId = id;
                        ++num;
                    }
                }
            }
            if (_observed) {
                for (int index = 0; index < __instance.star.planetCount; ++index) {
                    int waterItemId = __instance.star.planets[index].waterItemId;
                    "无".Translate();
                    if (waterItemId > 0) {
                        ItemProto itemProto = LDB.items.Select(waterItemId);
                        if (itemProto != null) {
                            Sprite iconSprite = itemProto.iconSprite;
                            string name = itemProto.name;
                            UIResAmountEntry entry = getEntry;
                            __instance.entries.Add(entry);
                            entry.SetInfo(num, name, iconSprite, itemProto.description, itemProto != null && waterItemId != 1000, false, string.Empty);
                            entry.valueString = "海洋".Translate();
                            ++num;
                        }
                    }
                }
            }
            
            /*
            Dictionary<int,float> ressources = new Dictionary<int,float>();
            
            for (int index1 = 0; index1 < __instance.star.planetCount; ++index1) {
                PlanetData planet = __instance.star.planets[index1];
                if (planet.type == EPlanetType.Gas && planet.gasItems != null) {
                    for (var i = 0; i < planet.gasItems.Length; i++) {
                        
                        if (ressources.ContainsKey(planet.gasItems[i])) {
                            ressources[planet.gasItems[i]] += planet.gasSpeeds[i];
                        }
                        else {
                            ressources[planet.gasItems[i]] = planet.gasSpeeds[i];
                        }
                    }
                }
            }
            
            
            foreach (var keyValuePair in ressources) {
                Debug.Log("keyValuePair.key" + keyValuePair.Key);
                ItemProto itemProto = LDB.items.Select(keyValuePair.Key);
                Debug.Log("itemProto.name" + itemProto.name);
                Debug.Log("keyValuePair.value" + keyValuePair.Value);
                
                UIResAmountEntry entry = getEntry;
                __instance.entries.Add(entry);
                if (_observed) {
                    entry.SetInfo(num, itemProto.name, itemProto.iconSprite, itemProto.description, false, false, "        /s");
                    if (__instance.star.loaded) {
                        StringBuilderUtility.WritePositiveFloat(entry.sb, 0, 7, keyValuePair.Value);
                        entry.DisplayStringBuilder();
                    }
                    else {
                        entry.valueString = "探测到信号".Translate();
                    }
                }
                else {
                    entry.SetInfo(num, "未知".Translate(), ___unknownResIcon, string.Empty, false, false, "        /s");
                    entry.valueString = "未知".Translate();
                }
                entry.SetObserved(_observed);
                ++num;
            }*/
            
            Debug.Log("Pool count : "+ __instance.pool.Count);
            
            for (int index1 = 0; index1 < __instance.star.planetCount; ++index1)
            {
                PlanetData planet = __instance.star.planets[index1];
                if (planet.type == EPlanetType.Gas && planet.gasItems != null)
                {
                    for (int index2 = 0; index2 < planet.gasItems.Length; ++index2)
                    {
                        ItemProto itemProto = LDB.items.Select(planet.gasItems[index2]);
                        if (itemProto != null)
                        {
                            UIResAmountEntry entry = getEntry;
                            __instance.entries.Add(entry);
                            if (_observed)
                            {
                                entry.SetInfo(num, itemProto.name, itemProto.iconSprite, itemProto.description, false, false, "        /s");
                                if (__instance.star.loaded)
                                {
                                    StringBuilderUtility.WritePositiveFloat(entry.sb, 0, 7, planet.gasSpeeds[index2]);
                                    entry.DisplayStringBuilder();
                                }
                                else
                                    entry.valueString = "探测到信号".Translate();
                            }
                            else
                            {
                                entry.SetInfo(num, "未知".Translate(), ___unknownResIcon, string.Empty, false, false, "        /s");
                                entry.valueString = "未知".Translate();
                            }
                            entry.SetObserved(_observed);
                            ++num;
                        }
                    }
                }
            }
            
            if (!_observed) {
                UIResAmountEntry entry = getEntry;
                __instance.entries.Add(entry);
                entry.SetInfo(num, string.Empty, (Sprite) null, string.Empty, false, false, string.Empty);
                ___tipEntry = entry;
                ++num;
            }
            __instance.SetResCount(num);
            __instance.RefreshDynamicProperties();

            return false;
        }
    }
}