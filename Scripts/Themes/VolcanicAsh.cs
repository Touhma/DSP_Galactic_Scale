using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme VolcanicAsh = new()
        {
            Name = "VolcanicAsh",
            Base = true,
            DisplayName = "Volcanic Ash".Translate(),
            PlanetType = EPlanetType.Vocano,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 13,
            Algo = 3,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Volcanic 1/",
            Temperature = 4.0f,
            Distribute = EThemeDistribute.Interstellar,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(1.0f, 1.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.1137255f, 0.06357255f, 0.09092872f, 1f),
                Color2 = new Color(0.1f, 0.0371f, 0.0470316f, 1f),
                Color3 = new Color(0.04f, 0.024f, 0.03466666f, 1f),
                WaterColor1 = new Color(0f, 0f, 0f, 1f),
                WaterColor2 = new Color(0.08331545f, 0.05882353f, 0.08627451f, 1f),
                WaterColor3 = new Color(0.1132075f, 0.07820842f, 0.04859379f, 1f),
                BiomeColor1 = new Color(0.3370824f, 0.3863237f, 0.454902f, 1f),
                BiomeColor2 = new Color(0.020825f, 0.03173334f, 0.119f, 1f),
                BiomeColor3 = new Color(0.2078431f, 0.01176471f, 0.01362718f, 1f),
                DustColor1 = new Color(0.492678f, 0.5353135f, 0.559f, 1f),
                DustColor2 = new Color(0.2077692f, 0.2134615f, 0.259f, 1f),
                DustColor3 = new Color(1, 0.3581382f, 0.1607843f, 0.1411765f),
                DustStrength1 = 5f,
                DustStrength2 = 1.5f,
                DustStrength3 = 0f,
                BiomeSound1 = 0,
                BiomeSound2 = 2,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.74f
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
                10,
                10,
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
                0.6f,
                1.0f,
                0.6f,
                0.3f,
                0.0f
            },
            VeinOpacity = new[]
            {
                1.0f,
                1.0f,
                0.6f,
                1.0f,
                0.5f,
                0.3f,
                0.0f
            },
            RareVeins = new int[]
            {
            },
            RareSettings = new float[]
            {
            },
            GasItems = new int[]
            {
            },
            GasSpeeds = new float[]
            {
            },
            UseHeightForBuild = false,
            Wind = 0.8f,
            IonHeight = 70f,
            WaterHeight = 0f,
            WaterItemId = 1116,
            Musics = new[]
            {
                10,
                11
            },
            SFXPath = "SFX/sfx-amb-lava-1",
            SFXVolume = 0.38f,
            CullingRadius = 0f,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new(0.09999985f, 0.06499988f, 0.08499984f, 1f),
                    ["_AmbientColor1"] = new(0.07058812f, 0.05098031f, 0.06412378f, 1f),
                    ["_AmbientColor2"] = new(0.07699988f, 0.05543988f, 0.07699988f, 1f),
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightEmissionColor"] = new(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new(0.6718506f, 0.5707102f, 0.7075471f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new(-0.9203354f, -0.0183137f, 0.3907009f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.5f,
                    ["_BioFuzzMask"] = 1f,
                    ["_BioFuzzStrength"] = 0.25f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 235.8372f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 60f,
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
                    ["_StepBlend"] = 0f,
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
                    ["_CausticsColor"] = new(0.3867925f, 0.3267751f, 0.08940012f, 1f),
                    ["_Color"] = new(0.745283f, 0.6000856f, 0.4745906f, 1f),
                    ["_Color0"] = new(0f, 0.1574037f, 0.2352941f, 1f),
                    ["_Color1"] = new(0.7264151f, 0.7264151f, 0.7264151f, 1f),
                    ["_Color2"] = new(0.6763465f, 0.6792453f, 0.6183695f, 1f),
                    ["_Color3"] = new(0.6037736f, 0.523596f, 0.4243503f, 1f),
                    ["_DensityParams"] = new(0.02f, 0.1f, 0f, 0f),
                    ["_DepthColor"] = new(0f, 0.06095791f, 0.1132075f, 1f),
                    ["_DepthFactor"] = new(0.4f, 0.4f, 0.5f, 0.1f),
                    ["_Foam"] = new(15f, 1f, 5f, 1.5f),
                    ["_FoamColor"] = new(0.3925616f, 0.4150942f, 0.2581885f, 1f),
                    ["_FoamParams"] = new(12f, 0.2f, 0.15f, 0.7f),
                    ["_FresnelColor"] = new(0.383585f, 0.2812832f, 0.4056602f, 1f),
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
                    ["_SpeclColor"] = new(0.4100249f, 0.4411012f, 0.497f, 1f),
                    ["_SpeclColor1"] = new(0.5257207f, 0.5331763f, 0.6226414f, 1f),
                    ["_Specular"] = new(0.9573934f, 0.8672858f, 0.5744361f, 0.9573934f),
                    ["_SunDirection"] = new(-0.6f, 0.8f, 0f, 0f),
                    ["_WorldLightDir"] = new(-0.6525278f, -0.6042119f, -0.4573132f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_CausticsTiling"] = 0.06f,
                    ["_DistortionStrength"] = 1f,
                    ["_FoamInvThickness"] = 3f,
                    ["_FoamSpeed"] = 0.1f,
                    ["_FoamSync"] = 20f,
                    ["_NormalSpeed"] = 0.2f,
                    ["_NormalStrength"] = 0.6f,
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
                    ["_RefractionStrength"] = 0.5f,
                    ["_SLCosCutoff1"] = 0.3f,
                    ["_SLIntensity1"] = 1f,
                    ["_SLRange1"] = 10f,
                    ["_Shininess"] = 40f,
                    ["_ShoreIntens"] = 1.5f,
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
                    ["_Color0"] = new(0.1537254f, 0.3754758f, 0.5490196f, 1f),
                    ["_Color1"] = new(0.4807998f, 0.6446f, 0.8f, 1f),
                    ["_Color2"] = new(0.5960784f, 0.7033011f, 0.8470588f, 1f),
                    ["_Color3"] = new(0.6784314f, 0.741826f, 0.8745098f, 1f),
                    ["_Color4"] = new(1f, 0.7506563f, 0.4171883f, 1f),
                    ["_Color5"] = new(0.6509804f, 0.5552863f, 0.642778f, 1f),
                    ["_Color6"] = new(0.4265727f, 0.1803921f, 0.4823529f, 1f),
                    ["_Color7"] = new(0.2075472f, 0.1047525f, 0.1703732f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new(0.75f, 0.75f, 0.75f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(-125.4152f, 79.22004f, 134.4849f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(200f, 100f, 270f, 0f),
                    ["_Sky0"] = new(0.3018867f, 0.2207191f, 0.2207191f, 0.8392157f),
                    ["_Sky1"] = new(0.1897027f, 0.1897027f, 0.2735848f, 0.8862745f),
                    ["_Sky2"] = new(0.9098039f, 0.963978f, 1f, 0.8588235f),
                    ["_Sky3"] = new(0.5372549f, 0.6232351f, 0.7333333f, 0.854902f),
                    ["_Sky4"] = new(1f, 0.7470356f, 0.330277f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 1f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0.177f,
                    ["_FogDensity"] = 1f,
                    ["_FogSaturate"] = 1f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_Intensity"] = 1.2f,
                    ["_IntensityControl"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_RimFogExp"] = 1.4f,
                    ["_RimFogPower"] = 2.5f,
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
                    ["_Color"] = new(0.5449647f, 0.5831854f, 0.6235294f, 1f),
                    ["_ColorBio0"] = new(0.6588517f, 0.6547847f, 0.85f, 1f),
                    ["_ColorBio1"] = new(0.2226199f, 0.09492546f, 0.3568627f, 1f),
                    ["_ColorBio2"] = new(0.37246f, 0.15092f, 0.385f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.05f, 0.05f),
                    ["_RimColor"] = new(0.6745098f, 0.7457068f, 0.8039216f, 1f),
                    ["_ShoreLineColor"] = new(0.490566f, 0.4013408f, 0.2661089f, 0f)
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
                    ["_Color"] = new(0.3993089f, 0.4539512f, 0.5169999f, 1f),
                    ["_ColorBio0"] = new(0.8166662f, 0.796549f, 0.923f, 1f),
                    ["_ColorBio1"] = new(0.2235293f, 0.09411754f, 0.3568627f, 1f),
                    ["_ColorBio2"] = new(0.4117647f, 0.02305882f, 0.32163f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.05f, 0.05f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(0.727f, 0.8771501f, 1f, 1f),
                    ["_Rotation"] = new(0.02728838f, 0.6484647f, 0.06027106f, 0.7583643f),
                    ["_ShoreLineColor"] = new(0.5283019f, 0.4548975f, 0.3463866f, 0f),
                    ["_SpecularColor"] = new(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new(-0.7895794f, -0.01615309f, 0.6134358f, 0f)
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