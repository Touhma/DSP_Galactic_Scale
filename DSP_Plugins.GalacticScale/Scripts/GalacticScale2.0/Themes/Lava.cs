using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Lava = new GSTheme()
        {
            Name = "Lava",
            Base = true,
            DisplayName = "Lava",
            PlanetType = EPlanetType.Vocano,
            LDBThemeId = 9,
            Algo = 5,
            MaterialPath = "Universe/Materials/Planets/Lava 1/",
            Temperature = 5.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(0.0f, 0.0f),
            VeinSettings = new GSVeinSettings() { Algorithm = "Vanilla" },
            TerrainSettings = new GSTerrainSettings() { BrightnessFix = true },
            Vegetables0 = new int[] {
                6,
                7,
                8,
                9,
                10,
                11,
                12
            },
            Vegetables1 = new int[] {
                1,
                2,
                3,
                4,
                5
            },
            Vegetables2 = new int[] {
            },
            Vegetables3 = new int[] {
            },
            Vegetables4 = new int[] { 
            },
            Vegetables5 = new int[] {
            },
            VeinSpot = new int[] {
                15,
                15,
                2,
                9,
                4,
                2,
                0
            },
            VeinCount = new float[] {
                1.0f,
                1.0f,
                0.6f,
                1.0f,
                0.6f,
                0.3f,
                0.0f
            },
            VeinOpacity = new float[] {
                1.0f,
                1.0f,
                0.6f,
                1.0f,
                0.5f,
                0.3f,
                0.0f
            },
            RareVeins = new int[] {
                9,
                10,
                12
            },
            RareSettings = new float[] {
                0.0f,
                0.2f,
                0.6f,
                0.7f,
                0.0f,
                0.2f,
                0.6f,
                0.7f,
                0.0f,
                0.1f,
                0.2f,
                0.8f
            },
            GasItems = new int[] {
            },
            GasSpeeds = new float[] {
            },
            UseHeightForBuild = false,
            Wind = 0.7f,
            IonHeight = 60f,
            WaterHeight = -2.50f,
            WaterItemId = -1,
            Musics = new int[] { 
                10
            },
            SFXPath = "SFX/sfx-amb-lava-1",
            SFXVolume = 0.4f,
            CullingRadius = 0f
        };
    }
}