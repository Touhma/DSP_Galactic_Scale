using HarmonyLib;
using NebulaAPI;
using NebulaCompatibility;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), "EnterGame")]
        public static bool EnterGame(ref GameDesc ___gameDesc, ref UIGalaxySelect __instance)
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
                bool advisorTips = DSPGame.globalOption.advisorTips;
                DSPGame.globalOption.advisorTips =
                    (__instance.uiCombat.advisorEnabled ?? DSPGame.globalOption.advisorTips);
                if (advisorTips != DSPGame.globalOption.advisorTips)
                {
                    UIAdvisorTip.showAdvisorTips = DSPGame.globalOption.advisorTips;
                    DSPGame.globalOption.SaveGlobal();
                }

                if (__instance.gameDesc.isCombatMode)
                {
                    DSPGame.RecordPlayCombat();
                }

                DSPGame.RecordPlayVersion();
                __instance.uiCombat.advisorEnabled = null;
                if (UIGalaxySelect.isPlayCutScene)
                {
                    UIRoot.instance.StartCombatCutscene(__instance.gameDesc);
                    return false;
                }

                DSPGame.StartGame(___gameDesc);
            }
            else
            {
                //Nebula
                if (NebulaCompat.IsMultiplayerActive)
                {
                    NEB.Hack.EnterGame(UIRoot._instance.galaxySelect);
                }
            }

            return false;
        }
    }
}