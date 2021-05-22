using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme OceanicJungle = new GSTheme()
        {
            Name = "OceanicJungle",
            Base = true,
            DisplayName = "Oceanic Jungle",
            PlanetType = EPlanetType.Ocean,
            LDBThemeId = 8,
            Algo = 1,
            MaterialPath = "Universe/Materials/Planets/Ocean 2/",
            Temperature = 0.0f,
            Distribute = EThemeDistribute.Interstellar,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(0.0f, 0.0f),
            VeinSettings = new GSVeinSettings() { Algorithm = "Vanilla" },
            Vegetables0 = new int[] {
                1023,
                124,
                603,
                126,
                121,
                605
            },
            Vegetables1 = new int[] {
                605,
                121,
                122,
                125,
                1021,
                604,
                603,
                126
            },
            Vegetables2 = new int[] {
                1023
            },
            Vegetables3 = new int[] {
                1023,
                1021,
                1006
            },
            Vegetables4 = new int[] {
                1023,
                1021
            },
            Vegetables5 = new int[] {
                1022
            },
            VeinSpot = new int[] {
                7,
                2,
                12,
                0,
                4,
                10,
                22
            },
            VeinCount = new float[] {
                0.6f,
                0.3f,
                0.9f,
                0.0f,
                0.8f,
                1.0f,
                1.0f
            },
            VeinOpacity = new float[] {
                0.6f,
                0.6f,
                0.6f,
                0.0f,
                0.5f,
                1.0f,
                1.0f
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
                0.8f
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
            SFXVolume = 0.52f,
            CullingRadius = 0f
        };
    }
}