using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale

{
    public class PatchOnWhatever
    {
        [HarmonyPrefix, HarmonyPatch(typeof(ThemeProto), "Preload")]
        public static bool Preload(ref ThemeProto __instance)
        {
            __instance.displayName = __instance.DisplayName.Translate();
            __instance.terrainMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "terrain", "{0}-{1}", true);
            __instance.oceanMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "ocean", "{0}-{1}", true);
            __instance.atmosMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "atmosphere", "{0}-{1}", true);
            __instance.lowMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "low", "{0}-{1}", true);
            __instance.thumbMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "thumb", "{0}-{1}", true);
            __instance.minimapMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "minimap", "{0}-{1}", true);
            __instance.ambientDesc = Utils.ResourcesLoadArray<AmbientDesc>(__instance.MaterialPath + "ambient", "{0}-{1}", true);
            __instance.ambientSfx = Utils.ResourcesLoadArray<AudioClip>(__instance.SFXPath, "{0}-{1}", true);
            if (__instance.RareSettings.Length != __instance.RareVeins.Length * 4)
            {
                Debug.LogError("稀有矿物数组长度有误 " + __instance.displayName);
            }
            return false;
        }

        //[HarmonyPrefix, HarmonyPatch(typeof(CommonUtils), "ResourcesLoadArray")]
        //public static bool ResourcesLoadArray<T>(ref T[] __result, string path, string format, bool emptyNull) where T : UnityEngine.Object
        //{
        //    List<T> list = new List<T>();
            
        //    T t = Resources.Load<T>(path);
        //    if (t == null)
        //    {
        //        GS2.Log("Resource returned null, exiting");
        //        __result = null;
        //        return false;
        //    }
        //    GS2.Log("Resource loaded");
        //    int num = 0;
        //    if (t != null)
        //    {
        //        list.Add(t);
        //        num = 1;
        //    }
        //    do
        //    {
        //        t = Resources.Load<T>(string.Format(format, path, num));
        //        if (t == null || ((num == 1 || num == 2) && list.Contains(t)))
        //        {
        //            break;
        //        }
        //        list.Add(t);
        //        num++;
        //    }
        //    while (num < 1024);
        //    if (emptyNull && list.Count == 0)
        //    {
        //        __result = null;
        //        return false;
        //    }
        //    __result = list.ToArray();
        //    return false;
        //}
        //[HarmonyPatch(typeof(WorkerThreadExecutor), "InserterPartExecute")]
        //[HarmonyPrefix]
        //public static bool InserterPartExecute(ref WorkerThreadExecutor __instance)
        //{
        //    if (__instance.inserterFactories == null) return true;
        //    for (var i=0;i<__instance.inserterFactoryCnt;i++)
        //    {

        //        if (__instance.inserterFactories[i].factorySystem == null)
        //        {
        //            Warn("Creating Factory");
        //            __instance.inserterFactories[i] = GameMain.data.GetOrCreateFactory(__instance.inserterFactories[i].planet);
        //        }
        //    }
        //    return true;
        //}
        [HarmonyPatch(typeof(UIReplicatorWindow), "OnPlusButtonClick")]
        [HarmonyPrefix]
        public static bool OnPlusButtonClick(ref UIReplicatorWindow __instance, int whatever)
        {
            // GS2.Log("Test");
            if (__instance.selectedRecipe != null)
            {
                if (!__instance.multipliers.ContainsKey(__instance.selectedRecipe.ID)) __instance.multipliers[__instance.selectedRecipe.ID] = 1;

                var num = __instance.multipliers[__instance.selectedRecipe.ID];
                if (VFInput.control) num += 10;
                else if (VFInput.shift) num += 100;
                else if (VFInput.alt) num = 999;
                else num++;
                if (num > 999) num = 999;

                __instance.multipliers[__instance.selectedRecipe.ID] = num;
                __instance.multiValueText.text = num + "x";
            }

            return false;
        }

        [HarmonyPatch(typeof(UIReplicatorWindow), "OnMinusButtonClick")]
        [HarmonyPrefix]
        public static bool OnMinusButtonClick(ref UIReplicatorWindow __instance, int whatever)
        {
            if (__instance.selectedRecipe != null)
            {
                if (!__instance.multipliers.ContainsKey(__instance.selectedRecipe.ID)) __instance.multipliers[__instance.selectedRecipe.ID] = 1;
                var num = __instance.multipliers[__instance.selectedRecipe.ID];
                if (VFInput.control) num -= 10;
                else if (VFInput.shift) num -= 100;
                else if (VFInput.alt) num = 1;
                else num--;
                if (num < 1) num = 1;
                __instance.multipliers[__instance.selectedRecipe.ID] = num;
                __instance.multiValueText.text = num + "x";
            }


            return false;
        }

        [HarmonyPatch(typeof(UIReplicatorWindow), "OnOkButtonClick")]
        [HarmonyPrefix]
        public static bool OnOkButtonClick(ref UIReplicatorWindow __instance, int whatever, bool button_enable)
        {
            // GS2.Log("Test2");
            if (__instance.selectedRecipe != null)
            {
                if (!__instance.selectedRecipe.Handcraft)
                {
                    UIRealtimeTip.Popup("该配方".Translate() + __instance.selectedRecipe.madeFromString + "生产".Translate());
                    return false;
                }

                var id = __instance.selectedRecipe.ID;
                if (!GameMain.history.RecipeUnlocked(id))
                {
                    UIRealtimeTip.Popup("配方未解锁".Translate());
                    return false;
                }

                var num = 1;
                if (__instance.multipliers.ContainsKey(id)) num = __instance.multipliers[id];

                if (num < 1)
                    num = 1;
                else if (num > 999) num = 1000;

                var num2 = __instance.mechaForge.PredictTaskCount(__instance.selectedRecipe.ID, 999);
                // GS2.Log($"{num} - {num2}");
                if (num > num2) num = num2;

                if (num == 0)
                {
                    UIRealtimeTip.Popup("材料不足".Translate());
                    return false;
                }

                if (__instance.mechaForge.AddTask(id, num) == null)
                {
                    UIRealtimeTip.Popup("材料不足".Translate());
                    return false;
                }

                GameMain.history.RegFeatureKey(1000104);
            }

            return false;
        }
    }
}