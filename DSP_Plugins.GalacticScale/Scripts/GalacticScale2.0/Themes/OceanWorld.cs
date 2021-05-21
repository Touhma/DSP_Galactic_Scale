using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        //Note: Culling Radius is -10
        public static GSTheme OceanWorld = new GSTheme()
        {
            Name = "OceanWorld",
            DisplayName = "Ocean World",
            PlanetType = EPlanetType.Ocean,
            LDBThemeId = 16,
            Algo = 7,
            MaterialPath = "Universe/Materials/Planets/Ocean 5/",
            Temperature = 0.0f,
            Distribute = EThemeDistribute.Interstellar,
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
                0,
                0,
                0,
                0,
                0,
                2,
                10
            },
            VeinCount = new float[] {
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                0.5f,
                5.0f
            },
            VeinOpacity = new float[] {
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                0.8f,
                2.0f
            },
            RareVeins = new int[] { 
                13 
            },
            RareSettings = new float[] {
                1.0f,
                1.0f,
                1.0f,
                0.6f
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
            CullingRadius = -10f
        };
    }
}