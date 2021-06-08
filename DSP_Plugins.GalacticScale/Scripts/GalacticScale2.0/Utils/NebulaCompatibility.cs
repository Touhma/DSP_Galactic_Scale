using System;
using System.Reflection;

namespace GalacticScale
{
    public static class NebulaCompatibility
    {
        public static bool Initialized
        {
            get
            {
                if (N_SimulatedWorld == null) return false;
                return (bool)N_SimulatedWorld.GetProperty("Initialized").GetValue(null);
            }
        }
        public static bool IsMasterClient
        {
            get
            {
                if (N_LocalPlayer == null) return true;
                return (bool)N_LocalPlayer.GetProperty("IsMasterClient").GetValue(null);
            }
        }
        public static Type N_LocalPlayer = null;
        public static Type N_SimulatedWorld = null;
        public static bool Nebula = false;

        public static void init()
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in asms)
            {
                //GS2.Warn(asm.GetName().Name);
                if (asm.GetName().Name == "NebulaWorld")
                {
                    Nebula = true;
                    foreach (Type t in asm.GetTypes())
                    {
                        if (t.Name == "LocalPlayer") N_LocalPlayer = t;
                        if (t.Name == "SimulatedWorld") N_SimulatedWorld = t;
                    }
                }
            }
        }
    }
}