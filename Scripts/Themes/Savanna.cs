using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Savanna = new()
        {
            Name = "Savanna",
            Base = true,
            DisplayName = "Savanna".Translate(),
            PlanetType = EPlanetType.Ocean,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 22,
            Algo = 10,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Desert 8/",
            Temperature = 0f,
            Distribute = EThemeDistribute.Interstellar,
            Habitable = true,
            ModX = new Vector2(1f, 1f),
            ModY = new Vector2(1f, 1f),
            CustomGeneration = false,
            TerrainSettings = new GSTerrainSettings
            {
                Algorithm = "Vanilla"
            },
            VeinSettings = new GSVeinSettings
            {
                Algorithm = "Vanilla",
                VeinTypes = new GSVeinTypes()
            },
//AmbientSettings 1
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.02357377f, 0.0588235f, 0.01605882f, 1f),
                Color2 = new Color(0.07058821f, 0.04912057f, 0.01334118f, 1f),
                Color3 = new Color(0.018473f, 0.0339699f, 0.09099997f, 1f),
                WaterColor1 = new Color(0f, 0f, 0f, 1f),
                WaterColor2 = new Color(0.1176471f, 0.09162502f, 0.06666667f, 1f),
                WaterColor3 = new Color(0.07466667f, 0.098f, 0.014f, 1f),
                BiomeColor1 = new Color(0.4544778f, 0.5754717f, 0.116723f, 1f),
                BiomeColor2 = new Color(0.4245283f, 0.3279137f, 0.1982467f, 1f),
                BiomeColor3 = new Color(0.3207547f, 0.2186015f, 0.1860982f, 1f),
                DustColor1 = new Color(0.6792453f, 0.618589f, 0.4069064f, 0.5019608f),
                DustColor2 = new Color(1f, 0.8619885f, 0.5613208f, 0.5019608f),
                DustColor3 = new Color(0.804f, 0.6252589f, 0.60702f, 0.8f),
                DustStrength1 = 1f,
                DustStrength2 = 7f,
                DustStrength3 = 7f,
                BiomeSound1 = 0,
                BiomeSound2 = 0,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.3f
            },
            Vegetables0 = new[]
            {
                1093,
                1093,
                1092,
                164,
                165,
                163,
                166,
                1094
            },
            Vegetables1 = new[]
            {
                692,
                693,
                691,
                694,
                167,
                168,
                161,
                162,
                169,
                170
            },
            Vegetables2 = new[]
            {
                1094,
                166,
                161,
                693,
                694
            },
            Vegetables3 = new[]
            {
                1093,
                1092,
                1091,
                1094
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
                4,
                7,
                2,
                3,
                6,
                14
            },
            VeinCount = new[]
            {
                0.7f,
                0.6f,
                1f,
                0.8f,
                0.7f,
                1f,
                1f
            },
            VeinOpacity = new[]
            {
                0.7f,
                0.6f,
                0.8f,
                0.7f,
                1f,
                1.2f,
                1f
            },
            RareVeins = new[]
            {
                11,
                13
            },
            RareSettings = new[]
            {
                0f,
                1f,
                0.5f,
                1f,
                0f,
                0.6f,
                0.25f,
                1f
            },
            GasItems = new int[]
            {
            },
            GasSpeeds = new float[]
            {
            },
            UseHeightForBuild = false,
            Wind = 1.1f,
            IonHeight = 60f,
            WaterHeight = -0.7f,
            WaterItemId = 1000,
            Musics = new[]
            {
                25,
                16
            },
            SFXPath = "SFX/sfx-amb-ocean-1",
            SFXVolume = 0.35f,
            CullingRadius = 0f,
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(0.3443396f, 0.734796f, 1f, 1f),
                    ["_Color0"] = new(0f, 0.2156333f, 0.4528298f, 1f),
                    ["_Color1"] = new(0.343494f, 0.6035818f, 0.8773585f, 1f),
                    ["_Color2"] = new(0.5235848f, 0.7776729f, 1f, 1f),
                    ["_Color3"] = new(0.7216981f, 0.9431427f, 1f, 1f),
                    ["_Color4"] = new(1f, 0.6908621f, 0.3956822f, 1f),
                    ["_Color5"] = new(0f, 0.6226414f, 0.5274139f, 1f),
                    ["_Color6"] = new(0.2196076f, 0.4207777f, 0.6235294f, 1f),
                    ["_Color7"] = new(0.1984245f, 0.6615355f, 0.8584906f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new(0.739854f, 0.8955411f, 0.9622641f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(200f, 100f, 260f, 0f),
                    ["_Sky0"] = new(0.5007563f, 0.7467627f, 0.8773585f, 0.6705883f),
                    ["_Sky1"] = new(0.3406014f, 0.3757178f, 0.8113207f, 0.427451f),
                    ["_Sky2"] = new(0.5411765f, 0.9579551f, 1f, 0.8862745f),
                    ["_Sky3"] = new(0.1785331f, 0.4731544f, 0.6415094f, 0.7490196f),
                    ["_Sky4"] = new(1f, 0.6575589f, 0.2149789f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 0.6f,
                    ["_AtmoThickness"] = 70f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_Density"] = 0.005f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0.02f,
                    ["_FogDensity"] = 0.3f,
                    ["_FogSaturate"] = 0.5f,
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
                    ["_SkyAtmosPower"] = 7f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_StencilComp"] = 8f,
                    ["_StencilRef"] = 0f,
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
                    ["_Color"] = new(0f, 0f, 0f, 1f),
                    ["_ColorBio0"] = new(0.6805413f, 0.7130001f, 0.2217256f, 1f),
                    ["_ColorBio1"] = new(0.944f, 0.7848157f, 0.4664469f, 1f),
                    ["_ColorBio2"] = new(0.79f, 0.6047847f, 0.355311f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_RimColor"] = new(0.5943396f, 0.417306f, 0f, 1f),
                    ["_ShoreLineColor"] = new(0.5660378f, 0.4281088f, 0.3818085f, 0f)
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
                    ["_ShoreHeight"] = -0.4f,
                    ["_ShoreInvThick"] = 2f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_UVSec"] = 0f,
                    ["_WireIntens"] = 1f,
                    ["_ZWrite"] = 1f
                }
            },
            oceanMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_BumpDirection"] = new(1f, 1f, -1f, 1f),
                    ["_BumpTiling"] = new(1f, 1f, -2f, 3f),
                    ["_CausticsColor"] = new(0.7830189f, 0.5542746f, 0.380429f, 1f),
                    ["_Color"] = new(0.9056604f, 0.8524331f, 0.4827341f, 1f),
                    ["_Color0"] = new(0f, 0.1574037f, 0.2352941f, 1f),
                    ["_Color1"] = new(0.4932656f, 0.51f, 0.2413393f, 1f),
                    ["_Color2"] = new(0.123312f, 0.734f, 0.2961482f, 1f),
                    ["_Color3"] = new(0.4771897f, 0.509804f, 0.2431373f, 1f),
                    ["_DensityParams"] = new(0.02f, 0.1f, 0f, 0f),
                    ["_DepthColor"] = new(0f, 0.06095791f, 0.1132075f, 1f),
                    ["_DepthFactor"] = new(0.6f, 0.8f, 0.4f, 0.04f),
                    ["_Foam"] = new(15f, 1f, 5f, 1.5f),
                    ["_FoamColor"] = new(0.531f, 0.4946684f, 0.4024421f, 1f),
                    ["_FoamParams"] = new(12f, 0.2f, 0.15f, 0.7f),
                    ["_FresnelColor"] = new(0.5903841f, 0.8207547f, 0.0967871f, 1f),
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
                    ["_SpeclColor1"] = new(0.8962264f, 0.614061f, 0.1733268f, 1f),
                    ["_Specular"] = new(0.9573934f, 0.8672858f, 0.5744361f, 0.9573934f),
                    ["_SunDirection"] = new(-0.6f, 0.8f, 0f, 0f),
                    ["_WorldLightDir"] = new(-0.6525278f, -0.6042119f, -0.4573132f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_CausticsTiling"] = 0.15f,
                    ["_DistortionStrength"] = 1f,
                    ["_FoamInvThickness"] = 8f,
                    ["_FoamSpeed"] = 0.15f,
                    ["_FoamSync"] = 10f,
                    ["_GIGloss"] = 0.409f,
                    ["_GISaturate"] = 0.567f,
                    ["_GIStrengthDay"] = 0.623f,
                    ["_GIStrengthNight"] = 0.05f,
                    ["_NormalSpeed"] = 0.5f,
                    ["_NormalStrength"] = 0.15f,
                    ["_NormalTiling"] = 0.2f,
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
                    ["_RefractionStrength"] = 0.13f,
                    ["_SLCosCutoff1"] = 0.3f,
                    ["_SLIntensity1"] = 1f,
                    ["_SLRange1"] = 10f,
                    ["_Shininess"] = 40f,
                    ["_ShoreIntens"] = 1f,
                    ["_SpeclColorDayStrength"] = 0f,
                    ["_SpotExp"] = 2f,
                    ["_Tile"] = 0.05f
                }
            },
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new(0.024f, 0.06f, 0.016f, 1f),
                    ["_AmbientColor1"] = new(0.07058821f, 0.04912057f, 0.01334118f, 1f),
                    ["_AmbientColor2"] = new(0.018473f, 0.0339699f, 0.09099997f, 1f),
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightEmissionColor"] = new(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new(0.336107f, 0.7975054f, 0.8584906f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new(-0.7156197f, -0.004051476f, 0.6984785f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.5f,
                    ["_BioFuzzMask"] = 1f,
                    ["_BioFuzzStrength"] = 0.2f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 203.8399f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_HeightEmissionRadius"] = 50f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 1.8f,
                    ["_NormalStrength"] = 1f,
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
                    ["_Color"] = new(0.4225828f, 0.5490196f, 0.3686275f, 1f),
                    ["_ColorBio0"] = new(0.6584412f, 0.734f, 0f, 1f),
                    ["_ColorBio1"] = new(0.93f, 0.744f, 0.313647f, 1f),
                    ["_ColorBio2"] = new(0.797f, 0.4946207f, 0.295687f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(0f, 0.08797646f, 0.7735849f, 1f),
                    ["_Rotation"] = new(-0.05411213f, 0.256475f, 0.1943572f, -0.9452607f),
                    ["_ShoreLineColor"] = new(0f, 0f, 0f, 0f),
                    ["_SpecularColor"] = new(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new(-0.6933538f, -0.07187035f, 0.7170043f, 0f)
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
                    ["_WireIntens"] = 2f,
                    ["_ZWrite"] = 1f
                }
            }
        };
    }
}