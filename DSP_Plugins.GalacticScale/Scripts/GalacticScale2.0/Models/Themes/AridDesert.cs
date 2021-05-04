using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme AridDesert = new GSTheme()
        {
            Name = "AridDesert",
            DisplayName = "Arid Desert",
            PlanetType = EPlanetType.Desert,
            LDBThemeId = 6,
            Algo = 2,
            MaterialPath = "Universe/Materials/Planets/Desert 1/",
            Temperature = 2.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(1.0f, 1.0f),
            ModY = new Vector2(0.0f, 0.0f),
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
                10,
                0,
                6,
                10,
                1,
                0
            },
            VeinCount = new float[] {
                0.5f,
                1.0f,
                0.0f,
                1.0f,
                1.0f,
                0.3f,
                0.0f
            },
            VeinOpacity = new float[] {
                0.6f,
                0.6f,
                0.0f,
                1.0f,
                1.0f,
                0.3f,
                0.0f
            },
            RareVeins = new int[] { 
                9 
            },
            RareSettings = new float[] {
                0.0f,
                0.18f,
                0.2f,
                0.3f
            },
            GasItems = new int[] {
            },
            GasSpeeds = new float[] {
            },
            UseHeightForBuild = false,
            Wind = 1.5f,
            IonHeight = 70f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new int[] { 
                11,4 
            },
            SFXPath = "SFX/sfx-amb-desert-3",
            SFXVolume = 0.4f,
            CullingRadius = 0f
        };
    }
}