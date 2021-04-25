using HarmonyLib;
using System.Reflection;
using System.IO;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using System;

namespace GalacticScale.Scripts.PatchUI
{
    [BepInPlugin("dsp.galactic-scale.ui", "Galactic Scale Plug-In - UI", "2.0.0")]
    [BepInDependency("space.customizing.console", BepInDependency.DependencyFlags.SoftDependency)]
    public class PatchForUI : BaseUnityPlugin
    {
        public new static ManualLogSource Logger;
        public static string Version = "2.0.0";
        public static AssetBundle bundle;
        public static class BCE
        {
            public static bool disabled = true;
            static bool initialized = false;
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
                            t = asm.GetType("BCE.console");
                            disabled = false;
                        }
                    }
                    initialized = true;
                }
                public static void Write(string s, ConsoleColor c)
                {
                    if (!initialized) init();
                    if (!disabled) t.GetMethod("Write", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { s, c });
                }
                public static void WriteLine(string s, ConsoleColor c)
                {
                    if (!initialized) init();
                    if (!disabled)
                    {
                        MethodInfo m = t.GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static);
                        if (m.Name != null) m.Invoke(null, new object[] { s, c });
                    }
                }
            }
        }
        internal void Awake()
        {
            var harmony = new Harmony("dsp.galactic-scale.ui");
            Logger = new ManualLogSource("PatchUI");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            BCE.console.WriteLine("┌──────────────────────────────────────────────────────────────────────┐", ConsoleColor.Red);
            BCE.console.Write("│", ConsoleColor.Red);
            BCE.console.Write("  ╔═╗┌─┐┬  ┌─┐┌─┐┌┬┐┬┌─┐  ╔═╗┌─┐┌─┐┬  ┌─┐                             ", ConsoleColor.White);
            BCE.console.Write("│\n", ConsoleColor.Red);
            BCE.console.Write("│", ConsoleColor.Red);
            BCE.console.Write("  ║ ╦├─┤│  ├─┤│   │ ││    ╚═╗│  ├─┤│  ├┤                              ", ConsoleColor.Gray);
            BCE.console.Write("│\n", ConsoleColor.Red);
            BCE.console.Write("│", ConsoleColor.Red);
            BCE.console.Write("  ╚═╝┴ ┴┴─┘┴ ┴└─┘ ┴ ┴└─┘  ╚═╝└─┘┴ ┴┴─┘└─┘ ", ConsoleColor.DarkGray);
            BCE.console.Write("Version " + Version + " Initializing  ", ConsoleColor.White);
            BCE.console.Write("│\n", ConsoleColor.Red);
            BCE.console.WriteLine("└──────────────────────────────────────────────────────────────────────┘", ConsoleColor.Red);
            if (BCE.disabled) Logger.LogMessage("Galactic Scale Version " + Version + " loading..."); // Failsafe if BCE not present

            try
            {
                harmony.PatchAll(typeof(PatchForUI));
                Harmony.CreateAndPatchAll(typeof(PatchOnUIEscMenu));
                Harmony.CreateAndPatchAll(typeof(PatchOnUIGameLoadingSplash));
                Harmony.CreateAndPatchAll(typeof(PatchOnUIVersionText));
                Harmony.CreateAndPatchAll(typeof(PatchOnUIPlanetDetail));
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                Logger.LogError(e.StackTrace);
            }
        }

        public static Sprite GetSpriteAsset(string name)
        {
            if (bundle == null) bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(PatchForUI)).Location), "galacticbundle"));
            if (bundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return null;
            }
            return bundle.LoadAsset<Sprite>(name);
        }
    }
}
