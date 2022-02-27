using HarmonyLib;

namespace GalacticScale
{
    /// <summary>
    ///     Add a bunch of Null Checks to stop the game from completely crashing when galaxy generation fails.
    /// </summary>
    public class PatchOnGameLoader
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameLoader), "FixedUpdate")]
        public static bool FixedUpdate(ref GameLoader __instance)
        {
            // GS2.Warn($"Start {__instance.frame}");
            if (GS2.IsMenuDemo || GS2.Vanilla) return true;
            // GS2.Warn("Not Vanilla");
            if (__instance.frame == 1)
            {
                // GS2.Warn("FRAME 1");
                DSPGame.CreateGameMainObject();
                DSPGame.Game.isMenuDemo = DSPGame.IsMenuDemo;
            }

            // GS2.Warn(".");
            if (__instance.frame == 3 && GameMain.isNull)
            {
                // GS2.Warn("FRAME 3");
                __instance.LoadFailed();
                __instance.frame = 3;
            }
            // GS2.Warn(".");
            if (__instance.frame == 5 && GameMain.localStar != null)
            {
                // GS2.Warn($"FRAME 5");
                // GS2.Warn($"{GameMain.localStar?.name}");
                if (GameMain.mainPlayer != null && GameMain.gameTick == 0L)
                {
                    GS2.Warn($"Setting uPosition");
                    GameMain.mainPlayer.uPosition = GameMain.localPlanet.uPosition;
                }
                else
                {
                    GS2.Warn("MainPlayer null or existing game");
                    
                }
                GameMain.localPlanet?.Load();
                GameMain.localStar?.Load();
                
            }

            if (__instance.frame >= 7)
            {
                // GS2.Warn("FRAME 7");
                // if (GameMain.localStar != null && !GameMain.localStar.loaded) __instance.frame = 7;
                if (GameMain.localPlanet != null && !GameMain.localPlanet.loaded) __instance.frame = 7;
                if (GameMain.localPlanet != null && !GameMain.localPlanet.factoryLoaded) __instance.frame = 7;
            }

            if (__instance.frame == 9)
            {
                GS2.Warn("FRAME 9");
                if (GameMain.gameTick == 0L)
                {
                    if (GameMain.data == null) return false;

                    if (DSPGame.SkipPrologue)
                        GameMain.data.SkipStandardModeGuide();
                    else
                        GameMain.data.StartStandardModeGuide();
                }
                // GS2.Warn("FRAME x");

                GameMain.data.SetReady();
                if (GameCamera.instance == null) return false;
                GameMain.preferences.LateRestore();
                GameCamera.instance.SetReady();
            }

            if (__instance.frame == 10)
            {
                // GS2.Warn($"FRAME 10 {!GameMain.instance?.isMenuDemo} {__instance.GetInstanceID()}");
                GameMain.Begin();
                __instance.SelfDestroy();
                if (!GameMain.instance.isMenuDemo)
                {
                    var str = "";
                    for (var index = GameMain.data.patch + 1; index <= 3; ++index)
                        str = str + "\r\n" + ("存档补丁提示" + index).Translate();

                    if (!string.IsNullOrEmpty(str))
                        UIMessageBox.Show("存档修复标题".Translate(), "存档修复提示".Translate() + "\r\n" + str, "确定".Translate(), 0);
                }

                GameMain.data.patch = 3;
            }
            // GS2.Warn("Increasing Frame");
            __instance.frame++;
            return false;
        }
    }
}