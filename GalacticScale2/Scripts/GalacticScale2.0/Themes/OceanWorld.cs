using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        //Note: Culling Radius is -10
        public static GSTheme OceanWorld = new GSTheme
        {
            Name = "OceanWorld",
            Base = true,
            DisplayName = "Ocean World".Translate(),
            PlanetType = EPlanetType.Ocean,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 16,
            Algo = 7,
            MinRadius = 50,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Ocean 5/",
            Temperature = 0.0f,
            Distribute = EThemeDistribute.Interstellar,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(1.0f, 1.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.1098039f, 0.1415094f, 0.1333333f, 1f),
                Color2 = new Color(0.03137255f, 0.05890403f, 0.06666667f, 1f),
                Color3 = new Color(0.03921569f, 0.03921569f, 0.1764706f, 1f),
                WaterColor1 = new Color(0f, 0f, 0f, 1f),
                WaterColor2 = new Color(0.1354839f, 0.1806452f, 0.2f, 1f),
                WaterColor3 = new Color(0.03888888f, 0.1444444f, 0.2f, 1f),
                BiomeColor1 = new Color(0.8745098f, 0.6923354f, 0.3960784f, 1f),
                BiomeColor2 = new Color(0.8745098f, 0.6941177f, 0.3960784f, 1f),
                BiomeColor3 = new Color(0.8745098f, 0.6941177f, 0.3960784f, 1f),
                DustColor1 = new Color(1, 0.8645418f, 0.6812749f, 1),
                DustColor2 = new Color(1, 0.8627452f, 0.682353f, 1),
                DustColor3 = new Color(1, 0.8627452f, 0.682353f, 1),
                DustStrength1 = 4,
                DustStrength2 = 4,
                DustStrength3 = 4,
                BiomeSound1 = 0,
                BiomeSound2 = 0,
                BiomeSound3 = 0,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.25f
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
                0,
                0,
                0,
                0,
                0,
                2,
                10
            },
            VeinCount = new[]
            {
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                0.5f,
                5.0f
            },
            VeinOpacity = new[]
            {
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                0.8f,
                2.0f
            },
            RareVeins = new[]
            {
                13
            },
            RareSettings = new[]
            {
                1.0f,
                1.0f,
                1.0f,
                0.6f
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
            WaterHeight = 0f,
            WaterItemId = 1000,
            Musics = new[]
            {
                9
            },
            SFXPath = "SFX/sfx-amb-ocean-1",
            SFXVolume = 0.35f,
            CullingRadius = -10f,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new Color(0.1098038f, 0.1415093f, 0.1333332f, 1f),
                    ["_AmbientColor1"] = new Color(0.06666655f, 0.03519787f, 0.03137255f, 1f),
                    ["_AmbientColor2"] = new Color(0.03921569f, 0.03921569f, 0.1764704f, 1f),
                    ["_Color"] = new Color(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_HeightEmissionColor"] = new Color(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new Color(0f, 0f, 0f, 1f),
                    ["_Rotation"] = new Color(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new Color(0.3786571f, 0.01833941f, 0.9253553f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.9f,
                    ["_BioFuzzMask"] = 1f,
                    ["_BioFuzzStrength"] = 0.1f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 314.7685f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_HeightEmissionRadius"] = 50f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 1.6f,
                    ["_NormalStrength"] = 1f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_Radius"] = 200f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_StepBlend"] = 0.55f,
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
                    ["_CausticsColor"] = new Color(0.5506853f, 0.7075471f, 0.6751246f, 1f),
                    ["_Color"] = new Color(1f, 0.9647059f, 0.7686275f, 1f),
                    ["_Color0"] = new Color(0f, 0.1574037f, 0.2352941f, 1f),
                    ["_Color1"] = new Color(0.5764706f, 0.8313726f, 0.7921569f, 1f),
                    ["_Color2"] = new Color(0.3551085f, 0.6892567f, 0.7169812f, 1f),
                    ["_Color3"] = new Color(0.2731398f, 0.504477f, 0.5849056f, 1f),
                    ["_DensityParams"] = new Color(0.02f, 0.1f, 0f, 0f),
                    ["_DepthColor"] = new Color(0f, 0.06095791f, 0.1132075f, 1f),
                    ["_DepthFactor"] = new Color(0.4f, 0.35f, 0.15f, 0.1f),
                    ["_Foam"] = new Color(15f, 1f, 5f, 1.5f),
                    ["_FoamColor"] = new Color(1f, 1f, 1f, 1f),
                    ["_FoamParams"] = new Color(12f, 0.2f, 0.15f, 0.7f),
                    ["_FresnelColor"] = new Color(0.2470588f, 0.6588235f, 0.8862746f, 1f),
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
                    ["_SpeclColor1"] = new Color(0.8962264f, 0.614061f, 0.1733267f, 1f),
                    ["_Specular"] = new Color(0.9573934f, 0.8672858f, 0.5744361f, 0.9573934f),
                    ["_SunDirection"] = new Color(-0.6f, 0.8f, 0f, 0f),
                    ["_WorldLightDir"] = new Color(-0.6525278f, -0.6042119f, -0.4573132f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_CausticsTiling"] = 0.03f,
                    ["_DistortionStrength"] = 1f,
                    ["_FoamInvThickness"] = 6f,
                    ["_FoamSpeed"] = 0.15f,
                    ["_FoamSync"] = 4f,
                    ["_GIGloss"] = 0.6f,
                    ["_GISaturate"] = 1f,
                    ["_GIStrengthDay"] = 1f,
                    ["_GIStrengthNight"] = 0.03f,
                    ["_NormalSpeed"] = 0.6f,
                    ["_NormalStrength"] = 0.4f,
                    ["_NormalTiling"] = 0.06f,
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
                    ["_ShoreIntens"] = 1.4f,
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
                    ["_Color0"] = new Color(0.3882353f, 0.7017767f, 1f, 1f),
                    ["_Color1"] = new Color(0.28854f, 0.7906604f, 0.916f, 1f),
                    ["_Color2"] = new Color(0.4466639f, 0.84536f, 0.888f, 1f),
                    ["_Color3"] = new Color(0.7357877f, 0.923f, 0.9083642f, 1f),
                    ["_Color4"] = new Color(1f, 0.7438396f, 0.4147364f, 1f),
                    ["_Color5"] = new Color(0.2392156f, 0.8758385f, 1f, 1f),
                    ["_Color6"] = new Color(0.3529411f, 0.7509555f, 1f, 1f),
                    ["_Color7"] = new Color(0.2078431f, 0.4966022f, 0.6980392f, 1f),
                    ["_Color8"] = new Color(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new Color(0.4669811f, 0.9485019f, 1f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new Color(78.1805f, 142.5101f, 140.7908f, 0f),
                    ["_PlanetPos"] = new Color(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new Color(200f, 199.98f, 270f, 0f),
                    ["_Sky0"] = new Color(0.4198112f, 0.6650285f, 1f, 0.1607843f),
                    ["_Sky1"] = new Color(0.485849f, 0.5557498f, 1f, 0.09803922f),
                    ["_Sky2"] = new Color(0.839f, 1f, 0.9984235f, 0.9176471f),
                    ["_Sky3"] = new Color(0.2666666f, 0.7909663f, 1f, 0.6705883f),
                    ["_Sky4"] = new Color(1f, 0.7368349f, 0.3171324f, 1f)
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
                    ["_FarFogDensity"] = 0.03f,
                    ["_FogDensity"] = 0.3f,
                    ["_FogSaturate"] = 1.5f,
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
                    ["_RimFogExp"] = 1.35f,
                    ["_RimFogPower"] = 3.2f,
                    ["_SkyAtmosPower"] = 7f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_SunColorAdd"] = 0f,
                    ["_SunColorSkyUse"] = 0.2f,
                    ["_SunColorUse"] = 0.6f,
                    ["_SunRiseScatterPower"] = 60f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f
                }
            },
            minimapMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new Color(0.3632075f, 0.7256894f, 1f, 1f),
                    ["_ColorBio0"] = new Color(0f, 0f, 0f, 0f),
                    ["_ColorBio1"] = new Color(0f, 0f, 0f, 0f),
                    ["_ColorBio2"] = new Color(0f, 0f, 0f, 0f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new Color(-1f, 0f, 0.1f, 0.3f),
                    ["_RimColor"] = new Color(0.7001489f, 1f, 0.5330188f, 1f),
                    ["_ShoreLineColor"] = new Color(0.2672659f, 0.3639862f, 0.3962263f, 1f)
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
                    ["_Color"] = new Color(0.3176471f, 0.9740799f, 1f, 1f),
                    ["_ColorBio0"] = new Color(0f, 0f, 0f, 0f),
                    ["_ColorBio1"] = new Color(0f, 0f, 0f, 0f),
                    ["_ColorBio2"] = new Color(0f, 0f, 0f, 0f),
                    ["_HeightSettings"] = new Color(-1f, 0f, 0.1f, 0.3f),
                    ["_RimColor"] = new Color(0.7001489f, 1f, 0.5330188f, 1f),
                    ["_Rotation"] = new Color(0f, 0f, 0f, 1f),
                    ["_ShoreLineColor"] = new Color(0.2672658f, 0.3639862f, 0.3962263f, 1f),
                    ["_SunDir"] = new Color(0f, 0f, 1f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0f,
                    ["_Diameter"] = 0.1f,
                    ["_FarHeight"] = 0.5f,
                    ["_ShoreHeight"] = 0f,
                    ["_ShoreInvThick"] = 6f,
                    ["_WireIntens"] = 2f
                }
            }
        };
    }
}