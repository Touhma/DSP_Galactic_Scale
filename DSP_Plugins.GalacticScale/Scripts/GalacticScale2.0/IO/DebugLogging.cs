using GSFullSerializer;
using System.Diagnostics;
using BepInEx.Logging;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void Log(string s)
        {
            if (debugOn) Bootstrap.Debug(GetCaller() +s);
        }
        public static void Error(string message)
        {
            Bootstrap.Debug(GetCaller()+message, LogLevel.Error, true);
        }
        public static void Warn(string message)
        {
            Bootstrap.Debug(GetCaller() + message, LogLevel.Warning, true);
        }
        public static void LogJson(object o)
        {
            if (!debugOn) return;
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(o, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            Bootstrap.Debug(GetCaller() + json);
        }
        public static string GetCaller()
        {
            StackTrace stackTrace = new StackTrace();
            string methodName = stackTrace.GetFrame(2).GetMethod().Name;
            string[] classPath = stackTrace.GetFrame(2).GetMethod().ReflectedType.ToString().Split('.');
            string className = classPath[classPath.Length - 1];
            if (methodName == ".ctor") methodName = "<Constructor>";
            return className+"|"+methodName+"|";
        }
    }
}