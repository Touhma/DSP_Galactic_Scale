using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Gobi = new()
        {
            Name = "Gobi",
            Base = true,
            DisplayName = "Gobi Desert".Translate(),
            PlanetType = EPlanetType.Desert,
            ThemeType = EThemeType.Telluric,
            LDBThemeId = 12,
            Algo = 3,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Desert 4/",
            Temperature = 1.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(1.0f, 1.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.1411765f, 0.09803922f, 0.03529412f, 1f),
                Color2 = new Color(0.1333333f, 0.09803922f, 0.1058824f, 1f),
                Color3 = new Color(0.0215379f, 0.02963686f, 0.03773582f, 1f),
                WaterColor1 = new Color(0f, 0f, 0f, 1f),
                WaterColor2 = new Color(0.06047527f, 0.07024744f, 0.08490568f, 1f),
                WaterColor3 = new Color(0.0260324f, 0.03199927f, 0.1415094f, 1f),
                BiomeColor1 = new Color(0.8627451f, 0.7137255f, 0.4352941f, 1f),
                BiomeColor2 = new Color(0.4509804f, 0.282353f, 0.0627451f, 1f),
                BiomeColor3 = new Color(0.145098f, 0.09411765f, 0.05098039f, 1f),
                DustColor1 = new Color(1f, 0.9046326f, 0.79f, 1),
                DustColor2 = new Color(0.832f, 0.7212619f, 0.517504f, 0.7607843f),
                DustColor3 = new Color(0.4156863f, 0.3687657f, 0.2905647f, 1f),
                DustStrength1 = 7.5f,
                DustStrength2 = 4f,
                DustStrength3 = 1f,
                BiomeSound1 = 0,
                BiomeSound2 = 2,
                BiomeSound3 = 3,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.21f
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
                2,
                7,
                8,
                0,
                7,
                3,
                0
            },
            VeinCount = new[]
            {
                0.4f,
                1.0f,
                1.0f,
                0.0f,
                1.0f,
                0.7f,
                0.0f
            },
            VeinOpacity = new[]
            {
                0.8f,
                1.0f,
                1.0f,
                0.0f,
                1.0f,
                0.7f,
                0.0f
            },
            RareVeins = new[]
            {
                9,
                10,
                12
            },
            RareSettings = new[]
            {
                0.0f,
                0.25f,
                0.6f,
                0.6f,
                0.0f,
                0.25f,
                0.6f,
                0.6f,
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
            Wind = 0.8f,
            IonHeight = 60f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new[]
            {
                11,
                4
            },
            SFXPath = "SFX/sfx-amb-desert-1",
            SFXVolume = 0.4f,
            CullingRadius = 0f,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new(0.07843118f, 0.04538548f, 0.01647059f, 1f),
                    ["_AmbientColor1"] = new(0.05098039f, 0.03529412f, 0.05098039f, 1f),
                    ["_AmbientColor2"] = new(0.01960784f, 0.02352941f, 0.03137255f, 1f),
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightEmissionColor"] = new(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new(0.93f, 0.93f, 0.93f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new(0.6943676f, -0.1823572f, 0.6961318f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.5f,
                    ["_BioFuzzMask"] = 1f,
                    ["_BioFuzzStrength"] = 0f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 290.6332f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_HeightEmissionRadius"] = 50f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 1.8f,
                    ["_NormalStrength"] = 0.55f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_Radius"] = 200f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_StepBlend"] = 0.5f,
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
                    ["_Color0"] = new(0.1411764f, 0.1602586f, 0.1999999f, 1f),
                    ["_Color1"] = new(0.262745f, 0.4114617f, 0.4509804f, 1f),
                    ["_Color2"] = new(0.2431372f, 0.4768073f, 0.517647f, 1f),
                    ["_Color3"] = new(0.63835f, 0.8962264f, 0.8160756f, 1f),
                    ["_Color4"] = new(1f, 0.7521219f, 0.4177153f, 1f),
                    ["_Color5"] = new(0.475525f, 0.6395648f, 0.735849f, 1f),
                    ["_Color6"] = new(0.02429692f, 0.4044923f, 0.735849f, 1f),
                    ["_Color7"] = new(0.1065326f, 0.3187077f, 0.3584906f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new(0.75f, 0.75f, 0.75f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(0f, 0f, 200f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(200f, 100f, 270f, 0f),
                    ["_Sky0"] = new(0.2794143f, 0.3679245f, 0.3553887f, 0.1607843f),
                    ["_Sky1"] = new(0.07450979f, 0.425214f, 0.4509804f, 0.09803922f),
                    ["_Sky2"] = new(0.510413f, 0.6981132f, 0.6553206f, 0.7137255f),
                    ["_Sky3"] = new(0.09803919f, 0.2076341f, 0.2470588f, 0.6862745f),
                    ["_Sky4"] = new(1f, 0.7492285f, 0.3331028f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 0.9f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0.22f,
                    ["_FogDensity"] = 0.8f,
                    ["_FogSaturate"] = 1f,
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
                    ["_SunColorAdd"] = 5f,
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
                    ["_Color"] = new(0.4431372f, 0.7078432f, 0.737255f, 1f),
                    ["_ColorBio0"] = new(0.9921568f, 0.9053907f, 0.5962862f, 1f),
                    ["_ColorBio1"] = new(0.772549f, 0.649399f, 0.3569176f, 1f),
                    ["_ColorBio2"] = new(0.5137254f, 0.4059545f, 0.2078431f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.05f, 0.05f),
                    ["_RimColor"] = new(0.07569417f, 0.764151f, 0.4546846f, 1f),
                    ["_ShoreLineColor"] = new(0.5849056f, 0.4023253f, 0.1296724f, 0f)
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
                    ["_Color"] = new(0.7547169f, 0.7161235f, 0.5660378f, 1f),
                    ["_ColorBio0"] = new(0.9921568f, 0.8632348f, 0.5529411f, 1f),
                    ["_ColorBio1"] = new(0.7960784f, 0.5621936f, 0.2117568f, 1f),
                    ["_ColorBio2"] = new(0.2862744f, 0.1438568f, 0.007843141f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.05f, 0.05f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(0f, 0.9056604f, 0.6770036f, 1f),
                    ["_Rotation"] = new(0.00745134f, 0.9836903f, 0.07837705f, -0.1617253f),
                    ["_ShoreLineColor"] = new(0.3584903f, 0.2679639f, 0.1302062f, 0f),
                    ["_SpecularColor"] = new(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new(-0.5486495f, -0.03537795f, -0.8353036f, 0f)
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