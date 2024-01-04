using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using GSSerializer;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS3
    {
        private static bool DebugOn => Config?.DebugMode ?? false;

        public static void Log(string s, [CallerLineNumber] int lineNumber = 0)
        {
            if (DebugOn) Bootstrap.Debug($"{lineNumber.ToString().PadLeft(4)}:{GetCaller()}{s}");
        }

        public static void LogTop(int width = 80)
        {
            if (BCE.disabled)
            {
                Log("-----");
                return;
            }

            var insert = width - 21;
            var insertLeft = Mathf.FloorToInt(insert / 2) - 2;
            var insertRight = insert - insertLeft - 4;
            BCE.Console.Write("\r\n╔═", ConsoleColor.White);
            BCE.Console.Write("═", ConsoleColor.Gray);
            BCE.Console.Write("═", ConsoleColor.DarkGray);
            BCE.Console.Write(SpaceString(insertLeft), ConsoleColor.Green);
            BCE.Console.Write("*", ConsoleColor.Yellow);
            BCE.Console.Write(".·", ConsoleColor.Gray);
            BCE.Console.Write(":", ConsoleColor.DarkGray);
            BCE.Console.Write("·.", ConsoleColor.Gray);
            BCE.Console.Write("✧", ConsoleColor.DarkCyan);
            BCE.Console.Write(" ✦ ", ConsoleColor.Cyan);
            BCE.Console.Write("✧", ConsoleColor.DarkCyan);
            BCE.Console.Write(".·", ConsoleColor.Gray);
            BCE.Console.Write(":", ConsoleColor.DarkGray);
            BCE.Console.Write("·.", ConsoleColor.Gray);
            BCE.Console.Write("*", ConsoleColor.Yellow);
            BCE.Console.Write(SpaceString(insertRight), ConsoleColor.Green);
            BCE.Console.Write("═", ConsoleColor.DarkGray);
            BCE.Console.Write("═", ConsoleColor.Gray);
            BCE.Console.Write("═╗\r\n", ConsoleColor.White);
        }

        public static void LogBot(int width = 80)
        {
            if (BCE.disabled)
            {
                Log("-----");
                return;
            }

            var insert = width - 21;
            var insertLeft = Mathf.FloorToInt(insert / 2) - 2;
            var insertRight = insert - insertLeft - 4;
            BCE.Console.Write("╚═", ConsoleColor.White);
            BCE.Console.Write("═", ConsoleColor.Gray);
            BCE.Console.Write("═", ConsoleColor.DarkGray);
            BCE.Console.Write(SpaceString(insertLeft), ConsoleColor.Green);
            BCE.Console.Write("*", ConsoleColor.Yellow);
            BCE.Console.Write(".·", ConsoleColor.Gray);
            BCE.Console.Write(":", ConsoleColor.DarkGray);
            BCE.Console.Write("·.", ConsoleColor.Gray);
            BCE.Console.Write("✧", ConsoleColor.DarkCyan);
            BCE.Console.Write(" ✦ ", ConsoleColor.Cyan);
            BCE.Console.Write("✧", ConsoleColor.DarkCyan);
            BCE.Console.Write(".·", ConsoleColor.Gray);
            BCE.Console.Write(":", ConsoleColor.DarkGray);
            BCE.Console.Write("·.", ConsoleColor.Gray);
            BCE.Console.Write("*", ConsoleColor.Yellow);
            BCE.Console.Write(SpaceString(insertRight), ConsoleColor.Green);

            BCE.Console.Write("═", ConsoleColor.DarkGray);
            BCE.Console.Write("═", ConsoleColor.Gray);
            BCE.Console.WriteLine("═╝", ConsoleColor.White);
        }

//─── ･ ｡ﾟ☆: *.☽ .* :☆ﾟ. ───
        public static string SpaceString(int spaces)
        {
            return new string(' ', spaces);
        }

        public static void LogMid(string text, int width = 80)
        {

            string[] lines = text.Split('\n');
            if (lines.Length > 1)
            {
                foreach (string line in lines)
                {
                    LogMid(line, width);
                }
            }

            if (BCE.disabled)
            {
                Log(text);
                return;
            }

            if (text.Length < width - 4) LogMidLine(text, width);
            else
            {
                List<string> texts = new List<string>();
                var i = 0;
                while (text.Length > width - 4)
                {
                    LogMidLine(text.Substring(i, width - 4), width);
                    text = text.Remove(i, width - 4);
                }
            }
        }

        public static void LogMidLine(string text, int width = 80)
        {
            BCE.Console.Write("║ ", ConsoleColor.White);
            BCE.Console.Write(String.Format("{0," + (width - 4) + "}", text), ConsoleColor.Green);
            BCE.Console.Write(" ║\r\n", ConsoleColor.White);
        }

        public static void LogSpace(int lineCount = 1)
        {
            if (DebugOn)
                for (var i = 0; i < lineCount; i++)
                    Bootstrap.Debug(" ", LogLevel.Message, true);
        }

        public static void Error(string message, [CallerLineNumber] int lineNumber = 0)
        {
            Bootstrap.Debug($"{lineNumber,4}:{GetCaller()}{message}", LogLevel.Error, true);
            DumpError(lineNumber + "|" + message);
        }

        public static void Warn(string message, [CallerLineNumber] int lineNumber = 0)
        {
            Bootstrap.Debug($"{lineNumber.ToString().PadLeft(4)}:{GetCaller()}{message}", LogLevel.Warning, true);
        }

        public static void LogJson(object o, bool force = false)
        {
            if (!DebugOn && !force) return;

            var serializer = new fsSerializer();
            serializer.TrySerialize(o, out var data).AssertSuccessWithoutWarnings();
            var json = fsJsonPrinter.PrettyJson(data);
            Bootstrap.Debug(GetCaller() + json);
        }

        public static void WarnJson(object o)
        {
            var serializer = new fsSerializer();
            serializer.TrySerialize(o, out var data).AssertSuccessWithoutWarnings();
            var json = fsJsonPrinter.PrettyJson(data);
            Bootstrap.Debug(GetCaller() + json, LogLevel.Warning, true);
        }

        public static string GetCaller(int depth = 0)
        {
            depth += 2;
            var stackTrace = new StackTrace();
            if (stackTrace.FrameCount <= depth) return "";

            var methodName = stackTrace.GetFrame(depth).GetMethod().Name;
            var reflectedType = stackTrace.GetFrame(depth).GetMethod().ReflectedType;
            if (reflectedType != null)
            {
                var classPath = reflectedType.ToString().Split('.');
                var className = classPath[classPath.Length - 1];
                if (methodName == ".ctor") methodName = "<Constructor>";
                return className + "|" + methodName + "|";
            }

            return "ERROR GETTING CALLER";
        }

        public static void LogError(object sender, UnhandledExceptionEventArgs e, [CallerLineNumber] int lineNumber = 0)
        {
            Error($"{lineNumber} {GetCaller(0)}");
            LogException(e.ExceptionObject as Exception);
        }

        private static void LogException(Exception ex)
        {
            Error(ex.StackTrace); //logs stack trace with line numbers
        }


            public static string FormatTableHeader(string[] headers, int[] columnWidths, params Alignment[] alignments)
            {
                if (headers.Length != columnWidths.Length || headers.Length != alignments.Length)
                {
                    throw new ArgumentException("Input arrays must have the same length.");
                }

                string formatString = GenerateFormatString(columnWidths, alignments);
                string headerRow = string.Format(formatString, headers);


                return $"{headerRow}";
            }
            public static string FormatTable(string[] values, int[] columnWidths, params Alignment[] alignments)
            {
                if (values.Length != columnWidths.Length || values.Length != alignments.Length)
                {
                    throw new ArgumentException("Input arrays must have the same length.");
                }
                string formatString = GenerateFormatString(columnWidths, alignments);
                string valueRow = string.Format(formatString, values);

                return $"{valueRow}";
            }
            private static string GenerateFormatString(int[] columnWidths, Alignment[] alignments)
            {
                if (columnWidths.Length != alignments.Length)
                {
                    throw new ArgumentException("Column widths and alignments must have the same length.");
                }

                string formatString = "";

                for (int i = 0; i < columnWidths.Length; i++)
                {
                    formatString += $"{{{i},-{columnWidths[i]}}}";

                    if (i < columnWidths.Length - 1)
                    {
                        formatString += "  "; // Adjust this space as needed
                    }
                }

                return formatString;
            }

            public enum Alignment
            {
                Left,
                Right,
                Center
            }


            public static IEnumerable<CodeInstruction> LogTranspilerError(IEnumerable<CodeInstruction> instructions, string text)
            {
                Error(text);
                Log(GetCaller());
                LogTop();
                foreach (CodeInstruction instruction in instructions)
                {
                    LogMid(instruction.ToString());
                }
                LogBot();
                return instructions;
            }
    }
}