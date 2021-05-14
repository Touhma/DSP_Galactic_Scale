using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme IceGelisol = new GSTheme()
        {
            Name = "IceGgelisol",
            DisplayName = "Ice Field Gelisol",
            PlanetType = EPlanetType.Ice,
            LDBThemeId = 10,
            Algo = 3,
            MaterialPath = "Universe/Materials/Planets/Ice 1/",
            Temperature = -5.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(1.0f, 1.0f),
            VeinSettings = new GSVeinSettings() { algorithm = "Vanilla" },
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
                5,
                1,
                3,
                10,
                2,
                1,
                0
            },
            VeinCount = new float[] {
                0.6f,
                0.2f,
                0.8f,
                1.0f,
                0.8f,
                0.2f,
                0.0f
            },
            VeinOpacity = new float[] {
                1.0f,
                0.5f,
                1.0f,
                1.0f,
                1.0f,
                0.3f,
                0.0f
            },
            RareVeins = new int[] {
                8,
                10,
                12
            },
            RareSettings = new float[] {
                0.3f,
                1.0f,
                0.8f,
                1.0f,
                0.0f,
                0.2f,
                0.6f,
                0.4f,
                0.0f,
                0.1f,
                0.2f,
                0.4f
            },
            GasItems = new int[] {
            },
            GasSpeeds = new float[] {
            },
            UseHeightForBuild = false,
            Wind = 0.7f,
            IonHeight = 55f,
            WaterHeight = 0f,
            WaterItemId = 1000,
            Musics = new int[] { 
                4,
                11
            },
            SFXPath = "SFX/sfx-amb-desert-2",
            SFXVolume = 0.3f,
            CullingRadius = 0f
        };
    }
}