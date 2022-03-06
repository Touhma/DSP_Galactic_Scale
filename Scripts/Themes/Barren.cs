using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Barren = new()
        {
            Name = "Barren",
            Base = true,
            DisplayName = "Barren Desert".Translate(),
            PlanetType = EPlanetType.Desert,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 11,
            Algo = 4,
            MinRadius = 5,
            MaxRadius = 250,
            MaterialPath = "Universe/Materials/Planets/Desert 3/",
            Temperature = 0.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(1.0f, 1.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.03529412f, 0.04313726f, 0.03921569f, 1),
                Color2 = new Color(0.04313726f, 0.05490196f, 0.0627451f, 1),
                Color3 = new Color(0.03137255f, 0.03921569f, 0.03137255f, 1),
                WaterColor1 = new Color(0f, 0f, 0f, 1f),
                WaterColor2 = new Color(0.06047527f, 0.07024744f, 0.08490568f, 1),
                WaterColor3 = new Color(0.0260324f, 0.03199927f, 0.1415094f, 1),
                BiomeColor1 = new Color(0.1137255f, 0.145098f, 0.1882353f, 1),
                BiomeColor2 = new Color(0.6352941f, 0.6352941f, 0.627451f, 1),
                BiomeColor3 = new Color(0.9098039f, 0.8901961f, 0.8509804f, 1),
                DustColor1 = new Color(0.5411765f, 0.562465f, 0.5882353f, 1),
                DustColor2 = new Color(0.8584906f, 0.8584906f, 0.8584906f, 1),
                DustColor3 = new Color(1, 1, 1, 1),
                DustStrength1 = 7.5f,
                DustStrength2 = 7.5f,
                DustStrength3 = 7.5f,
                BiomeSound1 = 0,
                BiomeSound2 = 0,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.55f
            },
            Vegetables0 = new int[]
            {
            },
            Vegetables1 = new int[]
            {
            },
            Vegetables2 = new int[]
            {
            },
            Vegetables3 = new int[]
            {
            },
            Vegetables4 = new int[]
            {
            },
            Vegetables5 = new int[]
            {
            },
            VeinSpot = new[]
            {
                3,
                3,
                3,
                6,
                12,
                0,
                0
            },
            VeinCount = new[]
            {
                0.5f,
                0.5f,
                0.5f,
                1.0f,
                1.2f,
                0.0f,
                0.0f
            },
            VeinOpacity = new[]
            {
                0.6f,
                0.6f,
                0.9f,
                0.9f,
                1.5f,
                0.0f,
                0.0f
            },
            RareVeins = new[]
            {
                8,
                9,
                12
            },
            RareSettings = new[]
            {
                0.25f,
                0.5f,
                0.6f,
                0.6f,
                0.0f,
                0.2f,
                0.6f,
                0.7f,
                0.0f,
                0.1f,
                0.2f,
                0.5f
            },
            GasItems = new int[]
            {
            },
            GasSpeeds = new float[]
            {
            },
            UseHeightForBuild = false,
            Wind = 0f,
            IonHeight = 0f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new[]
            {
                5,
                11
            },
            SFXPath = "SFX/sfx-amb-desert-2",
            SFXVolume = 0.15f,
            CullingRadius = 0f,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_Color0"] = new(1f, 1f, 1f, 1f),
                    ["_Color1"] = new(1f, 1f, 1f, 1f),
                    ["_Color2"] = new(1f, 1f, 1f, 1f),
                    ["_Color3"] = new(1f, 1f, 1f, 1f),
                    ["_Color4"] = new(1f, 0.738227f, 0.4127178f, 1f),
                    ["_Color5"] = new(1f, 1f, 1f, 1f),
                    ["_Color6"] = new(1f, 1f, 1f, 1f),
                    ["_Color7"] = new(1f, 1f, 1f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new(0.75f, 0.75f, 0.75f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(162.6652f, -110.0747f, -39.22104f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(200f, 100f, 270f, 0f),
                    ["_Sky0"] = new(1f, 1f, 1f, 0f),
                    ["_Sky1"] = new(1f, 1f, 1f, 0f),
                    ["_Sky2"] = new(1f, 1f, 1f, 0f),
                    ["_Sky3"] = new(1f, 1f, 1f, 0f),
                    ["_Sky4"] = new(1f, 0.7284361f, 0.30631f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 0.3f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0f,
                    ["_FogDensity"] = 0f,
                    ["_FogSaturate"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_Intensity"] = 0.2f,
                    ["_IntensityControl"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_RimFogExp"] = 0f,
                    ["_RimFogPower"] = 0f,
                    ["_SkyAtmosPower"] = 40f,
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
            oceanMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>(),
                Params = new Dictionary<string, float>()
            },
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_Color0"] = new(1f, 1f, 1f, 1f),
                    ["_Color1"] = new(1f, 1f, 1f, 1f),
                    ["_Color2"] = new(1f, 1f, 1f, 1f),
                    ["_Color3"] = new(1f, 1f, 1f, 1f),
                    ["_Color4"] = new(1f, 0.738227f, 0.4127178f, 1f),
                    ["_Color5"] = new(1f, 1f, 1f, 1f),
                    ["_Color6"] = new(1f, 1f, 1f, 1f),
                    ["_Color7"] = new(1f, 1f, 1f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new(0.75f, 0.75f, 0.75f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(162.6652f, -110.0747f, -39.22104f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(200f, 100f, 270f, 0f),
                    ["_Sky0"] = new(1f, 1f, 1f, 0f),
                    ["_Sky1"] = new(1f, 1f, 1f, 0f),
                    ["_Sky2"] = new(1f, 1f, 1f, 0f),
                    ["_Sky3"] = new(1f, 1f, 1f, 0f),
                    ["_Sky4"] = new(1f, 0.7284361f, 0.30631f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 0.3f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0f,
                    ["_FogDensity"] = 0f,
                    ["_FogSaturate"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_Intensity"] = 0.2f,
                    ["_IntensityControl"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_RimFogExp"] = 0f,
                    ["_RimFogPower"] = 0f,
                    ["_SkyAtmosPower"] = 40f,
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
                    ["_Color"] = new(0.6792453f, 0.6792453f, 0.6792453f, 1f),
                    ["_ColorBio0"] = new(0.3490566f, 0.3490566f, 0.3490566f, 1f),
                    ["_ColorBio1"] = new(0.5754716f, 0.5754716f, 0.5754716f, 1f),
                    ["_ColorBio2"] = new(0.8396226f, 0.8396226f, 0.8396226f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.05f, 0.05f),
                    ["_RimColor"] = new(0.8490566f, 0.8490566f, 0.8490566f, 1f),
                    ["_ShoreLineColor"] = new(0.3773585f, 0.3773585f, 0.3773585f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.6f,
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
                    ["_ShoreInvThick"] = 6f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_UVSec"] = 0f,
                    ["_WireIntens"] = 0.7f,
                    ["_ZWrite"] = 1f
                }
            },
            thumbMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor"] = new(0f, 0.2778448f, 0.2924528f, 1f),
                    ["_Color"] = new(0.6784314f, 0.6784314f, 0.6784314f, 1f),
                    ["_ColorBio0"] = new(0.3490196f, 0.3490196f, 0.3490196f, 1f),
                    ["_ColorBio1"] = new(0.5764706f, 0.5764706f, 0.5764706f, 1f),
                    ["_ColorBio2"] = new(0.8392157f, 0.8392157f, 0.8392157f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.05f, 0.05f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(0.8509804f, 0.8509804f, 0.8509804f, 1f),
                    ["_Rotation"] = new(-0.01256308f, -0.4212481f, -0.0277748f, 0.906433f),
                    ["_ShoreLineColor"] = new(0f, 0f, 0f, 0f),
                    ["_SpecularColor"] = new(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new(-0.9210891f, 0.1039214f, 0.3752268f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.6f,
                    ["_BodyIntensity"] = 0.27f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Diameter"] = 0.1f,
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
                    ["_ShoreInvThick"] = 6f,
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