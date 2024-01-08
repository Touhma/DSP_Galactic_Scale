using System;
using System.Collections;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    public partial class GS3
    {
        public static int PreferencesVersion = 30004;
    }

    // public static class MatcherExtensions
    // {
    //     public static void LogILPre(this CodeMatcher _matcher, int lines = 1, int pre = 5, int post = 5, [CallerLineNumber] int lineNumber = 0)
    //     {
    //         Bootstrap._DumpMatcher(_matcher, lines, pre, post, lineNumber, 1, GS3.GetCallerMethod());
    //     }
    //     public static void LogILPost(this CodeMatcher _matcher, int lines = 1, int pre = 5, int post = 5, [CallerLineNumber] int lineNumber = 0)
    //     {
    //         Bootstrap._DumpMatcher(_matcher, lines, pre, post, lineNumber, 2, GS3.GetCallerMethod());
    //     }
    //     public static void LogIL(this CodeMatcher _matcher, int lines = 1, int pre = 5, int post = 5, [CallerLineNumber] int lineNumber = 0)
    //     {
    //         Bootstrap._DumpMatcher(_matcher, lines, pre, post, lineNumber, 0, GS3.GetCallerMethod());
    //     }
    // }

    [BepInPlugin("dsp.galactic-scale.3", "Galactic Scale 3 Plug-In", "3.0.0")]
    [BepInDependency("space.customizing.console", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("dsp.nebula-multiplayer-api", BepInDependency.DependencyFlags.SoftDependency)]
    public class Bootstrap : BaseUnityPlugin
    {
        public static Bootstrap instance;
        public new static ManualLogSource Logger;
        public static Queue buffer = new();

        internal void Awake()
        {
            instance = this;
            InitializeLogger();
            InitializeComponents();
            ApplyHarmonyPatches();
        }

        private void InitializeLogger()
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) => GS3.LogError(o,e);
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            var a = Assembly.GetAssembly(typeof(GSStar));
            GS3.Version = $"{v.Major}.{v.Minor}.{v.Build}";
            // console.Init();
            Logger = new ManualLogSource("GS3");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            
            GS3.ConsoleSplash();
        }

        private void InitializeComponents()
        {
            if (GS3.TP == null) GS3.TP = gameObject.AddComponent<TeleportComponent>();
            if (GS3.InputComponent == null) GS3.InputComponent = gameObject.AddComponent<InputComponent>();
        }

        private void ApplyHarmonyPatches()
        {
            Directory.CreateDirectory("./mmdump");
            // {
                foreach (var file in new DirectoryInfo("./mmdump").GetFiles())
                {
                    file.Delete();
                }

                
            // }
            try
            {
                var harmony = new Harmony("dsp.galactic-scale.3");
                harmony.PatchAll(typeof(Patches.PlanetSizeTranspiler));
                harmony.PatchAll(typeof(Patches.EnemyUnitComponentTranspiler));
                harmony.PatchAll(typeof(Patches.UnitComponentTranspiler));
                harmony.PatchAll(typeof(Patches.TurretComponentTranspiler));
                
                harmony.PatchAll(typeof(Patches.PatchOnUnspecified_Debug));
                
                harmony.PatchAll(typeof(Patches.PatchOnBuildTool_Click));
                harmony.PatchAll(typeof(Patches.PatchOnBuildTool_Inserter));
                harmony.PatchAll(typeof(Patches.PatchOnBuildTool_Path));
                Environment.SetEnvironmentVariable("MONOMOD_DMD_TYPE", "cecil");
                Environment.SetEnvironmentVariable("MONOMOD_DMD_DUMP", "./mmdump");
                harmony.PatchAll(typeof(Patches.PatchOnBuildTool_PathAddon));
                // Environment.SetEnvironmentVariable("MONOMOD_DMD_DUMP", "");

                harmony.PatchAll(typeof(Patches.PatchOnDefenseSystem));
                harmony.PatchAll(typeof(Patches.PatchOnDFGTurretComponent));
                
                harmony.PatchAll(typeof(Patches.PatchOnDFRelayComponent));
                harmony.PatchAll(typeof(Patches.PatchOnDigitalSystem));
                harmony.PatchAll(typeof(Patches.PatchOnEnemyDFGroundSystem));
                harmony.PatchAll(typeof(Patches.PatchOnEnemyDFHiveSystem));
                harmony.PatchAll(typeof(Patches.PatchOnFactoryModel));
                harmony.PatchAll(typeof(Patches.PatchOnGalaxyData));
                harmony.PatchAll(typeof(Patches.PatchOnGameAbnormalityData));
                harmony.PatchAll(typeof(Patches.PatchOnGameAchievementData));
                harmony.PatchAll(typeof(Patches.PatchOnGameData));
                harmony.PatchAll(typeof(Patches.PatchOnGameDesc));
                harmony.PatchAll(typeof(Patches.PatchOnGameHistoryData));
                harmony.PatchAll(typeof(Patches.PatchOnGameLoader));
                harmony.PatchAll(typeof(Patches.PatchOnGameMain));
                harmony.PatchAll(typeof(Patches.PatchOnGameOption));
                harmony.PatchAll(typeof(Patches.PatchOnGameSave));
                harmony.PatchAll(typeof(Patches.PatchOnGraticulePoser));
                harmony.PatchAll(typeof(Patches.PatchOnGuideMissionStandardMode));
                harmony.PatchAll(typeof(Patches.PatchOnLocalLaserOneShot));
                harmony.PatchAll(typeof(Patches.PatchOnLocalLaserContinuous));
                harmony.PatchAll(typeof(Patches.PatchOnNearColliderLogic));
                harmony.PatchAll(typeof(Patches.PatchOnPlanetAlgorithm));
                harmony.PatchAll(typeof(Patches.PatchOnPlanetAtmoBlur));
                harmony.PatchAll(typeof(Patches.PatchOnPlanetData));
                harmony.PatchAll(typeof(Patches.PatchOnPlanetFactory));
                harmony.PatchAll(typeof(Patches.PatchOnPlanetGrid));
                harmony.PatchAll(typeof(Patches.PatchOnPlanetModelingManager));
                harmony.PatchAll(typeof(Patches.PatchOnPlanetRawData));
                harmony.PatchAll(typeof(Patches.PatchOnPlanetSimulator));
                harmony.PatchAll(typeof(Patches.PatchOnPlatformSystem));
                harmony.PatchAll(typeof(Patches.PatchOnPlayerController));
                harmony.PatchAll(typeof(Patches.PatchOnPlayerFootsteps));
                harmony.PatchAll(typeof(Patches.PatchOnPowerSystem));
                harmony.PatchAll(typeof(Patches.PatchOnSectorModel));
                harmony.PatchAll(typeof(Patches.PatchOnSpaceSector));
                harmony.PatchAll(typeof(Patches.PatchOnStationComponent));
                harmony.PatchAll(typeof(Patches.PatchOnThemeProto));
                harmony.PatchAll(typeof(Patches.PatchOnTrashSystem));
                harmony.PatchAll(typeof(Patches.PatchOnUIAchievementPanel));
                harmony.PatchAll(typeof(Patches.PatchOnUIAdvisorTip));
                harmony.PatchAll(typeof(Patches.PatchOnUIBuildingGrid));
                harmony.PatchAll(typeof(Patches.PatchOnUIEscMenu));
                harmony.PatchAll(typeof(Patches.PatchOnUIGalaxySelect));
                harmony.PatchAll(typeof(Patches.PatchOnUIGame));
                harmony.PatchAll(typeof(Patches.PatchOnUIGameLoadingSplash));
                harmony.PatchAll(typeof(Patches.PatchOnUILoadGameWindow));
                harmony.PatchAll(typeof(Patches.PatchOnUIMainMenu));
                harmony.PatchAll(typeof(Patches.PatchOnUIOptionWindow));
                harmony.PatchAll(typeof(Patches.PatchOnUIPlanetDetail));
                harmony.PatchAll(typeof(Patches.PatchOnUIResearchResultsWindow));
                harmony.PatchAll(typeof(Patches.PatchOnUIRoot));
                harmony.PatchAll(typeof(Patches.PatchOnUISaveGameWindow));
                harmony.PatchAll(typeof(Patches.PatchOnUISpaceGuide));
                harmony.PatchAll(typeof(Patches.PatchOnUIStarDetail));
                harmony.PatchAll(typeof(Patches.PatchOnUIStarmap));
                harmony.PatchAll(typeof(Patches.PatchOnUIStarmapPlanet));
                harmony.PatchAll(typeof(Patches.PatchOnUIStarmapStar));
                harmony.PatchAll(typeof(Patches.PatchOnUITutorialTip));
                harmony.PatchAll(typeof(Patches.PatchOnUIVeinDetail));
                harmony.PatchAll(typeof(Patches.PatchOnUIVersionText));
                harmony.PatchAll(typeof(Patches.PatchOnUIVirtualStarmap));
                harmony.PatchAll(typeof(Patches.PatchOnUniverseGen));
                harmony.PatchAll(typeof(Patches.PatchOnUniverseSimulator));
                harmony.PatchAll(typeof(Patches.PatchOnVFPreload));
                harmony.PatchAll(typeof(Patches.PatchOnWarningSystem));
            }
            catch (System.Exception e)
            {
                GS3.Error(e.ToString());
            }
        }

        public static void Debug(object data, LogLevel logLevel, bool isActive)
        {
            if (isActive && Logger != null)
            {
                while (buffer.Count > 0)
                {
                    var o = buffer.Dequeue();
                    var l = ((object data, LogLevel loglevel, bool isActive))o;
                    if (l.isActive) Logger.Log(l.loglevel, "Q:" + l.data);
                }

                Logger.Log(logLevel, data);
            }
            else
            {
                buffer.Enqueue((data, logLevel, true));
            }
        }

        public static void Debug(object data)
        {
            Debug(data, LogLevel.Message, true);
        }

        public static void WaitUntil(TestFunction t, TestThen u)
        {
            instance.StartCoroutine(WaitUntilRoutine(t, u));
        }
        public static IEnumerator WaitUntilRoutine(TestFunction t, TestThen u)
        {
           yield return new WaitUntil(()=>t());
           u();
        }

        // public static void DumpInstructions(IEnumerable<CodeInstruction> instructions,string method, int start = 0, int count = 0, [CallerLineNumber] int lineNumber = 0)
        // {
        //     var list = instructions.ToList();
        //     var caller = $"IL for {method} - {GS3.GetCallerMethod()} on line {lineNumber}";
        //     var len = Mathf.Max(caller.Length + 2, 120);
        //     var pad = len - caller.Length;
        //     pad /= 2;
        //     if (list.Count == 0) BCE.Console.WriteLine($"Dumpinstructions: List is empty | {GS3.GetCallerMethod()} on line {lineNumber}", ConsoleColor.Red);
        //     if (count == 0) count = list.Count;
        //     if (start > list.Count) start = 0;
        //     
        //     
        //     for (var i = start; i < count+start && i < list.Count; i++)
        //     {
        //         var z = list[i];
        //         var opcode = string.Format("{0,-10}", z.opcode);
        //         var operand = string.Format("{0,-60}", z.operand);
        //         BCE.Console.Write($"  {i}  {opcode} {operand}", ConsoleColor.DarkGray);
        //         BCE.Console.WriteLine($" // {z.operand?.GetType()}", ConsoleColor.DarkGreen);
        //     }
        // }

        
        // public static void _DumpMatcher(CodeMatcher _matcher, int lines = 1, int pre = 5,int post = 5, [CallerLineNumber] int lineNumber = 0, int type = 0, string callerMethod = "")
        // {
        //     // BCE.Console.WriteLine("__________________________________________________________", ConsoleColor.DarkGray);
        //     if (callerMethod == "") callerMethod = GS3.GetCallerMethod();
        //     var prefix = type == 1?"Pre":type == 2?"Post":"";
        //     var caller = $"{prefix}Transpile IL for {callerMethod} on line {lineNumber}";
        //     
        //     
        //     var len = Mathf.Max(caller.Length + 2, 120);
        //     var pad = len - caller.Length;
        //     pad /= 2;
        //     GS3.LogTop(len);
        //
        //     BCE.Console.WriteLine(string.Format("{0,"+pad+"}{1,"+pad+"}","",caller), ConsoleColor.DarkCyan);
        //     var matcher = _matcher.Clone();
        //     var step = lines + pre;
        //     while (matcher.Pos > 0 && step > 1)
        //     {
        //         matcher.Advance(-1);
        //         step--;
        //     }
        //
        //     step = pre;
        //     while (matcher.Remaining > 0 && step > 0)
        //     {
        //         var z = matcher.Instruction;
        //
        //         var opcode = string.Format("{0,-10}", z.opcode);
        //         var operand = string.Format("{0,-50}", z.operand);
        //         BCE.Console.Write($"  {matcher.Pos}  {opcode} {operand}", ConsoleColor.DarkGray);
        //         BCE.Console.WriteLine($" // {z.operand?.GetType()}", ConsoleColor.DarkGreen);
        //         matcher.Advance(1);
        //         step--;
        //     }
        //     step = lines;
        //     while (matcher.Remaining > 0 && step > 0)
        //     {
        //         var z = matcher.Instruction;
        //         var opcode = string.Format("{0,-10}", z.opcode);
        //         var operand = string.Format("{0,-50}", z.operand);
        //         BCE.Console.Write($"  {matcher.Pos}  {opcode} {operand}", ConsoleColor.White);
        //         BCE.Console.WriteLine($" // {z.operand?.GetType()}", ConsoleColor.DarkGreen);
        //         matcher.Advance(1);
        //         step--;
        //     }
        //     step = post;
        //     while (matcher.Remaining > 0 && step > 0)
        //     {
        //         var z = matcher.Instruction;
        //         var opcode = string.Format("{0,-10}", z.opcode);
        //         var operand = string.Format("{0,-50}", z.operand);
        //         BCE.Console.Write($"  {matcher.Pos}  {opcode} {operand}", ConsoleColor.DarkGray);
        //         BCE.Console.WriteLine($" // {z.operand?.GetType()}", ConsoleColor.DarkGreen);
        //         matcher.Advance(1);
        //         step--;
        //     }
        //     // BCE.Console.WriteLine("__________________________________________________________", ConsoleColor.DarkGray);
        //     GS3.LogBot(len);
        // }
    }

    public delegate bool TestFunction();

    public delegate void TestThen();
}