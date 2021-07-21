using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme Mediterranean = new GSTheme
        {
            Name = "Mediterranean",
            Base = true,
            DisplayName = "Mediterranean".Translate(),
            PlanetType = EPlanetType.Ocean,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 1,
            Algo = 1,
            MinRadius = 30,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Ocean 1/",
            Temperature = 0.0f,
            Distribute = EThemeDistribute.Birth,
            Habitable = true,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(0.0f, 0.0f),
            CustomGeneration = true,
            TerrainSettings = new GSTerrainSettings
            {
                Algorithm = "GSTA1"
            },
            VeinSettings = new GSVeinSettings
            {
                Algorithm = "GS2",
                VeinTypes = new GSVeinTypes()
            },
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.1098039f, 0.1415094f, 0.1333333f, 1),
                Color2 = new Color(0.06666667f, 0.03519787f, 0.03137255f, 1),
                Color3 = new Color(0.03921569f, 0.03921569f, 0.1764706f, 1),
                WaterColor1 = new Color(0f, 0f, 0f, 1f),
                WaterColor2 = new Color(0.06047527f, 0.07024744f, 0.08490568f, 1f),
                WaterColor3 = new Color(0.0260324f, 0.03199927f, 0.1415094f, 1f),
                BiomeColor1 = new Color(0.8745098f, 0.6923354f, 0.3960784f, 1f),
                BiomeColor2 = new Color(0.3002473f, 0.399f, 0.09775501f, 1f),
                BiomeColor3 = new Color(0.3002473f, 0.399f, 0.09775501f, 1f),
                DustColor1 = new Color(1, 0.8645418f, 0.6812749f, 1f),
                DustColor2 = new Color(0.5177712f, 0.6039216f, 0.2705569f, 0.6156863f),
                DustColor3 = new Color(0.6120654f, 0.6235294f, 0.1686274f, 0.6509804f),
                DustStrength1 = 4f,
                DustStrength2 = 0.5f,
                DustStrength3 = 0.5f,
                BiomeSound1 = 0,
                BiomeSound2 = 1,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.25f
            },
            VegeSettings = new GSVegeSettings
            {
                Algorithm = "GS2",
                Group1 = new List<string>
                {
                    // Highlands
                    "MedStone4",
                    "MedStone5",
                    "MedStone3",
                    "MedStone4",
                    "MedTree2",
                    "MedStone4",
                    "MedStone5",
                    "MedTree5",
                    "MedStone2",
                    "MedStone1"
                },
                Group2 = new List<string>
                {
                    "MedTree3",
                    "MedTree2",
                    "MedTree3",
                    "MedTree4",
                    "MedTree4",
                    "MedTree4",
                    "MedTree1",
                    "MedTree4",
                    "MedStone4",
                    "MedTree6"
                },
                Group3 = new List<string>
                {
                    "MedGrass7",
                    "MedGrass8",
                    "MedGrass9"
                },
                Group4 = new List<string>
                {
                    "MedFragment6",
                    "MedFragment7",
                    "MedFragment8",
                    "MedFragment7",
                    "MedFragment8" //This is a broken rock
                },
                Group5 = new List<string>
                {
                    // Lowlands - On Sand
                    "MedGrass10"
                },
                Group6 = new List<string>() // In Water
            },
            //Vegetables0 = new int[] {
            //    604,
            //    605,
            //    603,
            //    604,
            //    102,
            //    604,
            //    605,
            //    105,
            //    602,
            //    601
            //},
            //Vegetables1 = new int[] {
            //    103,
            //    102,
            //    103,
            //    104,
            //    104,
            //    104,
            //    101,
            //    104,
            //    604,
            //    106
            //},
            //Vegetables2 = new int[] {
            //    1001,
            //    1002,
            //    1003
            //},
            //Vegetables3 = new int[] {
            //    1005,
            //    1006,
            //    1007,
            //    1006,
            //    1007
            //},
            //Vegetables4 = new int[] {
            //    1004
            //},
            //Vegetables5 = new int[] {
            //},
            VeinSpot = new[]
            {
                7,
                5,
                0,
                0,
                8,
                11,
                18
            },
            VeinCount = new[]
            {
                0.7f,
                0.6f,
                0.0f,
                0.0f,
                1.0f,
                1.0f,
                1.0f
            },
            VeinOpacity = new[]
            {
                0.6f,
                0.5f,
                0.0f,
                0.0f,
                0.7f,
                1.0f,
                1.0f
            },
            RareVeins = new[]
            {
                11
            },
            RareSettings = new[]
            {
                0.0f,
                1.0f,
                0.3f,
                0.3f
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
                    ["_CausticsColor"] = new Color(0.851451f, 0.9019608f, 0.8009412f, 1f),
                    ["_Color"] = new Color(1f, 0.9644412f, 0.7688679f, 1f),
                    ["_Color0"] = new Color(0f, 0.1574037f, 0.2352941f, 1f),
                    ["_Color1"] = new Color(0.5756497f, 0.8301887f, 0.6801699f, 1f),
                    ["_Color2"] = new Color(0.4372706f, 0.7921569f, 0.7276321f, 1f),
                    ["_Color3"] = new Color(0.2980392f, 0.5845041f, 0.7450981f, 1f),
                    ["_DensityParams"] = new Color(0.02f, 0.1f, 0f, 0f),
                    ["_DepthColor"] = new Color(0f, 0.06095791f, 0.1132075f, 1f),
                    ["_DepthFactor"] = new Color(0.54f, 0.8f, 0.4f, 0.04f),
                    ["_Foam"] = new Color(15f, 1f, 5f, 1.5f),
                    ["_FoamColor"] = new Color(1f, 1f, 1f, 1f),
                    ["_FoamParams"] = new Color(12f, 0.2f, 0.15f, 0.7f),
                    ["_FresnelColor"] = new Color(0.2049662f, 0.6415741f, 0.8867924f, 1f),
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
                    ["_SpeclColor1"] = new Color(0.8962264f, 0.614061f, 0.1733268f, 1f),
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
                    ["_Color0"] = new Color(0.3899999f, 0.488919f, 1f, 1f),
                    ["_Color1"] = new Color(0f, 0.7073908f, 1f, 1f),
                    ["_Color2"] = new Color(0.2117646f, 0.8043795f, 0.9607843f, 1f),
                    ["_Color3"] = new Color(0.5727176f, 0.9294221f, 0.9529411f, 1f),
                    ["_Color4"] = new Color(1f, 0.7391673f, 0.413056f, 1f),
                    ["_Color5"] = new Color(0.240566f, 0.5836905f, 1f, 1f),
                    ["_Color6"] = new Color(0.6941177f, 0.3529412f, 1f, 1f),
                    ["_Color7"] = new Color(0.2074581f, 0.3139569f, 0.6981132f, 1f),
                    ["_Color8"] = new Color(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new Color(0.4327686f, 0.5402345f, 0.7372549f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new Color(-76.93655f, -113.229f, 165.6604f, 0f),
                    ["_PlanetPos"] = new Color(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new Color(200f, 199.98f, 270f, 0f),
                    ["_Sky0"] = new Color(0.4198112f, 0.6650285f, 1f, 0.1607843f),
                    ["_Sky1"] = new Color(0.485849f, 0.5557498f, 1f, 0.09803922f),
                    ["_Sky2"] = new Color(0.78f, 0.9567857f, 1f, 0.7490196f),
                    ["_Sky3"] = new Color(0.3450979f, 0.7052943f, 1f, 0.5411765f),
                    ["_Sky4"] = new Color(1f, 0.7298433f, 0.3081232f, 1f)
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
                    ["_FarFogDensity"] = 0.09f,
                    ["_FogDensity"] = 0.9f,
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