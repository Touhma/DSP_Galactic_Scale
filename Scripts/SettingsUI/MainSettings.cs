﻿using System.Collections.Generic;
using System.IO;
using GSSerializer;
using UnityEngine;
using static GalacticScale.GS3;

namespace GalacticScale
{
    public class GS3MainSettings : iConfigurableGenerator

    {
        private static GSUI _generatorsCombobox;
        private GSUI _cheatModeCheckbox;

        private GSUI _exportButton;

        // private GSUI GeneratorCombobox;
        private List<string> _generatorNames = new();
        public List<string> filenames = new();
        public GSUI JsonGalaxies;
        public GSGenPreferences Preferences = new();
        public Dictionary<string, GSUI> ThemeCheckboxes = new();
        public bool ForceRare => Preferences.GetBool("Force Rare Spawn");
        public bool DebugMode => Preferences.GetBool("Debug Log");
        public bool NewGravity => Preferences.GetBool("New Gravity", false);
        public float VirtualStarmapPlanetScaleFactor => Preferences.GetFloat("VSPlanetScaleFactor", 5f);
        public float VirtualStarmapStarScaleFactor => Preferences.GetFloat("VSStarScaleFactor", 0.5f);
        public float VirtualStarmapOrbitScaleFactor => Preferences.GetFloat("VSOrbitScaleFactor", 5f);
        public float VirtualStarmapClickTolerance => Preferences.GetFloat("VSClickTolerance", 3f);
        public bool Dev => Preferences.GetBool("Dev");
        public string ImportFilename => Preferences.GetString("Import Filename");
        public bool SkipPrologue => Preferences.GetBool("Skip Prologue", true);
        public bool SkipTutorials => Preferences.GetBool("Skip Tutorials");
        public bool ScarletRevert => Preferences.GetBool("RevertScarlet");
        public bool CheatMode => Preferences.GetBool("Cheat Mode");
        public float MechaScale => Preferences.GetFloat("MechaScale", 1f);

        // public bool VanillaGrid => Preferences.GetBool("Vanilla Grid");
        public bool MinifyJson
        {
            get => Preferences.GetBool("Minify JSON");
            set => Preferences.Set("Minify JSON", value);
        }

        public bool FixCopyPaste => true; //Preferences.GetBool("Fix CopyPaste", true);
        public string GeneratorID => Preferences.GetString("Generator ID", "space.customizing.generators.gs3");
        public bool UseExternalThemes => Preferences.GetBool("Use External Themes");

        public Dictionary<int, bool> VeinTips
        {
            get
            {
                var veintips = new Dictionary<int, bool>();
                for (var i = 1; i < 15; i++) veintips.Add(i, Preferences.GetBool($"veinTip{i}", true));

                return veintips;
            }
        }

        public float ResourceMultiplier => Preferences.GetFloat("Resource Multiplier", 1f);
        public float LogisticsShipMulti => Preferences.GetFloat("Logistics Ship Multi", 1f);
        public List<string> ExternalThemeNames => Preferences.GetStringList("External Themes", new List<string>());
        public string Name => "Main Settings";

        public string Author => "innominata";

        public string Description => "Main Settings";

        public string Version => "1";

        public string GUID => "main.settings";

        public GSGeneratorConfig Config => new();

        public GSOptions Options { get; } = new();
        //public bool Test => Preferences.GetBool("Test", false);
        //public float TestNum => Preferences.GetFloat("TestNum", 0f);

        public void OnUpdate(string key, Val val)
        {
            Preferences.Set(key, val);
        }

        public GSGenPreferences Export()
        {
            Preferences.Set("Generator ID", ActiveGenerator.GUID);
            Preferences.Set("Generator", _generatorNames.IndexOf(ActiveGenerator.Name));
            // foreach (var x in Preferences) GS3.Log($"Key:{x.Key} Value:{x.Value}");
            return Preferences;
        }

        public void Generate(int starCount, StarData birthStar = null)
        {
            _generatorNames = GS3.Generators.ConvertAll(iGen => iGen.Name);
            _generatorsCombobox?.SetItems(_generatorNames);
        }

        public void Import(GSGenPreferences preferences)
        {
            // GS3.Log("*");
            Preferences = preferences;
            // GS3.Log("*");

            var id = Preferences.GetString("Generator ID", "space.customizing.generators.gs3");
            // GS3.Log($"* { id}");

            ActiveGenerator = GetGeneratorByID(id);
            // GS3.Log($"* {ActiveGenerator.Name}");
            // LogJson(_generatorNames);
            Preferences.Set("Generator", _generatorNames.IndexOf(ActiveGenerator.Name));
            // GS3.Log($"* {_generatorNames.IndexOf(ActiveGenerator.Name)}");

            if (!filenames.Contains(Preferences.GetString("Import Filename", null))) Preferences.Set("Import Filename", filenames[0]);
            // GS3.Log("*");
        }

        public void Init()
        {
            GS3.Warn("!");
            _generatorNames = GS3.Generators.ConvertAll(iGen => iGen.Name);
            // GS3.LogJson(_generatorNames, true);
            _generatorsCombobox = Options.Add(GSUI.Combobox("Generator".Translate(), _generatorNames, 0, "Generator", GeneratorCallback, "Try them all!".Translate()));
            RefreshFileNames();

            // Options.Add(GSUI.Checkbox("Vanilla Grid (200r)".Translate(), false, "Vanilla Grid", VanillaGridCheckboxCallback, "Use the vanilla grid for 200 size planets".Translate()));

            var VeinOptions = new GSOptions();
            VeinOptions.Add(GSUI.Spacer());
            VeinOptions.Add(GSUI.Checkbox("Show Iron Vein Labels".Translate(), true, "veinTip1", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Copper Vein Labels".Translate(), true, "veinTip2", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Silicon Vein Labels".Translate(), true, "veinTip3", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Titanium Vein Labels".Translate(), true, "veinTip4", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Stone Vein Labels".Translate(), true, "veinTip5", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Coal Vein Labels".Translate(), true, "veinTip6", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Oil Vein Labels".Translate(), true, "veinTip7", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Fire Ice Vein Labels".Translate(), true, "veinTip8", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Kimberlite Labels".Translate(), true, "veinTip9", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Fractal Silicon Vein Labels".Translate(), true, "veinTip10", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Organic Crystal Vein Labels".Translate(), true, "veinTip11", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Optical Grating Vein Labels".Translate(), true, "veinTip12", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Spiriform Vein Labels".Translate(), true, "veinTip13", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Checkbox("Show Unipolar Vein Labels".Translate(), true, "veinTip14", UpdateVeinDetail, "When show vein markers is enabled".Translate()));
            VeinOptions.Add(GSUI.Spacer());

            var GameOptions = new GSOptions();
            GameOptions.Add(GSUI.Spacer());
            GameOptions.Add(GSUI.Checkbox("Skip Prologue".Translate(), true, "Skip Prologue"));
            GameOptions.Add(GSUI.Checkbox("Skip Tutorials".Translate(), false, "Skip Tutorials"));
            GameOptions.Add(GSUI.Group("Show/Hide Vein Labels".Translate(), VeinOptions, "Useful for finding veins".Translate()));
            GameOptions.Add(GSUI.Spacer());
            Options.Add(GSUI.Group("Quality of Life".Translate(), GameOptions, "Useful settings".Translate()));
            Options.Add(GSUI.Spacer());

            var DebugOptions = new GSOptions();
            // DebugOptions.Add(GSUI.Button("Debug", "Go", (o) => { GS3.Warn(GameMain.localPlanet.runtimePosition + " " + GameMain.localStar.uPosition); }, null, null));
            DebugOptions.Add(GSUI.Spacer());
            DebugOptions.Add(GSUI.Checkbox("Debug Log".Translate(), false, "Debug Log", null, "Print extra logs to BepInEx console".Translate()));
            DebugOptions.Add(GSUI.Checkbox("New Gravity Mechanics".Translate(), false, "New Gravity", null, "Large planets attract a lot more. Can cause issues with large planets".Translate()));
            DebugOptions.Add(GSUI.Checkbox("Force Rare Spawn".Translate(), false, "Force Rare Spawn", null, "Ignore randomness/distance checks".Translate()));
            _cheatModeCheckbox = DebugOptions.Add(GSUI.Checkbox("Enable Teleport".Translate(), false, "Cheat Mode", null, "TP by ctrl-click nav arrow in star map".Translate()));
            DebugOptions.Add(GSUI.Slider("Mecha Scale".Translate(), 0.1f, 1f, 10f, 0.1f, "MechaScale", ScaleMecha, "How big Icarus should be. 1 = default".Translate()));
            DebugOptions.Add(GSUI.Button("Reset Binary Star Position".Translate(), "Go", ResetBinaryStars, null, "Try and move binary stars to new position relative to main star".Translate()));
            DebugOptions.Add(GSUI.Slider("Ship Speed Multiplier".Translate(), 1f, 1f, 100f, "Logistics Ship Multi", null, "Multiplier for Warp Speed of Ships".Translate()));
            DebugOptions.Add(GSUI.Slider("GalaxySelect Planet ScaleFactor".Translate(), 0.1f, 0.6f, 100f, 0.1f, "VSPlanetScaleFactor", null, "How big planets should be in the new game system view".Translate()));
            DebugOptions.Add(GSUI.Slider("GalaxySelect Star ScaleFactor".Translate(), 0.1f, 0.6f, 100f, 0.1f, "VSStarScaleFactor", null, "How big star should be in the new game system view".Translate()));
            DebugOptions.Add(GSUI.Slider("GalaxySelect Orbit ScaleFactor".Translate(), 0.1f, 5f, 100f, 0.1f, "VSOrbitScaleFactor", null, "How spaced orbits should be in the new game system view".Translate()));
            DebugOptions.Add(GSUI.Slider("GalaxySelect Click Tolerance".Translate(), 1f, 3f, 10f, 0.1f, "VSClickTolerance", null, "How close to a star/planet your mouse needs to be to register a click".Translate()));

            DebugOptions.Add(GSUI.Button("Set ResourceMulti Infinite".Translate(), "Now", o =>
            {
                // GS3.WarnJson(gameDesc);
                gameDesc.resourceMultiplier = 100;
                GSSettings.GalaxyParams.resourceMulti = 100;
                //GS3.WarnJson(gameDesc.resourceMultiplier);
            }, null, "Will need to be saved and loaded to apply".Translate()));
            DebugOptions.Add(GSUI.Checkbox("Revert Scarlet Ice Lake".Translate(), false, "RevertScarlet", null, "2.2.0.23 Had a bug where Ice Lake had no terrain. Enable this to fix issues with saves started prior to .24".Translate()));
            // DebugOptions.Add(GSUI.Button("Test", "Now", (o) =>
            // {
            //     Warn("ExternalThemes:");
            //     GS3.WarnJson(externalThemes.Select(p=>p.Key).ToList());
            //     Warn("GS3.ThemeLibrary:");
            //     GS3.WarnJson(GS3.ThemeLibrary.Select(p=>p.Key).ToList());
            //     Warn("GSSettings.ThemeLibrary:");
            //     GS3.WarnJson(GSSettings.ThemeLibrary.Select(p=>p.Key).ToList());
            // }));
            DebugOptions.Add(GSUI.Spacer());
            DebugOptions.Add(GSUI.Button("Reset Logistic Bot Speed".Translate(), "Reset",
                (_) =>
                {
                    GameMain.data.history.logisticCourierSpeed = Configs.freeMode.logisticCourierSpeed;
                    GameMain.data.history.logisticCourierSpeedScale = 1f;
                    GameMain.data.history.logisticCourierCarries = Configs.freeMode.logisticCourierCarries;
                    GameMain.data.history.dispenserDeliveryMaxAngle = Configs.freeMode.dispenserDeliveryMaxAngle;
                    if (GameMain.data.history.techStates[3401].unlocked)
                    {
                        GameMain.data.history.logisticCourierSpeedScale += 0.1f;
                    }
                    if (GameMain.data.history.techStates[3402].unlocked)
                    {
                        GameMain.data.history.logisticCourierSpeedScale += 0.1f;
                    }
                    if (GameMain.data.history.techStates[3403].unlocked)
                    {
                        GameMain.data.history.logisticCourierSpeedScale += 0.15f;
                    }
                    if (GameMain.data.history.techStates[3404].unlocked)
                    {
                        GameMain.data.history.logisticCourierSpeedScale += 0.15f;
                    }
                    if (GameMain.data.history.techStates[3405].unlocked)
                    {
                        GameMain.data.history.logisticCourierSpeedScale += 0.15f;
                    }
                    if (GameMain.data.history.techStates[3406].unlocked)
                    {
                        GameMain.data.history.logisticCourierSpeedScale += 0.15f;
                    }
                    GameMain.data.history.logisticCourierSpeedScale += 0.2f * (float)(GameMain.data.history.techStates[3407].curLevel - 7);
                    for (int num12 = 3501; num12 <= 3507; num12++)
                    {
                        if (GameMain.data.history.techStates[num12].unlocked)
                        {
                            GameMain.data.history.logisticCourierCarries++;
                        }
                    }
                }, null, "Fix games created on 2.8.0"));
        
            
            Options.Add(GSUI.Group("Debug Options".Translate(), DebugOptions, "Useful for testing galaxies/themes".Translate()));
            Options.Add(GSUI.Spacer());

            var JsonOptions = new GSOptions();
            DebugOptions.Add(GSUI.Spacer());
            JsonOptions.Add(GSUI.Input("Export Filename".Translate(), "My First Custom Galaxy", "Export Filename", null, "Excluding .json".Translate()));
            JsonOptions.Add(GSUI.Checkbox("Minify Exported JSON".Translate(), false, "Minify JSON", null, "Only save changes".Translate()));
            _exportButton = JsonOptions.Add(GSUI.Button("Export Custom Galaxy".Translate(), "Export".Translate(), ExportJsonGalaxy, null, "Save Galaxy to File".Translate()));
            JsonGalaxies = JsonOptions.Add(GSUI.Combobox("Custom Galaxy".Translate(), filenames, CustomFileSelectorCallback, CustomFileSelectorPostfix));
            JsonOptions.Add(GSUI.Button("Load Custom Galaxy".Translate(), "Load", LoadJsonGalaxy, null, "Will end current game".Translate()));
            DebugOptions.Add(GSUI.Spacer());

            Options.Add(GSUI.Group("Custom Galaxy Export/Import".Translate(), JsonOptions, "Export available once in game".Translate()));
        }

        public void EnableDevMode()
        {
            Warn("Toggling Dev Mode");
            Preferences.Set("Dev", !Preferences.GetBool("Dev"));
            Warn($"Dev Mode: {Preferences.GetBool("Dev")}");
            SavePreferences();
        }

        public void ScaleMecha(Val o)
        {
            if (GameMain.mainPlayer != null) GameMain.mainPlayer.transform.localScale = Vector3.one * Preferences.GetFloat("MechaScale", 1f);
        }

        public void ResetBinaryStars(Val o)
        {
            var random = new GS3.Random(GSSettings.Seed);
            foreach (var star in GSSettings.Stars)
                if (star.BinaryCompanion != null)
                {
                    var binary = GetGSStar(star.BinaryCompanion);
                    if (binary == null)
                    {
                        Error($"Could not find Binary Companion:{star.BinaryCompanion}");
                        continue;
                    }

                    var offset = star.genData.Get("binaryOffset", (star.RadiusLY * 2 + binary.RadiusLY * 2) * Preferences.GetFloat("binaryDistanceMulti", 1f) * random.NextFloat(1.1f, 1.3f));
                    binary.position = new VectorLF3(offset, 0, 0);
                    Log($"Moving Companion Star {star.BinaryCompanion} who has offset {binary.position}");
                    // GS3.Warn("Setting Binary Star Position");
                    galaxy.stars[binary.assignedIndex].position = binary.position = star.position + binary.position;
                    galaxy.stars[binary.assignedIndex].uPosition = galaxy.stars[binary.assignedIndex].position * 2400000.0;
                    Log($"Host ({star.Name})Position:{star.position} . Companion ({binary.Name}) Position {binary.position}");
                }
        }

        public void UpdateVeinDetail(Val o)
        {
            if (GameMain.isRunning && !DSPGame.IsMenuDemo && GameMain.localPlanet != null && UIRoot.instance?.uiGame?.veinDetail != null) UIRoot.instance.uiGame.veinDetail.SetInspectPlanet(GameMain.localPlanet);
        }

        public void SetResourceMultiplier(float val)
        {
            Preferences.Set("Resource Multiplier", val);
            // SavePreferences();
        }

        public void SetExternalTheme(string folder, string name, bool value)
        {
            // GS3.Warn($"Setting External Theme:{folder}|{name} to {value}");
            var ExternalThemesList = Preferences.GetStringList("External Themes", new List<string>());
            var key = $"{folder}|{name}";
            if (ExternalThemesList.Contains(key))
            {
                if (!value) ExternalThemesList.Remove(key);
            }
            else
            {
                if (value)
                    // GS3.Warn("*****Adding ExternalTheme*******" + key);
                    ExternalThemesList.Add(key);
            }

            Preferences.Set("External Themes", ExternalThemesList);
        }

        public void InitThemePanel()
        {
            var externalThemesGroupOptions = new List<GSUI>
            {
                GSUI.Button("Export All Themes".Translate(), "Export".Translate(), ExportAllThemes)
            };
            // GS3.Warn("ExternalThemeNames");
            // GS3.WarnJson(ExternalThemeNames);
            foreach (var themelibrary in availableExternalThemes)
            {
                var Folder = themelibrary.Key;
                var Library = themelibrary.Value;
                if (Folder == "Root")
                {
                    foreach (var theme in Library)
                    {
                        var key = $"{Folder}|{theme.Key}";
                        // GS3.Warn("Checking Key");
                        var def = false;
                        if (ExternalThemeNames.Contains(key)) def = true;
                        else Log($"Doesnt Contain {key}");
                        // GS3.Log($"Setting {key} to {def}");
                        GSOptionCallback callback = o =>
                        {
                            // GS3.Log(key + " " + o); 
                            SetExternalTheme(Folder, theme.Key, o);
                        };
                        GSUI checkbox = null;
                        GSOptionPostfix postfix = () =>
                        {
                            // GS3.Log($"Executing Postfix for {key}");
                            ThemeCheckboxes[key].RectTransform.GetComponent<GSUIToggle>().Value = def;
                            // GS3.Log($"Now the value is {ThemeCheckboxes[key]?.RectTransform.GetComponent<GSUIToggle>().Value}");
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
                        var def = false;
                        var key = $"{Folder}|{theme.Key}";
                        if (ExternalThemeNames.Contains(key)) def = true;
                        GSOptionPostfix postfix = () =>
                        {
                            // GS3.Log($"Executing Postfix for {key}");
                            ThemeCheckboxes[key].RectTransform.GetComponent<GSUIToggle>().Value = def;
                            // GS3.Log($"Now the value is {ThemeCheckboxes[key]?.RectTransform.GetComponent<GSUIToggle>().Value}");
                        };
                        var checkbox = GSUI.Checkbox("  " + theme.Key, def, o =>
                        {
                            // GS3.Log(Folder + "|" + theme.Key + " " + o);
                            SetExternalTheme(Folder, theme.Key, o);
                        }, postfix);
                        if (!ThemeCheckboxes.ContainsKey(key)) ThemeCheckboxes.Add(key, checkbox);
                        listoptions.Add(checkbox);
                    }


                    var FolderGroup = GSUI.Group(Folder, listoptions, null, true, true, o =>
                    {
                        //Warn(o);
                        foreach (var option in listoptions)
                            option.Set(o);
                        //option.RectTransform.GetComponent<GSUIToggle>().Value = o;
                    });
                    externalThemesGroupOptions.Add(FolderGroup);
                }
            }

            Options.Add(GSUI.Spacer());
            var externalThemesGroup = Options.Add(GSUI.Group("External Themes".Translate(), externalThemesGroupOptions));
        }

        private void ExportAllThemes(Val o)
        {
            if (GameMain.isPaused)
            {
                var path = Path.Combine(DataDir, "ExportedThemes");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                foreach (var theme in GSSettings.ThemeLibrary)
                {
                    var filename = Path.Combine(path, theme.Value.Name + ".json");
                    var fs = new fsSerializer();
                    fs.TrySerialize(theme.Value, out var data);
                    var json = fsJsonPrinter.PrettyJson(data);
                    File.WriteAllText(filename, json);
                }

                UIMessageBox.Show("Success".Translate(), "Themes have been exported to ".Translate() + path + "/", "Noted!".Translate(), 2);
                return;
            }

            UIMessageBox.Show("Error".Translate(), "Please try again after creating a galaxy :)\r\nStart a game, then press ESC and click settings.".Translate(), "D'oh!".Translate(), 2);
        }


        private static void GeneratorCallback(Val result)
        {
            // if (GameMain.isPaused && result == 0)
            // {
            //     UIMessageBox.Show("Note".Translate(), "You cannot change the generator to vanilla while in game.".Translate(), "Of course not!".Translate(), 2);
            //     var genIndex = GS3.Generators.IndexOf(ActiveGenerator);
            //     SettingsUI.GeneratorIndex = (int)result;
            //
            //     return;
            // }

            ActiveGenerator = GS3.Generators[(int)result];
            GSEvents.GeneratorChange(ActiveGenerator);
            UpdateNebulaSettings();


            // Warn("Active Generator = " + ActiveGenerator.Name);
            foreach (var canvas in SettingsUI.GeneratorCanvases)
                canvas.gameObject.SetActive(false);
            // GS3.Warn("They have been set inactive");
            //Warn(SettingsUI.GeneratorCanvases.Count + " count , trying to set " + (int)result);
            SettingsUI.GeneratorCanvases[(int)result].gameObject.SetActive(true);
            // GS3.Warn("Correct one set active");
            SettingsUI.GeneratorIndex = (int)result;
            // GS3.Warn("Gen Index Set");
            //SettingsUI.UpdateContentRect();
            // GS3.Warn("Updated ContentRect");
            // SavePreferences();
            // GS3.Warn("Preferences Saved");
        }

        private void ExportJsonGalaxy(Val o)
        {
            var outputDir = Path.Combine(DataDir, "CustomGalaxies");
            var path = Path.Combine(outputDir, Preferences.GetString("Export Filename", "My First Galaxy") + ".json");

            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            if (GameMain.isPaused)
            {
                Log("Exporting Galaxy");
                DumpObjectToJson(path, GSSettings.Instance);
                Log("Exported");
                UIMessageBox.Show("Success".Translate(), "Galaxy Saved to ".Translate() + path, "Woohoo!".Translate(), 1);
                RefreshFileNames();
                return;
            }

            UIMessageBox.Show("Error".Translate(), "Please try again after creating a galaxy :)\r\nStart a game, then press ESC and click settings.".Translate(), "D'oh!".Translate(), 2);
        }


        public void DisableCheatMode()
        {
            _cheatModeCheckbox.Set(false);
        }

        public void SetExternalThemes(ExternalThemeSelector e)
        {
            // GS3.Warn("Setting External Themes");
            // GS3.WarnJson(e.Get());
            // var themeNames = e.Get();
            // Preferences.Set("External Themes", themeNames);
        }

        public void SetUseExternalThemes(bool val)
        {
            Preferences.Set("Use External Themes", val);
        }

        public void LoadJsonGalaxy(Val o)
        {
            var ImportFilename = Preferences.GetString("Import Filename", null);
            if (o != "Click") ImportFilename = "Pasta";
            if (string.IsNullOrEmpty(ImportFilename))
            {
                UIMessageBox.Show("Error".Translate(), "To use the Custom JSON Generator you must select a file to load.".Translate(), "Ok".Translate(), 1);
                RefreshFileNames();
                return;
            }

            GSSettings.Reset(GSSettings.Seed);
            var path = Path.Combine(Path.Combine(DataDir, "CustomGalaxies"), ImportFilename + ".json");
            GSSettings.ThemeLibrary = ThemeLibrary.Vanilla();
            //Warn(GSSettings.StarCount.ToString());
            LoadSettingsFromJson(path);
            Log("Generator:Json|Generate|End");
            GSSettings.Instance.imported = true;
            // GS3.gameDesc.playerProto = 2;
            //WarnJson(GSSettings.Instance.galaxyParams);
            //WarnJson(GSSettings.StarCount);
            gsStars.Clear();
            gsPlanets.Clear();
            //Warn(GSSettings.StarCount.ToString());

            // GS3.galaxy = new GalaxyData();
            var gameDesc = new GameDesc();
            gameDesc.SetForNewGame(UniverseGen.algoVersion, 1, 1, 1, 1f);
            if (GS3.Config.SkipPrologue) DSPGame.StartGameSkipPrologue(gameDesc);
            else DSPGame.StartGame(gameDesc);
            // UniverseGen.CreateGalaxy(GS3.gameDesc);
        }

        public void RefreshFileNames()
        {
            //GS3.Log("Refreshing Filenames");
            var customGalaxiesPath = Path.Combine(DataDir, "CustomGalaxies");
            if (!Directory.Exists(customGalaxiesPath)) Directory.CreateDirectory(customGalaxiesPath);

            filenames = new List<string>(Directory.GetFiles(customGalaxiesPath, "*.json")).ConvertAll(Path.GetFileNameWithoutExtension);
            if (filenames.Count == 0)
                filenames.Add("No Files Found"); //foreach (string n in filenames) GS3.Log("File:" + n);
            if (JsonGalaxies != null) JsonGalaxies.RectTransform.GetComponentInChildren<GSUIDropdown>().Items = filenames;
        }

        private void CustomFileSelectorCallback(Val result)
        {
            int index = result;
            if (index > filenames.Count - 1) index = 0;
            Preferences.Set("Import Filename", filenames[index]);
            if (Preferences.GetString("Import Filename", null) == "No Files Found") Preferences.Set("Import Filename", "");
            RefreshFileNames();
        }

        private void CustomFileSelectorPostfix()
        {
            Log("Json:Postfix");
            var index = 0;
            for (var i = 0; i < filenames.Count; i++)
                if (Preferences.GetString("Import Filename", null) == filenames[i])
                    index = i;
            JsonGalaxies.RectTransform.GetComponentInChildren<GSUIDropdown>().Value = index;
        }
    }
}