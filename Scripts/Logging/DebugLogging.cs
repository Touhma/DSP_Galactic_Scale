using System.Diagnostics;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using GSSerializer;

namespace GalacticScale
{
    public static partial class GS2
    {
        private static bool DebugOn => Config?.DebugMode ?? false;

        public static void Log(string s, [CallerLineNumber] int lineNumber = 0)
        {
            if (DebugOn) Bootstrap.Debug($"{lineNumber.ToString().PadLeft(4)}:{GetCaller()}{s}");
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
            var classPath = stackTrace.GetFrame(depth).GetMethod().ReflectedType.ToString().Split('.');
            var className = classPath[classPath.Length - 1];
            if (methodName == ".ctor") methodName = "<Constructor>";
            return className + "|" + methodName + "|";
        }
    }
}