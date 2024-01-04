using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnGameMain
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameMain), nameof(GameMain.End))]
        public static bool End()
        {
            // GS3.ResearchUnlocked = false;
            if (GameMain.instance == null) return false;

            if (GameMain.data == null)
            {
                GS3.Error("GameMain.data == null");
                return false;
            }

            if (GameMain.gameTime > 0.0 && GameMain.data.guideComplete)
            {
                if (GameMain.errored)
                {
                    GS3.Warn("Auto save disabled due to a previous error.");
                }
                else if (GameMain.isRunning && !GameSave.dontSaveToLastExit && !DSPGame.IsMenuDemo)
                {
                    GS3.Log("Auto saving to <last exit>");
                    GameSave.SaveAsLastExit();
                    Modeler.planetModQueue.Clear();
                    Modeler.planetQueue.Clear();
                    Modeler.planetModQueueSorted = false;
                    Modeler.planetQueueSorted = false;
                }
            }

            GS3.Log("GameMain.End()");
            if (GameMain.gameScenario != null)
            {
                GameMain.gameScenario.Free();
                GameMain.gameScenario = null;
            }

            if (GameMain.instance == null) return false;

            GameMain.instance._running = false;
            GameMain.instance._ended = true;
            if (UIRoot.instance == null)
            {
                GS3.Error("UIRoot Instance Null");
                return false;
            }

            UIRoot.instance.OnGameEnd();
            if (GameMain.universeSimulator == null)
            {
                GS3.Error("UniverseSimulator == null");
                return false;
            }

            GameMain.universeSimulator.OnGameEnd();
            return true;
        }
    }
}