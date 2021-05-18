using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Mediterranean = new GSTheme()
        {
            Name = "Mediterranean",
            DisplayName = "Mediterranean",
            PlanetType = EPlanetType.Ocean,
            LDBThemeId = 1,
            Algo = 1,
            MaterialPath = "Universe/Materials/Planets/Ocean 1/",
            Temperature = 0.0f,
            Distribute = EThemeDistribute.Birth,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(0.0f, 0.0f),
            VeinSettings = new GSVeinSettings() { 
                VeinAlgorithm = "Vanilla", 
                VeinTypes = new List<GSVeinType>()

            },
            Vegetables0 = new int[] {
                604,
                605,
                603,
                604,
                102,
                604,
                605,
                105,
                602,
                601
            },
            Vegetables1 = new int[] {
                103,
                102,
                103,
                104,
                104,
                104,
                101,
                104,
                604,
                106
            },
            Vegetables2 = new int[] {
                1001,
                1002,
                1003
            },
            Vegetables3 = new int[] {
                1005,
                1006,
                1007,
                1006,
                1007
            },
            Vegetables4 = new int[] {
                1004
            },
            Vegetables5 = new int[] {
            },
            VeinSpot = new int[] {
                7,
                5,
                0,
                0,
                8,
                11,
                18
            },
            VeinCount = new float[] {
                0.7f,
                0.6f,
                0.0f,
                0.0f,
                1.0f,
                1.0f,
                1.0f
            },
            VeinOpacity = new float[] {
                0.6f,
                0.5f,
                0.0f,
                0.0f,
                0.7f,
                1.0f,
                1.0f
            },
            RareVeins = new int[] {
                11
            },
            RareSettings = new float[] {
                0.0f,
                1.0f,
                0.3f,
                0.3f
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
