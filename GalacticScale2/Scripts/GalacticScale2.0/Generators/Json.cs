using System.Collections.Generic;
using System.IO;

namespace GalacticScale.Generators
{
    public class JsonImport : iConfigurableGenerator
    {
        // private GSUI minifyCheckbox;
        public readonly GSOptions options = new GSOptions();

        private List<string> filenames = new List<string>();
        // private string dumpFilename = "_dump";

        private string ImportFilename = "";

        public bool DisableStarCountSlider => true;
        public string Name => "Custom Json";

        public string Author => "innominata";

        public string Description =>
            "Nothing left to chance. This allows external generators to create a universe description.";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.customjson";

        public GSOptions Options => options;

        public GSGeneratorConfig Config => new GSGeneratorConfig(true, true);

        public void Init()
        {
            GS2.Log("Generator:Json|Init");
            RefreshFileNames();
            GS2.Log("Generator:Json|Init|FileCount = " + filenames.Count);
            options.Add(GSUI.Combobox("Custom Galaxy".Translate(), filenames, CustomFileSelectorCallback, CustomFileSelectorPostfix));
            // options.Add(GSUI.Input("Output File Name".Translate(), "Output", FilenameInputCallback, FilenameInputPostfix));
            // minifyCheckbox = options.Add(GSUI.Checkbox("Minify Output JSON".Translate(), false, MinifyCallback, MinifyPostfix));
            // options.Add(GSUI.Button("Export JSON".Translate(), "Export", DumpJSONCallback, () => { }));
            GS2.Log("Generator:Json|Init|End");
        }

        public void Generate(int starCount, StarData birthStar = null)
        {
            GS2.Log("Generator:Json|Generate");
            if (string.IsNullOrEmpty(ImportFilename))
            {
                UIMessageBox.Show("Error", "To use the Custom JSON Generator you must select a file to load.", "Ok", 1);
                RefreshFileNames();
            }

            var path = Path.Combine(Path.Combine(GS2.DataDir, "CustomGalaxies"), ImportFilename + ".json");
            GS2.LoadSettingsFromJson(path);
            GS2.Log("Generator:Json|Generate|End");
        }

        public void Import(GSGenPreferences preferences)
        {
            GS2.Log("Importing JSON Preferences");
            GS2.Log("Generator:Json|Import");
            ImportFilename = preferences.GetString("Import Filename");
            //GS2.Warn("Filename" + filename);
            if (!filenames.Contains(ImportFilename)) ImportFilename = filenames[0];
            //if (preferences != null && preferences.ContainsKey("dumpFilename")) dumpFilename = (string)preferences["dumpFilename"];
            // dumpFilename = preferences.GetString("dumpFilename", dumpFilename);
            // GS2.MinifyJson = preferences.GetBool("minify");
            GS2.Log("Generator:Json|Import|End");
        }

        public GSGenPreferences Export()
        {
            GS2.Log("Generator:Json|Export");
            return new GSGenPreferences
            {
                { "Import Filename", ImportFilename }
                // {"dumpFilename", dumpFilename},
                // {"minify", GS2.MinifyJson.ToString()}
            };
        }

        private void CustomFileSelectorCallback(Val result)
        {
            int index = result;
            if (index > filenames.Count - 1) index = 0;
            ImportFilename = filenames[index];
            if (ImportFilename == "No Files Found") ImportFilename = "";
            RefreshFileNames();
        }

        // public void MinifyCallback(Val result)
        // {
        //     GS2.Config.MinifyJson = result.Bool();
        // }
        //
        // public void MinifyPostfix()
        // {
        //     minifyCheckbox.Set(GS2.Config.MinifyJson);
        // }

        private void CustomFileSelectorPostfix()
        {
            GS2.Log("Json:Postfix");
            var index = 0;
            for (var i = 0; i < filenames.Count; i++)
                if (ImportFilename == filenames[i])
                    index = i;
            options[0].RectTransform.GetComponentInChildren<GSUIDropdown>().Value = index;
        }

        // private void FilenameInputPostfix()
        // {
        //     //GS2.Log("Json:Postfix Filename");
        //     options[1].RectTransform.GetComponentInChildren<InputField>().text = dumpFilename;
        // }

        // private void DumpJSONCallback(Val result)
        // {
        //     var outputDir = Path.Combine(GS2.DataDir, "CustomGalaxies");
        //     var path = Path.Combine(outputDir, dumpFilename + ".json");
        //     if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
        //     //string starlist = "ClassFactor,Type,Spectr,Age,Mass,Color,Luminosity,Lifetime,Radius,Dyson Radius,Temperature,Orbit Scaler,LightbalRadius\n";
        //     //foreach (StarData s in GameMain.galaxy.stars)
        //     //{
        //     //    starlist+=s.classFactor+","+s.type + "," + s.spectr + "," + s.age + "," + s.mass +"," + s.color + "," + s.luminosity + "," + s.lifetime + "," + s.radius + "," + s.dysonRadius + "," + s.temperature + "," + s.orbitScaler+"," + s.lightBalanceRadius+"\n";
        //     //}
        //     GS2.DumpObjectToJson(path, GSSettings.Instance);
        //     RefreshFileNames();
        // }

        // private void FilenameInputCallback(Val result)
        // {
        //     if (!string.IsNullOrEmpty(result)) dumpFilename = result;
        //     //GS2.Log("Changed Dump Filename to : " + fn);
        // }

        public void RefreshFileNames()
        {
            //GS2.Log("Refreshing Filenames");
            var customGalaxiesPath = Path.Combine(GS2.DataDir, "CustomGalaxies");
            if (!Directory.Exists(customGalaxiesPath)) Directory.CreateDirectory(customGalaxiesPath);

            filenames = new List<string>(Directory.GetFiles(customGalaxiesPath, "*.json")).ConvertAll(Path.GetFileNameWithoutExtension);
            if (filenames.Count == 0)
                filenames.Add("No Files Found"); //foreach (string n in filenames) GS2.Log("File:" + n);
            if (options != null && options.Count > 0 && options[0].RectTransform != null)
                options[0].RectTransform.GetComponentInChildren<GSUIDropdown>().Items = filenames;
        }
    }
}