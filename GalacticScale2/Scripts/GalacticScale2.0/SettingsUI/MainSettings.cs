using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalacticScale.Generators;
using GSSerializer;

namespace GalacticScale
{
    public class GS2MainSettings : iConfigurableGenerator

    {
        private GSUI _cheatModeCheckbox;

        private GSUI _exportButton;

        // private GSUI GeneratorCombobox;
        private List<string> _generatorNames;
        private static GSUI _generatorsCombobox;
        public GSGenPreferences Preferences = new GSGenPreferences();
        public bool ForceRare => Preferences.GetBool("Force Rare Spawn");
        public bool DebugMode => Preferences.GetBool("Debug Log");
        public bool Dev => Preferences.GetBool("Dev");
        public bool SkipPrologue => Preferences.GetBool("Skip Prologue");
        public bool SkipTutorials => Preferences.GetBool("Skip Tutorials");
        public bool CheatMode => Preferences.GetBool("Cheat Mode");
        public bool MinifyJson => Preferences.GetBool("Minify JSON");
        public bool FixCopyPaste => true; //Preferences.GetBool("Fix CopyPaste", true);
        public string GeneratorID => Preferences.GetString("Generator ID", "space.customizing.generators.vanilla");
        public bool UseExternalThemes => Preferences.GetBool("Use External Themes");
        public float ResourceMultiplier => Preferences.GetFloat("Resource Multiplier", 1f);
        public float LogisticsShipMulti => Preferences.GetFloat("Logistics Ship Multi", 1f);
        public List<string> ExternalThemeNames => Preferences.GetStringList("External Themes", new List<string>());
        public Dictionary<string, GSUI> ThemeCheckboxes = new Dictionary<string, GSUI>();
        public string Name => "Main Settings";

        public string Author => "innominata";

        public string Description => "Main Settings";

        public string Version => "1";

        public string GUID => "main.settings";

        public GSGeneratorConfig Config => new GSGeneratorConfig();

        public GSOptions Options { get; } = new GSOptions();
        //public bool Test => Preferences.GetBool("Test", false);
        //public float TestNum => Preferences.GetFloat("TestNum", 0f);

        public GSGenPreferences Export()
        {
            Preferences.Set("Generator ID", GS2.ActiveGenerator.GUID);
            return Preferences;
        }

        public void Generate(int starCount, StarData birthStar = null)
        {
            _generatorNames = GS2.Generators.ConvertAll(iGen => iGen.Name);
            _generatorsCombobox?.SetItems(_generatorNames);
        }

        public void Import(GSGenPreferences preferences)
        {
            Preferences = preferences;
            var id = Preferences.GetString("Generator ID", "space.customizing.generators.vanilla");
            GS2.ActiveGenerator = GS2.GetGeneratorByID(id);
            Preferences.Set("Generator", _generatorNames.IndexOf(GS2.ActiveGenerator.Name));
        }

        public void Init()
        {
            // GS2.Warn("!");
            _generatorNames = GS2.Generators.ConvertAll(iGen => iGen.Name);
            // GS2.LogJson(_generatorNames, true);
            _generatorsCombobox = Options.Add(GSUI.Combobox("Generator".Translate(), _generatorNames, 0, "Generator",
                GeneratorCallback, "Try them all!".Translate()));
            
            Options.Add(GSUI.Checkbox("Skip Prologue".Translate(), false, "Skip Prologue"));
            Options.Add(GSUI.Checkbox("Skip Tutorials".Translate(), false, "Skip Tutorials"));
            
            

            var DebugOptions = new GSOptions();
            DebugOptions.Add(GSUI.Checkbox("Debug Log".Translate(), false,  "Debug Log", null, "Print extra logs to BepInEx console".Translate()));
            DebugOptions.Add(GSUI.Checkbox("Force Rare Spawn".Translate(), false, "Force Rare Spawn", null, "Ignore randomness/distance checks".Translate()));
            _cheatModeCheckbox = DebugOptions.Add(GSUI.Checkbox("Cheat Mode".Translate(), false, "Cheat Mode", null, "All Research, TP by ctrl-click nav arrow".Translate()));
            DebugOptions.Add(GSUI.Slider("Ship Speed Multiplier".Translate(), 1f, 1f, 100f, "Logistics Ship Multi", null, "Multiplier for Warp Speed of Ships".Translate()));
            Options.Add(GSUI.Group("Debug Options".Translate(), DebugOptions, "Useful for testing galaxies/themes".Translate()));
            var JsonOptions = new GSOptions();
            JsonOptions.Add(GSUI.Input("Export Filename".Translate(), "My First Custom Galaxy", "Export Filename", null, "Excluding .json".Translate()));
            JsonOptions.Add(GSUI.Checkbox("Minify Exported JSON".Translate(), false, "Minify JSON", null, "Only save changes".Translate()));
            _exportButton = JsonOptions.Add(GSUI.Button("Export Custom Galaxy".Translate(), "Export".Translate(), ExportJsonGalaxy, null, "Save Galaxy to File".Translate()));
            Options.Add(GSUI.Group("Custom Galaxy Export".Translate(), JsonOptions, "Usable once in game".Translate()));
        }

        public void SetResourceMultiplier(float val)
        {
            Preferences.Set("Resource Multiplier", val);
            GS2.SavePreferences();
        }
        public void SetExternalTheme(string folder, string name, bool value)
        {
            // GS2.Warn($"Setting External Theme:{folder}|{name} to {value}");
            var ExternalThemes = Preferences.GetStringList("External Themes", new List<string>());
            string key = $"{folder}|{name}";
            if (ExternalThemes.Contains(key))
            {
                if (!value) ExternalThemes.Remove(key);
            }
            else
            {
                if (value) ExternalThemes.Add(key);
            }

            Preferences.Set("External Themes", ExternalThemes);
        }
        public void InitThemePanel()
        {
            var externalThemesGroupOptions = new List<GSUI>() {
                GSUI.Button("Export All Themes".Translate(), "Export".Translate(), ExportAllThemes)
            };
            // GS2.Warn("ExternalThemeNames");
            // GS2.WarnJson(ExternalThemeNames);
            foreach (var themelibrary in GS2.availableExternalThemes)
            {
                var Folder = themelibrary.Key;
                var Library = themelibrary.Value;
                if (Folder == "Root")
                {
                    foreach (var theme in Library)
                    {
                        string key = $"{Folder}|{theme.Key}";
                        // GS2.Warn("Checking Key");
                        bool def = false;
                        if (ExternalThemeNames.Contains(key)) def = true;
                        else GS2.Log($"Doesnt Contain {key}");
                        // GS2.Log($"Setting {key} to {def}");
                        GSOptionCallback callback = (Val o) =>
                        {
                            // GS2.Log(key + " " + o); 
                            SetExternalTheme(Folder, theme.Key, o);
                        };
                        GSUI checkbox = null;
                        GSOptionPostfix postfix = () =>
                        {
                            // GS2.Log($"Executing Postfix for {key}");
                            ThemeCheckboxes[key].RectTransform.GetComponent<GSUIToggle>().Value = def;
                            // GS2.Log($"Now the value is {ThemeCheckboxes[key]?.RectTransform.GetComponent<GSUIToggle>().Value}");
                        };
                        checkbox = GSUI.Checkbox(theme.Key.Translate(), def, callback, postfix);
                        if (!ThemeCheckboxes.ContainsKey(key)) ThemeCheckboxes.Add(key, checkbox);
                        externalThemesGroupOptions.Add(checkbox);
                    }

                }
                else
                {
                    var listoptions = new List<GSUI>();
                    foreach (var theme in Library)
                    {
                        bool def = false;
                        string key = $"{Folder}|{theme.Key}";
                        if (ExternalThemeNames.Contains(key)) def = true;
                        GSOptionPostfix postfix = () =>
                        {
                            // GS2.Log($"Executing Postfix for {key}");
                            ThemeCheckboxes[key].RectTransform.GetComponent<GSUIToggle>().Value = def;
                            // GS2.Log($"Now the value is {ThemeCheckboxes[key]?.RectTransform.GetComponent<GSUIToggle>().Value}");
                        };
                        var checkbox = GSUI.Checkbox("  " + theme.Key, def,
                            (o) =>
                            {
                                // GS2.Log(Folder + "|" + theme.Key + " " + o);
                                SetExternalTheme(Folder, theme.Key, o);
                            }, postfix

                        );
                        if (!ThemeCheckboxes.ContainsKey(key)) ThemeCheckboxes.Add(key, checkbox);
                        listoptions.Add(checkbox);
                    }
                    var FolderGroup = GSUI.Group(Folder, listoptions);
                    externalThemesGroupOptions.Add(FolderGroup);
                }
            }

            var externalThemesGroup = Options.Add(GSUI.Group("External Themes".Translate(), externalThemesGroupOptions));
        }
        private void ExportAllThemes(Val o)
        {
            if (GameMain.isPaused)
            {
                var path = Path.Combine(GS2.DataDir, "ExportedThemes");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                foreach (var theme in GSSettings.ThemeLibrary)
                {
                    var filename = Path.Combine(path, theme.Value.Name + ".json");
                    var fs = new fsSerializer();
                    fs.TrySerialize(theme.Value, out var data);
                    var json = fsJsonPrinter.PrettyJson(data);
                    File.WriteAllText(filename, json);
                    
                }
                UIMessageBox.Show("Success".Translate(),
                                         "Themes have been exported to "
                                             .Translate() + path + "/",
                                         "D'oh!".Translate(), 2);
                                     return;
            }
            UIMessageBox.Show("Error".Translate(),
                "Please try again after creating a galaxy :)\r\nStart a game, then press ESC and click settings."
                    .Translate(),
                "D'oh!".Translate(), 2);
        }


        private static void GeneratorCallback(Val result)
        {
            // GS2.Warn($"Generator Callback:{(int)result}");
            GS2.ActiveGenerator = GS2.Generators[(int)result];
            // GS2.Warn("Active Generator = " + GS2.ActiveGenerator.Name);
            foreach (var canvas in SettingsUI.GeneratorCanvases)
                canvas.gameObject.SetActive(false);
            // GS2.Warn("They have been set inactive");
            GS2.Warn(SettingsUI.GeneratorCanvases.Count + " count , trying to set "+(int)result);
            SettingsUI.GeneratorCanvases[(int)result].gameObject.SetActive(true);
            // GS2.Warn("Correct one set active");
            SettingsUI.GeneratorIndex = (int)result;
            // GS2.Warn("Gen Index Set");
            //SettingsUI.UpdateContentRect();
            // GS2.Warn("Updated ContentRect");
            GS2.SavePreferences();
            // GS2.Warn("Preferences Saved");
        }

        private void ExportJsonGalaxy(Val o)
        {
            var outputDir = Path.Combine(GS2.DataDir, "CustomGalaxies");
            var path = Path.Combine(outputDir, Preferences.GetString("Export Filename", "My First Galaxy".Translate()) + ".json");

            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            if (GameMain.isPaused)
            {
                GS2.DumpObjectToJson(path, GSSettings.Instance);
                UIMessageBox.Show("Success".Translate(),
                    "Galaxy Saved to ".Translate() + path,
                    "Woohoo!".Translate(), 1);
                var JsonGenerator = (JsonImport) GS2.GetGeneratorByID("space.customizing.generators.customjson");
                JsonGenerator.RefreshFileNames();
                return;
            }

            UIMessageBox.Show("Error".Translate(),
                "Please try again after creating a galaxy :)\r\nStart a game, then press ESC and click settings."
                    .Translate(),
                "D'oh!".Translate(), 2);
        }


        public void DisableCheatMode()
        {
            _cheatModeCheckbox.Set(false);
        }

        public void SetExternalThemes(ExternalThemeSelector e)
        {
            // GS2.Warn("Setting External Themes");
            // GS2.WarnJson(e.Get());
            // var themeNames = e.Get();
            // Preferences.Set("External Themes", themeNames);
        }

        public void SetUseExternalThemes(bool val)
        {
            Preferences.Set("Use External Themes", val);
        }
    }
}