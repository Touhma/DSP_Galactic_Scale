using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme IceGelisol3 = new()
        {
            Name = "IceGelisol3",
            Base = true,
            DisplayName = "Ice Field Gelisol III".Translate(),
            PlanetType = EPlanetType.Ice,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 10,
            Algo = 3,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Ice 1/",
            Temperature = -5.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(0.35f, -0.15f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.0224f, 0.08848375f, 0.2f, 1f),
                Color2 = new Color(0.05354035f, 0.04313723f, 0.0941176f, 1f),
                Color3 = new Color(0.019208f, 0.03367789f, 0.09799997f, 1f),
                WaterColor1 = new Color(0.02352941f, 0.04313726f, 0.1882353f, 1f),
                WaterColor2 = new Color(0.05490196f, 0.04313726f, 0.09411766f, 1f),
                WaterColor3 = new Color(0.01960784f, 0.03529412f, 0.09803922f, 1f),
                BiomeColor1 = new Color(0.4433962f, 0.1096583f, 0.0271894f, 1f),
                BiomeColor2 = new Color(1f, 1f, 1f, 1f),
                BiomeColor3 = new Color(1f, 1f, 1f, 1f),
                DustColor1 = new Color(0.6792453f, 0.4328387f, 0.3364187f, 1f),
                DustColor2 = new Color(0.9803922f, 0.9265213f, 0.9215686f, 1f),
                DustColor3 = new Color(1f, 1f, 1f, 1f),
                DustStrength1 = 4f,
                DustStrength2 = 8f,
                DustStrength3 = 8f,
                BiomeSound1 = 1,
                BiomeSound2 = 0,
                BiomeSound3 = 0,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 1f
            },

            Vegetables0 = new[]
            {
                669,
                670,
                671
            },
            Vegetables1 = new[]
            {
                665,
                666,
                667,
                668,
                665,
                666,
                667,
                668,
                664,
                663,
                662,
                661
            },
            Vegetables2 = new[]
            {
                1071,
                1072
            },
            Vegetables3 = new[]
            {
                1073,
                1074,
                1071,
                1072
            },
            Vegetables4 = new int[]
            {
            },
            Vegetables5 = new int[]
            {
            },
            VeinSpot = new[]
            {
                5,
                1,
                3,
                10,
                2,
                1,
                0
            },
            VeinCount = new[]
            {
                0.6f,
                0.2f,
                0.8f,
                1.0f,
                0.8f,
                0.2f,
                0.0f
            },
            VeinOpacity = new[]
            {
                1.0f,
                0.5f,
                1.0f,
                1.0f,
                1.0f,
                0.3f,
                0.0f
            },
            RareVeins = new[]
            {
                8,
                10,
                12
            },
            RareSettings = new[]
            {
                0.3f,
                1.0f,
                0.8f,
                1.0f,
                0.0f,
                0.2f,
                0.6f,
                0.4f,
                0.0f,
                0.1f,
                0.2f,
                0.4f
            },
            GasItems = new int[]
            {
            },
            GasSpeeds = new float[]
            {
            },
            UseHeightForBuild = false,
            Wind = 0.7f,
            IonHeight = 55f,
            WaterHeight = 0f,
            WaterItemId = 1000,
            Musics = new[]
            {
                27,
                4
            },
            SFXPath = "SFX/sfx-amb-desert-2",
            SFXVolume = 0.3f,
            CullingRadius = 0f,
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(200f, 100f, 255f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 1f,
                    ["_BumpScale"] = 1f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DistanceControl"] = 0f,
                    ["_DstBlend"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_GlossyReflections"] = 1f,
                    ["_Intensity"] = 1f,
                    ["_IntensityControl"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_RimFogPower"] = 3f,
                    ["_SkyAtmosPower"] = 7f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_StencilComp"] = 8f,
                    ["_StencilRef"] = 0f,
                    ["_SunColorAdd"] = 5f,
                    ["_SunRiseScatterPower"] = 60f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f
                }
            },
            minimapMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BumpScale"] = 1f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_GlossyReflections"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_ShoreHeight"] = -4f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_UVSec"] = 0f,
                    ["_WireIntens"] = 2f,
                    ["_ZWrite"] = 1f
                }
            },
            oceanMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_BumpDirection"] = new(1f, 1f, -1f, 1f),
                    ["_BumpTiling"] = new(1f, 1f, -2f, 3f),
                    ["_FoamColor"] = new(0f, 0f, 0f, 1f),
                    ["_PLColor1"] = new(0f, 0f, 0f, 1f),
                    ["_PLColor2"] = new(0f, 0f, 0f, 1f),
                    ["_PLColor3"] = new(1f, 1f, 1f, 1f),
                    ["_PLParam1"] = new(0f, 0f, 0f, 0f),
                    ["_PLParam2"] = new(0f, 0f, 0f, 0f),
                    ["_PLPos1"] = new(0f, 0f, 0f, 0f),
                    ["_PLPos2"] = new(0f, 0f, 0f, 0f),
                    ["_SLColor1"] = new(1f, 1f, 1f, 1f),
                    ["_SpecColor"] = new(1f, 1f, 1f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_DistortionStrength"] = 1f,
                    ["_FoamInvThickness"] = 2f,
                    ["_FoamSync"] = 5f,
                    ["_PLIntensity2"] = 0f,
                    ["_PLIntensity3"] = 0f,
                    ["_PLRange2"] = 10f,
                    ["_PLRange3"] = 10f,
                    ["_PointLightRange"] = 10f,
                    ["_Radius"] = 200f,
                    ["_ReflectionTint"] = 0f,
                    ["_RefractionAmt"] = 1000f,
                    ["_SLIntensity1"] = 1f,
                    ["_SLRange1"] = 10f,
                    ["_Shininess"] = 40f,
                    ["_SpotExp"] = 2f
                }
            },
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_GICloudColor"] = new(1f, 1f, 1f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioFuzzMask"] = 1f,
                    ["_BumpScale"] = 1f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 0f,
                    ["_GICloudStrength"] = 0f,
                    ["_GIStrengthNight"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_GlossyReflections"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
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
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new(0f, 0f, 1f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BumpScale"] = 1f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarHeight"] = 1f,
                    ["_GlossMapScale"] = 1f,
                    ["_GlossyReflections"] = 1f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 4f,
                    ["_NoiseThres"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_PolarWhirlPower"] = 8f,
                    ["_ShoreHeight"] = 0f,
                    ["_ShoreInvThick"] = 6f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_Speed"] = 2f,
                    ["_SrcBlend"] = 1f,
                    ["_TileX"] = 7f,
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