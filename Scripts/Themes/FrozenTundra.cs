using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme FrozenTundra = new()
        {
            Name = "FrozenTundra",
            Base = true,
            DisplayName = "Icefrostia".Translate(),
            PlanetType = EPlanetType.Ice,
            ThemeType = EThemeType.Telluric,
            BriefIntroduction = "极寒冻土介绍",
            Eigenbit = 18,
            LDBThemeId = 24,
            Algo = 12,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Desert 10/",
            Temperature = -4f,
            Distribute = EThemeDistribute.Default,
            Habitable = false,
            ModX = new Vector2(1f, 1f),
            ModY = new Vector2(1f, 1f),
            VeinSettings = new GSVeinSettings
            {
                Algorithm = "Vanilla",
                VeinTypes = new GSVeinTypes()
            },
//AmbientSettings 1
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.03399784f, 0.03458045f, 0.03773582f, 1f),
                Color2 = new Color(0.05237614f, 0.0626556f, 0.1037734f, 1f),
                Color3 = new Color(0.1243323f, 0.1385397f, 0.207547f, 1f),
                WaterColor1 = new Color(0f, 0f, 0f, 1f),
                WaterColor2 = new Color(0f, 0f, 0f, 1f),
                WaterColor3 = new Color(0f, 0f, 0f, 1f),
                BiomeColor1 = new Color(0.01668744f, 0.02338745f, 0.02830189f, 1f),
                BiomeColor2 = new Color(0.016f, 0.024f, 0.028f, 1f),
                BiomeColor3 = new Color(1f, 1f, 1f, 1f),
                DustColor1 = new Color(0.139952f, 0.1416541f, 0.1603774f, 1f),
                DustColor2 = new Color(0.42432f, 0.4750933f, 0.51f, 0.8f),
                DustColor3 = new Color(1f, 0.9699292f, 0.9198113f, 0.6f),
                DustStrength1 = 2f,
                DustStrength2 = 5f,
                DustStrength3 = 8f,
                BiomeSound1 = 0,
                BiomeSound2 = 0,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.5f
            },
            Vegetables0 = new[]
            {
                734,
                733,
                732,
                731,
                730,
                729,
                724,
                723,
                722,
                721
            },
            Vegetables1 = new[]
            {
                728,
                727,
                726,
                725
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
                9,
                2,
                2,
                6,
                2,
                1,
                0
            },
            VeinCount = new[]
            {
                0.8f,
                0.5f,
                0.8f,
                1f,
                0.7f,
                0.3f,
                0f
            },
            VeinOpacity = new[]
            {
                0.8f,
                0.8f,
                1.2f,
                1f,
                1f,
                0.3f,
                0f
            },
            RareVeins = new[]
            {
                8,
                9,
                12
            },
            RareSettings = new[]
            {
                0.3f,
                1f,
                0.8f,
                1f,
                0f,
                1f,
                0.7f,
                1f,
                0f,
                0.4f,
                0.5f,
                0.7f
            },
            GasItems = new int[]
            {
            },
            GasSpeeds = new float[]
            {
            },
            UseHeightForBuild = false,
            Wind = 1.3f,
            IonHeight = 55f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new[]
            {
                27,
                4
            },
            SFXPath = "SFX/sfx-amb-desert-1",
            SFXVolume = 0.4f,
            CullingRadius = 0f,
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(0.3443396f, 0.734796f, 1f, 1f),
                    ["_Color0"] = new(0.4509804f, 0.6627451f, 0.6705883f, 1f),
                    ["_Color1"] = new(0.5254902f, 0.7176471f, 0.8196079f, 1f),
                    ["_Color2"] = new(0.5254902f, 0.7176471f, 0.8196079f, 1f),
                    ["_Color3"] = new(0.5647059f, 0.7450981f, 0.9058824f, 1f),
                    ["_Color4"] = new(1f, 0.7520539f, 0.4176908f, 1f),
                    ["_Color5"] = new(0.1161444f, 0.2988604f, 0.5471698f, 1f),
                    ["_Color6"] = new(0.207854f, 0f, 0.6980392f, 1f),
                    ["_Color7"] = new(0.04705869f, 0.1607841f, 0.6039216f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new(0.572549f, 0.7529412f, 0.723219f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(-133.766f, 145.778f, 40.82097f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(200f, 100f, 260f, 0f),
                    ["_Sky0"] = new(0f, 0.5333136f, 0.6320754f, 0.3921569f),
                    ["_Sky1"] = new(0.001868997f, 0.3291016f, 0.3962263f, 0.2588235f),
                    ["_Sky2"] = new(0.4285332f, 0.7334126f, 0.8490566f, 0.7490196f),
                    ["_Sky3"] = new(0.03921569f, 0.2235293f, 0.3411763f, 0.5254902f),
                    ["_Sky4"] = new(1f, 0.7491269f, 0.3329717f, 1f)
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
                    ["_FarFogDensity"] = 0.05f,
                    ["_FogDensity"] = 0.6f,
                    ["_FogSaturate"] = 0.6f,
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
                    ["_RimFogPower"] = 4f,
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
                    ["_Color"] = new(0.1541918f, 0.1608487f, 0.1981131f, 1f),
                    ["_ColorBio0"] = new(0.03671236f, 0.1322978f, 0.2358489f, 1f),
                    ["_ColorBio1"] = new(0.2353595f, 0.3262192f, 0.3867924f, 1f),
                    ["_ColorBio2"] = new(0.8f, 0.8f, 0.8f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_RimColor"] = new(0.4342292f, 0.6761103f, 0.7735849f, 1f),
                    ["_ShoreLineColor"] = new(0f, 0f, 0f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.61f,
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
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new(0.03399784f, 0.03458045f, 0.03773582f, 1f),
                    ["_AmbientColor1"] = new(0.05237616f, 0.06265562f, 0.1037734f, 1f),
                    ["_AmbientColor2"] = new(0.1243323f, 0.1385397f, 0.207547f, 1f),
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_GICloudColor"] = new(1f, 1f, 1f, 1f),
                    ["_HeightEmissionColor"] = new(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new(0.7783019f, 0.907494f, 1f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_SpeclColor"] = new(1f, 1f, 1f, 1f),
                    ["_SunDir"] = new(0.1715005f, -0.07155281f, -0.9825822f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = -0.5f,
                    ["_BioFuzzMask"] = 1f,
                    ["_BioFuzzStrength"] = 0f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 260.3453f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 0f,
                    ["_GICloudStrength"] = 0f,
                    ["_GISaturate"] = 1f,
                    ["_GIStrengthDay"] = 1f,
                    ["_GIStrengthNight"] = 0.2f,
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
            thumbMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor"] = new(0.2274797f, 0.001245998f, 0.2641509f, 1f),
                    ["_Color"] = new(0.1975792f, 0.2087932f, 0.2264151f, 1f),
                    ["_ColorBio0"] = new(0.02069241f, 0.08110091f, 0.1415094f, 1f),
                    ["_ColorBio1"] = new(0.2352941f, 0.3254902f, 0.3882353f, 1f),
                    ["_ColorBio2"] = new(0.982906f, 0.9957265f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(0f, 0.7081624f, 0.8962264f, 1f),
                    ["_Rotation"] = new(-0.005426358f, 0.9974841f, -0.04323564f, 0.05591735f),
                    ["_ShoreLineColor"] = new(0f, 0f, 0f, 0f),
                    ["_SpecularColor"] = new(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new(0.9997593f, 0.003957998f, 0.02157881f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.8f,
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
                    ["_ShoreInvThick"] = 15f,
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
                    ["_WireIntens"] = 2.6f,
                    ["_ZWrite"] = 1f
                }
            }
        };
    }
}