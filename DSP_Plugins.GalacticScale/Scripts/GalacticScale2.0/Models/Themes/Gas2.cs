using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Gas2 = new GSTheme()
        {
            Name = "GasGiant2",
            DisplayName = "Gas Giant",
            PlanetType = EPlanetType.Gas,
            LDBThemeId = 3,
            Algo = 0,
            MaterialPath = "Universe/Materials/Planets/Gas 2/",
            Temperature = 1.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(0.0f, 0.0f),
            VeinSettings = new GSVeinSettings() { algorithm = "Vanilla" },
            Vegetables0 = new int[] { },
            Vegetables1 = new int[] { },
            Vegetables2 = new int[] { },
            Vegetables3 = new int[] { },
            Vegetables4 = new int[] { },
            Vegetables5 = new int[] { },
            VeinSpot = new int[] { },
            VeinCount = new float[] { },
            VeinOpacity = new float[] { },
            RareVeins = new int[] { },
            RareSettings = new float[] { },
            GasItems = new int[] {
                1120,
                1121
            },
            GasSpeeds = new float[] {
                0.96f,
                0.04f
            },
            UseHeightForBuild = false,
            Wind = 0f,
            IonHeight = 0f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new int[] { },
            SFXPath = "SFX/sfx-amb-massive",
            SFXVolume = 0.27f,
            CullingRadius = 0f
        };
    }
}