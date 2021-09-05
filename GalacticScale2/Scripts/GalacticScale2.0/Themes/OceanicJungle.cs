using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme OceanicJungle = new GSTheme
        {
            Name = "OceanicJungle",
            Base = true,
            DisplayName = "Oceanic Jungle".Translate(),
            PlanetType = EPlanetType.Ocean,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 8,
            Algo = 1,
            MinRadius = 30,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Ocean 2/",
            Temperature = 0.0f,
            Habitable = true,
            Distribute = EThemeDistribute.Interstellar,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(0.0f, 0.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.05673727f, 0.1415094f, 0.1034037f, 1f),
                Color2 = new Color(0.07008722f, 0.1549896f, 0.2358491f, 1f),
                Color3 = new Color(0.042f, 0.07f, 0.063f, 1f),
                WaterColor1 = new Color(0, 0, 0, 1f),
                WaterColor2 = new Color(0.06047527f, 0.07024744f, 0.08490568f, 1f),
                WaterColor3 = new Color(0.0260324f, 0.03199927f, 0.1415094f, 1f),
                BiomeColor1 = new Color(0.09887861f, 0.2075472f, 0.1230272f, 1f),
                BiomeColor2 = new Color(0.1048f, 0.2f, 0.1192243f, 1f),
                BiomeColor3 = new Color(0.08490568f, 0.05762628f, 0.04045035f, 1f),
                DustColor1 = new Color(0.16506f, 0.315f, 0.1790079f, 0.6980392f),
                DustColor2 = new Color(0.4336911f, 0.5254902f, 0.3709961f, 0.6705883f),
                DustColor3 = new Color(0.35f, 0.2821428f, 0.2464286f, 0.854902f),
                DustStrength1 = 1f,
                DustStrength2 = 3f,
                DustStrength3 = 3f,
                BiomeSound1 = 0,
                BiomeSound2 = 1,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.5f
            },
            Vegetables0 = new[]
            {
                1023,
                124,
                603,
                126,
                121,
                605
            },
            Vegetables1 = new[]
            {
                605,
                121,
                122,
                125,
                1021,
                604,
                603,
                126
            },
            Vegetables2 = new[]
            {
                1023
            },
            Vegetables3 = new[]
            {
                1023,
                1021,
                1006
            },
            Vegetables4 = new[]
            {
                1023,
                1021
            },
            Vegetables5 = new[]
            {
                1022
            },
            VeinSpot = new[]
            {
                7,
                2,
                12,
                0,
                4,
                10,
                22
            },
            VeinCount = new[]
            {
                0.6f,
                0.3f,
                0.9f,
                0.0f,
                0.8f,
                1.0f,
                1.0f
            },
            VeinOpacity = new[]
            {
                0.6f,
                0.6f,
                0.6f,
                0.0f,
                0.5f,
                1.0f,
                1.0f
            },
            RareVeins = new[]
            {
                11,
                13
            },
            RareSettings = new[]
            {
                0.0f,
                1.0f,
                0.3f,
                1.0f,
                0.0f,
                0.5f,
                0.2f,
                0.8f
            },
            GasItems = new int[]
            {
            },
            GasSpeeds = new float[]
            {
            },
            UseHeightForBuild = false,
            Wind = 1f,
            IonHeight = 60f,
            WaterHeight = 0f,
            WaterItemId = 1000,
            Musics = new[]
            {
                9
            },
            SFXPath = "SFX/sfx-amb-ocean-1",
            SFXVolume = 0.52f,
            CullingRadius = 0f,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new Color(0.05673727f, 0.1415094f, 0.1034037f, 1f),
                    ["_AmbientColor1"] = new Color(0.07008722f, 0.1549896f, 0.2358491f, 1f),
                    ["_AmbientColor2"] = new Color(0.055f, 0.09f, 0.06f, 1f),
                    ["_Color"] = new Color(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_HeightEmissionColor"] = new Color(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new Color(0f, 0f, 0f, 1f),
                    ["_Rotation"] = new Color(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new Color(0f, 1f, 0f, 0f)
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
                    ["_EmissionStrength"] = 50f,
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
                    ["_StepBlend"] = 1f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f
                }
            },
            oceanMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_BumpDirection"] = new Color(1f, 1f, -1f, 1f),
                    ["_BumpTiling"] = new Color(1f, 1f, -2f, 3f),
                    ["_CausticsColor"] = new Color(0f, 0.05572889f, 0.3018867f, 1f),
                    ["_Color"] = new Color(0.5896226f, 0.856845f, 1f, 1f),
                    ["_Color0"] = new Color(0f, 0.1574037f, 0.2352941f, 1f),
                    ["_Color1"] = new Color(0.1264941f, 0.484545f, 0.5647059f, 1f),
                    ["_Color2"] = new Color(0f, 0.3416627f, 0.462745f, 1f),
                    ["_Color3"] = new Color(0.1041182f, 0.3349892f, 0.4209999f, 1f),
                    ["_DensityParams"] = new Color(0.02f, 0.1f, 0f, 0f),
                    ["_DepthColor"] = new Color(0f, 0.06095791f, 0.1132075f, 1f),
                    ["_DepthFactor"] = new Color(0.6f, 0.55f, 0.7f, 0.1f),
                    ["_Foam"] = new Color(15f, 1f, 5f, 1.5f),
                    ["_FoamColor"] = new Color(0.5896226f, 0.9405546f, 1f, 1f),
                    ["_FoamParams"] = new Color(12f, 0.2f, 0.15f, 0.7f),
                    ["_FresnelColor"] = new Color(0.5299608f, 0.9137255f, 0.843032f, 1f),
                    ["_InvFadeParemeter"] = new Color(0.9f, 0.25f, 0.5f, 0.08f),
                    ["_PLColor1"] = new Color(0f, 0f, 0f, 1f),
                    ["_PLColor2"] = new Color(0f, 0f, 0f, 1f),
                    ["_PLColor3"] = new Color(1f, 1f, 1f, 1f),
                    ["_PLParam1"] = new Color(0f, 0f, 0f, 0f),
                    ["_PLParam2"] = new Color(0f, 0f, 0f, 0f),
                    ["_PLPos1"] = new Color(0f, 0f, 0f, 0f),
                    ["_PLPos2"] = new Color(0f, 0f, 0f, 0f),
                    ["_PLPos3"] = new Color(0f, 0.1f, -0.5f, 0f),
                    ["_Paremeters1"] = new Color(0.02f, 0.1f, 0f, 0f),
                    ["_PointAtten"] = new Color(0f, 0.1f, -0.5f, 0f),
                    ["_PointLightPos"] = new Color(0f, 0.1f, -0.5f, 0f),
                    ["_ReflectionColor"] = new Color(0.1933962f, 0.5064065f, 1f, 1f),
                    ["_SLColor1"] = new Color(1f, 1f, 1f, 1f),
                    ["_SLDir1"] = new Color(0f, 0.1f, -0.5f, 0f),
                    ["_SLPos1"] = new Color(0f, 0.1f, -0.5f, 0f),
                    ["_SpecColor"] = new Color(1f, 1f, 1f, 1f),
                    ["_SpeclColor"] = new Color(0.7215686f, 1f, 0.7590522f, 1f),
                    ["_SpeclColor1"] = new Color(0.9100575f, 1f, 0.7877358f, 1f),
                    ["_Specular"] = new Color(0.9573934f, 0.8672858f, 0.5744361f, 0.9573934f),
                    ["_SunDirection"] = new Color(-0.6f, 0.8f, 0f, 0f),
                    ["_WorldLightDir"] = new Color(-0.6525278f, -0.6042119f, -0.4573132f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_CausticsTiling"] = 0.03f,
                    ["_DistortionStrength"] = 1f,
                    ["_FoamInvThickness"] = 2f,
                    ["_FoamSpeed"] = 0.05f,
                    ["_FoamSync"] = 5f,
                    ["_NormalSpeed"] = 0.5f,
                    ["_NormalStrength"] = 0.41f,
                    ["_NormalTiling"] = 0.16f,
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
                    ["_RefractionStrength"] = 0.5f,
                    ["_SLCosCutoff1"] = 0.3f,
                    ["_SLIntensity1"] = 1f,
                    ["_SLRange1"] = 10f,
                    ["_Shininess"] = 40f,
                    ["_ShoreIntens"] = 1.5f,
                    ["_SpeclColorDayStrength"] = 0f,
                    ["_SpotExp"] = 2f,
                    ["_Tile"] = 0.05f
                }
            },
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new Color(0.3443396f, 0.734796f, 1f, 1f),
                    ["_Color0"] = new Color(0.4470588f, 0.8674654f, 1f, 1f),
                    ["_Color1"] = new Color(0.3921568f, 0.8644661f, 1f, 1f),
                    ["_Color2"] = new Color(0.294f, 0.8454528f, 1f, 1f),
                    ["_Color3"] = new Color(0.1764705f, 0.8974488f, 1f, 1f),
                    ["_Color4"] = new Color(1f, 0.7464952f, 0.4156916f, 1f),
                    ["_Color5"] = new Color(0.4470588f, 0.7491848f, 1f, 1f),
                    ["_Color6"] = new Color(0.6622748f, 0.2313725f, 1f, 1f),
                    ["_Color7"] = new Color(0.1137254f, 0.3749631f, 0.6117647f, 1f),
                    ["_Color8"] = new Color(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new Color(0.8184583f, 0.6462264f, 1f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new Color(187.6145f, 5.778888f, 70.39255f, 0f),
                    ["_PlanetPos"] = new Color(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new Color(200f, 199.98f, 270f, 0f),
                    ["_Sky0"] = new Color(0.1273583f, 0.4988209f, 1f, 0.1607843f),
                    ["_Sky1"] = new Color(0.4862745f, 0.7558302f, 1f, 0.09803922f),
                    ["_Sky2"] = new Color(0.610606f, 0.93f, 0.8595455f, 0.9019608f),
                    ["_Sky3"] = new Color(0.5921568f, 0.9113778f, 0.9921568f, 0.7960784f),
                    ["_Sky4"] = new Color(1f, 0.740809f, 0.3222534f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 0.9f,
                    ["_AtmoThickness"] = 70f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_Density"] = 0.005f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0.09f,
                    ["_FogDensity"] = 0.8f,
                    ["_FogSaturate"] = 1.1f,
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
                    ["_RimFogPower"] = 2.5f,
                    ["_SkyAtmosPower"] = 7f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_SunColorAdd"] = 0f,
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
                    ["_Color"] = new Color(0.3647059f, 1f, 0.9423965f, 1f),
                    ["_ColorBio0"] = new Color(0f, 0f, 0f, 0f),
                    ["_ColorBio1"] = new Color(0f, 0f, 0f, 0f),
                    ["_ColorBio2"] = new Color(0f, 0f, 0f, 0f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new Color(-1f, 0f, 0.1f, 0.3f),
                    ["_RimColor"] = new Color(0.943431f, 1f, 0.5333333f, 1f),
                    ["_ShoreLineColor"] = new Color(0.2672658f, 0.3639862f, 0.3962263f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0f,
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
                    ["_ShoreInvThick"] = 4f,
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
                    ["_AmbientColor"] = new Color(0f, 0.01454774f, 0.3113208f, 1f),
                    ["_Color"] = new Color(0.3647059f, 1f, 0.9423965f, 1f),
                    ["_ColorBio0"] = new Color(0f, 0f, 0f, 0f),
                    ["_ColorBio1"] = new Color(0f, 0f, 0f, 0f),
                    ["_ColorBio2"] = new Color(0f, 0f, 0f, 0f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new Color(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new Color(-1f, 0f, 0.1f, 0.3f),
                    ["_HoloColor"] = new Color(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new Color(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new Color(0.943431f, 1f, 0.5333333f, 1f),
                    ["_Rotation"] = new Color(0f, 0f, 0f, 1f),
                    ["_ShoreLineColor"] = new Color(0.2672658f, 0.3639862f, 0.3962263f, 1f),
                    ["_SpecularColor"] = new Color(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new Color(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new Color(0f, 0f, 1f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0f,
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
                    ["_ShoreInvThick"] = 4f,
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