using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme AshenGelisol = new()
        {
            Name = "AshenGelisol",
            Base = true,
            DisplayName = "Ashen Gelisol".Translate(),
            PlanetType = EPlanetType.Desert,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 1,
            Algo = 2,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Desert 2/",
            Temperature = -1.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(1.0f, 1.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.01481844f, 0.06821825f, 0.08490568f, 1),
                Color2 = new Color(0.01568628f, 0.05103511f, 0.05490196f, 1),
                Color3 = new Color(0.01735494f, 0.04482913f, 0.05660379f, 1),
                WaterColor1 = new Color(0f, 0f, 0f, 1f),
                WaterColor2 = new Color(0.06047527f, 0.07024744f, 0.08490568f, 1),
                WaterColor3 = new Color(0.0260324f, 0.03199927f, 0.1415094f, 1),
                BiomeColor1 = new Color(0.1215686f, 0.2470588f, 0.4117647f, 1),
                BiomeColor2 = new Color(0.3215686f, 0.4117647f, 0.5921569f, 1),
                BiomeColor3 = new Color(0.3510854f, 0.3328587f, 0.8301887f, 1),
                DustColor1 = new Color(0.3254902f, 0.4246865f, 0.5176471f, 1),
                DustColor2 = new Color(0.7128f, 0.7480628f, 0.825f, 1),
                DustColor3 = new Color(0.590764f, 0.5864831f, 0.762f, 1),
                DustStrength1 = 7f,
                DustStrength2 = 6f,
                DustStrength3 = 3f,
                BiomeSound1 = 0,
                BiomeSound2 = 0,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.3f
            },
            Vegetables0 = new[]
            {
                6,
                7,
                8,
                9,
                10,
                11,
                12
            },
            Vegetables1 = new[]
            {
                1,
                2,
                3,
                4,
                5
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
                7,
                2,
                7,
                3,
                8,
                1,
                0
            },
            VeinCount = new[]
            {
                1.0f,
                0.5f,
                1.0f,
                1.0f,
                0.7f,
                0.3f,
                0.0f
            },
            VeinOpacity = new[]
            {
                0.6f,
                0.6f,
                1.0f,
                1.0f,
                0.5f,
                0.3f,
                0.0f
            },
            RareVeins = new[]
            {
                8,
                10
            },
            RareSettings = new[]
            {
                0.3f,
                0.5f,
                0.7f,
                0.5f,
                0.0f,
                0.3f,
                0.2f,
                0.6f
            },
            GasItems = new int[]
            {
            },
            GasSpeeds = new float[]
            {
            },
            UseHeightForBuild = false,
            Wind = 0.4f,
            IonHeight = 50f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new[]
            {
                4,
                11
            },
            SFXPath = "SFX/sfx-amb-desert-4",
            SFXVolume = 0.2f,
            CullingRadius = 0f,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new(0.01481844f, 0.06821825f, 0.08490568f, 1f),
                    ["_AmbientColor1"] = new(0.01568628f, 0.05103511f, 0.05490196f, 1f),
                    ["_AmbientColor2"] = new(0.01735494f, 0.04482913f, 0.05660379f, 1f),
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightEmissionColor"] = new(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new(0f, 0f, 0f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new(0f, 1f, 0f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.5f,
                    ["_BioFuzzMask"] = 1f,
                    ["_BioFuzzStrength"] = 0f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 0f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_HeightEmissionRadius"] = 50f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 1.8f,
                    ["_NormalStrength"] = 0.3f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_Radius"] = 200f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_StepBlend"] = 1f,
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
                    ["_Color0"] = new(0.1429778f, 0.1688499f, 0.1981131f, 1f),
                    ["_Color1"] = new(0.262745f, 0.3607908f, 0.4509804f, 1f),
                    ["_Color2"] = new(0.2423014f, 0.4279418f, 0.5188677f, 1f),
                    ["_Color3"] = new(0f, 0.6056479f, 0.6981132f, 1f),
                    ["_Color4"] = new(0.5189682f, 0.614396f, 0.9245279f, 1f),
                    ["_Color5"] = new(0.3673905f, 0.3858076f, 0.6037736f, 1f),
                    ["_Color6"] = new(0.03444285f, 0.4792436f, 0.8113207f, 1f),
                    ["_Color7"] = new(0.1523673f, 0.2800914f, 0.3018867f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new(0.75f, 0.75f, 0.75f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(-0.5925552f, 199.9078f, 10.55712f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 200f),
                    ["_PlanetRadius"] = new(200f, 100f, 270f, 0f),
                    ["_Sky0"] = new(0f, 0.7672294f, 0.9137255f, 0.1607843f),
                    ["_Sky1"] = new(0.07475961f, 0.2709139f, 0.45283f, 0.09803922f),
                    ["_Sky2"] = new(0f, 0.5446182f, 0.6320754f, 0.7137255f),
                    ["_Sky3"] = new(0.09834459f, 0.1568432f, 0.2452829f, 0.6862745f),
                    ["_Sky4"] = new(0.4141322f, 0.556892f, 0.8867913f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 0.9f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0.2f,
                    ["_FogDensity"] = 1f,
                    ["_FogSaturate"] = 1.2f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_Intensity"] = 1.1f,
                    ["_IntensityControl"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_RimFogExp"] = 1.3f,
                    ["_RimFogPower"] = 3f,
                    ["_SkyAtmosPower"] = 7f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_SunColorAdd"] = 20f,
                    ["_SunColorSkyUse"] = 0.05f,
                    ["_SunColorUse"] = 0.2f,
                    ["_SunRiseScatterPower"] = 60f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f
                }
            },
            minimapMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(0.4669809f, 0.7334906f, 1f, 1f),
                    ["_ColorBio0"] = new(0.06848517f, 0.1863792f, 0.2547169f, 1f),
                    ["_ColorBio1"] = new(0.2693129f, 0.4783546f, 0.6415094f, 1f),
                    ["_ColorBio2"] = new(0.4269312f, 0.6635363f, 0.7735849f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.05f, 0.05f),
                    ["_RimColor"] = new(0.5281684f, 0.6999155f, 0.7830188f, 1f),
                    ["_ShoreLineColor"] = new(0f, 0f, 0f, 0f)
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
                    ["_Color"] = new(0.5947846f, 0.6841848f, 0.7735849f, 1f),
                    ["_ColorBio0"] = new(0.126157f, 0.2091852f, 0.254717f, 1f),
                    ["_ColorBio1"] = new(0.4387682f, 0.5518765f, 0.6415094f, 1f),
                    ["_ColorBio2"] = new(0.6019492f, 0.7125413f, 0.764151f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.05f, 0.05f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(0.623576f, 0.7165718f, 0.764151f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_ShoreLineColor"] = new(0f, 0f, 0f, 0f),
                    ["_SpecularColor"] = new(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new(0f, 0f, 1f, 0f)
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