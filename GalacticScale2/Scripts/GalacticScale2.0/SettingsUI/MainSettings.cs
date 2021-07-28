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
        private GSUI _generatorsCombobox;
        public GSGenPreferences Preferences = new GSGenPreferences();
        public bool ForceRare => Preferences.GetBool("Force Rare Spawn", false);
        public bool DebugMode => Preferences.GetBool("Debug Log", false);
        public bool Dev => Preferences.GetBool("Dev", false);
        public bool SkipPrologue => Preferences.GetBool("Skip Prologue", false);
        public bool SkipTutorials => Preferences.GetBool("Skip Tutorials", false);
        public bool CheatMode => Preferences.GetBool("Cheat Mode", false);
        public bool MinifyJson => Preferences.GetBool("Minify JSON", false);
        public bool FixCopyPaste => Preferences.GetBool("Fix CopyPaste", true);
        public string GeneratorID => Preferences.GetString("Generator ID", "space.customizing.generators.vanilla");
        public string Name => "Main Settings";

        public string Author => "innominata";

        public string Description => "Main Settings";

        public string Version => "1";

        public string GUID => "main.settings";

        public GSGeneratorConfig Config => new GSGeneratorConfig();

        public GSOptions Options { get; } = new GSOptions();
        public bool Test => Preferences.GetBool("Test", false);
        public float TestNum => Preferences.GetFloat("TestNum", 0f);

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
            // GS2.Warn("!");
            Preferences = preferences;
            var id = Preferences.GetString("Generator ID", "space.customizing.generators.vanilla");
            GS2.ActiveGenerator = GS2.GetGeneratorByID(id);
            Preferences.Set("Generator", _generatorNames.IndexOf(GS2.ActiveGenerator.Name));
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
            Options.Add(GSUI.Checkbox("Adjust Inserter Length ", false, "Test"));
            Options.Add(GSUI.Input("Inserter Length Adjust", "0", "TestNum"));
            Options.Add(GSUI.Button("Debug ThemeSelector", "Go", FixOrbits));

            _exportButton = Options.Add(GSUI.Button("Export Custom Galaxy".Translate(), "Export".Translate(), ExportJsonGalaxy));
        }

        private static void FixOrbits(Val o)
        {
            var v =GS2.themeSelector.Get();
            foreach (var c in v)
            {
                GS2.Warn($"::{c.Value.Name}");
            }
        }
        // private static void FixOrbits(Val o)
        // {
        //     if (GS2.Vanilla || GS2.IsMenuDemo) return;
        //     if (GameMain.localPlanet == null) return;
        //     if (GS2.galaxy == null || GS2.galaxy.stars == null) return;
        //     foreach (var star in GS2.galaxy.stars)
        //     foreach (var planet in star.planets)
        //         if (planet.orbitalPeriod >= 999f && planet.orbitalPeriod <= 1001f)
        //         {
        //             var gsPlanet = GS2.GetGSPlanet(planet);
        //             planet.orbitalPeriod = 50000f;
        //             gsPlanet.OrbitalPeriod = 50000f;
        //             GS2.Warn($"Fixing {planet.name}: New Orbit Period:{planet.orbitalPeriod}");
        //         }
        // }

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
    }
}