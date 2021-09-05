using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Sakura = new GSTheme
        {
            Name = "Sakura",
            Base = true,
            DisplayName = "Sakura Ocean".Translate(),
            PlanetType = EPlanetType.Ocean,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 18,
            Algo = 1,
            MinRadius = 25,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Ocean 6/",
            Temperature = 0.0f,
            Habitable = true,
            Distribute = EThemeDistribute.Interstellar,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(0.0f, 0.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            VegeSettings = new GSVegeSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.1264865f, 0.09135135f, 0.13f, 1.0f),
                Color2 = new Color(0.1385069f, 0.09019608f, 0.1411765f, 1.0f),
                Color3 = new Color(0.04666667f, 0.04f, 0.05f, 1.0f),
                WaterColor1 = new Color(0.08076718f, 0.0907268f, 0.1037736f, 1.0f),
                WaterColor2 = new Color(0.06047527f, 0.07024744f, 0.08490568f, 1.0f),
                WaterColor3 = new Color(0.0260324f, 0.03199927f, 0.1415094f, 1.0f),
                BiomeColor1 = new Color(0.573f, 0.460692f, 0.4706016f, 1.0f),
                BiomeColor2 = new Color(0.811f, 0.7858083f, 0.708814f, 1.0f),
                BiomeColor3 = new Color(0.3545426f, 0.3648191f, 0.483f, 1.0f),
                DustColor1 = new Color(0.7169812f, 0.6189036f, 0.6293248f, 1.0f),
                DustColor2 = new Color(0.8489498f, 0.8490566f, 0.8450516f, 1.0f),
                DustColor3 = new Color(0.8489498f, 0.8490566f, 0.8450516f, 1.0f),
                DustStrength1 = 3f,
                DustStrength2 = 3f,
                DustStrength3 = 1f,
                BiomeSound1 = 0,
                BiomeSound2 = 1,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.3f
            },
            Vegetables0 = new[]
            {
                605,
                602,
                133,
                135,
                1006,
                603,
                604,
                135,
                133,
                134
            },
            Vegetables1 = new[]
            {
                135,
                133,
                132,
                134,
                605,
                135,
                1033,
                131,
                1006,
                135
            },
            Vegetables2 = new[]
            {
                1033,
                1031,
                135
            },
            Vegetables3 = new[]
            {
                1005,
                1006,
                1007
            },
            Vegetables4 = new[]
            {
                1031,
                1031,
                1033
            },
            Vegetables5 = new[]
            {
                136
            },
            VeinSpot = new[]
            {
                5,
                6,
                8,
                0,
                4,
                8,
                22
            },
            VeinCount = new[]
            {
                0.6f,
                0.5f,
                0.8f,
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
            Wind = 1.0f,
            IonHeight = 60f,
            WaterHeight = 0f,
            WaterItemId = 1000,
            Musics = new[]
            {
                16,
                9
            },
            SFXPath = "SFX/sfx-amb-ocean-1",
            SFXVolume = 0.52f,
            CullingRadius = 0f,
            IceFlag = 0,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new Color(0.1886792f, 0.1450693f, 0.1455195f, 1f),
                    ["_AmbientColor1"] = new Color(0.1385069f, 0.09019607f, 0.1411764f, 1f),
                    ["_AmbientColor2"] = new Color(0.05334634f, 0.0470588f, 0.0588235f, 1f),
                    ["_Color"] = new Color(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_HeightEmissionColor"] = new Color(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new Color(0f, 0f, 0f, 1f),
                    ["_Rotation"] = new Color(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new Color(-0.04737228f, -0.2017308f, 0.978295f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.5f,
                    ["_BioFuzzMask"] = 1f,
                    ["_BioFuzzStrength"] = 0.37f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 224.6948f,
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
                    ["_StepBlend"] = 0.9f,
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
                    ["_CausticsColor"] = new Color(0.3333333f, 0.9407978f, 1f, 1f),
                    ["_Color"] = new Color(0.8666667f, 0.7803922f, 0.8392452f, 1f),
                    ["_Color0"] = new Color(0f, 0.1574037f, 0.2352941f, 1f),
                    ["_Color1"] = new Color(0.4690909f, 0.549948f, 0.72f, 1f),
                    ["_Color2"] = new Color(0.106647f, 0.4903297f, 0.7254902f, 1f),
                    ["_Color3"] = new Color(0.03420394f, 0.3143821f, 0.6980392f, 1f),
                    ["_DensityParams"] = new Color(0.02f, 0.1f, 0f, 0f),
                    ["_DepthColor"] = new Color(0f, 0.06095791f, 0.1132075f, 1f),
                    ["_DepthFactor"] = new Color(0.43f, 0.8f, 0.1f, 0.07f),
                    ["_Foam"] = new Color(15f, 1f, 5f, 1.5f),
                    ["_FoamColor"] = new Color(1f, 1f, 1f, 1f),
                    ["_FoamParams"] = new Color(12f, 0.2f, 0.15f, 0.7f),
                    ["_FresnelColor"] = new Color(0.5613207f, 0.74045f, 1f, 1f),
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
                    ["_SpeclColor"] = new Color(1f, 1f, 1f, 1f),
                    ["_SpeclColor1"] = new Color(0.8962264f, 0.614061f, 0.1733266f, 1f),
                    ["_Specular"] = new Color(0.9573934f, 0.8672858f, 0.5744361f, 0.9573934f),
                    ["_SunDirection"] = new Color(-0.6f, 0.8f, 0f, 0f),
                    ["_WorldLightDir"] = new Color(-0.6525278f, -0.6042119f, -0.4573132f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_CausticsTiling"] = 0.08f,
                    ["_DistortionStrength"] = 1f,
                    ["_FoamInvThickness"] = 4.5f,
                    ["_FoamSpeed"] = 0.15f,
                    ["_FoamSync"] = 20f,
                    ["_GIGloss"] = 0.6f,
                    ["_GISaturate"] = 1f,
                    ["_GIStrengthDay"] = 1f,
                    ["_GIStrengthNight"] = 0.03f,
                    ["_NormalSpeed"] = 0.6f,
                    ["_NormalStrength"] = 0.2f,
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
                    ["_RefractionStrength"] = 0.3f,
                    ["_SLCosCutoff1"] = 0.3f,
                    ["_SLIntensity1"] = 1f,
                    ["_SLRange1"] = 10f,
                    ["_Shininess"] = 40f,
                    ["_ShoreIntens"] = 0.46f,
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
                    ["_Color0"] = new Color(0.5365804f, 0.4645335f, 0.6886792f, 1f),
                    ["_Color1"] = new Color(0.6398669f, 0.528228f, 0.804f, 1f),
                    ["_Color2"] = new Color(0.738362f, 0.6167f, 0.881f, 1f),
                    ["_Color3"] = new Color(0.8870196f, 0.716822f, 0.986f, 1f),
                    ["_Color4"] = new Color(1f, 0.7549823f, 0.4187441f, 1f),
                    ["_Color5"] = new Color(0.440548f, 0.6862578f, 0.9433962f, 1f),
                    ["_Color6"] = new Color(0.7309523f, 0.407843f, 0.8666667f, 1f),
                    ["_Color7"] = new Color(0.8113207f, 0.3712174f, 0.8060739f, 1f),
                    ["_Color8"] = new Color(1f, 0.6273585f, 0.672164f, 1f),
                    ["_ColorF"] = new Color(0.8641903f, 0.7313545f, 0.8962264f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new Color(0f, 0f, 0f, 0f),
                    ["_PlanetPos"] = new Color(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new Color(200f, 199.98f, 270f, 0f),
                    ["_Sky0"] = new Color(0.5380029f, 0.6413553f, 0.735849f, 0.1607843f),
                    ["_Sky1"] = new Color(0.2014061f, 0.2253748f, 0.5849056f, 0.09803922f),
                    ["_Sky2"] = new Color(0.6859627f, 0.6117647f, 0.9294118f, 0.9019608f),
                    ["_Sky3"] = new Color(0.4509804f, 0.470867f, 0.6705883f, 0.7960784f),
                    ["_Sky4"] = new Color(1f, 0.753509f, 0.3386184f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 0.8f,
                    ["_AtmoThickness"] = 70f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_Density"] = 0.005f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0.1f,
                    ["_FogDensity"] = 0.3f,
                    ["_FogSaturate"] = 0.9f,
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
                    ["_RimFogExp"] = 1f,
                    ["_RimFogPower"] = 2.5f,
                    ["_SkyAtmosPower"] = 7f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_SunColorAdd"] = 0f,
                    ["_SunColorSkyUse"] = 0.5f,
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
                    ["_Color"] = new Color(1f, 0.7028302f, 0.7258278f, 1f),
                    ["_ColorBio0"] = new Color(0.6226414f, 0.4564158f, 0.3906193f, 0f),
                    ["_ColorBio1"] = new Color(1f, 1f, 1f, 0f),
                    ["_ColorBio2"] = new Color(0.5588926f, 0.4943483f, 0.6509434f, 0f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new Color(-1f, 0f, 0.15f, 0.3f),
                    ["_RimColor"] = new Color(1f, 0.4941176f, 0.7603109f, 1f),
                    ["_ShoreLineColor"] = new Color(0.490566f, 0.490566f, 0.490566f, 1f)
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
                    ["_ShoreHeight"] = 0.06f,
                    ["_ShoreInvThick"] = 7f,
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
                    ["_Color"] = new Color(0.9470959f, 0.8487896f, 0.9622642f, 1f),
                    ["_ColorBio0"] = new Color(0.7735849f, 0.5509967f, 0.5509967f, 0f),
                    ["_ColorBio1"] = new Color(1f, 1f, 1f, 0f),
                    ["_ColorBio2"] = new Color(0.2272161f, 0.2624335f, 0.4339623f, 0f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new Color(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new Color(-1f, 0f, 0.1f, 0.3f),
                    ["_HoloColor"] = new Color(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new Color(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new Color(0.5895567f, 0f, 1f, 1f),
                    ["_Rotation"] = new Color(0.0317241f, -0.8589735f, 0.08102829f, 0.5045717f),
                    ["_ShoreLineColor"] = new Color(0.8430492f, 0.6263794f, 0.8679245f, 1f),
                    ["_SpecularColor"] = new Color(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new Color(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new Color(0.5150122f, 0.006351451f, 0.8571593f, 0f)
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
                    ["_ShoreInvThick"] = 2f,
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
                    ["_WireIntens"] = 1.07f,
                    ["_ZWrite"] = 1f
                }
            }
        };
    }
}