using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Gobi = new GSTheme()
        {
            Name = "Gobi",
            DisplayName = "Gobi Desert",
            PlanetType = EPlanetType.Desert,
            LDBThemeId = 12,
            Algo = 3,
            MaterialPath = "Universe/Materials/Planets/Desert 4/",
            Temperature = 1.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(1.0f, 1.0f),
            VeinSettings = new GSVeinSettings() { Algorithm = "Vanilla" },
            Vegetables0 = new int[] {
            },
            Vegetables1 = new int[] {
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
                2,
                7,
                8,
                0,
                7,
                3,
                0
            },
            VeinCount = new float[] {
                0.4f,
                1.0f,
                1.0f,
                0.0f,
                1.0f,
                0.7f,
                0.0f
            },
            VeinOpacity = new float[] {
                0.8f,
                1.0f,
                1.0f,
                0.0f,
                1.0f,
                0.7f,
                0.0f
            },
            RareVeins = new int[] {
                9,
                10,
                12
            },
            RareSettings = new float[] {
                0.0f,
                0.25f,
                0.6f,
                0.6f,
                0.0f,
                0.25f,
                0.6f,
                0.6f,
                0.0f,
                0.1f,
                0.2f,
                0.5f
            },
            GasItems = new int[] {
            },
            GasSpeeds = new float[] {
            },
            UseHeightForBuild = false,
            Wind = 0.8f,
            IonHeight = 60f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new int[] { 
                11,
                4
            },
            SFXPath = "SFX/sfx-amb-desert-1",
            SFXVolume = 0.4f,
            CullingRadius = 0f
        };
    }
}