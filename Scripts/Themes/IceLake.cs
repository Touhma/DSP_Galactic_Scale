using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme IceLake = new()
        {
            Name = "IceLake",
            Base = true,
            DisplayName = "Maroonfrost".Translate(),
            PlanetType = EPlanetType.Desert,
            ThemeType = EThemeType.Telluric,
            BriefIntroduction = "猩红冰湖介绍",
            Eigenbit = 16,
            LDBThemeId = 20,
            Algo = 9,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Desert 7/",
            Temperature = -2.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(6.0f, 6.0f),
            ModY = new Vector2(8.0f, 8.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings //NEED CHANGING on sakura, and other themes imported
            {
                Color1 = new Color(0.1098037f, 0.1067106f, 0.1059606f, 1.0f),
                Color2 = new Color(0.01777778f, 0.03657109f, 0.07999983f, 1.0f),
                Color3 = new Color(0.031817377f, 0.03757387f, 0.103773594f, a: 1.0f),
                WaterColor1 = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                WaterColor2 = new Color(0.060475275f, 0.07024744f, 0.084905684f, 1.0f),
                WaterColor3 = new Color(0.031817377f, 0.03757387f, 0.103773594f, 1.0f),
                BiomeColor1 = new Color(0.5471698f, 0.5471698f, 0.5471698f, 1.0f),
                BiomeColor2 = new Color(0.20754719f, 0.05971876f, 0.07170483f, 1.0f),
                BiomeColor3 = new Color(0.2735849f, 0.17585924f, 0.12001601f, 1.0f),
                DustColor1 = new Color(1.0f, 1.0f, 1.0f, 1.0f),
                DustColor2 = new Color(0.6235294f, 0.3568628f, 0.3785979f, 1.0f),
                DustColor3 = new Color(0.7169812f, 0.5781871f, 0.4092204f, 1.0f),
                DustStrength1 = 3f,
                DustStrength2 = 2f,
                DustStrength3 = 2f,
                BiomeSound1 = 3,
                BiomeSound2 = 0,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.5f
            },
            Vegetables0 = new[]
            {
                748,
                749,
                750,
                743,
                744,
                745,
                746,
                747,
                741,
                742
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
                5,
                11,
                1,
                8,
                3,
                1,
                0
            },
            VeinCount = new[]
            {
                0.8f,
                1.0f,
                0.5f,
                1.0f,
                0.7f,
                0.3f,
                0.0f
            },
            VeinOpacity = new[]
            {
                1.0f,
                1.0f,
                1.0f,
                1.0f,
                0.5f,
                0.3f,
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
                1.0f,
                0.6f,
                0.6f,
                0.0f,
                0.2f,
                0.6f,
                0.7f,
                0.0f,
                0.15f,
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
            Wind = 0.7f,
            IonHeight = 55f,
            WaterHeight = 0f,
            WaterItemId = -2,
            Musics = new[]
            {
                4,
                11
            },
            SFXPath = "SFX/sfx-amb-desert-2",
            SFXVolume = 0.3f,
            CullingRadius = 0f,
            IceFlag = 1,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new(0.1098036f, 0.1067105f, 0.1059605f, 1f),
                    ["_AmbientColor1"] = new(0.01777778f, 0.03657109f, 0.07999978f, 1f),
                    ["_AmbientColor2"] = new(0.01944444f, 0.03333334f, 0.04999982f, 1f),
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightEmissionColor"] = new(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new(0.336107f, 0.7975054f, 0.8584906f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new(-0.5323462f, -0.03262195f, 0.8458977f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.5f,
                    ["_BioFuzzMask"] = 0.23f,
                    ["_BioFuzzStrength"] = 1f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 242.5393f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_HeightEmissionRadius"] = 50f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 1.8f,
                    ["_NormalStrength"] = 0.6f,
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
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_DayColor"] = new(0.5801887f, 0.8409807f, 1f, 0f),
                    ["_EmissionColor"] = new(0f, 0.6975104f, 0.735849f, 0.8039216f),
                    ["_IceColor"] = new(0.6421324f, 1f, 1f, 1f),
                    ["_NightColor"] = new(0.11639216f, 0.15227096f, 0.3137255f, 0f),
                    ["_RimColor"] = new(0.5215686f, 0.6563574f, 0.8666667f, 1f),
                    ["_SpeclColor"] = new(0.4759255f, 0.59973156f, 0.6509434f, 0.02745098f),
                    ["_SunColor"] = new(1f, 1f, 1f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_GiPower"] = 0.647f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_IceShift"] = 0.3f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_NormalStrength"] = 1f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_RefractionStrength"] = 0.75f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpeclIntensity"] = 1f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f
                }
            },
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(0.3443396f, 0.734796f, 1f, 1f),
                    ["_Color0"] = new(0.3247796f, 0.05882349f, 0.7843137f, 1f),
                    ["_Color1"] = new(0.6529663f, 0.6179333f, 0.8666667f, 1f),
                    ["_Color2"] = new(0.6332117f, 0.6510918f, 0.9058824f, 1f),
                    ["_Color3"] = new(0.5647059f, 0.7431953f, 0.9058824f, 1f),
                    ["_Color4"] = new(0.9803378f, 0.7627777f, 0.4320074f, 1f),
                    ["_Color5"] = new(0.3224255f, 0.262745f, 0.8862745f, 1f),
                    ["_Color6"] = new(0.3388532f, 0f, 0.6981132f, 1f),
                    ["_Color7"] = new(0.4475346f, 0.5393788f, 0.8396226f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new(0.7568628f, 0.8513581f, 0.9137255f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(200f, 100f, 260f, 0f),
                    ["_Sky0"] = new(0.5349729f, 0.367346f, 0.6132076f, 0.4078431f),
                    ["_Sky1"] = new(0.1397059f, 0.1434534f, 0.2379999f, 0.2588235f),
                    ["_Sky2"] = new(0.7294118f, 0.7927189f, 0.9607843f, 0.7490196f),
                    ["_Sky3"] = new(0.6696293f, 0.5921568f, 0.8196079f, 0.5254902f),
                    ["_Sky4"] = new(0.9926266f, 0.7900664f, 0.4038943f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 1f,
                    ["_AtmoThickness"] = 70f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_Density"] = 0.005f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DistanceControl"] = 0f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0.24f,
                    ["_FogDensity"] = 0.63f,
                    ["_FogSaturate"] = 0.69f,
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
                    ["_RimFogExp"] = 1.3f,
                    ["_RimFogPower"] = 3f,
                    ["_SkyAtmosPower"] = 8f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_StencilComp"] = 8f,
                    ["_StencilRef"] = 0f,
                    ["_SunColorAdd"] = 1f,
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
                    ["_Color"] = new(0.3920879f, 0.6082814f, 0.8396226f, 1f),
                    ["_ColorBio0"] = new(1f, 1f, 1f, 1f),
                    ["_ColorBio1"] = new(0.5960784f, 0.1862428f, 0.09411757f, 1f),
                    ["_ColorBio2"] = new(0.7547169f, 0.5113723f, 0.1886792f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_RimColor"] = new(0.07075468f, 0.7101329f, 1f, 1f),
                    ["_ShoreLineColor"] = new(0f, 0f, 0f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.38f,
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
                    ["_WireIntens"] = 0.6f,
                    ["_ZWrite"] = 1f
                }
            },
            thumbMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor"] = new(0.2274797f, 0.001245998f, 0.2641509f, 1f),
                    ["_Color"] = new(0.8396226f, 0.6930847f, 0.7047147f, 1f),
                    ["_ColorBio0"] = new(1f, 1f, 1f, 1f),
                    ["_ColorBio1"] = new(0.5188679f, 0.1112268f, 0.07587219f, 1f),
                    ["_ColorBio2"] = new(0.7264151f, 0.4494292f, 0.2501335f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(0.07058822f, 0.651593f, 0.8588235f, 1f),
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
                    ["_ShoreInvThick"] = 15.03f,
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