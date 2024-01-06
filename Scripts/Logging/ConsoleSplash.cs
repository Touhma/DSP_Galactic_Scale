using System;
using BCE;
namespace GalacticScale
{
    public static partial class GS3
    {
        public static void ConsoleSplash()
        {
            // if (!BCE.disabled)
            // {
                console.WriteLine("┌─────────────────────────────────────────────────────────────────────────┐", ConsoleColor.Red);
                console.Write("│", ConsoleColor.Red);
                console.Write("  ╔═╗┌─┐┬  ┌─┐┌─┐┌┬┐┬┌─┐  ╔═╗┌─┐┌─┐┬  ┌─┐                                ", ConsoleColor.White);
                console.Write("│\n", ConsoleColor.Red);
                console.Write("│", ConsoleColor.Red);
                console.Write("  ║ ╦├─┤│  ├─┤│   │ ││    ╚═╗│  ├─┤│  ├┤                                 ", ConsoleColor.Gray);
                console.Write("│\n", ConsoleColor.Red);
                console.Write("│", ConsoleColor.Red);
                console.Write("  ╚═╝┴ ┴┴─┘┴ ┴└─┘ ┴ ┴└─┘  ╚═╝└─┘┴ ┴┴─┘└─┘ ", ConsoleColor.DarkGray);
                console.Write($"Version {Version,9} Initializing ", ConsoleColor.White);
                console.Write("│\n", ConsoleColor.Red);
                console.WriteLine("└─────────────────────────────────────────────────────────────────────────┘", ConsoleColor.Red);
            // }
            // else
            // {
            //     //Bootstrap.Debug("Galactic Scale Version " + Version + " loading...", BepInEx.Logging.LogLevel.Message, true); // Failsafe if BCE not present
            //     Bootstrap.Debug(" ");
            //     Bootstrap.Debug(".─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─."); // Failsafe if BCE not present
            //     Bootstrap.Debug("│  ╔═╗┌─┐┬  ┌─┐┌─┐┌┬┐┬┌─┐  ╔═╗┌─┐┌─┐┬  ┌─┐                                │");
            //     Bootstrap.Debug("│  ║ ╦├─┤│  ├─┤│   │ ││    ╚═╗│  ├─┤│  ├┤                                 │");
            //     Bootstrap.Debug($"│  ╚═╝┴ ┴┴─┘┴ ┴└─┘ ┴ ┴└─┘  ╚═╝└─┘┴ ┴┴─┘└─┘ Version  {Version,7} Initializing │");
            //     Bootstrap.Debug("└──────https://discord.gg/NbpBn6gM6d──────────────────────────────────────┘");
            // }
        }
    }
}