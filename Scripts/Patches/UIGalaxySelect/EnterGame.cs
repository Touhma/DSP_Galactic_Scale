using HarmonyLib;
using NebulaAPI;
using NebulaCompatibility;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGoalSetting), nameof(UIGoalSetting.OnGoalButtonClick))]
        public static bool EnterGame(ref GameDesc ___gameDesc, ref UIGoalSetting __instance, int _data)
        {
            GS2.Warn("Entergame...");
            
            
            
            UIRoot.instance.uiGame.planetDetail.gameObject.SetActive(false);
            // GS2.Warn("2");
            UIRoot.instance.uiGame.starDetail.gameObject.SetActive(false);
            // GS2.Warn("3");
            
            if (!GS2.Vanilla && !NebulaCompat.IsMultiplayerActive) SystemDisplay.ResetView();
            
            // GS2.Warn("4");
            
            if (GS2.Config.SkipPrologue && !NebulaCompat.IsMultiplayerActive)
            {
                // GS2.Warn("5");
                GS2.Warn("Starting Game, Skipping Prologue.");
                DSPGame.StartGameSkipPrologue(___gameDesc);
                // GS2.Warn("6");
            }
            else if (!NebulaCompat.IsMultiplayerActive)
            {
                // GS2.Warn("7");
                __instance.level = (EGoalLevel)_data;
                // GS2.Warn("8");
                if (__instance.galaxySelect.active)
                {
                    // GS2.Warn("9");
                    // GS2.Warn($"__instance.gamedesc {__instance.gameDesc == null}");
                    // GS2.Warn($"goalLevel {__instance.gameDesc?.goalLevel == null}");
                    // GS2.Warn($"instance.level {__instance.level}");
                    __instance.gameDesc.goalLevel = __instance.level;
                    // GS2.Warn("10");
                    if (UIGalaxySelect.isPlayCutScene)
                    {
                        // GS2.Warn("11");
                        UIRoot.instance.StartCombatCutscene(__instance.gameDesc);
                        // GS2.Warn("12");
                        return false;
                    }
                    // GS2.Warn("13");
                    DSPGame.StartGame(__instance.gameDesc);
                    return false;
                }

                if (__instance.loadGameWindow.active)
                {
                    // GS2.Warn("14");
                    __instance.loadGameWindow.loadWithGoalLevel = __instance.level;
                    // GS2.Warn("15");
                    __instance.CloseSettingWindow();
                    // GS2.Warn("16");
                    return false;
                }
                if (GameMain.data != null)
                {
                    // GS2.Warn("17");
                    if (GameMain.data.gameDesc.goalLevel != __instance.level)
                    {
                        // GS2.Warn("18");
                        GameMain.data.gameDesc.goalLevel = __instance.level;
                        // GS2.Warn("19");
                        GameMain.gameScenario.goalLogic.NotifyOnGoalLevelChanged();
                        // GS2.Warn("20");
                        if (UIRoot.instance.uiGame.goalPanel.inited)
                        {
                            // GS2.Warn("21");
                            UIRoot.instance.uiGame.goalPanel.Reset();
                            // GS2.Warn("22");
                        }
                    }
                    // GS2.Warn("23");
                    if (GameMain.isPaused)
                    {
                        // GS2.Warn("24");
                        GameMain.Resume();
                    }
                    // GS2.Warn("25");
                    __instance.CloseSettingWindow();
                }
                // GS2.Warn("26");
                return false;
                // bool advisorTips = DSPGame.globalOption.advisorTips;
                // DSPGame.globalOption.advisorTips = (__instance.uiCombat.advisorEnabled ?? DSPGame.globalOption.advisorTips);
                // if (advisorTips != DSPGame.globalOption.advisorTips)
                // {
                //     UIAdvisorTip.showAdvisorTips = DSPGame.globalOption.advisorTips;
                //     DSPGame.globalOption.SaveGlobal();
                // }
                //
                // if (__instance.gameDesc.isCombatMode)
                // {
                //     DSPGame.RecordPlayCombat();
                // }
                //
                // DSPGame.RecordPlayVersion();
                // __instance.uiCombat.advisorEnabled = null;
                // if (UIGalaxySelect.isPlayCutScene)
                // {
                //     UIRoot.instance.StartCombatCutscene(__instance.gameDesc);
                //     return false;
                // }
                //
                // DSPGame.StartGame(___gameDesc);
                
            }
            else
            {
                //Nebula PROBABLY NEEDS FIXING AS OF 0.10.31.24646
                if (NebulaCompat.IsMultiplayerActive)
                {
                    // GS2.Warn("27");
                    NEB.Hack.EnterGame(UIRoot._instance.galaxySelect);
                    // GS2.Warn("28");
                }
            }
            // GS2.Warn("29");
            return false;
        }
    }
}