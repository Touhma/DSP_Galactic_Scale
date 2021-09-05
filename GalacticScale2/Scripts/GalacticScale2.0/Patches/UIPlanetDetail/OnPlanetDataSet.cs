using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIPlanetDetail
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIPlanetDetail), "OnPlanetDataSet")]
        public static void OnPlanetDataSet(ref UIPlanetDetail __instance, Text ___obliquityValueText, ref PlanetData ____planet)
        {
            // Add the planets radius to the Planet Detail UI
            if (___obliquityValueText.transform.parent.transform.parent.childCount == 6)
            {
                GameObject radiusLabel;
                var obliquityLabel = ___obliquityValueText.transform.parent.gameObject;
                radiusLabel = Object.Instantiate(obliquityLabel, obliquityLabel.transform.parent.transform);

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

            if (___obliquityValueText.transform.parent.transform.parent.childCount == 7)
            {
                var p = ___obliquityValueText.transform.parent.parent;
                var radiusLabel = p.GetChild(p.childCount - 1).gameObject;
                var radiusValueText = radiusLabel.transform.GetChild(0).GetComponent<Text>();
                if (__instance.planet != null) radiusValueText.text = __instance.planet.realRadius.ToString();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIPlanetDetail), "OnPlanetDataSet")]
        public static bool OnPlanetDataSet(ref UIPlanetDetail __instance, ref UIResAmountEntry ___tipEntry,
            // ref Text ___nameText,
            ref InputField ___nameInput, ref Text ___typeText, ref Text ___orbitRadiusValueText, ref Text ___orbitRadiusValueTextEx, ref Text ___orbitPeriodValueText, ref Text ___rotationPeriodValueText, ref Text ___inclinationValueText, ref Text ___obliquityValueText, ref Sprite ___unknownResIcon, ref Sprite ___sprite6, ref Sprite ___sprite8, ref Sprite ___sprite9)
        {
            var getEntry = Traverse.Create(__instance).Method("GetEntry");

            getEntry.GetValue<UIResAmountEntry>();
            for (var index = 0; index < __instance.entries.Count; ++index)
            {
                var entry = __instance.entries[index];
                entry.SetEmpty();
                __instance.pool.Add(entry);
            }

            __instance.entries.Clear();
            ___tipEntry = null;
            if (__instance.planet == null) return false;

            var _observed = GameMain.history.universeObserveLevel >= (__instance.planet != GameMain.localPlanet ? 2 : 1);
            ___nameInput.text = __instance.planet.displayName;
            var empty = string.Empty;
            ___typeText.text = string.Format("{0} {1}", __instance.planet.typeString, "<color=\"#FD965EC0\">" + __instance.planet.singularityString + "</color>");
            ___orbitRadiusValueText.text = __instance.planet.orbitRadius.ToString("0.00#") + " AU";
            ___orbitRadiusValueTextEx.text = __instance.planet.name;
            ___orbitPeriodValueText.text = __instance.planet.orbitalPeriod.ToString("#,##0") + "空格秒".Translate();
            ___rotationPeriodValueText.text = __instance.planet.rotationPeriod.ToString("#,##0") + "空格秒".Translate();
            var num1 = Mathf.Abs(__instance.planet.orbitInclination);
            var num2 = (int)num1;
            var num3 = (int)((num1 - (double)num2) * 60.0);
            if (__instance.planet.orbitInclination < 0.0) num2 = -num2;

            var num4 = Mathf.Abs(__instance.planet.obliquity);
            var num5 = (int)num4;
            var num6 = (int)((num4 - (double)num5) * 60.0);
            if (__instance.planet.obliquity < 0.0) num5 = -num5;

            ___inclinationValueText.text = string.Format("{0}° {1}′", num2, num3);
            ___obliquityValueText.text = string.Format("{0}° {1}′", num5, num6);
            var num7 = 0;
            if (__instance.planet.type != EPlanetType.Gas)
            {
                // Logger.LogMessage("TEST");
                for (var index = 0; index < 6; ++index)
                {
                    var id = index + 1;
                    var veinProto = LDB.veins.Select(id);
                    var itemProto = LDB.items.Select(veinProto.MiningItem);
                    if (veinProto != null && itemProto != null)
                    {
                        var entry = getEntry.GetValue<UIResAmountEntry>();
                        ;
                        __instance.entries.Add(entry);
                        entry.SetInfo(num7, itemProto.name, veinProto.iconSprite, veinProto.description, false, false, "                ");
                        entry.refId = id;
                        ++num7;
                    }
                }

                var waterItemId = __instance.planet.waterItemId;
                Sprite icon = null;
                var str = "无".Translate();
                if (waterItemId < 0) str = waterItemId != -1 ? "未知".Translate() : "熔岩".Translate();

                var itemProto1 = LDB.items.Select(waterItemId);
                if (itemProto1 != null)
                {
                    icon = itemProto1.iconSprite;
                    str = itemProto1.name;
                }

                var entry1 = getEntry.GetValue<UIResAmountEntry>();
                ;
                __instance.entries.Add(entry1);
                entry1.SetInfo(num7, "海洋类型".Translate(), icon, itemProto1 == null ? string.Empty : itemProto1.description, false, itemProto1 != null && waterItemId != 1000, string.Empty);
                entry1.valueString = str;
                var index1 = num7 + 1;
                var entry2 = getEntry.GetValue<UIResAmountEntry>();
                ;
                __instance.entries.Add(entry2);
                entry2.SetInfo(index1, "适建区域".Translate(), ___sprite6, string.Empty, false, false, "      %");
                if (__instance.planet.landPercentDirty)
                {
                    PlanetAlgorithm.CalcLandPercent(__instance.planet);
                    __instance.planet.landPercentDirty = false;
                }

                StringBuilderUtility.WritePositiveFloat(entry2.sb, 0, 5, __instance.planet.landPercent * 100f, 1);
                entry2.DisplayStringBuilder();
                var index2 = index1 + 1;
                var entry3 = getEntry.GetValue<UIResAmountEntry>();
                ;
                __instance.entries.Add(entry3);
                entry3.SetInfo(index2, "风能利用率".Translate(), ___sprite8, string.Empty, false, __instance.planet.windStrength > 1.49899995326996, "      %");
                StringBuilderUtility.WriteUInt(entry3.sb, 0, 5, (uint)(__instance.planet.windStrength * 100.0 + 0.499900013208389));
                entry3.DisplayStringBuilder();
                var index3 = index2 + 1;
                var entry4 = getEntry.GetValue<UIResAmountEntry>();
                ;
                __instance.entries.Add(entry4);
                entry4.SetInfo(index3, "光能利用率".Translate(), ___sprite9, string.Empty, false, __instance.planet.luminosity > 1.49899995326996, "      %");
                StringBuilderUtility.WriteUInt(entry4.sb, 0, 5, (uint)(__instance.planet.luminosity * 100.0 + 0.499900013208389));
                entry4.DisplayStringBuilder();
                num7 = index3 + 1;
                for (var index4 = 7; index4 < 15; ++index4)
                {
                    var id = index4;
                    var veinProto = LDB.veins.Select(id);
                    var itemProto2 = LDB.items.Select(veinProto.MiningItem);
                    if (veinProto != null && itemProto2 != null && __instance.planet.veinAmounts[id] > 0L)
                    {
                        var entry5 = getEntry.GetValue<UIResAmountEntry>();
                        ;
                        __instance.entries.Add(entry5);
                        entry5.SetInfo(num7, itemProto2.name, veinProto.iconSprite, veinProto.description, true, false, index4 != 7 ? "                " : "         /s");
                        entry5.refId = id;
                        ++num7;
                    }
                }

                if (!_observed)
                {
                    var entry5 = getEntry.GetValue<UIResAmountEntry>();
                    ;
                    __instance.entries.Add(entry5);
                    entry5.SetInfo(num7, string.Empty, null, string.Empty, false, false, string.Empty);
                    ___tipEntry = entry5;
                    ++num7;
                }
            }
            else
            {
                for (var index = 0; index < __instance.planet.gasItems.Length; ++index)
                {
                    var itemProto = LDB.items.Select(__instance.planet.gasItems[index]);
                    if (itemProto != null)
                    {
                        var entry = getEntry.GetValue<UIResAmountEntry>();
                        ;
                        __instance.entries.Add(entry);
                        if (_observed)
                        {
                            entry.SetInfo(num7, "可采集".Translate() + itemProto.name, itemProto.iconSprite, "环绕行星手动采集".Translate(), false, false, "        /s");
                            StringBuilderUtility.WritePositiveFloat(entry.sb, 0, 7, __instance.planet.gasSpeeds[index]);
                            entry.DisplayStringBuilder();
                        }
                        else
                        {
                            entry.SetInfo(num7, "未知".Translate(), ___unknownResIcon, "环绕行星手动采集".Translate(), false, false, "        /s");
                            entry.valueString = "未知".Translate();
                        }

                        entry.SetObserved(_observed);
                        ++num7;
                    }
                }
            }

            __instance.SetResCount(num7);
            __instance.RefreshDynamicProperties();
            return false;
        }
    }
}