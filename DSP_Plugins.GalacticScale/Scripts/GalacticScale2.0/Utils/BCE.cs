using System;
using System.Reflection;

namespace GalacticScale
{
    public static class BCE
    {
        public static bool disabled = true;
        private static bool initialized = false;
        public static Type t = null;
        public static class console
        {
            public static void init()
            {
                Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly asm in asms)
                {
                    if (asm.GetName().Name == "BCE")
                    {
                        //GS2.Warn("FOUND BCE");
                        t = asm.GetType("BCE.console");
                        disabled = false;
                    }
                }
                initialized = true;
            }
            public static void Write(string s, ConsoleColor c)
            {
                if (!initialized)
                {
                    init();
                }

                if (!disabled)
                {
                    t.GetMethod("Write", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { s, c });
                }
                else
                {
                    GS2.Log(s);
                }
            }
            public static void WriteLine(string s, ConsoleColor c)
            {
                if (!initialized)
                {
                    init();
                }

                if (!disabled)
                {
                    MethodInfo m = t.GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static);
                    if (m.Name != null)
                    {
                        m.Invoke(null, new object[] { s, c });
                    }
                    else
                    {
                        GS2.Log(s);
                    }
                }
            }
        }
    }
}