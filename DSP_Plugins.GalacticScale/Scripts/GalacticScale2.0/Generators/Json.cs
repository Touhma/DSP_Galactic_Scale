using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

namespace GalacticScale.Generators {
    public class JsonImport : iConfigurableGenerator {
        public string Name => "Custom Json";

        public string Author => "innominata";

        public string Description => "Nothing left to chance. This allows external generators to create a universe description.";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.customjson";

        public GSOptions Options => options;

        public bool DisableStarCountSlider => true;

        public GSGeneratorConfig Config => new GSGeneratorConfig(true, true);

        private string filename = "";
        private List<string> filenames = new List<string>();
        private string dumpFilename = "_dump";
        private GSUI minifyCheckbox;
        public void Init() {
            GS2.Log("Generator:Json|Init");
            RefreshFileNames();
            GS2.Log("Generator:Json|Init|FileCount = " + filenames.Count);
            options.Add(GSUI.Combobox("Custom Galaxy",filenames, CustomFileSelectorCallback, CustomFileSelectorPostfix));
            options.Add(GSUI.Input("Output File Name", "Output", FilenameInputCallback, FilenameInputPostfix));
            minifyCheckbox = options.Add(GSUI.Checkbox("Minify Output JSON", false, MinifyCallback, MinifyPostfix));
            options.Add(GSUI.Button("Export JSON", "Export", DumpJSONCallback, () => { }));
            GS2.Log("Generator:Json|Init|End");
        }
        public GSOptions options = new GSOptions();
        public void Generate(int starCount) {
            GS2.Log("Generator:Json|Generate");
            if (string.IsNullOrEmpty(filename)) {
                UIMessageBox.Show("Error", "To use the Custom JSON Generator you must select a file to load.", "Ok", 1);
                RefreshFileNames();
            }
            string path = Path.Combine(Path.Combine(GS2.DataDir, "CustomGalaxies"), filename + ".json");
            GS2.LoadSettingsFromJson(path);
            GS2.Log("Generator:Json|Generate|End");
        }

        public void Import(GSGenPreferences preferences) {
            GS2.Log("Importing JSON Preferences");
            GS2.Log("Generator:Json|Import");
            filename = preferences.GetString("filename");
            GS2.Warn("Filename" + filename);
            if (!filenames.Contains(filename)) {
                filename = filenames[0];
            }
            //if (preferences != null && preferences.ContainsKey("dumpFilename")) dumpFilename = (string)preferences["dumpFilename"];
            dumpFilename = preferences.GetString("dumpFilename", dumpFilename);
            GS2.minifyJSON = preferences.GetBool("minify", false);
            GS2.Log("Generator:Json|Import|End");
        }

        public GSGenPreferences Export() {
            GS2.Log("Generator:Json|Export");
            return new GSGenPreferences() {
                { "filename", filename },
                { "dumpFilename", dumpFilename },
                { "minify", GS2.minifyJSON.ToString() }
            };
        }
        public void CustomFileSelectorCallback(object result) {
            int index = (int)result;
            filename = filenames[index];
            RefreshFileNames();

        }
        public void MinifyCallback(object result) => GS2.minifyJSON = (bool)result;
        public void MinifyPostfix() => minifyCheckbox.Set(GS2.minifyJSON);
        private void CustomFileSelectorPostfix() {
            //GS2.Log("Json:Postfix");
            int index = 0;
            for (var i = 0; i < filenames.Count; i++) {
                if (filename == filenames[i]) {
                    index = i;
                }
            }
            options[0].RectTransform.GetComponentInChildren<UIComboBox>().itemIndex = index;
        }
        private void FilenameInputPostfix() =>
            //GS2.Log("Json:Postfix Filename");
            options[1].RectTransform.GetComponentInChildren<InputField>().text = dumpFilename;
        private void DumpJSONCallback(object result) {
            string outputDir = Path.Combine(GS2.DataDir, "CustomGalaxies");
            string path = Path.Combine(outputDir, dumpFilename + ".json");
            if (!Directory.Exists(outputDir)) {
                Directory.CreateDirectory(outputDir);
            }
            //string starlist = "ClassFactor,Type,Spectr,Age,Mass,Color,Luminosity,Lifetime,Radius,Dyson Radius,Temperature,Orbit Scaler,LightbalRadius\n";
            //foreach (StarData s in GameMain.galaxy.stars)
            //{
            //    starlist+=s.classFactor+","+s.type + "," + s.spectr + "," + s.age + "," + s.mass +"," + s.color + "," + s.luminosity + "," + s.lifetime + "," + s.radius + "," + s.dysonRadius + "," + s.temperature + "," + s.orbitScaler+"," + s.lightBalanceRadius+"\n";
            //}
            GS2.DumpObjectToJson(path, GSSettings.Instance);
            RefreshFileNames();
        }
        private void FilenameInputCallback(object result) {
            string fn = result as string;
            if (fn != "") {
                dumpFilename = fn;
            }
            //GS2.Log("Changed Dump Filename to : " + fn);
        }
        private void RefreshFileNames() {
            //GS2.Log("Refreshing Filenames");
            string customGalaxiesPath = Path.Combine(GS2.DataDir, "CustomGalaxies");
            if (!Directory.Exists(customGalaxiesPath)) {
                Directory.CreateDirectory(customGalaxiesPath);
            }

            filenames = new List<string>(Directory.GetFiles(customGalaxiesPath, "*.json")).ConvertAll<string>((original) => Path.GetFileNameWithoutExtension(original));
            //foreach (string n in filenames) GS2.Log("File:" + n);
            if (options != null && options.Count > 0 && options[0].RectTransform != null) {
                options[0].RectTransform.GetComponentInChildren<UIComboBox>().Items = filenames;
            }
        }
    }
}