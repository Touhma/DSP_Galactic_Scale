using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme IceGelisol = new()
        {
            Name = "IceGelisol",
            Base = true,
            DisplayName = "Ice Field Gelisol".Translate(),
            PlanetType = EPlanetType.Ice,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 10,
            Algo = 3,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Ice 1/",
            Temperature = -5.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(1.0f, 1.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings(),

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
                5,
                1,
                3,
                10,
                2,
                1,
                0
            },
            VeinCount = new[]
            {
                0.6f,
                0.2f,
                0.8f,
                1.0f,
                0.8f,
                0.2f,
                0.0f
            },
            VeinOpacity = new[]
            {
                1.0f,
                0.5f,
                1.0f,
                1.0f,
                1.0f,
                0.3f,
                0.0f
            },
            RareVeins = new[]
            {
                8,
                10,
                12
            },
            RareSettings = new[]
            {
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
            WaterItemId = 1000,
            Musics = new[]
            {
                4,
                11
            },
            SFXPath = "SFX/sfx-amb-desert-2",
            SFXVolume = 0.3f,
            CullingRadius = 0f,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new(0.02903226f, 0.04838707f, 0.09999997f, 1f),
                    ["_AmbientColor1"] = new(0.04912774f, 0.06479169f, 0.1509433f, 1f),
                    ["_AmbientColor2"] = new(0.02352941f, 0.03811032f, 0.08627441f, 1f),
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LightColorScreen"] = new(0.6807843f, 0.8886667f, 0.972549f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_SpeclColor"] = new(0.6183695f, 0.6253266f, 0.6792453f, 0.854902f),
                    ["_SunDir"] = new(-0.7954169f, -0.1475017f, -0.5878395f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.5f,
                    ["_BioFuzzMask"] = 1f,
                    ["_BioFuzzStrength"] = 0f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 278.5116f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 0f,
                    ["_GISaturate"] = 1.3f,
                    ["_GIStrengthDay"] = 0.2f,
                    ["_GIStrengthNight"] = 0.01f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 1.8f,
                    ["_NormalStrength"] = 0.5f,
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
                    ["_BumpDirection"] = new(1f, 1f, -1f, 1f),
                    ["_BumpTiling"] = new(1f, 1f, -2f, 3f),
                    ["_CausticsColor"] = new(0.1704156f, 0.4529853f, 0.7607843f, 1f),
                    ["_Color"] = new(0.75f, 0.8075757f, 1f, 1f),
                    ["_Color0"] = new(0f, 0.1574037f, 0.2352941f, 1f),
                    ["_Color1"] = new(0.572549f, 0.717065f, 0.8980392f, 1f),
                    ["_Color2"] = new(0.2470588f, 0.6364762f, 0.8666667f, 1f),
                    ["_Color3"] = new(0.3135902f, 0.4327468f, 0.764151f, 1f),
                    ["_DensityParams"] = new(0.02f, 0.1f, 0f, 0f),
                    ["_DepthColor"] = new(0f, 0.06095791f, 0.1132075f, 1f),
                    ["_DepthFactor"] = new(0.4f, 0.46f, 0.5f, 0.1f),
                    ["_Foam"] = new(15f, 1f, 5f, 1.5f),
                    ["_FoamColor"] = new(0f, 0f, 0f, 1f),
                    ["_FoamParams"] = new(12f, 0.2f, 0.15f, 0.7f),
                    ["_FresnelColor"] = new(0.2598789f, 0.4030452f, 0.7547169f, 1f),
                    ["_InvFadeParemeter"] = new(0.9f, 0.25f, 0.5f, 0.08f),
                    ["_PLColor1"] = new(0f, 0f, 0f, 1f),
                    ["_PLColor2"] = new(0f, 0f, 0f, 1f),
                    ["_PLColor3"] = new(1f, 1f, 1f, 1f),
                    ["_PLParam1"] = new(0f, 0f, 0f, 0f),
                    ["_PLParam2"] = new(0f, 0f, 0f, 0f),
                    ["_PLPos1"] = new(0f, 0f, 0f, 0f),
                    ["_PLPos2"] = new(0f, 0f, 0f, 0f),
                    ["_PLPos3"] = new(0f, 0.1f, -0.5f, 0f),
                    ["_Paremeters1"] = new(0.02f, 0.1f, 0f, 0f),
                    ["_PointAtten"] = new(0f, 0.1f, -0.5f, 0f),
                    ["_PointLightPos"] = new(0f, 0.1f, -0.5f, 0f),
                    ["_ReflectionColor"] = new(0.1933962f, 0.5064065f, 1f, 1f),
                    ["_SLColor1"] = new(1f, 1f, 1f, 1f),
                    ["_SLDir1"] = new(0f, 0.1f, -0.5f, 0f),
                    ["_SLPos1"] = new(0f, 0.1f, -0.5f, 0f),
                    ["_SpecColor"] = new(1f, 1f, 1f, 1f),
                    ["_SpeclColor"] = new(1f, 1f, 1f, 1f),
                    ["_SpeclColor1"] = new(0.7764706f, 0.7937432f, 1f, 1f),
                    ["_Specular"] = new(0.9573934f, 0.8672858f, 0.5744361f, 0.9573934f),
                    ["_SunDirection"] = new(-0.6f, 0.8f, 0f, 0f),
                    ["_WorldLightDir"] = new(-0.6525278f, -0.6042119f, -0.4573132f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_CausticsTiling"] = 0.08f,
                    ["_DistortionStrength"] = 1f,
                    ["_FoamInvThickness"] = 2f,
                    ["_FoamSpeed"] = 0.02f,
                    ["_FoamSync"] = 5f,
                    ["_NormalSpeed"] = 0.5f,
                    ["_NormalStrength"] = 0.3f,
                    ["_NormalTiling"] = 0.1f,
                    ["_PLEdgeAtten"] = 0.5f,
                    ["_PLIntensity2"] = 0f,
                    ["_PLIntensity3"] = 0f,
                    ["_PLRange2"] = 10f,
                    ["_PLRange3"] = 10f,
                    ["_PointLightK"] = 0.01f,
                    ["_PointLightRange"] = 10f,
                    ["_Radius"] = 200f,
                    ["_ReflectionBlend"] = 0.86f,
                    ["_ReflectionTint"] = 0f,
                    ["_RefractionAmt"] = 1000f,
                    ["_RefractionStrength"] = 0.4f,
                    ["_SLCosCutoff1"] = 0.3f,
                    ["_SLIntensity1"] = 1f,
                    ["_SLRange1"] = 10f,
                    ["_Shininess"] = 40f,
                    ["_ShoreIntens"] = 2f,
                    ["_SpeclColorDayStrength"] = 0.5f,
                    ["_SpotExp"] = 2f,
                    ["_Tile"] = 0.05f
                }
            },
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_Color0"] = new(0.236027f, 0.3353746f, 0.9811321f, 1f),
                    ["_Color1"] = new(0.3537736f, 0.5737655f, 1f, 1f),
                    ["_Color2"] = new(0.4386792f, 0.6454816f, 1f, 1f),
                    ["_Color3"] = new(0.5990566f, 0.743727f, 1f, 1f),
                    ["_Color4"] = new(0.9778188f, 0.7633963f, 0.4335699f, 1f),
                    ["_Color5"] = new(0.2311321f, 0.622728f, 1f, 1f),
                    ["_Color6"] = new(0.4741487f, 0.2688679f, 1f, 1f),
                    ["_Color7"] = new(0.04841579f, 0.1621637f, 0.6037736f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new(0.75f, 0.75f, 0.75f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(200f, 100f, 255f, 0f),
                    ["_Sky0"] = new(0.3460751f, 0.4696855f, 0.7264151f, 0.2039216f),
                    ["_Sky1"] = new(0.02024743f, 0.1816752f, 0.3301886f, 0.2784314f),
                    ["_Sky2"] = new(0.6916608f, 0.7837354f, 0.9339623f, 0.5529412f),
                    ["_Sky3"] = new(0.2078431f, 0.3593507f, 0.6588235f, 0.6f),
                    ["_Sky4"] = new(0.991682f, 0.7941809f, 0.4115238f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 1f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0.2f,
                    ["_FogDensity"] = 0.3f,
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
                    ["_Color"] = new(0.06763966f, 0.408475f, 0.7547169f, 1f),
                    ["_ColorBio0"] = new(0.5683072f, 0.6747923f, 0.9339623f, 0.4470588f),
                    ["_ColorBio1"] = new(0.8501246f, 0.8656746f, 0.9056604f, 1f),
                    ["_ColorBio2"] = new(0.7783019f, 0.8196354f, 1f, 0.427451f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.1f, 0.2f, 0.18f),
                    ["_RimColor"] = new(0.0600747f, 0.2438102f, 0.8490566f, 1f),
                    ["_ShoreLineColor"] = new(0.02398541f, 0.2327791f, 0.4622641f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.5f,
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
                    ["_ShoreHeight"] = -4f,
                    ["_ShoreInvThick"] = 0.25f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_UVSec"] = 0f,
                    ["_WireIntens"] = 2f,
                    ["_ZWrite"] = 1f
                }
            },
            thumbMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor"] = new(0.3018868f, 0.3018868f, 0.3018868f, 1f),
                    ["_Color"] = new(0.1735937f, 0.3903478f, 0.7830188f, 1f),
                    ["_ColorBio0"] = new(0.4772605f, 0.5466523f, 0.6132076f, 1f),
                    ["_ColorBio1"] = new(1f, 1f, 1f, 1f),
                    ["_ColorBio2"] = new(0.9292453f, 0.9461915f, 1f, 0.5882353f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(1.5f, 2.3f, 0.16f, 0.05f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(0.2156015f, 0.3932859f, 0.8962264f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_ShoreLineColor"] = new(0.4362761f, 0.4997399f, 0.7169812f, 0f),
                    ["_SpecularColor"] = new(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new(0f, 0f, 1f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.59f,
                    ["_BodyIntensity"] = 0.27f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Diameter"] = 0.1f,
                    ["_Distort"] = 0.01f,
                    ["_DstBlend"] = 0f,
                    ["_FarHeight"] = 1f,
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