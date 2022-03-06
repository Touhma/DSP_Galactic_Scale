using System;

namespace GalacticScale
{
    public static  class GSEvents
    {
        static public event EventHandler OnSettingsOpen = delegate {  };
        static public event EventHandler OnSettingsClose = delegate {  };
        static public event EventHandler OnSettingsCancel = delegate {  };
        static public event EventHandler<iGenerator> OnGeneratorChange = delegate {  };
        static public event EventHandler OnSettingsApply = delegate {  };
        static public event EventHandler OnGameStart = delegate {  };
        static public event EventHandler OnGameEnd = delegate {  };

        public static void GeneratorChange(iGenerator generator) => OnGeneratorChange(null, generator);
        public static void SettingsOpen() => OnSettingsOpen(null, null);
        public static void SettingsClose() => OnSettingsClose(null, null);
        public static void SettingsCancel() => OnSettingsCancel(null, null);
        public static void SettingsApply() => OnSettingsApply(null, null);
        public static void GameStart() => OnGameStart(null, null);
        public static void GameEnd() => OnGameEnd(null, null);
    }
}