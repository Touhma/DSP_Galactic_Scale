using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme RedStone = new GSTheme()
        {
            Name = "RedStone",
            DisplayName = "Red Stone",
            PlanetType = EPlanetType.Ocean,
            LDBThemeId = 14,
            Algo = 1,
            MaterialPath = "Universe/Materials/Planets/Ocean 3/",
            Temperature = 0.0f,
            Distribute = EThemeDistribute.Interstellar,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(0.0f, 0.0f),
            VeinSettings = new GSVeinSettings() { VeinAlgorithm = "Vanilla" },
            Vegetables0 = new int[] {
                604,
                114,
                603,
                605,
                116,
                113,
                115,
                112,
                111
            },
            Vegetables1 = new int[] {
                114,
                604,
                1011,
                114,
                116,
                1011,
                113,
                605,
                112,
                115,
                111
            },
            Vegetables2 = new int[] {
                1012
            },
            Vegetables3 = new int[] {
                1012,
                1013
            },
            Vegetables4 = new int[] {
                1013
            },
            Vegetables5 = new int[] {
            },
            VeinSpot = new int[] {
                4,
                6,
                0,
                0,
                10,
                8,
                12
            },
            VeinCount = new float[] {
                0.7f,
                0.7f,
                0.0f,
                0.0f,
                1.0f,
                1.0f,
                1.0f
            },
            VeinOpacity = new float[] {
                0.5f,
                0.6f,
                0.0f,
                0.0f,
                0.8f,
                1.0f,
                1.0f
            },
            RareVeins = new int[] {
                9,
                11,
                13
            },
            RareSettings = new float[] {
                0.0f,
                0.4f,
                0.3f,
                0.5f,
                0.0f,
                1.0f,
                0.3f,
                0.8f,
                0.0f,
                0.5f,
                0.2f,
                0.5f
            },
            GasItems = new int[] {
            },
            GasSpeeds = new float[] {
            },
            UseHeightForBuild = false,
            Wind = 1f,
            IonHeight = 60f,
            WaterHeight = 0f,
            WaterItemId = 1000,
            Musics = new int[] { 
                9 
            },
            SFXPath = "SFX/sfx-amb-ocean-1",
            SFXVolume = 0.53f,
            CullingRadius = 0f
        };
    }
}