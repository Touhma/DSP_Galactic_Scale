using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Prairie = new GSTheme()
        {
            Name = "Prairie",
            DisplayName = "Prairie",
            PlanetType = EPlanetType.Ocean,
            LDBThemeId = 15,
            Algo = 6,
            MaterialPath = "Universe/Materials/Planets/Ocean 4/",
            Temperature = 0.0f,
            Distribute = EThemeDistribute.Interstellar,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(1.0f, 1.0f),
            Vegetables0 = new int[] {
                1001,
                1001,
                1003,
                1003,
                1001,
                1002
            },
            Vegetables1 = new int[] {
                1001,
                1001,
                1003,
                604,
                1002,
                1003,
                1002
            },
            Vegetables2 = new int[] {
                1001,
                1002,
                1003
            },
            Vegetables3 = new int[] {
                1001,
                1003,
                1002
            },
            Vegetables4 = new int[] { 
                1004 
            },
            Vegetables5 = new int[] {
                1001,
                1001,
                1002,
                1002,
                1003,
                101
            },
            VeinSpot = new int[] {
                7,
                4,
                7,
                1,
                2,
                7,
                18
            },
            VeinCount = new float[] {
                0.7f,
                0.6f,
                0.7f,
                0.4f,
                0.5f,
                1.0f,
                1.0f
            },
            VeinOpacity = new float[] {
                0.6f,
                0.5f,
                0.6f,
                0.5f,
                0.7f,
                1.0f,
                1.2f
            },
            RareVeins = new int[] {
                11,
                13
            },
            RareSettings = new float[] {
                0.0f,
                1.0f,
                0.3f,
                1.0f,
                0.0f,
                0.5f,
                0.2f,
                1.0f
            },
            GasItems = new int[] {
            },
            GasSpeeds = new float[] {
            },
            UseHeightForBuild = false,
            Wind = 1.1f,
            IonHeight = 60f,
            WaterHeight = 0f,
            WaterItemId = 1000,
            Musics = new int[] { 
                9 
            },
            SFXPath = "SFX/sfx-amb-ocean-1",
            SFXVolume = 0.35f,
            CullingRadius = 0f
        };
    }
}