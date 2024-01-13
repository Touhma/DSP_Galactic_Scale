using HarmonyLib;
using UnityEngine.Experimental.UIElements;

namespace GalacticScale
{
    /// <summary>
    ///     Add a bunch of Null Checks to stop the game from completely crashing when galaxy generation fails.
    /// </summary>
    public class PatchOnGameLoader
    {
        private static int counter;
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
            GS2.DevLog("Not Vanilla");
            if (__instance.frame == 1)
            {
                GS2.DevLog("FRAME 1");
                DSPGame.CreateGameMainObject();
                DSPGame.Game.isMenuDemo = DSPGame.IsMenuDemo;
            }

            // GS2.DevLog(".");
            if (__instance.frame == 3 && GameMain.isNull)
            {
                GS2.DevLog("FRAME 3");
                __instance.LoadFailed();
                __instance.frame = 3;
            }

            // GS2.DevLog(".");
            if (__instance.frame == 5 && GameMain.localStar != null)
            {
                GS2.DevLog($"FRAME 5 {GameMain.mainPlayer != null} {GameMain.gameTick == 0L} {GS2.Config.SkipPrologue}");
                GS2.DevLog($"{GameMain.localStar?.name}");
                if (GameMain.mainPlayer != null && GameMain.gameTick == 0L && GS2.Config.SkipPrologue)
                {
                    GS2.DevLog($"Setting uPosition");
                    GameMain.mainPlayer.uPosition = GameMain.localPlanet.uPosition;
                }

                GS2.DevLog($"{GameMain.localPlanet == null}");
                GameMain.localPlanet?.Load();
                GameMain.localStar?.Load();
            }

            if (__instance.frame >= 7)
            {
                if (counter > 2000) __instance.LoadFailed();
                var planet = GameMain.localPlanet;
                counter++;
                if (planet != null &&  counter % 100 == 0) GS2.DevLog($"FRAME 7 ({counter}) LocalStar:{GameMain.localStar?.name} Local Planet:{planet?.name} LocalPlanet Loaded:{planet.loaded} LocalPlanet FactoryLoaded:{planet.factoryLoaded}");
                if (planet != null &&  counter % 500 == 0 && planet.loaded && !planet.factoryLoaded && !planet.factoryLoading)
                {
                    planet?.Unload();
                    planet?.Load();
                }
                if (planet != null && !planet.loaded) __instance.frame = 7;
                if (planet != null && !planet.factoryLoaded) __instance.frame = 7;
                // }
            }

            if (__instance.frame == 8)
            {
                GS2.DevLog($"FRAME 8 {__instance.gameObject.GetInstanceID()}");
                if (GameMain.gameTick == 0L)
                {
                    if (GameMain.data == null)
                    {
                        GS2.DevLog("GameMain.data null");
                        return false;
                    }

                    if (DSPGame.SkipPrologue)
                    {
                        GameMain.data.SkipStandardModeGuide();
                    }
                    else
                    {
                        GS2.DevLog("Starting standard mode guide");
                        GameMain.data.StartStandardModeGuide();
                    }
                }
                GS2.Warn("FRAME 8.5");

                GameMain.data.SetReady();
                if (GameCamera.instance == null)
                {
                    GS2.DevLog("Camera null setting frame 9");
                    __instance.frame = 9;
                    return false;
                }

                GameCamera.instance.SetReady();

                GameMain.preferences.LateRestore();
                GS2.DevLog("All good, Setting Frame 10");
                __instance.frame = 10;
            }

            if (__instance.frame == 9)
            {
                GS2.DevLog("Frame 9");
                if (GameCamera.instance == null)
                {
                    GS2.Warn("Camera null, setting 9");
                    __instance.frame = 9;
                    return false;
                }

                GS2.DevLog("All good on frame 9");
                GameCamera.instance.SetReady();
                GameMain.preferences.LateRestore();
            }

            if (__instance.frame == 10)
            {
                GS2.DevLog($"FRAME 10 {!GameMain.instance?.isMenuDemo} {__instance.GetInstanceID()}");
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
                // var x = AccessTools.Method(typeof(GameLoader), "onLoadComplete");
                //     if (x != null) x.Invoke(__instance, new object[]{}); //Test?

            }

            // GS2.DevLog("Increasing Frame");
            __instance.frame++;
            return false;
        }
    }
}