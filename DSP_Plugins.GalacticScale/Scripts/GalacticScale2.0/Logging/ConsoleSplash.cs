using System;

namespace GalacticScale {
    public static partial class GS2 {
        public static void ConsoleSplash() {
            if (!BCE.disabled) {
                BCE.console.WriteLine("┌─────────────────────────────────────────────────────────────────────────┐", ConsoleColor.Red);
                BCE.console.Write("│", ConsoleColor.Red);
                BCE.console.Write("  ╔═╗┌─┐┬  ┌─┐┌─┐┌┬┐┬┌─┐  ╔═╗┌─┐┌─┐┬  ┌─┐                                ", ConsoleColor.White);
                BCE.console.Write("│\n", ConsoleColor.Red);
                BCE.console.Write("│", ConsoleColor.Red);
                BCE.console.Write("  ║ ╦├─┤│  ├─┤│   │ ││    ╚═╗│  ├─┤│  ├┤                                 ", ConsoleColor.Gray);
                BCE.console.Write("│\n", ConsoleColor.Red);
                BCE.console.Write("│", ConsoleColor.Red);
                BCE.console.Write("  ╚═╝┴ ┴┴─┘┴ ┴└─┘ ┴ ┴└─┘  ╚═╝└─┘┴ ┴┴─┘└─┘ ", ConsoleColor.DarkGray);
                BCE.console.Write("Version " + Version + " Initializing  ", ConsoleColor.White);
                BCE.console.Write("│\n", ConsoleColor.Red);
                BCE.console.WriteLine("└─────────────────────────────────────────────────────────────────────────┘", ConsoleColor.Red);
            } else {
                Bootstrap.Debug("Galactic Scale Version " + Version + " loading...", BepInEx.Logging.LogLevel.Message, true); // Failsafe if BCE not present
            }
        }
    }
}