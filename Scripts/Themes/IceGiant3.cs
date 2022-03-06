using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme IceGiant3 = new()
        {
            Name = "IceGiant3",
            Base = true,
            DisplayName = "Ice Giant III".Translate(),
            PlanetType = EPlanetType.Gas,

            LDBThemeId = 4,
            Algo = 0,
            MinRadius = 5,
            MaxRadius = 510,
            ThemeType = EThemeType.Gas,
            MaterialPath = "Universe/Materials/Planets/Gas 3/",
            Temperature = -1.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(0.0f, 0.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings(),
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
            GasItems = new[]
            {
                1011,
                1120
            },
            GasSpeeds = new[]
            {
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
            CullingRadius = 0f,
            minimapMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(0.4048111f, 0.7830001f, 0.5900388f, 1f),
                    ["_ColorBio0"] = new(0.7688679f, 1f, 0.9134774f, 1f),
                    ["_ColorBio1"] = new(0.2901962f, 0.8f, 0.5625939f, 1f),
                    ["_ColorBio2"] = new(0.627451f, 1f, 0.8639832f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_RimColor"] = new(0.3921568f, 1f, 0.846783f, 1f),
                    ["_ShoreLineColor"] = new(0f, 0f, 0f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.35f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_ShoreHeight"] = 0f,
                    ["_ShoreInvThick"] = 13f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_UVSec"] = 0f,
                    ["_WireIntens"] = 1f,
                    ["_ZWrite"] = 1f
                }
            },
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new(0f, 0f, 0f, 0f),
                    ["_AmbientColor1"] = new(0f, 0f, 0f, 0f),
                    ["_AmbientColor2"] = new(0f, 0f, 0f, 0f),
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_DistortSettings1"] = new(100f, 27f, 10f, 17f),
                    ["_DistortSettings2"] = new(50f, 13f, 10f, 19f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(1f, 1f, 1f, 0.8039216f),
                    ["_RimColor"] = new(0.6078432f, 0.7529807f, 1f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_SpecularColor"] = new(0.0627451f, 0.2197301f, 0.3411765f, 1f),
                    ["_Spot0"] = new(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new(0.8f, 0.6f, 0f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 0f,
                    ["_Distort"] = 0.01f,
                    ["_DstBlend"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.15f,
                    ["_GlossyReflections"] = 1f,
                    ["_Metallic"] = 0.58f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 1.1f,
                    ["_NoiseThres"] = 0.2f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_PolarWhirl"] = -0.3f,
                    ["_PolarWhirlPower"] = 50f,
                    ["_Radius"] = 800f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_Speed"] = 1.5f,
                    ["_SrcBlend"] = 1f,
                    ["_TileX"] = 2f,
                    ["_TileY"] = 1f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f
                }
            },
            thumbMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor"] = new(0f, 0f, 0f, 1f),
                    ["_Color"] = new(0.3953756f, 0.881f, 0.5629805f, 1f),
                    ["_ColorBio0"] = new(0.768868f, 1f, 0.9134774f, 1f),
                    ["_ColorBio1"] = new(0.2901961f, 0.8f, 0.5625939f, 1f),
                    ["_ColorBio2"] = new(0.6274511f, 1f, 0.8639832f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(0.3921568f, 1f, 0.898458f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_ShoreLineColor"] = new(0f, 0f, 0f, 0f),
                    ["_SpecularColor"] = new(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new(0f, 0f, 1f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.35f,
                    ["_BodyIntensity"] = 0.27f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Diameter"] = 0.4f,
                    ["_Distort"] = 0.01f,
                    ["_DstBlend"] = 0f,
                    ["_FarHeight"] = 0.5f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.15f,
                    ["_GlossyReflections"] = 1f,
                    ["_HoloIntensity"] = 0.8f,
                    ["_Metallic"] = 0.58f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 4f,
                    ["_NoiseIntensity"] = 0.15f,
                    ["_NoiseThres"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_PolarWhirl"] = -0.3f,
                    ["_PolarWhirlPower"] = 8f,
                    ["_ShoreHeight"] = 0f,
                    ["_ShoreInvThick"] = 13f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_Speed"] = 2f,
                    ["_SrcBlend"] = 1f,
                    ["_Tile"] = 0.2f,
                    ["_TileX"] = 7f,
                    ["_TileY"] = 2.5f,
                    ["_TimeFactor"] = 0f,
                    ["_ToggleVerta"] = 0f,
                    ["_UVSec"] = 0f,
                    ["_WireIntens"] = 2f,
                    ["_ZWrite"] = 1f
                }
            }
        };
    }
}