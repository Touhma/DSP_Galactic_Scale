using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        public static string[] baseKeys;

        public static void SetupBaseThemes()
        {
            var newLibrary = new ThemeLibrary();
            foreach (var v in ThemeLibrary.Vanilla())
            {
                var clone = v.Value.Clone();
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
            var smolLibrary = new ThemeLibrary();
            var keys = new string[newLibrary.Count + 1];
            newLibrary.Keys.CopyTo(keys, 0);
            for (var i = 0; i < newLibrary.Count; i++)
            {
                var key = keys[i];
                var theme = newLibrary[key];
                theme.CustomGeneration = true;
                theme.VeinSettings.Algorithm = "GS2";
                if (theme.Algo == 7) theme.VeinSettings.Algorithm = "GS2W";
                if (theme.PlanetType == EPlanetType.Ocean) theme.MinRadius = 50;
                if (theme.PlanetType != EPlanetType.Gas && theme.PlanetType != EPlanetType.Ocean)
                {
                    //For rocky worlds
                    var smolTheme = theme.Clone();
                    smolTheme.DisplayName = theme.DisplayName;
                    smolTheme.Name += "smol";
                    smolTheme.VeinSettings.Algorithm = "GS2W";
                    smolTheme.CustomGeneration = true;
                    smolLibrary.Add(smolTheme.Name, smolTheme);
                    smolTheme.MaxRadius = 40;
                    theme.MinRadius = 50;
                    if (theme.PlanetType == EPlanetType.Vocano)
                    {
                        theme.TerrainSettings.BrightnessFix = true;
                        smolTheme.TerrainSettings.BrightnessFix = true;
                        theme.Init();
                        smolTheme.Init();
                    }

                    smolTheme.atmosphereMaterial.Params["_Intensity"] = 0f;
                    //string[] pKeys = new string[100];
                    //smolTheme.atmosphereMaterial.Params.Keys.CopyTo(pKeys, 0);
                    //for (var j = 0; j < smolTheme.atmosphereMaterial.Colors.Count; j++) smolTheme.atmosphereMaterial.Params[pKeys[j]] = Color.clear;
                }
            }

            foreach (var s in smolLibrary)
                if (!newLibrary.ContainsKey(s.Key)) newLibrary.Add(s.Key, s.Value);
                else newLibrary[s.Key] = s.Value;
            // GS2.WarnJson(GS2.externalThemes.Select(o=>o.Key).ToList());
            newLibrary.AddRange(GS2.externalThemes);
            GSSettings.ThemeLibrary = newLibrary;
            // GS2.Warn("End of Themes.CS, ThemeLibrary Contents:");
            // GS2.WarnJson(GSSettings.ThemeLibrary.Select(o=>o.Key).ToList());
        }

        public static void InitThemes()
        {
            var sulfursea = new GSTheme("SulfurSea", "Sulfurous Sea".Translate(), "OceanWorld");
            var giganticforest = new GSTheme("GiganticForest", "Gigantic Forest".Translate(), "OceanicJungle");
            var moltenworld = new GSTheme("MoltenWorld", "Molten World".Translate(), "Lava");
            var redforest = new GSTheme("RedForest", "Red Forest".Translate(), "OceanicJungle");
            var beach = new GSTheme("Beach", "Beach".Translate(), "OceanWorld");
            beach.VeinSettings = new GSVeinSettings
            {
                Algorithm = "GS2",
                VeinPadding = 1f,
                VeinTypes = new GSVeinTypes
                {
                    GSVeinType.Generate(EVeinType.Silicium, 10, 30, 1f, 10f, 5, 25, false),
                    GSVeinType.Generate(EVeinType.Bamboo, 2, 6, 1, 10f, 5, 25, true),
                    GSVeinType.Generate(EVeinType.Fractal, 2, 6, 1, 10f, 5, 25, false),
                    GSVeinType.Generate(EVeinType.Grat, 2, 6, 1, 10f, 5, 25, false)
                }
            };
            beach.VegeSettings = Themes.Mediterranean.VegeSettings.Clone();
            beach.VegeSettings.Group1.Clear();
            beach.VegeSettings.Group2.Clear();
            beach.VegeSettings.Group4 = beach.VegeSettings.Group3;
            beach.TerrainSettings.Algorithm = "GSTA1";
            beach.CustomGeneration = true;
            //GS2.LogJson(beach, true);
            giganticforest.StarTypes = new List<EStar> { EStar.G, EStar.F, EStar.YellowGiant };
            giganticforest.VegeSettings.Group1.Clear();
            giganticforest.VegeSettings.Group2.Clear();
            giganticforest.VegeSettings.Group3.Clear();
            giganticforest.VegeSettings.Group4.Clear();
            giganticforest.VegeSettings.Group5.Clear();
            giganticforest.VegeSettings.Group6.Clear();
            giganticforest.Vegetables1 = new[]
            {
                42, 42, 42, 46, 101, 101, 101, 101, 101, 101, 102, 102, 102, 102, 102, 102, 103, 103, 103, 103, 103,
                103, 104, 104, 104, 104, 104, 104, 125, 125, 125, 125, 125, 125, 601, 601, 601, 601, 601, 601, 602, 602,
                602, 602, 602, 602, 603, 603, 603, 603, 603, 603, 604, 604, 604, 604, 604, 604, 605, 605, 605, 605, 605,
                605
            }; // Medium density, Biome border only
            giganticforest.Vegetables2 = new[] { 1001, 1002, 1003, 1005, 1006, 1007 }; // Dense clumped ground scatter, everywhere
            giganticforest.Vegetables3 = new[] { 43, 46, 47, 47, 101, 102, 103, 104, 106, 601, 602, 604 }; // Sparse, lowland only
            giganticforest.Vegetables4 = new int[] { }; // Unused
            giganticforest.Vegetables5 = new[]
            {
                42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 43, 43, 43, 43, 43, 43, 46, 46, 47, 47, 47, 47, 47, 47, 47,
                102, 103, 103, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 104, 125, 125,
                125, 125, 125, 125, 125, 604, 604, 604, 604, 604, 604, 604, 605, 605, 605, 605, 605, 605, 605, 1001,
                1001, 1001, 1001, 1001, 1001, 1001, 1002, 1002, 1002, 1002, 1002, 1002, 1002, 1002, 1002
            }; // Dense, Highland only
            giganticforest.PopulateVegeData();
            giganticforest.Distribute = EThemeDistribute.Default;
            giganticforest.Habitable = true;
            giganticforest.CustomGeneration = true;
            giganticforest.TerrainSettings.Algorithm = "GSTA6";
            Themes.Mediterranean.InitMaterials();
            Themes.OceanicJungle.InitMaterials();
            giganticforest.oceanMat = Themes.Mediterranean.oceanMat;

            //moltenworld.IonHeight = Themes.OceanicJungle.IonHeight;
            moltenworld.CustomGeneration = true;
            moltenworld.TerrainSettings = new GSTerrainSettings
            {
                Algorithm = "GSTA1",
                BaseHeight = -1.5, //negative numbers lower the ocean
                xFactor = 0.01,
                yFactor = 0.012,
                zFactor = 0.01,
                HeightMulti = 0.4, //how exagerated the ups and downs are
                LandModifier = -0.9, //how clumpy the land is
                RandomFactor = -1,
                BiomeHeightMulti = 2.0,
                BiomeHeightModifier = 1.2
            };
            moltenworld.VeinSettings.Algorithm = "GS2W";
            moltenworld.MinRadius = 20;
            moltenworld.MaxRadius = 510;
            moltenworld.ThemeType = EThemeType.Telluric;
            redforest.VegeSettings.Group1.Clear();
            redforest.VegeSettings.Group2.Clear();
            redforest.VegeSettings.Group3.Clear();
            redforest.VegeSettings.Group4.Clear();
            redforest.VegeSettings.Group5.Clear();
            redforest.VegeSettings.Group6.Clear();
            redforest.Vegetables0 = new[] { 26, 26, 45, 603, 604 }; // lowlands
            redforest.Vegetables1 = new[] { 1001, 1001, 1001, 1001, 1001, 1001, 45, 26, 26 }; // Ground scatter, highlands
            redforest.Vegetables2 = new[] { 1001 }; // Grass ground scatter, highlands
            redforest.Vegetables3 = new[] { 26, 26, 26, 26, 45, 602, 603, 604 }; // Ground
            redforest.Vegetables4 = new[] { 1001, 26, 602, 603, 604 }; // Semi clumped shoreline
            redforest.Vegetables5 = new[] { 25, 32, 36, 37, 39, 41 }; // Water
            redforest.PopulateVegeData();
            redforest.VeinCount = Themes.RedStone.VeinCount;
            redforest.VeinSpot = Themes.RedStone.VeinSpot;
            redforest.VeinOpacity = Themes.RedStone.VeinOpacity;
            redforest.RareSettings = Themes.RedStone.RareSettings;
            redforest.RareVeins = Themes.RedStone.RareVeins;
            redforest.CustomGeneration = true;
            redforest.TerrainSettings = new GSTerrainSettings
            {
                Algorithm = "GSTA1",
                BaseHeight = -0.7,
                xFactor = 0.01,
                yFactor = 0.012,
                zFactor = 0.01,
                HeightMulti = 1.2,
                LandModifier = 1.3,
                RandomFactor = 0.3,
                BiomeHeightMulti = 2.0,
                BiomeHeightModifier = 0.2
            };

            redforest.terrainMaterial.Tint = new Color(.1f, 0.6f, 0.05f, 1f);

            sulfursea.CustomGeneration = true;
            sulfursea.Habitable = false;
            sulfursea.MinRadius = 20;
            sulfursea.MaxRadius = 510;
            sulfursea.Temperature = 1;
            sulfursea.TerrainSettings = new GSTerrainSettings
            {
                Algorithm = "GSTA1",
                HeightMulti = 1,
                BaseHeight = -1.3,
                LandModifier = -0.7,
                RandomFactor = 0.1,
                BiomeHeightMulti = 2.9,
                BiomeHeightModifier = 1
            };
            sulfursea.ThemeType = EThemeType.Telluric;
            sulfursea.AmbientSettings = Themes.VolcanicAsh.AmbientSettings.Clone();
            sulfursea.VegeSettings.Group1.Clear();
            sulfursea.VegeSettings.Group2.Clear();
            sulfursea.VegeSettings.Group3.Clear();
            sulfursea.VegeSettings.Group4.Clear();
            sulfursea.VegeSettings.Group5.Clear();
            sulfursea.VegeSettings.Group6.Clear();
            sulfursea.Vegetables0 = new int[] { }; // lowlands
            sulfursea.Vegetables1 = new int[] { }; // Ground scatter, highlands
            sulfursea.Vegetables2 = new int[] { }; // Grass ground scatter, highlands
            sulfursea.Vegetables3 = new[] { 601, 602, 603, 604, 605 }; // Ground
            sulfursea.Vegetables4 = new[] { 601, 602, 603, 604, 605 }; // Semi clumped shoreline
            sulfursea.Vegetables5 = new int[] { }; // Water
            sulfursea.terrainMaterial.CopyFrom = "Gobi.terrainMat";
            sulfursea.atmosphereMaterial.CopyFrom = "VolcanicAsh.atmosMat";
            //sulfursea.oceanMat = Themes.Mediterranean.oceanMat;
            sulfursea.oceanMaterial.Colors.Clear();
            foreach (var c in Themes.VolcanicAsh.oceanMaterial.Colors)
                sulfursea.oceanMaterial.Colors.Add(c.Key, c.Value);
            sulfursea.atmosphereMaterial.Tint = new Color(0.3f, 0.3f, 0f, 1f);
            sulfursea.oceanMaterial.Params["_GIGloss"] = 1;
            sulfursea.oceanMaterial.Params["_GISaturate"] = 0.8f;
            sulfursea.oceanMaterial.Params["_GIStrengthDay"] = 1;
            sulfursea.oceanMaterial.Params["_GIStrengthNight"] = 0.0f;
            sulfursea.terrainMaterial.Tint = new Color(.8f, .7f, .6f, 1f);
            sulfursea.thumbMaterial.CopyFrom = "Gobi.thumbMat";
            sulfursea.minimapMaterial.CopyFrom = "Gobi.minimapMat";
            sulfursea.WaterItemId = 1116;
            sulfursea.Process();
            giganticforest.Process();
            moltenworld.Process();
            redforest.Process();
            beach.Process();

            //GS2.Log("Creating Themes");
            var oiler = new GSTheme("OilGiant", "SpaceWhale Excrement".Translate(), "IceGiant");
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

            var obsidian = new GSTheme("Obsidian", "Obsidian".Translate(), "IceGelisol");

            // obsidian.oceanMaterial.Tint = new Color(0.005f, 0.005f, 0.005f, 1f);
            obsidian.atmosphereMaterial.Tint = Color.black;
            obsidian.TerrainSettings.Algorithm = "GSTA3";
            obsidian.TerrainSettings.BiomeHeightMulti = 0f;
            obsidian.TerrainSettings.BiomeHeightModifier = -111f;
            obsidian.TerrainSettings.HeightMulti = 0f;
            obsidian.TerrainSettings.RandomFactor = 1f;
            obsidian.CustomGeneration = true;
            // obsidian.AmbientSettings.CubeMap = "GS2";
            obsidian.AmbientSettings.LutContribution = 1;
            obsidian.AmbientSettings.Reflections = new Color(1, 0, 0, 1);
            obsidian.atmosphereMaterial.Colors["_Color"] = new Color(1, 1, 0, 1);
            obsidian.oceanMaterial.Tint = new Color(0, 0, 0, 0.5f);
            obsidian.terrainMaterial.Tint = new Color(0.15f, 0.15f, 0.15f, 1f);
            // obsidian.Process();
            // obsidian.terrainMat.SetFloat("_HeightEmissionRadius", 0f);
            // obsidian.terrainMat.SetFloat("_EmissionStrength", 0f);
            // obsidian.terrainMat.SetFloat("_NormalStrength", 0.01f);
            // obsidian.terrainMat.SetFloat("_Distance", 1f);
            obsidian.terrainMaterial.Params = new Dictionary<string, float>
            {
                ["_AmbientInc"] = 0f,
                ["_GISaturate"] = 0.0f,
                ["_GIStrengthDay"] = 0.0505f,
                ["_GIStrengthNight"] = 0.03f,
                ["_Multiplier"] = 0.0018f,
                ["_NormalStrength"] = 0.01010f, //1.5,
                ["_SpecularHighlights"] = 110.10f //x
            };
            obsidian.terrainMaterial.Colors = new Dictionary<string, Color>
            {
                ["_SpeclColor"] = new Color(0.14f, 0.14f, 0.04f, 1f)
            };
            // obsidian.terrainMat.SetFloat("_AmbientInc", 0);
            // obsidian.terrainMat.SetFloat("_GISaturate", 0);
            // obsidian.terrainMat.SetFloat("_GIStrengthDay", 0.05f);
            // obsidian.terrainMat.SetFloat("_GIStrengthNight", 0.03f);
            // obsidian.terrainMat.SetFloat("_SpecularHighlights", 10.0f);

            // obsidian.terrainMat.SetFloat("_GlossMapScale", 1.0f);
            // obsidian.terrainMat.SetFloat("_GlossyReflections", 11.1f);
            // obsidian.terrainMat.SetFloat("_Multiplier", 0.0018f);
            // obsidian.terrainMat.SetColor("_SpeclColor", new Color(0.14f, 0.14f, 0.04f, 1f));

            obsidian.Temperature = 2f;
            obsidian.Process();

            var hotObsidian = new GSTheme("HotObsidian", "Hot Obsidian".Translate(), "Obsidian");
            hotObsidian.MaxRadius = 40;
            hotObsidian.MinRadius = 5;
            hotObsidian.terrainMaterial.Tint = new Color(0.2f, 0.05f, 0.05f, 1);
            hotObsidian.Temperature = 5f;
            hotObsidian.atmosphereMaterial.Tint = Color.clear;
            hotObsidian.Process();

            //GS2.Log("About to Process redIce");
            var iceMalusol = new GSTheme("IceMalusol", "Ice Malusol".Translate(), "IceGelisol");
            iceMalusol.terrainMaterial.Textures.Add("_BioTex0A", "GS2|red-ice");
            iceMalusol.terrainMaterial.Textures.Add("_BioTex1A", "GS2|grey-rock");
            iceMalusol.terrainMaterial.Textures.Add("_BioTex2A", "GS2|grey-snow");
            iceMalusol.AmbientSettings.Reflections = new Color(1, 0, 0, 1);
            iceMalusol.AmbientSettings.CubeMap = "GS2";
            iceMalusol.Process();

            var acidGreenhouse = new GSTheme("AcidGreenhouse", "Acid Greenhouse".Translate(), "VolcanicAsh");
            acidGreenhouse.atmosphereMaterial.Tint = new Color(0.5f, 0.4f, 0.0f, 0.8f);
            acidGreenhouse.atmosphereMaterial.Params = new Dictionary<string, float>
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
            acidGreenhouse.terrainMaterial.Colors = new Dictionary<string, Color>
            {
                ["_AmbientColor0"] = new Color(0.9f, 0.9f, 0.1f, 1f),
                ["_AmbientColor0"] = new Color(0.8f, 0.8f, 0.7f, 1f),
                ["_AmbientColor0"] = new Color(0.5f, 0.5f, 0.0f, 1f),
                ["_Color"] = new Color(0.65f, 0.5f, 0.15f, 1f)
            };
            acidGreenhouse.terrainMaterial.Params = new Dictionary<string, float>
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
            acidGreenhouse.VeinSettings.VeinTypes = new GSVeinTypes
            {
                GSVeinType.Generate(EVeinType.Iron, 1, 10, 0.5f, 1.5f, 5, 15, false),
                GSVeinType.Generate(EVeinType.Stone, 1, 10, 0.5f, 1.5f, 5, 15, false),
                GSVeinType.Generate(EVeinType.Copper, 1, 20, 0.5f, 1.5f, 5, 15, true),
                GSVeinType.Generate(EVeinType.Silicium, 1, 20, 0.9f, 2.5f, 5, 25, false),
                GSVeinType.Generate(EVeinType.Diamond, 1, 20, 0.5f, 1.5f, 5, 15, true),
                GSVeinType.Generate(EVeinType.Fractal, 1, 20, 0.5f, 1.5f, 5, 15, true)
            };
            acidGreenhouse.VeinSettings.Algorithm = "GS2";
            acidGreenhouse.CustomGeneration = true;
            acidGreenhouse.MinRadius = 150;
            acidGreenhouse.Temperature = 3f;
            acidGreenhouse.ThemeType = EThemeType.Planet;
            acidGreenhouse.Process();

            var barrenSatellite = new GSTheme("BarrenSatellite", "Barren Satellite".Translate(), "Barren");
            barrenSatellite.VegeSettings.Group1 = new List<string>
            {
                "Stone1", "Stone2", "Stone3", "Stone4", "Stone5", "Stone6", "Stone7", "Stone8", "Stone9", "Stone10",
                "Stone11", "Stone12", "Stone1", "Stone2", "Stone3", "Stone4", "Stone5", "Stone6", "Stone7", "Stone8",
                "Stone9", "Stone10", "Stone11", "Stone12"
            };
            barrenSatellite.VegeSettings.Group2 = barrenSatellite.VegeSettings.Group1;
            barrenSatellite.VegeSettings.Group3 = barrenSatellite.VegeSettings.Group1;
            barrenSatellite.VegeSettings.Group4 = barrenSatellite.VegeSettings.Group1;
            barrenSatellite.VegeSettings.Group5 = barrenSatellite.VegeSettings.Group1;
            barrenSatellite.VegeSettings.Group6 = barrenSatellite.VegeSettings.Group1;
            barrenSatellite.atmosphereMaterial.Params = new Dictionary<string, float> { ["_AtmoDensity"] = 0f };
            barrenSatellite.VeinSettings.VeinTypes = new GSVeinTypes
            {
                GSVeinType.Generate(EVeinType.Titanium, 1, 10, 0.2f, 1.5f, 5, 15, false),
                GSVeinType.Generate(EVeinType.Stone, 1, 100, 0.5f, 1.5f, 5, 35, false),
                //GSVeinType.Generate(EVeinType.Copper, 1, 10, 0.5f, 1.5f, 5, 15, false ),
                //GSVeinType.Generate(EVeinType.Silicium, 1, 10, 0.5f, 1.5f, 5, 15, false ),
                GSVeinType.Generate(EVeinType.Grat, 1, 10, 0.5f, 1.5f, 5, 15, true)
            };
            barrenSatellite.VeinSettings.Algorithm = "GS2";
            barrenSatellite.CustomGeneration = true;
            barrenSatellite.ThemeType = EThemeType.Moon;
            barrenSatellite.atmosphereMaterial.Params["_Intensity"] = 0f;
            barrenSatellite.Process();

            var dwarf = new GSTheme("DwarfPlanet", "Dwarf Planet".Translate(), "Barren");
            dwarf.Algo = 3;
            dwarf.CustomGeneration = true;
            dwarf.Temperature = 0;
            dwarf.Distribute = EThemeDistribute.Default;
            dwarf.TerrainSettings = new GSTerrainSettings
            {
                RandomFactor = 10,
                LandModifier = .1f,
                Algorithm = "GSTA3"
            };
            dwarf.VeinSettings = new GSVeinSettings
            {
                Algorithm = "GS2",
                VeinPadding = 0.5f,
                VeinTypes = new GSVeinTypes
                {
                    GSVeinType.Generate(EVeinType.Iron, 1, 10, 0.5f, 1.5f, 5, 15, false),
                    GSVeinType.Generate(EVeinType.Stone, 1, 10, 0.5f, 1.5f, 5, 15, false),
                    GSVeinType.Generate(EVeinType.Copper, 1, 10, 0.5f, 1.5f, 5, 15, false),
                    GSVeinType.Generate(EVeinType.Silicium, 1, 10, 0.5f, 1.5f, 5, 15, false),
                    GSVeinType.Generate(EVeinType.Fireice, 1, 10, 0.5f, 1.5f, 5, 15, true)
                }
            };
            dwarf.Wind = 0f;
            dwarf.ThemeType = EThemeType.Planet;
            dwarf.Process();

            var center = new GSTheme("Center", " ", "Barren");
            center.PlanetType = EPlanetType.Gas;
            center.atmosphereMaterial.Tint = Color.black;
            center.thumbMaterial.Tint = Color.black;
            center.ThemeType = EThemeType.Private;
            center.Process();

            var hotGas = new GSTheme("Inferno", "Infernal Gas Giant".Translate(), "GasGiant");
            var stupid = new GSTheme("Lol", "Lol", "AridDesert");
            hotGas.terrainMaterial.Tint = new Color(1, 0.8f, 0.1f);
            hotGas.Temperature = 4f;
            hotGas.MinRadius = 5;
            hotGas.MaxRadius = 510;
            hotGas.ThemeType = EThemeType.Gas;

            stupid.terrainMaterial.Tint = new Color(1, 0.8f, 0.1f);
            stupid.Temperature = 4f;
            stupid.MinRadius = 510;
            stupid.MaxRadius = 510;
            stupid.ThemeType = EThemeType.Planet;
            //hotGas.atmosMat = y;
            //GS2.Log("Creating oceanmat");
            hotGas.oceanMaterial.CopyFrom = "GasGiant.terrainMat";
            //hotGas.oceanMat.SetColor("_Color1", new Color() { r = 0.866f, g = 0.407f, b = 0.172f, a = 1 }); 
            //hotGas.oceanMat.SetColor("_Color2", new Color() { r = 0.717f, g = 0.349f, b = 0.164f, a = 1 });
            //hotGas.oceanMat.SetColor("_Color", new Color() { r = 0.288f, g = 0.14f, b = 0.03f, a = 1 });
            hotGas.oceanMaterial.Colors["_Color"] = new Color { r = 0.288f, g = 0.14f, b = 0.03f, a = 1 };
            //hotGas.oceanMat.SetColor("_Color", new Color() { r = 0.917f, g = 0.776f, b = 0.6f, a = 1 });
            hotGas.CustomGeneration = true;
            hotGas.TerrainSettings = new GSTerrainSettings
            {
                Algorithm = "GSTA00"
            };
            hotGas.terrainMaterial.Path = "Universe/Materials/Stars/star-mass-m";

            hotGas.terrainMaterial.Colors = new Dictionary<string, Color>
            {
                ["_Color4"] = new Color { r = 0.0f, g = 0.0f, b = 0.0f, a = 1 }, //Highlights?
                ["_Color1"] = new Color { r = 0f, g = 0, b = 0f, a = 1 }, //Base?
                ["_Color2"] = new Color { r = 0, g = 0f, b = 0, a = 1 }, //SunSpots
                ["_Color3"] = new Color { r = 0, g = 0, b = 0f, a = 1 }
            }; //Fringe
            hotGas.terrainMaterial.Params = new Dictionary<string, float>
            {
                ["_SkyAtmosPower"] = 10,
                ["_Intensity"] = 0.5f,
                ["_Multiplier"] = 0.5f,
                ["_AtmoThickness"] = 3
            };
            hotGas.WaterItemId = 1000;
            hotGas.Process();
            stupid.CustomGeneration = true;
            stupid.TerrainSettings = new GSTerrainSettings
            {
                Algorithm = "GSTA00"
            };
            stupid.terrainMaterial.Path = "Universe/Materials/Stars/star-mass-o";

            stupid.terrainMaterial.Colors = new Dictionary<string, Color>
            {
                ["_Color4"] = new Color { r = 0.0f, g = 0.0f, b = 0.0f, a = 1 }, //Highlights?
                ["_Color1"] = new Color { r = 0f, g = 0, b = 0f, a = 1 }, //Base?
                ["_Color2"] = new Color { r = 0, g = 0f, b = 0, a = 1 }, //SunSpots
                ["_Color3"] = new Color { r = 0, g = 0, b = 0f, a = 1 }
            }; //Fringe
            stupid.terrainMaterial.Params = new Dictionary<string, float>
            {
                ["_SkyAtmosPower"] = 10,
                ["_Intensity"] = 0.5f,
                ["_Multiplier"] = 0.5f,
                ["_AtmoThickness"] = 3
            };
            stupid.WaterItemId = 1000;
            stupid.oceanMaterial.Path = "Universe/Materials/Stars/star-mass-o";
            //hotGas.oceanMat.SetColor("_Color1", new Color() { r = 0.866f, g = 0.407f, b = 0.172f, a = 1 }); 
            //hotGas.oceanMat.SetColor("_Color2", new Color() { r = 0.717f, g = 0.349f, b = 0.164f, a = 1 });
            //hotGas.oceanMat.SetColor("_Color", new Color() { r = 0.288f, g = 0.14f, b = 0.03f, a = 1 });
            stupid.oceanMaterial.Colors["_Color"] = new Color { r = 0.288f, g = 0.14f, b = 0.03f, a = 1 };
            stupid.Process();
            //hotGas.Process();
            //var x = Resources.FindObjectsOfTypeAll<Material>();
            //foreach (var y in x)
            //{
            //    if (y.name == "star-mass-m")
            //    {
            //        //hotGas.atmosMat = y;
            //        hotGas.oceanMat = hotGas.terrainMat;
            //        //hotGas.oceanMat.SetColor("_Color1", new Color() { r = 0.866f, g = 0.407f, b = 0.172f, a = 1 }); 
            //        //hotGas.oceanMat.SetColor("_Color2", new Color() { r = 0.717f, g = 0.349f, b = 0.164f, a = 1 });
            //        hotGas.oceanMat.SetColor("_Color", new Color() { r = 0.288f, g = 0.14f, b = 0.03f, a = 1 });
            //        //hotGas.oceanMat.SetColor("_Color", new Color() { r = 0.917f, g = 0.776f, b = 0.6f, a = 1 });
            //        hotGas.CustomGeneration = true;
            //        hotGas.TerrainSettings = new GSTerrainSettings()
            //        {
            //            Algorithm = "GSTA00",
            //        };
            //        hotGas.terrainMat = Object.Instantiate(y);
            //        hotGas.terrainMat.SetColor("_Color4", new Color() { r = 0.0f, g = 0.0f, b = 0.0f, a = 1 });//Highlights?
            //        hotGas.terrainMat.SetColor("_Color1", new Color() { r = 0f, g = 0, b = 0f, a = 1 });//Base?
            //        hotGas.terrainMat.SetColor("_Color2", new Color() { r = 0, g = 0f, b = 0, a = 1 });//SunSpots
            //        hotGas.terrainMat.SetColor("_Color3", new Color() { r = 0, g = 0, b = 0f, a = 1 });//Fringe
            //        hotGas.terrainMat.SetFloat("_SkyAtmosPower", 10);
            //        hotGas.terrainMat.SetFloat("_Intensity", 0.5f);
            //        hotGas.terrainMat.SetFloat("_Multiplier", 0.5f);
            //        hotGas.terrainMat.SetFloat("_AtmoThickness", 3);
            //        hotGas.WaterItemId = 1000;
            //    }
            //}
        }
    }
}