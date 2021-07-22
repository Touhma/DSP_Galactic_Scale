using System.Collections.Generic;
using System.IO;
using GalacticScale.Generators;

namespace GalacticScale
{
    public class GS2MainSettings : iConfigurableGenerator

    {
        private GSUI _cheatModeCheckbox;

        private GSUI _exportButton;

        // private GSUI GeneratorCombobox;
        private List<string> _generatorNames;
        public GSGenPreferences Preferences = new GSGenPreferences();
        public bool ForceRare => Preferences.GetBool("Force Rare Spawn");
        public bool DebugMode => Preferences.GetBool("Debug Mode");
        public bool SkipPrologue => Preferences.GetBool("Skip Prologue");
        public bool SkipTutorials => Preferences.GetBool("Skip Tutorials");
        public bool CheatMode => Preferences.GetBool("Cheat Mode");
        public bool MinifyJson => Preferences.GetBool("Minify JSON");
        public string GeneratorID => Preferences.GetString("Generator ID", "space.customizing.generators.vanilla");
        public string Name => "Main Settings";

        public string Author => "innominata";

        public string Description => "Main Settings";

        public string Version => "1";

        public string GUID => "main.settings";

        public GSGeneratorConfig Config => new GSGeneratorConfig();

        public GSOptions Options { get; } = new GSOptions();

        public GSGenPreferences Export()
        {
            GS2.Warn("!");
            Preferences.Set("Generator ID", GS2.ActiveGenerator.GUID);
            return Preferences;
        }

        public void Generate(int starCount)
        {
        }

        public void Import(GSGenPreferences preferences)
        {
            GS2.Warn("!");
            Preferences = preferences;
            var id = Preferences.GetString("Generator ID", "space.customizing.generators.vanilla");
            GS2.ActiveGenerator = GS2.GetGeneratorByID(id);
            Preferences.Set("Generator", _generatorNames.IndexOf(GS2.ActiveGenerator.Name));
        }

        public void Init()
        {
            GS2.Warn("!");
            _generatorNames = GS2.Generators.ConvertAll(iGen => iGen.Name);
            Options.Add(GSUI.Combobox("Generator".Translate(), _generatorNames, 0, "Generator", GeneratorCallback));
            Options.Add(GSUI.Checkbox("Force Rare Spawn".Translate(), false, "Force Rare Spawn"));
            Options.Add(GSUI.Checkbox("Skip Prologue".Translate(), false, "Skip Prologue"));
            Options.Add(GSUI.Checkbox("Skip Tutorials".Translate(), false, "Skip Tutorials"));
            Options.Add(GSUI.Checkbox("Debug Log".Translate(), false, "Debug Log"));
            _cheatModeCheckbox = Options.Add(GSUI.Checkbox("Cheat Mode".Translate(), false, "Cheat Mode"));
            Options.Add(GSUI.Input("Export Filename".Translate(), "My First Custom Galaxy", "Export Filename"));
            Options.Add(GSUI.Checkbox("Minify Exported JSON".Translate(), false, "Minify JSON"));
            _exportButton = Options.Add(GSUI.Button("Export Custom Galaxy", "Export", ExportJsonGalaxy));
        }

        private static void GeneratorCallback(Val result)
        {
            GS2.ActiveGenerator = GS2.Generators[result];
            foreach (var canvas in SettingsUI.GeneratorCanvases)
                canvas.gameObject.SetActive(false);
            SettingsUI.GeneratorCanvases[result].gameObject.SetActive(true);
            SettingsUI.GeneratorIndex = result;
            SettingsUI.UpdateContentRect();
            GS2.SavePreferences();
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
                    $"Galaxy Saved to ".Translate() + path,
                    "Woohoo!".Translate(), 1);
                JsonImport JsonGenerator = (JsonImport)GS2.GetGeneratorByID("space.customizing.generators.customjson");
                JsonGenerator.RefreshFileNames();
                return;
            }

            UIMessageBox.Show("Error".Translate(),
                "Please try again after creating a galaxy :)\r\nStart a game, then press ESC and click settings.".Translate(),
                "D'oh!".Translate(), 2);
        }


        public void DisableCheatMode()
        {
            _cheatModeCheckbox.Set(false);
        }
    }
}