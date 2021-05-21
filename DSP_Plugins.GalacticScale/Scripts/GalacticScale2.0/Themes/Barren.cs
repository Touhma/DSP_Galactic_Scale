using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Barren = new GSTheme()
        {
            Name = "Barren",
            DisplayName = "Barren Desert",
            PlanetType = EPlanetType.Desert,
            LDBThemeId = 11,
            Algo = 4,
            MaterialPath = "Universe/Materials/Planets/Desert 3/",
            Temperature = 0.0f,
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
                3,
                3,
                3,
                6,
                12,
                0,
                0
            },
            VeinCount = new float[] {
                0.5f,
                0.5f,
                0.5f,
                1.0f,
                1.2f,
                0.0f,
                0.0f
            },
            VeinOpacity = new float[] {
                0.6f,
                0.6f,
                0.9f,
                0.9f,
                1.5f,
                0.0f,
                0.0f
            },
            RareVeins = new int[] {
                8,
                9,
                12
            },
            RareSettings = new float[] {
                0.25f,
                0.5f,
                0.6f,
                0.6f,
                0.0f,
                0.2f,
                0.6f,
                0.7f,
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
            Wind = 0f,
            IonHeight = 0f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new int[] { 
                5,
                11 
            },
            SFXPath = "SFX/sfx-amb-desert-2",
            SFXVolume = 0.15f,
            CullingRadius = 0f
        };
    }
}