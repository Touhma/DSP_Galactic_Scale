using System;
using System.Reflection;
using HarmonyLib;

namespace GalacticScale
{
    public static class NEB
    {
        public static bool disabled = true;
        private static bool initialized;
        private static Type nebulaPatcher;

        public static class Hack
        {
            public static void Init()
            {
                nebulaPatcher = AccessTools.TypeByName("NebulaPatcher.Patches.Dynamic.UIGalaxySelect_Patch");
                if (nebulaPatcher != null) disabled = false;
                initialized = true;
            }

            public static void EnterGame(UIGalaxySelect __instance)
            {
                if (!initialized) Init();

                if (!disabled)
                    nebulaPatcher.GetMethod("EnterGame_Prefix", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { __instance });
                else
                    GS3.Log("Couldn't run nebula EnterGame method");
            }

        }
    }
}