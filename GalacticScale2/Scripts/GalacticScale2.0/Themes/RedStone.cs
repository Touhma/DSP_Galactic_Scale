using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme RedStone = new GSTheme
        {
            Name = "RedStone",
            Base = true,
            DisplayName = "Red Stone".Translate(),
            PlanetType = EPlanetType.Ocean,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 14,
            Algo = 1,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Ocean 3/",
            Temperature = 0.0f,
            Habitable = true,
            Distribute = EThemeDistribute.Interstellar,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(0.0f, 0.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "GS2" },
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.1333333f, 0.07843138f, 0.08235294f, 1f),
                Color2 = new Color(0.1320755f, 0.06666667f, 0.03177287f, 1f),
                Color3 = new Color(0.06f, 0.0258f, 0.05229965f, 1f),
                WaterColor1 = new Color(0f, 0f, 0f, 1f),
                WaterColor2 = new Color(0.03292801f, 0.196f, 0.130144f, 1f),
                WaterColor3 = new Color(0.05826098f, 0.06201299f, 0.203f, 1f),
                BiomeColor1 = new Color(0.3568628f, 0.1254902f, 0.1058824f, 1f),
                BiomeColor2 = new Color(0.5647059f, 0.2745098f, 0.145098f, 1f),
                BiomeColor3 = new Color(0.3098039f, 0.1058824f, 0.1058824f, 1f),
                DustColor1 = new Color(0.769f, 0.5454975f, 0.5871674f, 0.6f),
                DustColor2 = new Color(0.9607843f, 0.7825242f, 0.6783137f, 1f),
                DustColor3 = new Color(0.7725491f, 0.5215687f, 0.4627451f, 0.6352941f),
                DustStrength1 = 5f,
                DustStrength2 = 5f,
                DustStrength3 = 2f,
                BiomeSound1 = 0,
                BiomeSound2 = 2,
                BiomeSound3 = 3,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 1f
            },
            Vegetables0 = new[]
            {
                604,
                114,
                603,
                605,
                116,
                113,
                115,
                112,
                111
            },
            Vegetables1 = new[]
            {
                114,
                604,
                1011,
                114,
                116,
                1011,
                113,
                605,
                112,
                115,
                111
            },
            Vegetables2 = new[]
            {
                1012
            },
            Vegetables3 = new[]
            {
                1012,
                1013
            },
            Vegetables4 = new[]
            {
                1013
            },
            Vegetables5 = new int[]
            {
            },
            VeinSpot = new[]
            {
                4,
                6,
                0,
                0,
                10,
                8,
                12
            },
            VeinCount = new[]
            {
                0.7f,
                0.7f,
                0.0f,
                0.0f,
                1.0f,
                1.0f,
                1.0f
            },
            VeinOpacity = new[]
            {
                0.5f,
                0.6f,
                0.0f,
                0.0f,
                0.8f,
                1.0f,
                1.0f
            },
            RareVeins = new[]
            {
                9,
                11,
                13
            },
            RareSettings = new[]
            {
                0.0f,
                0.4f,
                0.3f,
                0.5f,
                0.0f,
                1.0f,
                0.3f,
                0.8f,
                0.0f,
                0.5f,
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
            Wind = 1f,
            IonHeight = 60f,
            WaterHeight = 0f,
            WaterItemId = 1000,
            Musics = new[]
            {
                9
            },
            SFXPath = "SFX/sfx-amb-ocean-1",
            SFXVolume = 0.53f,
            CullingRadius = 0f,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new Color(0.1333332f, 0.06533324f, 0.09157962f, 1f),
                    ["_AmbientColor1"] = new Color(0.1320753f, 0.09728687f, 0.03177287f, 1f),
                    ["_AmbientColor2"] = new Color(0.05142856f, 0.03214287f, 0.09f, 1f),
                    ["_Color"] = new Color(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_HeightEmissionColor"] = new Color(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new Color(0.8396226f, 0.4079297f, 0.7331161f, 1f),
                    ["_Rotation"] = new Color(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new Color(-0.9664816f, -0.2524303f, -0.04682636f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.9f,
                    ["_BioFuzzMask"] = 1f,
                    ["_BioFuzzStrength"] = 0f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 291.26f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 50f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_HeightEmissionRadius"] = 50f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 1.8f,
                    ["_NormalStrength"] = 0.8f,
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
                    ["_CausticsColor"] = new Color(1f, 0.8940002f, 0.485849f, 1f),
                    ["_Color"] = new Color(1f, 0.6839622f, 0.7887316f, 1f),
                    ["_Color0"] = new Color(0f, 0.1574037f, 0.2352941f, 1f),
                    ["_Color1"] = new Color(0.420624f, 0.7619999f, 0.7586573f, 1f),
                    ["_Color2"] = new Color(0f, 0.5289927f, 0.643f, 1f),
                    ["_Color3"] = new Color(0.2309999f, 0.6222066f, 0.7333333f, 1f),
                    ["_DensityParams"] = new Color(0.02f, 0.1f, 0f, 0f),
                    ["_DepthColor"] = new Color(0f, 0.06095791f, 0.1132075f, 1f),
                    ["_DepthFactor"] = new Color(0.4f, 1.2f, 0.35f, 0.1f),
                    ["_Foam"] = new Color(15f, 1f, 5f, 1.5f),
                    ["_FoamColor"] = new Color(1f, 1f, 1f, 1f),
                    ["_FoamParams"] = new Color(12f, 0.2f, 0.15f, 0.7f),
                    ["_FresnelColor"] = new Color(0.7620355f, 0.6839622f, 1f, 1f),
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
                    ["_SpeclColor"] = new Color(0.8584906f, 0.8584906f, 0.8584906f, 1f),
                    ["_SpeclColor1"] = new Color(1f, 0.6678602f, 0.572549f, 1f),
                    ["_Specular"] = new Color(0.9573934f, 0.8672858f, 0.5744361f, 0.9573934f),
                    ["_SunDirection"] = new Color(-0.6f, 0.8f, 0f, 0f),
                    ["_WorldLightDir"] = new Color(-0.6525278f, -0.6042119f, -0.4573132f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_CausticsTiling"] = 0.06f,
                    ["_DistortionStrength"] = 1f,
                    ["_FoamInvThickness"] = 3f,
                    ["_FoamSpeed"] = 0.15f,
                    ["_FoamSync"] = 15f,
                    ["_GIGloss"] = 0.6f,
                    ["_GISaturate"] = 1f,
                    ["_GIStrengthDay"] = 1f,
                    ["_GIStrengthNight"] = 0.03f,
                    ["_NormalSpeed"] = 0.5f,
                    ["_NormalStrength"] = 0.12f,
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
                    ["_ShoreIntens"] = 0.4f,
                    ["_SpeclColorDayStrength"] = 0.2f,
                    ["_SpotExp"] = 2f,
                    ["_Tile"] = 0.05f
                }
            },
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new Color(0.3443396f, 0.734796f, 1f, 1f),
                    ["_Color0"] = new Color(0.2660198f, 0.3085042f, 0.9245283f, 1f),
                    ["_Color1"] = new Color(0.4280078f, 0.6013333f, 0.902f, 1f),
                    ["_Color2"] = new Color(0.4421568f, 0.6579294f, 0.902f, 1f),
                    ["_Color3"] = new Color(0.7678006f, 0.7216981f, 1f, 1f),
                    ["_Color4"] = new Color(1f, 0.5910653f, 0.3921568f, 1f),
                    ["_Color5"] = new Color(0.2405658f, 0.5836905f, 1f, 1f),
                    ["_Color6"] = new Color(0.3985406f, 0.3101636f, 0.7735849f, 1f),
                    ["_Color7"] = new Color(0.1128068f, 0.2254752f, 0.6132076f, 1f),
                    ["_Color8"] = new Color(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new Color(0.7490196f, 0.6022118f, 0.6022118f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new Color(-61.91296f, 50.18437f, 183.6744f, 0f),
                    ["_PlanetPos"] = new Color(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new Color(200f, 199.98f, 270f, 0f),
                    ["_Sky0"] = new Color(0.7221571f, 0.572549f, 1f, 0.1607843f),
                    ["_Sky1"] = new Color(0.5206776f, 0.4862745f, 1f, 0.09803922f),
                    ["_Sky2"] = new Color(1f, 0.7803922f, 0.9201165f, 0.7490196f),
                    ["_Sky3"] = new Color(0.3450977f, 0.7052943f, 1f, 0.5411765f),
                    ["_Sky4"] = new Color(1f, 0.6463005f, 0.1979999f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 1.2f,
                    ["_AtmoThickness"] = 70f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_Density"] = 0.005f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0.12f,
                    ["_FogDensity"] = 0.9f,
                    ["_FogSaturate"] = 1.3f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_GroundAtmosPower"] = 3f,
                    ["_Intensity"] = 1.15f,
                    ["_IntensityControl"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_RimFogExp"] = 1.3f,
                    ["_RimFogPower"] = 3.2f,
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
                    ["_Color"] = new Color(0.9254902f, 0.5182047f, 0.5098039f, 1f),
                    ["_ColorBio0"] = new Color(0f, 0f, 0f, 1f),
                    ["_ColorBio1"] = new Color(0f, 0f, 0f, 1f),
                    ["_ColorBio2"] = new Color(0f, 0f, 0f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new Color(-1f, 0f, 0.1f, 0.3f),
                    ["_RimColor"] = new Color(1f, 0.657f, 0.9876023f, 1f),
                    ["_ShoreLineColor"] = new Color(1f, 0.500879f, 0.259434f, 0f)
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
                    ["_AmbientColor"] = new Color(0f, 0.01454774f, 0.3113208f, 1f),
                    ["_Color"] = new Color(0.916f, 0.5586367f, 0.5504215f, 1f),
                    ["_ColorBio0"] = new Color(0f, 0f, 0f, 0f),
                    ["_ColorBio1"] = new Color(0f, 0f, 0f, 1f),
                    ["_ColorBio2"] = new Color(0f, 0f, 0f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new Color(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new Color(-1f, 0f, 0.1f, 0.3f),
                    ["_HoloColor"] = new Color(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new Color(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new Color(1f, 0.6084906f, 0.7648496f, 1f),
                    ["_Rotation"] = new Color(0.1348138f, -0.9674751f, 0.07657813f, -0.1998829f),
                    ["_ShoreLineColor"] = new Color(0.6784314f, 0.4612247f, 0.1662157f, 1f),
                    ["_SpecularColor"] = new Color(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new Color(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new Color(0.9062576f, 0.07863805f, 0.4153472f, 0f)
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