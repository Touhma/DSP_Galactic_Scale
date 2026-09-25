using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace GalacticScale
{
    /// <summary>Translations for Galactic Scale's own UI. Keys remain the original English text.</summary>
    public static class GSLocalization
    {
        private static readonly Dictionary<int, Dictionary<string, string>> Languages = new()
        {
            { 2052, Load("zh-CN") }
        };

        public static string Translate(string text)
        {
            return Translate(text, Localization.CurrentLanguageLCID);
        }

        public static string Translate(string text, int lcid)
        {
            if (string.IsNullOrEmpty(text) || !Languages.TryGetValue(lcid, out var entries)) return text;
            if (entries.TryGetValue(text, out var translation)) return translation;
            foreach (var starType in StarTypes)
            {
                if (!entries.TryGetValue(starType, out var translatedType)) continue;
                if (text == starType + " Overrides") return translatedType + "专属设置";
                if (text == "Change Settings for Type " + starType + " stars") return "修改" + translatedType + "的设置";
                if (text == "Allow " + starType + " to spawn as binary companions") return "允许" + translatedType + "作为双星伴星生成";
            }
            return text;
        }

        private static readonly string[] StarTypes =
        {
            "Type K", "Type M", "Type F", "Type G", "Type A", "Type B", "Type O",
            "White Dwarf", "Red Giant", "Yellow Giant", "White Giant", "Blue Giant",
            "O Giant", "Neutron Star", "Black Hole"
        };

        private static Dictionary<string, string> Load(string language)
        {
            var entries = new Dictionary<string, string>(StringComparer.Ordinal);
            var name = $"GalacticScale.Translations.{language}.tsv";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                if (stream == null) return entries;
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length == 0 || line[0] == '#') continue;
                        var separator = line.IndexOf('\t');
                        if (separator < 1 || separator == line.Length - 1) continue;
                        var key = line.Substring(0, separator).Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\s", " ");
                        var value = line.Substring(separator + 1).Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\s", " ");
                        entries[key] = value;
                    }
                }
            }
            return entries;
        }
    }
}
