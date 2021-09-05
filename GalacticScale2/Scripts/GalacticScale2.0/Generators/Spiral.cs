using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale.Generators
{
    public class Spiral : iGenerator
    {
        public bool DisableStarCountSlider => false;
        public string Name => "Spiral";

        public string Author => "innominata";

        public string Description => "The most basic generator. Simply to test";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.Spiral";
        public GSGeneratorConfig Config { get; } = new GSGeneratorConfig();

        public void Init()
        {
        }


        public void Generate(int starCount, StarData birthStar = null)
        {
            generate(starCount);
        }

        public void InitThemes()
        {
            //GS2.Log("Spiral:Initializing");
            Config.DisableSeedInput = true;
            Config.DisableStarCountSlider = false;
            Config.MaxStarCount = 1048;
            Config.MinStarCount = 1;


            var sulfursea = new GSTheme("SulfurSea", "Sulfurous Sea", "OceanWorld");
            var giganticforest = new GSTheme("GiganticForest", "Gigantic Forest", "OceanicJungle");
            var moltenworld = new GSTheme("MoltenWorld", "Molten World", "Lava");
            var redforest = new GSTheme("RedForest", "Red Forest", "OceanicJungle");
            var beach = new GSTheme("Beach", "Beach", "OceanWorld");
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
            GS2.LogJson(beach, true);
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
            moltenworld.Temperature = 7;

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
            sulfursea.Habitable = true;
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
            sulfursea.Process();
            giganticforest.Process();
            moltenworld.Process();
            redforest.Process();
            beach.Process();
        }
        ////////////////////////////////////////////////////////////////////


        public void generate(int starCount)
        {
            InitThemes();
            //GS2.Log("Spiral:Creating New Settings");
            //foreach (KeyValuePair<string, GSTheme> g in GS2.ThemeLibrary) GS2.Log("Theme " + g.Key + " is in library with name " + g.Value.Name);
            var positions = new List<VectorLF3>();
            for (var i = 0; i < starCount; i++)
            {
                var x = i * Math.Cos(6 * i) / 3;
                var y = i * Math.Sin(6 * i) / 3;
                double z = i / 4;
                positions.Add(new VectorLF3(y, z, x));
            }

            var random = new GS2.Random(GSSettings.Seed);
            //beach.InitTheme(Themes.OceanWorld);
            var p = new GSPlanets
            {
                new GSPlanet("SulfurSea", "SulfurSea", 200, 3f, -1, -1, 1, -1, -1, -1, -1),
                new GSPlanet("GiganticForest", "GiganticForest", 200, 3f, -1, -1, 10, 1, -1, -1, -1),
                new GSPlanet("Molten", "MoltenWorld", 200, 3f, -1, -1, 50, 15, -1, -1, -1),
                new GSPlanet("RedForest", "RedForest", 200, 3f, -1, -1, 80, -1, -1, -1, -1),
                new GSPlanet("Beach", "Beach", 200, 3f, -1, -1, -1, 45, -1, -1, -1)
                //new GSPlanet("IceGiant2", "IceGiant2", 80, 3f, -1, -1, -1, 60, -1, -1, -1, -1, null),
                //new GSPlanet("Arid", "Arid", 80, 3f, -1, -1, -1, 75, -1, -1, -1, -1, null),
                //new GSPlanet("AshenGelisol", "AshenGelisol", 80, 3f, -1, -1, -1, 90, -1, -1, -1, -1, null),
                //new GSPlanet("Jungle", "Jungle", 80, 3f, -1, -1, -1, 105, -1, -1, -1, -1, null),
                //new GSPlanet("Lava", "Lava", 80, 3f, -1, -1, -1, 120, -1, -1, -1, -1, null),
                //new GSPlanet("YellowGas", "YellowGas", 180, 3f, -1, -1, -1, 3f, -1, -1, -1, -1, null),
                //new GSPlanet("GreenGas", "GreenGas", 180, 3f, -1, -1, -1, 6f, -1, -1, -1, -1, null),
                //new GSPlanet("MagentaGas", "MagentaGas", 180, 3f, -1, -1, -1, 9f, -1, -1, -1, -1, null),
                //new GSPlanet("Ice", "Ice", 80, 3f, -1, -1, -1, 150, -1, -1, -1, -1, null),
                //new GSPlanet("Barren", "Barren", 80, 3f, -1, -1, -1, 165, -1, -1, -1, -1, null),
                //new GSPlanet("Gobi", "Gobi", 80, 3f, -1, -1, -1, 180, -1, -1, -1, -1, null),
                //new GSPlanet("VolcanicAsh", "VolcanicAsh", 80, 3f, -1, -1, -1, 195, -1, -1, -1, -1, null),
                //new GSPlanet("RedStone", "RedStone", 80, 3f, -1, -1, -1, 210, -1, -1, -1, -1, null),
                //new GSPlanet("Prarie", "Prarie", 80, 3f, -1, -1, -1, 225, -1, -1, -1, -1, null),
                //new GSPlanet("Ocean", "Ocean", 80, 3f, -1, -1, -1, 240, -1, -1, -1, -1, null),
            };
            //for (var i = 0; i <= 10; i++)
            //{
            //    p.Add(new GSPlanet("ss" + i, "ss" + i, 200, 2f, -1, 99999999991, 2f * i, -1, -1, -1, 1f, null));
            //}
            GSSettings.BirthPlanetName = "SulfurSea";
            GSSettings.Stars.Add(new GSStar(1, "BeatleJooce", ESpectrType.G, EStarType.MainSeqStar, p));
            for (var i = 1; i < starCount; i++)
            {
                //int t = i % 7;
                //ESpectrType e = (ESpectrType)t;
                //GSSettings.Stars.Add(new GSStar(1, "Star" + i.ToString(), ESpectrType.F, EStarType.GiantStar, new GSPlanets()));
                var s = StarDefaults.Random(random, i);
                GSSettings.Stars.Add(s);
                GSSettings.Stars[i].position = positions[i];
                //GSSettings.Stars[i].classFactor = (float)(new Random(i).NextDouble() * 6.0)-4f;
                //GSSettings.Stars[i].Spectr = e;
                //GSSettings.Stars[i].Name = "CF" + GSSettings.Stars[i].classFactor + "-" + e.ToString();
            }
        }
    }
}