using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme VolcanicAsh = new GSTheme()
        {
            Name = "VolcanicAsh",
            DisplayName = "Volcanic Ash",
            PlanetType = EPlanetType.Vocano,
            LDBThemeId = 13,
            Algo = 3,
            MaterialPath = "Universe/Materials/Planets/Volcanic 1/",
            Temperature = 4.0f,
            Distribute = EThemeDistribute.Interstellar,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(1.0f, 1.0f),
            VeinSettings = new GSVeinSettings() { VeinAlgorithm = "Vanilla" },
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
                10,
                10,
                2,
                7,
                4,
                1,
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
            },
            RareSettings = new float[] {
            },
            GasItems = new int[] {
            },
            GasSpeeds = new float[] {
            },
            UseHeightForBuild = false,
            Wind = 0.8f,
            IonHeight = 70f,
            WaterHeight = 0f,
            WaterItemId = 1116,
            Musics = new int[] { 
                10,
                11
            },
            SFXPath = "SFX/sfx-amb-lava-1",
            SFXVolume = 0.38f,
            CullingRadius = 0f
        };
    }
}