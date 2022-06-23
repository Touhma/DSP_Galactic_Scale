using System;

namespace GalacticScale
{
    public static class GSEvents
    {
        public static event EventHandler OnSettingsOpen = delegate { };
        public static event EventHandler OnSettingsClose = delegate { };
        public static event EventHandler OnSettingsCancel = delegate { };
        public static event EventHandler<iGenerator> OnGeneratorChange = delegate { };
        public static event EventHandler OnSettingsApply = delegate { };
        public static event EventHandler OnGameStart = delegate { };
        public static event EventHandler OnGameEnd = delegate { };

        public static void GeneratorChange(iGenerator generator)
        {
            OnGeneratorChange(null, generator);
        }

        public static void SettingsOpen()
        {
            OnSettingsOpen(null, null);
        }

        public static void SettingsClose()
        {
            OnSettingsClose(null, null);
        }

        public static void SettingsCancel()
        {
            OnSettingsCancel(null, null);
        }

        public static void SettingsApply()
        {
            OnSettingsApply(null, null);
        }

        public static void GameStart()
        {
            OnGameStart(null, null);
        }

        public static void GameEnd()
        {
            OnGameEnd(null, null);
        }
    }
}