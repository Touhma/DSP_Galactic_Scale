using System;
using System.Reflection;

namespace GalacticScale
{
    // ReSharper disable once InconsistentNaming
    public static class BCE
    {
        public static bool disabled = true;
        private static bool initialized;
        private static Type t;

        public static class Console
        {
            public static void Init()
            {
                var asms = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var asm in asms)
                {
                    if (asm.GetName().Name != "BCE") continue;
                    t = asm.GetType("BCE.console");
                    disabled = false;
                }

                initialized = true;
            }

            public static void Write(string s, ConsoleColor c)
            {
                if (!initialized) Init();

                if (!disabled)
                    t.GetMethod("Write", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { s, c });
                else
                    GS2.Log(s);
            }

            public static void WriteLine(string s, ConsoleColor c)
            {
                if (!initialized) Init();

                if (disabled) return;
                var m = t.GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static);
                m?.Invoke(null, new object[] { s, c });
            }
        }
    }
}