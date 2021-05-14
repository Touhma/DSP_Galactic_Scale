using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme IceGiant = new GSTheme()
        {
            Name = "IceGiant",
            DisplayName = "Ice Giant",
            PlanetType = EPlanetType.Gas,
            LDBThemeId = 4,
            Algo = 0,
            MaterialPath = "Universe/Materials/Planets/Gas 3/",
            Temperature = -1.0f,
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
                1011,
                1120
            },
            GasSpeeds = new float[] {
                0.7f,
                0.3f
            },
            UseHeightForBuild = false,
            Wind = 0f,
            IonHeight = 0f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new int[] { },
            SFXPath = "SFX/sfx-amb-massive",
            SFXVolume = 0.2f,
            CullingRadius = 0f
        };
    }
}