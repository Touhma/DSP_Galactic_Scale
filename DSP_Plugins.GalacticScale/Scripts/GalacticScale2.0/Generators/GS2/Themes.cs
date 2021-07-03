using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
namespace GalacticScale.Generators
{
    public partial class GS2Generator : iConfigurableGenerator
    {
        public static string[] baseKeys;
        public void SetupBaseThemes()
        {
            ThemeLibrary newLibrary = new ThemeLibrary();
            foreach(var v in ThemeLibrary.Vanilla())
            {
                GSTheme clone = v.Value.Clone();
                //GS2.Log("Adding Theme " + clone.Name + " themeCount:" + newLibrary.Count);
                newLibrary.Add(v.Key, clone);
            }
            baseKeys = new string[newLibrary.Keys.Count];
            //GS2.Warn(newLibrary.Count.ToString());
            newLibrary.Keys.CopyTo(baseKeys, 0);
            //foreach (var k in baseKeys) GS2.Log("BaseKey:" +k);
            //foreach (var old in GS2.ThemeLibrary)
            //{
            //    GSTheme newTheme = old.Value.Clone();
            //    newTheme.Name += "_";
            //    GS2.Log($"Cloned {old.Key} to {newTheme.Name}");
            //    newTheme.DisplayName = old.Value.DisplayName;
            //    newLibrary.Add(newTheme.Name, newTheme );
            //}
            ThemeLibrary smolLibrary = new ThemeLibrary();
            string[] keys = new string[newLibrary.Count + 1];
            newLibrary.Keys.CopyTo(keys, 0);
            for (var i = 0; i < newLibrary.Count; i++)
            {
                string key = keys[i];
                GSTheme theme = newLibrary[key];
                if (theme.PlanetType == EPlanetType.Ocean) theme.MinRadius = 50;
                if (theme.PlanetType != EPlanetType.Gas && theme.PlanetType != EPlanetType.Ocean)
                {
                    //For rocky worlds
                    GSTheme smolTheme = theme.Clone();
                    smolTheme.DisplayName = "Smol"+theme.DisplayName;
                    smolTheme.Name += "smol";
                    smolLibrary.Add(smolTheme.Name, smolTheme);
                    smolTheme.MaxRadius = 40;
                    theme.MinRadius = 50;
                    if (theme.PlanetType == EPlanetType.Vocano)
                    {
                        theme.TerrainSettings.BrightnessFix = true;
                        smolTheme.TerrainSettings.BrightnessFix = true;
                        theme.Init(); smolTheme.Init();
                    }
                    string[] cKeys = new string[100];
                    smolTheme.atmosphereMaterial.Colors.Keys.CopyTo(cKeys, 0);
                    for (var j = 0; j < smolTheme.atmosphereMaterial.Colors.Count; j++) smolTheme.atmosphereMaterial.Colors[cKeys[j]] = Color.clear;
                }


            }
            foreach (var s in smolLibrary)
            {
                if (!newLibrary.ContainsKey(s.Key)) newLibrary.Add(s.Key, s.Value);
                else newLibrary[s.Key] = s.Value;
            }
            GS2.ThemeLibrary = GSSettings.ThemeLibrary = newLibrary;
            
        }
        public static void InitThemes()
        {
            GS2.Log("Creating Themes");
            GSTheme oiler = new GSTheme("OilGiant", "SpaceWhale Excrement", "IceGiant");
            oiler.terrainMaterial.Tint = new Color(0.3f, 0.3f, 0.3f, 1f);
            oiler.atmosphereMaterial.Tint = new Color(0f, 0f, 0f, 1);
            oiler.thumbMaterial.Tint = new Color(0.01f, 0.005f, 0f, 0.001f);
            oiler.PlanetType = EPlanetType.Gas;
            oiler.TerrainSettings.Algorithm = "GSTA1";
            oiler.TerrainSettings.HeightMulti = 1.4;
            oiler.CustomGeneration = true;
            oiler.GasItems[0] = 1114;
            oiler.GasItems[1] = 1120;
            oiler.GasSpeeds[0] = 0.1f;
            oiler.GasSpeeds[1] = 10f;
            oiler.ThemeType = EThemeType.Private;
            oiler.Process();
            //GS2.Log("Oiler Processed");

            GSTheme obsidian = new GSTheme("Obsidian", "Obsidian", "IceGelisol");
            obsidian.terrainMaterial.Tint = new Color(0.05f, 0.05f, 0.05f, 1);
            obsidian.oceanMaterial.Tint = new Color(0.0f, 0.0f, 0.0f, 0.5f);
            obsidian.atmosphereMaterial.Tint = Color.black;
            obsidian.TerrainSettings.Algorithm = "GSTA3";
            obsidian.TerrainSettings.BiomeHeightMulti = -10f;
            obsidian.CustomGeneration = true;
            obsidian.AmbientSettings.CubeMap = "GS2";
            obsidian.terrainMat.SetFloat("_HeightEmissionRadius", 0f);
            obsidian.terrainMat.SetFloat("_EmissionStrength", 0f);
            obsidian.terrainMat.SetFloat("_NormalStrength", 0.3f);
            obsidian.terrainMat.SetFloat("_Distance", 0f);
            obsidian.Temperature = 2f;
            obsidian.Process();

            GSTheme hotObsidian = new GSTheme("HotObsidian", "Hot Obsidian", "Obsidian");
            hotObsidian.MaxRadius = 40;
            hotObsidian.MinRadius = 5;
            hotObsidian.terrainMaterial.Tint = new Color(0.2f, 0.05f, 0.05f, 1);
            hotObsidian.Temperature = 5f;
            hotObsidian.atmosphereMaterial.Tint = Color.clear;
            hotObsidian.Process();

            //GS2.Log("About to Process redIce");
            GSTheme iceMalusol = new GSTheme("IceMalusol", "Ice Malusol", "IceGelisol");
            iceMalusol.terrainMaterial.Textures.Add("_BioTex0A", "GS2|red-ice");
            iceMalusol.terrainMaterial.Textures.Add("_BioTex1A", "GS2|grey-rock");
            iceMalusol.terrainMaterial.Textures.Add("_BioTex2A", "GS2|grey-snow");
            iceMalusol.AmbientSettings.Reflections = new Color(1, 0, 0, 1);
            iceMalusol.AmbientSettings.CubeMap = "GS2";
            iceMalusol.Process();

            GSTheme acidGreenhouse = new GSTheme("AcidGreenhouse", "Acid Greenhouse", "VolcanicAsh");
            acidGreenhouse.atmosphereMaterial.Tint = new Color(0.5f, 0.4f, 0.0f, 0.8f);
            acidGreenhouse.atmosphereMaterial.Params = new Dictionary<string, float>()
            {
                ["_AtmoDensity"] = 1,
                ["_Cutoff"] = 0.1f,
                ["_FarFogDensity"] = 1f,
                ["_FogDensity"] = 1f,
                ["_FogSaturate"] = 1.8f,
                ["_Intensity"] = 1.8f,
                ["_Parallax"] = 0.02f,
                ["_RimFogExp"] = 1.3f,
                ["_RimFogPower"] = 3f,
                ["_SkyAtmosPower"] = 17f,
                ["_SunColorAdd"] = 40f,
                ["_SunColorSkyUse"] = 0.4f,
                ["_SunColorUse"] = 0.2f,
                ["_SunRiseScatterPower"] = 160f
            };
            acidGreenhouse.oceanMaterial.Tint = new Color(0.7f, 0.7f, 0.07f);
            acidGreenhouse.terrainMaterial.Tint = new Color(0.901f, 0.686f, 0.098f, 1);
            acidGreenhouse.thumbMaterial.Tint = new Color(0.901f, 0.686f, 0.098f, 1);
            acidGreenhouse.minimapMaterial.Tint = new Color(0.901f, 0.686f, 0.098f, 1);
            acidGreenhouse.terrainMaterial.CopyFrom = "AshenGelisol.terrainMat";
            acidGreenhouse.terrainMaterial.Colors = new Dictionary<string, Color>()
            {
                ["_AmbientColor0"] = new Color(0.9f, 0.9f, 0.1f, 1f),
                ["_AmbientColor0"] = new Color(0.8f, 0.8f, 0.7f, 1f),
                ["_AmbientColor0"] = new Color(0.5f, 0.5f, 0.0f, 1f),
                ["_Color"] = new Color(0.65f, 0.5f, 0.15f, 1f),
            };
            acidGreenhouse.terrainMaterial.Params = new Dictionary<string, float>()
            {
                ["_AmbientInc"] = 0.5f
            };
            acidGreenhouse.AmbientSettings.DustStrength1 = 10;
            acidGreenhouse.AmbientSettings.DustStrength2 = 10;
            acidGreenhouse.AmbientSettings.DustStrength3 = 10;
            acidGreenhouse.AmbientSettings.DustColor1 = new Color(0.95f, 0.75f, 0.25f, 1f); //new Color(0.38f, 0.38f, 0f, 1f);
            acidGreenhouse.AmbientSettings.DustColor2 = new Color(0.95f, 0.75f, 0.25f, 1f);
            acidGreenhouse.AmbientSettings.DustColor3 = new Color(0.9f, 0.7f, 0.2f, 1f);
            acidGreenhouse.AmbientSettings.LutContribution = 0.8f;
            acidGreenhouse.CustomGeneration = true;
            acidGreenhouse.TerrainSettings.BrightnessFix = true;
            acidGreenhouse.TerrainSettings.Algorithm = "GSTA3";
            acidGreenhouse.TerrainSettings.LandModifier = 1;
            acidGreenhouse.TerrainSettings.HeightMulti = 2;
            acidGreenhouse.VeinSettings.VeinTypes = new GSVeinTypes()
                {
                    GSVeinType.Generate(EVeinType.Iron, 1, 10, 0.5f, 1.5f, 5, 15, false ),
                    GSVeinType.Generate(EVeinType.Stone, 1, 10, 0.5f, 1.5f, 5, 15, false ),
                    GSVeinType.Generate(EVeinType.Copper, 1, 20, 0.5f, 1.5f, 5, 15, true ),
                    GSVeinType.Generate(EVeinType.Silicium, 1, 20, 0.9f, 2.5f, 5, 25, false ),
                    GSVeinType.Generate(EVeinType.Diamond, 1, 20, 0.5f, 1.5f, 5, 15, true ),
                    GSVeinType.Generate(EVeinType.Fractal, 1, 20, 0.5f, 1.5f, 5, 15, true ),
                };
            acidGreenhouse.VeinSettings.Algorithm = "GS2";
            acidGreenhouse.CustomGeneration = true;
            acidGreenhouse.MinRadius = 150;
            acidGreenhouse.Temperature = 3f;
            acidGreenhouse.ThemeType = EThemeType.Planet;
            acidGreenhouse.Process();

            GSTheme barrenSatellite = new GSTheme("BarrenSatellite", "Barren Satellite", "Barren");
            barrenSatellite.VegeSettings.Group1 = new List<string>() {
                    "Stone1","Stone2","Stone3","Stone4","Stone5","Stone6","Stone7","Stone8","Stone9","Stone10","Stone11","Stone12","Stone1","Stone2","Stone3","Stone4","Stone5","Stone6","Stone7","Stone8","Stone9","Stone10","Stone11","Stone12"};
            barrenSatellite.VegeSettings.Group2 = barrenSatellite.VegeSettings.Group1;
            barrenSatellite.VegeSettings.Group3 = barrenSatellite.VegeSettings.Group1;
            barrenSatellite.VegeSettings.Group4 = barrenSatellite.VegeSettings.Group1;
            barrenSatellite.VegeSettings.Group5 = barrenSatellite.VegeSettings.Group1;
            barrenSatellite.VegeSettings.Group6 = barrenSatellite.VegeSettings.Group1;
            barrenSatellite.atmosphereMaterial.Params = new Dictionary<string, float>() { ["_AtmoDensity"] = 0f };
            barrenSatellite.VeinSettings.VeinTypes = new GSVeinTypes()
                {
                    GSVeinType.Generate(EVeinType.Titanium, 1, 10, 0.2f, 1.5f, 5, 15, false ),
                    GSVeinType.Generate(EVeinType.Stone, 1, 100, 0.5f, 1.5f, 5, 35, false ),
                    //GSVeinType.Generate(EVeinType.Copper, 1, 10, 0.5f, 1.5f, 5, 15, false ),
                    //GSVeinType.Generate(EVeinType.Silicium, 1, 10, 0.5f, 1.5f, 5, 15, false ),
                    GSVeinType.Generate(EVeinType.Grat, 1, 10, 0.5f, 1.5f, 5, 15, true ),
                };
            barrenSatellite.VeinSettings.Algorithm = "GS2";
            barrenSatellite.CustomGeneration = true;
            barrenSatellite.ThemeType = EThemeType.Moon;
            barrenSatellite.Process();

            GSTheme dwarf = new GSTheme("DwarfPlanet", "Dwarf Planet", "Barren");
            dwarf.Algo = 3;
            dwarf.CustomGeneration = true;
            dwarf.Temperature = 0;
            dwarf.Distribute = EThemeDistribute.Default;
            dwarf.TerrainSettings = new GSTerrainSettings()
            {
                RandomFactor = 10,
                LandModifier = .1f,
                Algorithm = "GSTA3"
            };
            dwarf.VeinSettings = new GSVeinSettings()
            {
                Algorithm = "GS2",
                VeinPadding = 0.5f,
                VeinTypes = new GSVeinTypes()
                {
                    GSVeinType.Generate(EVeinType.Iron, 1, 10, 0.5f, 1.5f, 5, 15, false ),
                    GSVeinType.Generate(EVeinType.Stone, 1, 10, 0.5f, 1.5f, 5, 15, false ),
                    GSVeinType.Generate(EVeinType.Copper, 1, 10, 0.5f, 1.5f, 5, 15, false ),
                    GSVeinType.Generate(EVeinType.Silicium, 1, 10, 0.5f, 1.5f, 5, 15, false ),
                    GSVeinType.Generate(EVeinType.Fireice, 1, 10, 0.5f, 1.5f, 5, 15, true ),
                }
            };
            dwarf.Wind = 0f;
            dwarf.ThemeType = EThemeType.Planet;
            dwarf.Process();

            GSTheme center = new GSTheme("Center", " ", "Barren");
            center.PlanetType = EPlanetType.Gas;
            center.atmosphereMaterial.Tint = Color.black;
            center.thumbMaterial.Tint = Color.black;
            center.ThemeType = EThemeType.Private;
            center.Process();

            GSTheme hotGas = new GSTheme("Inferno", "Infernal Gas Giant", "GasGiant");
            hotGas.terrainMaterial.Tint = new Color(1, 0.8f, 0.1f);
            hotGas.Temperature = 4f;
            hotGas.MinRadius = 5;
            hotGas.MaxRadius = 510;
            hotGas.ThemeType = EThemeType.Gas;
            hotGas.Process();

            //GS2.LogJson(GS2.ThemeLibrary);
            //foreach (string key in baseKeys)
            //{
            //    GS2.Warn("Private setting for " + key);
            //    GS2.ThemeLibrary[key].ThemeType = EThemeType.Private;
            //}
            //foreach (var kvp in GS2.ThemeLibrary) GS2.Log("Final:" + kvp.Key);
        }
    }
}