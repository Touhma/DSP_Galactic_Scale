using System;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void ConsoleSplash()
        {
            //GlobalObject.LoadVersions();
            string dspVersion = GameConfig.gameVersion.ToString();
            
            if (!BCE.disabled)
            {
                BCE.Console.WriteLine("┌─────────────────────────────────────────────────────────────────────────┐", ConsoleColor.Red);
                BCE.Console.Write("│", ConsoleColor.Red);
                BCE.Console.Write("  ╔═╗┌─┐┬  ┌─┐┌─┐┌┬┐┬┌─┐  ╔═╗┌─┐┌─┐┬  ┌─┐                                ", ConsoleColor.White);
                BCE.Console.Write("│\n", ConsoleColor.Red);
                BCE.Console.Write("│", ConsoleColor.Red);
                BCE.Console.Write($"  ║ ╦├─┤│  ├─┤│   │ ││    ╚═╗│  ├─┤│  ├┤  DSP Version  {dspVersion, 17} ", ConsoleColor.Gray);
                BCE.Console.Write("│\n", ConsoleColor.Red);
                BCE.Console.Write("│", ConsoleColor.Red);
                //GlobalObject.LoadVersions();
                BCE.Console.Write("  ╚═╝┴ ┴┴─┘┴ ┴└─┘ ┴ ┴└─┘  ╚═╝└─┘┴ ┴┴─┘└─┘ ", ConsoleColor.DarkGray);
                BCE.Console.Write($"Version {Version,9} Initializing ", ConsoleColor.White);
                BCE.Console.Write("│\n", ConsoleColor.Red);
                BCE.Console.WriteLine("└─────────────────────────────────────────────────────────────────────────┘", ConsoleColor.Red);
            }
            else
            {
                //Bootstrap.Debug("Galactic Scale Version " + Version + " loading...", BepInEx.Logging.LogLevel.Message, true); // Failsafe if BCE not present
                Bootstrap.Debug(" ");
                Bootstrap.Debug(".─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─_─."); // Failsafe if BCE not present
                Bootstrap.Debug("│  ╔═╗┌─┐┬  ┌─┐┌─┐┌┬┐┬┌─┐  ╔═╗┌─┐┌─┐┬  ┌─┐                                │");
                
                Bootstrap.Debug($"│  ║ ╦├─┤│  ├─┤│   │ ││    ╚═╗│  ├─┤│  ├┤  DSP Version  {dspVersion, 17} │");
                Bootstrap.Debug($"│  ╚═╝┴ ┴┴─┘┴ ┴└─┘ ┴ ┴└─┘  ╚═╝└─┘┴ ┴┴─┘└─┘ Version  {Version,8} Initializing │");
                Bootstrap.Debug("└──────https://discord.gg/NbpBn6gM6d──────────────────────────────────────┘");
            }
            
            Bootstrap.Debug($"");
        }
    }
}