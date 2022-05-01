using HarmonyLib;

namespace GalacticScale
{
    /// <summary>
    ///     Add a bunch of Null Checks to stop the game from completely crashing when galaxy generation fails.
    /// </summary>
    public class PatchOnGameLoader
    {
        // [HarmonyPostfix, HarmonyPatch(typeof(GameLoader), "CreateLoader")]
        // public static void CreateLoader(GameLoader __instance)
        // {
        //     GS2.Warn($"Creating Loader Instance");
        // }
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
                // GS2.Warn($"FRAME 5 {GameMain.mainPlayer != null} {GameMain.gameTick == 0L} {GS2.Config.SkipPrologue}");
                // GS2.Warn($"{GameMain.localStar?.name}");
                if (GameMain.mainPlayer != null && GameMain.gameTick == 0L && GS2.Config.SkipPrologue)
                {
                    //GS2.Warn($"Setting uPosition");
                    GameMain.mainPlayer.uPosition = GameMain.localPlanet.uPosition;
                }
                else
                {
                    //GS2.Warn($" Didnt Set uPosition: MainPlayer null:{GameMain.mainPlayer == null} Existing Game:{GameMain.gameTick} Skip Prologue:{GS2.Config.SkipPrologue}");


                }
                GS2.Warn($"{GameMain.localPlanet == null}");
                GameMain.localPlanet?.Load();
                GameMain.localStar?.Load();
                
            }

            if (__instance.frame >= 7)
            {
                //GS2.Warn("FRAME 7");
                // if (!GS2.Config.SkipPrologue) if (GameMain.localStar != null && !GameMain.localStar.loaded) __instance.frame = 7;
                //     else
                //     {
                if (GameMain.localPlanet != null && !GameMain.localPlanet.loaded) __instance.frame = 7;
                        if (GameMain.localPlanet != null && !GameMain.localPlanet.factoryLoaded) __instance.frame = 7;
                    // }
                
            }

            if (__instance.frame == 8)
            {
                // GS2.Warn($"FRAME 8 {__instance.gameObject.GetInstanceID()}");
                if (GameMain.gameTick == 0L)
                {
                    if (GameMain.data == null)
                    {
                        GS2.Warn("GameMain.data null");
                        return false;
                    }

                    if (DSPGame.SkipPrologue)
                        GameMain.data.SkipStandardModeGuide();
                    else
                    {
                        GS2.Warn("Starting standard mode guide");
                        GameMain.data.StartStandardModeGuide();
                    }
                }
                // GS2.Warn("FRAME 8.5");

                GameMain.data.SetReady();
                if (GameCamera.instance == null)
                {
                    // GS2.Warn("Camera null setting frame 9");
                    __instance.frame = 9;
                    return false;
                }
                GameCamera.instance.SetReady();
                
                GameMain.preferences.LateRestore();
                // GS2.Warn("All good, Setting Frame 10");
                __instance.frame = 10;
            }

            if (__instance.frame == 9)
            {
                // GS2.Warn("Frame 9");
                if (GameCamera.instance == null)
                {
                    // GS2.Warn("Camera null, setting 9");
                    __instance.frame = 9;
                    return false;
                }

                // GS2.Warn("All good on frame 9");
                GameCamera.instance.SetReady();
                GameMain.preferences.LateRestore();
            }
            if (__instance.frame == 10)
            {
                // GS2.Warn($"FRAME 10 {!GameMain.instance?.isMenuDemo} {__instance.GetInstanceID()}");
                GameMain.Begin();
                __instance.SelfDestroy();
                if (!GameMain.instance.isMenuDemo)
                {
                    var str = "";
                    for (int i = GameMain.data.patch + 1; i <= 6; i++)
                    {
                        if (i != 3 && i != 4 && i != 5 && i != 6)
                        {
                            str = str + "\r\n" + ("存档补丁提示" + i.ToString()).Translate();
                        }
                    }

                    if (!string.IsNullOrEmpty(str))
                    {
                        UIMessageBox.Show("存档修复标题".Translate(), "存档修复提示".Translate() + "\r\n" + str, "确定".Translate(), 0);
                    }
                }

                GameMain.data.patch = 3;
            }
            //GS2.Warn("Increasing Frame");
            __instance.frame++;
            return false;
        }
    }
}