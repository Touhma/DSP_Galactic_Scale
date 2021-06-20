using BepInEx.Logging;
using GSSerializer;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace GalacticScale
{
    public static partial class GS2
    {
        public static void Log(string s, [CallerLineNumber] int lineNumber = 0)
        {
            if (debugOn)
            {
                Bootstrap.Debug($"{lineNumber.ToString().PadLeft(4)}:{GetCaller()}{s}");
            }
        }
        public static void LogSpace(int lineCount = 1)
        {
            if (debugOn)
            {
                for (var i = 0; i < lineCount; i++)
                {
                    Bootstrap.Debug(" ", LogLevel.Message, true);
                }
            }
        }
        public static void Error(string message,
            [CallerLineNumber] int lineNumber = 0)
        {
            Bootstrap.Debug($"{lineNumber.ToString().PadLeft(4)}:{GetCaller()}{message}", LogLevel.Error, true);
            DumpError(message);
        }
        public static void Warn(string message , 
            [CallerLineNumber] int lineNumber = 0) => Bootstrap.Debug($"{lineNumber.ToString().PadLeft(4)}:{GetCaller()}{message}", LogLevel.Warning, true);
        public static void LogJson(object o, bool force = false)
        {
            if (!debugOn && !force)
            {
                return;
            }

            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(o, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            Bootstrap.Debug(GetCaller() + json);
        }
        public static string GetCaller(int depth = 0)
        {
            depth += 2;
            StackTrace stackTrace = new StackTrace();
            if (stackTrace.FrameCount <= depth)
            {
                return "";
            }

            string methodName = stackTrace.GetFrame(depth).GetMethod().Name;
            string[] classPath = stackTrace.GetFrame(depth).GetMethod().ReflectedType.ToString().Split('.');
            string className = classPath[classPath.Length - 1];
            if (methodName == ".ctor")
            {
                methodName = "<Constructor>";
            }
            return className + "|" + methodName + "|";
        }
    }
}