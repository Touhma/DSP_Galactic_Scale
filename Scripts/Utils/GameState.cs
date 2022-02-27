namespace GalacticScale
{
    public static partial class GS2
    {
        public static bool AbortGameStart(string message)
        {
            Error("Aborting Game Start|" + message);
            Failed = true;
            UIRoot.instance.CloseLoadingUI();
            UIRoot.instance.CloseGameUI();
            UIRoot.instance.launchSplash.Restart();
            DSPGame.StartDemoGame(0);
            UIMessageBox.Show("Somewhat Fatal Error", "Cannot Start Game. Possibly reason: " + message, "Rats!", 3, () =>
            {
                UIRoot.instance.OpenMainMenuUI();
                UIRoot.ClearFatalError();
            });
            UIRoot.ClearFatalError();
            return false;
        }

        public static void EndGame()
        {
            GameMain.End();
        }
    }
}