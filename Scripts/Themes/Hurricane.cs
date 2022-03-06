using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Hurricane = new()
        {
            Name = "Hurricane",
            Base = true,
            DisplayName = "Hurricane Stone Forest".Translate(),
            PlanetType = EPlanetType.Desert,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 19,
            Algo = 8,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Desert 6/",
            Temperature = 1.0f,
            Distribute = EThemeDistribute.Interstellar,
            ModX = new Vector2(1.5f, 1.5f),
            ModY = new Vector2(-5f, -5f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings //NEED CHANGING on sakura, and other themes imported
            {
                Color1 = new Color(0.1783553f, 0.1903702f, 0.2264151f, 1.0f),
                Color2 = new Color(0.01960784f, 0.0720694f, 0.07843135f, 1.0f),
                Color3 = new Color(0.01960785f, 0.03172881f, 0.05098039f, 1.0f),
                WaterColor1 = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                WaterColor2 = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                WaterColor3 = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                BiomeColor1 = new Color(0.764151f, 0.764151f, 0.764151f, 1.0f),
                BiomeColor2 = new Color(0.259f, 0.09062929f, 0.05076399f, 1.0f),
                BiomeColor3 = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                DustColor1 = new Color(0.9058824f, 0.9038799f, 0.8678353f, 1.0f),
                DustColor2 = new Color(0.573f, 0.3345466f, 0.2882795f, 1.0f),
                DustColor3 = new Color(0.203f, 0.1973611f, 0.1973611f, 1.0f),
                DustStrength1 = 1f,
                DustStrength2 = 8f,
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
                2,
                8,
                2,
                7,
                4,
                1,
                0
            },
            VeinCount = new[]
            {
                1.0f,
                1.0f,
                1.0f,
                1.0f,
                1.0f,
                0.5f,
                0.0f
            },
            VeinOpacity = new[]
            {
                0.6f,
                0.8f,
                1.0f,
                0.8f,
                0.6f,
                0.3f,
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
                0.4f,
                0.3f,
                0.5f
            },
            GasItems = new int[]
            {
            },
            GasSpeeds = new float[]
            {
            },
            UseHeightForBuild = false,
            Wind = 1.6f,
            IonHeight = 70f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new[]
            {
                11,
                4
            },
            SFXPath = "SFX/sfx-amb-desert-3",
            SFXVolume = 0.4f,
            CullingRadius = 0f,
            IceFlag = 0,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new(0.1783552f, 0.1903702f, 0.226415f, 1f),
                    ["_AmbientColor1"] = new(0.01960784f, 0.07206935f, 0.07843129f, 1f),
                    ["_AmbientColor2"] = new(0.01960785f, 0.03172881f, 0.05098036f, 1f),
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightEmissionColor"] = new(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new(0.336107f, 0.7975054f, 0.8584906f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new(0.1599953f, -0.02663486f, 0.9867584f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.5f,
                    ["_BioFuzzMask"] = 1f,
                    ["_BioFuzzStrength"] = 0f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 227.6019f,
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
                Colors = new Dictionary<string, Color>(),
                Params = new Dictionary<string, float>()
            },
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(0.3443396f, 0.734796f, 1f, 1f),
                    ["_Color0"] = new(0f, 0.7931722f, 0.839f, 1f),
                    ["_Color1"] = new(0.4073725f, 0.8313726f, 0.7922341f, 1f),
                    ["_Color2"] = new(0.5727875f, 0.853f, 0.8117923f, 1f),
                    ["_Color3"] = new(0.7686275f, 0.9294118f, 0.7959186f, 1f),
                    ["_Color4"] = new(1f, 0.7532963f, 0.4181377f, 1f),
                    ["_Color5"] = new(0.06345668f, 0.5849056f, 0.4881024f, 1f),
                    ["_Color6"] = new(0f, 0.5033613f, 0.5754716f, 1f),
                    ["_Color7"] = new(0f, 0.5659182f, 0.615f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new(0.8746889f, 0.9137255f, 0.7568628f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(-179.2943f, 38.5314f, -82.56784f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(200f, 100f, 260f, 0f),
                    ["_Sky0"] = new(0.367654f, 0.385f, 0.225995f, 0.4078431f),
                    ["_Sky1"] = new(0.07845306f, 0.4056602f, 0.3857085f, 0.2588235f),
                    ["_Sky2"] = new(0.9169731f, 0.9622641f, 0.7307761f, 0.7490196f),
                    ["_Sky3"] = new(0.5252487f, 0.7026507f, 0.727f, 0.5254902f),
                    ["_Sky4"] = new(1f, 0.750986f, 0.3353673f, 1f)
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
                    ["_FarFogDensity"] = 0.24f,
                    ["_FogDensity"] = 0.63f,
                    ["_FogSaturate"] = 1f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_GroundAtmosPower"] = 3f,
                    ["_Intensity"] = 0.9f,
                    ["_IntensityControl"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_RimFogExp"] = 1.3f,
                    ["_RimFogPower"] = 3f,
                    ["_SkyAtmosPower"] = 9f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_SunColorAdd"] = 0.5f,
                    ["_SunColorSkyUse"] = 0.5f,
                    ["_SunColorUse"] = 0.5f,
                    ["_SunRiseScatterPower"] = 60f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f
                }
            },
            minimapMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(0.8617778f, 0.9150943f, 0.8158152f, 1f),
                    ["_ColorBio0"] = new(0.8962264f, 0.8545086f, 0.8243592f, 1f),
                    ["_ColorBio1"] = new(0.5960785f, 0.09411765f, 0.1139749f, 1f),
                    ["_ColorBio2"] = new(0f, 0f, 0f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_RimColor"] = new(0.03137255f, 1f, 0.4975654f, 1f),
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
                    ["_Color"] = new(0.5f, 0.5f, 0.4316038f, 1f),
                    ["_ColorBio0"] = new(1f, 0.948292f, 0.8726415f, 1f),
                    ["_ColorBio1"] = new(0.6226415f, 0.1171052f, 0.09104663f, 1f),
                    ["_ColorBio2"] = new(0.01886791f, 0.01886791f, 0.01886791f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(0.9433962f, 0.8321868f, 0.6185475f, 1f),
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