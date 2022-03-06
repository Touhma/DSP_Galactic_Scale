using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme AridDesert = new()
        {
            Name = "AridDesert",
            Base = true,
            DisplayName = "Arid Desert".Translate(),
            PlanetType = EPlanetType.Desert,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 6,
            Algo = 2,
            MinRadius = 100,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Desert 1/",
            Temperature = 2.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(1.0f, 1.0f),
            ModY = new Vector2(0.0f, 0.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.1509434f, 0.08707725f, 0.06763973f, 1f),
                Color2 = new Color(0.02352941f, 0.02611319f, 0.1411765f, 1f),
                Color3 = new Color(0.02681303f, 0.01960784f, 0.1215686f, 1f),
                WaterColor1 = new Color(0, 0, 0, 1),
                WaterColor2 = new Color(0.06047527f, 0.07024744f, 0.08490568f, 1f),
                WaterColor3 = new Color(0.0260324f, 0.03199927f, 0.1415094f, 1f),
                BiomeColor1 = new Color(0.6415094f, 0.4455594f, 0.251157f, 1f),
                BiomeColor2 = new Color(0.7075472f, 0.3318314f, 0.2302866f, 1f),
                BiomeColor3 = new Color(0.4245283f, 0.1293824f, 0.08610716f, 1f),
                DustColor1 = new Color(0.888f, 0.7521883f, 0.6128941f, 1f),
                DustColor2 = new Color(0.8313726f, 0.5626393f, 0.4589177f, 1f),
                DustColor3 = new Color(0.7843137f, 0.457984f, 0.3882353f, 1f),
                DustStrength1 = 7.5f,
                DustStrength2 = 6.5f,
                DustStrength3 = 3.5f,
                BiomeSound1 = 0,
                BiomeSound2 = 0,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.35f
            },
            Vegetables0 = new[]
            {
                617,
                619,
                616,
                618,
                615,
                618,
                615,
                614,
                613,
                612,
                611
            },
            Vegetables1 = new[]
            {
                616,
                618,
                619,
                617,
                619,
                615,
                613,
                614,
                612
            },
            Vegetables2 = new[]
            {
                1041,
                1042,
                1043
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
                10,
                0,
                6,
                10,
                1,
                0
            },
            VeinCount = new[]
            {
                0.5f,
                1.0f,
                0.0f,
                1.0f,
                1.0f,
                0.3f,
                0.0f
            },
            VeinOpacity = new[]
            {
                0.6f,
                0.6f,
                0.0f,
                1.0f,
                1.0f,
                0.3f,
                0.0f
            },
            RareVeins = new[]
            {
                9
            },
            RareSettings = new[]
            {
                0.0f,
                0.18f,
                0.2f,
                0.3f
            },
            GasItems = new int[]
            {
            },
            GasSpeeds = new float[]
            {
            },
            UseHeightForBuild = false,
            Wind = 1.5f,
            IonHeight = 70f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new[]
            {
                11, 4
            },
            SFXPath = "SFX/sfx-amb-desert-3",
            SFXVolume = 0.4f,
            CullingRadius = 0f,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new(0.1509434f, 0.08707725f, 0.06763973f, 1f),
                    ["_AmbientColor1"] = new(0.02352941f, 0.02611319f, 0.1411765f, 1f),
                    ["_AmbientColor2"] = new(0.02681303f, 0.01960784f, 0.1215686f, 1f),
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
                    ["_NormalStrength"] = -0.2f,
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
                    ["_Color"] = new(0.3443396f, 0.734796f, 1f, 1f),
                    ["_Color0"] = new(0.9693012f, 0.8632076f, 1f, 1f),
                    ["_Color1"] = new(0.9921568f, 0.8872655f, 0.8352941f, 1f),
                    ["_Color2"] = new(1f, 0.8569432f, 0.7764706f, 1f),
                    ["_Color3"] = new(1f, 0.8532762f, 0.768868f, 1f),
                    ["_Color4"] = new(1f, 0.7366369f, 0.6556604f, 1f),
                    ["_Color5"] = new(0.3679245f, 0.2169366f, 0.3509532f, 1f),
                    ["_Color6"] = new(0.7194457f, 0.5852616f, 0.9056604f, 1f),
                    ["_Color7"] = new(0.3753907f, 0.1137253f, 0.6117647f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new(0.75f, 0.75f, 0.75f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(-92.77879f, 112.7748f, 137.4607f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 200f),
                    ["_PlanetRadius"] = new(200f, 100f, 270f, 0f),
                    ["_Sky0"] = new(0.6792453f, 0.483802f, 0.4895427f, 0.1607843f),
                    ["_Sky1"] = new(0.7226284f, 0.6933962f, 1f, 0.09803922f),
                    ["_Sky2"] = new(1f, 0.808275f, 0.6745283f, 0.7490196f),
                    ["_Sky3"] = new(0.7126696f, 0.7003382f, 0.8113207f, 1f),
                    ["_Sky4"] = new(0.8117647f, 0.658834f, 0.4941176f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 1f,
                    ["_AtmoThickness"] = 70f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_Density"] = 0.005f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0.5f,
                    ["_FogDensity"] = 0.4f,
                    ["_FogSaturate"] = 1.2f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_GroundAtmosPower"] = 3f,
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
                    ["_Color"] = new(1f, 0.6134822f, 0.5333333f, 1f),
                    ["_ColorBio0"] = new(1f, 0.6498615f, 0.5613207f, 1f),
                    ["_ColorBio1"] = new(0.8018868f, 0.345823f, 0.2912513f, 1f),
                    ["_ColorBio2"] = new(1f, 0.677347f, 0.6273585f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_RimColor"] = new(1f, 0.6575472f, 0.4292453f, 1f),
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
            thumbMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor"] = new(0.2274797f, 0.001245998f, 0.2641509f, 1f),
                    ["_Color"] = new(1f, 0.6134822f, 0.5333333f, 1f),
                    ["_ColorBio0"] = new(1f, 0.6498615f, 0.5613207f, 1f),
                    ["_ColorBio1"] = new(0.8018868f, 0.345823f, 0.2912512f, 1f),
                    ["_ColorBio2"] = new(1f, 0.677347f, 0.6273585f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(1f, 0.6575472f, 0.4292452f, 1f),
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