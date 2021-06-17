using BepInEx;
using GSSerializer;
using System;
using System.IO;

namespace GalacticScale {
    public static partial class GS2 {
        public static bool LoadSettingsFromJson(string path) {
            Log("Start");
            if (!CheckJsonFileExists(path)) {
                return false;
            }

            Log("Path = " + path);
            fsSerializer serializer = new fsSerializer();
            GSSettings.Stars.Clear();
            Log("Initializing ThemeLibrary");
            GSSettings.ThemeLibrary = ThemeLibrary.Vanilla();
            Log("Reading JSON");
            string json = File.ReadAllText(path);
            GSSettings result = GSSettings.Instance;
            Log("Parsing JSON");
            fsData data = fsJsonParser.Parse(json);
            Log("Trying To Deserialize JSON");
            serializer.TryDeserialize(data, ref result);
            Log("End");
            return true;
        }
        public static void SaveSettingsToJson(string path) {
            Log("Saving Settings to " + path);
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize<GSSettings>(GSSettings.Instance, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(path, json);
            Log("End");

        }
        public static void DumpObjectToJson(string path, object obj) {
            Log("Dumping Object to " + path);
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(obj, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(path, json);
            Log("End");
        }
        private class exceptionOutput {
            public string version;
            public string generator;
            public string exception;
            //public GSSettings settings;
            public exceptionOutput(string e) {
                version = GS2.Version;
                exception = e;
                //settings = GSSettings.Instance;
                generator = GS2.generator?.Name;
            }
        }
        public static void DumpException(Exception e) {
            Error($"{e.Message} {GetCaller(1)} {GetCaller(2)} {GetCaller(3)} {GetCaller(4)} {GetCaller(5)}");
            string path = Path.Combine(Path.Combine(Path.Combine(Paths.BepInExRootPath, "plugins"), "GalacticScale"), "ErrorLog");
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, DateTime.Now.ToString("yyMMddHHmmss"));
            path += ".exceptionlog.json";
            if (File.Exists(path)) {
                return;
            }

            GS2.Log(path);
            Log("Logging Error to " + path);
            exceptionOutput eo = new exceptionOutput(e.ToString());
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(eo, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(path, json);
            Log("End");
        }
        private class ErrorObject {
            public string message;
            public string version = GS2.Version;
            public System.Collections.Generic.List<string> stack = new System.Collections.Generic.List<string>();
        }
        public static void DumpError(string message) {
            string path = Path.Combine(Path.Combine(Path.Combine(Paths.BepInExRootPath, "plugins"), "GalacticScale"), "ErrorLog");
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, DateTime.Now.ToString("yyMMddHHmmss"));
            path += ".errorlog.json";
            GS2.Log(path);
            Log("Logging Error to " + path);
            ErrorObject errorObject = new ErrorObject();
            errorObject.message = message;
            for (var i = 0; i < 100; i++) {
                string caller = GetCaller(i);
                if (caller != "") {
                    errorObject.stack.Add(caller);
                }
            }
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(errorObject, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            File.AppendAllText(path, json);
            Log("End");
        }
        private static bool CheckJsonFileExists(string path) {
            Log("Checking if Json File Exists");
            if (File.Exists(path)) {
                return true;
            }

            Log("Json file does not exist at " + path);
            return false;
        }
    }
}