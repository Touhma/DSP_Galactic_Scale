using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using static GalacticScale.GS2;

namespace GalacticScale

{
    public class PatchOnWhatever
    {
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
            GS2.Log("Test");
            if (__instance.selectedRecipe != null)
            {
                if (!__instance.multipliers.ContainsKey(__instance.selectedRecipe.ID))
                {
                    __instance.multipliers[__instance.selectedRecipe.ID] = 1;
                }

                int num = __instance.multipliers[__instance.selectedRecipe.ID];
                if (VFInput.shift) num += 10;
                else if (VFInput.control) num += 100;
                else num++;
                if (num > 999)
                {
                    num = 1000;
                }

                __instance.multipliers[__instance.selectedRecipe.ID] = num;
                __instance.multiValueText.text = num.ToString() + "x";
            }

            return false;
        }

        [HarmonyPatch(typeof(UIReplicatorWindow), "OnOkButtonClick")]
        [HarmonyPrefix]
        public static bool OnOkButtonClick(ref UIReplicatorWindow __instance, int whatever, bool button_enable)
        {
            GS2.Log("Test2");
            if (__instance.selectedRecipe != null)
            {
                if (!__instance.selectedRecipe.Handcraft)
                {
                    UIRealtimeTip.Popup("该配方".Translate() + __instance.selectedRecipe.madeFromString + "生产".Translate(), true, 0);
                    return false;
                }

                int id = __instance.selectedRecipe.ID;
                if (!GameMain.history.RecipeUnlocked(id))
                {
                    UIRealtimeTip.Popup("配方未解锁".Translate(), true, 0);
                    return false;
                }

                int num = 1;
                if (__instance.multipliers.ContainsKey(id))
                {
                    num = __instance.multipliers[id];
                }

                if (num < 1)
                {
                    num = 1;
                }
                else if (num > 999)
                {
                    num = 1000;
                }

                int num2 = __instance.mechaForge.PredictTaskCount(__instance.selectedRecipe.ID, 99);
                GS2.Log($"{num} - {num2}");
                if (num > num2)
                {
                    num = num2;
                }

                if (num == 0)
                {
                    UIRealtimeTip.Popup("材料不足".Translate(), true, 0);
                    return false;
                }

                if (__instance.mechaForge.AddTask(id, num) == null)
                {
                    UIRealtimeTip.Popup("材料不足".Translate(), true, 0);
                    return false;
                }

                GameMain.history.RegFeatureKey(1000104);
                
            }return false;
        }







}
}