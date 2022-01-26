using System;
using System.Collections.Generic;
using System.IO;
using GSSerializer;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void LoadExternalThemes(string path)
        {
            // GS2.Log($"Loading External Themes from: {path}");
            var tl = new ThemeLibrary();
            availableExternalThemes.Clear();
            if (!Directory.Exists(path))
            {
                Warn("External Theme Directory Not Found. Creating");
                Directory.CreateDirectory(path);
                return;
            }

            var files = Directory.GetFiles(path);
            //LogJson(files);
            var directories = Directory.GetDirectories(path);
            //LogJson(directories);
            if (files.Length == 0 && directories.Length == 0) return;
            foreach (var directory in directories)
            {
                var DirName = new DirectoryInfo(directory).Name;
                // GS2.Log($"Searching directory:{directory}");
                if (availableExternalThemes.ContainsKey(DirName))
                    availableExternalThemes[DirName] = LoadDirectoryJsonThemes(directory);
                else availableExternalThemes.Add(DirName, LoadDirectoryJsonThemes(directory));
            }

            foreach (var filename in files)
            {
                // Log($"Found file:{filename}");
                // Warn(new FileInfo(filename).Extension);
                if (new FileInfo(filename).Extension != ".json") continue;
                var theme = LoadJsonTheme(filename);
                if (theme != null)
                {
                    if (tl.ContainsKey(theme.Name)) tl[theme.Name] = theme;
                    else tl.Add(theme.Name, theme);
                }
            }

            //LogJson(tl.Keys.ToList());
            if (availableExternalThemes.ContainsKey("Root"))
                availableExternalThemes["Root"] = tl;
            else availableExternalThemes.Add("Root", tl);
        }

        public static ThemeLibrary LoadDirectoryJsonThemes(string path)
        {
            var tl = new ThemeLibrary();
            // GS2.Log("Loading directory themes and created a new themelibrary with the following contents:");
            // LogJson(tl.Select(o=>o.Key).ToList());
            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            foreach (var directory in directories)
            {
                var dtl = LoadDirectoryJsonThemes(directory);
                tl.AddRange(dtl);
            }

            foreach (var file in files)
            {
                var theme = LoadJsonTheme(file);
                if (theme != null) tl.Add(theme.Name, theme);
            }

            return tl;
        }

        public static GSTheme LoadJsonTheme(string filename)
        {
            // Log("Loading JSON Theme " + filename);
            if (new FileInfo(filename).Extension != ".json")
                // GS2.Warn($"Non Json File Skipped: {filename}");
                return null;
            var json = File.ReadAllText(filename);
            var result = new GSTheme();
            fsData data;
            var fsresult = fsJsonParser.Parse(json, out data);
            if (fsresult.Failed)
            {
                Error("Loading of Json Theme " + filename + " failed. " + fsresult.FormattedMessages);
                return null;
            }

            // Log("Trying To Deserialize JSON");
            var serializer = new fsSerializer();
            var deserializeResult = serializer.TryDeserialize(data, ref result);
            if (deserializeResult.Failed)
            {
                Error("Failed to deserialize " + filename + ": " + deserializeResult.FormattedMessages);
                return null;
            }

            return result;
        }

        public static bool LoadSettingsFromJson(string path)
        {
            Log("Start");
            if (!CheckJsonFileExists(path)) return false;

            Log("Path = " + path);
            var serializer = new fsSerializer();
            GSSettings.Stars.Clear();
            Log("Initializing ThemeLibrary");
            GSSettings.ThemeLibrary = ThemeLibrary.Vanilla();
            GSSettings.GalaxyParams = new GSGalaxyParams();
            Log("Reading JSON");
            var json = File.ReadAllText(path);
            var result = GSSettings.Instance;
            Log("Parsing JSON");
            var data = fsJsonParser.Parse(json);
            Log("Trying To Deserialize JSON");
            serializer.TryDeserialize(data, ref result);
            LogJson(result.galaxyParams);
            Log("End");
            return true;
        }

        public static void SaveSettingsToJson(string path)
        {
            // Log("Saving Settings to " + path);
            var serializer = new fsSerializer();
            serializer.TrySerialize(GSSettings.Instance, out var data).AssertSuccessWithoutWarnings();
            var json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(path, json);
            // Log("End");
        }

        public static void DumpObjectToJson(string path, object obj)
        {
            // Log("Dumping Object to " + path);
            var serializer = new fsSerializer();
            serializer.TrySerialize(obj, out var data).AssertSuccessWithoutWarnings();
            var json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(path, json);
            // Log("End");
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
                generator = ActiveGenerator?.Name;
            }
        }

        private class ErrorObject
        {
            public readonly List<string> stack = new List<string>();
            public string message;
            public string version = Version;
        }
    }
}