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
            UIRoot.instance.uiGame.starDetail.gameObject.SetActive(false);
            
            
            if (!GS2.Vanilla && !NebulaCompat.IsMultiplayerActive) SystemDisplay.ResetView();
            

            
            if (GS2.Config.SkipPrologue && !NebulaCompat.IsMultiplayerActive)
            {
                GS2.Warn("Starting Game, Skipping Prologue.");
                DSPGame.StartGameSkipPrologue(___gameDesc);
            }
            else if (!NebulaCompat.IsMultiplayerActive)
            {
                __instance.level = (EGoalLevel)_data;
                if (__instance.galaxySelect.active)
                {
                    __instance.gameDesc.goalLevel = __instance.level;
                    if (UIGalaxySelect.isPlayCutScene)
                    {
                        UIRoot.instance.StartCombatCutscene(__instance.gameDesc);
                        return false;
                    }
                    DSPGame.StartGame(__instance.gameDesc);
                    return false;
                }
                else
                {
                    if (__instance.loadGameWindow.active)
                    {
                        __instance.loadGameWindow.loadWithGoalLevel = __instance.level;
                        __instance.CloseSettingWindow();
                        return false;
                    }
                    if (GameMain.data != null)
                    {
                        if (GameMain.data.gameDesc.goalLevel != __instance.level)
                        {
                            GameMain.data.gameDesc.goalLevel = __instance.level;
                            GameMain.gameScenario.goalLogic.NotifyOnGoalLevelChanged();
                            if (UIRoot.instance.uiGame.goalPanel.inited)
                            {
                                UIRoot.instance.uiGame.goalPanel.Reset();
                            }
                        }
                        if (GameMain.isPaused)
                        {
                            GameMain.Resume();
                        }
                        __instance.CloseSettingWindow();
                    }
                    return false;
                }
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
                    NEB.Hack.EnterGame(UIRoot._instance.galaxySelect);
                }
            }

            return false;
        }
    }
}