using HarmonyLib;

namespace GalacticScale.Patches
{
    /// <summary>
    ///     Add a bunch of Null Checks to stop the game from completely crashing when galaxy generation fails.
    /// </summary>
    public class PatchOnGameLoader
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameLoader), nameof(GameLoader.FixedUpdate))]
        public static bool FixedUpdate(ref GameLoader __instance)
        {
            // GS3.Warn($"Start {__instance.frame}");
            if (GS3.IsMenuDemo) return true;
            // GS3.Warn("Not Vanilla");
            if (__instance.frame == 1)
            {
                // GS3.Warn("FRAME 1");
                DSPGame.CreateGameMainObject();
                DSPGame.Game.isMenuDemo = DSPGame.IsMenuDemo;
            }

            // GS3.Warn(".");
            if (__instance.frame == 3 && GameMain.isNull)
            {
                // GS3.Warn("FRAME 3");
                __instance.LoadFailed();
                __instance.frame = 3;
            }

            // GS3.Warn(".");
            if (__instance.frame == 5 && GameMain.localStar != null)
            {
                // GS3.Warn($"FRAME 5 {GameMain.mainPlayer != null} {GameMain.gameTick == 0L} {GS3.Config.SkipPrologue}");
                // GS3.Warn($"{GameMain.localStar?.name}");
                if (GameMain.mainPlayer != null && GameMain.gameTick == 0L && GS3.Config.SkipPrologue)
                {
                    // GS3.Warn($"Setting uPosition");
                    GameMain.mainPlayer.uPosition = GameMain.localPlanet.uPosition;
                }

                // GS3.Warn($"{GameMain.localPlanet == null}");
                GameMain.localPlanet?.Load();
                GameMain.localStar?.Load();
            }

            if (__instance.frame >= 7)
            {
                // GS3.Warn("FRAME 7");
                // if (!GS3.Config.SkipPrologue) if (GameMain.localStar != null && !GameMain.localStar.loaded) __instance.frame = 7;
                //     else
                //     {
                if (GameMain.localPlanet != null && !GameMain.localPlanet.loaded) __instance.frame = 7;
                if (GameMain.localPlanet != null && !GameMain.localPlanet.factoryLoaded) __instance.frame = 7;
                // }
            }

            if (__instance.frame == 8)
            {
                // GS3.Warn($"FRAME 8 {__instance.gameObject.GetInstanceID()}");
                if (GameMain.gameTick == 0L)
                {
                    if (GameMain.data == null)
                    {
                        // GS3.Warn("GameMain.data null");
                        return false;
                    }

                    if (DSPGame.SkipPrologue)
                    {
                        GameMain.data.SkipStandardModeGuide();
                    }
                    else
                    {
                        GS3.Warn("Starting standard mode guide");
                        GameMain.data.StartStandardModeGuide();
                    }
                }
                // GS3.Warn("FRAME 8.5");

                GameMain.data.SetReady();
                if (GameCamera.instance == null)
                {
                    // GS3.Warn("Camera null setting frame 9");
                    __instance.frame = 9;
                    return false;
                }

                GameCamera.instance.SetReady();

                GameMain.preferences.LateRestore();
                // GS3.Warn("All good, Setting Frame 10");
                __instance.frame = 10;
            }

            if (__instance.frame == 9)
            {
                // GS3.Warn("Frame 9");
                if (GameCamera.instance == null)
                {
                    // GS3.Warn("Camera null, setting 9");
                    __instance.frame = 9;
                    return false;
                }

                // GS3.Warn("All good on frame 9");
                GameCamera.instance.SetReady();
                GameMain.preferences.LateRestore();
            }

            if (__instance.frame == 10)
            {
                // GS3.Warn($"FRAME 10 {!GameMain.instance?.isMenuDemo} {__instance.GetInstanceID()}");
                GameMain.Begin();
                __instance.SelfDestroy();
                if (!GameMain.instance.isMenuDemo)
                {
                    var str = "";
                    for (var i = GameMain.data.patch + 1; i <= 6; i++)
                        if (i != 3 && i != 4 && i != 5 && i != 6)
                            str = str + "\r\n" + ("存档补丁提示" + i).Translate();

                    if (!string.IsNullOrEmpty(str)) UIMessageBox.Show("存档修复标题".Translate(), "存档修复提示".Translate() + "\r\n" + str, "确定".Translate(), 0);
                }

                GameMain.data.patch = 9;
                // if (__instance.onLoadComplete != null)
                // {
                //     __instance.onLoadComplete();
                // }
                var x = AccessTools.Method(typeof(GameLoader), "onLoadComplete");
                    if (x != null) x.Invoke(__instance, new object[]{}); //Test?

            }

            //GS3.Warn("Increasing Frame");
            __instance.frame++;
            return false;
        }
    }
}