using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Gas4 = new()
        {
            Name = "GasGiant4",
            Base = true,
            DisplayName = "Gas Giant IV".Translate(),
            PlanetType = EPlanetType.Gas,
            LDBThemeId = 3,
            Algo = 0,
            MinRadius = 5,
            MaxRadius = 510,
            ThemeType = EThemeType.Gas,
            MaterialPath = "Universe/Materials/Planets/Gas 2/",
            Temperature = 0.0f,
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
                1120,
                1121
            },
            GasSpeeds = new[]
            {
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
            CullingRadius = 0f,
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(0.3443396f, 0.734796f, 1f, 1f),
                    ["_Color0"] = new(0.8584906f, 0.4737896f, 0.5086713f, 1f),
                    ["_Color1"] = new(0.867f, 0.5423699f, 0.5463288f, 1f),
                    ["_Color2"] = new(0.846f, 0.5680286f, 0.5962285f, 1f),
                    ["_Color3"] = new(0.846f, 0.5568061f, 0.6172346f, 1f),
                    ["_Color4"] = new(1f, 0.7494254f, 0.4167455f, 1f),
                    ["_Color5"] = new(0f, 0f, 0f, 1f),
                    ["_Color6"] = new(0.2823529f, 0f, 0.1778538f, 1f),
                    ["_Color7"] = new(0f, 0f, 0f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(800f, 800f, 830f, 0f),
                    ["_Sky0"] = new(0f, 0f, 0f, 0.1607843f),
                    ["_Sky1"] = new(0f, 0f, 0f, 0.09803922f),
                    ["_Sky2"] = new(0f, 0f, 0f, 0.9176471f),
                    ["_Sky3"] = new(0f, 0f, 0f, 0.5411765f),
                    ["_Sky4"] = new(1f, 0.7398548f, 0.3210239f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 2f,
                    ["_AtmoThickness"] = 70f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_Density"] = 0.005f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FogDensity"] = 1.1f,
                    ["_FogSaturate"] = 1f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_GroundAtmosPower"] = 3f,
                    ["_Intensity"] = 1f,
                    ["_IntensityControl"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_SkyAtmosPower"] = 7f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_SunColorAdd"] = 0f,
                    ["_SunColorSkyUse"] = 1f,
                    ["_SunColorUse"] = 1f,
                    ["_SunRiseScatterPower"] = 60f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f
                }
            },
            minimapMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(0.8113208f, 0.5396048f, 0.5978298f, 1f),
                    ["_ColorBio0"] = new(0.8301887f, 0.7009612f, 0.7412124f, 1f),
                    ["_ColorBio1"] = new(0.8f, 0.2901962f, 0.4837265f, 1f),
                    ["_ColorBio2"] = new(1f, 0.627451f, 0.6870648f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_RimColor"] = new(1f, 0.3921568f, 0.6361164f, 1f),
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
                    ["_FlowColor"] = new(0.5943396f, 0.3710105f, 0.367257f, 0.5607843f),
                    ["_RimColor"] = new(1f, 1f, 1f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_SpecularColor"] = new(0.3411765f, 0.2612392f, 0.0627451f, 1f),
                    ["_Spot0"] = new(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new(0.8f, 0.6f, 0f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 0f,
                    ["_Distort"] = 0f,
                    ["_DstBlend"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.15f,
                    ["_GlossyReflections"] = 1f,
                    ["_Metallic"] = 0.58f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 1.4f,
                    ["_NoiseThres"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_PolarWhirl"] = -0.4f,
                    ["_PolarWhirlPower"] = 20f,
                    ["_Radius"] = 800f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_Speed"] = 2f,
                    ["_SrcBlend"] = 1f,
                    ["_TileX"] = 2f,
                    ["_TileY"] = 2f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f
                }
            },
            thumbMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor"] = new(0f, 0f, 0f, 1f),
                    ["_Color"] = new(0.6705883f, 0.4901961f, 0.5343012f, 1f),
                    ["_ColorBio0"] = new(0.9150943f, 0.5913581f, 0.690125f, 1f),
                    ["_ColorBio1"] = new(0.8f, 0.2901962f, 0.7173324f, 1f),
                    ["_ColorBio2"] = new(1f, 0.627451f, 0.9476808f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(1f, 0.3921568f, 0.6361164f, 1f),
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