using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GalacticScale.Generators
{
    public class JsonImport : iConfigurableGenerator
    {
        public string Name => "Custom Json";

        public string Author => "innominata";

        public string Description => "Nothing left to chance. This allows external generators to create a universe description.";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.customjson";

        public List<GSOption> Options => options;

        public bool DisableStarCountSlider => true;

        public GSGeneratorConfig Config => new GSGeneratorConfig(true, true);

        private string filename = "GSData";
        private List<string> filenames = new List<string>();
        public void Init()
        {
            RefreshFileNames();
            GS2.Log("Json.cs:Init: filename count = " + filenames.Count);
            options.Add(new GSOption("Custom Galaxy", "ComboBox", filenames, CustomFileSelectorCallback, CustomFileSelectorPostfix));
            options.Add(new GSOption("Dump JSON", "Button", "Export", DumpJSONCallback, ()=>{}));
        }
        public List<GSOption> options = new List<GSOption>();
        public void Generate(int starCount)
        {
            GS2.Log("Json Importer Generating");
            string path = Path.Combine(Path.Combine(GS2.DataDir,"CustomGalaxies"), filename + ".json");
            GS2.LoadSettingsFromJson(path);
        }

        public void Import(GSGenPreferences preferences)
        {
            GS2.Log("Importing JSON Preferences");
            if (preferences != null && preferences.ContainsKey("filename")) filename = (string)preferences["filename"];
        }

        public GSGenPreferences Export()
        {
            return new GSGenPreferences() { { "filename", filename } };
        }
        public void CustomFileSelectorCallback(object result)
        {
            int index = (int)result;
            filename = filenames[index];
            RefreshFileNames();

        }
        private void CustomFileSelectorPostfix()
        {
            GS2.Log("Json:Postfix");
            int index = 0;
            for (var i = 0; i < filenames.Count; i++)
            {
                if (filename == filenames[i]) index = i;
            }
            options[0].rectTransform.GetComponentInChildren<UIComboBox>().itemIndex = index;
        }
        private void DumpJSONCallback(object result)
        {
            string outputDir = Path.Combine(GS2.DataDir, "output");
            string path = Path.Combine(outputDir, DateTime.Now.ToString("yyMMddHHmmss") + "dump.json");
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            GS2.DumpObjectToJson(path, GS2.gameDesc);
        }
        private void RefreshFileNames()
        {
            GS2.Log("Refreshing Filenames");
            string customGalaxiesPath = Path.Combine(GS2.DataDir, "CustomGalaxies");
            if (!Directory.Exists(customGalaxiesPath)) Directory.CreateDirectory(customGalaxiesPath);
            filenames = new List<string>(Directory.GetFiles(customGalaxiesPath, "*.json")).ConvertAll<string>((original) => Path.GetFileNameWithoutExtension(original));
            foreach (string n in filenames) GS2.Log("File:" + n);
            //options[0].rectTransform.GetComponentInChildren<UIComboBox>().Items = filenames;
        }
    }
}