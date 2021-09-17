using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Lava = new GSTheme
        {
            Name = "Lava",
            Base = true,
            DisplayName = "Lava".Translate(),
            PlanetType = EPlanetType.Vocano,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 9,
            Algo = 5,
            MinRadius = 30,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Lava 1/",
            Temperature = 5.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(0.0f, 0.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "GS2" },
            TerrainSettings = new GSTerrainSettings { BrightnessFix = true },
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.2588235f, 0.05098039f, 0.02352941f, 1f),
                Color2 = new Color(0.145098f, 0.04313726f, 0.04313726f, 1f),
                Color3 = new Color(0.1490196f, 0.03921569f, 0.03529412f, 1f),
                WaterColor1 = new Color(0f, 0f, 0f, 1f),
                WaterColor2 = new Color(0.06047527f, 0.07024744f, 0.08490568f, 1f),
                WaterColor3 = new Color(0.0260324f, 0.03199927f, 0.1415094f, 1f),
                BiomeColor1 = new Color(0.1603774f, 0.03101638f, 0.03782489f, 1f),
                BiomeColor2 = new Color(0.1716636f, 0.1597989f, 0.254717f, 1f),
                BiomeColor3 = new Color(0.1037291f, 0.1383366f, 0.1981132f, 1f),
                DustColor1 = new Color(0.867f, 0.3440481f, 0.08588208f, 0.1411765f),
                DustColor2 = new Color(0.2964689f, 0.2822179f, 0.3962264f, 0.5254902f),
                DustColor3 = new Color(0.3110638f, 0.4268085f, 0.51f, 1f),
                DustStrength1 = 2f,
                DustStrength2 = 0.5f,
                DustStrength3 = 3,
                BiomeSound1 = 0,
                BiomeSound2 = 2,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.5f
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
            Vegetables2 = new int[] { },
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
                15,
                15,
                2,
                9,
                4,
                2,
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
            RareVeins = new[]
            {
                9,
                10,
                12
            },
            RareSettings = new[]
            {
                0.0f,
                0.2f,
                0.6f,
                0.7f,
                0.0f,
                0.2f,
                0.6f,
                0.7f,
                0.0f,
                0.1f,
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
            Wind = 0.7f,
            IonHeight = 60f,
            WaterHeight = -2.50f,
            WaterItemId = -1,
            Musics = new[]
            {
                10
            },
            SFXPath = "SFX/sfx-amb-lava-1",
            SFXVolume = 0.4f,
            CullingRadius = 0f,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new Color(0.1490196f, 0.0967137f, 0.0967137f, 1f),
                    ["_AmbientColor1"] = new Color(0.09411757f, 0.02745098f, 0.02745098f, 1f),
                    ["_AmbientColor2"] = new Color(0.2470588f, 0.1716928f, 0.1675059f, 1f),
                    ["_Color"] = new Color(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_HeightEmissionColor"] = new Color(1f, 0.4318202f, 0.08962256f, 0f),
                    ["_LightColorScreen"] = new Color(0f, 0f, 0f, 1f),
                    ["_Rotation"] = new Color(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new Color(-0.09377031f, -0.1095983f, -0.989543f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.5f,
                    ["_BioFuzzMask"] = 1f,
                    ["_BioFuzzStrength"] = 0f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 228.5976f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 70f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_HeightEmissionRadius"] = 5f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 1.8f,
                    ["_NormalStrength"] = 1.5f,
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
                    ["_CausticsColor"] = new Color(0.7830188f, 0.4789426f, 0.1514328f, 1f),
                    ["_Color"] = new Color(0.5283019f, 0.06240926f, 0.02741189f, 1f),
                    ["_Color0"] = new Color(1f, 0.6005338f, 0.2039215f, 1f),
                    ["_Color1"] = new Color(0.5943396f, 0.1462626f, 0.05887323f, 1f),
                    ["_Color2"] = new Color(0.2169811f, 0.04338999f, 0.01944642f, 1f),
                    ["_Color3"] = new Color(0.608f, 0.2877792f, 0.102144f, 1f),
                    ["_DensityParams"] = new Color(0.02f, 0.1f, 0f, 0f),
                    ["_DepthColor"] = new Color(0f, 0.06095791f, 0.1132075f, 1f),
                    ["_DepthFactor"] = new Color(0.57f, 0.8f, 0.4f, 0.04f),
                    ["_Foam"] = new Color(15f, 1f, 5f, 1.5f),
                    ["_FoamColor"] = new Color(1f, 0.4062188f, 0.2311321f, 1f),
                    ["_FoamParams"] = new Color(12f, 0.2f, 0.15f, 0.7f),
                    ["_FresnelColor"] = new Color(1f, 0.5575014f, 0f, 1f),
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
                    ["_SpeclColor"] = new Color(1f, 0f, 0f, 1f),
                    ["_SpeclColor1"] = new Color(1f, 0.7611813f, 0f, 1f),
                    ["_Specular"] = new Color(0.9573934f, 0.8672858f, 0.5744361f, 0.9573934f),
                    ["_SunDirection"] = new Color(-0.6f, 0.8f, 0f, 0f),
                    ["_WorldLightDir"] = new Color(-0.6525278f, -0.6042119f, -0.4573132f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_CausticsTiling"] = 0.08f,
                    ["_ChaosDistort"] = 0.45f,
                    ["_ChaosOverlay"] = 0.95f,
                    ["_ChaosTile"] = 1.5f,
                    ["_DistortionStrength"] = 1f,
                    ["_FoamInvThickness"] = 4.5f,
                    ["_FoamSpeed"] = 0.15f,
                    ["_FoamSync"] = 20f,
                    ["_Multiplier"] = 7f,
                    ["_NoiseSpeed"] = 0.25f,
                    ["_NoiseTile"] = 1200f,
                    ["_NormalSpeed"] = 0.06f,
                    ["_NormalStrength"] = 0.2f,
                    ["_NormalTiling"] = 0.16f,
                    ["_PLEdgeAtten"] = 0.5f,
                    ["_PLIntensity2"] = 0f,
                    ["_PLIntensity3"] = 0f,
                    ["_PLRange2"] = 10f,
                    ["_PLRange3"] = 10f,
                    ["_PointLightK"] = 0.01f,
                    ["_PointLightRange"] = 10f,
                    ["_Radius"] = 197.5f,
                    ["_ReflectionBlend"] = 0.86f,
                    ["_ReflectionTint"] = 0f,
                    ["_RefractionAmt"] = 1000f,
                    ["_RefractionStrength"] = 0.3f,
                    ["_RimPower"] = 4f,
                    ["_RotSpeed"] = 0.0005f,
                    ["_SLCosCutoff1"] = 0.3f,
                    ["_SLIntensity1"] = 1f,
                    ["_SLRange1"] = 10f,
                    ["_Shininess"] = 40f,
                    ["_ShoreIntens"] = 1.5f,
                    ["_SpotExp"] = 2f,
                    ["_SpotIntens"] = 5f,
                    ["_Tile"] = 0.05f
                }
            },
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new Color(0.3443396f, 0.734796f, 1f, 1f),
                    ["_Color0"] = new Color(0.517647f, 0.3568627f, 0.4803259f, 1f),
                    ["_Color1"] = new Color(0.6980392f, 0.4488391f, 0.5287111f, 1f),
                    ["_Color2"] = new Color(0.8235294f, 0.4694117f, 0.5056283f, 1f),
                    ["_Color3"] = new Color(1f, 0.5202159f, 0.43f, 1f),
                    ["_Color4"] = new Color(1f, 0.7571103f, 0.4195095f, 1f),
                    ["_Color5"] = new Color(0.8207547f, 0.6521561f, 0.5768512f, 1f),
                    ["_Color6"] = new Color(0.9811321f, 0.4069164f, 0.3100747f, 1f),
                    ["_Color7"] = new Color(0.7547169f, 0.1830218f, 0.08187964f, 1f),
                    ["_Color8"] = new Color(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new Color(0.75f, 0.75f, 0.75f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new Color(196.5235f, 3.715548f, 39.00184f, 0f),
                    ["_PlanetPos"] = new Color(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new Color(200f, 197.48f, 270f, 0f),
                    ["_Sky0"] = new Color(1f, 0.4374347f, 0.3058822f, 0.1607843f),
                    ["_Sky1"] = new Color(0.8301887f, 0.5208257f, 0.5956718f, 0.09803922f),
                    ["_Sky2"] = new Color(1f, 0.5187412f, 0.4103773f, 0.7490196f),
                    ["_Sky3"] = new Color(0.7924528f, 0.5868636f, 0.6128866f, 0.7764706f),
                    ["_Sky4"] = new Color(1f, 0.7566933f, 0.3427217f, 1f)
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
                    ["_FarFogDensity"] = 0.2f,
                    ["_FogDensity"] = 0.5f,
                    ["_FogSaturate"] = 0.9f,
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
                    ["_Color"] = new Color(0.8117647f, 0.3544552f, 0.1411764f, 0.7137255f),
                    ["_ColorBio0"] = new Color(0.3962264f, 0.08584323f, 0.03551085f, 0f),
                    ["_ColorBio1"] = new Color(0.2452829f, 0.0266109f, 0.0266109f, 0f),
                    ["_ColorBio2"] = new Color(0.3882353f, 0.2941177f, 0.2941176f, 0f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new Color(-2.6f, -2f, 1f, 0.1f),
                    ["_RimColor"] = new Color(0.6037736f, 0.1518671f, 0.05980769f, 1f),
                    ["_ShoreLineColor"] = new Color(1f, 0.5998349f, 0.2028302f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 3f,
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
                    ["_ShoreHeight"] = -2.3f,
                    ["_ShoreInvThick"] = 1.2f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_UVSec"] = 0f,
                    ["_WireIntens"] = 0.9f,
                    ["_ZWrite"] = 1f
                }
            },
            thumbMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor"] = new Color(0.09456214f, 0.1019983f, 0.2358491f, 1f),
                    ["_Color"] = new Color(1f, 0.3901146f, 0.117647f, 0.5529412f),
                    ["_ColorBio0"] = new Color(0.235294f, 0.1645803f, 0.1595294f, 0f),
                    ["_ColorBio1"] = new Color(0.3499999f, 0.2344036f, 0.2344036f, 0f),
                    ["_ColorBio2"] = new Color(0.3779999f, 0.3225137f, 0.3641284f, 0.01960784f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new Color(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new Color(-3f, 0f, 1f, 0f),
                    ["_HoloColor"] = new Color(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new Color(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new Color(0.5566037f, 0.4371345f, 0.4122018f, 1f),
                    ["_Rotation"] = new Color(0.2484018f, 0.8877195f, 0.2692202f, -0.2788749f),
                    ["_ShoreLineColor"] = new Color(0.5568628f, 0.407498f, 0.2494745f, 1f),
                    ["_SpecularColor"] = new Color(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new Color(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new Color(0.2099585f, -0.006163432f, -0.9776909f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 2f,
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
                    ["_Multiplier"] = 0.8f,
                    ["_NoiseIntensity"] = 0.15f,
                    ["_NoiseThres"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_PolarWhirl"] = -0.3f,
                    ["_PolarWhirlPower"] = 8f,
                    ["_ShoreHeight"] = -1.5f,
                    ["_ShoreInvThick"] = 0.5f,
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