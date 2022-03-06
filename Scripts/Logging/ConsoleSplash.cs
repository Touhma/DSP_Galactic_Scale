using System;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void ConsoleSplash()
        {
            if (!BCE.disabled)
            {
                BCE.Console.WriteLine("┌─────────────────────────────────────────────────────────────────────────┐", ConsoleColor.Red);
                BCE.Console.Write("│", ConsoleColor.Red);
                BCE.Console.Write("  ╔═╗┌─┐┬  ┌─┐┌─┐┌┬┐┬┌─┐  ╔═╗┌─┐┌─┐┬  ┌─┐                                ", ConsoleColor.White);
                BCE.Console.Write("│\n", ConsoleColor.Red);
                BCE.Console.Write("│", ConsoleColor.Red);
                BCE.Console.Write("  ║ ╦├─┤│  ├─┤│   │ ││    ╚═╗│  ├─┤│  ├┤                                 ", ConsoleColor.Gray);
                BCE.Console.Write("│\n", ConsoleColor.Red);
                BCE.Console.Write("│", ConsoleColor.Red);
                BCE.Console.Write("  ╚═╝┴ ┴┴─┘┴ ┴└─┘ ┴ ┴└─┘  ╚═╝└─┘┴ ┴┴─┘└─┘ ", ConsoleColor.DarkGray);
                BCE.Console.Write("Version " + Version + " Initializing   ", ConsoleColor.White);
                BCE.Console.Write("│\n", ConsoleColor.Red);
                BCE.Console.WriteLine("└─────────────────────────────────────────────────────────────────────────┘", ConsoleColor.Red);
            }
            else
            {
                //Bootstrap.Debug("Galactic Scale Version " + Version + " loading...", BepInEx.Logging.LogLevel.Message, true); // Failsafe if BCE not present
                Bootstrap.Debug(" ");
                Bootstrap.Debug(".─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─."); // Failsafe if BCE not present
                Bootstrap.Debug("│  ╔═╗┌─┐┬  ┌─┐┌─┐┌┬┐┬┌─┐  ╔═╗┌─┐┌─┐┬  ┌─┐                                │");
                Bootstrap.Debug("│  ║ ╦├─┤│  ├─┤│   │ ││    ╚═╗│  ├─┤│  ├┤                                 │");
                Bootstrap.Debug("│  ╚═╝┴ ┴┴─┘┴ ┴└─┘ ┴ ┴└─┘  ╚═╝└─┘┴ ┴┴─┘└─┘ Version " + Version + " Initializing   │");
                Bootstrap.Debug("└──────https://discord.gg/NbpBn6gM6d──────────────────────────────────────┘");
            }
        }
    }
}