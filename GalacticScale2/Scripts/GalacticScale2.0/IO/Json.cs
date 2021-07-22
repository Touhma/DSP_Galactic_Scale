using System;
using System.Collections.Generic;
using System.IO;
using GSSerializer;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static bool LoadSettingsFromJson(string path)
        {
            Log("Start");
            if (!CheckJsonFileExists(path)) return false;

            Log("Path = " + path);
            var serializer = new fsSerializer();
            GSSettings.Stars.Clear();
            Log("Initializing ThemeLibrary");
            GSSettings.ThemeLibrary = ThemeLibrary.Vanilla();
            Log("Reading JSON");
            var json = File.ReadAllText(path);
            var result = GSSettings.Instance;
            Log("Parsing JSON");
            var data = fsJsonParser.Parse(json);
            Log("Trying To Deserialize JSON");
            serializer.TryDeserialize(data, ref result);
            Log("End");
            return true;
        }

        public static void SaveSettingsToJson(string path)
        {
            Log("Saving Settings to " + path);
            var serializer = new fsSerializer();
            serializer.TrySerialize(GSSettings.Instance, out var data).AssertSuccessWithoutWarnings();
            var json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(path, json);
            Log("End");
        }

        public static void DumpObjectToJson(string path, object obj)
        {
            Log("Dumping Object to " + path);
            var serializer = new fsSerializer();
            serializer.TrySerialize(obj, out var data).AssertSuccessWithoutWarnings();
            var json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(path, json);
            Log("End");
        }

        public static void DumpException(Exception e)
        {
            Error($"{e.Message} {GetCaller(1)} {GetCaller(2)} {GetCaller(3)} {GetCaller(4)} {GetCaller(5)}");
            var path = Path.Combine(AssemblyPath, "ErrorLog");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            path = Path.Combine(path, DateTime.Now.ToString("yyMMddHHmmss"));
            path += ".exceptionlog.json";
            if (File.Exists(path)) return;

            Log(path);
            Log("Logging Error to " + path);
            var eo = new exceptionOutput(e.ToString());
            var serializer = new fsSerializer();
            serializer.TrySerialize(eo, out var data).AssertSuccessWithoutWarnings();
            var json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(path, json);
            Log("End");
        }

        public static void DumpError(string message)
        {
            var path = Path.Combine(AssemblyPath, "ErrorLog");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            path = Path.Combine(path, DateTime.Now.ToString("yyMMddHHmmss"));
            path += ".errorlog.json";
            Log(path);
            Log("Logging Error to " + path);
            var errorObject = new ErrorObject();
            errorObject.message = message;
            for (var i = 0; i < 100; i++)
            {
                var caller = GetCaller(i);
                if (caller != "") errorObject.stack.Add(caller);
            }

            var serializer = new fsSerializer();
            serializer.TrySerialize(errorObject, out var data).AssertSuccessWithoutWarnings();
            var json = fsJsonPrinter.PrettyJson(data);
            File.AppendAllText(path, json);
            Log("End");
        }

        private static bool CheckJsonFileExists(string path)
        {
            Log("Checking if Json File Exists");
            if (File.Exists(path)) return true;

            Log("Json file does not exist at " + path);
            return false;
        }

        private class exceptionOutput
        {
            public string exception;
            public string generator;

            public string version;

            //public GSSettings settings;
            public exceptionOutput(string e)
            {
                version = Version;
                exception = e;
                //settings = GSSettings.Instance;
                generator = GS2.ActiveGenerator?.Name;
            }
        }

        private class ErrorObject
        {
            public string message;
            public readonly List<string> stack = new List<string>();
            public string version = Version;
        }
    }
}