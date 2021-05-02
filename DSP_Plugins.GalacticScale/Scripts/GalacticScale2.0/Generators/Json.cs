using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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
        private string dumpFilename = "_dump";
        public void Init()
        {
            RefreshFileNames();
            GS2.Log("Json.cs:Init: filename count = " + filenames.Count);
            options.Add(new GSOption("Custom Galaxy", "ComboBox", filenames, CustomFileSelectorCallback, CustomFileSelectorPostfix));
            options.Add(new GSOption("Output File Name", "Input", "Output", FilenameInputCallback, FilenameInputPostfix));
            options.Add(new GSOption("Export JSON", "Button", "Export", DumpJSONCallback, ()=>{}));
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
            //if (preferences != null && preferences.ContainsKey("dumpFilename")) dumpFilename = (string)preferences["dumpFilename"];
            dumpFilename = preferences.GetString("dumpFilename", dumpFilename);
        }

        public GSGenPreferences Export()
        {
            return new GSGenPreferences() { { "filename", filename }, { "dumpFilename", dumpFilename } };
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
        private void FilenameInputPostfix()
        {
            GS2.Log("Json:Postfix Filename");
            options[1].rectTransform.GetComponentInChildren<InputField>().text = dumpFilename;
        }
        private void DumpJSONCallback(object result)
        {
            string outputDir = Path.Combine(GS2.DataDir, "CustomGalaxies");
            string path = Path.Combine(outputDir, dumpFilename + ".json");
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            //string starlist = "ClassFactor,Type,Spectr,Age,Mass,Color,Luminosity,Lifetime,Radius,Dyson Radius,Temperature,Orbit Scaler,LightbalRadius\n";
            //foreach (StarData s in GameMain.galaxy.stars)
            //{
            //    starlist+=s.classFactor+","+s.type + "," + s.spectr + "," + s.age + "," + s.mass +"," + s.color + "," + s.luminosity + "," + s.lifetime + "," + s.radius + "," + s.dysonRadius + "," + s.temperature + "," + s.orbitScaler+"," + s.lightBalanceRadius+"\n";
            //}
            GS2.DumpObjectToJson(path, GSSettings.Instance);
        }
        private void FilenameInputCallback(object result)
        {
            string fn = result as string;
            if (fn != "") dumpFilename = fn;
            GS2.Log("Changed Dump Filename to : " + fn);
        }
        private void RefreshFileNames()
        {
            GS2.Log("Refreshing Filenames");
            string customGalaxiesPath = Path.Combine(GS2.DataDir, "CustomGalaxies");
            if (!Directory.Exists(customGalaxiesPath)) Directory.CreateDirectory(customGalaxiesPath);
            filenames = new List<string>(Directory.GetFiles(customGalaxiesPath, "*.json")).ConvertAll<string>((original) => Path.GetFileNameWithoutExtension(original));
            foreach (string n in filenames) GS2.Log("File:" + n);
            if (options != null && options.Count > 0 && options[0].rectTransform != null) options[0].rectTransform.GetComponentInChildren<UIComboBox>().Items = filenames;
        }
    }
}