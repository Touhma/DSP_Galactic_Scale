using System.Collections.Generic;
using System.IO;
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
        private GSUI _generatorsCombobox;
        public GSGenPreferences Preferences = new GSGenPreferences();
        public bool ForceRare => Preferences.GetBool("Force Rare Spawn");
        public bool DebugMode => Preferences.GetBool("Debug Log");
        public bool Dev => Preferences.GetBool("Dev");
        public bool SkipPrologue => Preferences.GetBool("Skip Prologue");
        public bool SkipTutorials => Preferences.GetBool("Skip Tutorials");
        public bool CheatMode => Preferences.GetBool("Cheat Mode");
        public bool MinifyJson => Preferences.GetBool("Minify JSON");
        public bool FixCopyPaste => Preferences.GetBool("Fix CopyPaste", true);
        public string GeneratorID => Preferences.GetString("Generator ID", "space.customizing.generators.vanilla");
        public bool UseExternalThemes => Preferences.GetBool("Use External Themes");
        public List<string> ExternalThemeNames => Preferences.StringList("External Themes", new List<string>());
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
            // GS2.Warn("!");
            Preferences.Set("Generator ID", GS2.ActiveGenerator.GUID);
            return Preferences;
        }

        public void Generate(int starCount)
        {
            _generatorNames = GS2.Generators.ConvertAll(iGen => iGen.Name);
            _generatorsCombobox.SetItems(_generatorNames);
        }

        public void Import(GSGenPreferences preferences)
        {
            GS2.Warn("!");
            Preferences = preferences;
            GS2.Warn("!!");
            var id = Preferences.GetString("Generator ID", "space.customizing.generators.vanilla");
            GS2.Warn("!!!");
            GS2.ActiveGenerator = GS2.GetGeneratorByID(id);
            GS2.Warn("!!!!");
            Preferences.Set("Generator", _generatorNames.IndexOf(GS2.ActiveGenerator.Name));
            GS2.Warn("!!!!!");
        }

        public void Init()
        {
            // GS2.Warn("!");
            _generatorNames = GS2.Generators.ConvertAll(iGen => iGen.Name);
            GS2.LogJson(_generatorNames, true);
            _generatorsCombobox = Options.Add(GSUI.Combobox("Generator".Translate(), _generatorNames, 0, "Generator",
                GeneratorCallback));
            Options.Add(GSUI.Checkbox("Force Rare Spawn".Translate(), false, "Force Rare Spawn"));
            Options.Add(GSUI.Checkbox("Skip Prologue".Translate(), false, "Skip Prologue"));
            Options.Add(GSUI.Checkbox("Skip Tutorials".Translate(), false, "Skip Tutorials"));
            Options.Add(GSUI.Checkbox("Debug Log".Translate(), false, "Debug Log"));
            _cheatModeCheckbox = Options.Add(GSUI.Checkbox("Cheat Mode".Translate(), false, "Cheat Mode"));
            Options.Add(GSUI.Input("Export Filename".Translate(), "My First Custom Galaxy", "Export Filename"));
            Options.Add(GSUI.Checkbox("Minify Exported JSON".Translate(), false, "Minify JSON"));
            Options.Add(GSUI.Checkbox("(Test) Fix CopyPaste Inserter Length".Translate(), true, "Fix CopyPaste"));
            Options.Add(GSUI.Button("Export All Themes".Translate(), "Export", ExportAllThemes));
            //Options.Add(GSUI.Checkbox("Adjust Inserter Length ", false, "Test"));
            //Options.Add(GSUI.Input("Inserter Length Adjust", "0", "TestNum"));
            //Options.Add(GSUI.Button("Debug ThemeSelector", "Go", FixOrbits));

            _exportButton = Options.Add(GSUI.Button("Export Custom Galaxy".Translate(), "Export".Translate(),
                ExportJsonGalaxy));
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
            GS2.Warn($"Generator Callback:{(int)result}");
            GS2.ActiveGenerator = GS2.Generators[(int)result];
            GS2.Warn("Active Generator = " + GS2.ActiveGenerator.Name);
            foreach (var canvas in SettingsUI.GeneratorCanvases)
                canvas.gameObject.SetActive(false);
            GS2.Warn("They have been set inactive");
            GS2.Warn(SettingsUI.GeneratorCanvases.Count + " count , trying to set "+(int)result);
            SettingsUI.GeneratorCanvases[(int)result].gameObject.SetActive(true);
            GS2.Warn("Correct one set active");
            SettingsUI.GeneratorIndex = (int)result;
            GS2.Warn("Gen Index Set");
            SettingsUI.UpdateContentRect();
            GS2.Warn("Updated ContentRect");
            GS2.SavePreferences();
            GS2.Warn("Preferences Saved");
        }

        private void ExportJsonGalaxy(Val o)
        {
            var outputDir = Path.Combine(GS2.DataDir, "CustomGalaxies");
            var path = Path.Combine(outputDir, Preferences.GetString("Export Filename", "My First Galaxy") + ".json");

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
            GS2.Warn("Setting External Themes");
            GS2.WarnJson(e.Get());
            var themeNames = e.Get();
            Preferences.Set("External Themes", themeNames);
        }

        public void SetUseExternalThemes(bool val)
        {
            Preferences.Set("Use External Themes", val);
        }
    }
}