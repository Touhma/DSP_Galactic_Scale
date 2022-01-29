using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale

{

    public class PatchOnWhatever
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "AddHeightMapModLevel")]
        public static bool AddHeightMapModLevel(int index, int level, PlanetData __instance)
        {
            if (__instance.data.AddModLevel(index, level))
            {
                int num = __instance.precision / __instance.segment;
                int num2 = index % __instance.data.stride;
                int num3 = index / __instance.data.stride;
                int num4 = ((num2 < __instance.data.substride) ? 0 : 1) + ((num3 < __instance.data.substride) ? 0 : 2);
                int num5 = num2 % __instance.data.substride;
                int num6 = num3 % __instance.data.substride;
                int num7 = (num5 - 1) / num;
                int num8 = (num6 - 1) / num;
                int num9 = num5 / num;
                int num10 = num6 / num;
                if (num9 >= __instance.segment)
                {
                    num9 = __instance.segment - 1;
                }
                if (num10 >= __instance.segment)
                {
                    num10 = __instance.segment - 1;
                }
                int num11 = num4 * __instance.segment * __instance.segment;
                int num12 = num7 + num8 * __instance.segment + num11;
                int num13 = num9 + num8 * __instance.segment + num11;
                int num14 = num7 + num10 * __instance.segment + num11;
                int num15 = num9 + num10 * __instance.segment + num11;
                num12 = Mathf.Clamp(num12, 0, 99);
                num13 = Mathf.Clamp(num13, 0, 99);
                num14 = Mathf.Clamp(num14, 0, 99);
                num15 = Mathf.Clamp(num15, 0, 99);
                __instance.dirtyFlags[num12] = true;
                __instance.dirtyFlags[num13] = true;
                __instance.dirtyFlags[num14] = true;
                __instance.dirtyFlags[num15] = true;
            }

            return false;
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(BuildTool_Inserter), "CheckBuildConditions")]
        public static void BuildToolInserter(BuildTool_Inserter __instance, ref bool __result)
        {
            if (__instance.buildPreviews.Count == 0)
            {
                return;
            }
            // if (__instance.buildPreviews == null) return;
            var preview = __instance.buildPreviews[0];
            // GS2.Warn(preview?.condition.ToString());
            
            if (__instance.planet.realRadius < 20)
            {
                if (preview.condition == EBuildCondition.TooSkew)
                {
                    preview.condition = EBuildCondition.Ok;
                    // GS2.Warn("TooSkew");
                    __instance.cursorValid = true; // Prevent red text
                    __result = true; // Override the build condition check
                    UICursor.SetCursor(ECursor.Default); // Get rid of that ban cursor
                    __instance.actionBuild.model.cursorText = "Click to build";
                    __instance.actionBuild.model.cursorState = 0;
                }
            } 
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(UILoadGameWindow), "_OnOpen")]
        public static bool UILoadGameWindow_OnOpen()
        {
            GS2.Warn("Disabled Import");
            GS2.SaveOrLoadWindowOpen = true;
            return true;

        }

        [HarmonyPrefix, HarmonyPatch(typeof(UILoadGameWindow), "LoadSelectedGame")]
        public static bool UILoadGameWindow_LoadSelectedGame()
        {
            GS2.Warn("Enabled Import");
            GS2.SaveOrLoadWindowOpen = false;
            return true;

        }

        [HarmonyPrefix, HarmonyPatch(typeof(UILoadGameWindow), "_OnClose")]
        public static bool UILoadGameWindow_OnClose()
        {
            GS2.Warn("Enabled Import");

            GS2.SaveOrLoadWindowOpen = false;
            return true;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(UISaveGameWindow), "_OnOpen")]
        public static bool UISaveGameWindow_OnOpen()
        {
            GS2.Warn("Disabled Import");

            GS2.SaveOrLoadWindowOpen = true;
            return true;

        }


        [HarmonyPrefix, HarmonyPatch(typeof(UISaveGameWindow), "_OnClose")]
        public static bool UISaveGameWindow_OnClose()
        {
            GS2.Warn("Enabled Import");

            GS2.SaveOrLoadWindowOpen = false;
            return true;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(UIAchievementPanel), "LoadData")]
        public static bool LoadData(UIAchievementPanel __instance)
        {
            // __instance.unlockedEntries.Clear();
            // __instance.lockedEntries.Clear();
            // __instance.inProgressEntries.Clear();
            __instance.uiEntries.Clear();
            // foreach (KeyValuePair<int, AchievementState> keyValuePair in DSPGame.achievementSystem.achievements)
            // {
            //     if (keyValuePair.Value.unlocked)
            //     {
            //         UIAchievementEntry uiachievementEntry = UnityEngine.Object.Instantiate<UIAchievementEntry>(__instance.entryPrefab, __instance.unlockedContainerRect);
            //         uiachievementEntry._Create();
            //         uiachievementEntry._Init(null);
            //         uiachievementEntry.SetAchievementData(keyValuePair.Value.id);
            //         if (!__instance.unlockedEntries.Contains(uiachievementEntry)) __instance.unlockedEntries.Add(uiachievementEntry);
            //         uiachievementEntry.index = __instance.unlockedEntries.IndexOf(uiachievementEntry);
            //         __instance.uiEntries.Add(keyValuePair.Key, uiachievementEntry);
            //         uiachievementEntry._Open();
            //     }
            //     else if (keyValuePair.Value.targetValue > 1L && keyValuePair.Value.progressValue > 0L)
            //     {
            //         UIAchievementEntry uiachievementEntry2 = UnityEngine.Object.Instantiate<UIAchievementEntry>(__instance.entryPrefab, __instance.inProgressContainerRect);
            //         uiachievementEntry2._Create();
            //         uiachievementEntry2._Init(null);
            //         uiachievementEntry2.SetAchievementData(keyValuePair.Value.id);
            //         __instance.inProgressEntries.Add(uiachievementEntry2);
            //         uiachievementEntry2.index = __instance.inProgressEntries.IndexOf(uiachievementEntry2);
            //         __instance.uiEntries.Add(keyValuePair.Key, uiachievementEntry2);
            //         uiachievementEntry2._Open();
            //     }
            //     else
            //     {
            //         UIAchievementEntry uiachievementEntry3 = UnityEngine.Object.Instantiate<UIAchievementEntry>(__instance.entryPrefab, __instance.lockedContainerRect);
            //         uiachievementEntry3._Create();
            //         uiachievementEntry3._Init(null);
            //         uiachievementEntry3.SetAchievementData(keyValuePair.Value.id);
            //         __instance.lockedEntries.Add(uiachievementEntry3);
            //         uiachievementEntry3.index = __instance.lockedEntries.IndexOf(uiachievementEntry3);
            //         __instance.uiEntries.Add(keyValuePair.Key, uiachievementEntry3);
            //         uiachievementEntry3._Open();
            //     }
            // }
            // __instance.inProgressEntries.Sort(new AchievementProgressComparer());
            // foreach (UIAchievementEntry uiachievementEntry4 in __instance.inProgressEntries)
            // {
            //     uiachievementEntry4.index = __instance.inProgressEntries.IndexOf(uiachievementEntry4);
            // }

            return true;
        }

        // [HarmonyPrefix, HarmonyPatch(typeof(VegeRenderer), "AddInst")]
        // public static bool AddInst(VegeRenderer __instance, int __result, int objId, Vector3 pos, Quaternion rot, bool setBuffer = true)
        // {
        //     return false;
        // }
        
        [HarmonyPostfix, HarmonyPatch(typeof(WarningSystem), "Init")]
        public static void Init(ref WarningSystem __instance)
        {
            GS2.Warn("Warning System Initializing");
            GS2.Warn($"Star Count: {GSSettings.StarCount}");
            var planetCount = GSSettings.PlanetCount;
            GS2.Warn($"Planet Count: {planetCount}");
            GS2.Warn($"Factory Length: {__instance.gameData.factories.Length}");
            if (__instance.gameData.factories.Length > planetCount) planetCount = __instance.gameData.factories.Length;
            __instance.tmpEntityPools = new EntityData[planetCount][];
            __instance.tmpPrebuildPools = new PrebuildData[planetCount][];
            __instance.tmpSignPools = new SignData[planetCount][];
            __instance.warningCounts = new int[GameMain.galaxy.starCount * 1024];
            __instance.warningSignals = new int[GameMain.galaxy.starCount * 32];
            __instance.focusDetailCounts = new int[GameMain.galaxy.starCount * 1024];
            __instance.focusDetailSignals = new int[GameMain.galaxy.starCount * 32];
            var l = GameMain.galaxy.starCount * 400;
            __instance.astroArr = new AstroPoseR[l];
            __instance.astroBuffer = new ComputeBuffer(l, 32, ComputeBufferType.Default);
            GS2.Warn($"Pool Length: {__instance.tmpEntityPools.Length}");
        }
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
        // [HarmonyPrefix, HarmonyPatch(typeof(LandPlanetCountCondition), "Check")]
        // public static bool Check(ref LandPlanetCountCondition __instance, ref bool __result)
        // {
        //     int num = 0;
        //     int num2 = 0;
        //     for (int i = 0; i < __instance.landTypeArr.Length; i++)
        //     {
        //         __instance.landTypeArr[i] = 0;
        //     }
        //     StarData[] stars = __instance.gameData.galaxy.stars;
        //     for (int j = 0; j < stars.Length; j++)
        //     {
        //         if (stars[j] != null)
        //         {
        //             PlanetData[] planets = stars[j].planets;
        //             for (int k = 0; k < planets.Length; k++)
        //             {
        //                 GS2.Log($"Checking Planet {k} {planets[k].name} with theme {planets[k].theme}");
        //                 if (planets[k] != null && planets[k].factory != null && planets[k].factory.landed)
        //                 {
        //                     num++;
        //                     if (__instance.landTypeArr[planets[k].theme] == 0)
        //                     {
        //                         GS2.Log($"theme == 0");
        //                         __instance.landTypeArr[planets[k].theme] = 1;
        //                         num2++;
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     GS2.Log($"Finshed That part. Setting Refarances");
        //     __instance.trigger.SetReferances(num, num2);
        //     GS2.Log("Returning");
        //     __result = Utility.Compare(num, __instance.count1, __instance.c1) && Utility.Compare(num2, __instance.count2, __instance.c2);
        //     return false;
        // }
        //[HarmonyPrefix, HarmonyPatch(typeof(LandPlanetCountCondition), "OnCreate")]
        //public static bool OnCreate(ref LandPlanetCountCondition __instance)
        //{
        //    __instance.c1 = Utility.ToCompare(__instance.parameters["c1"]);
        //    __instance.count1 = Utility.ToInt(__instance.parameters["count1"]);
        //    __instance.c2 = Utility.ToCompare(__instance.parameters["c2"]);
        //    __instance.count2 = Utility.ToInt(__instance.parameters["count2"]);
        //    if (__instance.landTypeArr == null)
        //    {
        //        __instance.landTypeArr = new byte[512]; //Also in galaxy generation the LDB.themes array is truncated to 128, leaving 384 theme slots avail. if DSP adds more themes this will need to be tweaked
        //    }

        //    return false;
        //}
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