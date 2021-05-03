using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme AshenGelisol = new GSTheme()
        {
            name = "AshenGelisol",
            DisplayName = "Ashen Gelisol",
            type = EPlanetType.Desert,
            LDBThemeId = 1,
            algo = 2,
            MaterialPath = "Universe/Materials/Planets/Desert 2/",
            Temperature = -1.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(1.0f, 1.0f),
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
                7,
                2,
                7,
                3,
                8,
                1,
                0
            },
            VeinCount = new float[] {
                1.0f,
                0.5f,
                1.0f,
                1.0f,
                0.7f,
                0.3f,
                0.0f
            },
            VeinOpacity = new float[] {
                0.6f,
                0.6f,
                1.0f,
                1.0f,
                0.5f,
                0.3f,
                0.0f
            },
            RareVeins = new int[] { 
                8,
                10
            },
            RareSettings = new float[] {
                0.3f,
                0.5f,
                0.7f,
                0.5f,
                0.0f,
                0.3f,
                0.2f,
                0.6f
            },
            GasItems = new int[] {
            },
            GasSpeeds = new float[] {
            },
            UseHeightForBuild = false,
            Wind = 0.4f,
            IonHeight = 50f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new int[] { 
                4,
                11 
            },
            SFXPath = "SFX/sfx-amb-desert-4",
            SFXVolume = 0.2f,
            CullingRadius = 0f
        };
    }
}